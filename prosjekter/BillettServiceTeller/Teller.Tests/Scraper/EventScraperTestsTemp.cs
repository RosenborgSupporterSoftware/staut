using System.Linq;
using Teller.Scraper;
using Xunit;

namespace Teller.Tests.Scraper
{
    public class EventScraperTestsTemp
    {
        [Fact]
        public void TestScraping()
        {
            var sut = new EventScraper();

            var res = sut.Scrape().ToList();
        }
    }
}
