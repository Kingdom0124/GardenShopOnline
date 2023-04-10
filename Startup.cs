using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GardenShopOnline.Startup))]
namespace GardenShopOnline
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
