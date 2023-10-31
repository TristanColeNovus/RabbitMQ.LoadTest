using System;
using System.Data;
using Foundations.Extensions.LocalMachineCommands.Broker;
using RabbitMQ.LoadTester.BLL.Shared;

namespace RabbitMQ.LoadTester.BLL.Morphis
{
    public class NovusMsgBrokerEC : MsgBrokerExecutionContext
    {
        private readonly NovusServiceBusConfigSettings novusConfigurationSettings_ServiceBusSettings;
        //private readonly INovusLMCLoggingPolicy loggingPolicy;
        //private readonly INovusLMCErrorHandlingPolicy errorHandlingPolicy;
        //private readonly ICurrentUsernameResolver currentUserNameResolver;

        private readonly DataSetConfiguration dataset;
        private readonly string _username;

        public NovusMsgBrokerEC(string username, string dataSetName)
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
            _username = username;
            dataset = new DataSetConfiguration() { Name = dataSetName };
            novusConfigurationSettings_ServiceBusSettings = new NovusServiceBusConfigSettings();

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
                RabbitMqConfiguration rabbitMqConfiguration = GetRabbitMqConfiguration();

                Setup.VirtualHost(rabbitMqConfiguration);
                Setup.Exchange(rabbitMqConfiguration);
                Setup.UserAccount(rabbitMqConfiguration, "guest", "administrator");
                Setup.UserAccountVHostPermissions(rabbitMqConfiguration,
                                                    GetBackendUserPermissionsConfig(),
                                                    GetBackendUserPermissionsRead(),
                                                    GetBackendUserPermissionsWrite());
                //rabbitMqConfiguration.Username = GetFrontendUserQueueName();
                //Setup.UserAccount(rabbitMqConfiguration, novusConfigurationSettings.ServiceBusSettings.ServiceAccountPassword, rabbitMqConfiguration.VirtualHost);
                //Setup.UserAccountVHostPermissions(rabbitMqConfiguration,
                //                                GetFrontendUserPermissionsConfig(),
                //                                GetFrontendUserPermissionsRead(),
                //                                GetFrontendUserPermissionsWrite());
            }
            catch (Exception e)
            {
                //errorHandlingPolicy.HandleTryToSetupException(e);
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }

        private RabbitMqConfiguration GetRabbitMqConfiguration()
        {
            return GetRabbitMqConfigurationFor(novusConfigurationSettings_ServiceBusSettings.ServiceAccount);
        }
        private RabbitMqConfiguration GetRabbitMqConfigurationFor(string username)
        {
            var virtualHost = dataset.Name.ToLower();
            return new RabbitMqConfiguration()
            {
                Url = novusConfigurationSettings_ServiceBusSettings.Host,
                Username = username,
                ServiceAccountUsername = novusConfigurationSettings_ServiceBusSettings.ServiceAccount,
                ServiceAccountPassword = novusConfigurationSettings_ServiceBusSettings.ServiceAccountPassword,
                ManagementPort = novusConfigurationSettings_ServiceBusSettings.ApiPort,
                VirtualHost = virtualHost,
                ExchangeName = ResolvedExchangeName()
            };
        }

        private string GetFrontendUserQueueName()
        {
            return $"{dataset.Name}.rmq.morphis.{_username}".ToLower();
        }

        private string ResolvedExchangeName()
        {
            return $"{dataset.Name}.rmq.exchange.morphis".ToLower();
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
            return $"^({dataset.Name}\\.rmq\\.queue\\.morphis\\.{_username}|{dataset.Name}\\.rmq\\.exchange\\.morphis|amq\\.default)$".ToLower();
        }

        private string GetFrontendUserPermissionsWrite()
        {
            return $"^({dataset.Name}\\.rmq\\.queue\\.morphis\\.{_username}|{dataset.Name}\\.rmq\\.exchange\\.morphis|amq\\.default)$".ToLower();
        }

        private string GetFrontendUserPermissionsConfig()
        {
            return $"^({dataset.Name}\\.rmq\\.queue\\.morphis\\.{_username})$".ToLower();
        }

        private IMsgBrokerPublisher CreatePublisher()
        {
            return new NovusMsgBrokerPublisher(_username, dataset.Name);// novusConfigurationSettings, currentUserNameResolver);
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
            RabbitMqConfiguration rabbitMqConfiguration = GetRabbitMqConfigurationFor(GetFrontendUserQueueName());
            Setup.UserAccount(rabbitMqConfiguration, password, rabbitMqConfiguration.VirtualHost);
            //rabbitMqConfiguration.VirtualHost = "%2F"; // backend can now access all users, so must do so using the / virtual host %2F when http encoding escaped
            Setup.UserAccountVHostPermissions(rabbitMqConfiguration,
                                                    GetFrontendUserPermissionsConfig(),
                                                    GetFrontendUserPermissionsRead(),
                                                    GetFrontendUserPermissionsWrite());
        }

        public override void Terminate()
        {

        }
    }
}
