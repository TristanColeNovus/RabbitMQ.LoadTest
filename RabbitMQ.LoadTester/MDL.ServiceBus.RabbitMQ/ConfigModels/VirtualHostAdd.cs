using Newtonsoft.Json;
using System;

namespace MDL.ServiceBus.ConfigModels
{
    /// <summary>
    /// Config Information for adding a new RabbitMQ Virtual Host
    /// </summary>
    public class VirtualHostAdd : ConfigModelBase
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }
    }
}
