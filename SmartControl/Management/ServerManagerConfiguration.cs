using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControl.Management
{
    public class ServerManagerConfiguration
    {        
        [JsonProperty(PropertyName = "serverconfigurations")]
        public ServerConfiguration[] ServerConfigurations { get; set; }
    }
}
