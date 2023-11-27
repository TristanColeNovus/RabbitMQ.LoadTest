using System;
using System.Collections.Generic;
using System.Text;

namespace Foundations.Extensions.LocalMachineCommands.Broker
{
    public interface IMsgBrokerExecutionContext
    {
        void SendMessage<T>(T message);
    }
}
