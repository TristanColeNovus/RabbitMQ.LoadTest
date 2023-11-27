using RabbitMQ.Client;
using System;

namespace RabbitMQ.LoadTester.NEW.Morphis
{
    public class Globals
    {

        public static ConnectionFactory ServiceBusFactory { get; set; }

        public static IConnection ServiceBusConnection { get; set; }

        public static IModel ServiceBusChannel { get; set; }
    }
}
