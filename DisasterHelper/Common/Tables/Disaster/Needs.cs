using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tables.Disaster
{
    public class Needs
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "centerid")]
        public string CenterId { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}
