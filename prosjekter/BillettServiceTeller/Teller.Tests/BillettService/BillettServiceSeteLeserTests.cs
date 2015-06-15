using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FluentAssertions;
using Teller.Core;
using Teller.Core.BillettService;
using Xunit;

namespace Teller.Tests.BillettService
{
    public class BillettServiceSeteLeserTests
    {
        [Fact]
        public void ReadSeats_WhenGivenNullXDocument_ReturnsEmptySequence()
        {
            // Arrange
            XDocument xdoc = null;
            var sut = new BillettServiceSeteLeser();

            // Act
            var res = sut.ReadSeats(xdoc).ToList();

            // Assert
            res.Should().NotBeNull("we should get an empty enumeration");
            res.Count.Should().Be(0, "there should be no items in the list");
        }

        [Fact]
        public void ReadSeats_WhenGivenActualBSFile_ReturnsCorrectSeatInfo()
        {
            // Arrange
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\Test.xml");

            // Act
            var res = sut.ReadSeats(file).ToList();

            // Assert
            res.Count.Should().Be(21645, "that is the number of seats in the file");

            // Stikkprøver
            var remTaRow24 = res.Where(s => s.SectionName == "REM-TA" && s.RowName == "24").ToList();
            remTaRow24.Count.Should().Be(22, "there should be 22 seats in this row");

            remTaRow24[0].EttCode.Code.Should().Be("000004CD0001", "that is the code on the first seat in the XML file");
            remTaRow24[7].EttCode.Code.Should().Be("000000000000", "that is the code on the eight seat in the XML file");
        }

        [Fact]
        public void ReadSeats_WhenGivedActualBSFile_HaveSomeFun()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-TIL-180515-1007.xml");

            // Act
            var res = sut.ReadSeats(file).ToList();

            var grouped = res.GroupBy(s => s.EttCode.Code)
                             .Select(g => new {Ett = g.Key, Seats = g.ToList()})
                             .OrderByDescending(g => g.Seats.Count)
                             .ToList();

            var builder = new StringBuilder();

            foreach (var grp in grouped)
            {
                builder.AppendLine(String.Format("{0}: {1}", grp.Ett, grp.Seats.Count));
            }

            var sesongkort = res.Count(s => s.EttCode.QualifierBitsHex.StartsWith("4"));

            var stats = res.Select(s => SeatStatusClassifier.Classify(s.EttCode))
                           .GroupBy(s => s)
                           .Select(s => new {Status = s.Key, Count = s.Count()})
                           .OrderByDescending(g => g.Count)
                           .ToList();

            var oversikt = builder.ToString();

            Console.WriteLine(oversikt);
        }

        [Fact]
        public void surr_sull_tull()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-TIL-010615-0812.xml");

            // Act
            var res = sut.ReadSeats(file).ToList();
            var stats = res.Select(s => SeatStatusClassifier.Classify(s.EttCode))
                           .GroupBy(s => s)
                           .Select(s => new { Status = s.Key, Count = s.Count() })
                           .OrderByDescending(g => g.Count)
                           .ToList();

            var builder = new StringBuilder();
            var sum = 0;

            foreach (var item in stats)
            {
                builder.AppendLine(item.Status + ": " + item.Count);
                sum += item.Count;
            }
            builder.AppendLine("--------------------------");
            builder.AppendLine("Sum: " + sum);
            var result = builder.ToString();

            var ukjent = res//.Where(s => SeatStatusClassifier.Classify(s.EttCode) == SeatStatus.Unavailable)
                            .GroupBy(s => s.EttCode.Code)
                            .Select(s => new {Code = s.Key, Seats = s.ToList()})
                            .OrderBy(g => g.Code)
                            .ToList();

            var bldr = new StringBuilder();
            var ukjente = 0;

            foreach (var gruppe in ukjent)
            {
                bldr.AppendLine("Code: " + gruppe.Code + " Seter: " + gruppe.Seats.Count);
                ukjente += gruppe.Seats.Count;
            }
            bldr.AppendLine("Sum: " + ukjente);
            var moro = bldr.ToString();

            var kanskjesolgt = res.Count(s => s.EttCode.QualifierBitsHex.StartsWith("5"));

