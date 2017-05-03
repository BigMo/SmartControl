using Newtonsoft.Json;
using SmartControl.Management.Screen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SmartControl.Management
{
    public class ServerManager
    {
        private ServerManagerConfiguration config;

        public ServerConfiguration[] Configurations { get { return config.ServerConfigurations; } }

        public string ConfigFile { get; private set; }
        public ScreenInstance[] ActiveInstances { get; private set; }

        public ServerManager(string configFile)
        {
            ConfigFile = configFile;
            try
            {
                LoadConfigs();

                Program.Logger.Info(string.Format("Loaded {0} config(s)", Configurations.Length));

                foreach (var config in Configurations)
                    Program.Logger.Debug(string.Format("\tLoaded configuration \"{0}\"", config.Name));
            }
            catch (FileNotFoundException ex)
            {
                Program.Logger.Error("You're missing the configuration-file", ex);
                CreateConfigs();
            }
            catch (Exception ex)
            {
                Program.Logger.Error("Failed to load configs", ex);
            }
        }

        public void Update()
        {
            try
            {
                var sessions = ScreenHandler.GetActiveScreens();
                List<ScreenInstance> instances = new List<ScreenInstance>();

                foreach (var cfg in config.ServerConfigurations)
                {
                    var cnt = sessions.Count(x => x.Split('.')[1] == cfg.Name);
                    if (cnt != 0)
                    {
                        var cfgSessions = sessions.Where(x => x.Split('.')[1] == cfg.Name);
                        if (cnt > 1)
                        {
                            if (cfg.AllowMultipleInstances)
                                foreach (var cfgS in cfgSessions)
                                    instances.Add(new ScreenInstance(cfg, cfgS));
                            else
                                Program.Logger.ErrorFormat("Multiple active instances found, yet \"{0}\" does not allow multiple instances", cfg.Name);
                        }
                        else
                        {
                            foreach (var cfgS in cfgSessions)
                                instances.Add(new ScreenInstance(cfg, cfgS));
                        }
                    }
                }

                ActiveInstances = instances.ToArray();
            }catch(Exception ex)
            {
                Program.Logger.Error("Failed to update list of active instances", ex);
            }
        }

        public void Handle(string name, Mode mode)
        {
            switch (mode)
            {
                case Mode.Start:
                    Start(name);
                    break;
                case Mode.Stop:
                    Stop(name);
                    break;
                case Mode.Restart:
                    Restart(name);
                    break;
            }
        }

        private bool ConfigExists(string name)
        {
            if (!Configurations.Any(x => x.Name == name))
            {
                Program.Logger.Error("Unknown config \"" + name + "\"");
                return false;
            }
            return true;
        }

        private bool Stop(string name)
        {
            if (!ConfigExists(name))
                return false;

            if (!ActiveInstances.Any(x=>x.Config.Name == name))
            {
                Program.Logger.Error("Config \"" + name + "\" not running");
                return false;
            }

            foreach (var inst in ActiveInstances.Where(x=>x.Config.Name == name))
            {
                Program.Logger.Info("Sending stop-messages to instance \"" + inst.ScreenSession + "\"");
                foreach (var msg in inst.Config.SendOnStop)
                    inst.Send(msg);
            }
            return true;
        }

        private bool Start(string name)
        {
            if (!ConfigExists(name))
                return false;

            var cfg = Configurations.First(x => x.Name == name);

            if (!cfg.AllowMultipleInstances && ActiveInstances.Any(x => x.Config.Name == name))
            {
                Program.Logger.Error("Config \"" + name + "\" already running");
                return false;
            }

            var inst = ScreenHandler.CreateScreen(cfg);
            Program.Logger.Info("Sending start-messages to instance \"" + inst.ScreenSession + "\"");
            foreach (var msg in inst.Config.SendOnStart)
                inst.Send(msg);

            return true;
        }

        public bool Restart(string name)
        {
            if (!ConfigExists(name))
                return false;

            if (!Stop(name))
                return false;
            
            var cfg = Configurations.First(x => x.Name == name);
            if (cfg.RestartDelay > 0)
            {
                Program.Logger.Info(string.Format("Delaying start after stop for {0} seconds...", cfg.RestartDelay));
                Thread.Sleep(cfg.RestartDelay * 1000);
            }
            return Start(name);
        }

        private void LoadConfigs()
        {
            Program.Logger.Info("Loading server configurations...");
            if (!File.Exists(ConfigFile))
                throw new FileNotFoundException("Failed to load configs", ConfigFile);

            using (var reader = new StreamReader(ConfigFile))
            {
                Program.Logger.Info("Parsing server configurations...");
                config = (ServerManagerConfiguration)new JsonSerializer().Deserialize(reader, typeof(ServerManagerConfiguration));
            }
        }

        private void CreateConfigs()
        {
            Program.Logger.Info("Creating sample configuration files...");

            var cfg = new ServerConfiguration();
            cfg.Name = "NanoDemo";
            cfg.RestartDelay = 1;
            cfg.FilePath = "nano";
            cfg.WorkingDirectory = "/home/" + Environment.UserName;
            cfg.Arguments = "demo.txt";
            cfg.SendOnStart = new string[]
            {
                "This ",
                "is ",
                "a ",
                "demo!^M"
            };
            cfg.SendOnStop = new string[]
            {
                "^X",
                "j",
                "^M"
            };

            var cfgs = new ServerConfiguration[] { cfg };
            var smcfg = new ServerManagerConfiguration();
            smcfg.ServerConfigurations = cfgs;
            using (var writer = new StreamWriter(ConfigFile))
                new JsonSerializer() { Formatting = Formatting.Indented }.Serialize(writer, smcfg);

            config = smcfg;
        }
    }
}