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
        /// Hent event basert på id hos BillettService
        /// </summary>
        /// <param name="id">Id som eventen har hos BillettService</param>
        /// <returns>BillettServiceEvent-objektet med riktig id, eller null om vi ikke har noe event med denne id'en</returns>
        BillettServiceEvent GetByBillettServiceId(int id);

        /// <summary>
        /// Hent events som forekommer innenfor et gitt år
        /// </summary>
        /// <param name="year">Året vi ønsker events for</param>
        /// <returns>En sekvens med aktuelle events</returns>
        IEnumerable<BillettServiceEvent> GetByYear(int year);
    }
}
