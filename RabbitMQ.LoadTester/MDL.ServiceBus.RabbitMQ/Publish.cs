using RabbitMQ.Client;
using System;

namespace MDL.ServiceBus
{
    /// <summary>
    /// Object that uses a current connection to a particular vhost of a RabbitMQ instance and then publish messages onto that queue (routing key) through the given exchange.
    /// <para>Will fail if queue is not setup or connection closed</para>
    /// </summary>
    /// <remarks>Used by Morphis.Transformer (NOVUS.Morphis.Common)</remarks>
    public class Publish
    {

        /// <summary>
        /// Send a message onto the configured queue(s) through an exchange
        /// NB. provide a blank string for the exchange if you wish to use the default for the given vhost
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel">The Open Channel</param>
        /// <param name="queueName">Full queue name <seealso cref="PathHelper.GetQueueName(string, string)"/></param>
        /// <param name="payload">Payload message object</param>
        public static void SendMessage<T>(IModel channel, string queueName, object payload)
        {
            // Check channel is open
            if (channel == null) throw new ArgumentException("Channel must be provided");
            if (channel.IsClosed) throw new ArgumentException("Channel is closed, must be open");

            // Sent message
            var payloadCast = (T)payload;
            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.ContentType = "application/json";
            basicProperties.Type = typeof(T).ToString();
            channel.BasicPublish("", queueName, basicProperties, payloadCast.Serialize());
        }
    }
}
