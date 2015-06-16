using System;

namespace Teller.Core.Entities
{
    /// <summary>
    /// En klasse som representerer et "snapshot" av salget til en kamp
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Unik id for denne målingen
        /// </summary>
        public int MeasurementId { get; set; }

        /// <summary>
        /// Id på event som denne målingen hører til
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Tidspunktet denne målingen ble foretatt
        /// </summary>
        public DateTime MeasurementTime { get; set; }

        /// <summary>
        /// Antall enkeltseter som er solgt på dette tidspunktet
        /// </summary>
        public int AmountSold { get; set; }

        /// <summary>
        /// Antall sesongkort som er solgt på dette tidspunktet
        /// </summary>
        public int AmountSeasonTicket { get; set; }

        /// <summary>
        /// Totalt antall solgte seter - summen av Sold og SeasonTicket
        /// </summary>
        public int TotalAmountSold { get { return AmountSold + AmountSeasonTicket; } }

        /// <summary>
        /// Seter som er tilgjengelige for salg
        /// </summary>
        public int AmountAvailable { get; set; }

        /// <summary>
        /// Seter som er reservert for salg
        /// </summary>
        public int AmountReserved { get; set; }

        /// <summary>
        /// Seter som utilgjengelige
        /// </summary>
        public int AmountUnavailable { get; set; }

        /// <summary>
        /// Seter holdt av i flash-applikasjonen
        /// </summary>
        public int AmountTicketMaster { get; set; }

        /// <summary>
        /// Seter vi ikke har en god peiling på statusen til
        /// </summary>
        public int AmountUnknown { get; set; }
    }
}
