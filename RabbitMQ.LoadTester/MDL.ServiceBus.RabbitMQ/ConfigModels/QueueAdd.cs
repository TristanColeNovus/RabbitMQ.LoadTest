﻿using Newtonsoft.Json;
using System;

namespace MDL.ServiceBus.ConfigModels
{
    /// <summary>
    /// Config Information for adding a new RabbitMQ Queue
    /// </summary>
    /// <remarks>Has the default settings</remarks>
    public class QueueAdd : ConfigModelBase
    {
        [JsonProperty("vhost")]
        public string VHost { get; set; }

        [JsonProperty("name")]
        public string QueueName { get; set; }

        [JsonProperty("type")]
        public string QueueType { get; set; } = "classic";

        [JsonProperty("durable")]
        public bool Durable { get; set; } = false;

        [JsonProperty("auto_delete")]
        public bool AutoDelete { get; set; } = false;

        [JsonProperty("arguments")]
        public ArgumentOption Arguments { get; set; } = new ArgumentOption() { 
            QueueType = "classic",
            Expires = 600000,
            MessageTTL = 300000
        };
    }
}
