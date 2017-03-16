using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Teller.Charts;
using Teller.Core.Entities;
using Teller.Core.Filedata;
using Teller.Core.Infrastructure;
using Teller.Core.Ingestion;
using Teller.Persistance;
using Teller.Persistance.Implementations;

namespace Ingest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            SetupTraceListeners();

            List<BillettServiceEvent> updatedEvents;

            // Ikke akkurat dependency injection
            using (var context = new TellerContext())
            {
                var eventRepo = new EventRepository(context);
                var measurementRepo = new MeasurementRepository(context);
                var eventFetcher = new FilesystemEventDataFetcher();
                var measurementReader = new MeasurementReader();
                var fileArchiver = new FileArchiver();

                var ingestor = new MeasurementIngestor(eventRepo, measurementRepo, eventFetcher, measurementReader, fileArchiver);

                updatedEvents = ingestor.ReadAndIngestData().ToList();
            }

            Trace.TraceInformation("{0} events updated: {1}", updatedEvents.Count, String.Join(", ", updatedEvents.Select(e => e.EventNumber)));

            using (var context = new TellerContext())
            {
                var eventRepo = new EventRepository(context);

                var allEvents = eventRepo.GetAll()
                                         //.Where(e => updatedEvents.Any(ue => ue.EventNumber == e.EventNumber))
                                         .ToList();

                var missingFileEvents =
                    allEvents.Where(
                                 ev =>
                                     !File.Exists(Path.Combine(StautConfiguration.Current.StaticImageDirectory,
                                         ev.EventNumber + ".png")))
                             .ToList();

                var eventsToRender =
                    missingFileEvents.Concat(
                        allEvents.Where(
                            ae =>
                                !missingFileEvents.Contains(ae) &&
                                updatedEvents.Any(ue => ue.EventNumber == ae.EventNumber))).ToList();

                var test = new RenderTest();

                foreach (var bsEvent in eventsToRender)
                {
                    test.Render(bsEvent);
                }
            }

            sw.Stop();
            Trace.TraceInformation("Ingest process complete, took {0}ms", sw.ElapsedMilliseconds);

            Trace.Flush();
            Trace.Close();
        }

        private static void SetupTraceListeners()
        {
            var today = DateTime.Today;
            var path = $"Trace\\{today.Year}\\{today.Month}\\{today.Day}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fullPath = Path.Combine(Environment.CurrentDirectory, path, "IngestTrace.txt");

            Trace.Listeners.Add(new ConsoleTraceListener());
            Trace.Listeners.Add(new TextWriterTraceListener(fullPath));
        }
    }
}
