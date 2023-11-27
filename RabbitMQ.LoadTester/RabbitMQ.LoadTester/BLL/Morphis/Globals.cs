using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.LoadTester.BLL.Morphis
{
    public class Globals
    {

        public static ConnectionFactory ServiceBusFactory { get; set; }

        public static IConnection ServiceBusConnection { get; set; }

        public static IModel ServiceBusChannel { get; set; }
    }
}
