using MDL.ServiceBus.ConfigModels;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace MDL.ServiceBus
{
    /// <summary>
    /// Used to Create Virtual Hosts, Exchanges, Queues, Users, and User Permissions
    /// </summary>
    public class Setup : IDisposable
    {
        private HttpClient _httpClient;
        private readonly ILogger<Setup> _logger = null;

        public Setup(ILogger<Setup> logger = null)
        {
            _logger = logger;
        }


        public void Dispose()
        {
            ((IDisposable)_httpClient).Dispose();
        }


        /// <summary>
        /// Chheck and create (if needed) a connection to the RabbitMQ Management Web API
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, ServiceAccountUsername, ServiceAccountPassword)</param>
        private void CreateHttpClient(RabbitMQConfiguration cfg)
        {
            if (_httpClient == null)
            {
                var credentials = new NetworkCredential() { UserName = cfg.ServiceAccountUsername, Password = cfg.ServiceAccountPassword };
                var handler = new HttpClientHandler { Credentials = credentials };

                _httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri($"http://{cfg.HostURL}:{cfg.ManagementApiPort}")
                };
            }
        }

        #region "Virtual Host"

        /// <summary>
        /// Setup RabbitMQ VirtualHost
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, ServiceAccountUsername, ServiceAccountPassword)</param>
        public void VirtualHost(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Create Object using Put
            var url = $"api/vhosts/{cfg.VirtualHost}";
            var configModel = new VirtualHostAdd()
            {
                Description = $"{cfg.VirtualHost} Dataset",
                Tags = cfg.VirtualHost
            };
            var content = new StringContent(configModel.ToString(), Encoding.UTF8, "application/json");
            var result = _httpClient.PutAsync(url, content).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.VirtualHost({0}) Result:{1}", cfg, result.StatusCode);

            if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.VirtualHost({cfg}): {result}");
        }



        /// <summary>
        /// Checks to see if a RabbitMQ VirtualHost exists
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, ServiceAccountUsername, ServiceAccountPassword)</param>
        /// <returns>True if found, false if not or throws error if failed</returns>
        public bool HasVirtualHost(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Do Lookup
            var url = $"api/vhosts/{cfg.VirtualHost}";
            var result = _httpClient.GetAsync(url).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.HasVirtualHost({0}) Result:{1}", cfg, result.StatusCode);

            // Check if VHost is found
            if ((int)result.StatusCode == 200)  // Found
                return true;

            else if ((int)result.StatusCode == 404) // Not Found
                return false;

            else if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.HasVirtualHost({cfg}): {result}");

            return false;
        }

        #endregion

        #region "User Account"

        /// <summary>
        /// Create RabbitMQ User account
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, RabbitMQUsername, VirtualHost, ServiceAccountUsername, ServiceAccountPassword)</param>
        /// <param name="password">User account password</param>
        /// <param name="tags">User account tags (administrator or other)</param>
        public void UserAccount(RabbitMQConfiguration cfg, string password, string tags)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Do Create
            var url = $"api/users/{cfg.RabbitMQUsername}";
            var configModel = new UserAccountAdd()
            {
                PasswordHash = EncodePassword(password),
                Description = $"{cfg.VirtualHost} Dataset",
                Tags = tags
            };
            var content = new StringContent(configModel.ToString(), Encoding.UTF8, "application/json");
            var result = _httpClient.PutAsync(url, content).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.UserAccount({0}) Result:{1}", cfg, result.StatusCode);

            if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.UserAccount({cfg}): {result}");
        }



        /// <summary>
        /// Checks to see if a RabbitMQ User Account exists
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, RabbitMQUsername, ServiceAccountUsername, ServiceAccountPassword)</param>
        /// <returns>True if found, false if not or throws error if failed</returns>
        public bool HasUserAccount(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Do Get Lookup
            var url = $"api/users/{cfg.RabbitMQUsername}";
            var result = _httpClient.GetAsync(url).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.HasUserAccount({0}) Result:{1}", cfg, result.StatusCode);

            // Check if User is found
            if ((int)result.StatusCode == 200)  // Found
                return true;

            else if ((int)result.StatusCode == 404) // Not Found
                return false;

            else if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.HasUserAccount({cfg}): {result}");

            return false;
        }



        /// <summary>
        /// Setup RabbitMQ VirtualHost Permissions for a User Account
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, RabbitMQUsername, ServiceAccountUsername, ServiceAccountPassword)</param>
        /// <param name="configure">Regex Permissions</param>
        /// <param name="write">Regex Permissions</param>
        /// <param name="read">Regex Permissions</param>
        public void UserAccountVHostPermissions(RabbitMQConfiguration cfg, string configure = ".*", string write = ".*", string read = ".*")
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            var url = $"api/permissions/{cfg.VirtualHost}/{cfg.RabbitMQUsername}";
            var configModel = new UserPermissionAdd()
            {
                Username = cfg. RabbitMQUsername,
                VHost = cfg.VirtualHost,
                Configure = configure,
                Write = write,
                Read = read
            };
            var content = new StringContent(configModel.ToString(), Encoding.UTF8, "application/json");
            var result = _httpClient.PutAsync(url, content).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.UserAccountVHostPermissions({0}) Result:{1}", cfg, result.StatusCode);

            if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.UserAccountVHostPermissions({cfg}): {result}");
        }



        /// <summary>
        /// Setup RabbitMQ VirtualHost Permissions for the Service Account
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, ServiceAccountUsername, ServiceAccountPassword)</param>
        public void ServiceAccountVHostPermissions(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            var url = $"api/permissions/{cfg.VirtualHost}/{cfg.ServiceAccountUsername}";
            var configModel = new UserPermissionAdd()
            {
                Username = cfg.ServiceAccountUsername,
                VHost = cfg.VirtualHost
            };
            var content = new StringContent(configModel.ToString(), Encoding.UTF8, "application/json");
            var result = _httpClient.PutAsync(url, content).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.ServiceAccountVHostPermissions({0}) Result:{1}", cfg, result.StatusCode);

            if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.ServiceAccountVHostPermissions({cfg}): {result}");
        }
        #endregion

        #region "Exchange"

        /// <summary>
        /// Setup RabbitMQ Exchange for a VirtualHost
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, ExchangeName, ServiceAccountUsername, ServiceAccountPassword)</param>
        public void Exchange(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Create using Put
            var url = $"api/exchanges/{cfg.VirtualHost}/{cfg.ExchangeName}";
            var configModel = new ExchangeAdd()
            {
                VHost = cfg.VirtualHost,
                ExchangeName = cfg.ExchangeName
            };
            var content = new StringContent(configModel.ToString(), Encoding.UTF8, "application/json");
            var result = _httpClient.PutAsync(url, content).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.Exchange({0}) Result:{1}", cfg, result.StatusCode);

            if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.Exchange({cfg}): {result}");
        }



        /// <summary>
        /// Checks to see if a RabbitMQ Exchange for a VirtualHost exists
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, ExchangeName, ServiceAccountUsername, ServiceAccountPassword)</param>
        /// <returns>True if found, false if not or throws error if failed</returns>
        public bool HasExchange(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Do Get Lookup
            var url = $"api/exchanges/{cfg.VirtualHost}/{cfg.ExchangeName}";
            var result = _httpClient.GetAsync(url).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.HasExchange({0}) Result:{1}", cfg, result.StatusCode);

            // Check if Exchange is found
            if ((int)result.StatusCode == 200)  // Found
                return true;

            else if ((int)result.StatusCode == 404) // Not Found
                return false;

            else if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.HasExchange({cfg}): {result}");

            return false;
        }

        #endregion

        #region "Queue"

        /// <summary>
        /// Setup RabbitMQ Queue for a VirtualHost
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, QueueName, ServiceAccountUsername, ServiceAccountPassword)</param>
        public void Queue(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Create using Put
            var url = $"api/queues/{cfg.VirtualHost}/{cfg.QueueName}";
            var configModel = new QueueAdd()
            {
                VHost = cfg.VirtualHost,
                QueueName = cfg.QueueName
            };

            Debug.WriteLine(configModel.ToString());

            var content = new StringContent(configModel.ToString(), Encoding.UTF8, "application/json");
            var result = _httpClient.PutAsync(url, content).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.Queue({0}) Result:{1}", cfg, result.StatusCode);

            if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.Queue({cfg}): {result}");
        }



        /// <summary>
        /// Checks to see if a RabbitMQ Queue for a VirtualHost exists
        /// </summary>
        /// <param name="cfg">Rabbit MQ Config, (HostURL, ManagementApiPort, VirtualHost, QueueName, ServiceAccountUsername, ServiceAccountPassword)</param>
        /// <returns>True if found, false if not or throws error if failed</returns>
        public bool HasQueue(RabbitMQConfiguration cfg)
        {
            // Check and Create Connection
            CreateHttpClient(cfg);

            // Do Get Lookup
            var url = $"api/queues/{cfg.VirtualHost}/{cfg.QueueName}";
            var result = _httpClient.GetAsync(url).Result;

            _logger?.LogTrace("MDL.ServiceBus.RabbitMQ.Setup.HasQueue({0}) Result:{1}", cfg, result.StatusCode);

            // Check if Exchange is found
            if ((int)result.StatusCode == 200)  // Found
                return true;

            else if ((int)result.StatusCode == 404) // Not Found
                return false;

            else if ((int)result.StatusCode >= 300)
                throw new Exception($"RabbitMQ.Setup.HasQueue({cfg}): {result}");

            return false;
        }

        #endregion

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

}
