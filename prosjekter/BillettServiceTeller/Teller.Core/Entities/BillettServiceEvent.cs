using System;
using System.Collections.Generic;

namespace Teller.Core.Entities
{
    /// <summary>
    /// En klasse som representerer en BillettService-"event" - i praksis en kamp for vår del
    /// </summary>
    public class BillettServiceEvent
    {
        #region Properties

        /// <summary>
        /// Database id for denne eventen. 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// BillettService sin egen id på arrangementet
        /// </summary>
        public int EventNumber { get; set; }

        /// <summary>
        /// BillettService sin event-kode på arrangementet, f.eks. "TLD0915-2015" (Tippeligaen LerkenDal tippeligaen runde 9/15 - 2015)
        /// </summary>
        public string EventCode { get; set; }

        /// <summary>
        /// Displaynavn på eventen, typisk "Rosenborg - Fiendelag [ev. turneringsinfo]"
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Turneringen denne kampen hører til
        /// </summary>
        public string Tournament { get; set; }

        /// <summary>
        /// Event-nummer i denne ligaen (teller kun kamper på denne lokasjonen i denne turneringen, bortekamper teller ikke med)
        /// </summary>
        public int Round { get; set; }

        /// <summary>
        /// Sesongen denne kampen hører til
        /// </summary>
        public string Season { get; set; }

        /// <summary>
        /// Lokasjon denne kampen spilles (TLD = Lerkendal, f.eks.)
        /// </summary>
        public string Location { get; set; }

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
        /// URL for fil som viser layout på stadion (brukt av BS sin app)
        /// </summary>
        public string GeometryUrl { get; set; }

        /// <summary>
        /// Vårt endelige estimat på hvor mange plasser som ble solgt
        /// </summary>
        public int FinalEstimatedSeatCount { get; set; }

        /// <summary>
        /// Tallet klubben rapporterer på solgte plasser
        /// </summary>
        public int OfficialSeatCount { get; set; }

        /// <summary>
        /// Alle målinger foretatt på denne eventen
        /// </summary>
        public virtual List<Measurement> Measurements { get; set; }

        #endregion
    }
}
