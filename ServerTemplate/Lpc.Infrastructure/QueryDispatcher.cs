namespace Lpc.Infrastructure;

using Lpc.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class QueryDispatcher(IServiceProvider sp) : IQueryDispatcher
{
    public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken ct = default)
        where TQuery : IQuery<TResult>
    {
        var handler = sp.GetRequiredService<IQueryHandler<TQuery, TResult>>();
        return await handler.HandleAsync(query, ct);
    }
}
