using RabbitMQ.Client;
using System;

namespace MDL.ServiceBus
{
    /// <summary>
    /// Object that connects to a particular vhost of a RabbitMQ instance, ensures the queue it is to use exists and can then publish messages onto that queue (routing key) through the given exchange.
    /// </summary>
    public class Publish
    {
        private readonly string ExchangeName;
        private readonly string QueueName;
        private readonly ConnectionFactory Factory;
        [ThreadStatic]
        private static IConnection Connection;
        [ThreadStatic]
        private static IModel Channel;

        public Publish(string exchangeName, string queueName, string hostName, string vhost, string userName, string password, ushort port)
        {
            if (string.IsNullOrEmpty(queueName)) throw new Exception("Queue name must be provided");
            if (string.IsNullOrEmpty(hostName)) throw new Exception("Host name must be provided");
            if (string.IsNullOrEmpty(userName)) throw new Exception("Username must be provided");
            if (string.IsNullOrEmpty(password)) throw new Exception("Password must be provided");
            if (port <= 0 || port > ushort.MaxValue) throw new Exception("Invalid port number");

            ExchangeName = exchangeName;
            QueueName = queueName;
            if (Connection == null)
            {
                Factory = new ConnectionFactory { HostName = hostName, UserName = userName, Password = password, Port = port, VirtualHost = vhost };
                Connection = Factory.CreateConnection();
                Channel = Connection.CreateModel();
            }
            /* 
            * Name         the name of the queue
            * Durable      persisting the queue to disk (performance hit)
            * Exclusive    delete queue when not needed
            * Auto delete  queue deleted when consumer unsubscribes
            * Aurguments   ??
            */
            Channel.QueueDeclare(QueueName, false, false, true, null);
        }
        /// <summary>
        /// Send a message onto the configured queue(s) through an exchange
        /// NB. provide a blank string for the exchange if you wish to use the default for the given vhost
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        public void SendMessage<T>(object payload)
        {
            var payloadCast = (T)payload;
            IBasicProperties basicProperties = Channel.CreateBasicProperties();
            basicProperties.ContentType = "application/json";
            basicProperties.Type = typeof(T).ToString();
            Channel.BasicPublish(ExchangeName, QueueName, basicProperties, payloadCast.Serialize());
        }
    }
}
