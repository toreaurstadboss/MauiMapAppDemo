using System.Net.Http.Json;

namespace MauiMapAppDemo.Services
{
    /// <summary>
    /// This service is using Open Topo Data elevation API 
    /// </summary>
    /// <remarks>See https://www.opentopodata.org/ for more information about Open Topo Data</remarks>
    public class OpenTopoService : IElevationService
    {
        private readonly HttpClient _httpClient = new();

        public string ProviderName => "OpenTopoData";

        /// <summary>
        /// Retrieves the elevation response for a given latitude and longitude
        /// </summary>
        /// <param name="latitude">Latitude of point to get elevation for. Note that EU-DEM has +- 7m RMSE precision</param>
        /// <param name="longitude"></param>
        /// <param name="dataSetId">Eudem25m is default data set id. See https://www.opentopodata.org/datasets/eudem/ There are other data set Ids covering rest of the regions of the world, a global dataSetId is 'aster30m'</param>
        /// <returns>Elevation response (full response deserialized Json)</returns>
        public async Task<ElevationResponse?> GetElevationResponseAsync(double latitude, double longitude, string dataSetId = "eudem25m")
        {
            string url =
                $"https://api.opentopo.org/v1/{dataSetId}?locations={latitude},{longitude}";

            var response = await _httpClient.GetFromJsonAsync<ElevationResponse>(url);

            return response;
        }

        /// <summary>
        /// Retrieves the elevation if found for a given latitude and longitude. Returns the scalar elevation, if found.
        /// </summary>  
        /// <param name="latitude">Latitude of point to get elevation for. Note that EU-DEM has +- 7m RMSE precision</param>
        /// <param name="longitude"></param>
        /// <param name="dataSetId">Eudem25m is default data set id. See https://www.opentopodata.org/datasets/eudem/ There are other data set Ids covering rest of the regions of the world, a global dataSetId is 'aster30m'</param>
        /// <returns>Elevation scalar value if found (null if there is no elevation data or Open Topo API did not reply.)</returns>

        public async Task<double?> GetElevationAsync(double latitude, double longitude, string dataSetId = "eudem25m", CancellationToken cancellationToken = default)
        {
            string url =
                $"https://api.opentopodata.org/v1/eudem25m?locations={latitude},{longitude}";

            var response = await _httpClient.GetFromJsonAsync<ElevationResponse>(url, cancellationToken);

            var elevation = response?.Results?.FirstOrDefault()?.Elevation;
            return elevation.HasValue ? Math.Round(elevation.Value * 2, MidpointRounding.AwayFromZero) / 2.0 : null; //approximate to closest half meters
        }

        async Task<double?> IElevationService.GetElevationAsync(double latitude, double longitude, CancellationToken cancellationToken)
        {
            return await GetElevationAsync(latitude, longitude, cancellationToken: cancellationToken);
        }

    }
}
