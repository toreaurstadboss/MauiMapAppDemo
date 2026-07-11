using MauiMapAppDemo.Repositories.PinLocations;
using MauiMapAppDemo.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MauiMapAppDemo.Pages;

public partial class MapsDemo : ContentPage
{
    private readonly OpenTopoService _openTopoService;

    public MapsDemo(OpenTopoService openTopoService)
	{
		InitializeComponent();

        var initialLocation = new Location(63.4305, 10.3951); //Initial location - Trondheim

        MapCtrl.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        initialLocation,
                        Distance.FromKilometers(8)));

        AddPinToCabins();
        _openTopoService = openTopoService;
    }

    private void AddPinToCabins()
    {
        var cabins = TrondheimCabins.GetSampleData();

        foreach (var cabin in cabins)
        {
            AddPin(cabin.Name, cabin.Description, cabin.Latitude, cabin.Longitude);
        }
    }

    private void AddPin(
        string label,
        string address,
        double latitude,
        double longitude)
    {
        var pin = new Pin
        {
            Label = label,
            Address = address,
            Location = new Location(latitude, longitude)
        };

        pin.MarkerClicked += async (s, e) =>
        {

            var elevationOfPoint = await _openTopoService.GetElevationAsync(latitude, longitude);

            await DisplayAlertAsync(
                    label,
                    address + $"\nElevation (Open Topo Data API): {elevationOfPoint} m (m.a.s.)",
                    "OK"
                ); //on click , alert the pin data also via this marker clicked callback 

        };

        MapCtrl.Pins.Add(pin);    
    }


}