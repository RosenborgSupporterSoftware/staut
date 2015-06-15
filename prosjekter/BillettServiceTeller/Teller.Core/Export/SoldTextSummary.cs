using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Teller.Core.BillettService;
using Teller.Core.Entities;

namespace Teller.Core.Export
{
    public class SoldTextSummary
    {
        public void SaveSummary(string filename, IEnumerable<BillettServiceSete> seats)
        {
            var summary = CreateSummary(seats);
            File.WriteAllText(filename, summary);
        }

        public void SaveSummary(string filename, IEnumerable<Tribune> info)
        {
            var summary = CreateSummary(info);
            File.WriteAllText(filename, summary);
        }

        public string CreateSummary(IEnumerable<BillettServiceSete> seats)
        {
            var oppsummering = new StringBuilder();
            oppsummering.AppendLine("Oppsummering:\r\n");

            var opptatt = 0;
            var totalt = 0;

            var summary = seats.GroupBy(s => s.SectionName)
                               .Select(g => new
                               {
                                   Section = g.Key,
                                   Sold = g.Count(s =>
                                   {
                                       var stat = SeatStatusClassifier.Classify(s.EttCode);
                                       return stat == SeatStatus.Sold || stat == SeatStatus.SeasonTicket;
                                   }),
                                   Total = g.Count()
                               })
                               .OrderBy(g => g.Section);

            foreach (var section in summary)
            {
                oppsummering.AppendFormat("{0}: {1} av {2}\r\n", section.Section, section.Sold, section.Total);
                opptatt += section.Sold;
                totalt += section.Total;
            }

            oppsummering.AppendFormat("GRAND SUM TOTAL: {0} seter solgt av {1} tilgjengelige.\r\n", opptatt, totalt);

            return oppsummering.ToString();
        }

        public string CreateSummary(IEnumerable<Tribune> info)
        {
            var oppsummering = new StringBuilder();
            oppsummering.AppendLine("Oppsummering:\r\n");

            var opptatt = 0;
            var totalt = 0;

            try
            {
                foreach (var tribune in info)
                {
                    oppsummering.AppendFormat("Tribune '{0}'\r\n", tribune.Navn);
                    foreach (var seksjon in tribune.Seksjoner)
                    {
                        oppsummering.AppendFormat(" {0}: {1} av {2}\r\n", seksjon.SeksjonsNavn, seksjon.AntallOpptatt,
                            seksjon.AntallSeter);
                    }
                    oppsummering.AppendFormat("Totalt {0} solgt av {1} tilgjengelige\r\n\r\n", tribune.AntallOpptatt,
                        tribune.AntallSeter);
                    opptatt += tribune.AntallOpptatt;
                    totalt += tribune.AntallSeter;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            oppsummering.AppendFormat("GRAND SUM TOTAL: {0} seter solgt av {1} tilgjengelige.\r\n", opptatt, totalt);

            return oppsummering.ToString();
        }
    }
}
