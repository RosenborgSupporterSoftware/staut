using System;
using System.Linq;
using Teller.Charts;
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
            // Ikke akkurat dependency injection
            using (var context = new TellerContext())
            {
                var eventRepo = new EventRepository(context);
                var measurementRepo = new MeasurementRepository(context);
                var eventFetcher = new FilesystemEventDataFetcher();
                var measurementReader = new MeasurementReader();
                var fileArchiver = new FileArchiver();

                var ingestor = new MeasurementIngestor(eventRepo, measurementRepo, eventFetcher, measurementReader, fileArchiver);

                ingestor.ReadAndIngestData();
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
        }
    }
}
