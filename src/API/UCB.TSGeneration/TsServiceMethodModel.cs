using System.Globalization;
using System.Net.Http;

namespace UCB.TSGeneration
{
    public class TsServiceMethodModel
    {
        public string MethodName { get; set; }
        public string InputParameterList { get; set; }
        public string ResponseType { get; set; }
        public string Uri { get; set; }
        public string Verb { get; set; }
        public object DataParameterName { get; set; }
        public bool IsUrlOnly { get; set; }
        public bool IsNotGet { get { return Verb != HttpMethod.Get.Method.ToLower(CultureInfo.InvariantCulture); } }
        public bool ReturnIsNotEmpty { get { return !string.IsNullOrEmpty(ResponseType); } }
    }
}
