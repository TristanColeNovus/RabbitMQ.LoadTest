using System;
using System.Data;
using Foundations.Extensions.LocalMachineCommands.Broker;
using MDL.ServiceBus;

namespace RabbitMQ.LoadTester.NEW.Morphis
{
    public class NovusMsgBrokerEC : MsgBrokerExecutionContext
    {
        private readonly RabbitMQConfiguration novusConfigurationSettings_ServiceBusSettings;
        //private readonly INovusLMCLoggingPolicy loggingPolicy;
        //private readonly INovusLMCErrorHandlingPolicy errorHandlingPolicy;
        //private readonly ICurrentUsernameResolver currentUserNameResolver;

        private readonly DataSetConfiguration dataset;

        public NovusMsgBrokerEC(RabbitMQConfiguration novusConfigurationSettings)
        {
            //            IOptions<NovusConfigurationSettings> novusConfigurationSettings, 
            //            ICurrentUsernameResolver currentUserNameResolver,
            //            INovusLMCLoggingPolicy loggingPolicy,
            //            INovusLMCErrorHandlingPolicy errorHandlingPolicy)
            //{
            //    this.novusConfigurationSettings = novusConfigurationSettings.Value;
            //    this.currentUserNameResolver = currentUserNameResolver;
            //    this.loggingPolicy = loggingPolicy;
            //    this.errorHandlingPolicy = errorHandlingPolicy;

            //    // get the current user context's dataset
            //    dataset = NovusCurrentUserProvider.GetDataSetConfiguration();
            dataset = new DataSetConfiguration() { Name = novusConfigurationSettings.DataSetName };
            novusConfigurationSettings_ServiceBusSettings = novusConfigurationSettings;

            Initialize();
        }

        public override void Initialize()
        {
            TryToSetup();
            InitializePublisher(CreatePublisher());
        }



        /// <summary>
        /// Setup RabbitMQ Vhost/Exchange/Novus User (inc permissions)
        /// </summary>
        private void TryToSetup()
        {
            try
            {
                // RabbitMQConfiguration rabbitMQConfiguration = GetRabbitMQConfiguration();

                using (var rabbitSetup = new Setup())
                {
                    // Create New VHost
                    if (!rabbitSetup.HasVirtualHost(novusConfigurationSettings_ServiceBusSettings))
                    {
                        rabbitSetup.VirtualHost(novusConfigurationSettings_ServiceBusSettings);
                        // Add morphis-service permissions to vhost
                        rabbitSetup.ServiceAccountVHostPermissions(novusConfigurationSettings_ServiceBusSettings);
                    }
                    // Create New Exchange
                    if (!rabbitSetup.HasExchange(novusConfigurationSettings_ServiceBusSettings))
                    {
                        rabbitSetup.Exchange(novusConfigurationSettings_ServiceBusSettings);
                    }
                    // Create New Queue
                    if (!rabbitSetup.HasQueue(novusConfigurationSettings_ServiceBusSettings))
                    {
                        rabbitSetup.Queue(novusConfigurationSettings_ServiceBusSettings);
                    }
                    // Create Novus User & permissions
                    if (!rabbitSetup.HasUserAccount(novusConfigurationSettings_ServiceBusSettings))
                    {
                        rabbitSetup.UserAccount(novusConfigurationSettings_ServiceBusSettings, novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword, novusConfigurationSettings_ServiceBusSettings.VirtualHost);
                        rabbitSetup.UserAccountVHostPermissions(novusConfigurationSettings_ServiceBusSettings,
                                                            PermissionHelper.GetFrontendUserConfig(novusConfigurationSettings_ServiceBusSettings.VirtualHost, novusConfigurationSettings_ServiceBusSettings.Username),
                                                            PermissionHelper.GetFrontendUserWrite(novusConfigurationSettings_ServiceBusSettings.VirtualHost, novusConfigurationSettings_ServiceBusSettings.Username),
                                                            PermissionHelper.GetFrontendUserRead(novusConfigurationSettings_ServiceBusSettings.VirtualHost, novusConfigurationSettings_ServiceBusSettings.Username));
                    }
                    // Create Queue

                }
            }
            catch (Exception e)
            {
                //errorHandlingPolicy.HandleTryToSetupException(e);
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }



        //private RabbitMQConfiguration GetRabbitMQConfiguration()
        //{
        //    return GetRabbitMQConfigurationFor(novusConfigurationSettings_ServiceBusSettings.ServiceAccountUsername);
        //}

        //private RabbitMQConfiguration GetRabbitMQConfigurationFor(string username)
        //{
        //    var virtualHost = dataset.Name.ToLower();
        //    return new RabbitMQConfiguration()
        //    {
        //        HostURL = novusConfigurationSettings_ServiceBusSettings.HostURL,
        //        Username = username,
        //        ServiceAccountUsername = novusConfigurationSettings_ServiceBusSettings.ServiceAccountUsername,
        //        ServiceAccountPassword = novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword,
        //        ManagementApiPort = novusConfigurationSettings_ServiceBusSettings.ManagementApiPort,
        //        DataSetName = d
        //        VirtualHost = virtualHost,
        //        ExchangeName = PathHelper.GetExchangeName(dataset.Name),
        //    };
        //}



        private string GetFrontendUserQueueName()
        {
            return PathHelper.GetQueueName(dataset.Name, novusConfigurationSettings_ServiceBusSettings.Username);
        }


        private IMsgBrokerPublisher CreatePublisher()
        {
            return new NovusMsgBrokerPublisher(novusConfigurationSettings_ServiceBusSettings);//, currentUserNameResolver);
        }


        public override void SendMessage<T>(T message)
        {
            NovusMessageWriter writer = NovusMessageWriter.Create<T>(message);
            System.Diagnostics.Trace.TraceInformation(message.ToString());

            //loggingPolicy.LogSendMessage(writer);

            base.SendMessage(message);
        }

        public override void Terminate()
        {

        }
    }
}
