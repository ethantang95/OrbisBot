using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using System.Threading;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class RestartContextTask : CommandChannelTaskAbstract
    {
        public RestartContextTask(CommandChannelTaskPermission permission) : base(permission)
        {

        }
        public override string AboutText()
        {
            return "Recreate the context object... not even sure if this works";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string CommandText()
        {
            return "bot-restart";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            Context.Instance.SignalRestart();

            DiscordMethods.SetGame("Bot Restarting...");

            Thread.Sleep(5000);

            Context.Instance.DestorySelf();
            Thread.Sleep(2000);

            //for some reason, it exceptons out when the result is returned
            return String.Empty;
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}
