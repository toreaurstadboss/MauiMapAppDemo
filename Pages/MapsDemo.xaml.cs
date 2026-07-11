using MauiMapAppDemo.Repositories.PinLocations;
using MauiMapAppDemo.Services;
using MauiMapAppDemo.ViewModels;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MauiMapAppDemo.Pages;

public partial class MapsDemo : ContentPage
{
    private readonly OpenTopoService _openTopoService;
    private readonly GeocodingService _geocodingService;
    private bool _pinClickInProgress = false;

    public MapsDemo(OpenTopoService openTopoService, GeocodingService geocodingService, DialogService dialogService)
	{
		InitializeComponent();

        BindingContext = new MapsViewModel(openTopoService, geocodingService, dialogService);

        //var initialLocation = new Location(63.4305, 10.3951); //Initial location - Trondheim

        //MapCtrl.MoveToRegion(
        //            MapSpan.FromCenterAndRadius(
        //                initialLocation,
        //                Distance.FromKilometers(8)));

        //AddPinToCabins();
        //_openTopoService = openTopoService;
        //_geocodingService = geocodingService;
    }

    //private void AddPinToCabins()
    //{
    //    var cabins = TrondheimCabins.GetSampleData();

    //    foreach (var cabin in cabins)
    //    {
    //        AddPin(cabin.Name, cabin.Description, cabin.Latitude, cabin.Longitude);
    //    }
    //}

    //private void AddPin(
    //    string label,
    //    string address,
    //    double latitude,
    //    double longitude)
    //{
    //    var pin = new Pin
    //    {
    //        Label = label,
    //        Address = address,
    //        Location = new Location(latitude, longitude)
    //    };

    //    pin.MarkerClicked += async (s, e) =>
    //    {
    //        _pinClickInProgress = true;
    //        try
    //        {
    //            await ShowLocationInformationAlert(label, address, latitude, longitude);
    //        }
    //        finally
    //        {
    //            _pinClickInProgress = false;
    //        }
    //    };

    //    MapCtrl.Pins.Add(pin);    
    //}

    //private async Task ShowLocationInformationAlert(string label, string address, double latitude, double longitude)
    //{
    //    var elevationOfPoint = await _openTopoService.GetElevationAsync(latitude, longitude);

    //    var placementInfo = await _geocodingService.GetGeocodingPlacemark(latitude, longitude);

    //    await DisplayAlertAsync(
    //            label,
    //            address + $"\n\nElevation: {elevationOfPoint} m\n\nGeocoding (Placement) info:\n {placementInfo ?? "<None>"}",
    //            "OK"
    //        ); //on click , alert the pin data also via this marker clicked callback 
    //}

    //public async void MapCtrl_MapClicked(object sender, MapClickedEventArgs e)
    //{
    //    if (_pinClickInProgress)
    //    {
    //        return;
    //    }

    //    var elevationOfPoint = await _openTopoService.GetElevationAsync(e.Location.Latitude, e.Location.Longitude);

    //    await ShowLocationInformationAlert($"Clicked point in the map:", $"Showing elevation of clicked point:", e.Location.Latitude, e.Location.Longitude);
    //}

}