using System.Collections.Generic;
using System.Linq;

namespace Teller.Core.Entities
{
    public class TribuneSeksjon
    {
        public string SeksjonsNavn { get; private set; }

        public IList<Rad> Rader { get; private set; }

        public int AntallSeter { get { return Rader.Sum(rad => rad.AntallSeter); } }

        public int AntallOpptatt { get { return Rader.Sum(rad => rad.AntallOpptatt); } }

        public int AntallLedige { get { return Rader.Sum(rad => rad.AntallLedig); } }

        public TribuneSeksjon(string seksjonsNavn)
        {
            SeksjonsNavn = seksjonsNavn;
            Rader = new List<Rad>();
        }
    }
}
