using MDL.ServiceBus;
using RabbitMQ.Client.Events;
using RabbitMQ.LoadTester.BLL;
using RabbitMQ.LoadTester.NEW.Morphis;
using RabbitMQ.LoadTester.NEW.Novus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RabbitMQ.LoadTester
{
    public partial class FormNewPackage : Form
    {
        public FormNewPackage()
        {
            InitializeComponent();
        }


        private void Logaction(string message)
        {
            textBox1.Invoke(new Action(() =>
                {
                    textBox1.Text = message + Environment.NewLine + Environment.NewLine + textBox1.Text;
                }));
        }


        private ServiceBusManager _SINGLEserviceBusManager;
        string _SINGLEDataSetName = "loadtestc";
        string _SINGLEnovusUsername = "tcole";
        string _SINGLEserviceBusAccessCode = "guest";


        private void button1_Click(object sender, EventArgs e)
        {
            // NOVUS
            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "FormNewPackage.button1_Click() Start");

            string serviceBusHost = Properties.Settings.Default.ServiceBusHost;
            ushort amqpPort;
            ushort.TryParse(Properties.Settings.Default.ServiceBusAMQPPort, out amqpPort);

            Action<BasicDeliverEventArgs> queueNotification = ea =>
                {
                    var body = ea.Body.ToArray();
                    var message = (object)Encoding.UTF8.GetString(body);
                    var contentType = ea.BasicProperties.ContentType;
                    var objectType = ea.BasicProperties.Type;

                    Logaction($"SingleServiceBus.Received({objectType}): {message} " + TimeHelp.GetTimeText());
                };


            _SINGLEserviceBusManager = new ServiceBusManager(_SINGLEDataSetName, _SINGLEnovusUsername, _SINGLEserviceBusAccessCode, serviceBusHost, amqpPort, queueNotification);

            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "FormNewPackage.button1_Click() End");
            ((Button)sender).Enabled = false;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // MORPHIS
            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "FormNewPackage.button2_Click() Start");

            string serviceBusHost = Properties.Settings.Default.ServiceBusHost;
            ushort amqpPort;
            ushort.TryParse(Properties.Settings.Default.ServiceBusAMQPPort, out amqpPort);


            // Send a message
            var RabbitConfig = new RabbitMQConfiguration()
            {
                AMQPPort = amqpPort,
                HostURL = serviceBusHost,
                ServiceAccountPassword = _SINGLEserviceBusAccessCode,
                Username =  _SINGLEnovusUsername,
                DataSetName  = _SINGLEDataSetName
            };


            var bob = new NovusMsgBrokerEC(RabbitConfig);
            //var userLoop = new NovusMsgBrokerSetupClientUserCmd(bob, "guest", "guest");
            //userLoop.Execute();

            bob.SendMessage("guest: sta" + TimeHelp.GetTimeText());

            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "FormNewPackage.button2_Click() End");

        }
    }
}
