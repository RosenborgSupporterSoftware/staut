using System;
using System.Collections.Generic;

namespace StautApi.Models
{
    public class CreateEvent
    {
        public int EventNumber { get; set; }
        public string EventCode { get; set; }
        public string DisplayName { get; set; }
        public string Tournament { get; set; }
        public int Round { get; set; }
        public string Season { get; set; }
        public string Location { get; set; }
        public string Opponent { get; set; }
        public DateTime Start { get; set; }
        public string EventUrl { get; set; }
        public string AvailibilityUrl { get; set; }
        public string GeometryUrl { get; set; }
        public int FinalEstimatedSeatCount { get; set; }
        public int OfficialSeatCount { get; set; }
    }

    public class EventDto
    {
        public int Id { get; set; }
        public int EventNumber { get; set; }
        public string EventCode { get; set; }
        public string DisplayName { get; set; }
        public string Tournament { get; set; }
        public int Round { get; set; }
        public string Season { get; set; }
        public string Location { get; set; }
        public string Opponent { get; set; }
        public DateTime Start { get; set; }
        public string EventUrl { get; set; }
        public string AvailibilityUrl { get; set; }
        public string GeometryUrl { get; set; }
        public int FinalEstimatedSeatCount { get; set; }
        public int OfficialSeatCount { get; set; }
        public List<MeasurementDto> Measurements { get; set; }
    }

    public class MeasurementDto
    {
        public int Id { get; set; }
        public DateTime MeasurementTime { get; set; }
        public int AmountSold { get; set; }
        public int AmountSeasonTicket { get; set; }
        public int TotalAmountSold { get; set; }
        public int AmountAvailable { get; set; }
        public int AmountReserved { get; set; }
        public int AmountUnavailable { get; set; }
        public int AmountTicketMaster { get; set; }
        public int AmountUnknown { get; set; }
    }
}