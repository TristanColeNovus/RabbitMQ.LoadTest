using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using RabbitMQ.Client.Events;
using RabbitMQ.LoadTester.BLL.Shared;

namespace RabbitMQ.LoadTester.BLL.Novus
{
    public class ServiceBusManager
    {
        private string ExchangeName;
        private string QueueName;
        private string NovusClientAccountName;
        //private string SubscriberQueuePassword;
        private Subscribe Subscriber;



        /// <summary>
        /// Subscribe an action delegate to the service bus using the Morphis web app exchange and queue names for the current user
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="novusUsername"></param>
        /// <param name="queuePassword"></param>
        /// <param name="serviceBusHost"></param>
        /// <param name="ampqPort"></param>
        /// <param name="queueEvent"></param>
        public ServiceBusManager(string dataset, string novusUsername, string queuePassword, string serviceBusHost, ushort ampqPort, Action<BasicDeliverEventArgs> queueEvent)
        {
            // setting up the exchange And queue names for the current dataset And NOVUS client user
            ExchangeName = $"{dataset}.rmq.exchange.morphis".ToLower();
            QueueName = $"{dataset}.rmq.queue.morphis.{novusUsername}".ToLower();

            // setting up the NOVUS client user account name to be used by the RabbitMQ Service Bus
            NovusClientAccountName = $"{dataset}.rmq.morphis.{novusUsername}".ToLower();

            // the password used to access the queue
            //SubscriberQueuePassword = queuePassword;

            // setting up object to contain data to perform the Service Bus setup process
            //var novusClientCredentials = SetupNovusClientAccountInCredentialManager();

            // if there's no password stored in the credential manager, do not subscribe to the service bus
            //if (string.IsNullOrEmpty(novusClientCredentials.Password))
            //    return;

            // setup NOVUS client as a service bus subscriber
            Subscriber = new Subscribe(ExchangeName, QueueName, serviceBusHost, dataset.ToLower(), NovusClientAccountName, queuePassword, ampqPort, queueEvent);
        }
        
        
        
        ///// <summary>
        ///// Setup NOVUS client service bus account in the Windows credential manager
        ///// </summary>
        ///// <returns></returns>
        //private Credential SetupNovusClientAccountInCredentialManager()
        //{
        //    var novusClientCredentials = new Credential()
        //    {
        //        Target = NovusClientAccountName.ToLower()
        //    };
        //    if (!novusClientCredentials.Load())
        //    {
        //        // no record of user in credential manager, so create one
        //        novusClientCredentials.Target = NovusClientAccountName.ToLower();
        //        novusClientCredentials.Username = NovusClientAccountName.ToLower();
        //        novusClientCredentials.Password = SubscriberQueuePassword;
        //        novusClientCredentials.PersistanceType = PersistanceType.Enterprise;
        //        novusClientCredentials.Save();
        //    }
        //    else if ((!string.IsNullOrEmpty(SubscriberQueuePassword)))
        //    {
        //        // update the credential manager with the provided password
        //        novusClientCredentials.Password = SubscriberQueuePassword;
        //        novusClientCredentials.Save();
        //    }
        //    return novusClientCredentials;
        //}
    }
}
