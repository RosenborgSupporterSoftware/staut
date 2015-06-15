using System;
using FluentAssertions;
using Teller.Core.Entities;
using Xunit;

namespace Teller.Tests.Entities
{
    public class StadiumPositionTests
    {
        #region Constructor tests

        [Fact]
        public void Constructor_WhenGivenSaneProperties_SetsTheCorrectProperties()
        {
            // Arrange
            
            // Act
            var sut = new StadiumPosition("SEC", "ROW", "SEAT");

            // Assert
            sut.SectionName.Should().Be("SEC", "that is the section name given");
            sut.RowName.Should().Be("ROW", "that is the row name given");
            sut.SeatName.Should().Be("SEAT", "that is the seat name given");
        }

        [Fact]
        public void Constructor_WhenGivenNoRowName_StillFunctionsProperly()
        {
            // Arrange
            
            // Act
            var sut = new StadiumPosition("KJ", String.Empty, "1100");

            // Assert
            sut.SectionName.Should().Be("KJ", "that is the section name given");
            sut.RowName.Should().Be(String.Empty, "that is the row name given");
            sut.SeatName.Should().Be("1100", "that is the seat name given");
        }

        #endregion

        #region ToString tests

        [Fact]
        public void ToString_WhenCalled_ProducesExpectedOutput()
        {
            // Arrange
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.ToString();
            
            // Assert
            res.Should().Be("BA:12:34", "those are the 'coordinates' of the seat");
        }

        #endregion

        #region IsMatch tests

        [Fact]
        public void IsMatch_WhenGivenExactMatch_ReturnsTrue()
        {
            // Arrange
            var filter = "BA:12:34";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeTrue("this is an exact match");
        }

        [Fact]
        public void IsMatch_WhenGivenWrongSection_ReturnsFalse()
        {
            // Arrange
            var filter = "QD:12:34";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeFalse("the section is different");
        }

        [Fact]
        public void IsMatch_WhenGivenWrongRow_ReturnsFalse()
        {
            // Arrange
            var filter = "BA:12:1";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeFalse("the row is different");
        }

        [Fact]
        public void IsMatch_WhenGivenOnlySectionThatMatches_ReturnsTrue()
        {
            // Arrange
            var filter = "BA";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeTrue("we only care about the section - which is correct");
        }

        [Fact]
        public void IsMatch_WhenGivenOnlySectionThatDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var filter = "BA";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeTrue("we only care about the section - which is correct");
        }

        [Fact]
        public void IsMatch_WhenGivenOnlyRowThatMatches_ReturnsTrue()
        {
            // Arrange
            var filter = ":12";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeTrue("we only care about the row - which is correct");
        }

        [Fact]
        public void IsMatch_WhenGivenOnlyRowThatDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var filter = ":15";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeFalse("we only care about the row - which is wrong");
        }

        [Fact]
        public void IsMatch_WhenGivenOnlySeatThatMatches_ReturnsTrue()
        {
            // Arrange
            var filter = "::34";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeTrue("we only care about the seat - which is correct");
        }

        [Fact]
        public void IsMatch_WhenGivenOnlySeatThatDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var filter = "::35";
            var sut = new StadiumPosition("BA", "12", "34");

            // Act
            var res = sut.IsMatch(filter);

            // Assert
            res.Should().BeFalse("we only care about the seat - which is wrong");
        }

        #endregion
    }
}
