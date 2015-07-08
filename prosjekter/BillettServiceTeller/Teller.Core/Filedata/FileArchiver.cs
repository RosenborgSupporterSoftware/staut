using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Teller.Core.BillettService;
using Teller.Core.Entities;
using Teller.Core.Extensions;
using Teller.Core.Infrastructure;

namespace Teller.Core.Filedata
{
    public class FileArchiver : IFileArchiver
    {
        public void MoveToArchive(MeasurementFile measurementFile)
        {
            var sourceDirectory = Path.GetDirectoryName(measurementFile.FullPath);
            if(String.IsNullOrWhiteSpace(sourceDirectory))
                return; // Kutt resharper-mas
            
            var eventDate =
                new EventInfoPropertyReader().ReadProperties(
                    Path.Combine(sourceDirectory, "eventinfo.properties")).Start;
            var destinationPath = PathUtils.GetStoragePath(eventDate, PathUtils.GetLeafDirectoryName(measurementFile.FullPath));
            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            var destinationPropertyFile = Path.Combine(destinationPath, "eventinfo.properties");
            if (!File.Exists(destinationPropertyFile))
                File.Copy(Path.Combine(sourceDirectory, "eventinfo.properties"),
                    destinationPropertyFile);

            var destinationFileName = Path.Combine(destinationPath, Path.GetFileName(measurementFile.FullPath));
            File.Move(measurementFile.FullPath, destinationFileName);
        }

        public void PerformCleanup()
        {
            var operations = FindCleanupOperations();

            foreach (var operation in operations)
            {
                operation.PerformCleanup();
            }

            // ICleanupOperation-implementasjoner som hadde vært kjekke å ha
            // TODO: ZIP opp kamper hvor directory finnes men den er over (dvs. hele datoen passert)
            // TODO: Legg til ZIP-filer for en måned som er ferdig i den månedens zip-fil
            // TODO: Når et år har gått, legg zip-filer for det årets måneder i års-zip
        }

        private IEnumerable<ICleanupOperation> FindCleanupOperations()
        {
            return Assembly.GetExecutingAssembly()
                           .GetTypes()
                           .Where(t => !t.IsInterface && typeof (ICleanupOperation).IsAssignableFrom(t))
                           .Select(t => Activator.CreateInstance(t) as ICleanupOperation)
                           .OrderBy(i => i.Priority);
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
                var leafDir = PathUtils.GetLeafDirectoryName(propFile);
                var storagePropFilename = Path.Combine(PathUtils.GetStoragePath(info.Start, leafDir), "eventinfo.properties");
                if(!File.Exists(storagePropFilename)) // Hvis ikke eventinfo-fila er kopiert over holder vi oss unna.
                    continue;
                // Kommer vi hit må vi vel endelig innse at dette directoriet kan slettes
                Directory.Delete(Path.GetDirectoryName(propFile), true);
            }
        }

        #region Utility methods


        #endregion
    }
}
