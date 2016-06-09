using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class ProxyPMTask : CommandChannelTaskAbstract
    {
        public ProxyPMTask(CommandChannelTaskPermission permission) : base(permission)
        {

        }
        public override string AboutText()
        {
            return "sends a private message to user with their ID from OrbisBot";
        }

        public override bool CheckArgs(string[] args)
        {
            long temp = 0;
            return (args.Length == 3 && Int64.TryParse(args[1], out temp));
        }

        public override string CommandText()
        {
            return "pm";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var userID = UInt64.Parse(args[1]);

            var result = DiscordMethods.SendPrivateMessage(userID, args[2]);

            return result.Result ? "message successfully sent" : "User is not found";
        }

        public override string UsageText()
        {
            return "<user ID> \"<message>\"";
        }
    }
}
