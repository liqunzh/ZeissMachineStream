using Newtonsoft.Json;
using System;

namespace ZeissMachineStream.Models
{
    public class StatusDataDetail
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "machine_id")]
        public string MachineId { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        public override string ToString()
        {
            return String.Format("[Id:{0}, MachineId:{1}, Timestamp: {2}, Status:{3}]", Id, MachineId, Timestamp, Status);
        }

    }
}