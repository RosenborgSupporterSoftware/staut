using System.Web;
using System.Web.Http;
using AutoMapper;

namespace StautApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(IocConfig.Bootstrap);
        }
    }
}
