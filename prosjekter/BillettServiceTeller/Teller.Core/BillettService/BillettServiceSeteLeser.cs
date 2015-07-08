using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Teller.Core.Extensions;

namespace Teller.Core.BillettService
{
    public class BillettServiceSeteLeser
    {
        public IEnumerable<BillettServiceSete> ReadSeats(BillettServiceXmlFile file)
        {
            return ReadSeats(file == null ? null : file.XDocument);
        }

        public IEnumerable<BillettServiceSete> ReadSeats(XDocument xdoc)
        {
            if(xdoc==null)
                yield break;

            var sectionsElement = FindSectionsElement(xdoc);
            if(sectionsElement == null)
                yield break;

            foreach (var xElement in sectionsElement.Elements("E"))
            {
                var sectionId = xElement.GetValueOrDefault("Section_id", String.Empty);
                var sectionName = xElement.GetValueOrDefault("Section_name", String.Empty);
                var sectionTag = xElement.GetValueOrDefault("Section_tag", String.Empty);

                var rowName = GetRowName(xElement);
                var expectedSeatCount = xElement.GetValueOrDefault("Seat_count", 0);

                var seatNames = ExtractSeatNames(xElement);
                if(seatNames.Count != expectedSeatCount)
                    Console.WriteLine("Rotten element? {0}/{1}/{2}", sectionId, sectionName, sectionTag);

                var esdEntries = ExtractEsds(xElement);
                if(esdEntries.Select(e => e.Item2).Sum() != expectedSeatCount)
                    Console.WriteLine("Rotten element? {0}/{1}/{2}", sectionId, sectionName, sectionTag);

                var currentEttEntry = 0;
                var currentEttCode = esdEntries[currentEttEntry].Item1;
                var countLeft = esdEntries[currentEttEntry].Item2;

                for (var i = 0; i < expectedSeatCount; i++)
                {
                    var sete = new BillettServiceSete
                    {
                        SectionId = sectionId,
                        SectionName = sectionName,
                        SectionTag = sectionTag,
                        RowName = rowName,
                        SeatName = seatNames[i],
                        EttCode = currentEttCode
                    };
                    yield return sete;

                    if (--countLeft == 0 && i < expectedSeatCount-1)
                    {
                        currentEttCode = esdEntries[++currentEttEntry].Item1;
                        countLeft = esdEntries[currentEttEntry].Item2;
                    }
                }
            }
        }

        private IList<Tuple<string,int>> ExtractEsds(XElement xElement)
        {
            var esdElement = xElement.Element("Esd");
            if (esdElement == null)
                return new Tuple<string, int>[0];

            var elements = esdElement.Elements("E").ToList();
            var result = new List<Tuple<string, int>>(elements.Count);

            foreach (var element in elements)
            {
                var vals = element.Value.Split(",".ToCharArray());
                if (vals.Length != 2)
                    continue;
                int count;
                if(!int.TryParse(vals[1], out count))
                    continue;
                result.Add(new Tuple<string, int>(vals[0], count));
            }

            return result;
        }

        private IList<string> ExtractSeatNames(XElement xElement)
        {
            var seatNamesElements = xElement.Element("Seat_names");
            if (seatNamesElements == null)
                return new List<string>();

            var elements = seatNamesElements.Elements("E")
                                            .ToList();

            return elements.Select(e => e.Value).ToList();
        }

        private string GetRowName(XElement xElement, string defaultValue = "")
        {
            if (xElement == null)
                return defaultValue;
            var rowsElement = xElement.Element("Row_names");
            if (rowsElement == null)
                return defaultValue;
            var firstName = rowsElement.Element("E");
            if (firstName == null || String.IsNullOrWhiteSpace(firstName.Value))
                return defaultValue;

            return firstName.Value;
        }

        private XElement FindSectionsElement(XDocument xdoc)
        {
            var root = xdoc.Root;
            if (root == null)
                return null;

            var eventDetails = root.Element("Event_details");
            if (eventDetails == null)
                return null;

            var e = eventDetails.Element("E");
            if (e == null)
                return null;

            return e.Element("Section_id_list");
        }

        public IEnumerable<SeatSummary> ReadSeatSummary(BillettServiceXmlFile file)
        {
            var summary =
                GetSummaryElements(file.XDocument)
                    .Select(x => new SeatSummary(x.Element("Ett").Value, x.Element("C").Value))
                    .ToList();

            return summary;
        }

        private IEnumerable<XElement> GetSummaryElements(XDocument doc)
        {
            if(doc == null || doc.Root==null)
                yield break;
            var tlElement = doc.Root.Element("Grand_total_summary");
            if(tlElement == null)
                yield break;
            tlElement = tlElement.Element("TL");
            if(tlElement==null)
                yield break;

            foreach (var xElement in tlElement.Elements("E"))
            {
                yield return xElement;
            }
        }
    }
}
