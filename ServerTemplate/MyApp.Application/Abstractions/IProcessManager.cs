namespace MyApp.Application.Abstractions;

// Saga listens to events emitted by aggregates.

// Determines next command(s) to execute.

// Keeps workflow logic out of aggregates.

// Can implement retry, compensation, or branching logic.

// Works well with Outbox + EventBus for reliability.

// no need if implement using Actor
public interface IProcessManager
{
    Task HandleEventAsync(object domainEvent, CancellationToken ct = default);
}


// using Proto;
// using System.Threading.Tasks;

// // Messages
// public record ChangeApprovedEvent(string ChangeId);
// public record ReleaseChange(string ChangeId);

// // Saga actor state
// public class ChangeApprovalSagaState
// {
//     public bool Released { get; set; } = false;
// }

// // Actor implementation
// public class ChangeApprovalSagaActor : IActor
// {
//     private readonly IAsyncCommandHandler<ReleaseChange, CommandResult> _releaseHandler;
//     private readonly ChangeApprovalSagaState _state = new();

//     public ChangeApprovalSagaActor(IAsyncCommandHandler<ReleaseChange, CommandResult> releaseHandler)
//     {
//         _releaseHandler = releaseHandler;
//     }

//     public Task ReceiveAsync(IContext context)
//     {
//         switch (context.Message)
//         {
//             case ChangeApprovedEvent evt when !_state.Released:
//                 _state.Released = true;
//                 _ = _releaseHandler.HandleAsync(new ReleaseChange { ChangeId = evt.ChangeId });
//                 break;
//         }
//         return Task.CompletedTask;
//     }
// }
