using System;

namespace StautApi.Models
{
    public class CreateMeasurement
    {
        public DateTime MeasurementTime { get; set; }
        public int AmountSold { get; set; }
        public int AmountSeasonTicket { get; set; }
        public int AmountAvailable { get; set; }
        public int AmountReserved { get; set; }
        public int AmountUnavailable { get; set; }
        public int AmountTicketMaster { get; set; }
        public int AmountUnknown { get; set; }
    }
}