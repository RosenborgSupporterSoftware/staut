using System.Linq;
using System.Management.Automation;
using Teller.Core.BillettService;

namespace PowerSTAut.Commandlets
{
    [Cmdlet(VerbsCommunications.Read, "Seats")]
    public class ReadSeats : PSCmdlet
    {
        #region Parameters

        [Parameter(Position = 0, Mandatory = true)]
        public string Filename { get { return _filename; } set { _filename = value; } }
        private string _filename;

        #endregion

        #region Cmdlet functionality

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var reader = new BillettServiceSeteLeser();
            var file = BillettServiceXmlFile.LoadFile(_filename);
            var seats = reader.ReadSeats(file).ToList();

            WriteVerbose("Antall seter: " + seats.Count);

            try
            {
                WriteObject(seats.Select(s => new Seat(s)), true);
            }
            catch (PipelineStoppedException e)
            {
            }
        }

        #endregion
    }
}
