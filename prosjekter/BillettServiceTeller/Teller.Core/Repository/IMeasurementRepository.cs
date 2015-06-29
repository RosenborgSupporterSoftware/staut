using System.Collections.Generic;
using Teller.Core.Entities;

namespace Teller.Core.Repository
{
    public interface IMeasurementRepository : IRepository<Measurement>
    {
        IEnumerable<Measurement> GetForBillettServiceEvent(int billetServiceId);
    }
}
