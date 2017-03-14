using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Teller.Core.Entities;

namespace PowerSTAut
{
    [Cmdlet(VerbsCommon.Get, "FilteredSeats")]
    public class FilterSeats : PSCmdlet
    {
        #region Fields

        private string _position;
        private string[] _code;
        private Seat _sete;
        private int[] _codeValues;

        #endregion

        #region Parameters
        
        [Parameter, ValidateNotNullOrEmpty, Alias("pos")]
        public string Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [Parameter, ValidateNotNullOrEmpty]
        public string[] Code
        {
            get { return _code; }
            set { _code = value; }
        }

        [Parameter(ValueFromPipeline = true, ValueFromRemainingArguments = true)]
        public Seat Seat
        {
            get { return _sete; }
            set { _sete = value; }
        }

        #endregion

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            var codes = new List<int>();

            if(_code != null && _code.Length >= 1)
            {
                foreach(var code in _code)
                {
                    if (!String.IsNullOrWhiteSpace(code))
                        codes.Add(int.Parse(code, NumberStyles.HexNumber));
                }
                _codeValues = codes.ToArray();
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                var seatPassesFilter = true;

                if(!String.IsNullOrWhiteSpace(Position))
                {
                    var pos = new StadiumPosition(Seat.SectionName, Seat.RowName, Seat.SeatName);
                    if (!pos.IsMatch(Position))
                        seatPassesFilter = false;
                }

                if(_codeValues != null)
                {
                    if (!_codeValues.Any(cv => cv == Seat.EttCode.CodeValue))
                        seatPassesFilter = false;
                }

                if(seatPassesFilter)
                    WriteObject(Seat);
            }
            catch (Exception)
            {
            }
        }
    }
}
