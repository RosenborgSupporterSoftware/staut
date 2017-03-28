using System;
using System.Collections.Generic;
using System.Linq;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace Teller.Scraper
{
    public class EventScraper
    {
        public IEnumerable<ListedWebEvent> Scrape()
        {
            var browser = new ScrapingBrowser();

            var resultsPage = browser.NavigateToPage(new Uri("http://www.ticketmaster.no/venue/lerkendal-stadion-trondheim-billetter/tld/25"));

            var items = resultsPage.Html.CssSelect("div#event-lists > div#national-events > div > ul.table__row--event");

            foreach (var htmlNode in items)
            {
                yield return ListedWebEvent.ParseListedWebEvent(htmlNode);
            }
        }

    }
}
