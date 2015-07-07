using System;
using System.IO;
using System.Linq;
using Teller.Core.BillettService;
using Teller.Core.Entities;
using Teller.Core.Infrastructure;

namespace Teller.Core.Filedata
{
    public class FileArchiver : IFileArchiver
    {
        public void MoveToArchive(MeasurementFile measurementFile)
        {
            if(measurementFile.FullPath==null)
                return;

            var dirInfo = new DirectoryInfo(measurementFile.FullPath);
            if(dirInfo.Parent==null) // Skjer vel ikke, mest for å få ReSharper til å slutte å mase mens jeg holder på.
                return;
            var eventDate =
                new EventInfoPropertyReader().ReadProperties(
                    Path.Combine(Path.GetDirectoryName(measurementFile.FullPath), "eventinfo.properties")).Start;
            var destinationPath = GetDestinationPath(eventDate, dirInfo.Parent.Name);
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            var destinationPropertyFile = Path.Combine(destinationPath, "eventinfo.properties");
            if (!File.Exists(destinationPropertyFile))
                File.Copy(Path.Combine(Path.GetDirectoryName(measurementFile.FullPath), "eventinfo.properties"),
                    destinationPropertyFile);

            var destinationFileName = Path.Combine(destinationPath, Path.GetFileName(measurementFile.FullPath));
            File.Move(measurementFile.FullPath, destinationFileName);
        }

        // TODO: Alle operasjonene som kjøres her bør skilles ut i egne objekter som implementerer et interface så vi lett kan loope igjennom dem. Finn dem med reflection.
        public void PerformCleanup()
        {
            PurgeOldCollectorFolders();

            // TODO: ZIP opp kamper hvor directory finnes men den er over (dvs. hele datoen passert)
            // TODO: Legg til ZIP-filer for en måned som er ferdig i den månedens zip-fil
            // TODO: Når et år har gått, legg zip-filer for det årets måneder i års-zip
        }

        private void PurgeOldCollectorFolders()
        {
            var reader = new EventInfoPropertyReader();

            var propFiles = Directory.GetFiles(StautConfiguration.Current.CollectorDirectory, "eventinfo.properties",
                SearchOption.AllDirectories);
            foreach (var propFile in propFiles)
            {
                var info = reader.ReadProperties(propFile);
                if(info.Start.Date >= DateTime.Today) // Vi rører ikke events før de er over.
                    continue;
                var fileCount = Directory.GetFiles(Path.GetDirectoryName(propFile)).Count();
                if(fileCount > 1) // Ligger det flere filer der holder vi avstand.
                    continue;
                var leafDir = GetLeafDirectoryName(propFile);
                var storagePropFilename = Path.Combine(GetDestinationPath(info.Start, leafDir), "eventinfo.properties");
                if(!File.Exists(storagePropFilename)) // Hvis ikke eventinfo-fila er kopiert over holder vi oss unna.
                    continue;
                // Kommer vi hit må vi vel endelig innse at dette directoriet kan slettes
                Directory.Delete(Path.GetDirectoryName(propFile), true);
            }
        }

        #region Utility methods

        private string GetDestinationPath(DateTime eventDate, string eventFolderName)
        {
            return Path.Combine(StautConfiguration.Current.StorageDirectory,
                eventDate.Year.ToString(), eventDate.Month.ToString(),
                eventFolderName);
        }

        private string GetLeafDirectoryName(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            if(dirInfo.Parent==null)
                throw new ArgumentException("Fila ligger tilsynelatende ikke i noe directory + " + path, "path");
            return dirInfo.Parent.Name;
        }

        #endregion
    }
}
