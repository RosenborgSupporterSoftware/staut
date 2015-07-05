using System;
using System.Collections.Generic;
using Teller.Core.Entities;

namespace Teller.Core.Repository
{
    /// <summary>
    /// Repository-interface for oppbevaring, henting o.l. av Measurement-objekter
    /// </summary>
    public interface IMeasurementRepository : IRepository<Measurement>
    {
        /// <summary>
        /// Hent alle målinger for en angitt event
        /// </summary>
        /// <param name="billettServiceId">Event-id man vil ha målinger for</param>
        /// <returns>Alle målinger som finnes for denne eventen</returns>
        IEnumerable<Measurement> GetForBillettServiceEvent(int billettServiceId);

        IEnumerable<Measurement> GetForEventAndDateTimes(BillettServiceEvent bsEvent, IEnumerable<DateTime> dateTimes);
    }
}
