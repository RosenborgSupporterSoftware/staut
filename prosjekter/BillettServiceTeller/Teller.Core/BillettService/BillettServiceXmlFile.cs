using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Teller.Core.Infrastructure;

namespace Teller.Core.BillettService
{
    public class BillettServiceXmlFile
    {
        #region Fields 

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the XDocument representation of the data file
        /// </summary>
        public XDocument XDocument { get; private set; }

        #endregion

        #region Constructor

        private BillettServiceXmlFile(XDocument xml)
        {
            XDocument = xml;
        }

        #endregion

        #region Static stuff

        /// <exception cref="ArgumentException">Ugyldig filangivelse</exception>
        public static BillettServiceXmlFile LoadFile(string filePath)
        {
            if (String.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                throw new ArgumentException("Ugyldig filangivelse");

            try
            {
                var xdoc = LoadXDocument(filePath);
                return new BillettServiceXmlFile(xdoc);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static XDocument LoadXDocument(string filePath)
        {
            XDocument xml;
            
            try
            {
                xml = XDocument.Load(filePath);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Ugyldig filangivelse - kan ikke tolke fil som XML", e);
            }
            if (xml.Root == null)
                throw new ArgumentException("Ugyldig filangivelse - finner ikke rot-element i XML");

            if (xml.Root.Elements().Count() == 1 && xml.Root.Elements().Single().Name == "ev_comp")
            {
                // Ond BillettService-kodet XML som må "pakkes opp" først
                var decoder = new TicketDecoder();
                xml = decoder.ExtractXml(xml);
                if (xml.Root == null)
                    throw new ArgumentException("Komprimert XML mangler rot-element");
            }

            return xml;
        }

        #endregion

    }
}
