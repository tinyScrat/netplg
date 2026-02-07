namespace MyApp.WebUI.Features.Storage;

using MyApp.Application.Features.Storage;
using System.Text.Json;
using Microsoft.JSInterop;

public sealed class BrowserLocalStorage : IKeyValueStorage
{
    private readonly IJSRuntime _js;
    private readonly JsonSerializerOptions _json;

    public BrowserLocalStorage(
        IJSRuntime js,
        JsonSerializerOptions? jsonOptions = null)
    {
        _js = js;
        _json = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async ValueTask SetAsync<T>(
        string key,
        T value,
        CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value, _json);

        await _js.InvokeVoidAsync(
            "localStorage.setItem",
            ct,
            key,
            json
        );
    }

    public async ValueTask<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default)
    {
        var json = await _js.InvokeAsync<string?>(
            "localStorage.getItem",
            ct,
            key
        );

        if (json is null)
            return default;

        return JsonSerializer.Deserialize<T>(json, _json);
    }

    public async ValueTask<bool> ContainsKeyAsync(
        string key,
        CancellationToken ct = default)
    {
        var value = await _js.InvokeAsync<string?>(
            "localStorage.getItem",
            ct,
            key
        );

        return value is not null;
    }

    public async ValueTask RemoveAsync(
        string key,
        CancellationToken ct = default)
    {
        await _js.InvokeVoidAsync(
            "localStorage.removeItem",
            ct,
            key
        );
    }

    public async ValueTask ClearAsync(CancellationToken ct = default)
    {
        await _js.InvokeVoidAsync(
            "localStorage.clear",
            ct
        );
    }
}
