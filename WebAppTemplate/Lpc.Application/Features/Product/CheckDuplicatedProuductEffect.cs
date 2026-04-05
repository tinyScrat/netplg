namespace Lpc.Application.Features.Products;

using Lpc.Application.Abstractions;
using System;
using System.Reactive.Linq;
using Lpc.Application.Contracts;

public sealed record CheckDuplicatedProductCmd(Guid ProductId) : ICommand<DuplicatedProductResultDTO>;

public sealed class CheckDuplicatedProuductEffect : IEffect<CheckDuplicatedProductCmd, DuplicatedProductResultDTO>
{
    private static readonly Dictionary<Guid, string> existingProducts = new()
    {
        [Guid.Parse("a1f3c9d2-1b4a-4f6d-9c01-0c2e8d9a0002")] = "PROD-001",
        [Guid.Parse("a1f3c9d2-1b4a-4f6d-9c01-0c2e8d9a0003")] = "PROD-002",
        [Guid.Parse("a1f3c9d2-1b4a-4f6d-9c01-0c2e8d9a0006")] = "PROD-006"
    };

    public IObservable<DuplicatedProductResultDTO> Handle(CheckDuplicatedProductCmd command, CancellationToken ct)
    {
        var isDuplicated = existingProducts.TryGetValue(command.ProductId, out var existingNumber);

        return Observable.FromAsync(async () =>
        {
            await Task.Delay(600, ct); // Simulate async work

            return new DuplicatedProductResultDTO
            {
                ProductId = command.ProductId,
                IsDuplicated = isDuplicated,
                ExistingProductNumber = existingNumber
            };
        });
    }
}
