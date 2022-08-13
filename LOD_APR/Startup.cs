using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LOD_APR.Startup))]
namespace LOD_APR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
