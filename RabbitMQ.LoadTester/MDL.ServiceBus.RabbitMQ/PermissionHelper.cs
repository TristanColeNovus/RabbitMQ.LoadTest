using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MDL.ServiceBus
{
    /// <summary>
    /// Used to Create Regex Permissions
    /// </summary>
    public class PermissionHelper
    {

        public static string GetFrontendUserRead(string vHost, string novusUsername)
        {
            return $"^({vHost}\\.rmq\\.queue\\.morphis\\.{novusUsername}|{vHost}\\.rmq\\.exchange\\.morphis|amq\\.default)$".ToLower();
        }

        public static string GetFrontendUserWrite(string vHost, string novusUsername)
        {
            return $"^({vHost}\\.rmq\\.queue\\.morphis\\.{novusUsername}|{vHost}\\.rmq\\.exchange\\.morphis|amq\\.default)$".ToLower();
        }

        public static string GetFrontendUserConfig(string vHost, string novusUsername)
        {
            return $"^({vHost}\\.rmq\\.queue\\.morphis\\.{novusUsername})$".ToLower();
        }


        public static string GetBackendUserRead(string vHost)
        {
            return $"^{vHost}\\.rmq\\.queue\\.morphis.*|{vHost}\\.rmq\\.exchange\\.morphis|amq\\.default$".ToLower();
        }

        public static string GetBackendUserWrite(string vHost)
        {
            return $"^{vHost}\\.rmq\\.queue\\.morphis.*|{vHost}\\.rmq\\.exchange\\.morphis|amq\\.default$".ToLower();
        }

        public static string GetBackendUserConfig(string vHost)
        {
            return $"^{vHost}\\.rmq\\.queue\\.morphis.*|{vHost}\\.rmq\\.exchange\\.morphis$".ToLower();
        }

    }
}
