using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tables.Disaster
{
    public class DisasterAttend
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "disasterid")]
        public string DisasterId { get; set; }

        [JsonProperty(PropertyName = "participantid")]
        public string ParticipantId { get; set; }
    }
}
