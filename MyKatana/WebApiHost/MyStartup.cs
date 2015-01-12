using System;
using System.Threading.Tasks;
using System.Web.Http; 
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApiHost.MyStartup))]

namespace WebApiHost
{
    public class MyStartup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);
        } 
    }
}
