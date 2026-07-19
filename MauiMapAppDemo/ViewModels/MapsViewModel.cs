using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiMapAppDemo.Repositories.PinLocations;
using MauiMapAppDemo.ViewModels.Messages;
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
        public bool _isHeightProfileMode;

        [ObservableProperty]
        private Location? _firstLocationMeasureMode;

        [ObservableProperty]
        private Location? _secondLocationMeasureMode;

        [ObservableProperty]
        private double _distanceMeasuredKm;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private Dictionary<double, double> _heightProfiles = new();

        [ObservableProperty]
        private bool _isHeightProfilesUpdated;

        public ObservableCollection<MapPinModel> CabinPins { get; } = [];

        public Location MapCenter { get; } = new(63.4305, 10.3951);

        public MapsViewModel(OpenTopoService openTopoService, GeocodingService geocodingService, DialogService dialogService)
        {
            InitCabinPins();

            _openTopoService = openTopoService;
            _geocodingService = geocodingService;
            _dialogService = dialogService;

            WeakReferenceMessenger.Default.Register<ToggleMeasureModeMessage>(this, (_, _) =>
            {
                ToggleMeasureModeCommand.Execute(null);
            });

            WeakReferenceMessenger.Default.Register<ToggleHeightProfileCommandMessage>(this, (_, _) =>
            {
                ToggleHeightProfileCommand.Execute(null);
            });
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
        private void ToggleHeightProfile()
        {
            IsHeightProfileMode = !IsHeightProfileMode;

            if (IsHeightProfileMode && !IsMeasuringMode)
            {
                IsMeasuringMode = true;
            }
        }

        [RelayCommand]
        private void ToggleMeasureMode()
        {
            IsMeasuringMode = !IsMeasuringMode;
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
                await HandleMeasuringMode(location);
                return;
            }

            await HandleDefaultMapClicked(location);
        }

        private async Task HandleMeasuringMode(Location location)
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

                if (IsHeightProfileMode)
                {
                    await HandleHeightProfileMode();
                }

                return;
            }
            
            //Third click restarts over 
            FirstLocationMeasureMode = location;
            SecondLocationMeasureMode = null;
        }

        private async Task HandleHeightProfileMode()
        {
            // OpenTopoData Api got these limits 
            // Max 1000 calls per day
            // Max 100 locations per request
            // https://www.opentopodata.org/

            try
            {

                IsBusy = true; //since OpenTopoApi is rate-limited with 1 request per second (..) we show ActivityIndicator

                const int maxSamples = 10;
                const int sleepMillisecondsBetweenRequest = 1050; //sleep at least a second to avoid 429 Too Many Requests

                var samples = maxSamples;

                var startLocation = FirstLocationMeasureMode!;
                var endLocation = SecondLocationMeasureMode!;

                var heightProfiles = new Dictionary<double, double>();

                for (var sampleIndex = 0; sampleIndex < samples; sampleIndex++)
                {
                    var fraction = samples == 1 ? 0d : sampleIndex / (double)(samples - 1);
                    var currentLatitude = startLocation.Latitude + ((endLocation.Latitude - startLocation.Latitude) * fraction);
                    var currentLongitude = startLocation.Longitude + ((endLocation.Longitude - startLocation.Longitude) * fraction);
                    var currentProfile = DistanceMeasuredKm * fraction;

                    var elevationOfPoint = await _openTopoService.GetElevationAsync(currentLatitude, currentLongitude);
                    if (elevationOfPoint.HasValue)
                    {
                        heightProfiles[currentProfile] = elevationOfPoint.Value;
                    }

                    await Task.Delay(sleepMillisecondsBetweenRequest);

                }

                HeightProfiles = heightProfiles;
                IsHeightProfilesUpdated = !IsHeightProfilesUpdated;

            }
            finally
            {
                IsBusy = false;
            }

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
