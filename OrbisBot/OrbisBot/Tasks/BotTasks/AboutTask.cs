using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class AboutTask : TaskAbstract
    {
        public AboutTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //we do not need any args... really... it will just process itself
            var message = new StringBuilder().AppendLine("Hello, my name is OrbisBot. I am a Discord Bot created by Cygnus/Zero2G")
                .AppendLine("I can do many different things, including being your mini personal assistance.")
                .AppendLine("I am optimized for the game DN but can also handle tasks for other games and general purposes.")
                .Append("Currently I am on version ").AppendLine(Constants.APP_VERSION)
                .AppendLine("All my commands are activated with a dash, like \"-About\".")
                .AppendLine("Type \"-Commands\" to see what I can do")
                .AppendLine("You should also register yourself to me by typing \"-Register\".")
                .AppendLine("If you have any questions, please pm me and I'll send your message to the developers")
                .AppendLine("If you would like to see the source code, it is available at https://github.com/ethantang95/OrbisBot");

            return message.ToString();
        }

        public override string CommandText()
        {
            return "about";
        }

        public override string AboutText()
        {
            return "Shows information about OrbisBot";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }

        public override IEnumerable<string> AdditionalCommandTexts()
        {
            return new List<string> { "info" };
        }
    }
}
