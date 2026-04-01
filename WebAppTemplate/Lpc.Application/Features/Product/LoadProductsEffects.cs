namespace Lpc.Application.Features.Products;

using Lpc.Application.Abstractions;
using System;
using System.Reactive.Linq;
using Lpc.Application.Contracts;
using Microsoft.Extensions.Logging;

public sealed record LoadProductsCmd : ICommand<IEnumerable<ProductOverviewDTO>>;

public sealed class LoadProductsEffect(
    IProductApi api,
    ILogger<LoadProductsEffect> logger) : IEffect<LoadProductsCmd, IEnumerable<ProductOverviewDTO>>
{
    public IObservable<IEnumerable<ProductOverviewDTO>> Handle(LoadProductsCmd command, CancellationToken ct)
    {
        return Observable
            .FromAsync(async () => await api.LoadProductsAsync(ct))
            .WithRetry(logger);
    }
}
