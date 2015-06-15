using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teller.Core.BillettService
{
    public class SeatChange
    {
        public BillettServiceSete SeatA { get; private set; }
        public BillettServiceSete SeatB { get; private set; }

        public string FromEtt { get { return SeatA.EttCode; } }
        public string ToEtt { get { return SeatB.EttCode; } }

        public SeatChange(BillettServiceSete seatA, BillettServiceSete seatB)
        {
            SeatA = seatA;
            SeatB = seatB;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2} {3} -> {4}", SeatA.SectionName, SeatA.RowName, SeatA.SeatName, FromEtt,
                ToEtt);
        }
    }
}
