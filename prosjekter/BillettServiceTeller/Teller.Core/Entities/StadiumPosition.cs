using System;
using Teller.Core.Extensions;

namespace Teller.Core.Entities
{
    public class StadiumPosition
    {
        public string SectionName { get; set; }
        public string RowName { get; set; }
        public string SeatName { get; set; }

        public string Position { get { return String.Format("{0}:{1}:{2}", SectionName, RowName, SeatName); } }

        public StadiumPosition(string sectionName, string rowName, string seatName)
        {
            SectionName = sectionName;
            RowName = rowName;
            SeatName = seatName;
        }

        /// <summary>
        /// Sjekker om denne instansen matcher et gitt pattern
        /// </summary>
        /// <param name="pattern">Pattern å matche mot. * er eneste wildcard.</param>
        /// <returns>true om det er match, false ellers</returns>
        public bool IsMatch(string pattern)
        {
            var split = pattern.Split(":".ToCharArray());
            if (split.Length > 0) // Vi har seksjon.
            {
                if (split[0] != "*" && split[0] != "")
                {
                    if (!SectionName.Contains(split[0], StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }
            if (split.Length > 1) // Vi har rad.
            {
                if (split[1] != "*" && split[1] != "")
                {
                    if (RowName != split[1])
                        return false;
                }
            }
            if (split.Length > 2) // Vi har sete.
            {
                if (split[2] != "*" && split[2] != "")
                {
                    if (SeatName != split[2])
                        return false;
                }
                
            }
            return true;
        }

        public override string ToString()
        {
            return Position;
        }
    }
}
