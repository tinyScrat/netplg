namespace MyApp.Application.Features.Products;

using MyApp.Application.Abstractions;
using System;
using System.Reactive.Linq;

// one effect map to one commnad

public sealed record PublishProductCmd(Guid ProductId) : ICommand<int>;


public sealed class PublishProductEffect(IProductApi api) : IEffect<PublishProductCmd, int>
{
    public IObservable<int> Handle(PublishProductCmd command, CancellationToken ct)
    {
        return Observable.FromAsync(async () =>
        {
            await api.PublishProductAsync(command.ProductId);
            return 1;
        });
    }
}
