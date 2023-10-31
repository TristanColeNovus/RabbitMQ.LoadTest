using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.LoadTester.BLL
{
    public class TimeHelp
    {
        public static string GetTimeText()
        {
            return DateTime.Now.ToString( "MM/dd HH:mm:ss.ffff ");
        }
    }
}
