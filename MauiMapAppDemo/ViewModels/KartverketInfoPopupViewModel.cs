using MauiMapAppDemo.Services;
using System.Globalization;

namespace MauiMapAppDemo.ViewModels
{
    public sealed record KartverketPopupRow(string Icon, string Label, string Value, string Description);

    public sealed record KartverketPopupExplanation(string Icon, string Label, string Description);

    public sealed class KartverketInfoPopupViewModel
    {
        public KartverketInfoPopupViewModel(double latitude, double longitude, KartverketPunktResponse? response, KartverketOmraadeResponse? omraadeResponse)
        {
            var hit = response?.Eiendom?.FirstOrDefault();
            var totalHits = response?.Metadata?.TotaltAntallTreff ?? 0;

            Title = "🗺️ Kartverket matrikkel";
            Subtitle = "Et kort sammendrag av treffet og hva feltene betyr.";

            if (hit == null)
            {
                StatusText = totalHits > 0
                    ? $"🔎 {totalHits} treff i Kartverket, men ingen entydig eiendom ble valgt."
                    : "🙈 Ingen matrikkeldata ble funnet for de oppgitte koordinatene.";
            }
            else
            {
                StatusText = $"✅ {totalHits} treff i Kartverket, vist med det nærmeste treffet først.";
            }

            SummaryRows = BuildSummaryRows(latitude, longitude, hit, totalHits, omraadeResponse);
            DetailRows = BuildDetailRows(response, hit, omraadeResponse);
            Explanations = BuildExplanations();
        }

        public string Title { get; }

        public string Subtitle { get; }

        public string StatusText { get; }

        public IReadOnlyList<KartverketPopupRow> SummaryRows { get; }

        public IReadOnlyList<KartverketPopupRow> DetailRows { get; }

        public IReadOnlyList<KartverketPopupExplanation> Explanations { get; }

        public bool HasDetailRows => DetailRows.Count > 0;

        private static IReadOnlyList<KartverketPopupRow> BuildSummaryRows(
            double latitude,
            double longitude,
            Punkt? hit,
            int totalHits,
            KartverketOmraadeResponse? omraadeResponse)
        {
            var rows = new List<KartverketPopupRow>
            {
                new(
                    "📍",
                    "Koordinater",
                    $"{latitude.ToString("F6", CultureInfo.InvariantCulture)}, {longitude.ToString("F6", CultureInfo.InvariantCulture)}",
                    "Koordinaten du klikket på i kartet."),
                new(
                    "📊",
                    "Totalt antall treff",
                    totalHits.ToString(CultureInfo.InvariantCulture),
                    "Hvor mange matrikkelobjekter Kartverket fant i radiusen."),
                new(
                    "🎯",
                    "Nøyaktighet",
                    hit?.Noyaktighetsklasseteig ?? "<ukjent>",
                    "Kartverkets grove kvalitetsindikator for stedfestingen."),
                new(
                    "🧭",
                    "Objekttype",
                    hit?.Objekttype ?? "<ukjent>",
                    "Hvilken type matrikkelobjekt som ble returnert."),
                new(
                    "🏷️",
                    "Matrikkelnummer",
                    hit?.Matrikkelnummertekst ?? "<ingen treff>",
                    "Den offisielle eiendomsidentifikatoren."),
                new(
                    "🏷️",
                    "Fullstendig Matrikkelnummer",
                    hit?.FullstendigMatrikkelNummer ?? "<ingen treff>",
                    "Den offisielle eiendomsidentifikatoren."),
                new(
                    "📏",
                    "Avstand fra punkt",
                    hit == null ? "<ukjent>" : $"{hit.MeterFraPunkt} m",
                    "Hvor mange meter treffet ligger fra klikkpunktet."),
                new(
                    " 📐",
                    " Beregnet areal (GeoJSON)",
                    hit == null || omraadeResponse?.Features?.Any() != true ? "ukjent" : $"{omraadeResponse.TotalAreaOfAllAreas}  m² (kvm. sq.metres)",
                    "Størrelse på eiendom utregnet fra polygon")
            };

            return rows;
        }

