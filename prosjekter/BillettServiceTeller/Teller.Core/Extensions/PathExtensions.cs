using System;
using System.IO;
using Teller.Core.Infrastructure;

namespace Teller.Core.Extensions
{
    /// <summary>
    /// Diverse utility-metoder for å forenkle filsystem-relaterte ting. Ikke extension-metoder da man ikke kan lage det på statisk stuff som Path.
    /// </summary>
    public static class PathUtils
    {
        public static string GetLeafDirectoryName(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            if (dirInfo.Parent == null)
                throw new ArgumentException("Fila ligger tilsynelatende ikke i noe directory + " + path, "path");
            return dirInfo.Parent.Name;
        }

        /// <summary>
        /// Bestem riktig storage-folder for måledata
        /// </summary>
        /// <param name="eventDate">Datoen på kampen det skal lagres data for</param>
        /// <param name="eventFolderName">Navnet på mappa for måledata</param>
        /// <returns>Folder som kan brukes for lagring av data</returns>
        public static string GetStoragePath(DateTime eventDate, string eventFolderName)
        {
            return Path.Combine(StautConfiguration.Current.StorageDirectory,
                eventDate.Year.ToString(), eventDate.Month.ToString(),
                eventFolderName);
        }

    }
}
