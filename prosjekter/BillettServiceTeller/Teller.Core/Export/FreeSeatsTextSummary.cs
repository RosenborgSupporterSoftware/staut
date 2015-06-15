using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Teller.Core.BillettService;
using Teller.Core.Entities;

namespace Teller.Core.Export
{
    public class FreeSeatsTextSummary
    {
        public void SaveSummary(string filename, IList<BillettServiceSete> seats)
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

            var ledig = 0;
            var totalt = 0;

            var sections = seats.GroupBy(s => s.SectionName)
                                .Select(
                                    g =>
                                        new
                                        {
                                            Section = g.Key,
                                            Ledige =
                                                g.Count(
                                                    s =>
                                                        SeatStatusClassifier.Classify(new EttCode(s.EttCode)) ==
                                                        SeatStatus.AvailableForPurchase),
                                            Totalt = g.Count()
                                        })
                                .OrderBy(g => g.Section)
                                .ToList();

            try
            {
                foreach (var section in sections)
                {
                    oppsummering.AppendFormat("{0}: {1} av {2}\r\n", section.Section, section.Ledige,
                        section.Totalt);
                    //oppsummering.AppendFormat("Totalt {0} ledig av {1} totale seter\r\n\r\n", tribune.AntallLedige,
                    //    tribune.AntallSeter);
                    ledig += section.Ledige;
                    totalt += section.Totalt;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            oppsummering.AppendFormat("GRAND SUM TOTAL: {0} seter ledig av {1} totale seter.\r\n", ledig, totalt);

            return oppsummering.ToString();
        }

        public string CreateSummary(IEnumerable<Tribune> info)
        {
            var oppsummering = new StringBuilder();
            oppsummering.AppendLine("Oppsummering:\r\n");

            var ledig = 0;
            var totalt = 0;

            try
            {
                foreach (var tribune in info)
                {
                    oppsummering.AppendFormat("Tribune '{0}'\r\n", tribune.Navn);
                    foreach (var seksjon in tribune.Seksjoner)
                    {
                        oppsummering.AppendFormat(" {0}: {1} av {2}\r\n", seksjon.SeksjonsNavn, seksjon.AntallLedige,
                            seksjon.AntallSeter);
                    }
                    oppsummering.AppendFormat("Totalt {0} ledig av {1} totale seter\r\n\r\n", tribune.AntallLedige,
                        tribune.AntallSeter);
                    ledig += tribune.AntallLedige;
                    totalt += tribune.AntallSeter;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            oppsummering.AppendFormat("GRAND SUM TOTAL: {0} seter ledig av {1} totale seter.\r\n", ledig, totalt);

            return oppsummering.ToString();
        }
    }
}
