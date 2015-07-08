using System;
using System.IO;
using System.Linq;
using Teller.Core.BillettService;
using Teller.Core.Extensions;
using Teller.Core.Infrastructure;

namespace Teller.Core.Filedata
{
    public class PurgeCollectorFoldersOperation : ICleanupOperation
    {
        public double Priority
        {
            get { return 10; }
        }

        public void PerformCleanup()
        {
            var reader = new EventInfoPropertyReader();

            var propFiles = Directory.GetFiles(StautConfiguration.Current.CollectorDirectory, "eventinfo.properties",
                SearchOption.AllDirectories);
            foreach (var propFile in propFiles)
            {
                var propDir = Path.GetDirectoryName(propFile);
                if (String.IsNullOrWhiteSpace(propDir))
                    continue;
                var info = reader.ReadProperties(propFile);
                if (info.Start.Date >= DateTime.Today) // Vi rører ikke events før de er over.
                    continue;
                var fileCount = Directory.GetFiles(propDir).Count();
                if (fileCount > 1) // Ligger det flere filer der holder vi avstand.
                    continue;
                var leafDir = PathUtils.GetLeafDirectoryName(propFile);
                var storagePropFilename = Path.Combine(PathUtils.GetStoragePath(info.Start, leafDir), "eventinfo.properties");
                if (!File.Exists(storagePropFilename)) // Hvis ikke eventinfo-fila er kopiert over holder vi oss unna.
                    continue;
                // Kommer vi hit må vi vel endelig innse at dette directoriet kan slettes
                Directory.Delete(propDir, true);
            }
        }
    }
}
