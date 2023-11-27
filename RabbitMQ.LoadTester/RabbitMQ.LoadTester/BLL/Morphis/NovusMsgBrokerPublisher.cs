using System;

using Foundations.Extensions.LocalMachineCommands.Broker;

using RabbitMQ.Client;
using RabbitMQ.LoadTester.BLL.Shared;

namespace RabbitMQ.LoadTester.BLL.Morphis
{
    public class NovusMsgBrokerPublisher : IMsgBrokerPublisher
    {
        //NovusConfigurationSettings config;
        //private readonly ICurrentUsernameResolver currentUserNameResolver;
        private DataSetConfiguration dataset;
        private readonly NovusServiceBusConfigSettings novusConfigurationSettings_ServiceBusSettings;

        private readonly string _userName;

        public NovusMsgBrokerPublisher(string userName, string dataSetName) //NovusConfigurationSettings config, ICurrentUsernameResolver currentUserNameResolver)
        {
            //this.config = config;
            //this.currentUserNameResolver = currentUserNameResolver;

            //if (!GetCurrentDataset()) return;
            _userName = userName;
            dataset = new DataSetConfiguration() { Name = dataSetName };
            novusConfigurationSettings_ServiceBusSettings = new NovusServiceBusConfigSettings();
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
            return true;
        }

        public void SendMessage<T>(T message)
        {
            //if (WebExecutionContext.Current?.Cache == null) return;

            //GetCurrentDataset();

            // if the service bus is already initialised
            if (Globals.ServiceBusFactory != null)
            {
                // and the dataset has changed for this session to another dataset
                if (Globals.ServiceBusFactory.VirtualHost.ToLower() != dataset.Name.ToLower())
                {
                    // reinitialise the service bus factory, connection and channel
                    Globals.ServiceBusFactory = new ConnectionFactory
                    {
                        HostName = novusConfigurationSettings_ServiceBusSettings.Host,
                        UserName = novusConfigurationSettings_ServiceBusSettings.ServiceAccount,
                        Password = novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword,
                        Port = ushort.Parse(novusConfigurationSettings_ServiceBusSettings.AMQPPort),
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
                        HostName = novusConfigurationSettings_ServiceBusSettings.Host,
                        UserName = novusConfigurationSettings_ServiceBusSettings.ServiceAccount,
                        Password = novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword,
                        Port = ushort.Parse(novusConfigurationSettings_ServiceBusSettings.AMQPPort),
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

            return $"{dataset.Name}.rmq.queue.morphis.{currentUserName}".ToLower();
        }

        private string GetCurrentUsername()
        {
            return _userName;
        }
    }
}
