using MauiMapAppDemo.Services;
using MauiMapAppDemo.ViewModels;

namespace MauiMapAppDemo.Pages;

public partial class MapsDemo : ContentPage
{


    public MapsDemo(IElevationService elevationService, GeocodingService geocodingService, DialogService dialogService,
        KartverketService kartverketService)
    {
        InitializeComponent();

        BindingContext = new MapsViewModel(elevationService, geocodingService, dialogService, kartverketService);
    }

}