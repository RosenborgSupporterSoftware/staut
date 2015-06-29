using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teller.Persistance;

namespace Ingest
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new TellerContext();

            foreach (var billettServiceEvent in db.Events)
            {
                Console.WriteLine(billettServiceEvent.DisplayName);
            }
        }
    }
}
