namespace MyApp.WebUI.Layouts;

using System.Reactive.Linq;
using MyApp.Application.Features.User;
using MyApp.WebUI.Abstractions;

internal sealed class HeaderViewModel : ViewModelBase
{
    private const string GuestDisplayName = "Guest";

    public HeaderViewModel(IUserProfileStore userProfileStore)
    {
        userProfileStore.Changes
            .DistinctUntilChanged()
            .Select(profile => profile?.DisplayName ?? GuestDisplayName)
            .StartWith(userProfileStore.Current?.DisplayName ?? GuestDisplayName)
            .Subscribe(displayName =>
            {
                UserDisplayName = displayName;
                RaiseStateChanged();
            })
            .DisposeWith(this);
    }

    public string UserDisplayName { get; private set; } = GuestDisplayName;
}
