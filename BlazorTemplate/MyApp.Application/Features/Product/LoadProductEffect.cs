namespace MyApp.Application.Features.Products;

using MyApp.Application.Abstractions;
using System;
using System.Reactive.Linq;

public sealed class LoadProductEffect(IProductApi api) : IEffect<LoadProductCmd, ProductDetailDTO>
{
    public IObservable<ProductDetailDTO> Handle(LoadProductCmd command)
    {
        return Observable.FromAsync(async () =>
        {
            return await api.LoadProductAsync(command.ProductId);
        });
    }
}
