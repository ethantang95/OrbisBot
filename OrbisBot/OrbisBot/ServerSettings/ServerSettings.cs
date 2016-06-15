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
        public bool EnableGoodbyeMsgs { get; set; }
        public bool EnableGoodbyePms { get; set; }
        public string WelcomeMsg { get; set; }
        public string GoodbyeMsg { get; set; }
        public string GoodbyePms { get; set; }

        public ServerSetting()
        {

        }
        public ServerSetting(ulong serverID, bool enableWelcome, bool enableGoodBye, bool enableGoodByePms, string welcomeMsg, string goodbyeMsg, string goodbyePms)
        {
            ServerId = serverID;
            EnableWelcome = enableWelcome;
            EnableGoodbyeMsgs = enableGoodBye;
            WelcomeMsg = welcomeMsg;
            GoodbyeMsg = goodbyeMsg;
            GoodbyePms = goodbyePms;
        }
    }
}
