using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tables.Disaster
{
    public class Pacient
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "status")]
        public PacientStatusType Status { get; set; }

        [JsonProperty(PropertyName = "blood")]
        public BloodType BloodType { get; set; }

        [JsonProperty(PropertyName = "image")]
        public byte[] Image { get; set; }

        [JsonProperty(PropertyName = "centerid")]
        public string CenterId { get; set; }
    }
}
