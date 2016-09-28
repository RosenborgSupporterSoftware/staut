using System;
using System.Linq;
using System.Management.Automation;

namespace PowerSTAut.Commandlets
{
    [Cmdlet(VerbsCommon.Get, "SeatDiff")]
    public class GetSeatDiff : PSCmdlet
    {
        #region Fields

        private Seat[] _firstStream, _secondStream;

        #endregion

        #region Parameters

        [Parameter(Mandatory = true, Position = 0)]
        public Object[] FirstStream
        {
            get
            {
                return _firstStream;
            }
            set
            {
                _firstStream = value.OfType<Seat>().ToArray();
            }
        }

        [Parameter(Mandatory = true, Position = 1)]
        public Object[] SecondStream
        {
            get
            {
                return _secondStream;
            }
            set
            {
                _secondStream = value.OfType<Seat>().ToArray();
            }
        }

        #endregion

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            Console.WriteLine("First: {0} Second: {1}", _firstStream.Count(), _secondStream.Count());
        }
    }
}
