using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Teller.Core.BillettService;
using Teller.Core.Entities;

namespace PowerSTAut
{
    [Cmdlet(VerbsCommon.Get, "FilteredSeats")]
    public class FilterSeats : PSCmdlet
    {
        #region Fields

        private string _filter;
        private BillettServiceSete _sete;

        #endregion

        #region Parameters

        [Parameter, ValidateNotNullOrEmpty]
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        [Parameter(ValueFromPipeline = true, ValueFromRemainingArguments = true)]
        public BillettServiceSete Seat
        {
            get { return _sete; }
            set { _sete = value; }
        }

        #endregion

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var pos = new StadiumPosition(Seat.SectionName, Seat.RowName, Seat.SeatName);
            if (pos.IsMatch(Filter))
                WriteObject(Seat);
        }
    }
}
