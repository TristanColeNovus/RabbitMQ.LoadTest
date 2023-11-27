using System;
using System.Collections.Generic;
using System.Text;

namespace Foundations.Extensions.LocalMachineCommands.Broker
{
    public interface IMsgBrokerExecutionContext
    {
        public void SendMessage<T>(T message);
    }
}
