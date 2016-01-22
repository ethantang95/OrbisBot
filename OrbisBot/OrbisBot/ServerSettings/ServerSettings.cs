using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.ServerSettings
{
    public class ServerSetting
    {
        public long ServerId { get; private set; }
        public bool EnableWelcome { get; set; }
        public string WelcomeMsg { get; set; }

        public ServerSetting(long serverID, bool enableWelcome, string welcomeMsg)
        {
            this.ServerId = serverID;
            this.EnableWelcome = enableWelcome;
            this.WelcomeMsg = welcomeMsg;
        }

        public Dictionary<string, string> toFileOutput()
        {
            var toReturn = new Dictionary<string, string>();
            toReturn.Add(Constants.SERVER_ID, ServerId.ToString());
            toReturn.Add(Constants.ENABLE_WELCOME, EnableWelcome.ToString());
            toReturn.Add(Constants.WELCOME_MSG, WelcomeMsg);

            return toReturn;   
        }
    }
}
