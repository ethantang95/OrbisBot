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
        public bool EnableBanNotificaftion { get; set; }
        public string WelcomeMsg { get; set; }
        public string GoodbyeMsg { get; set; }
        public string GoodbyePms { get; set; }
        public char TriggerChar { get; set; }

        public ServerSetting()
        {
            TriggerChar = '-';
        }
        public ServerSetting(ulong serverID, bool enableWelcome = false, bool enableGoodBye = false, bool enableGoodByePms = false, bool enableBanNotification = false, string welcomeMsg = "", string goodbyeMsg = "", string goodbyePms = "", char triggerChar = '-')
        {
            ServerId = serverID;
            EnableWelcome = enableWelcome;
            EnableGoodbyeMsgs = enableGoodBye;
            EnableBanNotificaftion = enableBanNotification;
            WelcomeMsg = welcomeMsg;
            GoodbyeMsg = goodbyeMsg;
            GoodbyePms = goodbyePms;
            TriggerChar = triggerChar;
        }
    }
}
