using System.Web;
using System.Web.Http;
using AutoMapper;

namespace StautApi
{
    public class WebApiApplication : HttpApplication
    {
        private MapperConfiguration _config;

        public MapperConfiguration MapperConfiguration => _config;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(IocConfig.Bootstrap);
            _config = MapperConfig.Configure();
        }
    }
}
