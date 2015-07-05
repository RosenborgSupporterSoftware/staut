using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Teller.Core.Entities
{
    /// <summary>
    /// En klasse som representerer en datafil fra disk
    /// </summary>
    public class MeasurementFile
    {
        #region Fields

        private readonly string _fullPath;
        private static readonly Regex FilenameRegex = new Regex(@"^(?<eventid>\d*)_(?<opponent>[^_]*)_(?<year>\d\d\d\d)-(?<month>\d\d)-(?<day>\d\d)T(?<hour>\d\d)-(?<minute>\d\d)\.xml$", RegexOptions.Compiled);

        #endregion

        #region Public properties

        /// <summary>
        /// Filnavn og plassering
        /// </summary>
        public string FullPath { get { return _fullPath; } }

        /// <summary>
        /// BillettService-id for denne kampen
        /// </summary>
        public long EventId { get; set; }

        /// <summary>
        /// Fienden i denne kampen
        /// </summary>
        public string Opponent { get; set; }

        /// <summary>
        /// Tidspunkt for når målingen er gjort
        /// </summary>
        public DateTime MeasurementTime { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Opprett et nytt MeasurementFile-objekt med utgangspunkt i en gitt datafil
        /// </summary>
        /// <param name="fullPath">Datafil som er utgangspunkt for dette MeasurementFile-objektet</param>
        public MeasurementFile(string fullPath)
        {
            _fullPath = fullPath;
            InterpretFilename();
        }

        #endregion

        #region Private methods

        private void InterpretFilename()
        {
            var filename = Path.GetFileName(_fullPath);
            if(String.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Dårlig filnavn");

            var match = FilenameRegex.Match(filename);
            if(match.Groups.Count != 8)
                throw new ArgumentException("Dårlig filnavn");

            if (match.Groups["eventid"] != null)
                EventId = Convert.ToInt64(match.Groups["eventid"].Value);
            if (match.Groups["opponent"] != null)
                Opponent = match.Groups["opponent"].Value;
            // TODO: Jeg TROR dette under her bør være nært opptil failsafe når vi får match på regex og kommer hit. Verdt å lete etter corner case? Tror ikke det.
            MeasurementTime = new DateTime(Convert.ToInt32(match.Groups["year"].Value),
                Convert.ToInt32(match.Groups["month"].Value), Convert.ToInt32(match.Groups["day"].Value),
                Convert.ToInt32(match.Groups["hour"].Value), Convert.ToInt32(match.Groups["minute"].Value), 0);
        }

        #endregion
    }
}
