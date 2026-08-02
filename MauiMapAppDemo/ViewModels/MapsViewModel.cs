using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiMapAppDemo.Repositories.PinLocations;
using MauiMapAppDemo.Services;
using MauiMapAppDemo.ViewModels.Messages;
using System.Collections.ObjectModel;

namespace MauiMapAppDemo.ViewModels
{

    public partial class MapsViewModel : ObservableObject
    {
        private readonly IElevationService _elevationService;
        private readonly GeocodingService _geocodingService;
        private readonly DialogService _dialogService;
        private readonly KartverketService _kartverketService;

        private bool _pinClickInProgress = false;

        [ObservableProperty]
        private Location _mapCenter = new(63.4305, 10.3951);

        [ObservableProperty]
        public bool _isMeasuringMode;

        [ObservableProperty]
        public bool _isHeightProfileMode;

        [ObservableProperty]
        public bool _isMatrikkelMode;

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

        [ObservableProperty]
        private Location[] _matrikkelPolygonPath = Array.Empty<Location>();

        [ObservableProperty]
        private string _matrikkelAreaText = string.Empty;

        public ObservableCollection<MapPinModel> CabinPins { get; } = [];

        public MapsViewModel(IElevationService elevationService, GeocodingService geocodingService, DialogService dialogService, KartverketService kartverketService)
        {
            InitCabinPins();

            _elevationService = elevationService;
            _geocodingService = geocodingService;
            _dialogService = dialogService;
            _kartverketService = kartverketService;

            WeakReferenceMessenger.Default.Register<ToggleMeasureModeMessage>(this, (_, _) =>
            {
                ToggleMeasureModeCommand.Execute(null);
            });

            WeakReferenceMessenger.Default.Register<ToggleHeightProfileCommandMessage>(this, (_, _) =>
            {
                ToggleHeightProfileCommand.Execute(null);
            });

            WeakReferenceMessenger.Default.Register<ToggleMatrikkelInfoCommandMessage>(this, (_, _) =>
            {
                ToggleShowMatrikkelInformationCommand.Execute(null);
            });

            InitMapCenterLocation();

        }

        private void InitMapCenterLocation()
        {
            MapCenter = new Location(63.4305, 10.3951); //init to center over Trondheim, Norway
        }

