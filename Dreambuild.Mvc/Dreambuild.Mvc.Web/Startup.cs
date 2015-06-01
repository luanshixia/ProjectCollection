using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dreambuild.Mvc.Web.Startup))]
namespace Dreambuild.Mvc.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
