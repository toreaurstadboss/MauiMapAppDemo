namespace MauiMapAppDemo.Services
{
    /// <summary>
    /// Represents a service that can return elevation values for geographic coordinates.
    /// </summary>
    public interface IElevationService
    {
        /// <summary>
        /// Gets the elevation in meters for a coordinate pair.
        /// </summary>
        /// <param name="latitude">Latitude of the location.</param>
        /// <param name="longitude">Longitude of the location.</param>
        /// <param name="cancellationToken">Cancellation token for the request.</param>
        /// <returns>The elevation in meters, or null if the service cannot resolve a value.</returns>
        Task<double?> GetElevationAsync(double latitude, double longitude, CancellationToken cancellationToken = default);

        string ProviderName { get; }
    }
}