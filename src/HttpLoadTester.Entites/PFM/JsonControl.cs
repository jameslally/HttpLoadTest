using System.Collections.Generic;

namespace PFM.net.Model.JsonPatient
{
    public class JsonControl : JsonBase
    {
        public int ControlId { get; set; }
        public string DataType { get; set; }
        public string ControlType { get; set; }       
        public string DefaultValue { get; set; }
        public bool IsIntegrated { get; set; }
        public string SourceSystemCode { get; set; }
        public bool IsValueChanged { get; set; }

        public int UserInputId { get; set; }
        public int EpisodeId { get; set; }
        public string InputData { get; set; }
        public string BehaviourTypeCode { get; set; }

        public List<int> Children { get; set; }
        public string Behaviour { get; set; }
    }
}
