using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Colonizator.Startup))]
namespace Colonizator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            ConfigurationSignalr(app);
        }
    }
}
