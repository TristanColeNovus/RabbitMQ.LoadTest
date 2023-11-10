// Imports MDL.CredentialManager
using MDL.ServiceBus;
using RabbitMQ.Client.Events;
using System;
using System.Drawing.Text;

namespace RabbitMQ.LoadTester.NEW.Novus
{

    public class ServiceBusManager
    {
        private RabbitMQConfiguration RabbitConfig;
        private string QueueName;
        private string NovusClientAccountName;
        private string SubscriberQueuePassword;
        private Subscribe Subscriber;



        /// <summary>
        /// Subscribe an action delegate to the service bus using the Morphis web app exchange and queue names for the current user
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="novusUsername"></param>
        /// <param name="queuePassword"></param>
        /// <param name="serviceBusHost"></param>
        /// <param name="amqpPort"></param>
        /// <param name="queueEvent"></param>
        public ServiceBusManager(string dataset, string novusUsername, string queuePassword, string serviceBusHost, ushort amqpPort, Action<BasicDeliverEventArgs> queueEvent)
        {
            // setting up the exchange And queue names for the current dataset And NOVUS client user
            QueueName = PathHelper.GetQueueName(dataset, novusUsername);

            // setting up the NOVUS client user account name to be used by the RabbitMQ Service Bus
            NovusClientAccountName = PathHelper.GetUserName(dataset, novusUsername);
            // the password used to access the queue
            SubscriberQueuePassword = queuePassword;

            RabbitConfig = new RabbitMQConfiguration()
            {
                AMQPPort = amqpPort,
                ExchangeName = PathHelper.GetExchangeName(dataset),
                HostURL = serviceBusHost,
                Username = NovusClientAccountName,
                VirtualHost = dataset
            };

            // setting up object to contain data to perform the Service Bus setup process
            //var novusClientCredentials = SetupNovusClientAccountInCredentialManager();

            // if there//s no password stored in the credential manager, do not subscribe to the service bus
            //if (String.IsNullOrEmpty(novusClientCredentials.Password)) return;

            // setup NOVUS client as a service bus subscriber
            Subscriber = new Subscribe(RabbitConfig, QueueName, queuePassword, queueEvent);
        }


        ///// <summary>
        ///// Setup NOVUS client service bus account in the Windows credential manager
        ///// </summary>
        ///// <returns></returns>
        //private Credential SetupNovusClientAccountInCredentialManager()
        //{
        //    Dim novusClientCredentials = New Credential With
        //{
        //        .Target = NovusClientAccountName.ToLower
        //}
        //    If Not novusClientCredentials.Load() Then
        //        // no record of user in credential manager, so create one
        //        novusClientCredentials.Target = NovusClientAccountName.ToLower
        //        novusClientCredentials.Username = NovusClientAccountName.ToLower
        //        novusClientCredentials.Password = SubscriberQueuePassword
        //        novusClientCredentials.PersistanceType = PersistanceType.Enterprise
        //        novusClientCredentials.Save()
        //    ElseIf(Not String.IsNullOrEmpty(SubscriberQueuePassword)) Then
        //        // update the credential manager with the provided password
        //        novusClientCredentials.Password = SubscriberQueuePassword
        //        novusClientCredentials.Save()
        //    End If
        //    Return novusClientCredentials
        //}


    }
}