        [RelayCommand]
        private async Task PinClicked(MapPinModel pin)
        {
            _pinClickInProgress = true;
            try
            {
                var elevation =
                    await _elevationService.GetElevationAsync(
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
        private void ToggleShowMatrikkelInformation()
        {
            IsMeasuringMode = false;
            IsHeightProfileMode = false;
            IsMatrikkelMode = !IsMatrikkelMode;

            if (IsMatrikkelMode)
            {
                MatrikkelAreaText = "Klikk i kartet for å hente eiendomsgrensen.";
            }
            else
            {
                ClearMatrikkelOverlay();
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

                int maxSamples = _elevationService.ProviderName?.Contains("Google") == true ? 50 : 10;
                int sleepMillisecondsBetweenRequest = _elevationService.ProviderName?.Contains("Google") == true ? 10 : 1050; ; //sleep at least a second to avoid 429 Too Many Requests for OpenTopo

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

                    var elevationOfPoint = await _elevationService.GetElevationAsync(currentLatitude, currentLongitude);
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
            await ShowLocationInformationAlert(location.Latitude, location.Longitude);
        }

        private async Task ShowLocationInformationAlert(double latitude, double longitude)
        {
            await UpdateMatrikkelOverlayAsync(latitude, longitude);

            var elevationOfPoint = await _elevationService.GetElevationAsync(latitude, longitude);

            var placementInfo = await _geocodingService.GetGeocodingPlacemark(latitude, longitude);
            var label = BuildLocationLabel(latitude, longitude, placementInfo);

            var latitudeHemisphere = latitude >= 0 ? "N" : "S";
            var longitudeHemisphere = longitude >= 0 && longitude < 180 ? "E" : "W";

            string pointClickedMessageInfo = $"""
               🧭 Position:
               Latitude: {latitude:F6}° {latitudeHemisphere}
               Longitude: {longitude:F6}° {longitudeHemisphere}
           
               🏔️ Elevation: {elevationOfPoint} m
           
               ℹ️Geocoding (Placement) info:
               {placementInfo ?? "<None>"}
               """;

            const string showDetails = "🗺️Show details about point ➡️";

            const string showMatrikkelInformation = "🧭Show matrikkel info";

            const string copyLat = "📋 Copy latitude";
            const string copyLong = "📋Copy longitude";
            const string copyLatLong = "📋Copy latitude+longitude";

            //first show the point details 

            await _dialogService.ShowAlertAsync(label, pointClickedMessageInfo);

            var shouldInspectLocation = await _dialogService.ShowInspectLocationConfirmationPopupAsync(label);
            if (!shouldInspectLocation)
            {
                return;
            }

            var chosenAction = await _dialogService.ShowActionSheetAsync(label, "Select an option ⬇️",
               showDetails, copyLat, copyLong, copyLatLong, IsMatrikkelMode ? showMatrikkelInformation : string.Empty);

            bool copiedToClipboard = new[] { copyLat, copyLong, copyLatLong }.Contains(chosenAction);
            switch (chosenAction)
            {
                case copyLat:
                    await Clipboard.SetTextAsync($"{latitude}");
                    break;
                case copyLong:
                    await Clipboard.SetTextAsync($"{longitude}");
                    break;
                case copyLatLong:
                    await Clipboard.SetTextAsync($"{latitude},{longitude}");
                    break;
            }

            if (copiedToClipboard)
            {
                Toast.Make($"✅ Copied coordinates to clipboard: {chosenAction}!", CommunityToolkit.Maui.Core.ToastDuration.Short, 18);
            }

            if (chosenAction == showDetails)
            {
                await _dialogService.ShowAlertAsync(label, pointClickedMessageInfo);
            }

            if (chosenAction == showMatrikkelInformation)
            {
                await ShowMatrikkelInformationAsync(latitude, longitude);
            }
        }

        private static string BuildLocationLabel(double latitude, double longitude, string? placementInfo)
        {
            var coordinateLabel = $"{latitude:F6}, {longitude:F6}";
            var placemarkLine = GetFirstLine(placementInfo);

            if (string.IsNullOrWhiteSpace(placemarkLine))
            {
                return $"🚩 {coordinateLabel}";
            }

            return $"🚩 {coordinateLabel} · {placemarkLine}";
        }

        private static string? GetFirstLine(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();
        }

        private async Task ShowMatrikkelInformationAsync(double latitude, double longitude)
        {
            var kartverketResponse = await _kartverketService.GetMatrikkelInformationFromLocationAsync(latitude, longitude);

            if (kartverketResponse?.Eiendom?.Any() != true)
            {
                return;
            }

            var omraadeResponse = await _kartverketService.GetGeoJsonFromLocationAsync(latitude, longitude);            

            await _dialogService.ShowKartverketInfoPopupAsync(latitude, longitude, kartverketResponse, omraadeResponse);
        }

        private async Task UpdateMatrikkelOverlayAsync(double latitude, double longitude)
        {
            if (!IsMatrikkelMode)
            {
                ClearMatrikkelOverlay();
                return;
            }

            var omraadeResponse = await _kartverketService.GetGeoJsonFromLocationAsync(latitude, longitude);
            var feature = omraadeResponse?.Features?.FirstOrDefault();
            var coordinates = feature?.Geometry?.Coordinates;

            if (coordinates?.Length > 0)
            {
                var outerRing = coordinates[0];

                if (outerRing?.Length >= 3)
                {
                    MatrikkelPolygonPath = BuildMatrikkelPolygonPath(outerRing);

                    if (omraadeResponse?.TotalAreaOfAllAreas is double totalArea && totalArea > 0)
                    {
                        MatrikkelAreaText = FormatMatrikkelAreaText(totalArea);
                    }
                    else
                    {
                        MatrikkelAreaText = "Areal: ukjent";
                    }

                    return;
                }
            }

            ClearMatrikkelOverlay();
            MatrikkelAreaText = "Ingen eiendom funnet for valgt punkt.";
        }

        private static Location[] BuildMatrikkelPolygonPath(double[][] outerRing)
        {
            var path = new List<Location>(outerRing.Length + 1);

            for (int index = 0; index < outerRing.Length; index++)
            {
                path.Add(new Location(outerRing[index][1], outerRing[index][0]));
            }

            if (path.Count > 0)
            {
                var firstLocation = path[0];
                var lastLocation = path[^1];

                if (firstLocation.Latitude != lastLocation.Latitude || firstLocation.Longitude != lastLocation.Longitude)
                {
                    path.Add(firstLocation);
                }
            }

            return path.ToArray();
        }

        private static string FormatMatrikkelAreaText(double areaSquareMetres)
        {
            var maal = areaSquareMetres / 1000d;
            return $"Areal: {areaSquareMetres:N0} m² ({maal:N2} mål)";
        }

        private void ClearMatrikkelOverlay()
        {
            MatrikkelPolygonPath = Array.Empty<Location>();
            MatrikkelAreaText = string.Empty;
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
