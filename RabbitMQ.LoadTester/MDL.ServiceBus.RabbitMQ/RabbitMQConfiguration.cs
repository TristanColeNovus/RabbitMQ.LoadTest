using System;
using System.Threading;

namespace MDL.ServiceBus
{
    /// <summary>
    /// RabbitMQ Configuration
    /// </summary>
    /// <remarks>Used in both Novus and Morphis</remarks>
    public class RabbitMQConfiguration : ICloneable
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
        /// Dataset Name in lowercase
        /// </summary>
        private string _dataSetName;

        /// <summary>
        /// Novus DataSet Name
        /// </summary>
        public string DataSetName
        {
            get { return _dataSetName; }
            set { _dataSetName = value.ToLower(); }
        }

        /// <summary>
        /// Username in lowercase
        /// </summary>
        private string _username;

        /// <summary>
        /// Novus Username
        /// </summary>
        public string Username
        {
            get { return _username; }
            set { _username = value.ToLower(); }
        }

        #region "Derived values"

        /// <summary>
        /// Virtual Host (dataset name)
        /// </summary>
        /// <remarks>In lowercase</remarks>
        public string VirtualHost
        {
            get { return DataSetName; }
        }

        /// <summary>
        /// Novus Username (full path)
        /// </summary>
        /// <remarks>In lowercase</remarks>
        public string RabbitMQUsername
        {
            get { return PathHelper.GetUserName(DataSetName, Username); }
        }

        /// <summary>
        /// VHost Exchange Name (full path)
        /// </summary>
        /// <remarks>In lowercase</remarks>
        public string ExchangeName
        {
            get { return PathHelper.GetExchangeName(DataSetName); }
        }

        /// <summary>
        /// VHost Queue Name (full path)
        /// </summary>
        /// <remarks>In lowercase</remarks>
        public string QueueName
        {
            get { return PathHelper.GetQueueName(DataSetName, Username); }
        }

        #endregion

        public bool Development { get; set; }

        public bool DevMessages { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{HostURL}:{AMQPPort} / {VirtualHost} ({Username})";
        }

    }
}
