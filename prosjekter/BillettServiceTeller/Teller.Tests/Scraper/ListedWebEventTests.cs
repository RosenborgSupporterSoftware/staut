using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Teller.Scraper;
using Xunit;

namespace Teller.Tests.Scraper
{
    public class ListedWebEventTests
    {
        [Fact]
        public void Constructor_GivenTicketMasterHtml_ProducesExpectedEvent()
        {
            // Arrange
            const string html = // Actual TicketMaster HTML
                "\r\n\t<li class=\"table__cell--date\" itemprop=\"startDate\" content=\"2017-04-02T20:00\">\r\n\t\t<div class=\"table__cell__body--date\">\r\n\t\t\t<div class=\"calendaritem\">\r\n\t\t\t\t<span class=\"calendaritem__day\">02</span>\r\n\t\t\t\t<span class=\"calendaritem__date--month\">\r\n\t\t\t\t\t<span class=\"calendaritem__date__item month\">apr</span>\r\n\t\t\t\t\t<span class=\"calendaritem__date__item year\">2017</span>\r\n\t\t\t\t</span>\r\n\t\t\t\t<span class=\"calendaritem__date--day\">\r\n\t\t\t\t\t<span class=\"calendaritem__date__item day\">søn</span>\r\n\t\t\t\t\t<span class=\"calendaritem__date__item time\">20:00</span>\r\n\t\t\t\t</span>\r\n\t\t\t</div>\r\n\t\t</div>\r\n\t</li>\r\n\r\n\t<li class=\"table__cell--eventname\">\r\n\t\t<div class=\"table__cell__body--eventname\">\r\n\t\t\t\t\t\t<h4 class=\"table__heading eventname\" itemprop=\"name\">Rosenborg - Odd</h4>\r\n\t\t\t<span class=\"calendaritem eventtime--tabletportrait\">\r\n\t\t\t\t<span class=\"calendaritem__date__item day\">søn</span>\r\n\t\t\t\t<span class=\"calendaritem__date__item time\">20:00</span>\r\n\t\t\t</span>\r\n\t\t</div>\r\n\t</li>\r\n\r\n\t\t\r\n\t<li class=\"table__cell--availability\" itemscope=\"itemscope\" itemprop=\"offers\" itemtype=\"http://schema.org/Offer\">\r\n\t\t<div class=\"table__cell__body--availability\">\r\n\t\t\t\t\t\t\t\t\t\t<a class=\"button--buy roundedButton\" itemprop=\"url\" href=\"/event/rosenborg-odd-billetter/515827\">Kjøp billetter</a>\r\n\t\t\t\t\t\t\t\t<a class=\"icon-chevron button--buy--smallscreen\" href=\"/event/rosenborg-odd-billetter/515827\"></a>\r\n\t\t\t\t\t</div>\r\n\t</li>\r\n";
            
            // Act
            var res = ListedWebEvent.ParseListedWebEvent(html);

            // Assert
            res.Title.Should().Be("Rosenborg - Odd");

            res.Start.Year.Should().Be(2017);
            res.Start.Month.Should().Be(4);
            res.Start.Day.Should().Be(2);
            res.Start.Hour.Should().Be(20);
            res.Start.Minute.Should().Be(0);

            res.EventId.Should().Be(515827);
            res.TicketUrl.Should().Be("/event/rosenborg-odd-billetter/515827");

            res.Opponent.Should().Be("Odd");
            res.IsSaleStarted.Should().BeTrue();
        }
    }
}
