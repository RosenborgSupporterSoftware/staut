using System.Collections.Generic;
using System.IO;
using System.Linq;
using Teller.Core.BillettService;

namespace Teller.Core.Export
{
    public class CsvExport
    {
        public void ExportCsv(string filename, IEnumerable<BillettServiceSete> seter)
        {
            // En linje per tribuneseksjon
            var seksjoner =
                seter.GroupBy(s => new {Section = s.SectionName, Status = SeatStatusClassifier.Classify(s.EttCode)})
                     .Select(g => new {g.Key.Section, g.Key.Status, Count = g.Count()})
                     .GroupBy(g => g.Section)
                     .Select(g => new
                     {
                         Section = g.Key,
                         AllSold = g.Where(s => s.Status == SeatStatus.Sold || s.Status == SeatStatus.SeasonTicket)
                                    .Sum(s => s.Count),
                         Available = g.Where(s => s.Status == SeatStatus.AvailableForPurchase)
                                      .Sum(s => s.Count),
                         Reserved = g.Where(s => s.Status == SeatStatus.Reserved)
                                      .Sum(s => s.Count),
                         Unavailable = g.Where(s => s.Status == SeatStatus.Unavailable)
                                      .Sum(s => s.Count),
                         Unknown = g.Where(s => s.Status == SeatStatus.Unknown)
                                      .Sum(s => s.Count)
                     })
                     .ToList();

            var file = new StreamWriter(filename);
            file.WriteLine("TribuneSeksjon,Solgte,Tilgjengelig,Reservert,Utilgjengelig,Ukjent");

            foreach (var seksjon in seksjoner)
            {
                file.WriteLine("{0},{1},{2},{3},{4},{5}", seksjon.Section, seksjon.AllSold, seksjon.Available,
                    seksjon.Reserved, seksjon.Unavailable, seksjon.Unknown);
            }

            file.Flush();
            file.Close();
        }
    }
}
