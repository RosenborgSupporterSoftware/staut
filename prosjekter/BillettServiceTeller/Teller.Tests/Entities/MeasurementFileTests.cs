using FluentAssertions;
using Teller.Core.Entities;
using Xunit;

namespace Teller.Tests.Entities
{
    /// <summary>
    /// Tester som sjekker at MeasurementFile-klassen gjør jobben sin
    /// </summary>
    public class MeasurementFileTests
    {
        [Fact]
        public void Constructor_WhenGivenRealFilename_ConstructsProperObject()
        {
            // Arrange
            const string filename = "438525_Sarpsborg_2015-06-30T15-12.xml";

            // Act
            var sut = new MeasurementFile(filename);

            // Assert
            sut.EventId.Should().Be(438525, "that is the eventId in the filename");
            sut.Opponent.Should().Be("Sarpsborg", "that is the opponent in the filename");
            sut.MeasurementTime.Year.Should().Be(2015);
            sut.MeasurementTime.Month.Should().Be(6);
            sut.MeasurementTime.Day.Should().Be(30);
            sut.MeasurementTime.Hour.Should().Be(15);
            sut.MeasurementTime.Minute.Should().Be(12);
        }

    }
}
