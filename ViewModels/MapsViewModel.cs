using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiMapAppDemo.Repositories.PinLocations;
using MauiMapAppDemo.Services;
using System.Collections.ObjectModel;

namespace MauiMapAppDemo.ViewModels
{

    public partial class MapsViewModel : ObservableObject
    {
        private readonly OpenTopoService _openTopoService;
        private readonly GeocodingService _geocodingService;
        private readonly DialogService _dialogService;

        private bool _pinClickInProgress = false;

        [ObservableProperty]
        public bool _isMeasuringMode;

        [ObservableProperty]
        private Location? _firstLocationMeasureMode;

        [ObservableProperty]
        private Location? _secondLocationMeasureMode;

        [ObservableProperty]
        private double _distanceMeasuredKm;

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
            finally
            {
                _pinClickInProgress = false;
            }
        }

        [RelayCommand]
        private async Task ToggleMeasureMode()
        {
            IsMeasuringMode = !IsMeasuringMode; //toggle the measuring mode
        }

        [RelayCommand]
        private async Task MapClicked(Location location)
        {
            if (_pinClickInProgress)
            {
                return;
            }

            if (IsMeasuringMode)
            {
                HandleMeasuringMode(location);
                return;
            }

            await HandleDefaultMapClicked(location);
        }

        private void HandleMeasuringMode(Location location)
        {
            if (FirstLocationMeasureMode == null)
            {
                FirstLocationMeasureMode = location;
                return;
            }

            if (SecondLocationMeasureMode == null)
            {
                SecondLocationMeasureMode = location;

                var distance = Location.CalculateDistance(
                    FirstLocationMeasureMode,
                    SecondLocationMeasureMode,
                    DistanceUnits.Kilometers
                    );

                DistanceMeasuredKm = Math.Round(distance, 1);

                return;
            }

            //Third click restarts over 
            FirstLocationMeasureMode = location;
            SecondLocationMeasureMode = null;
        }      

        private async Task HandleDefaultMapClicked(Location location)
        {
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
