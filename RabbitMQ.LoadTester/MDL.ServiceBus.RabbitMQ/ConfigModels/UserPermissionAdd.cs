using Newtonsoft.Json;
using System;

namespace MDL.ServiceBus.ConfigModels
{
    /// <summary>
    /// Config Information for adding a new RabbitMQ User Account VHost Permission
    /// </summary>
    public class UserPermissionAdd : ConfigModelBase
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("vhost")]
        public string VHost { get; set; }

        /// <summary>
        /// Reg Ex
        /// </summary>
        [JsonProperty("configure")]
        public string Configure { get; set; } = ".*";

        /// <summary>
        /// Reg Ex
        /// </summary>
        [JsonProperty("write")]
        public string Write { get; set; } = ".*";

        /// <summary>
        /// Reg Ex
        /// </summary>
        [JsonProperty("read")]
        public string Read { get; set; } = ".*";

    }
}
