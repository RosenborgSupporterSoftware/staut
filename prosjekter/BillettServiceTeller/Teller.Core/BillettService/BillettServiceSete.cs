using System;
using System.Diagnostics;
using Teller.Core.Entities;

namespace Teller.Core.BillettService
{
    /// <summary>
    /// Et DTO med info pr. sete på stadion
    /// </summary>
    [DebuggerDisplay("{SectionName}: Row {RowName}, Seat {SeatName} - {EttCode}")]
    public class BillettServiceSete
    {
        public string Position { get { return String.Format("{0}:{1}:{2}", SectionName, RowName, SeatName); } }

        public string SectionId { get; set; }
        public string SectionName { get; set; }
        public string SectionTag { get; set; }

        public string RowName { get; set; }
        public string SeatName { get; set; }
        public EttCode EttCode { get; set; }
    }
}
