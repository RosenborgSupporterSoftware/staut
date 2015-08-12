using System;
using System.IO;
using Fclp;

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

        public bool Hate { get; set; }

        #endregion

        #region Constructor

        public CommandLineOptions(string[] args)
        {
            Args = args;
            
            var parser = new FluentCommandLineParser();

            parser.Setup<string>('i').Callback(val => { InputFile = val; });
            parser.Setup<string>('c').Callback(val => { CsvOutputFile = val; });
            parser.Setup<string>('l').Callback(val => { LedigTextFile = val; });
            parser.Setup<string>('o').Callback(val => { OpptattTextFile = val; });
            parser.Setup<string>('x').Callback(val => { XmlOutputFile = val; });
            parser.Setup<string>('s').Callback(val => { SeatQuery = val; });
            parser.Setup<bool>('a').Callback(val => { DumpArgs = val; });
            parser.Setup<bool>('h').Callback(val => { Hate = val; });
            parser.Setup<string>('t').Callback(val =>
            {
                if (val == string.Empty) return;
                PrintCountSummary = true;
                CountFile = val;
            });

            parser.Parse(args);
        }

        #endregion
    }
}
