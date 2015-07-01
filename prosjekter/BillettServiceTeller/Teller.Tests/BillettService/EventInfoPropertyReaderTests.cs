using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Teller.Core.BillettService;
using Xunit;

namespace Teller.Tests.BillettService
{
    public class EventInfoPropertyReaderTests
    {
        #region ReadProperties tests

        #region Bad arguments

        [Fact]
        public void ReadProperties_WhenGivenNull_ThrowsArgumentException()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            Assert.Throws<ArgumentException>(() => sut.ReadProperties(null));

            // Assert
        }

        [Fact]
        public void ReadProperties_WhenGivenEmptyString_ThrowsArgumentException()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            Assert.Throws<ArgumentException>(() => sut.ReadProperties(String.Empty));

            // Assert
        }

        [Fact]
        public void ReadProperties_WhenGivenNonExistingFile_ThrowsArgumentException()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();
            const string badFilename = @"buh:\nah\d'oh\eventinfo.properties";

            // Act
            Assert.Throws<ArgumentException>(() => sut.ReadProperties(badFilename));

            // Assert
        }

        #endregion

        #region Good test file

        [Fact]
        public void ReadProperties_GivenGoodInputFile_ReturnsValidObject()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");
            
            // Assert
            res.Should().NotBeNull("valid filename should result in valid object");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectDisplayName()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.DisplayName.Should()
               .Be("Rosenborg - Sarpsborg 08", "that is what the eventname field of the file contains");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectEventNumber()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.EventNumber.Should()
               .Be(438525, "that is what the eventid field of the file contains");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectEventCode()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.EventCode.Should()
               .Be("TLD0915-2015", "that is what the eventcode field of the file contains");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectLocation()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.Location.Should()
               .Be("TLD", "that is what the location field of the file contains");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectTournament()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.Tournament.Should()
               .Be("LEAGUE", "that is what the competition field of the file contains");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectRound()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.Round.Should()
               .Be(9, "that is what the round field of the file contains");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectOpponent()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.Opponent.Should()
               .Be("Sarpsborg", "that is what the opponent field of the file contains");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectStartTime()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.Start.Year.Should().Be(2015, "that is the correct year");
            res.Start.Month.Should().Be(8, "that is the correct month");
            res.Start.Day.Should().Be(2, "that is the correct day");
            res.Start.Hour.Should().Be(20, "that is the correct hour");
            res.Start.Minute.Should().Be(00, "that is the correct minute");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectAvailabilityUrl()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.AvailibilityUrl.Should().Be("http://www.billettservice.no/seatmap/proxy/availability/NO/TLD0915-2015,NO-438525.xml", "that is the correct url");
        }

        [Fact]
        public void ReadProperties_GivenGoodInputFile_GetsCorrectGeometryUrl()
        {
            // Arrange
            var sut = new EventInfoPropertyReader();

            // Act
            var res = sut.ReadProperties(@"..\..\TestData\eventinfo.properties");

            // Assert
            res.GeometryUrl.Should().Be("http://www.billettservice.no/seatmap/proxy/geometry/syst-25/geometry/TLD12_V07_geometry.xml", "that is the correct url");
        }

        #endregion

        #endregion
    }
}
