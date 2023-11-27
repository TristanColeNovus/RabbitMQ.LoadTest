using System;
using System.Collections.Generic;
using System.Text;

namespace Foundations.Extensions.LocalMachineCommands.Broker
{
    public interface IMsgBrokerPublisher
    {
        void SendMessage<T>(T message);
    }
}
