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
            var res = sut.FetchEvents(@"..\..\TestData\archive\2015")
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
            var events = sut.FetchEvents(@"..\..\TestData\archive\2015")
                            .ToList();
            var vikingur = events.Single(e => e.Opponent == "Vikingur");

            // Act
            var res = sut.GetMeasurements(vikingur)
                         .ToList();

            // Assert
            res.Count.Should().Be(399, "there are 399 measurements in this directory");
            res.All(m => m.EventId == 467263).Should().BeTrue("all measurements should return the same event id");
        }

        #endregion
    }
}
