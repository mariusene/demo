using System.Collections.Generic;

namespace UCB.JapanCimzia.TSGeneration
{
    public class TsServiceModel
    {
        public string ServiceName { get; set; }
        public string SourceTypeFullName { get; set; }
        public string TsGenProjectName { get; set; }
        public IEnumerable<TsServiceMethodModel> Methods { get; set; }
        public bool IsAuthServiceRequired { get; set; }
        public bool IsHttpServiceRequired { get; set; }
    }
}
