using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace UCB.JapanCimzia.API
{
    public static class JsonResponseConfig
    {
        public static void UseJsonResponseAsDefault(this HttpConfiguration config)
        {
            config.Formatters.Add(new JsonResponseFormatter());
        }

        public static void UseJsonCamelCase(this HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.JsonFormatter;
            var setting = jsonFormatter.SerializerSettings;
            setting.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}