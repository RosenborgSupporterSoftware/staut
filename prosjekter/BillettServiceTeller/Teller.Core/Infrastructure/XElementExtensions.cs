using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Teller.Core.Infrastructure
{
    public static class XElementExtensions
    {
        public static string GetValueOrDefault(this XElement element, string name, string defaultValue)
        {
            if (element == null)
                return defaultValue;
            var subElement = element.Element(name);
            if (subElement == null)
                return defaultValue;

            return subElement.Value;
        }

        public static int GetValueOrDefault(this XElement element, string name, int defaultValue)
        {
            if (element == null)
                return defaultValue;
            var subElement = element.Element(name);
            if (subElement == null)
                return defaultValue;

            int result;
            if (int.TryParse(subElement.Value, out result))
                return result;

            return defaultValue;
        }

    }
}
