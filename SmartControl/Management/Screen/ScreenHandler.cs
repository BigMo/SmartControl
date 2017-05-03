using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SmartControl.Management.Screen
{
    public static class ScreenHandler
    {
        private static ExecResult RunScreen(string args)
        {
            return ExecHandler.Execute("/usr/bin/screen", args);
        }

        private static ExecResult RunScreen(string args, string workingDir)
        {
            return ExecHandler.Execute("/usr/bin/screen", args, workingDir);
        }

        public static string[] GetActiveScreens()
        {
            var resp = RunScreen("-list");

            if (resp.Output.Length == 1 && resp.Output[0].StartsWith("No Sockets found"))
                return new string[0];

            if (resp.ExitCode != 0)
                throw new ExecException("Failed to retrieve active screens", resp);

            if (resp.Output.Length == 0)
                throw new ExecException("Empty screen-output", resp);

            if (!resp.Output.Any(x => x.Count(c => c == '\t') >= 2))
                return new string[0];

            var lines = resp.Output.Where(x => x.Count(c => c == '\t') >= 2);
            var names = lines.Select(x => x.Split('\t')[0]);
            return names.ToArray();
        }

        private static ExecResult CreateScreen(string name, string command, string args, string workingDir)
        {
            return RunScreen(string.Format("-dmS {0} {1} {2}", name, command, args), workingDir);
        }

        public static bool GetScreenActiveShort(string name)
        {
            var resp = GetActiveScreens();
            return resp.Any(x => x.Split('.')[1] == name);
        }

        public static bool GetScreenActiveLong(string session)
        {
            var resp = GetActiveScreens();
            return resp.Any(x => x == session);
        }

        public static ScreenInstance CreateScreen(ServerConfiguration config)
        {
            if (GetScreenActiveShort(config.Name))
                throw new Exception("Screen \"" + config.Name + "\" already running!");

            var resp = CreateScreen(config.Name, config.FilePath, config.Arguments, config.WorkingDirectory);
            if (resp.ExitCode != 0)
                throw new ExecException("Failed to create screen", resp);

            var act = GetActiveScreens();
            return new ScreenInstance(config,
                act.First(x => x.Split('.')[1] == config.Name));
        }

        public static void SendToScreen(ScreenInstance inst, string message)
        {
            if (!GetScreenActiveLong(inst.ScreenSession))
                throw new Exception(string.Format("The instance \"{0}\" is not alive anymore", inst.ScreenSession));

            var resp = RunScreen(string.Format("-r {0} -X stuff \"{1}\"", inst.ScreenSession, message));
            if (resp.ExitCode != 0)
                throw new ExecException("Failed to send message", resp);
        }
    }
}
