using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(UCB.JapanCimzia.API.Startup))]
namespace UCB.JapanCimzia.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.EnableCors();
            config.MapHttpAttributeRoutes(new InheritanceRouteProvider());
            config.UseJsonResponseAsDefault();
            config.UseJsonCamelCase();
            config.AddSwagger();
            app.UseWebApi(config);
        }
    }
}