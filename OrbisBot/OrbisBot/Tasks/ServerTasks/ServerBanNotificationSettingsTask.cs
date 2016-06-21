using Discord;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Tasks.ServerTasks
{
    class ServerBanNotificationSettingsTask : TaskAbstract
    {
        public ServerBanNotificationSettingsTask(DiscreteTaskPermission permission) : base(permission)
        {

        }

        public override string AboutText()
        {
            return "Enables or disables the notification when a user gets banned or unbanned";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2 && (args[1].Equals("enable", StringComparison.InvariantCultureIgnoreCase) || args[1].Equals("disable", StringComparison.InvariantCultureIgnoreCase));
        }

        public override string CommandText()
        {
            return "server-ban-notif";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args[1].Equals("enable", StringComparison.InvariantCultureIgnoreCase))
            { 
                Context.Instance.ServerSettings.SetBanNotifEnable(messageSource.Server.Id, true);
                return "Server ban notification is now enabled";
            }
            else
            {
                Context.Instance.ServerSettings.SetBanNotifEnable(messageSource.Server.Id, false);
                return "Server ban notification is now disabled";
            }
        }

        public override string UsageText()
        {
            return "<enable|disable>";
        }
    }
}
