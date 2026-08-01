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
        /// <param name="transformToMetricCoordinateSystem">
        /// Transforms WGS84 longitude and latitude into UTM Zone 33N before running the area formula.
        /// Leave this enabled when the input coordinates are geographic degrees, and set it to false if the ring is already projected in meters.
        /// </param>
        /// <returns>The polygon area.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when fewer than three points are provided.
        /// </exception>
        /// <remarks>
        /// Kartverket GeoJSON polygons use the first ring as the exterior boundary and any later rings as holes.
        /// That convention matters because the shoelace formula returns signed area for each ring.
        /// </remarks>
        public static double CalculatePolygonArea(double[][] ring, bool transformToMetricCoordinateSystem = true)
        {
            if (ring == null || ring.Length < 3)
            {
                throw new ArgumentException("Polygon must contain at least three points.", nameof(ring));
            }

            var projectedRing = new double[ring.Length][];

            if (transformToMetricCoordinateSystem)
            {
                // Transform WGS84 longitude/latitude into UTM Zone 33N so the shoelace formula works in meters.
                var ctFactory = new CoordinateTransformationFactory();

                var wgs84 = GeographicCoordinateSystem.WGS84;
                var utm33 = ProjectedCoordinateSystem.WGS84_UTM(33, true);
                var transform = ctFactory.CreateFromCoordinateSystems(wgs84, utm33);

                for (int i = 0; i < ring.Length; i++)
                {
                    var projected = transform.MathTransform.Transform(ring[i][0], ring[i][1]);
                    projectedRing[i] = new[]
                    {
                        projected.x,
                        projected.y

                    };
                }
            }
            else
            {
                ring.CopyTo(projectedRing);
            }

            double area = 0;

            for (int i = 0; i < projectedRing.Length; i++)
            {
                int next = (i + 1) % projectedRing.Length;

                area += projectedRing[i][0] * projectedRing[next][1];
                area -= projectedRing[next][0] * projectedRing[i][1];
            }

            var calculatedArea = Math.Abs(area) / 2.0;
            return Math.Round(calculatedArea, 3);
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
        /// <param name="transformToMetricCoordinateSystem">
        /// Transforms WGS84 longitude and latitude into UTM Zone 33N before running the area formula.
        /// Leave this enabled when the input coordinates are geographic degrees, and set it to false if the rings are already projected in meters.
        /// </param>
        /// <returns>The total area.</returns>
        /// <remarks>
        /// The first ring is treated as the exterior boundary and all subsequent rings are treated as interior holes.
        /// This is the GeoJSON convention used by Kartverket for polygon geometry.
        /// </remarks>
        public static double CalculateTotalArea(double[][][] coordinates, bool transformToMetricCoordinateSystem = true)
        {
            if (coordinates == null || coordinates.Length == 0)
            {
                return 0;
            }

            double totalArea = 0;

            for (int ringIndex = 0; ringIndex < coordinates.Length; ringIndex++)
            {
                var ringArea = CalculatePolygonArea(coordinates[ringIndex], transformToMetricCoordinateSystem);

                // GeoJSON ring 0 is the exterior polygon; later rings subtract out holes.
                if (ringIndex == 0)
                {
                    totalArea += ringArea;
                }
                else
                {
                    totalArea -= ringArea;
                }
            }

            var area = Math.Round(totalArea, 3);
            return area;
        }

    }
}
