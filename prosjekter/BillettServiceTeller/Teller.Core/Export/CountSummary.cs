using System.Text;
using Teller.Core.BillettService;

namespace Teller.Core.Export
{
    public class CountSummary
    {
        public string CreateCountSummary(BillettServiceXmlFile ticketFile)
        {
            var stats = new SummaryGenerator().CreateSummary(ticketFile);

            var builder = new StringBuilder();
            var sum = 0;
            var tilskuere = 0;

            foreach (var kvp in stats)
            {
                builder.AppendLine(kvp.Key.ToString().PadRight(30) + kvp.Value.ToString().PadLeft(5));
                sum += kvp.Value;
                if (kvp.Key == SeatStatus.Sold || kvp.Key == SeatStatus.SeasonTicket)
                    tilskuere += kvp.Value;
            }
            builder.AppendLine("-----------------------------------");
            builder.AppendLine("Tilskuere                     " + tilskuere.ToString().PadLeft(5));
            builder.AppendLine("Totale seter                  " + sum.ToString().PadLeft(5));

            return builder.ToString();
        }
    }
}

