using Teller.Core.Entities;

namespace Teller.Core.Filedata
{
    public interface IFileArchiver
    {
        /// <summary>
        /// Flytt én enkelt MeasurementFile til arkivet
        /// </summary>
        /// <param name="measurementFile">Fila som skal arkiveres</param>
        void MoveToArchive(MeasurementFile measurementFile);

        /// <summary>
        /// Utfører diverse oppgaver for å holde arkivet ryddig, som f.eks. zipping av filer.
        /// </summary>
        void PerformCleanup();
    }
}
