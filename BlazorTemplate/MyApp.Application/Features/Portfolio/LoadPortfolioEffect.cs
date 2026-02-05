namespace MyApp.Application.Features.Portfolios;

using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MyApp.Application.Abstractions;

/*
| Concept     | Role                 | Lifetime    |
| ----------- | -------------------- | ----------- |
| **State**   | What *is*            | Long-lived  |
| **Intent**  | What *should happen* | Short-lived |
| **Effects** | How it happens       | Stateless   |

State is concrete and owned.
Intent is abstract and replaceable.
Effects are stateless executors.

Register concrete for ownership, interface for visibility — both pointing to the same instance.
*/

// Concrete for mutation
internal sealed class LoadPortfolioEffect(PortfolioStore store) : IEffect<LoadPortfolioCmd, Unit>
{
    public IObservable<Unit> Handle(LoadPortfolioCmd command, CancellationToken ct)
    {
        return Observable.FromAsync(async () =>
        {
            await Task.Delay(1000);
            store.UpdateName("Scrat");
            return Unit.Default;
        });
    }
}

// interface for passsive observer
internal sealed class PortfolioFeedEffect : IDisposable
{
    private readonly CompositeDisposable _disposables = [];

    public PortfolioFeedEffect(IPortfolioStore store)
    {
        _disposables.Add(store.Changes.Subscribe(s =>
            Console.WriteLine(s.Name)));
    }

    public void Dispose()
    {
        _disposables.Dispose();

        GC.SuppressFinalize(this);
    }
}
