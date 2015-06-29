using System.Collections.Generic;
using Teller.Core.Entities;

namespace Teller.Core.Repository
{
    /// <summary>
    /// Repo-interface for BillettServiceEvent-objekter - i praksis kamper
    /// </summary>
    public interface IEventRepository : IRepository<BillettServiceEvent>
    {
        /// <summary>
        /// Hent events som forekommer innenfor et gitt år
        /// </summary>
        /// <param name="year">Året vi ønsker events for</param>
        /// <returns>En sekvens med aktuelle events</returns>
        IEnumerable<BillettServiceEvent> GetByYear(int year);
    }
}
