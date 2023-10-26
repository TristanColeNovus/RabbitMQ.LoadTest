using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace RabbitMQ.LoadTester.BLL.Shared
{
    /// <summary>
    /// Object that connects to a particular vhost of a RabbitMQ instance, ensures the queue it is to use exists (it presumes the exchange already exists) and will then call the provided subscription delegate when ever a message is pulled off of the queue.
    /// </summary>
    public class Subscribe
    {
        private readonly string QueueName;
        private ConnectionFactory Factory;
        private static IConnection Connection;
        private static IModel Channel;
        private Action<BasicDeliverEventArgs> SubscriptionDelegate;

        public Subscribe(string exchangeName, string queueName, string hostName, string vhost, string userName, string password, ushort port, Action<BasicDeliverEventArgs> subscriptionDelegate)
        {
            if (string.IsNullOrEmpty(queueName)) throw new Exception("Queue name must be provided");
            if (string.IsNullOrEmpty(hostName)) throw new Exception("Host name must be provided");
            if (string.IsNullOrEmpty(userName)) throw new Exception("Username must be provided");
            if (string.IsNullOrEmpty(password)) throw new Exception("Password must be provided");
            if (port <= 0 || port > ushort.MaxValue) throw new Exception("Invalid port number");

            QueueName = queueName;
            SubscriptionDelegate = subscriptionDelegate;
            if (Connection == null)
            {
                Factory = new ConnectionFactory { HostName = hostName, UserName = userName, Password = password, Port = port, VirtualHost = vhost };
                Connection = Factory.CreateConnection();
                Channel = Connection.CreateModel();
            }
            if (Channel == null) Channel = Connection.CreateModel();
            /* 
             * Name         the name of the queue
             * Durable      persisting the queue to disk (performance hit)
             * Exclusive    delete queue when not needed
             * Auto delete  queue deleted when consumer unsubscribes
             * Aurguments   ??
             */
            //Channel.QueueDeclare(QueueName, true, false, false, null);
            Channel.QueueDeclare(QueueName, false, false, true, null);
            //Channel.ExchangeDeclare(exchangeName, "direct", false, false);
            Channel.QueueBind(queueName, !string.IsNullOrEmpty(exchangeName) ? exchangeName : queueName, string.Empty);
            //Channel.QueueBind(queueName, queueName, string.Empty);
            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += Receive;
            Channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }



        /// <summary>
        /// Event is triggered by a message being placed onto the configured queue.
        /// The exchange is presumed to be setup as a direct queue.
        /// If there are multiple subscribers messages will be consumed by then in turn, not all messages will be received by both subscriber processes.
        /// This method is currently an example of how to parse a message and simply returns the un-parsed json.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        public void Receive(object sender, BasicDeliverEventArgs ea)
        {
            //var body = ea.Body.ToArray();
            //var message = (object)Encoding.UTF8.GetString(body);
            //var contentType = ea.BasicProperties.ContentType;
            //var objectType = ea.BasicProperties.Type;
            //var result = body.DeSerialize(Type.GetType(objectType));
            SubscriptionDelegate(ea);
        }
    }
}
