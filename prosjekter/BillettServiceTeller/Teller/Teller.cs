using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Teller.Core.BillettService;
using Teller.Core.Export;
using Teller.Core.Infrastructure;

namespace Teller
{
    public class Teller
    {
        private readonly CommandLineOptions _opts;
        private BillettServiceXmlFile _ticketFile;
        private IList<BillettServiceSete> _seter;
        private int _outputsUsed;

        public Teller(CommandLineOptions opts)
        {
            _opts = opts;
        }

        public void Run()
        {
            using (PerformanceTester.Start(PrintPerformance))
            {
                PrintHeader();
                if (_opts.GotInputFile)
                {
                    _ticketFile = BillettServiceXmlFile.LoadFile(_opts.InputFile);
                    if (_opts.DumpArgs)             DumpArgs();
                    if (_opts.OutputCsv)            Export<SeterToCsv>(_opts.CsvOutputFile);
                    if (_opts.OutputLedigFile)      Export<LedigFile>(_opts.LedigTextFile);
                    if (_opts.OutputOpptattFile)    Export<OpptattFile>(_opts.OpptattTextFile);
                    if (_opts.OutputXmlFile)        WriteXml();
                    if (_opts.PerformSeatQuery)     Export<SeatQuery>(_opts.SeatQuery);
                    //Export<SeatQuery>("XA:24");
                    if (_opts.PrintCountSummary)    PrintCountSummary();

                    Print("Ingen gyldige outputs angitt.", _outputsUsed == 0);
                }
                else
                {
                    Print("Manglende eller ugyldig inputfil");
                }
            }
            
            if (_opts.Hate) PrintHate();

            Console.ReadKey();
        }

        private void PrintHeader()
        {
            Console.WriteLine("Teller v0.3");
            Console.WriteLine("haavarl velvære-edition");
            Console.WriteLine("Utviklet for RBKweb uten tillatelse eller tilgivelse");
            Console.WriteLine();
        }

        private void DumpArgs()
        {
            Console.WriteLine("Dumper args:");
            for (var i = 0; i < _opts.Args.Length; i++)
            {
                Console.WriteLine("args[{0}] = {1}", i, _opts.Args[i]);
            }
            Console.WriteLine("Dump ferdig");
        }

        private void Export<TExporter>(string input) where TExporter : IExporter, new()
        {
            var exporter = new TExporter();
            exporter.Export(Seter, input);
            
            _outputsUsed++;
        }

        private void WriteXml()
        {
            Console.WriteLine("Skriver dekodet xml-fil til {0}", _opts.XmlOutputFile);
            _ticketFile.XDocument.Save(_opts.XmlOutputFile);
            _outputsUsed++;
        }

        private void PrintCountSummary()
        {
            var summary = new CountSummary().CreateCountSummary(_ticketFile);
            if (!String.IsNullOrWhiteSpace(_opts.CountFile) && _opts.CountFile != "console")
            {
                Console.WriteLine("Skriver opptellingsinformasjon til {0}", _opts.CountFile);
                File.WriteAllText(_opts.CountFile, summary);
            }
                
            Console.Write(summary);

            _outputsUsed++;
        }

        private void PrintHate()
        {
            Console.WriteLine("Vi hate Molde og Lillestrøm.");
            Console.WriteLine("Vi hate Molde og Lillestrøm.");
            Console.WriteLine("Vi hate Molde og Lillestrøm.");
            Console.WriteLine("Men vi ælske RBK.");
        }

        private void Print(string text, bool predicate = true)
        {
            if(predicate) Console.WriteLine(text);
        }

        private void PrintPerformance(TimeSpan timeSpan)
        {
            Console.WriteLine("Ferdig - tok {0}ms", timeSpan.Milliseconds);
        }

        public IList<BillettServiceSete> Seter
        {
            get
            {
                if (_seter == null)
                {
                    var leser = new BillettServiceSeteLeser();
                    _seter = leser.ReadSeats(_ticketFile).ToList();
                }

                return _seter;
            }
        }
    }
}