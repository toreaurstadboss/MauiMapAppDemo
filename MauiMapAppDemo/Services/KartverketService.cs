using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace MauiMapAppDemo.Services
{

    public class KartverketService
    {

        private const string _apiKartverketEiendomV1BaseUrl = "https://api.kartverket.no/eiendom/v1/";
        private static readonly JsonSerializerOptions s_camelCaseJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_apiKartverketEiendomV1BaseUrl)
        };

        /// <summary>
        /// Retrieves matrikkel informasjon from given point (Punkt) from Kartverket's Eiendom API v1
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="koordSys">Defaulting here to EUREF89 = 4258 as the coordinate system id, which Kartverket uses and is also what Google maps coords are using</param>
        /// <returns></returns>
        public async Task<KartverketPunktResponse?> GetMatrikkelInformationFromLocationAsync(double latitude, double longitude, int koordSys = 4258)
        {
            string url = $"punkt?ost={longitude.ToString(CultureInfo.InvariantCulture)}&nord={latitude.ToString(CultureInfo.InvariantCulture)}&koordsys={koordSys}&radius=10&utkoordsys={koordSys}&treffPerSide=1&side=1";

            var kartverketResponseForLocation = await _httpClient.GetFromJsonAsync<KartverketPunktResponse>(url, options: s_camelCaseJsonOptions);
            return kartverketResponseForLocation;
        }

        /// <summary>
        /// Retrieves GeoJson from given point (Punkt) from Kartverket's Eiendom API v1.
        /// Please note that multiple feature are returned for the GeoJson, which are the Geometry Json coordinates.
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <param name="koordSys">Defaulting here to EUREF89 = 4258 as the coordinate system id, which Kartverket uses and is also what Google maps coords are using</param>
        /// <returns></returns>
        public async Task<KartverketOmraadeResponse?> GetGeoJsonFromLocationAsync(double latitude, double longitude, int koordSys = 4258)
        {
            string url = $"punkt/omrader?ost={longitude.ToString(CultureInfo.InvariantCulture)}&nord={latitude.ToString(CultureInfo.InvariantCulture)}&koordsys={koordSys}&radius=10&utkoordsys={koordSys}&treffPerSide=1&side=1";

            var kartverketResponseForLocation = await _httpClient.GetFromJsonAsync<KartverketOmraadeResponse>(url, options: s_camelCaseJsonOptions);
            return kartverketResponseForLocation;


        }

    }
}
