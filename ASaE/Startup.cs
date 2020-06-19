using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASaE.Startup))]
namespace ASaE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
