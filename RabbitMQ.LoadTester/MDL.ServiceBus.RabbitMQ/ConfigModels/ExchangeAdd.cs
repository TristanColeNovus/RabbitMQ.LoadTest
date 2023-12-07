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

        /// <summary>
        /// If yes, the exchange will delete itself after at least one queue or exchange has been bound to this one, and then all queues or exchanges have been unbound
        /// </summary>
        [JsonProperty("auto_delete")]
        public bool AutoDelete { get; set; } = false;

        [JsonProperty("durable")]
        public bool Durable { get; set; } = true;

        /// <summary>
        /// If yes, clients cannot publish to this exchange directly. It can only be used with exchange to exchange bindings
        /// </summary>
        [JsonProperty("internal")]
        public bool Internal { get; set; } = false;

        [JsonProperty("arguments")]
        public string[] Arguments { get; set; } = new string[0];
    }
}
