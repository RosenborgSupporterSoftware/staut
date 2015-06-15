using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Teller.Core.BillettService;
using Teller.Core.Entities;
using Teller.Core.Export;

namespace Teller
{
    class Program
    {
        private static IList<BillettServiceSete> _seter;
        private static BillettServiceXmlFile _ticketFile;

        /// <summary>
        /// Cachet for gjenbruk, liksom
        /// </summary>
        static IList<BillettServiceSete> Seter
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

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Teller v0.3");
            Console.WriteLine("haavarl velvære-edition");
            Console.WriteLine("Utviklet for RBKweb uten tillatelse eller tilgivelse");
            Console.WriteLine();

            var opts = new CommandLineOptions(args);

            if (!opts.GotInputFile)
            {
                Console.WriteLine("Manglende eller ugyldig inputfil");
                return;
            }

            _ticketFile = BillettServiceXmlFile.LoadFile(opts.InputFile);
            
            var outputsUsed = 0;

            if (opts.DumpArgs)
            {
                Console.WriteLine("Dumper args:");
                for (var i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("args[{0}] = {1}", i, args[i]);
                }
                Console.WriteLine("Dump ferdig");
            }

            if (opts.OutputCsv)
            {
                Console.WriteLine("Skriver til CSV-fil {0}", opts.CsvOutputFile);
                var csv = new CsvExport();
                csv.ExportCsv(opts.CsvOutputFile, Seter);
                outputsUsed++;
            }

            if (opts.OutputLedigFile)
            {
                Console.WriteLine("Skriver ledig-oppsummering til tekstfil {0}", opts.LedigTextFile);
                var ledig = new FreeSeatsTextSummary();
                var summary = ledig.CreateSummary(Seter);
                File.WriteAllText(opts.LedigTextFile, summary);
                outputsUsed++;
            }

            if (opts.OutputOpptattFile)
            {
                Console.WriteLine("Skriver opptatt-oppsummering til tekstfil {0}", opts.OpptattTextFile);
                var opptatt = new SoldTextSummary();
                var summary = opptatt.CreateSummary(Seter);
                File.WriteAllText(opts.OpptattTextFile, summary);
                outputsUsed++;
            }

            if (opts.OutputXmlFile)
            {
                Console.WriteLine("Skriver dekodet xml-fil til {0}", opts.XmlOutputFile);
                _ticketFile.XDocument.Save(opts.XmlOutputFile);
                outputsUsed++;
            }

            if (opts.PerformSeatQuery)
            {
                Console.WriteLine("Rapporterer status på seter som matcher {0}", opts.SeatQuery);
                var leser = new BillettServiceSeteLeser();
                var seats = leser.ReadSeats(_ticketFile);
                foreach (var seat in seats)
                {
                    var pos = new StadiumPosition(seat.SectionName, seat.RowName, seat.SeatName);
                    if(pos.IsMatch(opts.SeatQuery))
                        Console.WriteLine("{0} {1} ({2})", pos.ToString().PadRight(15), seat.EttCode.Code, SeatStatusClassifier.Classify(seat.EttCode.Code));
                }
                outputsUsed++;
            }

            if (opts.PrintCountSummary)
            {
                if(!String.IsNullOrWhiteSpace(opts.CountFile))
                    Console.WriteLine("Skriver opptellingsinformasjon til {0}", opts.CountFile);
                var summary = new CountSummary().CreateCountSummary(_ticketFile);
                if(!String.IsNullOrWhiteSpace(opts.CountFile))
                    File.WriteAllText(opts.CountFile, summary);
                Console.Write(summary);

                outputsUsed++;
            }

            if(outputsUsed==0)
                Console.WriteLine("Ingen gyldige outputs angitt.");
            else
            {
                sw.Stop();
                Console.WriteLine("Ferdig - tok {0}ms", sw.ElapsedMilliseconds);
            }
        }
    }
}
