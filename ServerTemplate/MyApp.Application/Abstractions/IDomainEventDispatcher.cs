namespace MyApp.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<object> events, CancellationToken ct = default);
}

// public class DomainEventDispatcher : IDomainEventDispatcher
// {
//     private readonly IEventBus _bus;
//     public DomainEventDispatcher(IEventBus bus) => _bus = bus;

//     public async Task DispatchAsync(IEnumerable<object> events, CancellationToken ct = default)
//     {
//         foreach (var evt in events)
//         {
//             // publish to message bus or notify local handlers
//             await _bus.PublishAsync(evt, ct);
//         }
//     }
// }

// public class ApproveChangeHandler
// {
//     private readonly IDomainEventDispatcher _dispatcher;
//     public ApproveChangeHandler(IDomainEventDispatcher dispatcher) => _dispatcher = dispatcher;

//     public async Task HandleAsync(ApproveChange cmd)
//     {
//         var change = LoadAggregate(cmd.ChangeId);
//         change.Approve(cmd.UserId);
//         Save(change);

//         // dispatch events
//         await _dispatcher.DispatchAsync(change.DomainEvents);
//     }
// }

// public class OutboxEventDispatcher : IDomainEventDispatcher
// {
//     private readonly IOutboxRepository _outbox;
//     private readonly IEventBus _bus;

//     public async Task DispatchAsync(IEnumerable<object> events)
//     {
//         foreach (var evt in events)
//         {
//             await _bus.PublishAsync(evt); // may fail
//             await _outbox.MarkProcessedAsync(evt); // only after success
//         }
//     }
// }
