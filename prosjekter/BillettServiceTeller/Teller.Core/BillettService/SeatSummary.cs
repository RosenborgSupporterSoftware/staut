using Teller.Core.Entities;

namespace Teller.Core.BillettService
{
    /// <summary>
    /// En klasse brukt til oppsummering av seteantall innen en gitt kode
    /// </summary>
    public class SeatSummary
    {
        public EttCode EttCode { get; private set; }
        public int Count { get; private set; }

        public SeatSummary(string ettCode, string count)
        {
            EttCode = new EttCode(ettCode);

            int parsedCount;
            if (int.TryParse(count, out parsedCount))
                Count = parsedCount;
        }
    }
}
