using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Avaliador_Textual.Startup))]
namespace Avaliador_Textual
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
