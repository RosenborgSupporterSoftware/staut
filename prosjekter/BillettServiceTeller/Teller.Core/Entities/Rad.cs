using System.Linq;

namespace Teller.Core.Entities
{
    /// <summary>
    /// En klasse som representerer en rad på en tribuneseksjon
    /// </summary>
    public class Rad
    {
        #region Fields

        private string _seatsAvail;

        #endregion

        public string RadNummer { get; set; }

        public string SeteTilgjengelighet
        {
            get { return _seatsAvail; }
            set
            {
                _seatsAvail = value;
                UpdateCount();
            }
        }

        public int AntallSeter { get; private set; }

        public int AntallOpptatt { get; private set; }
        public int AntallLedig { get; private set; }

        public Rad(string radNummer, string seteTilgjengelighet)
        {
            RadNummer = radNummer;
            SeteTilgjengelighet = seteTilgjengelighet;
        }

        private void UpdateCount()
        {
            AntallOpptatt = _seatsAvail.Count(c => (c == 'A'));
            AntallLedig = _seatsAvail.Count(c => (c == 'O'));

            AntallSeter = AntallOpptatt + AntallLedig;
        }
    }
}
