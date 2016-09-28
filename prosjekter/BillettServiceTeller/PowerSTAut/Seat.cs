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
        public string Position { get { return String.Format("{0}:{1}:{2}", SectionName, RowName, SeatName); } }

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

            Classification = SeatStatusClassifier.Classify(EttCode);
        }
    }
}
