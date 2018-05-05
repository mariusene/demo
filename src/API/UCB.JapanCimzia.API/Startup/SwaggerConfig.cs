using System.Web.Http;
using Swashbuckle.Application;


namespace UCB.JapanCimzia.API
{
    public static class SwaggerConfig
    {
        public static void AddSwagger(this HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "UCB.JapanCimzia.API");
            })
            .EnableSwaggerUi(c =>
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
