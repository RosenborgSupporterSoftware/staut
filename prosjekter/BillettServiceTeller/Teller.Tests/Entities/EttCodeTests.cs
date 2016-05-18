using System;
using System.Linq;
using FluentAssertions;
using Teller.Core.Entities;
using Xunit;

namespace Teller.Tests.Entities
{
    public class EttCodeTests
    {
        #region Constructor tests

        [Fact]
        public void Constructor_WhenGivenNullInput_ThrowsArgumentException()
        {
            // Arrange
            EttCode code;

            // Act
            Assert.Throws<ArgumentException>(() => code = new EttCode(null));

            // Assert
        }

        [Fact]
        public void Constructor_WhenGivenEmptyInput_ThrowsArgumentException()
        {
            // Arrange
            EttCode code;

            // Act
            Assert.Throws<ArgumentException>(() => code = new EttCode(String.Empty));

            // Assert
        }

        [Fact]
        public void Constructor_GivenGoodValue_DoesNotThrow()
        {
            // Arrange
            
            // Act
            var sut = new EttCode("000004C70001");

            // Assert
            sut.Should().NotBeNull("because that would be bad");
        }

        #endregion

        #region QualifierBitsHex tests

        [Fact]
        public void QualifierBitsHex_WhenGivenTypicalEttCode_ReturnsExpectedValue()
        {
            // Arrange
            const string ettCode = "000004C70001";
            var sut = new EttCode(ettCode);

            // Act
            var res = sut.QualifierBitsHex;

            // Assert
            res.Should().Be("4C7", "those are the qualifier bits of the ETT code");
        }

        #endregion

        #region SeatFlagsHex tests

        [Fact]
        public void SeatFlagsHex_WhenGivenTypicalEttCode_ReturnsExpectedValue()
        {
            // Arrange
            const string ettCode = "000004C70001";
            var sut = new EttCode(ettCode);

            // Act
            var res = sut.SeatFlagsHex;

            // Assert
            res.Should().Be("00", "those are the seat flags of the ETT code");
        }

        #endregion

        #region BaseTypeHex tests

        [Fact]
        public void BaseTypeHex_WhenGivenTypicalEttCode_ReturnsExpectedValue()
        {
            // Arrange
            const string ettCode = "000004C70001";
            var sut = new EttCode(ettCode);

            // Act
            var res = sut.BaseTypeHex;

            // Assert
            res.Should().Be("01", "that is the base type of the ETT code");
        }

        #endregion

        #region Code tests

        [Fact]
        public void Code_WhenGivenTypicalEttCode_ReturnsExpectedValue()
        {
            // Arrange
            // "000004C80001" == 00000000 00000000 00000100 11001000 00000000 00000001
            const string ettCode = "000004C70001";
            var sut = new EttCode(ettCode);

            // Act
            var res = sut.Code;

            // Assert
            res.Should().Be(ettCode, "it should match the original input");
        }

        #endregion

    }
}
