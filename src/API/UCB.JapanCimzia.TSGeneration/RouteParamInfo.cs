using System.Reflection;

namespace UCB.JapanCimzia.TSGeneration
{
    public class RouteParamInfo
    {
        public string Name { get; set; }
        public bool IsUrlParam { get; set; }
        public bool IsQueryParam { get; set; }
        public bool IsDataParam { get; set; }
        public ParameterInfo ParameterInfo { get; set; }
    }
}