using log4net;
using log4net.Config;
using SmartControl.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmartControl
{
    class Program
    {
        private static string CONFIG_FILE = "SmartControlConfigs.json";
        public static ILog Logger { get; private set; } =  LogManager.GetLogger("SMRTCNTRL");
        public static ServerManager Manager { get; private set; }

        static void Main(string[] args)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            DOMConfigurator.Configure();
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            Program.Logger.Info("Logger initialized!");

            Program.Logger.Debug(string.Format("Args[{0}]: {1}", args.Length, args.Length > 0 ? string.Join(", ", args.Select(x => "\"" + x + "\"").ToArray()) : "<none>"));
            string name = "";
            Mode mode = Mode.Start;

            if (args.Length == 1 && args[0].StartsWith("-"))
            {
                switch (args[0])
                {
                    case "--help":
                    case "-h":
                        PrintUsage();
                        break;
                    case "--version":
                    case "-v":
                        var a = Assembly.GetExecutingAssembly();
                        Console.WriteLine("{0} v{1}\n", a.GetName().Name, a.GetName().Version);
                        break;
                    case "--list":
                    case "-l":
                        Manager = new ServerManager(CONFIG_FILE);
                        foreach (var cfg in Manager.Configurations)
                            Console.WriteLine("\t{0}", cfg);
                        break;
                    default:
                        break;
                }
                return;
            }
            if (args.Length != 2)
            {
                Program.Logger.Error("Expected 2 arguments");
                return;
            }
            else
            {
                name = args[1];
                if (!Enum.TryParse(args[0], out mode))
                {
                    Program.Logger.Error("Invalid mode \"" + args[0] + "\"");
                    return;
                }
            }

            Manager = new ServerManager(CONFIG_FILE);
            Manager.Update();
            Manager.Handle(name, mode);

            Program.Logger.Info("Exiting...");
        }

        private static void PrintUsage()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("Use: SmartControl MODE NAME");
            b.AppendLine(" or: SmartControl MODE all");
            b.AppendLine();
            b.AppendLine("Modes:");
            b.AppendLine("start\tStarts a configuration");
            b.AppendLine("stop\tStops a configuration");
            b.AppendLine("restart\tRestarts a configuration");
            b.AppendLine();
            b.AppendLine("NAME: The name of the configuration to handle (or \"all\" for all available configurations)");
            b.AppendLine();
            b.AppendLine("Example: SmartControl start NanoDemo");
            b.AppendLine("*DUH*");
            Console.WriteLine(b.ToString());
        }
    }
}
