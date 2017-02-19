using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Teller.Core.BillettService;

namespace PowerSTAut.Commandlets
{
    [Cmdlet(VerbsCommon.Get, "SeatDiff")]
    public class GetSeatDiff : PSCmdlet
    {
        #region Fields

        private List<BillettServiceSete> _seats;
        private Seat _sete;

        #endregion

        #region Parameters

        [Parameter(Position = 0, Mandatory = true)]
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        private string _filename;

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

            var reader = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(_filename);
            _seats = reader.ReadSeats(file).ToList();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var ourSeat = _seats.FirstOrDefault(s => s.Position == Seat.Position);

            if (ourSeat == null)
                return;

            _seats.Remove(ourSeat);
                // Fjerner det aktuelle setet fra lista, så blir det mindre å lete gjennom neste runde.

            if (ourSeat.EttCode.Equals(Seat.EttCode))
                return;

            WriteVerbose("Sete endret: " + ourSeat.Position + ": " + ourSeat.EttCode + " -> " + Seat.EttCode);

            // TODO: Skriv et setediff-objekt til strømmen.
            try
            {
                WriteObject(new SeatDiff(Seat, new Seat(ourSeat)));
            }
            catch (PipelineStoppedException e)
            {
                WriteWarning("Caught exception: " + e.Message);
            }
        }
    }
}
