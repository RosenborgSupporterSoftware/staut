using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Teller.Core.Entities;

namespace Teller.Core.Infrastructure
{
    /// <summary>
    /// En klasse som leser XML-filer fra BillettService
    /// </summary>
    [Obsolete("Denne brukes ikke lenger.")]
    public class BillettServiceReader
    {
        /// <summary>
        /// Leser data fra en XML-fil fra BillettService og returnerer Tribune-info derfra
        /// </summary>
        /// <param name="filename">Fila man skal lese</param>
        /// <returns>En liste med det vi finner av Tribune-elementer</returns>
        public IList<Tribune> ReadData(string filename)
        {
            if(String.IsNullOrWhiteSpace(filename) || !File.Exists(filename))
                throw new ArgumentException("Ugyldig filangivelse");

            XDocument xml;

            try
            {
                xml = XDocument.Load(filename);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Ugyldig filangivelse - kan ikke tolke fil som XML");
            }
            if(xml.Root==null)
                throw new ArgumentException("Ugyldig filangivelse - finner ikke rot-element i XML");

            if (xml.Root.Elements().Count() == 1 && xml.Root.Elements().Single().Name == "ev_comp")
            {
                // Ond BillettService-kodet XML som må "pakkes opp" først
                var decoder = new TicketDecoder();
                xml = decoder.ExtractXml(xml);
                if(xml.Root == null)
                    throw new ArgumentException("Komprimert XML mangler rot-element");
            }

            // TODO: Bør kanskje gjøre dette på en litt vakrere og/eller sikrere måte.
            var sectionsElement = xml.Root.Element("Event_details").Element("E").Element("Section_id_list");

            return ReadSectionElements(sectionsElement);
        }

        public IList<Tribune> ReadSectionElements(XElement sections)
        {
            var tribuner = new List<Tribune>();

            foreach (var element in sections.Elements("E"))
            {
                ParseSingleElement(element, tribuner);
            }

            return tribuner;
        }

        public void ParseSingleElement(XElement element, IList<Tribune> tribuner)
        {
            if(element==null)
                return;

            var name = element.Element("Section_name").Value;
            var split = name.Split("-".ToCharArray());
            var tribname = split[0];
            var seksjon = split[1];
            var rowName = element.Element("Row_names").Element("E").Value;
            var avail = element.Element("Seat_characters").Value;

            AddAvailability(tribuner, tribname, seksjon, rowName, avail);
        }

        public void AddAvailability(IList<Tribune> tribuner, string tribuneNavn, string seksjonsNavn, string radNummer, string tilgjengelighet)
        {
            var tribune = tribuner.SingleOrDefault(t => t.Navn == tribuneNavn);
            if (tribune == null)
            {
                tribune = new Tribune(tribuneNavn);
                tribuner.Add(tribune);
            }

            var seksjon = tribune.Seksjoner.SingleOrDefault(s => s.SeksjonsNavn == seksjonsNavn);
            if (seksjon == null)
            {
                seksjon = new TribuneSeksjon(seksjonsNavn);
                tribune.Seksjoner.Add(seksjon);
            }

            var rad = new Rad(radNummer, tilgjengelighet);
            seksjon.Rader.Add(rad);
        }
    }
}
