using Teller.Core;
using Teller.Core.Entities;

namespace PowerSTAut
{
    public class SeatCodeSummary
    {
        public int Count { get; set; }
        public EttCode EttCode { get; set; }
        public SeatStatus SeatStatus { get; set; }
    }
}
