using System.Text;

namespace MauiMapAppDemo.Services
{

    public class GeocodingService
    {

        public async Task<string?> GetGeocodingPlacemark(double latitude, double longitude)
        {
            var placemark = (await Geocoding.GetPlacemarksAsync(latitude, longitude))?.FirstOrDefault();
            if (placemark == null)
            {
                return null;
            }

            var placemarkDescription = GetPlacementDescription(placemark);
            return placemarkDescription;            
        }

        private string? GetPlacementDescription(Placemark placemark)
        {
            var sb = new StringBuilder(); 

            AddIfAvailable(nameof(placemark.FeatureName), placemark.FeatureName);
            AddIfAvailable(nameof(placemark.Thoroughfare), placemark.Thoroughfare);
            AddIfAvailable(nameof(placemark.SubThoroughfare), placemark.SubThoroughfare);
            AddIfAvailable(nameof(placemark.Locality), placemark.Locality);
            AddIfAvailable(nameof(placemark.SubLocality), placemark.SubLocality);
            AddIfAvailable(nameof(placemark.AdminArea), placemark.AdminArea);
            AddIfAvailable(nameof(placemark.SubAdminArea), placemark.SubAdminArea);
            AddIfAvailable(nameof(placemark.PostalCode), placemark.PostalCode);
            AddIfAvailable(nameof(placemark.CountryName), placemark.CountryName);
            AddIfAvailable(nameof(placemark.CountryCode), placemark.CountryCode);

            return sb.ToString();

            void AddIfAvailable(string name, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    sb.AppendLine($"{name}: {value}");
            }

        }




    }

}
