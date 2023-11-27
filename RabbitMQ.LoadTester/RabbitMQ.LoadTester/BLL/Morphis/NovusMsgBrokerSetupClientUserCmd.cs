using System;
using System.Collections.Generic;
using System.Text;

using Foundations.Extensions.LocalMachineCommands;
using Foundations.Extensions.LocalMachineCommands.Broker;

namespace RabbitMQ.LoadTester.BLL.Morphis
{
    public class NovusMsgBrokerSetupClientUserCmd : ILocalMachineCmd
    {
        private readonly IMsgBrokerExecutionContext msgBrokerExecutionContext;
        
        private readonly string username;
        private readonly string password;

        public NovusMsgBrokerSetupClientUserCmd(IMsgBrokerExecutionContext msgBrokerExecutionContext, string username, string password)
        {
            this.msgBrokerExecutionContext = msgBrokerExecutionContext;
            this.username = username;
            this.password = password;
        }

        public void Execute()
        {
            if (msgBrokerExecutionContext is NovusMsgBrokerEC) {
                NovusMsgBrokerEC novusMsgBrokerEC = msgBrokerExecutionContext as NovusMsgBrokerEC;
                novusMsgBrokerEC.SetupClientUser(username, password);
            }
        }
    }
}
