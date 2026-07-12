
using System.Diagnostics;

namespace MauiMapAppDemo.Services
{
    public class ElevationResponse
    {

        public List<ElevationResult> Results { get; set; } = [];

        public string Status { get; set; } = string.Empty;

        public class ElevationResult
        {
            /// <summary>
            /// Raw value of elevation. Since elevations can be queried in locations outside the data sets being used for the
            /// Open Topo API request, if null is returned, this means you have clicked outside the area the data set coverage of elevation data
            /// </summary>
            /// <remarks>For example, most data sets does not contain barymetric data (sea depths) , so clicking in a point in the ocean will give a non parseable value for the elevation</remarks>
            public double? Elevation { get; set; }

           
            public ElevationLocation Location { get; set; } 

            public string DataSet { get; set; }

        }

        public class ElevationLocation
        {
            public double Lat { get; set; }
            public double Long { get; set; }
        }

    }
    
}