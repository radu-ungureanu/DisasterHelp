using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tables.Disaster
{
    public class Disaster
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "plus")]
        public int RatingPlus { get; set; }

        [JsonProperty(PropertyName = "minus")]
        public int RatingMinus { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "initiatorid")]
        public string InitiatorId { get; set; }

        [JsonProperty(PropertyName = "initiatorname")]
        public string InitiatorName { get; set; }

        [JsonProperty(PropertyName = "active")]
        public DisasterStatusType Active { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public double Latitude { get; set; }
    }
}
