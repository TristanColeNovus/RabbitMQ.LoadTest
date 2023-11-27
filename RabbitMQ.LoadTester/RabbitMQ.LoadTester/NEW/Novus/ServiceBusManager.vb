// Imports MDL.CredentialManager
using MDL.ServiceBus;
using RabbitMQ.Client.Events;
using System;
using System.Drawing.Text;
using System.ComponentModel;
using System.Threading;

namespace RabbitMQ.LoadTester.NEW.Novus
{

    public class ServiceBusManager
    {
        private RabbitMQConfiguration RabbitConfig;
        private string NovusClientAccountName;
        private string SubscriberQueuePassword;
        private Subscribe Subscriber;

        private bool SetupOkay;

        private BackgroundWorker bwSubscribe;

        /// <summary>
        /// Connection Subscribe Retry Max Times
        /// </summary>
        const int MaxRetry = 10;

        /// <summary>
        /// Connection Subscribe Retry interval (milliseconds)
        /// </summary>
        const int RetryInterval = 10000;


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

            // setting up the NOVUS client user account name to be used by the RabbitMQ Service Bus
            //NovusClientAccountName = PathHelper.GetUserName(dataset, novusUsername);
            // the password used to access the queue
            SubscriberQueuePassword = queuePassword;

            RabbitConfig = new RabbitMQConfiguration()
            {
                AMQPPort = amqpPort,
                DataSetName = dataset,
                HostURL = serviceBusHost,
                Username = novusUsername,
            };

            // setting up object to contain data to perform the Service Bus setup process
            //var novusClientCredentials = SetupNovusClientAccountInCredentialManager();

            // if there//s no password stored in the credential manager, do not subscribe to the service bus
            //if (String.IsNullOrEmpty(novusClientCredentials.Password)) return;

            //Use background worker to setup, on a loop if the User/Exchange/Vhost/Queue is not setup yet

            bwSubscribe = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = false
            };
            bwSubscribe.DoWork += BwSubscribe_DoWork;
            bwSubscribe.ProgressChanged += BwSubscribe_ProgressChanged;

            bwSubscribe.RunWorkerAsync(queueEvent);

        }

        private void BwSubscribe_DoWork(Object sender, DoWorkEventArgs e)
        {
            this.SetupOkay = false;
            int n = 0;
            do
            {
                try
                {
                    // setup NOVUS client as a service bus subscriber
                    Subscriber = new Subscribe(RabbitConfig, SubscriberQueuePassword, (Action<BasicDeliverEventArgs>)e.Argument);

                    this.SetupOkay = true;
                    bwSubscribe.ReportProgress(n, "Okay");
                    break;
                }
                catch (Exception ex)
                {
                    bwSubscribe.ReportProgress(n, $"Failed: {ex.Message} {ex.InnerException?.Message}");
                }

                Thread.Sleep(RetryInterval);
                n++;
            } while (n < MaxRetry);

            bwSubscribe.ReportProgress(MaxRetry, "Ended");

        }

        private void BwSubscribe_ProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            System.Diagnostics.Trace.TraceInformation(BLL.TimeHelp.GetTimeText() + "ServiceBusManager.BwSubscribe_ProgressChanged: {0} | {1}", e.ProgressPercentage, e.UserState);
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