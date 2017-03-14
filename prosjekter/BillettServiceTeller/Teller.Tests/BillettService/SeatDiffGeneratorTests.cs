using System.Linq;
using System.Text;
using FluentAssertions;
using Teller.Core;
using Teller.Core.BillettService;
using Xunit;

namespace Teller.Tests.BillettService
{
    public class SeatDiffGeneratorTests
    {
        [Fact]
        public void GenerateDiff_WhenCalledWithTwoDifferentFiles_ReturnsExpectedResults()
        {
            // Arrange
            var sut = new SeatDiffGenerator();
            var fileA = @"..\..\TestData\438533_Viking_2015-09-08T10-57.xml";
            var fileB = @"..\..\TestData\438533_Viking_2015-09-25T09-47.xml";
            
            // Act
            var res = sut.GenerateDiff(fileA, fileB)
                         .ToList();

            var builder = new StringBuilder();
            foreach (var seatChange in res)
            {
                builder.AppendLine(seatChange.ToString());
            }

            var r = builder.ToString();

            // Assert
            res.Count.Should().NotBe(0);
        }

        [Fact]
        public void Mystisk_MFK_dupp()
        {
            // Arrange
            var sut = new SeatDiffGenerator();
            var fileA = @"..\..\TestData\0604mfk1500.xml";
            var fileB = @"..\..\TestData\0604mfk1800.xml";

            // Act
            var res = sut.GenerateDiff(fileA, fileB)
                         //.Where(sc => SeatStatusClassifier.Classify(sc.ToEtt) == SeatStatus.AvailableForPurchase)
                         .ToList();

            var builder = new StringBuilder();
            foreach (var seatChange in res)
            {
                builder.AppendLine(seatChange.ToString());
            }

            var r = builder.ToString();

            // Assert
            res.Count.Should().NotBe(0);
        }
    }
}
