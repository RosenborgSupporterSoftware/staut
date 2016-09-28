using System.Linq;
using FluentAssertions;
using Teller.Core;
using Teller.Core.Classification;
using Teller.Core.Entities;
using Xunit;

namespace Teller.Tests.Classification
{
    public class SeatClassificationEngineTests
    {
        public class UseExperimentalRules
        {
            [Fact]
            public void UseExperimentalRules_WhenSetToFalse_HidesExperimentalRules()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                var rule = new SeatClassificationRule
                {
                    RuleName = "Test rule",
                    Notes = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Reserved,
                    Order = 1
                };

                // Act
                sut.AddRule(rule, true);

                // Assert
                sut.AllRules.Contains(rule).Should().BeFalse("we added it as an experimental rule");
                sut.UseExperimentalRules = true;
                sut.AllRules.Contains(rule).Should().BeTrue("the rule should now be visible");
            }
        }

        public class AllRules
        {
            [Fact]
            public void AddRules_WhenCalled_ReturnsAllExpectedData()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                var normalRule = new SeatClassificationRule
                {
                    RuleName = "Normal rule",
                    Notes = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Reserved,
                    Order = 1
                };
                var experimentalRule = new SeatClassificationRule
                {
                    RuleName = "Experimental rule",
                    Notes = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Reserved,
                    Order = 1
                };
                sut.UseExperimentalRules = true;
                sut.AddRule(normalRule, false);
                sut.AddRule(experimentalRule, true);

                // Act
                var res = sut.AllRules.ToList();

                // Assert
                res.Should().NotBeNull("we should end up with a valid, populated list");
                res.Count.Should().Be(2, "we added two rules to the engine");
                res.Contains(normalRule).Should().BeTrue("this rule should be in the list");
                res.Contains(experimentalRule).Should().BeTrue("this rule should be in the list");
            }

        }

        public class AddRule
        {
            [Fact]
            public void AddRule_WithNoExperimentalRule_AddsRuleToList()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                var rule = new SeatClassificationRule
                {
                    RuleName = "Test rule",
                    Notes = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Reserved,
                    Order = 1
                };

                // Act
                sut.AddRule(rule, false);

                // Assert
                sut.AllRules.Contains(rule).Should().BeTrue("we added it as a non-experimental rule");
            }

            [Fact]
            public void AddRule_WithExperimentalRule_AddsRuleToList()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                var rule = new SeatClassificationRule
                {
                    RuleName = "Test rule",
                    Notes = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Reserved,
                    Order = 1
                };
                sut.UseExperimentalRules = true;

                // Act
                sut.AddRule(rule, true);

                // Assert
                sut.AllRules.Contains(rule).Should().BeTrue("we set UseExperimentalRules to true");
            }

        }

        public class RemoveRule
        {
            [Fact]
            public void RemoveRule_CalledWithProperRule_RemovesRuleFromList()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                var rule = new SeatClassificationRule
                {
                    RuleName = "Test rule",
                    Notes = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Reserved,
                    Order = 1
                };
                sut.AddRule(rule, false);
                sut.AllRules.Count().Should().Be(1, "the rule should be added to the list");

                // Act
                sut.RemoveRule(rule);

                // Assert
                sut.AllRules.Contains(rule).Should().BeFalse("we removed this rule");
            }
        }

        public class ToggleExperimental
        {
            [Fact]
            public void ToggleExperimental_WhenCalledOnExistingRule_ActuallyTogglesExperimentalStatus()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                var rule = new SeatClassificationRule
                {
                    RuleName = "Test rule",
                    Notes = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Reserved,
                    Order = 1
                };
                sut.AddRule(rule, false);
                sut.AllRules.Count().Should().Be(1, "the rule should be added to the list");

                // Act
                sut.ToggleExperimental(rule);

                // Assert
                sut.AllRules.Contains(rule).Should().BeFalse("the rule should now not be visible");
            }
        }

        public class Classify
        {
            [Fact]
            public void Classify_WhenMatchingRuleExists_ClassifiesCorrectly()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                sut.AddRule(new SeatClassificationRule
                            {
                                RuleName = "Test",
                                Field = ClassificationRuleField.BaseType,
                                Operator = "Equals",
                                Value = "1",
                                Status = SeatStatus.Sold
                            }, false);
                var code = new EttCode("000004130001");

                // Act
                var res = sut.Classify(code);

                // Assert
                res.Should().Be(SeatStatus.Sold, "the rule should match");
            }
        }

        public class GetMatchingRule
        {
            [Fact]
            public void GetMatchingRule_WhenMatchingRuleExists_ReturnsCorrectRule()
            {
                // Arrange
                var sut = new SeatClassificationEngine();
                sut.AddRule(new SeatClassificationRule
                {
                    RuleName = "Test",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.Sold
                }, false);
                var wantedRule = new SeatClassificationRule
                {
                    RuleName = "Test 2",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "0",
                    Status = SeatStatus.Unknown
                };
                sut.AddRule(wantedRule, false);
                var code = new EttCode("000004130000");

                // Act
                var res = sut.GetMatchingRule(code);

                // Assert
                res.Should().BeSameAs(wantedRule, "this is the rule that should match");
            }
        }
    }
}
