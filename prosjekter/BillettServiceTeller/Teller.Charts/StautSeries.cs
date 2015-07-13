using System.Collections.Generic;

namespace Teller.Charts
{
    /// <summary>
    /// En klasse som representerer en "serie" i en graf, typisk en kamp
    /// </summary>
    public class StautSeries
    {
        public string Title { get; set; }
        public ICollection<StautPoint> Points { get; set; }
    }
}
