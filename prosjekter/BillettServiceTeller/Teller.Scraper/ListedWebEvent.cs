using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

namespace Teller.Scraper
{
    /// <summary>
    /// En klasse som inneholder data om events vi henter fra ticketmaster.no e.l.
    /// </summary>
    [DebuggerDisplay("{EventId}: {Title} ({Start})")]
    public class ListedWebEvent
    {
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public string Opponent { get; set; }
        public int EventId { get; set; }
        public string TicketUrl { get; set; }
        public bool IsSaleStarted { get; set; }
        public DateTime SaleStart { get; set; }

        public static ListedWebEvent ParseListedWebEvent(string itemHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(itemHtml);
            return ParseListedWebEvent(doc.DocumentNode);
        }

        internal static ListedWebEvent ParseListedWebEvent(HtmlNode node)
        {
            DateTime saleStart = DateTime.MinValue;
            var saleStarted = true;

            var title = node.CssSelect("h4.eventname").Single().InnerText;
            var dateString = node.CssSelect("li.table__cell--date").Single().GetAttributeValue("content");
            DateTime.TryParse(dateString, out DateTime date);
            var url = node.CssSelect("a.button--buy").SingleOrDefault()?.GetAttributeValue("href") ?? String.Empty;
            if (string.IsNullOrWhiteSpace(url))
            {
                // Event som ikke er til salgs enda
                url = node.CssSelect("a.link--viewdates").SingleOrDefault()?.GetAttributeValue("href") ?? String.Empty;
                var startString = node.CssSelect("span.onsaletime").SingleOrDefault()?.InnerText;
                if (!string.IsNullOrWhiteSpace(startString))
                {
                    saleStart =
                        DateTime.Parse(
                            startString.Substring(startString.IndexOf(", ", StringComparison.InvariantCulture) + 1));
                    saleStarted = false;
                    // Må konstruere event-dato manuelt
                    dateString = string.Format("{0}. {1} {2} {3}",
                        node.CssSelect("span.calendaritem__day").Single().InnerText,
                        node.CssSelect("span.calendaritem__date__item.month").Single().InnerText,
                        node.CssSelect("span.calendaritem__date__item.year").Single().InnerText,
                        node.CssSelect("span.calendaritem__date--day > span.calendaritem__date__item.time")
                            .Single()
                            .InnerText);
                    date = DateTime.ParseExact(dateString, "dd. MMM yyyy HH:mm", CultureInfo.GetCultureInfo("NB-no"));
                }
            }
            int.TryParse(url.Substring(url.LastIndexOf("/", StringComparison.InvariantCulture) + 1), out int eventId);

            return new ListedWebEvent
            {
                Title = title,
                Start = date,
                Opponent = title.Substring(12),
                EventId = eventId,
                TicketUrl = url,
                IsSaleStarted = saleStarted,
                SaleStart = saleStart
            };
        }
    }
}
