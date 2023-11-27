using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundations.Extensions.LocalMachineCommands.Broker;
//using Foundations.Extensions.Runtime.Web;

using MDL.ServiceBus;

//using NOVUS.Morphis.Common.Configuration;
//using NOVUS.Morphis.Common.Providers;
using RabbitMQ.Client;
//using static NOVUS.Morphis.Common.Configuration.NovusConfigurationSettings;

namespace RabbitMQ.LoadTester.NEW.Morphis
{
    public class NovusMsgBrokerPublisher : IMsgBrokerPublisher
    {
        RabbitMQConfiguration config;
        //private readonly ICurrentUsernameResolver currentUserNameResolver;
        private DataSetConfiguration dataset;

        public NovusMsgBrokerPublisher(RabbitMQConfiguration config)//, ICurrentUsernameResolver currentUserNameResolver)
        {
            this.config = config;
            //this.currentUserNameResolver = currentUserNameResolver;
        }

        private bool GetCurrentDataset()
        {
            //dataset = NovusCurrentUserProvider.GetDataSetConfiguration();

            //if (WebExecutionContext.Current?.Cache == null) return false;
            //else
            //{
            //    dataset = string.IsNullOrEmpty(Globals.ConnectionStringHost)
            //    ? config.Dataset.FirstOrDefault()
            //    : config.Dataset.FirstOrDefault(x => x.Name == Globals.ConnectionStringHost);
            //}
            dataset = new DataSetConfiguration()
            {
                Name = config.VirtualHost
            };
            return true;
        }

        public void SendMessage<T>(T message)
        {
            //if (WebExecutionContext.Current?.Cache == null) return;

            GetCurrentDataset();

            // if the service bus is already initialised
            if (Globals.ServiceBusFactory != null)
            {
                // and the dataset has changed for this session to another dataset
                if (Globals.ServiceBusFactory.VirtualHost.ToLower() != dataset.Name.ToLower())
                {
                    // reinitialise the service bus factory, connection and channel
                    Globals.ServiceBusFactory = new ConnectionFactory
                    {
                        HostName = config.HostURL,
                        UserName = config.ServiceAccountUsername,
                        Password = config.ServiceAccountPassword,
                        Port = config.AMQPPort,
                        VirtualHost = dataset.Name.ToLower()
                    };
                    Globals.ServiceBusConnection = Globals.ServiceBusFactory.CreateConnection();
                    Globals.ServiceBusChannel = Globals.ServiceBusConnection.CreateModel();
                }
            }

            // if the service bus channel is not initialised or is closed
            if (Globals.ServiceBusChannel == null || Globals.ServiceBusChannel.IsClosed)
            {
                // if the factory hasn't been initialised, initialise it
                if (Globals.ServiceBusFactory == null)
                {
                    Globals.ServiceBusFactory = new ConnectionFactory
                    {
                        HostName = config.HostURL,
                        UserName = config.ServiceAccountUsername,
                        Password = config.ServiceAccountPassword,
                        Port = config.AMQPPort,

                        VirtualHost = dataset.Name.ToLower()
                    };
                }
                // initalise a connection and channel
                Globals.ServiceBusConnection = Globals.ServiceBusFactory.CreateConnection();
                Globals.ServiceBusChannel = Globals.ServiceBusConnection.CreateModel();
            }

            Publish.SendMessage<T>(Globals.ServiceBusChannel, GetResolvedQueueName(), message);
        }

        private string GetResolvedQueueName()
        {
            string currentUserName = GetCurrentUsername();

            return PathHelper.GetQueueName(dataset.Name, currentUserName);
        }

        private string GetCurrentUsername()
        {
            return config.Username;
            //   return currentUserNameResolver.GetUsername();
        }
    }
}
