namespace Teller.Core
{
    public enum SeatStatus
    {
        /// <summary>
        /// Ukjent status - vi vet virkelig ikke hva den betyr
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Setet er til salgs
        /// </summary>
        AvailableForPurchase,

        /// <summary>
        /// Noen lukter på å kjøpe plassen
        /// </summary>
        HeldByTicketMasterApplication,

        /// <summary>
        /// Noen har sesongkort på setet
        /// </summary>
        SeasonTicket,

        /// <summary>
        /// Noen har kjøpt setet for gjeldende kamp
        /// </summary>
        Sold,

        /// <summary>
        /// Setet er holdt av til ett eller annet formål
        /// </summary>
        Reserved,

        /// <summary>
        /// Setet er utilgjengelig, uten at vi vet om det er solgt, reservert eller annet.
        /// </summary>
        Unavailable
    }
}
