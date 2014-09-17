using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SRHS2backend.Startup))]
namespace SRHS2backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
