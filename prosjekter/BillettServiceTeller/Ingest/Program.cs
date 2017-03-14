using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Teller.Charts;
using Teller.Core.Entities;
using Teller.Core.Filedata;
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

            if (!updatedEvents.Any())
            {
                Trace.Flush();
                Trace.Close();
                return;
            }

            using (var context = new TellerContext())
            {
                var eventRepo = new EventRepository(context);

                var allEvents = eventRepo.GetAll()
                                         .ToList();

                var test = new RenderTest();

                foreach (var bsEvent in allEvents)
                {
                    test.Render(bsEvent);
                }
            }

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
