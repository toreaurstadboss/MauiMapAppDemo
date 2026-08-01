using System.Text;
using System.Text.Json.Serialization;

namespace MauiMapAppDemo.Services
{
    public class KartverketPunktResponse
    {
        [JsonPropertyName("eiendom")]
        public List<Punkt>? Eiendom { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata? Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("side")]
        public int Side { get; set; }

        [JsonPropertyName("sokeStreng")]
        public string? SokeStreng { get; set; }

        [JsonPropertyName("totaltAntallTreff")]
        public int TotaltAntallTreff { get; set; }

        [JsonPropertyName("treffPerSide")]
        public int TreffPerSide { get; set; }

        [JsonPropertyName("viserFra")]
        public int ViserFra { get; set; }

        [JsonPropertyName("viserTil")]
        public int ViserTil { get; set; }
    }

    public class Punkt
    {
        [JsonPropertyName("bruksnummer")]
        public int Bruksnummer { get; set; }

        [JsonPropertyName("festenummer")]
        public int Festenummer { get; set; }

        [JsonPropertyName("gardsnummer")]
        public int Gardsnummer { get; set; }

        [JsonPropertyName("hovedområde")]
        public bool HovedOmrade { get; set; }

        [JsonPropertyName("kommunenummer")]
        public string? Kommunenummer { get; set; }

        [JsonPropertyName("lokalid")]
        public int Lokalid { get; set; }

        [JsonPropertyName("matrikkelnummertekst")]
        public string? Matrikkelnummertekst { get; set; }

        [JsonPropertyName("meterFraPunkt")]
        public int MeterFraPunkt { get; set; }

        [JsonPropertyName("nøyaktighetsklasseteig")]
        public string? Noyaktighetsklasseteig { get; set; }

        [JsonPropertyName("objekttype")]
        public string? Objekttype { get; set; }

        [JsonPropertyName("oppdateringsdato")]
        public DateTime? Oppdateringsdato { get; set; }

        [JsonPropertyName("representasjonspunkt")]
        public Representasjonspunkt? Representasjonspunkt { get; set; }

        [JsonPropertyName("seksjonsnummer")]
        public int Seksjonsnummer { get; set; }

        [JsonPropertyName("teigmedflerematrikkelenheter")]
        public bool Teigmedflerematrikkelenheter { get; set; }

        [JsonPropertyName("uregistrertjordsameie")]
        public bool Uregistrertjordsameie { get; set; }

        public string FullstendigMatrikkelNummer
        {
            get
            {
                var sb = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(Kommunenummer))
                {
                    sb.Append(Kommunenummer?.ToString().PadLeft(4, '0'));
                }
                if (Gardsnummer > 0)
                {
                    sb.Append("/" + Gardsnummer);
                }
                if (Bruksnummer > 0)
                {
                    sb.Append("/" + Bruksnummer);
                }
                if (Festenummer > 0)
                {
                    sb.Append("/" + Festenummer);
                }
                if (Seksjonsnummer > 0)
                {
                    sb.Append("/" + Seksjonsnummer);
                }

                return sb.ToString();
            }
        }
    }

    public class Representasjonspunkt
    {
        [JsonPropertyName("koordsys")]
        public int Koordsys { get; set; }

        [JsonPropertyName("nord")]
        public double Nord { get; set; }

        [JsonPropertyName("øst")]
        public double Ost { get; set; }
    }

}
