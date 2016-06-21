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
    class BotSetupTask : MultiInputTaskAbstract
    {
        public BotSetupTask(DiscreteTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Setting up the bot for the server";
        }

        public override bool CheckArgs(string[] args)
        {
            return true; //no need for any args really
        }

        public override string CommandText()
        {
            return "setup";
        }

        public override bool StateCheckArgs(int state, string[] args)
        {
            throw new NotImplementedException();
        }

        public override int StateTaskComponent(int state, string[] args, MessageEventArgs messageSource)
        {
            throw new NotImplementedException();
        }

        public override string StateUsageText(int state)
        {
            throw new NotImplementedException();
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var message = new StringBuilder();
            message.Append("Welcome to Orbisbot set up, ");
            message.AppendLine("This is to help you to set up OrbisBot so you may start using it to the best of its abilities. ");
            message.AppendLine("Let's start by handling the permission of the members of the server. ");
            message.AppendLine("Input anything to see what OrbisBot has determined which role should each user get for the bot");

            return message.ToString();
        }

        public override string UsageText()
        {
            throw new NotImplementedException();
        }
    }
}
