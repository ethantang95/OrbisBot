using OrbisBot.TaskAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.TaskPermissions;
using Discord;

namespace OrbisBot.Tasks.ServerTasks
{
    class ServerSetTriggerCharTask : TaskAbstract
    {
        private static string[] _validChars = new string[] { "!", "~", "?", ".", ",", "-", "_", "+", "=", "*", "&", "^", "%", "$", "`", ">", "<", "|" };

        public ServerSetTriggerCharTask(DiscreteTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return $"Sets the command triggering character for this server, the possible characters are {string.Join(" ", _validChars)}";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length != 2)
            {
                return false;
            }

            return _validChars.Contains(args[1]);
        }

        public override string CommandText()
        {
            return "server-trigger";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            Context.Instance.ServerSettings.SetTriggerChar(messageSource.Server.Id, args[1][0]);

            return $"This server's command triggering character is now {args[1]}";
        }

        public override string UsageText()
        {
            return $"(character); possible characters are {string.Join(" ", _validChars)}";
        }
    }
}
