using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SmartControl
{
    class Program
    {
        private static string CONFIG_FILE = "SmartControlConfigs.json";
        private static ServerConfiguration[] serverConfigs;
        public static ILog Logger { get; private set; } =  LogManager.GetLogger("SMRTCNTRL");

        private static void LoadConfigs()
        {
            Logger.Info("Loading server configurations...");
            if (!File.Exists(CONFIG_FILE))
                throw new FileNotFoundException("Couldn't load configs", CONFIG_FILE);

            using (var reader = new StreamReader(CONFIG_FILE))
            {
                Logger.Info("Parsing server configurations...");
                serverConfigs = (ServerConfiguration[])new JsonSerializer().Deserialize(reader, typeof(ServerConfiguration[]));
            }
        }

        private static void CreateConfigs()
        {
            Logger.Info("Creating sample configuration files...");

            var cfg = new ServerConfiguration();
            cfg.Name = "SampleConfiguration";
            cfg.FilePath = "/some/path";
            cfg.WorkingDirectory = "/some/directory";
            cfg.Arguments = "-a -r -g -s";
            
            var cfgs = new ServerConfiguration[] { cfg};
            using (var writer = new StreamWriter(CONFIG_FILE))
                new JsonSerializer() { Formatting = Formatting.Indented }.Serialize(writer, cfgs);
        }

        static void Main(string[] args)
        {
            DOMConfigurator.Configure();
            try
            {
                Logger.Info("Logger initialized!");

                LoadConfigs();

                Logger.Info(string.Format("Loaded {0} config(s):", serverConfigs.Length));

                foreach (var config in serverConfigs)
                    Logger.Info(string.Format("\tLoaded configuration \"{0}\"", config.Name));
            }
            catch (FileNotFoundException ex)
            {
                Logger.Error("You're missing the configuration-file", ex);
                CreateConfigs();
            }
            catch (Exception ex)
            {
                Logger.Error("SmartControl crashed", ex);
            }
            Logger.Info("Exiting...");
            Console.ReadKey();
        }
    }
}
