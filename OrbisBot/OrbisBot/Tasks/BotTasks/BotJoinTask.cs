using OrbisBot.TaskAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.TaskPermissions;
using Discord;

namespace OrbisBot.Tasks.BotTasks
{
    class BotJoinTask : TaskAbstract
    {
        public BotJoinTask(DiscreteTaskPermission permission) : base(permission)
        {

        }

        public override string AboutText()
        {
            return "Allows the bot to join another server given an invite";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 1;
        }

        public override string CommandText()
        {
            return "server-join";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            return "https://discordapp.com/oauth2/authorize?client_id=176176252963520512&scope=bot&permissions=0 \n please click on this link for the bot to join your server";
        }

        public override string UsageText()
        {
            return "<invite url>";
        }
    }
}
