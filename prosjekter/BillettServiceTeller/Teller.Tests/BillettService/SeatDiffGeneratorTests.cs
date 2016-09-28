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
            var fileA = @"D:\temp\staut\archive\til\476973_TromsÃ¸_2016-08-16T10-48.xml";
            var fileB = @"D:\temp\staut\archive\til\476973_TromsÃ¸_2016-08-16T11-28.xml";
            
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
