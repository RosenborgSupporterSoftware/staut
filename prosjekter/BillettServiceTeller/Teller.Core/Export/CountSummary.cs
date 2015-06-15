using System.Linq;
using System.Text;
using Teller.Core.BillettService;

namespace Teller.Core.Export
{
    public class CountSummary
    {
        public string CreateCountSummary(BillettServiceXmlFile ticketFile)
        {
            var leser = new BillettServiceSeteLeser();
            var seats = leser.ReadSeatSummary(ticketFile);
            var stats = seats.Select(s => new { Code = SeatStatusClassifier.Classify(s.EttCode), Seats = s.Count })
                             .GroupBy(s => s.Code)
                             .Select(g => new { Status = g.Key, Count = g.Sum(f => f.Seats) })
                             .OrderByDescending(g => g.Count)
                             .ToList();
            var builder = new StringBuilder();
            var sum = 0;
            var tilskuere = 0;

            foreach (var item in stats)
            {
                builder.AppendLine(item.Status.ToString().PadRight(30) + item.Count.ToString().PadLeft(5));
                sum += item.Count;
                if (item.Status == SeatStatus.Sold || item.Status == SeatStatus.SeasonTicket)
                    tilskuere += item.Count;
            }
            builder.AppendLine("-----------------------------------");
            builder.AppendLine("Tilskuere                     " + tilskuere.ToString().PadLeft(5));
            builder.AppendLine("Totale seter                  " + sum.ToString().PadLeft(5));

            return builder.ToString();
        }
    }
}
