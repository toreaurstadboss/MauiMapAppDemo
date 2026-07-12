namespace MauiMapAppDemo.Repositories.PinLocations
{
    public static partial class TrondheimCabins
    {
        public record Cabin(
            string Name,
            string Description,
            double Latitude,
            double Longitude);
    }
}
