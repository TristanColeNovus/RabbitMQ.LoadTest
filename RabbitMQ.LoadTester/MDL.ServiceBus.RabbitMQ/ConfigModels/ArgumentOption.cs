using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MDL.ServiceBus.ConfigModels
{
    public class ArgumentOption
    {
        /// <summary>
        /// How long a queue can be unused for before it is automatically deleted (milliseconds)
        /// </summary>
        [JsonProperty("x-expires", NullValueHandling = NullValueHandling.Ignore)]
        public int? Expires { get; set; }

        /// <summary>
        /// How long a message published to a queue can live before it is discarded (milliseconds)
        /// </summary>
        [JsonProperty("x-message-ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? MessageTTL { get; set; }


        [JsonProperty("x-queue-type", NullValueHandling = NullValueHandling.Ignore)]
        public string QueueType { get; set; }

    }
}
