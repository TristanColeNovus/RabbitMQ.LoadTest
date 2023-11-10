﻿using System;

namespace MDL.ServiceBus
{
    /// <summary>
    /// RabbitMQ Configuration
    /// </summary>
    /// <remarks>Used in both Novus and Morphis</remarks>
    public class RabbitMQConfiguration
    {
        /// <summary>
        /// Service Account Username
        /// </summary>
        public string ServiceAccountUsername { get; set; } = "morphis-service";

        /// <summary>
        /// Service Account Password
        /// </summary>
        public string ServiceAccountPassword { get; set; }

        /// <summary>
        /// Rabbit MQ Host Name or IP address
        /// </summary>
        public string HostURL { get; set; } = "127.0.0.1";

        /// <summary>
        /// Rabbit MQ Queue Port Number
        /// </summary>
        public int AMQPPort { get; set; } = 5672;

        /// <summary>
        /// Rabbit MQ WebAPI Management Port
        /// </summary>
        public int ManagementApiPort { get; set; } = 15672;

        /// <summary>
        /// Virtual Host (dataset name)
        /// </summary>
        /// <remarks>In lowercase</remarks>
        public string VirtualHost { get; set; }

        /// <summary>
        /// Novus Username
        /// </summary>
        /// <remarks>In lowercase</remarks>
        public string Username { get; set; }

        /// <summary>
        /// VHost Exchange Name (full path)
        /// </summary>
        /// <remarks>In lowercase</remarks>
        public string ExchangeName { get; set; }

        public bool Development { get; set; }

        public bool DevMessages { get; set; }


        public override string ToString()
        {
            return $"{HostURL}:{AMQPPort} / {VirtualHost} / {ExchangeName} ({Username})";
        }
    }
}
