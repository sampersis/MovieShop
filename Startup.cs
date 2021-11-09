using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MovieShop.Startup))]
namespace MovieShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
