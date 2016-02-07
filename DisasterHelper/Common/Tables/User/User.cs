using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tables.User
{
    public class User
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "channeluri")]
        public string ChannelUri { get; set; }

        [JsonProperty(PropertyName = "channeltype")]
        public ChannelType ChanneltType { get; set; }

        [JsonProperty(PropertyName = "fullname")]
        public string Fullname { get; set; }

        [JsonProperty(PropertyName = "address")]
        public string Address { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "usertype")]
        public UserType Type { get; set; }
    }
}
