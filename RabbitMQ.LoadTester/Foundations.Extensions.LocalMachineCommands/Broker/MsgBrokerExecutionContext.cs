using System;
using System.Collections.Generic;
using System.Text;

namespace Foundations.Extensions.LocalMachineCommands.Broker
{
    public abstract class MsgBrokerExecutionContext : IMsgBrokerExecutionContext
    {
        private IMsgBrokerPublisher publisher;

        protected MsgBrokerExecutionContext() : this (new PasstroughMsgBrokerPublisher()) { }

        protected MsgBrokerExecutionContext(IMsgBrokerPublisher publisher)
        {
            InitializePublisher(publisher);
        }

        protected void InitializePublisher(IMsgBrokerPublisher publisher)
        {
            this.publisher = publisher;
        }

        public virtual void SendMessage<T>(T message)
        {
            this.publisher.SendMessage(message);
        }

        public abstract void Initialize();

        public abstract void Terminate();

        internal class PasstroughMsgBrokerPublisher : IMsgBrokerPublisher
        {
            public void SendMessage<T>(T message)
            {
                // do nothing
            }
        }
    }
}
