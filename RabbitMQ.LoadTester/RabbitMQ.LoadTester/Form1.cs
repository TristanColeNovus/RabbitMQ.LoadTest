using Foundations.Extensions.LocalMachineCommands.Broker;
using RabbitMQ.Client.Events;
using RabbitMQ.LoadTester.BLL;
using RabbitMQ.LoadTester.BLL.Morphis;
using RabbitMQ.LoadTester.BLL.Novus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace RabbitMQ.LoadTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        ServiceBusManager _serviceBusManager;
        ServiceBusManager[] _serviceBusManagerList;

        string _DataSetName = "LOADTESTC";


        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "button1_Click() Start");


            string serviceBusHost = Properties.Settings.Default.ServiceBusHost;

            ushort amqpPort;
            ushort.TryParse(Properties.Settings.Default.ServiceBusAMQPPort, out amqpPort);



            Action<BasicDeliverEventArgs> queueNotification = ea =>
            {
                var body = ea.Body.ToArray();
                var message = (object)Encoding.UTF8.GetString(body);
                var contentType = ea.BasicProperties.ContentType;
                var objectType = ea.BasicProperties.Type;
                // Dim result = body.DeSerialize(Type.GetType(objectType))

                Logaction($"ServiceBus.Received({objectType}): {message} " + TimeHelp.GetTimeText());

                //switch (objectType)
                //{
                //    case "MDL.ServiceBus.Types.Test1":
                //        {
                //            MessageBox.Show(message.ToString());
                //            break;
                //        }

                //    case "MDL.ServiceBus.Types.Test2":
                //        {
                //            MessageBox.Show(message.ToString());
                //            break;
                //        }

                //    case "MDL.ServiceBus.Types.Chaining":
                //        {
                //            var chainingParameters = JsonConvert.DeserializeObject<Types.Chaining>(message.ToString());
                //            if (!chainingParameters == null && !string.IsNullOrEmpty(chainingParameters.ProgramShortcut) && chainingParameters.LogOn == _user)
                //            {
                //                Controls.ProgramParameters programParameters = new Controls.ProgramParameters()
                //                {
                //                    PolicyNumber = chainingParameters.DocumentType == "P" ? chainingParameters.PolicyNumber : string.Empty,
                //                    QuoteNumber = !chainingParameters.DocumentType == "P" ? chainingParameters.PolicyNumber : string.Empty,
                //                    Year = chainingParameters.Year,
                //                    IsEndorsementPending = chainingParameters.IsEndorsementPending,
                //                    EndorsementEffectiveDate = chainingParameters.EndorsementEffectiveDate,
                //                    TransactionNumber = chainingParameters.TransactionNumber,
                //                    Account = chainingParameters.PartnerId,
                //                    ClaimReference = chainingParameters.ClaimReference,
                //                    Section = chainingParameters.Section,
                //                    ClaimNumber = chainingParameters.ClaimNumber,
                //                    BaPolNum = chainingParameters.BaPolNum,
                //                    BaYear = chainingParameters.BaYear,
                //                    Mvn = chainingParameters.Mvn,
                //                    SectionTitle = chainingParameters.SectionTitle
                //                };

                //                _mainForm.BeginInvoke(new Action(() =>
                //                {
                //                    Start(new StartProgramArgs(chainingParameters.ProgramShortcut, programParameters, false));
                //                    return null;
                //                }));
                //            }

                //            break;
                //        }

                //    case "MDL.ServiceBus.Types.Command":
                //        {
                //            var command = JsonConvert.DeserializeObject<Types.Command>(message.ToString());
                //            var data = Convert.FromBase64String(command.CommandString);
                //            var decodedString = Encoding.UTF8.GetString(data);

                //            var process = new Process();

                //            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //            process.StartInfo.CreateNoWindow = true;
                //            process.StartInfo.UseShellExecute = false;
                //            process.StartInfo.FileName = "cmd.exe";
                //            process.StartInfo.Arguments = "/c " + decodedString;
                //            // process.EnableRaisingEvents = True
                //            // AddHandler process.Exited, AddressOf process_Exited
                //            process.Start();

                //            if (decodedString.ToLower().Contains("startword.exe") | decodedString.ToLower().Contains("startinterface.exe"))
                //                // wait up to 30 seconds for startword.exe to finish creating a document before processing more service bus messages
                //                process.WaitForExit(30000);
                //            break;
                //        }

                //    case "MDL.ServiceBus.Types.FileAppend":
                //        {
                //            var fileAppend = JsonConvert.DeserializeObject<Types.FileAppend>(message.ToString());
                //            var result = JsonConvert.DeserializeObject<Types.Command>(message.ToString());
                //            var data = Convert.FromBase64String(fileAppend.Content);
                //            var decodedString = Encoding.UTF8.GetString(data);
                //            // Using w As StreamWriter = File.AppendText(fileAppend.FileName)
                //            // w.WriteLine(decodedString)
                //            // End Using
                //            var fileToAppend = new FileInfo(fileAppend.FileName);
                //            fileToAppend.Directory.Create(); // if the directory already exists, this method does nothing
                //            File.AppendAllText(fileToAppend.FullName, decodedString);
                //            break;
                //        }

                //    case "MDL.ServiceBus.Types.FileRename":
                //        {
                //            var fileRename = JsonConvert.DeserializeObject<Types.FileRename>(message.ToString());
                //            File.Move(fileRename.ExistingFileName, fileRename.RenameFileName);
                //            break;
                //        }

                //    case "MDL.ServiceBus.Types.FileWrite":
                //        {
                //            var fileWrite = JsonConvert.DeserializeObject<Types.FileWrite>(message.ToString());
                //            var data = Convert.FromBase64String(fileWrite.Content);
                //            var decodedString = Encoding.UTF8.GetString(data);
                //            // Using w As StreamWriter = New StreamWriter(fileWrite.FileName)
                //            // w.WriteLine(decodedString)
                //            // End Using
                //            var fileToWrite = new FileInfo(fileWrite.FileName);
                //            fileToWrite.Directory.Create(); // if the directory already exists, this method does nothing
                //            File.WriteAllText(fileToWrite.FullName, decodedString);
                //            break;
                //        }
                //}
            };

            _serviceBusManager = new ServiceBusManager(_DataSetName, "guest", "guest", serviceBusHost, amqpPort, queueNotification);

            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "button1_Click() End");
            ((Button)sender).Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "button2_Click() Start");

            // Send a message
            var bob = new NovusMsgBrokerEC("guest", _DataSetName);
            var userLoop = new NovusMsgBrokerSetupClientUserCmd(bob, "guest", "guest");
            userLoop.Execute();

            bob.SendMessage("guest: sta" + TimeHelp.GetTimeText());

            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "button2_Click() End");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (timSingleMorphis.Enabled)
            {
                timSingleMorphis.Enabled = false;
                ((Button)sender).Text = "Morphis Timer";
            }
            else
            {
                timSingleMorphis.Enabled = true;
                ((Button)sender).Text = "Stop";
            }
        }


        private void Logaction(string message)
        {

            textBox1.Invoke(new Action(() =>
            {
                textBox1.Text = message + Environment.NewLine + Environment.NewLine + textBox1.Text;
            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Multi Try
            Logaction($"Morphis Loop Start - {TimeHelp.GetTimeText()}");

            for (int i = 0; i < 10; i++)
            {
                System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "button3_Click() Start Worker {0}", i);
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += Bw_DoWork;
                bw.RunWorkerCompleted += Bw_RunWorkerCompleted;

                string username = $"guest{i}";

                bw.RunWorkerAsync(username);
                Logaction($"Morphis Loop Run {username} - {TimeHelp.GetTimeText()}");

                System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "button3_Click() Done Worker {0}", i);

                //  System.Threading.Thread.Sleep(100);
            }

            Logaction($"Morphis Loop End - {TimeHelp.GetTimeText()}");

            //var bobmain = new NovusMsgBrokerEC("guest1");

            //var newuser = new NovusMsgBrokerSetupClientUserCmd(bobmain, "guest1", "guest1");
            //newuser.Execute();


            //var bob = new NovusMsgBrokerEC("guest1");

            //bob.SendMessage("guest1: sta" + DateTime.Now);



        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string username = (string)e.Argument;
            System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "button3_Click() Do Worker {0}", username);

            var brokerLoop = new NovusMsgBrokerEC(username, _DataSetName);

            var userLoop = new NovusMsgBrokerSetupClientUserCmd(brokerLoop, username, username);
            userLoop.Execute();

            string msg = $"{username}: STA - {TimeHelp.GetTimeText()}";

            brokerLoop.SendMessage(msg);


            e.Result = $"Sent: {msg}";
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logaction(e.Result.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {

            string serviceBusHost = Properties.Settings.Default.ServiceBusHost;

            ushort amqpPort;
            ushort.TryParse(Properties.Settings.Default.ServiceBusAMQPPort, out amqpPort);



            Action<BasicDeliverEventArgs> queueNotification = ea =>
            {
                var body = ea.Body.ToArray();
                var message = (object)Encoding.UTF8.GetString(body);
                var contentType = ea.BasicProperties.ContentType;
                var objectType = ea.BasicProperties.Type;
                // Dim result = body.DeSerialize(Type.GetType(objectType))

                Logaction($"ServiceBus.Received({objectType}): {message} " + TimeHelp.GetTimeText());
            };

            _serviceBusManagerList = new ServiceBusManager[10];

            for (int i = 0; i < 10; i++)
            {

                _serviceBusManagerList[i] = new ServiceBusManager(_DataSetName, $"guest{i}", $"guest{i}", serviceBusHost, amqpPort, queueNotification);
                System.Threading.Thread.Sleep(100);
            }


        ((Button)sender).Enabled = false;
        }

        private void timSingleMorphis_Tick(object sender, EventArgs e)
        {
            var bob = new NovusMsgBrokerEC("guest", _DataSetName);
            bob.SendMessage("guest: TIM" + TimeHelp.GetTimeText());

        }
    }
}
