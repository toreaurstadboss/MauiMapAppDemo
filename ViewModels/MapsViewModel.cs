using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiMapAppDemo.Repositories.PinLocations;
using MauiMapAppDemo.Services;
using System.Collections.ObjectModel;
using Microsoft.Maui.Maps;

namespace MauiMapAppDemo.ViewModels
{

    public partial class MapsViewModel : ObservableObject
    {
        private readonly OpenTopoService _openTopoService;
        private readonly GeocodingService _geocodingService;
        private readonly DialogService _dialogService;
        private bool _pinClickInProgress = false;

        public ObservableCollection<MapPinModel> CabinPins { get; } = [];

        public Location MapCenter { get; } = new(63.4305, 10.3951);

        public MapsViewModel(OpenTopoService openTopoService, GeocodingService geocodingService, DialogService dialogService)
        {
            InitCabinPins();

            _openTopoService = openTopoService;
            _geocodingService = geocodingService;
            _dialogService = dialogService;
        }


        [RelayCommand]
        private async Task PinClicked(MapPinModel pin)
        {
            _pinClickInProgress = true;
            try
            {
                var elevation =
                    await _openTopoService.GetElevationAsync(
                        pin.Latitude,
                        pin.Longitude);

                var placementInfo =
                    await _geocodingService.GetGeocodingPlacemark(
                        pin.Latitude,
                        pin.Longitude);

                await _dialogService.ShowAlertAsync(
                    pin.Label,
                    $"{pin.Address}\n\nElevation: {elevation}m\n\n{placementInfo}",
                    "OK");
            }
            catch
            {
                _pinClickInProgress = false;
            }
        }

        [RelayCommand]
        private async Task MapClicked(Location location)
        {
            if (_pinClickInProgress)
            {
                return;
            }

            var elevationOfPoint = await _openTopoService.GetElevationAsync(location.Latitude, location.Longitude);
            await ShowLocationInformationAlert($"Clicked point in the map:", $"Showing elevation of clicked point:", location.Latitude, location.Longitude);
        }

        private async Task ShowLocationInformationAlert(string label, string address, double latitude, double longitude)
        {
            var elevationOfPoint = await _openTopoService.GetElevationAsync(latitude, longitude);

            var placementInfo = await _geocodingService.GetGeocodingPlacemark(latitude, longitude);

            await _dialogService.ShowAlertAsync(
                    label,
                    address + $"\n\nElevation: {elevationOfPoint} m\n\nGeocoding (Placement) info:\n {placementInfo ?? "<None>"}",
                    "OK"
                ); //on click , alert the pin data also via this marker clicked callback 
        }

        private void InitCabinPins()
        {
            foreach (var cabin in TrondheimCabins.GetSampleData())
            {
                CabinPins.Add(
                    new MapPinModel
                    {
                        Label = cabin.Name,
                        Address = cabin.Description,
                        Latitude = cabin.Latitude,
                        Longitude = cabin.Longitude
                    });
            }
        }

    }
}
