namespace MyApp.Application.Features.Storage;

public interface IKeyValueStorage
{
    ValueTask SetAsync<T>(string key, T value, CancellationToken ct = default);

    ValueTask<T?> GetAsync<T>(string key, CancellationToken ct = default);

    ValueTask<bool> ContainsKeyAsync(string key, CancellationToken ct = default);

    ValueTask RemoveAsync(string key, CancellationToken ct = default);

    ValueTask ClearAsync(CancellationToken ct = default);
}
