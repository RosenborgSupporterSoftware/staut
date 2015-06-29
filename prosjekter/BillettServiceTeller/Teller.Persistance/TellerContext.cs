using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teller.Core.Entities;

namespace Teller.Persistance
{
    public class TellerContext : DbContext
    {
        /// <summary>
        /// Kampene vi teller
        /// </summary>
        public DbSet<BillettServiceEvent> Events { get; set; }

        /// <summary>
        /// Målinger gjort på kamper
        /// </summary>
        public DbSet<Measurement> Measurements { get; set; }

        public TellerContext()
            : base("STAut")
        {
            
        }
    }
}
