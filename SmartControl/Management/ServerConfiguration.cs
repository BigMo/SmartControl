using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControl.Management
{
    public class ServerConfiguration
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "restartDelay")]
        public int RestartDelay { get; set; }

        [JsonProperty(PropertyName = "allowMultipleInstances")]
        public bool AllowMultipleInstances { get; set; }

        [JsonProperty(PropertyName = "filepath")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "workingdirectory")]
        public string WorkingDirectory { get; set; }

        [JsonProperty(PropertyName = "arguments")]
        public string Arguments { get; set; }

        [JsonProperty(PropertyName = "sendOnStart")]
        public string[] SendOnStart { get; set; }

        [JsonProperty(PropertyName = "sendOnStop")]
        public string[] SendOnStop { get; set; }

        public override string ToString()
        {
            var type = typeof(ServerConfiguration);
            var props = type.GetProperties();
            var form = props.Select(x => string.Format("{0}: {1}", x.Name, x.GetValue(this, null)));
            return string.Join(", ", form.ToArray());
        }
    }
}
