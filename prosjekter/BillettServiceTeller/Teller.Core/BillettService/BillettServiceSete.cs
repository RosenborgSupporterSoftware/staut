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
        private string _position;

        public string Position
        {
            get
            {
                if(String.IsNullOrWhiteSpace(_position))
                    _position = $"{SectionName}:{RowName}:{SeatName}";
                return _position;
            }
        }

        public string SectionId { get; set; }
        public string SectionName { get; set; }
        public string SectionTag { get; set; }

        public string RowName { get; set; }
        public string SeatName { get; set; }
        public EttCode EttCode { get; set; }
    }
}
