using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MDL.ServiceBus
{
    /// <summary>
    /// Used to create Rabbit MQ queue/exchange names
    /// </summary>
    public class PathHelper
    {

        /// <summary>
        /// Returns the Exchange name
        /// <code>{datasetName}.rmq.exchange.morphis</code>
        /// </summary>
        /// <param name="datasetName">Dataset Name (vhost)</param>
        /// <returns>Lowercase full exchange name</returns>
        public static string GetExchangeName(string datasetName)
        {
            return $"{datasetName}.rmq.exchange.morphis".ToLower();
        }



        /// <summary>
        /// Returns the Queue name
        /// <code>{datasetName}.rmq.queue.morphis.{novusUsername}</code>
        /// </summary>
        /// <param name="datasetName">Dataset Name (vhost)</param>
        /// <param name="novusUsername">Novus Client Username</param>
        /// <returns>Lowercase full queue name</returns>
        public static string GetQueueName(string datasetName, string novusUsername)
        {
            return $"{datasetName}.rmq.queue.morphis.{novusUsername}".ToLower();
        }



        /// <summary>
        /// Returns the (Novus) User name
        /// <code>{datasetName}.rmq.morphis.{novusUsername}</code>
        /// </summary>
        /// <param name="datasetName">Dataset Name (vhost)</param>
        /// <param name="novusUsername">Novus Client Username</param>
        /// <returns>Lowercase full user name</returns>
        public static string GetUserName(string datasetName, string novusUsername)
        {
            return $"{datasetName}.rmq.morphis.{novusUsername}".ToLower();
        }


    }
}
