using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace StautApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Remove(config.Formatters.XmlFormatter); // runej: Det holder vel med JSON-støtte? Eller kan vi trenge XML for... noe?
            config.EnsureInitialized();
        }
    }
}
