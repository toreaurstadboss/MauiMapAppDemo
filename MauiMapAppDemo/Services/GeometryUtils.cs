using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace MauiMapAppDemo.Services
{

    /// <summary>
    /// Utility methods for calculating polygon areas.
    /// </summary>
    public static class GeometryUtils
    {
        /// <summary>
        /// Calculates the area of a polygon using the shoelace formula.
        /// For a visual presentation of the Shoelace formula, see this page:
        /// https://courseware.cemc.uwaterloo.ca/42/143/assignments/1140/0
        /// </summary>
        /// <param name="ring">
        /// Polygon ring coordinates in the format:
        /// [[x1, y1], [x2, y2], ...].
        /// The ring should be in a projected coordinate system (meters)
        /// for the result to be returned in square meters.
        /// </param>
        /// <param name="transformToMetricCoordinateSystem">Because coordinates are usually from MAP, we need to transform for metric coordinate system.
        /// Default, WGS 84 is used</param>
        /// <returns>The polygon area.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than three points are provided.
        /// </exception>
        public static double CalculatePolygonArea(double[][] ring, bool transformToMetricCoordinateSystem = true)
        {
            if (ring == null || ring.Length < 3)
            {
                throw new ArgumentException("Polygon must contain at least three points.", nameof(ring));
            }

            if (transformToMetricCoordinateSystem)
            {
                //Transform to WGS 84 metric system necessary, using Universal Transverse Mercator UTM-33 and zone is north
                var csFactory = new CoordinateSystemFactory();
                var ctFactory = new CoordinateTransformationFactory();

                var wgs84 = GeographicCoordinateSystem.WGS84;
                var utm33 = ProjectedCoordinateSystem.WGS84_UTM(33, true);
                var transform = ctFactory.CreateFromCoordinateSystems(wgs84, utm33);

                for (int i = 0; i < ring.Length; i++)
                {
                    var projected = transform.MathTransform.Transform(ring[i][0], ring[i][1]);
                    ring[i][0] = projected.x;
                    ring[i][1] = projected.y;
                }
            }

            double area = 0;

            for (int i = 0; i < ring.Length; i++)
            {
                int next = (i + 1) % ring.Length;

                area += ring[i][0] * ring[next][1];
                area -= ring[next][0] * ring[i][1];
            }

            return Math.Abs(area) / 2.0;
        }

        /// <summary>
        /// Calculates the total area of all rings in a GeoJSON polygon.
        /// For a visual presentation of the Shoelace formula, see this page:
        /// https://courseware.cemc.uwaterloo.ca/42/143/assignments/1140/0
        /// </summary>
        /// <param name="coordinates">
        /// GeoJSON polygon coordinates in the format:
        /// coordinates[ring][point][x/y].
        /// </param>
        ///   /// <param name="transformToMetricCoordinateSystem">Because coordinates are usually from MAP, we need to transform for metric coordinate system.
        /// Default, WGS 84 is used</param>
        /// <returns>The total area.</returns>
        public static double CalculateTotalArea(double[][][] coordinates, bool transformToMetricCoordinateSystem = true)
        {
            if (coordinates == null || coordinates.Length == 0)
            {
                return 0;
            }

            double totalArea = 0;

            foreach (var ring in coordinates)
            {
                totalArea += CalculatePolygonArea(ring);
            }

            return totalArea;
        }

    }
}
