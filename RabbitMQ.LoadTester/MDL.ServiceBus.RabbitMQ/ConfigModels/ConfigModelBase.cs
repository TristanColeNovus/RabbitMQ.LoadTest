using Newtonsoft.Json;
using System;

namespace MDL.ServiceBus.ConfigModels
{
    /// <summary>
    /// Abstract (MustInherit) class for base of Config Models
    /// </summary>
    public abstract class ConfigModelBase
    {

        /// <summary>
        /// Uses JconConvert to output the Class to Json string
        /// </summary>
        /// <returns>Json String</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, formatting: Formatting.None);
        }
    }
}
