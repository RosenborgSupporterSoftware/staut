using System.Linq;
using FluentAssertions;
using Teller.Core.Filedata;
using Xunit;

namespace Teller.Tests.Filedata
{
    public class FilesystemEventDataFetcherTests
    {
        #region FetchEvents tests

        [Fact]
        public void FetchEvents_WhenGivenRealDataDirectory_ReturnsExpectedObjects()
        {
            // Arrange
            var sut = new FilesystemEventDataFetcher();

            // Act
            var res = sut.FetchEvents(@"..\..\TestData\archive")
                         .ToList();

            // Assert
            res.Count.Should().Be(3, "we have three directories with eventinfo.properties files");
        }

        #endregion

        #region GetMeasurements tests

        [Fact]
        public void GetMeasurements_WhenGivenRealDataDirectory_ReturnsExpectedMeasurements()
        {
            // Arrange
            var sut = new FilesystemEventDataFetcher();
            var events = sut.FetchEvents(@"..\..\TestData\archive")
                            .ToList();
            var odd = events.Single(e => e.Opponent == "Odd");

            // Act
            var res = sut.GetMeasurements(odd)
                         .ToList();

            // Assert
            res.Count.Should().Be(3, "there are three measurements in this directory");
            res.All(m => m.EventId == 438523).Should().BeTrue("all measurements should return the same event id");
            res.All(m => m.MeasurementTime.Year == 2015 && m.MeasurementTime.Month == 6 && m.MeasurementTime.Day == 30)
               .Should()
               .BeTrue("all measurements should have the same date");
            res.Count(m => m.MeasurementTime.Hour == 12 && m.MeasurementTime.Minute == 12)
               .Should()
               .Be(1, "only one of the measurements should match");
            res.Count(m => m.MeasurementTime.Hour == 12 && m.MeasurementTime.Minute == 42)
               .Should()
               .Be(1, "only one of the measurements should match");
            res.Count(m => m.MeasurementTime.Hour == 13 && m.MeasurementTime.Minute == 12)
               .Should()
               .Be(1, "only one of the measurements should match");
        }

        #endregion
    }
}
