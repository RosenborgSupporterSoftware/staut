using Teller.Core.Entities;

namespace Teller.Core.Repository
{
    /// <summary>
    /// Et interface som representerer funksjonaliteten for å lese et Measurement-objekt ut fra en MeasurementFile
    /// </summary>
    public interface IMeasurementReader
    {
        Measurement ReadMeasurement(MeasurementFile measurementFile);
    }
}
