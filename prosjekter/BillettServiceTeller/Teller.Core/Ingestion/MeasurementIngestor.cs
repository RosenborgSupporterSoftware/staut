using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Teller.Core.Entities;
using Teller.Core.Filedata;
using Teller.Core.Infrastructure;
using Teller.Core.Repository;

namespace Teller.Core.Ingestion
{
    /// <summary>
    /// Klassen som tar data fra disk og legger det inn i databasen.
    /// Selvfølgelig vha. av masse mockable repositories.
    /// </summary>
    public class MeasurementIngestor
    {
        #region Fields

        private readonly IEventRepository _eventRepository;
        private readonly IMeasurementRepository _measurementRepository;
        private readonly IEventDataFetcher _eventDataFetcher;
        private readonly IMeasurementReader _measurementReader;

        #endregion

        #region Constructor

        public MeasurementIngestor(IEventRepository eventRepository, IMeasurementRepository measurementRepository, IEventDataFetcher eventDataFetcher, IMeasurementReader measurementReader)
        {
            _eventRepository = eventRepository;
            _measurementRepository = measurementRepository;
            _eventDataFetcher = eventDataFetcher;
            _measurementReader = measurementReader;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Hent nye datafiler fra disk og legg dem i databasen. Etterhvert: Arkivér dem også.
        /// </summary>
        public void ReadAndIngestData()
        {
            var eventsOnDisk = GetEventsFromDisk();

            foreach (var diskEvent in eventsOnDisk)
            {
                var dbEvent = _eventRepository.GetByBillettServiceId(diskEvent.EventNumber);
                if (dbEvent == null)
                {
                    _eventRepository.Store(diskEvent);

                    dbEvent = diskEvent;
                }

                var diskMeasurements = _eventDataFetcher.GetMeasurements(diskEvent).ToList();
                var existingMeasurements = _measurementRepository.GetForEventAndDateTimes(dbEvent,
                    diskMeasurements.Select(dm => dm.MeasurementTime)).ToList();

                foreach (var diskMeasurement in diskMeasurements)
                {
                    if (existingMeasurements.Any(e => e.MeasurementTime == diskMeasurement.MeasurementTime))
                    {
                        Console.WriteLine("Vi vil ikke ha denne: " + diskMeasurement.FullPath);
                        // TODO: Arkivér de vi har fra før
                        continue;
                    }
                    var measurement = _measurementReader.ReadMeasurement(diskMeasurement);
                    dbEvent.Measurements.Add(measurement);
                    // TODO: Arkivér dem etterhvert
                }
    
            }

            _eventRepository.SaveChanges();
        }

        #endregion

        #region Private methods

        private IEnumerable<BillettServiceEvent> GetEventsFromDisk()
        {
            var path = StautConfiguration.Current.CollectorDirectory;
            if(!Directory.Exists(path))
                throw new ConfigurationErrorsException("collectorDirectory er ikke satt til et gyldig directory");

            return _eventDataFetcher.FetchEvents(path);
        }

        #endregion
    }
}
