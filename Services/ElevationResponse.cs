using System.Security.Cryptography.X509Certificates;

namespace MauiMapAppDemo.Services
{
    public class ElevationResponse
    {

        public List<ElevationResult> Results { get; set; } = [];

        public string Status { get; set; } = string.Empty;

        public class ElevationResult
        {
            public double Elevation { get; set; }

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