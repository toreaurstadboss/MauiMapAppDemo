using MauiMapAppDemo.Services;
using MauiMapAppDemo.ViewModels;

namespace MauiMapAppDemo.Pages;

public partial class MapsDemo : ContentPage
{
  
    public MapsDemo(OpenTopoService openTopoService, GeocodingService geocodingService, DialogService dialogService)
	{
		InitializeComponent();

        BindingContext = new MapsViewModel(openTopoService, geocodingService, dialogService);
    }

}