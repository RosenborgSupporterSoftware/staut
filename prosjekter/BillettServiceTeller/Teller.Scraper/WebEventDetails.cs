using System;

namespace Teller.Scraper
{
    /// <summary>
    /// En klasse som inneholder data om events vi henter fra ticketmaster.no e.l.
    /// </summary>
    public class WebEventDetails
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public string Opponent { get; set; }
        public int EventId { get; set; }
    }
}
