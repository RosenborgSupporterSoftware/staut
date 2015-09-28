using Teller.Core.BillettService;
using Teller.Core.Entities;

namespace Teller.Core.Filedata
{
    public class MeasurementReader : IMeasurementReader
    {
        public Measurement ReadMeasurement(MeasurementFile measurementFile)
        {
            var ticketFile = BillettServiceXmlFile.LoadFile(measurementFile.FullPath);
            if (ticketFile == null)
                return null;

            var summary = new SummaryGenerator().CreateSummary(ticketFile);

            Measurement measurement = new Measurement
            {
                MeasurementTime = measurementFile.MeasurementTime,
                AmountSold = summary.ContainsKey(SeatStatus.Sold) ? summary[SeatStatus.Sold] : 0,
                AmountSeasonTicket = summary.ContainsKey(SeatStatus.SeasonTicket) ? summary[SeatStatus.SeasonTicket] : 0,
                AmountAvailable =
                    summary.ContainsKey(SeatStatus.AvailableForPurchase) ? summary[SeatStatus.AvailableForPurchase] : 0,
                AmountReserved = summary.ContainsKey(SeatStatus.Reserved) ? summary[SeatStatus.Reserved] : 0,
                AmountUnavailable = summary.ContainsKey(SeatStatus.Unavailable) ? summary[SeatStatus.Unavailable] : 0,
                AmountTicketMaster =
                    summary.ContainsKey(SeatStatus.HeldByTicketMasterApplication)
                        ? summary[SeatStatus.HeldByTicketMasterApplication]
                        : 0,
                AmountUnknown = summary.ContainsKey(SeatStatus.Unknown) ? summary[SeatStatus.Unknown] : 0
            };

            return measurement;
        }
    }
}
