using MauiMapAppDemo.Services;
using MauiMapAppDemo.ViewModels;

namespace MauiMapAppDemo.Pages;

public partial class MapsDemo : ContentPage
{
  

    public MapsDemo(IElevationService elevationService, GeocodingService geocodingService, DialogService dialogService)
	{
		InitializeComponent();

        BindingContext = new MapsViewModel(elevationService, geocodingService, dialogService);
    }

}