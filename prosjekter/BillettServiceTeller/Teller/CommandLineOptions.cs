using System;
using System.IO;

namespace Teller
{
    /// <summary>
    /// En klasse for å parse command line options
    /// </summary>
    public class CommandLineOptions
    {
        #region Public properties

        public string[] Args { get; set; }
        public string InputFile { get; private set; }
        public string CsvOutputFile { get; private set; }
        public string LedigTextFile { get; private set; }
        public string OpptattTextFile { get; private set; }
        public string XmlOutputFile { get; private set; }
        public string SeatQuery { get; private set; }
        public string CountFile { get; private set; }
        public bool DumpArgs { get; private set; }

        /// <summary>
        /// Skal vi telle opp seter og gi en oppsummering?
        /// </summary>
        public bool PrintCountSummary { get; private set; }

        /// <summary>
        /// Har vi input-fil som eksisterer?
        /// </summary>
        public bool GotInputFile { get { return !String.IsNullOrWhiteSpace(InputFile) && File.Exists(InputFile); } }

        /// <summary>
        /// Skal vi generere CSV output?
        /// </summary>
        public bool OutputCsv { get { return !String.IsNullOrWhiteSpace(CsvOutputFile); } }

        /// <summary>
        /// Skal vi generere en fil med oversikt over ledige seter?
        /// </summary>
        public bool OutputLedigFile { get { return !String.IsNullOrWhiteSpace(LedigTextFile); } }

        /// <summary>
        /// Skal vi generere en fil med oversikt over opptatte seter?
        /// </summary>
        public bool OutputOpptattFile { get { return !String.IsNullOrWhiteSpace(OpptattTextFile); } }

        /// <summary>
        /// Skal vi lagre dekodet XML-fil til disk?
        /// </summary>
        public bool OutputXmlFile { get { return !String.IsNullOrWhiteSpace(XmlOutputFile); } }

        /// <summary>
        /// Skal vi sjekke status på noen seter?
        /// </summary>
        public bool PerformSeatQuery { get { return !String.IsNullOrWhiteSpace(SeatQuery); } }

        /// <summary>
        /// Skal vi lagre tellefil til disk?
        /// </summary>
        public bool OutputCountFile { get { return !String.IsNullOrWhiteSpace(CountFile); } }

        #endregion

        #region Constructor

        public CommandLineOptions(string[] args)
        {
            Args = args;
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-i")) // Sett inputFile
                    InputFile = args[i].Length > 2 ? args[i].Substring(2) : args[i+1];

                if (args[i].StartsWith("-c")) // Output CSV
                    CsvOutputFile = args[i].Length > 2 ? args[i].Substring(2) : args[++i];

                if (args[i].StartsWith("-l")) // Output tekstfil med ledige plasser
                    LedigTextFile = args[i].Length > 2 ? args[i].Substring(2) : args[++i];

                if (args[i].StartsWith("-o")) // Output tekstfil med opptatte plasser
                    OpptattTextFile = args[i].Length > 2 ? args[i].Substring(2) : args[++i];

                if (args[i].StartsWith("-x")) // Output dekodet xml-fil
                    XmlOutputFile = args[i].Length > 2 ? args[i].Substring(2) : args[++i];
                if (args[i].StartsWith("-s")) // Rapporter setestatus
                    SeatQuery = args[i].Length > 2 ? args[i].Substring(2) : args[++i];
                if (args[i].StartsWith("-t")) // Opptelling!
                {
                    PrintCountSummary = true;
                    if (args[i].Length > 2)
                        CountFile = args[i].Substring(2);
                    else if (args.Length > i + 1 && !args[i + 1].StartsWith("-"))
                        CountFile = args[++i];
                }
                if (args[i].StartsWith("-a")) // Dump argumentinfo
                    DumpArgs = true;
            }
        }

        #endregion
    }
}
