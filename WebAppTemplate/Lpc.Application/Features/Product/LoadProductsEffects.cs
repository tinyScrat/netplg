namespace Lpc.Application.Features.Products;

using Lpc.Application.Abstractions;
using System;
using System.Reactive.Linq;
using Lpc.Application.Contracts;
using Microsoft.Extensions.Logging;

public sealed record LoadProductsCmd(int Page, int PageSize) : ICommand<PagedResult<ProductOverviewDTO>>;

public sealed class LoadProductsEffect(
    IProductApi api,
    ILogger<LoadProductsEffect> logger) : IEffect<LoadProductsCmd, PagedResult<ProductOverviewDTO>>
{
    public IObservable<PagedResult<ProductOverviewDTO>> Handle(LoadProductsCmd command, CancellationToken ct)
    {
        return Observable
            .FromAsync(async () => await api.LoadProductsAsync(command.Page, command.PageSize, ct))
            .WithRetry(logger);
    }
}
