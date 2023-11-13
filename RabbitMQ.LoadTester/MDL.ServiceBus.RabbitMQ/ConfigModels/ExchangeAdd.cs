using Newtonsoft.Json;
using System;

namespace MDL.ServiceBus.ConfigModels
{
    /// <summary>
    /// Config Information for adding a new RabbitMQ Exchange
    /// </summary>
    /// <remarks>Has the default settings</remarks>
    public class ExchangeAdd : ConfigModelBase
    {
        [JsonProperty("vhost")]
        public string VHost { get; set; }

        [JsonProperty("name")]
        public string ExchangeName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "direct";

        [JsonProperty("auto_delete")]
        public bool AutoDelete { get; set; } = false;

        [JsonProperty("durable")]
        public bool Durable { get; set; } = false;

        [JsonProperty("internal")]
        public bool Internal { get; set; } = false;

        [JsonProperty("arguments")]
        public string[] Arguments { get; set; } = new string[0];
    }
}
