# Developer Guide: Adding a New Dialog

This guide explains how to add a new dialog using the strongly-typed
orchestrator design with a generic Radzen handler.

# Architecture Overview

ViewModel ↓ DialogOrchestrator ↓ IDialogHandler\<TRequest, TResult\> ↓
(generic RadzenDialogHandler) DialogService.OpenTypedAsync ↓ Blazor
Dialog Component

The Orchestrator remains UI-agnostic.

------------------------------------------------------------------------

# Step 1 --- Create the Dialog Request

``` csharp
public sealed class DeleteOrderDialogRequest : IDialogRequest<bool>
{
    public string Title { get; }
    public string OrderNumber { get; }

    public DialogPolicy Policy { get; } = new()
    {
        IsModal = true,
        EnforceExclusiveModal = true,
        QueueMode = DialogQueueMode.Immediate,
        Priority = 10,
        SingleInstanceKey = "DeleteOrder"
    };

    public DeleteOrderDialogRequest(string orderNumber)
    {
        Title = "Delete Order";
        OrderNumber = orderNumber;
    }
}
```

------------------------------------------------------------------------

# Step 2 --- Create the Dialog Component

``` razor
@implements IDialogComponent<DeleteOrderDialogRequest, bool>
@inject DialogService DialogService

<RadzenDialog Style="width:400px">
    <ChildContent>
        <p>Are you sure you want to delete order @Request.OrderNumber?</p>

        <RadzenButton Text="Delete"
                      Click="@(() => Close(true))"
                      Style="margin-right:8px" />

        <RadzenButton Text="Cancel"
                      ButtonStyle="ButtonStyle.Secondary"
                      Click="@(() => Close(false))" />
    </ChildContent>
</RadzenDialog>

@code {
    [Parameter]
    public DeleteOrderDialogRequest Request { get; set; } = default!;

    public void Close(bool result)
    {
        DialogService.Close(result);
    }
}
```

------------------------------------------------------------------------

# Step 3 --- Generic Radzen Dialog Handler (Created Once)

``` csharp
public sealed class RadzenDialogHandler<TComponent, TRequest, TResult>
    : IDialogHandler<TRequest, TResult>
    where TComponent : ComponentBase, IDialogComponent<TRequest, TResult>
    where TRequest : IDialogRequest<TResult>
{
    private readonly DialogService _dialogService;

    public RadzenDialogHandler(DialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public Task<TResult> HandleAsync(
        TRequest request,
        CancellationToken ct)
    {
        return _dialogService.OpenTypedAsync<
            TComponent,
            TRequest,
            TResult>(request);
    }
}
```

------------------------------------------------------------------------

# Step 4 --- DI Registration Helper

``` csharp
public static class DialogRegistrationExtensions
{
    public static IServiceCollection AddDialog<TComponent, TRequest, TResult>(
        this IServiceCollection services)
        where TComponent : ComponentBase, IDialogComponent<TRequest, TResult>
        where TRequest : IDialogRequest<TResult>
    {
        services.AddScoped<
            IDialogHandler<TRequest, TResult>,
            RadzenDialogHandler<TComponent, TRequest, TResult>>();

        return services;
    }
}
```

Register your dialog:

``` csharp
builder.Services.AddDialog<
    DeleteOrderDialog,
    DeleteOrderDialogRequest,
    bool>();
```

------------------------------------------------------------------------

# Step 5 --- Call from ViewModel

``` csharp
var confirmed = await _dialogOrchestrator.RequestAsync<
    DeleteOrderDialogRequest,
    bool>(
    new DeleteOrderDialogRequest(orderNumber),
    defaultResult: false);

if (confirmed)
{
    await DeleteOrderFromDatabase();
}
```

------------------------------------------------------------------------

# OpenTypedAsync Extension

``` csharp
public static class DialogServiceExtensions
{
    public static async Task<TResult?> OpenTypedAsync<TComponent, TRequest, TResult>(
        this DialogService dialogService,
        TRequest request,
        TResult defaultResult = default!)
        where TComponent : ComponentBase, IDialogComponent<TRequest, TResult>
        where TRequest : IDialogRequest<TResult>
    {
        var parameters = new Dictionary<string, object>
        {
            { nameof(IDialogComponent<,>.Request), request }
        };

        var result = await dialogService.OpenAsync<TComponent>(
            request.Title,
            parameters);

        return result is TResult typedResult ? typedResult : defaultResult;
    }
}
```

------------------------------------------------------------------------
