using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Teller.Charts.Helpers;
using Teller.Core.Entities;
using Teller.Core.Infrastructure;

namespace Teller.Charts.Viewmodels
{
    public class SingleMatchViewmodel : BaseViewModel
    {
        #region Fields

        private DateTime _matchStart;

        #endregion

        #region Public properties

        public DateTime LastUpdateTime { get; set; }
        public string Opponent { get; private set; }
        public string Tournament { get; private set; }

        public string MatchStart
        {
            get
            {
                var noDtFormat = new CultureInfo("no", false).DateTimeFormat;

                return String.Format("{0} {1}/{2}-{3} kl. {4}",
                    UppercaseFirst(noDtFormat.GetDayName(_matchStart.DayOfWeek)),
                    _matchStart.Day,
                    _matchStart.Month,
                    _matchStart.Year,
                    _matchStart.ToString(noDtFormat.ShortTimePattern));
            }
        }

        public string Location { get; private set; }
        public string EnemyLogoPath { get; private set; }

        public string UpdateTime 
        { 
            get
            {
                var now = LastUpdateTime; // DateTime.Now
                return String.Format("Siste måling {0}/{1} kl. {2:00}:{3:00}", now.Day, now.Month, now.Hour, now.Minute);
            }  
        }

        public int SoldTickets { get; set; }
        public int SoldSeasonTickets { get; set; }
        public int ReservedUnknownTickets { get; set; }
        public int TicketsForSale { get; set; }
        public int GrandSumTotal { get { return SoldTickets + SoldSeasonTickets; } }

        public ObservableCollection<StautSeries> DataSeries { get; set; }

        #endregion

        /// <summary>
        /// Constructor som kun blir brukt av Visual Studio i design mode - for å ha litt data.
        /// </summary>
        public SingleMatchViewmodel()
        {
            Opponent = "Molde";
            Tournament = "Tippeligaen 2015";
            _matchStart = new DateTime(2015, 6, 27, 20, 00, 00);
            Location = "Lerkendal Stadion";
            EnemyLogoPath = @"D:\temp\staut\logo\Molde.png";

            SoldTickets = 10000;
            SoldSeasonTickets = 8100;
            ReservedUnknownTickets = 4800;
            TicketsForSale = 17;

            LastUpdateTime = new DateTime(2015, 06, 27, 20, 00, 00).Subtract(new TimeSpan(30, 0, 0, 0));

            DataSeries = new ObservableCollection<StautSeries>
            {
                new StautSeries
                {
                    Title = "Rosenborg - molde",
                    Start = new DateTime(2015, 06, 27, 20, 00, 00),
                    Points = new List<StautPoint>
                    {
                        new StautPoint {XValue = 30, YValue = 8100},
                        new StautPoint {XValue = 0, YValue = 21404}
                    }
                }
            };
        }

        public SingleMatchViewmodel(BillettServiceEvent bsEvent)
        {
            Opponent = bsEvent.Opponent;
            Tournament = CreateTournamentName(bsEvent);

            Location = bsEvent.Location == "TLD" ? "Lerkendal Stadion" : "";
            _matchStart = bsEvent.Start;
            
            var logoPath = StautConfiguration.Current.LogoDirectory ?? Environment.CurrentDirectory;
            EnemyLogoPath = Path.Combine(logoPath, Opponent.Replace("/", "_") + ".png"); // Helvetes Bodø/Glimt.
            if (!File.Exists(EnemyLogoPath))
                EnemyLogoPath = Path.Combine(logoPath, "NoLogo.png");

            var lastMeasurement = bsEvent.Measurements.LastOrDefault();
            if (lastMeasurement != null)
            {
                SoldTickets = lastMeasurement.AmountSold;
                SoldSeasonTickets = lastMeasurement.AmountSeasonTicket;
                ReservedUnknownTickets = lastMeasurement.AmountReserved + lastMeasurement.AmountUnknown;
                TicketsForSale = lastMeasurement.AmountAvailable;   
            }

            LastUpdateTime = bsEvent.Measurements
                                    .OrderByDescending(m => m.MeasurementTime)
                                    .First()
                                    .MeasurementTime;

            var reducedMeasurements = new PointReducer().Reduce(bsEvent.Measurements);

            DataSeries = new ObservableCollection<StautSeries>
            {
                new StautSeries
                {
                    Title = "Rosenborg - " + bsEvent.Opponent,
                    Start = bsEvent.Start,
                    Points =
                        new List<StautPoint>(
                            reducedMeasurements
                                   .Where(m => m.TotalAmountSold > 0)
                                   .Select(
                                m =>
                                    new StautPoint
                                    {
                                        XValue = bsEvent.Start.Subtract(m.MeasurementTime).TotalDays,
                                        YValue = m.TotalAmountSold
                                    }))
                }
            };
        }

        public void UpdateAll()
        {
            OnPropertyChanged(String.Empty);
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public string CreateTournamentName(BillettServiceEvent bsEvent)
        {
            var year = bsEvent.Start.Year;
            switch (bsEvent.Tournament)
            {
                case "LEAGUE":
                    if (year >= 2017)
                        return "Elite Serien " + year; // Haw haw haw
                    return "Tippeligaen " + year;
                case "NM":
                    return "Norgesmesterskapet " + year;
                case "EC":
                    return "Europacup";
                default:
                    return String.Empty;
            }
        }
    }
}
