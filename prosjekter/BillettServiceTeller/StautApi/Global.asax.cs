using System.Web;
using System.Web.Http;

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
