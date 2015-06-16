using System;

namespace Teller.Core.Entities
{
    /// <summary>
    /// En klasse som representerer en BillettService-"event" - i praksis en kamp for vår del
    /// </summary>
    public class BillettServiceEvent
    {
        #region Properties

        /// <summary>
        /// BillettService sin egen id på arrangementet
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Navnet på motstanderen i kampen
        /// </summary>
        public string Opponent { get; set; }

        /// <summary>
        /// Starttidspunkt for kampen
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// URL for selve eventet - typisk siden der en vanlig bruker bestiller billetter
        /// </summary>
        public string EventUrl { get; set; }

        /// <summary>
        /// URL for datafila til BillettService med setedata
        /// </summary>
        public string AvailibilityUrl { get; set; }

        /// <summary>
        /// Vårt endelige estimat på hvor mange plasser som ble solgt
        /// </summary>
        public string FinalEstimatedSeatCount { get; set; }

        /// <summary>
        /// Tallet klubben rapporterer på solgte plasser
        /// </summary>
        public int OfficialSeatCount { get; set; }

        #endregion

    }
}
