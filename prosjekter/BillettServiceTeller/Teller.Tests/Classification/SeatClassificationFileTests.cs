using System.IO;
using System.Linq;
using FluentAssertions;
using Teller.Core;
using Teller.Core.Classification;
using Xunit;

namespace Teller.Tests.Classification
{
    public class SeatClassificationFileTests
    {
        public class SaveAndLoad
        {
            [Fact]
            public void Save_WhenCalledWithNonEmptyFileObject_StoresFileToDisk()
            {
                // Arrange
                var sut = new SeatClassificationFile();
                sut.Add(new SeatClassificationRule
                        {
                            RuleName = "Test",
                            Field = ClassificationRuleField.BaseType,
                            Operator = "Equals",
                            Value = "1",
                            Status = SeatStatus.AvailableForPurchase
                        });
                var path = "ClassificationRules.json";

                if(File.Exists(path))
                    File.Delete(path);

                // Act
                sut.Save(path);

                // Assert
                File.Exists(path).Should().BeTrue("the file should have been saved");
                File.Delete(path);
            }

            [Fact]
            public void Load_WhenLoadingSavedFile_GetsCorrectData()
            {
                // Arrange
                var sut = new SeatClassificationFile();
                sut.Add(new SeatClassificationRule
                {
                    RuleName = "Test",
                    Notes = "Test notes",
                    Field = ClassificationRuleField.BaseType,
                    Operator = "Equals",
                    Value = "1",
                    Status = SeatStatus.AvailableForPurchase
                });
                var path = "ClassificationRulesLoadAndSave.json";

                if (File.Exists(path))
                    File.Delete(path);

                // Act
                sut.Save(path);
                var res = SeatClassificationFile.Load(path);
                File.Delete(path);

                // Assert
                res.Rules.Count().Should().Be(1, "we should have a single rule");
                var rule = res.Rules.Single();
                rule.RuleName.Should().Be("Test", "that was the name given to the original rule");
                rule.Notes.Should().Be("Test notes");
                rule.Field.Should().Be(ClassificationRuleField.BaseType, "that was the original content of Field");
                rule.Operator.Should().Be("Equals");
                rule.Value.Should().Be("1");
                rule.Status.Should().Be(SeatStatus.AvailableForPurchase);
            }

        }
    }
}
