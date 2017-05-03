using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControl.Management.Screen
{
    public class ExecResult
    {
        public string CommandLine { get; private set; }
        public int ExitCode { get; private set; }
        public string[] Output { get; private set; }
        public string[] Error { get; private set; }

        public ExecResult(int exitCode, string cmd, string[] output, string[] error)
        {
            ExitCode = exitCode;
            CommandLine = cmd;
            Output = output;
            Error = error;
        }

        public override string ToString()
        {
            return string.Format(
                "ExitCode: {0}\n" +
                "CommandLine: \"{1}\"\n" +
                "Output[{2}]:\n" +
                "\t{3}\n" +
                "Error[{4}]:\n" +
                "\t{5}",
                ExitCode,
                CommandLine,
                Output.Length,
                Output.Length > 0 ? string.Join("\n\t", Output.Select(x => "\"" + x + "\"").ToArray()) : "<none>",
                Error.Length,
                Error.Length > 0 ? string.Join("\n\t", Error.Select(x => "\"" + x + "\"").ToArray()) : "<none>");
        }
    }
}
