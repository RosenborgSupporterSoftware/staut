using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Teller.Charts.Helpers;
using Teller.Core.Entities;
using Xunit;

namespace Teller.Tests.Charts.Helpers
{
    public class PointReducerTests
    {
        #region Reduce tests

        [Fact]
        public void Reduce_WhenGivenNullValue_ReturnsEmptySequence()
        {
            // Arrange
            var sut = new PointReducer();

            // Act
            var res = sut.Reduce(null).ToList();

            // Assert
            res.Should().NotBeNull("we should get an empty sequence");
            res.Should().BeEmpty("there should be nothing here");
        }


        [Fact]
        public void Reduce_WhenGivenEmptySequence_ReturnsEmptySequence()
        {
            // Arrange
            var sut = new PointReducer();

            // Act
            var res = sut.Reduce(Enumerable.Empty<Measurement>()).ToList();

            // Assert
            res.Count.Should().Be(0, "the source list was empty");
        }

        [Fact]
        public void Reduce_WhenGivenSequenceWithNoPossibilityOfReduction_ReturnsSameSequence()
        {
            // Arrange
            var points = new List<Measurement>
            {
                new Measurement { MeasurementTime = new DateTime(2015, 10, 4, 15, 00, 00), AmountSold = 1000, AmountSeasonTicket = 1000 },
                new Measurement { MeasurementTime = new DateTime(2015, 10, 4, 15, 10, 00), AmountSold = 1050, AmountSeasonTicket = 1000 },
                new Measurement { MeasurementTime = new DateTime(2015, 10, 4, 15, 20, 00), AmountSold = 1080, AmountSeasonTicket = 1000 }
            };
            var sut = new PointReducer();

            // Act
            var res = sut.Reduce(points).ToList();

            // Assert
            res.Count.Should().Be(3, "the three have different values");
            res[0].Should().BeSameAs(points[0]);
            res[1].Should().BeSameAs(points[1]);
            res[2].Should().BeSameAs(points[2]);
        }

        [Fact]
        public void Reduce_WhenGivenSequenceWithPossibilityOfReduction_ReturnsReducedSequence()
        {
            // Arrange
            var points = new List<Measurement>
            {
                new Measurement { MeasurementTime = new DateTime(2015, 10, 4, 15, 00, 00), AmountSold = 1000, AmountSeasonTicket = 1000 },
                new Measurement { MeasurementTime = new DateTime(2015, 10, 4, 15, 10, 00), AmountSold = 1000, AmountSeasonTicket = 1000 },
                new Measurement { MeasurementTime = new DateTime(2015, 10, 4, 15, 20, 00), AmountSold = 1000, AmountSeasonTicket = 1000 }
            };
            var sut = new PointReducer();

            // Act
            var res = sut.Reduce(points).ToList();

            // Assert
            res.Count.Should().Be(2, "the three have the same value");
            res[0].Should().BeSameAs(points[0]);
            res[1].Should().BeSameAs(points[2]);
        }

        #endregion
    }
}
