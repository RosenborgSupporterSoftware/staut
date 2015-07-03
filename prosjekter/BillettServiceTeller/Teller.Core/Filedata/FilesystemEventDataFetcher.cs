using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Teller.Core.BillettService;
using Teller.Core.Entities;

namespace Teller.Core.Filedata
{
    public class FilesystemEventDataFetcher : IEventDataFetcher
    {
        private Dictionary<string, BillettServiceEvent> _events;

        public IEnumerable<BillettServiceEvent> FetchEvents(string archivePath)
        {
            if(String.IsNullOrWhiteSpace(archivePath) || !Directory.Exists(archivePath))
                throw new ArgumentException("Dårlig filområde");

            // Loop gjennom archivePath sine subdirectories etter eventinfo.properties-filer
            var reader = new EventInfoPropertyReader();

            _events = Directory.GetFiles(archivePath, "eventinfo.properties", SearchOption.AllDirectories)
                               .ToDictionary(file => Path.GetDirectoryName(file), file => reader.ReadProperties(file));

            return _events.Values;
        }

        public IEnumerable<MeasurementFile> GetMeasurements(BillettServiceEvent bsEvent)
        {
            if(!_events.ContainsValue(bsEvent))
                yield break;

            var dir = _events.Single(kvp => kvp.Value == bsEvent).Key;

            var files = Directory.GetFiles(dir, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                yield return new MeasurementFile(file);
            }
        }
    }
}
