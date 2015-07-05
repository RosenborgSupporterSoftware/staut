using System;
using System.Collections.Generic;
using System.Linq;
using Teller.Core.Entities;
using Teller.Core.Repository;

namespace Teller.Persistance.Implementations
{
    public class MeasurementRepository : IMeasurementRepository
    {
        #region Fields

        private readonly TellerContext _context;

        #endregion

        #region Interface implementation

        public Measurement Get(long id)
        {
            return _context.Measurements.Find(id);
        }

        public IEnumerable<Measurement> GetAll()
        {
            return _context.Measurements;
        }

        public void Store(Measurement evt)
        {
            _context.Measurements.Add(evt);

            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Delete(Measurement evt)
        {
            _context.Measurements.Remove(evt);
            _context.SaveChanges();
        }

        public IEnumerable<Measurement> GetForBillettServiceEvent(int billetServiceId)
        {
            return _context.Measurements.Where(m => m.BillettServiceEventId == billetServiceId);
        }

        public IEnumerable<Measurement> GetForEventAndDateTimes(BillettServiceEvent bsEvent, IEnumerable<DateTime> dateTimes)
        {
            return
                _context.Measurements.Where(
                    m => m.BillettServiceEventId == bsEvent.Id && dateTimes.Contains(m.MeasurementTime));
        }

        #endregion

        public MeasurementRepository(TellerContext context)
        {
            _context = context;
        }
    }
}
