namespace Lpc.WebUI.Layouts;

using System.Reactive.Linq;
using Lpc.Application.Abstractions;
using Lpc.Application.Features.User;
using Lpc.Presentation.Abstractions;

internal sealed class HeaderViewModel : ViewModelBase
{
    private const string GuestDisplayName = "Guest";

    public HeaderViewModel(IUserProfileStore userProfileStore, GlobalErrorStore errorStore) : base(errorStore)
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
