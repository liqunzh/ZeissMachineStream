using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZeissMachineStream.Models
{
    public class StatusData
    {
        [JsonProperty(PropertyName = "topic")]
        public string Topic { get; set; }

        [JsonProperty(PropertyName = "ref")]
        public string Ref { get; set; }

        [JsonProperty(PropertyName = "payload")]
        public StatusDataDetail Payload { get; set; }

        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        public override string ToString()
        {
            return String.Format("[Topic:{0}, Ref:{1}, Payload: {2}, Event:{3}]", Topic, Ref, Payload, Event);
        }

    }
}
