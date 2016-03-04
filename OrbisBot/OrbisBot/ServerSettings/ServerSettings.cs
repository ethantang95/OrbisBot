using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.ServerSettings
{
    public class ServerSetting
    {
        public ulong ServerId { get; private set; }
        public bool EnableWelcome { get; set; }
        public string WelcomeMsg { get; set; }

        public ServerSetting(ulong serverID, bool enableWelcome, string welcomeMsg)
        {
            this.ServerId = serverID;
            this.EnableWelcome = enableWelcome;
            this.WelcomeMsg = welcomeMsg;
        }
    }
}
