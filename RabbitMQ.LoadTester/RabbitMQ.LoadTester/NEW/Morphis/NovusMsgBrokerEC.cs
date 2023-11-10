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
            dataset = new DataSetConfiguration() { Name = novusConfigurationSettings.VirtualHost };
            novusConfigurationSettings_ServiceBusSettings = novusConfigurationSettings;

            Initialize();
        }

        public override void Initialize()
        {
            TryToSetup();
            InitializePublisher(CreatePublisher());
        }

        private void TryToSetup()
        {
            try
            {
                // RabbitMQConfiguration rabbitMQConfiguration = GetRabbitMQConfiguration();

                using (var rabbitSetup = new Setup())
                {
                    rabbitSetup.VirtualHost(novusConfigurationSettings_ServiceBusSettings);
                    rabbitSetup.Exchange(novusConfigurationSettings_ServiceBusSettings);
                    // rabbitSetup.UserAccount(rabbitMQConfiguration, novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword, "administrator");
                    // Setup.UserAccountVHostPermissions(rabbitMQConfiguration,
                    //                                    GetBackendUserPermissionsConfig(),
                    //                                    GetBackendUserPermissionsRead(),
                    //                                    GetBackendUserPermissionsWrite());
                    rabbitSetup.UserAccount(novusConfigurationSettings_ServiceBusSettings, novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword, novusConfigurationSettings_ServiceBusSettings.VirtualHost);

                }
            }
            catch (Exception e)
            {
                //errorHandlingPolicy.HandleTryToSetupException(e);
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }


        private RabbitMQConfiguration GetRabbitMQConfiguration()
        {
            return GetRabbitMQConfigurationFor(novusConfigurationSettings_ServiceBusSettings.ServiceAccountUsername);
        }

        private RabbitMQConfiguration GetRabbitMQConfigurationFor(string username)
        {
            var virtualHost = dataset.Name.ToLower();
            return new RabbitMQConfiguration()
            {
                HostURL = novusConfigurationSettings_ServiceBusSettings.HostURL,
                Username = username,
                ServiceAccountUsername = novusConfigurationSettings_ServiceBusSettings.ServiceAccountUsername,
                ServiceAccountPassword = novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword,
                ManagementApiPort = novusConfigurationSettings_ServiceBusSettings.ManagementApiPort,
                VirtualHost = virtualHost,
                ExchangeName = PathHelper.GetExchangeName(dataset.Name),
            };
        }



        private string GetFrontendUserQueueName()
        {
            return PathHelper.GetUserName(dataset.Name, novusConfigurationSettings_ServiceBusSettings.Username);
        }

        private string GetBackendUserPermissionsRead()
        {
            return $"^{dataset.Name}\\.rmq\\.queue\\.morphis.*|{dataset.Name}\\.rmq\\.exchange\\.morphis|amq\\.default$".ToLower();
        }

        private string GetBackendUserPermissionsWrite()
        {
            return $"^{dataset.Name}\\.rmq\\.queue\\.morphis.*|{dataset.Name}\\.rmq\\.exchange\\.morphis|amq\\.default$".ToLower();
        }

        private string GetBackendUserPermissionsConfig()
        {
            return $"^{dataset.Name}\\.rmq\\.queue\\.morphis.*|{dataset.Name}\\.rmq\\.exchange\\.morphis$".ToLower();
        }

        private string GetFrontendUserPermissionsRead()
        {
            return $"^({dataset.Name}\\.rmq\\.queue\\.morphis\\.{novusConfigurationSettings_ServiceBusSettings.Username}|{dataset.Name}\\.rmq\\.exchange\\.morphis|amq\\.default)$".ToLower();
        }

        private string GetFrontendUserPermissionsWrite()
        {
            return $"^({dataset.Name}\\.rmq\\.queue\\.morphis\\.{novusConfigurationSettings_ServiceBusSettings.Username}|{dataset.Name}\\.rmq\\.exchange\\.morphis|amq\\.default)$".ToLower();
        }

        private string GetFrontendUserPermissionsConfig()
        {
            return $"^({dataset.Name}\\.rmq\\.queue\\.morphis\\.{novusConfigurationSettings_ServiceBusSettings.Username})$".ToLower();
        }


        private IMsgBrokerPublisher CreatePublisher()
        {
            return new NovusMsgBrokerPublisher(novusConfigurationSettings_ServiceBusSettings);//, currentUserNameResolver);
        }


        public override void SendMessage<T>(T message)
        {
            NovusMessageWriter writer = NovusMessageWriter.Create<T>(message);
            System.Diagnostics.Trace.TraceInformation(writer.WriteToString());

            //loggingPolicy.LogSendMessage(writer);

            base.SendMessage(message);
        }

        public void SetupClientUser(string username, string password)
        {
            RabbitMQConfiguration rabbitMQConfiguration = GetRabbitMQConfigurationFor(GetFrontendUserQueueName());
            //// Setup.UserAccount(rabbitMQConfiguration, password, rabbitMQConfiguration.VirtualHost);
            //rabbitMqConfiguration.VirtualHost = "%2F"; // backend can now access all users, so must do so using the / virtual host %2F when http encoding escaped
            Setup.UserAccountVHostPermissions(rabbitMQConfiguration,
                                                    GetFrontendUserPermissionsConfig(),
                                                    GetFrontendUserPermissionsRead(),
                                                    GetFrontendUserPermissionsWrite());
        }

        public override void Terminate()
        {

        }
    }
}
