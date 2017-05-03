using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControl.Management.Screen
{
    public class ExecException : Exception
    {
        public ExecResult Result { get; private set; }
        
        public ExecException(string message, ExecResult res) : base(message)
        {
            Result = res;
        }

        public override string Message
        {
            get
            {
                return string.Format("{0}\nResult: {1}", base.Message, Result);
            }
        }
    }
}
