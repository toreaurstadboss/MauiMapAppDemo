using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MauiMapAppDemo.Pages;

public partial class MapsDemo : ContentPage
{
	public MapsDemo()
	{
		InitializeComponent();

        var initialLocation = new Location(63.4305, 10.3951); //Initial location - Trondheim

        MapCtrl.MoveToRegion(
                    MapSpan.FromCenterAndRadius(
                        initialLocation,
                        Distance.FromKilometers(5)));

        AddPinToCabins();

    }

    private void AddPinToCabins()
    {
        AddPin("Estenstadhytta",
                   "Popular hiking and ski cabin",
                   63.39478511341777, 10.488396418945399);

        AddPin("Elgsethytta",
            "Classic cabin in Bymarka",
           63.42046109744264, 10.21251573534687);

        AddPin("Skistua",
            "Gateway to Bymarka",
            63.41789876758627, 10.26377206090971);

        AddPin("Grønlia",
            "Popular stop for skiers",
            63.40284632068321, 10.243509429760273);

        AddPin("Rønningen",
            "Cabin and cafe",
            63.37881115331552, 10.262531628360053);

    }

    private void AddPin(
        string label,
        string address,
        double latitude,
        double longitude)
    {
        MapCtrl.Pins.Add(new Pin
        {            
            Label = label,
            Address = address,
            Location = new Location(latitude, longitude)
        });    
    }


}