using Newtonsoft.Json;
using System;

namespace MDL.ServiceBus.ConfigModels
{
    /// <summary>
    /// Config Information for adding a new RabbitMQ User Account
    /// </summary>
    public class UserAccountAdd : ConfigModelBase
    {
        [JsonProperty("password_hash")]
        public string PasswordHash { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// User account tags (administrator or other)
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }
    }
}
