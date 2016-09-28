using FluentAssertions;
using Teller.Core.Classification;
using Teller.Core.Entities;
using Xunit;

namespace Teller.Tests.Classification
{
    public class SeatClassificationRuleTests
    {
        public class IsMatchTests
        {
            [Fact]
            public void IsMatch_WhenRuleIsMatch_ReturnsTrue()
            {
                // Arrange
                var sut = new SeatClassificationRule
                {
                    RuleName = "Test rule",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1"
                };

                var code = new EttCode("000004130001");

                // Act
                var res = sut.IsMatch(code);

                // Assert
                res.Should().BeTrue("BaseType equals 1 in this case");
            }
        }
    }
}
