using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FreshenerShopSDM.Startup))]
namespace FreshenerShopSDM
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
