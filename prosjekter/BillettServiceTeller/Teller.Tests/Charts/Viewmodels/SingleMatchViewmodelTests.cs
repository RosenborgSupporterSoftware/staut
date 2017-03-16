using System;
using System.Collections.Generic;
using FluentAssertions;
using Teller.Charts.Viewmodels;
using Teller.Core.Entities;
using Xunit;

namespace Teller.Tests.Charts.Viewmodels
{
    public class SingleMatchViewmodelTests
    {
        #region CreateTournamentName tests

        private BillettServiceEvent CreateTournament()
        {
            var evt = new BillettServiceEvent
            {
                Tournament = "LEAGUE",
                Opponent = "Odd",
                Measurements = new List<Measurement>
                {
                    (new Measurement
                    {
                        AmountAvailable = 21667,
                        AmountReserved = 2000,
                        AmountSeasonTicket = 10000,
                        AmountSold = 1000,
                        AmountTicketMaster = 12,
                        AmountUnavailable = 100,
                        AmountUnknown = 100
                    })
                }
            };

            return evt;
        }

        [Fact]
        public void CreateTournamentName_WhenGivenEliteSerienEvent_ReturnsExpectedName()
        {
            // Arrange
            var evt = CreateTournament();
            evt.Start = new DateTime(2017, 4, 2, 20, 00, 00);
            var sut = new SingleMatchViewmodel(evt);

            // Act
            var res = sut.CreateTournamentName(evt);

            // Assert
            res.Should().Be("Elite Serien 2017", "that is the correct league name for this date");
        }

        [Fact]
        public void CreateTournamentName_WhenGivenTippeligaenEvent_ReturnsExpectedName()
        {
            // Arrange
            var evt = CreateTournament();
            evt.Start = new DateTime(2016, 4, 2, 20, 00, 00);
            var sut = new SingleMatchViewmodel(evt);

            // Act
            var res = sut.CreateTournamentName(evt);

            // Assert
            res.Should().Be("Tippeligaen 2016", "that is the correct league name for this date");
        }

        [Fact]
        public void CreateTournamentName_WhenGivenCupEvent_ReturnsExpectedName()
        {
            // Arrange
            var evt = CreateTournament();
            evt.Start = new DateTime(2016, 4, 2, 20, 00, 00);
            evt.Tournament = "NM";
            var sut = new SingleMatchViewmodel(evt);

            // Act
            var res = sut.CreateTournamentName(evt);

            // Assert
            res.Should().Be("Norgesmesterskapet 2016", "that is the correct tournament name for this date");
        }

        [Fact]
        public void CreateTournamentName_WhenGivenEuropeanCupEvent_ReturnsExpectedName()
        {
            // Arrange
            var evt = CreateTournament();
            evt.Start = new DateTime(2016, 4, 2, 20, 00, 00);
            evt.Tournament = "EC";
            var sut = new SingleMatchViewmodel(evt);

            // Act
            var res = sut.CreateTournamentName(evt);

            // Assert
            res.Should().Be("Europacup", "that is the correct tournament name for this date");
        }

        #endregion
    }
}
