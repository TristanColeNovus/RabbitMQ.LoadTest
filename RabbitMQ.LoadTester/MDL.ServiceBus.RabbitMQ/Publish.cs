using RabbitMQ.Client;
using System;

namespace MDL.ServiceBus
{
    /// <summary>
    /// Object that connects to a particular vhost of a RabbitMQ instance, ensures the queue it is to use exists and can then publish messages onto that queue (routing key) through the given exchange.
    /// </summary>
    public class Publish
    {
        /// <summary>
        /// Send a message onto the configured queue(s) through an exchange
        /// NB. provide a blank string for the exchange if you wish to use the default for the given vhost
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload"></param>
        public static void SendMessage<T>(IModel channel, string queueName, object payload)
        {
            if (channel == null) throw new ArgumentException("Channel must be provided");
            if (channel.IsClosed) throw new ArgumentException("Channel is closed, must be open");

            channel.QueueDeclare(queueName, false, false, true, null);

            var payloadCast = (T)payload;
            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.ContentType = "application/json";
            basicProperties.Type = typeof(T).ToString();
            channel.BasicPublish("", queueName, basicProperties, payloadCast.Serialize());
        }
    }
}
