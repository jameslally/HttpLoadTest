using System.Collections.Generic;

namespace PFM.net.Model.JsonPatient
{
    public class JsonPatient : JsonBase
    {

        public List<KeyValuePair<string, string>> Patient { get; set; }
        public List<JsonControl> Controls { get; set; }
        public string HomerUserName { get; set; }
        public string HomerPassword { get; set; }
        public int CurrentEpisodeId { get; set; }
        public int NextEpisodeId { get; set; }
    }
}
