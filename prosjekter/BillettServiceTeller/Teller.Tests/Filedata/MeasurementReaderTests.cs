using FluentAssertions;
using Teller.Core.Entities;
using Teller.Core.Filedata;
using Xunit;

namespace Teller.Tests.Filedata
{
    public class MeasurementReaderTests
    {
        #region ReadMeasurement tests

        #region Good file tests

        [Fact]
        public void ReadMeasurement_WhenGivenGoodFile_ReturnsExpectedData()
        {
            // Arrange
            const string inputFile = @"..\..\TestData\archive\2015_LEAGUE_8_Odd_438523\438523_Odd_2015-06-30T12-12.xml";
            var measurementFile = new MeasurementFile(inputFile);
            var sut = new MeasurementReader();

            // Act
            var res = sut.ReadMeasurement(measurementFile);

            // Assert
            res.AmountSold.Should().Be(1352);
            res.AmountSeasonTicket.Should().Be(8098);
            res.AmountAvailable.Should().Be(9201);
            res.AmountReserved.Should().Be(2933);
            res.AmountUnavailable.Should().Be(0);
            res.AmountTicketMaster.Should().Be(0);
            res.AmountUnknown.Should().Be(61);
        }


        #endregion

        #endregion
    }
}
