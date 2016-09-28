using System.Linq;
using FluentAssertions;
using Teller.Core.Classification.Operators;
using Xunit;

namespace Teller.Tests.Classification.Operators
{
    public class OperatorManagerTests
    {
        [Fact]
        public void GetOperator_WhenAskedForValidOperator_ReturnsExpectedObject()
        {
            // Arrange
            var sut = new OperatorManager();

            // Act
            var res = sut.GetOperator("Equals");

            // Assert
            res.Should().NotBeNull("we should get a valid object back");
            res.Should().BeOfType<EqualsOperator>("that is the operator we asked for");
        }

        [Fact]
        public void GetOperator_WhenAskedForSomethingThatDoesNotExist_ReturnsNull()
        {
            // Arrange
            var sut = new OperatorManager();

            // Act
            var res = sut.GetOperator("Magne Hoseth");

            // Assert
            res.Should().BeNull("we entered an invalid operator name");
        }

        [Fact]
        public void GetOperatorNames_WhenCalled_ReturnsListOfOperators()
        {
            // Arrange
            var sut = new OperatorManager();

            // Act
            var res = sut.GetOperatorNames().ToList();

            // Assert
            res.Should().NotBeNull("we should get a valid sequence back");
            res.Count.Should().BeGreaterThan(0, "we should have more than zero operators");
            res.Contains("Equals").Should().BeTrue("the list should contain the Equals operator");
        }

    }
}
