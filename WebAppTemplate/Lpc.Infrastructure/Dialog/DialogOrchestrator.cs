namespace Lpc.Infrastructure.Services;

using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Lpc.Application.Abstractions;

public sealed class DialogOrchestrator : IDialogOrchestrator, IDisposable
{
    private interface IPriorityDialog
    {
        int Priority { get; }
        void Cancel();
    }

    private interface IModalDialog
    {
        bool IsModal { get; }
    }

    private abstract class ActiveDialog(int priority, bool isModal) : IPriorityDialog, IModalDialog
    {
        public int Priority { get; } = priority;
        public bool IsModal { get; } = isModal;

        public abstract Task AsTask();
        public abstract void Cancel();
    }

    private sealed class QueuedDialog(Func<Task> execute)
    {
        public Func<Task> Execute { get; } = execute;
    }

    private sealed class ActiveDialog<TResult>(
        Task<TResult?> task,
        int priority,
        bool isModal,
        CancellationTokenSource cts) : ActiveDialog(priority, isModal)
    {
        public Task<TResult?> Task { get; } = task;
        private readonly CancellationTokenSource _cts = cts;

        public override Task AsTask() => Task;

        public override void Cancel() => _cts.Cancel();
    }

    private readonly IServiceProvider _provider;
    private readonly INavigationDialogCancellation? _navigationCancellation;

    private readonly object _queueLock = new();
    private readonly PriorityQueue<QueuedDialog, int> _queue = new();
    private bool _isProcessingQueue = false;

    private readonly ConcurrentDictionary<string, ActiveDialog> _activeDialogs = new();

    public DialogOrchestrator(IServiceProvider provider, INavigationDialogCancellation? navigationCancellation)
    {
        _provider = provider;
        _navigationCancellation = navigationCancellation;

        if (_navigationCancellation != null)
        {
            _navigationCancellation.NavigationTriggered += CancelAllDialogs;
            _navigationCancellation.Start();
        }
    }

    public async Task<TResult?> RequestAsync<TRequest, TResult>(
        TRequest request,
        TResult? defaultResult = default,
        CancellationToken ct = default)
        where TRequest : IDialogRequest<TResult>
    {
        if (request.Policy.QueueMode == DialogQueueMode.Immediate)
        {
            return await ExecuteWithGuards(request, defaultResult, ct);
        }

        if (request.Policy.QueueMode == DialogQueueMode.ReplaceLower)
        {
            CancelLowerPriorityDialogs(request.Policy.Priority);
        }

        return await Enqueue(request, defaultResult, ct);
    }

    private async Task<TResult?> ExecuteHandler<TRequest, TResult>(
        TRequest request,
        CancellationToken ct)
        where TRequest : IDialogRequest<TResult>
    {
        var handler = _provider
            .GetRequiredService<IDialogHandler<TRequest, TResult>>();

        var middlewares = _provider
            .GetServices<IDialogMiddleware<TRequest, TResult>>()
            .Reverse()
            .ToList();

        DialogHandlerDelegate<TResult?> pipeline =
            () => handler.HandleAsync(request, ct);

        foreach (var middleware in middlewares)
        {
            var next = pipeline;
            pipeline = () => middleware.HandleAsync(request, next, ct);
        }

        return await pipeline();
    }

    private async Task<TResult?> ExecuteWithGuards<TRequest, TResult>(
        TRequest request,
        TResult? defaultResult = default,
        CancellationToken token = default)
        where TRequest : IDialogRequest<TResult>
    {
        var key = request.Policy.SingleInstanceKey;

        if (key != null && _activeDialogs.TryGetValue(key, out var existing))
        {
            switch (request.Policy.InstanceBehavior)
            {
                case SingleInstanceBehavior.IgnoreIfExists:
                    if (existing is ActiveDialog<TResult> typed)
                    {
                        try
                        {
                            return await typed.Task;
                        }
                        catch (OperationCanceledException)
                        {
                            return defaultResult;
                        }
                    }

                    throw new InvalidOperationException(
                        $"Dialog key '{key}' is already used with different result type.");

                case SingleInstanceBehavior.Throw:
                    throw new InvalidOperationException(
                        $"Dialog with key '{key}' is already open.");

                case SingleInstanceBehavior.ReplaceExisting:
                    existing.Cancel();
                    _activeDialogs.TryRemove(key, out _);
                    break;
            }
        }

        if (request.Policy.IsModal && request.Policy.EnforceExclusiveModal)
        {
            CancelAllModalDialogs();
        }

        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        var handlerTask = ExecuteHandler<TRequest, TResult>(request, cts.Token);

        var activeDialog = new ActiveDialog<TResult>(handlerTask, request.Policy.Priority, request.Policy.IsModal, cts);
        if (key != null)
            _activeDialogs[key] = activeDialog;

        try
        {
            var result = await handlerTask;
            return result is TResult typedResult ? typedResult : defaultResult;
        }
        catch (OperationCanceledException)
        {
            return defaultResult;
        }
        finally
        {
            if (key != null)
            {
                _activeDialogs.TryRemove(key, out _);
            }
            cts.Dispose();
        }
    }

    private void CancelAllModalDialogs()
    {
        var modalDialogs = _activeDialogs
            .Where(kv => kv.Value.IsModal)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var key in modalDialogs)
        {
            if (_activeDialogs.TryRemove(key, out var dialog))
            {
                dialog.Cancel();
            }
        }
    }

    private void CancelAllDialogs()
    {
        foreach (var dialog in _activeDialogs.Values)
            dialog.Cancel();

        _activeDialogs.Clear();

        lock (_queueLock)
        {
            _queue.Clear();
        }
    }

    public void Dispose()
    {
        if (_navigationCancellation != null)
        {
            _navigationCancellation.NavigationTriggered -= CancelAllDialogs;
            _navigationCancellation.Stop();
        }

        CancelAllDialogs();
    }

    private Task<TResult?> Enqueue<TRequest, TResult>(
        TRequest request,
        TResult? defaultResult = default,
        CancellationToken token = default)
        where TRequest : IDialogRequest<TResult>
    {
        var tcs = new TaskCompletionSource<TResult?>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var queued = new QueuedDialog(async () =>
        {
            try
            {
                var result = await ExecuteWithGuards(request, defaultResult, token);
                tcs.SetResult(result);
            }
            catch (OperationCanceledException oce)
            {
                tcs.SetCanceled(oce.CancellationToken);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        lock (_queueLock)
        {
            _queue.Enqueue(queued, -request.Policy.Priority);
        }

        _ = ProcessQueueAsync();

        return tcs.Task;
    }

    private async Task ProcessQueueAsync()
    {
        lock (_queueLock)
        {
            if (_isProcessingQueue)
                return;

            _isProcessingQueue = true;
        }

        try
        {
            while (true)
            {
                QueuedDialog? next;

                lock (_queueLock)
                {
                    if (!_queue.TryDequeue(out next, out _))
                    {
                        _isProcessingQueue = false;
                        return;
                    }
                }

                await next.Execute();
            }
        }
        finally
        {
            lock (_queueLock)
            {
                _isProcessingQueue = false;
            }
        }
    }

    private void CancelLowerPriorityDialogs(int priority)
    {
        var toCancel = _activeDialogs
            .Where(kv => kv.Value.Priority < priority)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var key in toCancel)
        {
            if (_activeDialogs.TryRemove(key, out var dialog))
            {
                dialog.Cancel();
            }
        }
    }
}
