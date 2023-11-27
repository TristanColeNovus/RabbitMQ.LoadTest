using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.LoadTester.BLL.Morphis
{
    public class NovusServiceBusConfigSettings
    {
        public string Host { get; set; } = Properties.Settings.Default.ServiceBusHost;

        public string AMQPPort { get; set; } = Properties.Settings.Default.ServiceBusAMQPPort;

        public string ApiPort { get; set; } = "15672";

        public string ServiceAccount { get; set; } = "sysman";

        public string ServiceAccountPassword { get; set; } = "guest";

        public bool Development { get; set; }

        public bool DevMessages { get; set; }
    }
}
