namespace Lpc.Application.Features.Products;

using Lpc.Application.Abstractions;
using System;
using System.Reactive.Linq;
using Lpc.Application.Contracts;

public sealed record LoadProductDetailCmd(Guid ProductId) : ICommand<ProductDetailDTO>;

public sealed class LoadProductDetailEffect(IProductApi api) : IEffect<LoadProductDetailCmd, ProductDetailDTO>
{
    public IObservable<ProductDetailDTO> Handle(LoadProductDetailCmd command, CancellationToken ct)
    {
        return Observable.FromAsync(async () =>
        {
            return await api.LoadProductDetailAsync(command.ProductId, ct);
        });
    }
}
