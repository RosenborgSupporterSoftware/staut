using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teller.Persistance;
using Teller.Persistance.Implementations;

namespace Ingest
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new EventRepository(new TellerContext());

            var stuff = db.Get(1);
        }
    }
}
