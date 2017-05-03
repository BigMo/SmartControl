using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SmartControl.Management.Screen
{
    public static class ExecHandler
    {
        public static ExecResult Execute(string filename, string args)
        {
            return Execute(filename, args, Process.GetCurrentProcess().StartInfo.WorkingDirectory);
        }

        public static ExecResult Execute(string filename, string args, string workingDir)
        {
            Process p = new Process();

            List<string> output = new List<string>();
            List<string> error = new List<string>();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = filename;
            psi.WorkingDirectory = workingDir;
            psi.Arguments = args;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            p.OutputDataReceived += (o, e) => { if (e.Data != null) output.Add(e.Data); };
            p.ErrorDataReceived += (o, e) => { if (e.Data != null) error.Add(e.Data); };

            p.StartInfo = psi;

            Program.Logger.Debug(string.Format("Executing [wd:\"{0}\"] \"{1} {2}\"...", psi.WorkingDirectory, psi.FileName, psi.Arguments));

            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();

            string[] outputArray = output.Any(x => !string.IsNullOrEmpty(x.Trim())) ? output.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim()).ToArray() : new string[0];
            string[] errorArray = error.Any(x => !string.IsNullOrEmpty(x.Trim())) ? error.Where(x => !string.IsNullOrEmpty(x.Trim())).Select(x => x.Trim()).ToArray() : new string[0];

            return new ExecResult(p.ExitCode, string.Format("{0} {1}", psi.FileName, psi.Arguments), outputArray, errorArray);
        }
    }
}
