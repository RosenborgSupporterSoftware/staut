using FluentAssertions;
using Teller.Core.Classification.Operators;
using Xunit;

namespace Teller.Tests.Classification.Operators
{
    public class GreaterThanOperatorTests
    {
        public class DefinitionTests
        {
            [Fact]
            public void GetOperator_WhenCalledWithGreaterThan_ReturnsExpectedClass()
            {
                // Arrange
                var mgr = new OperatorManager();

                // Act
                var sut = mgr.GetOperator("GreaterThan");

                // Assert
                sut.Should().NotBeNull("the object should be properly set up to be returned from the OperatorManager");
            }
        }

        public class FunctionalityTests
        {
            [Fact]
            public void OperatorIsMatch_WhenValueIsGreaterThan_ReturnsTrue()
            {
                // Arrange
                var sut = new GreaterThanOperator();
                sut.SetParameter("10");

                // Act
                var res = sut.OperatorIsMatch(50);

                // Assert
                res.Should().BeTrue("50 is greater than 10");
            }

            [Fact]
            public void OperatorIsMatch_WhenValueIsNotGreaterThan_ReturnsFalse()
            {
                // Arrange
                var sut = new GreaterThanOperator();
                sut.SetParameter("100");

                // Act
                var res = sut.OperatorIsMatch(20);

                // Assert
                res.Should().BeFalse("20 is not greater than 100");
            }

            [Fact]
            public void OperatorIsMatch_WhenValuesAreEqual_ReturnsFalse()
            {
                // Arrange
                var sut = new GreaterThanOperator();
                sut.SetParameter("7");

                // Act
                var res = sut.OperatorIsMatch(7);

                // Assert
                res.Should().BeFalse("7 is not greater than 7");
            }
        }
    }
}
