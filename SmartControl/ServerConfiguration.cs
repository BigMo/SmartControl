using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControl
{
    public class ServerConfiguration
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "filepath")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "workingdirectory")]
        public string WorkingDirectory { get; set; }

        [JsonProperty(PropertyName = "arguments")]
        public string Arguments { get; set; }
    }
}
