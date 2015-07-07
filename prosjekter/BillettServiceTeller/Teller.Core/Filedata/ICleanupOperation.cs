using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teller.Core.Filedata
{
    /// <summary>
    /// Overengineering 101 - et interface for diverse tiltak vi gjør for å rydde opp i filstrukturen
    /// </summary>
    public interface ICleanupOperation
    {
        /// <summary>
        /// Prioritet - rekkefølge som operasjonene kjøres i. Lavere = tidligere
        /// </summary>
        double Priority { get; }

        /// <summary>
        /// Utfør selve cleanup-jobben
        /// </summary>
        void PerformCleanup();
    }
}
