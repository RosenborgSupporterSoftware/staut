using Microsoft.Owin;
using Owin;
using StautApi.App_Start;

[assembly: OwinStartup(typeof(SignalRConfig))]

namespace StautApi.App_Start
{
    public class SignalRConfig
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }
    }
}
