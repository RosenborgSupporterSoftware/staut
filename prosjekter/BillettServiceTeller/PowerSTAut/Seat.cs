using System;
using Teller.Core;
using Teller.Core.BillettService;
using Teller.Core.Entities;

namespace PowerSTAut
{
    /// <summary>
    /// En klasse som er laget for å lette scripting
    /// </summary>
    public class Seat
    {
        public string Position { get; private set; }

        public string SectionName { get; set; }

        public string RowName { get; set; }
        public string SeatName { get; set; }
        public EttCode EttCode { get; set; }

        public SeatStatus Classification { get; set; }

        public Seat(BillettServiceSete seat)
        {
            SectionName = seat.SectionName;
            RowName = seat.RowName;
            SeatName = seat.SeatName;
            EttCode = seat.EttCode;
            Position = $"{SectionName}:{RowName}:{SeatName}";

            Classification = SeatStatusClassifier.Classify(EttCode);
        }
    }
}
