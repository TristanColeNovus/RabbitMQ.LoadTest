using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace RabbitMQ.LoadTester.BLL.Shared
{
    public static class Setup
    {
        /// <summary>
        /// EnsureExistRabbitMqVirtualHost
        /// </summary>
        /// <param name="cfg"></param>
        public static void VirtualHost(RabbitMqConfiguration cfg)
        {
            var credentials = new NetworkCredential() { UserName = cfg.ServiceAccountUsername, Password = cfg.ServiceAccountPassword };
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var client = new HttpClient(handler))
            {
                var url = $"http://{cfg.Url}:{cfg.ManagementPort}/api/vhosts/{cfg.VirtualHost}";

                var content = new StringContent($"{{\"description\":\"{cfg.VirtualHost} Dataset\", \"tags\":\"{cfg.VirtualHost}\"}}", Encoding.UTF8, "application/json");
                var result = client.PutAsync(url, content).Result;

                if ((int)result.StatusCode >= 300)
                    throw new Exception(result.ToString());


                System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "Setup.VirtualHost({0}) : {1}", cfg.VirtualHost, result.StatusCode);
            }
        }

        /// <summary>
        /// EnsureExistRabbitMqUserAccount
        /// </summary>
        /// <param name="cfg"></param>
        public static void UserAccount(RabbitMqConfiguration cfg, string password, string tags)
        {
            var credentials = new NetworkCredential() { UserName = cfg.ServiceAccountUsername, Password = cfg.ServiceAccountPassword };
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var client = new HttpClient(handler))
            {
                var url = $"http://{cfg.Url}:{cfg.ManagementPort}/api/users/{cfg.Username}";

                var passwordHash = EncodePassword(password);

                var content = new StringContent($"{{\"password_hash\":\"{passwordHash}\", \"description\":\"{cfg.VirtualHost} Dataset\", \"tags\":\"{tags}\"}}", Encoding.UTF8, "application/json");
                var result = client.PutAsync(url, content).Result;

                if ((int)result.StatusCode >= 300)
                    throw new Exception(result.ToString());


                System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "Setup.UserAccount({0}) : {1}", cfg.Username, result.StatusCode);
            }
        }

        /// <summary>
        /// SetRabbitMqUserAccountVHostPermissions
        /// </summary>
        /// <param name="cfg"></param>
        public static void UserAccountVHostPermissions(RabbitMqConfiguration cfg, string configure, string write, string read)
        {
            var credentials = new NetworkCredential() { UserName = cfg.ServiceAccountUsername, Password = cfg.ServiceAccountPassword };
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var client = new HttpClient(handler))
            {
                var url = $"http://{cfg.Url}:{cfg.ManagementPort}/api/permissions/{cfg.VirtualHost}/{cfg.Username}";

                //configure = ".*";
                //write = ".*";
                //read = ".*";

                var content = new StringContent($"{{\"username\":\"{cfg.Username}\",\"vhost\":\"{cfg.VirtualHost}\",\"configure\":\"{configure}\",\"write\":\"{write}\",\"read\":\"{read}\"}}", Encoding.UTF8, "application/json");
                var result = client.PutAsync(url, content).Result;
                Debug.WriteLine($"{{\"username\":\"{cfg.Username}\",\"vhost\":\"{cfg.VirtualHost}\",\"configure\":\"{configure}\",\"write\":\"{write}\",\"read\":\"{read}\"}}");

                if ((int)result.StatusCode >= 300)
                    throw new Exception(result.ToString());


                System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "Setup.UserAccountVHostPermissions({0}/{1}) : {2}", cfg.VirtualHost, cfg.Username, result.StatusCode);
            }
        }

        /// <summary>
        /// EnsureExistRabbitMqExchange
        /// </summary>
        /// <param name="cfg"></param>
        public static void Exchange(RabbitMqConfiguration cfg)
        {
            var credentials = new NetworkCredential() { UserName = cfg.ServiceAccountUsername, Password = cfg.ServiceAccountPassword };
            using (var handler = new HttpClientHandler { Credentials = credentials })
            using (var client = new HttpClient(handler))
            {
                var url = $"http://{cfg.Url}:{cfg.ManagementPort}/api/exchanges/{cfg.VirtualHost}/{cfg.ExchangeName}";

                var content = new StringContent($"{{\"type\":\"direct\",\"auto_delete\":false,\"durable\":false,\"internal\":false,\"arguments\":{{}}}}", Encoding.UTF8, "application/json");
                var result = client.PutAsync(url, content).Result;

                if ((int)result.StatusCode >= 300)
                    throw new Exception(result.ToString());


                System.Diagnostics.Trace.TraceInformation(TimeHelp.GetTimeText() + "Setup.Exchange({0}/{1}) : {2}", cfg.VirtualHost, cfg.ExchangeName, (int)result.StatusCode);
            }
        }

        #region "Helpers"
        private static string EncodePassword(string password)
        {
            using (RandomNumberGenerator rand = RandomNumberGenerator.Create())
            using (var sha256 = SHA256.Create())
            {
                byte[] salt = new byte[4];

                rand.GetBytes(salt);

                byte[] saltedPassword = MergeByteArray(salt, Encoding.UTF8.GetBytes(password));
                byte[] saltedPasswordHash = sha256.ComputeHash(saltedPassword);

                return Convert.ToBase64String(MergeByteArray(salt, saltedPasswordHash));
            }
        }

        private static byte[] MergeByteArray(byte[] array1, byte[] array2)
        {
            byte[] merge = new byte[array1.Length + array2.Length];
            array1.CopyTo(merge, 0);
            array2.CopyTo(merge, array1.Length);

            return merge;
        }
        #endregion // Helpers
    }

    public class RabbitMqConfiguration
    {
        public string ServiceAccountUsername { get; set; }
        public string ServiceAccountPassword { get; set; }
        public string Url { get; set; }
        public string ManagementPort { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string ExchangeName { get; set; }
    }
}
