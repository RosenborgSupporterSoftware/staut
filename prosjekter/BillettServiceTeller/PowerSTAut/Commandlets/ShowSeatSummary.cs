using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PowerSTAut.Commandlets
{
    [Cmdlet(VerbsCommon.Show, "SeatSummary")]
    public class ShowSeatSummary : Cmdlet
    {
        #region Fields

        private List<Seat> _seats;
        private Seat _currentSeat;
        private bool _byCode, _byStatus, _bySection;

        #endregion

        #region Parameters

        [Parameter(ValueFromPipeline = true, ValueFromRemainingArguments = true)]
        public Seat Seat
        {
            get { return _currentSeat; }
            set { _currentSeat = value; }
        }

        [Parameter, Alias("ByCode", "Code")]
        public SwitchParameter SummarizeByCode
        {
            get { return _byCode; }
            set { _byCode = value; }
        }

        [Parameter, Alias("ByStatus", "Status")]
        public SwitchParameter SummarizeByStatus
        {
            get { return _byStatus; }
            set { _byStatus = value; }
        }

        [Parameter, Alias("BySection", "Section")]
        public SwitchParameter SummarizeBySection
        {
            get { return _bySection; }
            set { _bySection = value; }
        }

        #endregion

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            _seats = new List<Seat>();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            _seats.Add(_currentSeat);
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();

            var group = GetGroupValue();

            if (group == 0) // Group by EttCode
            {
                var summary = _seats.GroupBy(s => s.EttCode.Code)
                                    .Select(g => new SeatCodeSummary { EttCode = g.First().EttCode, Count = g.Count(), SeatStatus = g.First().Classification })
                                    .OrderByDescending(ss => ss.Count)
                                    .ToList();

                WriteObject(summary, true);
            }

            if(group == 1) // Group by classified status
            {
                var summary = _seats.GroupBy(s => s.Classification)
                                    .Select(g => new SeatStatusSummary { SeatStatus = g.First().Classification, Count = g.Count() })
                                    .OrderByDescending(ss => ss.Count)
                                    .ToList();

                WriteObject(summary, true);
            }

            if (group == 2) // Group by stadium section
            {
                var summary = _seats.GroupBy(s => s.SectionName)
                                    .Select(g => new SeatSectionSummary { Section = g.First().SectionName, Count = g.Count() })
                                    .OrderByDescending(ss => ss.Count)
                                    .ToList();

                WriteObject(summary, true);
            }

            WriteVerbose("Antall seter totalt: " + _seats.Count);
        }

        private int GetGroupValue()
        {
            if (_byCode)
                return 0;

            if (_byStatus)
                return 1;

            if (_bySection)
                return 2;

            return 0;
        }
    }
}
