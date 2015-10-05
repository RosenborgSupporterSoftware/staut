using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teller.Core.Entities;

namespace Teller.Charts.Helpers
{
    /// <summary>
    /// A class that removes points that add no value from a sequence
    /// </summary>
    public class PointReducer
    {
        public IEnumerable<Measurement> Reduce(IEnumerable<Measurement> measurements)
        {
            if (measurements == null)
                yield break;

            var lastValue = -1;
            Measurement candidate = null;

            foreach (var measurement in measurements)
            {
                if (measurement.TotalAmountSold != lastValue)
                {
                    // Ny verdi. Do the stuff.
                    if (candidate != null)
                    {
                        yield return candidate;
                        candidate = null;
                    }

                    lastValue = measurement.TotalAmountSold;
                    yield return measurement;
                }
                else
                    candidate = measurement;
            }
            if(candidate != null)
                yield return candidate;
        }
    }
}
