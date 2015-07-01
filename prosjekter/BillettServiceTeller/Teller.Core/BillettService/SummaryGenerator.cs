using System.Collections.Generic;
using System.Linq;

namespace Teller.Core.BillettService
{
    /// <summary>
    /// En klasse som summerer opp setestatuser i en XML-fil
    /// </summary>
    public class SummaryGenerator
    {
        /// <summary>
        /// Lag en oppsummering av salgsstatus fra en XML-fil
        /// </summary>
        /// <param name="ticketFile">Fila vi skal lese fra</param>
        /// <returns>En dictionary med setestatus og antall seter</returns>
        public IDictionary<SeatStatus, int> CreateSummary(BillettServiceXmlFile ticketFile)
        {
            var leser = new BillettServiceSeteLeser();
            var stats = leser.ReadSeatSummary(ticketFile)
                             .Select(s => new { Code = SeatStatusClassifier.Classify(s.EttCode), Seats = s.Count })
                             .GroupBy(s => s.Code)
                             .Select(g => new { Status = g.Key, Count = g.Sum(f => f.Seats) })
                             .OrderByDescending(g => g.Count)
                             .ToDictionary(arg => arg.Status, arg => arg.Count);

            return stats;
        }
    }
}