            Console.WriteLine(moro);
        }


        [Fact]
        public void vennebillett_stuff_test()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-TIL-310515-1415.xml");
            var res = sut.ReadSeats(file).ToList();

            var pa = res.Where(s => s.SectionName.Contains("EA") && s.RowName == "21").ToList();

            var joakim = res.Where(s => s.EttCode.QualifierBitsHex == "591").ToList();
        }

        [Fact]
        public void ukjente_sesongkort_locator()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-TIL-250515-1409.xml");
            var res = sut.ReadSeats(file).ToList();

            var ukjenteKoder = res.Where(s => s.EttCode.QualifierBitsHex.StartsWith("4") && !SeatStatusClassifier.IsKnownSeasonTicketCode(s.EttCode))
                                  .GroupBy(s => s.EttCode.Code)
                                  .Select(s => new { Code = s.Key, Seats = s.ToList()})
                                  .OrderByDescending(g => g.Seats.Count)
                                  .ToList();

            var bldr = new StringBuilder();
            var ukjente = 0;

            foreach (var gruppe in ukjenteKoder)
            {
                bldr.AppendLine("Code: " + gruppe.Code + " Seter: " + gruppe.Seats.Count);
                ukjente += gruppe.Seats.Count;
            }
            bldr.AppendLine("Sum: " + ukjente);
            var moro = bldr.ToString();

        }

        [Fact]
        public void finn_vanligste_koder()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-TIL-310515-1415.xml");
            var res = sut.ReadSeats(file).ToList();

            var pa = res.Where(s => s.SectionName.Contains("KJ"))
                        .GroupBy(s => s.EttCode.Code)
                        .Select(g => new {Code = g.Key, Seats = g.Count()})
                        .OrderByDescending(g => g.Seats)
                        .ToList();

            Console.WriteLine("Break here");
        }

        [Fact]
        public void Kode_By_Field()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-TIL-300515-0040.xml");
            var res = sut.ReadSeats(file).ToList();

            var pa = res.Where(s => s.EttCode.QualifierBitsHex == "17")
                        .GroupBy(s => s.SectionName)
                        .Select(g => new { Section = g.Key, Seats = g.Count() })
                        .OrderByDescending(g => g.Seats)
                        .ToList();

            Console.WriteLine(pa.Count);
        }

        [Fact]
        public void Sesongkort_Only_FinnKoder()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-Glimt-020615-2048.xml");
            var res = sut.ReadSeats(file).ToList();

            var allRelevantCodes = res.Where(s => s.EttCode.QualifierBits > 0x400 && s.EttCode.QualifierBits < 0x600)
                                      .GroupBy(s => s.EttCode.QualifierBitsHex)
                                      .Select(g => new {Code = g.Key, Seats = g.Count()})
                                      .OrderByDescending(g => g.Seats)
                                      .ToList();

            var bldr = new StringBuilder();
            var seats = 0;

            foreach (var gruppe in allRelevantCodes)
            {
                bldr.AppendLine("Code: " + gruppe.Code + " Seter: " + gruppe.Seats);
                seats += gruppe.Seats;
            }
            bldr.AppendLine("Sum: " + seats);

            var output = bldr.ToString();
            Console.WriteLine(output);
        }

        [Fact]
        public void Plasser_hos_Kjernen()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-Glimt-020615-2048.xml");
            var res = sut.ReadSeats(file).ToList();

            var allRelevantCodes = res.Where(s => s.SectionName.Contains("KJ"))
                                      .ToList();

            var output = allRelevantCodes.Count;

            Console.WriteLine(output);
        }

        [Fact]
        public void Status_Plasser_hos_Kjernen()
        {
            var sut = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(@"..\..\TestData\RBK-mfk-150615-0913.xml");
            var res = sut.ReadSeats(file).ToList();

            var allRelevantCodes = res.Where(s => s.SectionName.Contains("KJ"))
                                      .Select(s => SeatStatusClassifier.Classify(s.EttCode))
                                      .GroupBy(g => g)
                                      .Select(g => new { Code = g.Key, Count = g.Count()})
                                      .ToList();

            var output = allRelevantCodes.Count;

            Console.WriteLine(output);
        }

    }
}
