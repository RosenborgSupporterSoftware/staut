using System.Collections.Generic;
using Teller.Core.Entities;

namespace Teller.Core.Filedata
{
    /// <summary>
    /// Interface for lesing av event-data fra et directory
    /// </summary>
    public interface IEventDataFetcher
    {
        IEnumerable<BillettServiceEvent> FetchEvents(string archivePath);

        IEnumerable<MeasurementFile> GetMeasurements(BillettServiceEvent bsEvent);
    }
}
