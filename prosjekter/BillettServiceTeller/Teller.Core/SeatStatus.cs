namespace Teller.Core
{
    public enum SeatStatus
    {
        /// <summary>
        /// Setet er til salgs
        /// </summary>
        AvailableForPurchase = 0,

        /// <summary>
        /// Noen lukter på å kjøpe plassen
        /// </summary>
        HeldByTicketMasterApplication = 30,

        /// <summary>
        /// Noen har sesongkort på setet
        /// </summary>
        SeasonTicket = 1,

        /// <summary>
        /// Noen har kjøpt setet for gjeldende kamp
        /// </summary>
        Sold = 2,

        /// <summary>
        /// Et bortesvin har kjøpt setet for gjeldende kamp
        /// </summary>
        Bortesvin = 3,

        /// <summary>
        /// Setet er holdt av til ett eller annet formål
        /// </summary>
        Reserved = 5,

        /// <summary>
        /// Setet er utilgjengelig, uten at vi vet om det er solgt, reservert eller annet.
        /// </summary>
        Unavailable = 10,

        /// <summary>
        /// Ukjent status - vi vet virkelig ikke hva den betyr
        /// </summary>
        Unknown = 20,
}
}
