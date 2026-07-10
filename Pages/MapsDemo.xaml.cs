using Microsoft.Maui.Maps;

namespace MauiMapAppDemo.Pages;

public partial class MapsDemo : ContentPage
{
	public MapsDemo()
	{
		InitializeComponent();

        MapCtrl.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        new Location(63.4305, 10.3951), // Trondheim
                        Distance.FromKilometers(10)));

    }
}