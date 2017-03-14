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
            const string inputFile = @"..\..\TestData\438533_Viking_2015-09-08T10-57.xml";
            var measurementFile = new MeasurementFile(inputFile);
            var sut = new MeasurementReader();

            // Act
            var res = sut.ReadMeasurement(measurementFile);

            // Assert
            res.AmountSold.Should().Be(375);
            res.AmountSeasonTicket.Should().Be(8161);
            res.AmountAvailable.Should().Be(9610);
            res.AmountReserved.Should().Be(3280);
            res.AmountUnavailable.Should().Be(0);
            res.AmountTicketMaster.Should().Be(0);
            res.AmountUnknown.Should().Be(216);
        }


        #endregion

        #endregion
    }
}
