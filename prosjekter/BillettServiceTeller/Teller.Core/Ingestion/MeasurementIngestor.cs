using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
        private readonly IFileArchiver _fileArchiver;

        #endregion

        #region Constructor

        public MeasurementIngestor(IEventRepository eventRepository, IMeasurementRepository measurementRepository, IEventDataFetcher eventDataFetcher, IMeasurementReader measurementReader, IFileArchiver fileArchiver)
        {
            _eventRepository = eventRepository;
            _measurementRepository = measurementRepository;
            _eventDataFetcher = eventDataFetcher;
            _measurementReader = measurementReader;
            _fileArchiver = fileArchiver;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Hent nye datafiler fra disk og legg dem i databasen. Etterhvert: Arkivér dem også.
        /// </summary>
        /// <returns>A sequence of events that had data added to them</returns>
        public IEnumerable<BillettServiceEvent> ReadAndIngestData()
        {
            List<BillettServiceEvent> eventsOnDisk;

            try
            {
                eventsOnDisk = GetEventsFromDisk().ToList();
            }
            catch (Exception e)
            {
                Trace.TraceError("Fetching events failed, exiting: {0}", e.Message);
                return Enumerable.Empty<BillettServiceEvent>();
            }

            var updatedEvents = new List<BillettServiceEvent>();


            foreach (var diskEvent in eventsOnDisk)
            {
                var dbEvent = _eventRepository.GetByBillettServiceId(diskEvent.EventNumber);
                if (dbEvent == null)
                {
                    _eventRepository.Store(diskEvent);

                    dbEvent = diskEvent;
                }

                var diskMeasurements = _eventDataFetcher.GetMeasurements(diskEvent).ToList();
                if (!diskMeasurements.Any())
                    continue;
                
                Trace.TraceInformation("Got {0} new measurement{2} to process for event {1}: ", diskMeasurements.Count, diskEvent.EventNumber, diskMeasurements.Count == 1 ? "" : "s");
                foreach (var measurement in diskMeasurements)
                {
                    Trace.TraceInformation(measurement.FullPath);
                }

                var existingMeasurements = _measurementRepository.GetForEventAndDateTimes(dbEvent,
                    diskMeasurements.Select(dm => dm.MeasurementTime)).ToList();

                foreach (var diskMeasurement in diskMeasurements)
                {
                    if (existingMeasurements.Any(e => e.MeasurementTime == diskMeasurement.MeasurementTime))
                    {
                        Trace.TraceInformation("File skipped because of duplicate time: " + diskMeasurement.FullPath);
                        _fileArchiver.MoveToArchive(diskMeasurement);
                        continue;
                    }
                    var measurement = _measurementReader.ReadMeasurement(diskMeasurement);

                    if (measurement != null) // Er denne null, så var .xml-fila ikke brukbar for oss - vedlikehold hos BS, f.eks.
                    {
                        dbEvent.Measurements.Add(measurement);
                        Trace.TraceInformation("Added measurement for time {0} to event {1}", measurement.MeasurementTime, dbEvent.EventNumber);
                        if (!updatedEvents.Contains(dbEvent))
                        {
                            updatedEvents.Add(dbEvent);
                            Trace.TraceInformation("Adding {0} to list of updated events", dbEvent.EventNumber);
                        }
                    }

                    _fileArchiver.MoveToArchive(diskMeasurement);
                }
    
            }

            _eventRepository.SaveChanges();

            _fileArchiver.PerformCleanup();

            return updatedEvents;
        }

        #endregion

        #region Private methods

        private IEnumerable<BillettServiceEvent> GetEventsFromDisk()
        {
            var path = StautConfiguration.Current.CollectorDirectory;
            if (!Directory.Exists(path))
            {
                Trace.TraceError("collectorDirectory is not set to a valid directory: {0}", path);
                throw new ConfigurationErrorsException("collectorDirectory er ikke satt til et gyldig directory");
            }

            return _eventDataFetcher.FetchEvents(path);
        }

        #endregion
    }
}
