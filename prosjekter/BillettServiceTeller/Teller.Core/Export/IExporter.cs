using System;
using System.Collections.Generic;
using System.IO;
using Teller.Core.BillettService;
using Teller.Core.Entities;

namespace Teller.Core.Export
{
    public interface IExporter
    {
        void Export(IList<BillettServiceSete> seter, string input);
    }

    public class SeterToCsv : IExporter
    {
        public void Export(IList<BillettServiceSete> seter, string input)
        {
            Console.WriteLine("Skriver til CSV-fil {0}", input);
            var exporter = new CsvExport();
            exporter.ExportCsv(input, seter);
        }
    }

    public class LedigFile : IExporter
    {
        public void Export(IList<BillettServiceSete> seter, string input)
        {
            Console.WriteLine("Skriver ledig-oppsummering til tekstfil {0}", input);
            var ledig = new FreeSeatsTextSummary();
            var summary = ledig.CreateSummary(seter);
            File.WriteAllText(input, summary);
        }
    }

    public class OpptattFile : IExporter
    {
        public void Export(IList<BillettServiceSete> seter, string input)
        {
            Console.WriteLine("Skriver opptatt-oppsummering til tekstfil {0}", input);
            var opptatt = new SoldTextSummary();
            var summary = opptatt.CreateSummary(seter);
            File.WriteAllText(input, summary);
        }
    }

    public class SeatQuery : IExporter
    {
        public void Export(IList<BillettServiceSete> seter, string input)
        {
            Console.WriteLine("Rapporterer status på seter som matcher {0}", input);
            foreach (var seat in seter)
            {
                var pos = new StadiumPosition(seat.SectionName, seat.RowName, seat.SeatName);
                if (pos.IsMatch(input))
                    Console.WriteLine("{0} {1} ({2})", pos.ToString().PadRight(15), seat.EttCode.Code, SeatStatusClassifier.Classify(seat.EttCode.Code));
            }
        }
    }
}