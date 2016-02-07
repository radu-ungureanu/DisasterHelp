using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tables.Disaster
{
    public class Missing
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "userid")]
        public string UserPostingId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "image")]
        public byte[] Image { get; set; }

        [JsonProperty(PropertyName = "found")]
        public bool Found { get; set; }

        [JsonProperty(PropertyName = "feedback")]
        public string Feedback { get; set; }

        [JsonProperty(PropertyName = "disasterId")]
        public string DisasterId { get; set; }
    }
}
