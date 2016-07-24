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
    class BotMentionTask : TaskAbstract
    {
        public BotMentionTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Short intro to the bot when it is mentioned";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string CommandText()
        {
            return "bot-mention";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var triggerChar = Context.Instance.ServerSettings.GetTriggerChar(messageSource.Server.Id);
            return $"Hello {messageSource.User.Name}, I am OrbisBot, type {triggerChar}About to learn more about me. To activate this bot in this server, the character is {triggerChar}";
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}
