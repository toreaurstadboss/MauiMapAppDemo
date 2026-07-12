using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiMapAppDemo.ViewModels
{

    public partial class MapPinModel : ObservableObject
    {
        [ObservableProperty]
        private string label = string.Empty;

        [ObservableProperty]
        private string address = string.Empty;

        [ObservableProperty]
        private double latitude;

        [ObservableProperty]
        private double longitude;

    }

}
