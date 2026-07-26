using CommunityToolkit.Maui.Views;
using MauiMapAppDemo.Services;
using MauiMapAppDemo.ViewModels;

namespace MauiMapAppDemo.Views;

public partial class KartverketInfoPopup : Popup
{
    public KartverketInfoPopup(double latitude, double longitude, KartverketPunktResponse? response)
    {
        InitializeComponent();

        BindingContext = new KartverketInfoPopupViewModel(latitude, longitude, response);
    }

    private async Task OnCloseClicked(object? sender, EventArgs e)
    {
        await this.CloseAsync();
    }
}