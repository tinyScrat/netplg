namespace MyApp.WebUI.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MyApp.Application.Abstractions;

public sealed class BlazorNavigationDialogCancellation(NavigationManager navigation) : INavigationDialogCancellation, IDisposable
{
    private bool _started = false;

    public event Action? NavigationTriggered;

    public void Start()
    {
        if (_started)
            return;

        navigation.LocationChanged += OnLocationChanged;
        _started = true;
    }

    public void Stop()
    {
        if (!_started)
            return;

        navigation.LocationChanged -= OnLocationChanged;
        _started = false;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        NavigationTriggered?.Invoke();
    }

    public void Dispose()
    {
        Stop();
    }
}
