using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Teller.Core.Entities;
using Teller.Core.Repository;

namespace Teller.Persistance.Implementations
{
    public class EventRepository : IEventRepository
    {
        #region Fields

        private readonly TellerContext _context;

        #endregion

        #region IEventRepository implementation

        public BillettServiceEvent Get(long id)
        {
            return _context.Events.Include(e => e.Measurements).FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<BillettServiceEvent> GetAll()
        {
            return _context.Events;
        }

        public void Store(BillettServiceEvent evt)
        {
            _context.Events.Add(evt);

            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Delete(BillettServiceEvent evt)
        {
            _context.Events.Remove(evt);
            _context.SaveChanges();
        }

        public BillettServiceEvent GetByBillettServiceId(int id)
        {
            return _context.Events.SingleOrDefault(e => e.EventNumber == id);
        }

        public IEnumerable<BillettServiceEvent> GetByYear(int year)
        {
            return _context.Events.Where(bse => bse.Start.Year == year);
        }

        #endregion

        public EventRepository(TellerContext context)
        {
            _context = context;
        }
    }
}
