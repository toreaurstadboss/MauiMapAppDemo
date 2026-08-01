using CommunityToolkit.Maui.Views;
using MauiMapAppDemo.Services;
using MauiMapAppDemo.ViewModels;

namespace MauiMapAppDemo.Views;

public partial class KartverketInfoPopup : Popup
{
    public KartverketInfoPopup(double latitude, double longitude, KartverketPunktResponse? response, KartverketOmraadeResponse? omraadeResponse)
    {
        InitializeComponent();

        BindingContext = new KartverketInfoPopupViewModel(latitude, longitude, response, omraadeResponse);
    }

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        this.CloseAsync();
    }
}