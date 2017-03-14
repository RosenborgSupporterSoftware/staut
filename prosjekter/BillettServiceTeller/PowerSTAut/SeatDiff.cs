using Teller.Core;
using Teller.Core.Entities;

namespace PowerSTAut
{
    /// <summary>
    /// En klasse som inneholder forskjellene på setestatus i to filer
    /// </summary>
    public class SeatDiff
    {
        /// <summary>
        /// Gets the position of the seat in question
        /// </summary>
        public string Position { get; private set; }

        public string SectionName { get; private set; }

        public string RowName { get; private set; }
        public string SeatName { get; private set; }
        public EttCode OriginalEttCode { get; private set; }
        public EttCode NewEttCode { get; private set; }

        public SeatStatus OriginalClassification { get; private set; }
        public SeatStatus NewClassification { get; private set; }
        
        public SeatDiff(Seat originalSeat, Seat newSeat)
        {
            Position = originalSeat.Position;
            SectionName = originalSeat.SectionName;
            RowName = originalSeat.RowName;
            SeatName = originalSeat.SeatName;
            OriginalEttCode = originalSeat.EttCode;
            NewEttCode = newSeat.EttCode;
            OriginalClassification = originalSeat.Classification;
            NewClassification = newSeat.Classification;
        }
    }
}
