// namespace MyApp.Application.Abstractions;

// using System.Reactive.Linq;

// public sealed class AsyncStateStore<T> : IDisposable
// {
//     private readonly ReactiveState<AsyncState<T>> _state =
//         new ReactiveState<AsyncState<T>>(AsyncState<T>.IdleState);

//     public AsyncState<T> Value => _state.Value;

//     public IObservable<AsyncState<T>> Changes => _state.Changes;

//     public bool IsLoading =>
//         _state.Value is AsyncState<T>.Loading;

//     public bool HasValue =>
//         _state.Value is AsyncState<T>.Success;

//     public T? CurrentValue =>
//         HasValue ? (_state.Value as AsyncState<T>.Success)!.Value : default(T);

//     public void SetIdle() =>
//         _state.Set(AsyncState<T>.IdleState);

//     public void SetLoading() =>
//         _state.Set(AsyncState<T>.LoadingState);

//     public void SetSuccess(T value) =>
//         _state.Set(new AsyncState<T>.Success(value));

//     public void SetError(Exception ex) =>
//         _state.Set(new AsyncState<T>.Error(ex.Message, ex));

//     public void Dispose() => _state.Dispose();
// }

// public static class AsyncStateExtensions
// {
//     public static IObservable<T> TrackAsync<T>(
//         this IObservable<T> source,
//         AsyncStateStore<T> state)
//     {
//         return Observable.Defer(() =>
//         {
//             state.SetLoading();

//             return source
//                 .Do(
//                     onNext: value => state.SetSuccess(value),
//                     onError: ex => state.SetError(ex)
//                 );
//         });
//     }
// }
