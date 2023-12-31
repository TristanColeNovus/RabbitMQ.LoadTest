﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;

namespace MDL.ServiceBus
{
    /// <summary>
    /// Object that connects to a particular vhost of a RabbitMQ instance and will then call the provided subscription delegate when ever a message is pulled off of the queue.
    /// <para>Will fail if user, queue or exchange is not setup</para>
    /// </summary>
    /// <remarks>Used by Novus (Dashboard)</remarks>
    public class Subscribe : IDisposable
    {
        private ConnectionFactory Factory;
        private IConnection Connection;
        private IModel Channel;
        private Action<BasicDeliverEventArgs> SubscriptionDelegate;


        /// <summary>
        /// Connects to a particular vhost of a RabbitMQ instance and will then call the provided subscription delegate when ever a message is pulled off of the queue.
        /// Will create the Connection if not setup
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, AMQPPort, VirtualHost, ExchangeName, QueueName, Username)</param>
        /// <param name="password">Standard Queue Password</param>
        /// <param name="subscriptionDelegate">Delegate Event</param>
        public Subscribe(RabbitMQConfiguration cfg, string password, Action<BasicDeliverEventArgs> subscriptionDelegate)
        {
            // Check Inputs
            if (cfg == null) throw new Exception("Rabbit MQ Config invalid");
            if (string.IsNullOrEmpty(cfg.HostURL)) throw new Exception("Host name must be provided");
            if (cfg.AMQPPort <= 0 || cfg.AMQPPort > ushort.MaxValue) throw new Exception("Invalid port number");
            if (string.IsNullOrEmpty(cfg.VirtualHost)) throw new Exception("VirtualHost must be provided");
            if (string.IsNullOrEmpty(cfg.QueueName)) throw new Exception("Queue name must be provided");
            if (string.IsNullOrEmpty(cfg.Username)) throw new Exception("Username must be provided");
            if (string.IsNullOrEmpty(password)) throw new Exception("Password must be provided");

            // Create Connection if needed
            if (Connection == null)
            {
                Factory = new ConnectionFactory { HostName = cfg.HostURL, UserName = cfg.RabbitMQUsername, Password = password, Port = cfg.AMQPPort, VirtualHost = cfg.VirtualHost };
                Connection = Factory.CreateConnection();
                Channel = Connection.CreateModel();
            }
            if (Channel == null) Channel = Connection.CreateModel();

            // Bind to Queue and Listen
            Channel.QueueBind(cfg.QueueName, !string.IsNullOrEmpty(cfg.ExchangeName) ? cfg.ExchangeName : cfg.QueueName, string.Empty);

            SubscriptionDelegate = subscriptionDelegate;

            var consumer = new EventingBasicConsumer(Channel);
            consumer.Received += Receive;
            Channel.BasicConsume(queue: cfg.QueueName,
                                 autoAck: true,
                                 consumer: consumer);
        }



        /// <summary>
        /// Event is triggered by a message being placed onto the configured queue.
        /// The exchange is presumed to be setup as a direct queue.
        /// If there are multiple subscribers messages will be consumed by then in turn, not all messages will be received by both subscriber processes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        public void Receive(object sender, BasicDeliverEventArgs ea)
        {
            SubscriptionDelegate(ea);
        }



        /// <summary>
        /// Checks the RabbitMQ Connection state & channel state
        /// </summary>
        /// <returns>True if open and usable, else false</returns>
        public bool ConnectionState()
        {
            return Connection != null && Channel != null && Channel.IsOpen;
        }

        
        
        /// <summary>
        /// Close all connections, and dispose of the objects
        /// </summary>
        public void Dispose()
        {
            // Close Connection if open
            if (Connection != null && Connection.IsOpen)
            {
                Connection.Close();
            }
            
            // Unbind Action target
            SubscriptionDelegate = null;

            // Dispose of the object
            Connection.Dispose();
        }
    }
}
