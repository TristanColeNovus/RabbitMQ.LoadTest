using RabbitMQ.Client.Events;
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

namespace RabbitMQ.LoadTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        ServiceBusManager _serviceBusManager;



        private void button1_Click(object sender, EventArgs e)
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

                Logaction($"ServiceBus.Received({objectType}): {message}");

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

            _serviceBusManager = new ServiceBusManager("LOADTEST", "guest", "guest", serviceBusHost, amqpPort, queueNotification);

        }

        private void button2_Click(object sender, EventArgs e)
        {

            var bob = new NovusMsgBrokerEC("guest");

            bob.SendMessage("guest: sta" + DateTime.Now);

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

            var bob = new NovusMsgBrokerEC("guest1");

            bob.SendMessage("guest1: sta" + DateTime.Now);



        }
    }
}
