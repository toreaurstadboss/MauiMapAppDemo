using System.Text.Json.Serialization;

namespace MauiMapAppDemo.Services
{

    public class KartverketOmraadeResponse
    {
        [JsonPropertyName("features")]
        public Feature[] Features { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public double? TotalAreaOfAllAreas
        {
            get
            {
                try
                {
                    double totalArea = Features.Sum(f => GeometryUtils.CalculateTotalArea(f.Geometry.Coordinates));
                    return totalArea;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }

    public class Feature
    {
        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; }

        [JsonPropertyName("properties")]
        public Properties Properties { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

    }

    public class Geometry
    {
        [JsonPropertyName("coordinates")]
        public double[][][] Coordinates { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public double? TotalArea
        {
            get
            {
                try
                {
                    double totalArea = GeometryUtils.CalculateTotalArea(Coordinates);
                    return totalArea;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }

    public class Properties
    {
        [JsonPropertyName("bruksnummer")]
        public int Bruksnummer { get; set; }

        [JsonPropertyName("festenummer")]
        public int Festenummer { get; set; }

        [JsonPropertyName("gardsnummer")]
        public int Gardsnummer { get; set; }

        [JsonPropertyName("hovedområde")]
        public bool HovedOmråde { get; set; }

        [JsonPropertyName("kommunenummer")]
        public string KommuneNummer { get; set; }

        [JsonPropertyName("lokalid")]
        public long LokalId { get; set; }

        [JsonPropertyName("matrikkelnummertekst")]
        public string MatrikkelNummerTekst { get; set; }

        [JsonPropertyName("meterFraPunkt")]
        public int MeterFraPunkt { get; set; }

        [JsonPropertyName("nøyaktighetsklasseteig")]
        public string NøyaktighetsKlasseTeig { get; set; }

        [JsonPropertyName("objekttype")]
        public string ObjektType { get; set; }

        [JsonPropertyName("oppdateringsdato")]
        public DateTime OppdateringsDato { get; set; }

        [JsonPropertyName("seksjonsnummer")]
        public int SeksjonsNummer { get; set; }

        [JsonPropertyName("teigmedflerematrikkelenheter")]
        public bool TeigMedFlereMatrikkelenheter { get; set; }

        [JsonPropertyName("uregistrertjordsameie")]
        public bool UregistrertJordsameie { get; set; }
    }

}

