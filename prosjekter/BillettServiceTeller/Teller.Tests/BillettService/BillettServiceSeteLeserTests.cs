using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
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
            // ReSharper disable once ExpressionIsAlwaysNull
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
    }
}
