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
    class BotHelpTask : TaskAbstract
    {
        public BotHelpTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Get help information for this bot";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string CommandText()
        {
            return "help";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var returnText = new StringBuilder();
            returnText.AppendLine("If you would like to get help for this bot, please visit https://discord.gg/gYaXtX2");
            returnText.AppendLine("Additionally, if you would like to see its source code, it is available at https://github.com/ethantang95/OrbisBot");
            returnText.AppendLine("You may also find the developer Cygnus - Zero2G in various bot related channels and pm me for extra help");

            return returnText.ToString();
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}
