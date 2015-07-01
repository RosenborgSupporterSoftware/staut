using System;
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
            : this(ettCode, Convert.ToInt32(count))
        {
        }

        public SeatSummary(string ettCode, int count)
        {
            EttCode = new EttCode(ettCode);
            Count = count;
        }
    }
}
