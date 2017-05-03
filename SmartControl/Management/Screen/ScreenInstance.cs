using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartControl.Management.Screen
{
    public class ScreenInstance
    {
        public ServerConfiguration Config { get; private set; }
        public string ScreenSession { get; private set; }
        public bool IsAlive {
            get
            {
                return ScreenHandler.GetActiveScreens().Contains(ScreenSession);
            }
        }

        public ScreenInstance(ServerConfiguration config, string session)
        {
            Config = config;
            ScreenSession = session;
        }

        public void Send(string message)
        {
            ScreenHandler.SendToScreen(this, message);
        }
    }
}
