using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

#if ANDROID
using Android.App;
using Android.Content.PM;
#endif

namespace MauiMapAppDemo.Services
{
    /// <summary>
    /// Elevation service backed by the Google Elevation API.
    /// </summary>
    public sealed class GoogleElevationService : IElevationService
    {
        private readonly HttpClient _httpClient = new();
        private readonly string _googleMapsKey;

        public GoogleElevationService(IConfiguration configuration)
        {
            _googleMapsKey =
                configuration["GoogleElevationApiKey"]
                ?? configuration["GoogleMapsKey"]
                ?? GetAndroidManifestGoogleMapsKey()
                ?? string.Empty;
        }

        public string ProviderName => "Google";

        /// <summary>
        /// Gets the elevation in meters for a coordinate pair from Google Elevation API.
        /// </summary>
        public async Task<double?> GetElevationAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_googleMapsKey))
            {
                return null;
            }

            var url =
                $"https://maps.googleapis.com/maps/api/elevation/json?locations={latitude},{longitude}&key={_googleMapsKey}";

            var response = await _httpClient.GetFromJsonAsync<GoogleElevationResponse>(url, cancellationToken);

            var elevation = response?.Results?.FirstOrDefault()?.Elevation;
            return elevation.HasValue ? Math.Round(elevation.Value * 2, MidpointRounding.AwayFromZero) / 2.0 : null;
        }

        private sealed class GoogleElevationResponse
        {
            [JsonPropertyName("results")]
            public List<GoogleElevationResult>? Results { get; set; }

            [JsonPropertyName("status")]
            public string? Status { get; set; }
        }

        private sealed class GoogleElevationResult
        {
            [JsonPropertyName("elevation")]
            public double? Elevation { get; set; }
        }

#if ANDROID
        private static string? GetAndroidManifestGoogleMapsKey()
        {
            var context = Android.App.Application.Context;
            var applicationInfo = context.PackageManager?.GetApplicationInfo(context.PackageName, PackageInfoFlags.MetaData);

            return applicationInfo?.MetaData?.GetString("com.google.android.geo.API_KEY");
        }
#else
        private static string? GetAndroidManifestGoogleMapsKey()
        {
            return null;
        }
#endif
    }
}