using System.Collections.Generic;
using System.Linq;

namespace Teller.Core.Entities
{
    public class Tribune
    {
        public string Navn { get; private set; }

        public IList<TribuneSeksjon> Seksjoner { get; private set; }

        public int AntallLedige
        {
            get
            {
                return Seksjoner.Sum(seksjon => seksjon.AntallLedige);
            }
        }

        public int AntallOpptatt
        {
            get
            {
                return Seksjoner.Sum(seksjon => seksjon.AntallOpptatt);
            }
        }

        public int AntallSeter
        {
            get { return AntallLedige + AntallOpptatt; }
        }

        public Tribune(string navn)
        {
            Navn = navn;
            Seksjoner = new List<TribuneSeksjon>();
        }
    }
}
