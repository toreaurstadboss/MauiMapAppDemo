using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiMapAppDemo.ViewModels.Messages;

namespace MauiMapAppDemo.ViewModels
{
    public partial class ShellViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task ToggleMeasureModeAsync()
        {
            await NavigateToMapsDemoAsync();
            WeakReferenceMessenger.Default.Send(new ToggleMeasureModeMessage());
        }

        [RelayCommand]
        private async Task ToggleHeightProfileAsync()
        {
            await NavigateToMapsDemoAsync();
            WeakReferenceMessenger.Default.Send(new ToggleHeightProfileCommandMessage());
        }

        private static async Task NavigateToMapsDemoAsync()
        {
            var currentLocation = Shell.Current?.CurrentState.Location.OriginalString;
            if (!string.IsNullOrWhiteSpace(currentLocation) &&
                currentLocation.Contains("MapsPage", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (Shell.Current == null)
            {
                return;
            }

            await Shell.Current.GoToAsync("//MapsPage");
        }
    }
}