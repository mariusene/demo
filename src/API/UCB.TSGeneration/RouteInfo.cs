using System.Net.Http;
using System.Reflection;

namespace UCB.TSGeneration
{
    public class RouteInfo
    {
        public string UrlTemplate { get; set; }
        public HttpMethod Verb { get; set; }
        public MethodInfo Method { get; set; }
        public RouteParamInfo[] Params { get; set; }
    }
}