        private static IReadOnlyList<KartverketPopupRow> BuildDetailRows(KartverketPunktResponse? response, Punkt? hit, KartverketOmraadeResponse? kartverketOmraadeResponse)
        {
            var rows = new List<KartverketPopupRow>();

            if (hit != null)
            {
                rows.Add(new KartverketPopupRow("📐", "Beregnet areal", kartverketOmraadeResponse?.TotalAreaOfAllAreas.HasValue == true ? kartverketOmraadeResponse.TotalAreaOfAllAreas.Value.ToString("F1") : "<ukjent>", "Beregnet areal av eiendommen"));
                rows.Add(new KartverketPopupRow("📐", "Beregnet areal i mål (1000 kvm)", kartverketOmraadeResponse?.TotalAreaOfAllAreas.HasValue == true ? (kartverketOmraadeResponse.TotalAreaOfAllAreas.Value / 1000.0).ToString("F1") : "<ukjent>", "Beregnet areal av eiendommen"));
                rows.Add(new KartverketPopupRow("🏛️", "Kommunenummer", hit.Kommunenummer ?? "<ukjent>", "Kommunekoden til eiendommen."));
                rows.Add(new KartverketPopupRow("📌", "Gårdsnummer", hit.Gardsnummer.ToString(CultureInfo.InvariantCulture), "Hoveddelen av matrikkelnummeret."));
                rows.Add(new KartverketPopupRow("🧱", "Bruksnummer", hit.Bruksnummer.ToString(CultureInfo.InvariantCulture), "Undernummeret i matrikkelnummeret."));
                rows.Add(new KartverketPopupRow("🧩", "Festenummer", hit.Festenummer.ToString(CultureInfo.InvariantCulture), "Nummer for festet grunn."));
                rows.Add(new KartverketPopupRow("🔢", "Seksjonsnummer", hit.Seksjonsnummer.ToString(CultureInfo.InvariantCulture), "Seksjon i borettslag/sameie når det er relevant."));
                rows.Add(new KartverketPopupRow("🏘️", "Fullstendig matrikkelnummer", hit.FullstendigMatrikkelNummer.ToString(CultureInfo.InvariantCulture), "Fullstendig matrikkelnummer. Unikt på nasjonalt nivå."));
                rows.Add(new KartverketPopupRow("🆔", "Lokalid", hit.Lokalid.ToString(CultureInfo.InvariantCulture), "Intern identifikator i Kartverket."));
                rows.Add(new KartverketPopupRow("🗺️", "Representasjonspunkt", hit.Representasjonspunkt == null ? "<ukjent>" : $"{hit.Representasjonspunkt.Nord.ToString("F6", CultureInfo.InvariantCulture)}, {hit.Representasjonspunkt.Ost.ToString("F6", CultureInfo.InvariantCulture)}", "Kartverkets representasjon av eiendommen."));
                rows.Add(new KartverketPopupRow("🧪", "Teig med flere matrikkelenheter", ToYesNo(hit.Teigmedflerematrikkelenheter), "Om teigen deles av flere registrerte enheter."));
                rows.Add(new KartverketPopupRow("🧷", "Uregistrert jordsameie", ToYesNo(hit.Uregistrertjordsameie), "Om registrerte nummer har andel i samme teig."));
                rows.Add(new KartverketPopupRow("🕒", "Oppdateringsdato", hit.Oppdateringsdato?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? "<ukjent>", "Når objektet sist ble oppdatert."));
            }

            if (response?.Metadata != null)
            {
                rows.Add(new KartverketPopupRow("🧾", "Søkestreng", response.Metadata.SokeStreng ?? "<ukjent>", "Spørringen som ble sendt til Kartverket."));
                rows.Add(new KartverketPopupRow("🔎", "Side", response.Metadata.Side.ToString(CultureInfo.InvariantCulture), "Hvilken resultatside som er returnert."));
                rows.Add(new KartverketPopupRow("↔️", "Viser fra", response.Metadata.ViserFra.ToString(CultureInfo.InvariantCulture), "Første treff som vises i returlisten."));
                rows.Add(new KartverketPopupRow("↔️", "Viser til", response.Metadata.ViserTil.ToString(CultureInfo.InvariantCulture), "Siste treff som vises i returlisten."));
                rows.Add(new KartverketPopupRow("📃", "Treff per side", response.Metadata.TreffPerSide.ToString(CultureInfo.InvariantCulture), "Hvor mange treff som er inkludert per side."));
            }

            return rows;
        }

        private static IReadOnlyList<KartverketPopupExplanation> BuildExplanations()
        {
            return new[]
            {
                new KartverketPopupExplanation("🏘️", "Fullstendig matrikkelnummer", "Unikt på nasjonalt nivå. Formatet er følgende: Kommunenummer / Gårdsnummer (gnr) / Bruksnummer (bnr) Festenummer (frn) Seksjonsnummer (snr). De to sistnevnte kan mangle hvis det ikke er hhvs festegrunn eller eierseksjon. Manglende verdier for de to siste kan enten være tom verdi eller tallet 0."),
                new KartverketPopupExplanation("🏷️", "Matrikkelnummer", "Den offisielle identifikatoren for eiendommen, for eksempel 48/5."),
                new KartverketPopupExplanation("📌", "Gardsnummer og bruksnummer", "Gardsnummeret er hovednummeret. Bruksnummeret peker på eiendommen innenfor gården."),
                new KartverketPopupExplanation("🧩", "Festenummer og seksjonsnummer", "Festenummer brukes for festet grunn. Seksjonsnummer brukes når eiendommen er seksjonert."),
                new KartverketPopupExplanation("🎯", "Nøyaktighet", "Kartverkets enkle kvalitetsmerking. Gult betyr at det bør sjekkes nærmere."),
                new KartverketPopupExplanation("🗺️", "Representasjonspunkt", "Punktet Kartverket bruker som kartnær plassering av treffet."),
            };
        }

        private static string ToYesNo(bool value)
        {
            return value ? "Ja" : "Nei";
        }
    }
}