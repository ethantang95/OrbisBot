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
    class ServerGoodByeMsgSettingsTask : TaskAbstract
    {
        public ServerGoodByeMsgSettingsTask(DiscreteTaskPermission permission) : base(permission)
        {

        }

        public override string AboutText()
        {
            return "Enables or disables the goodbye message for the server and sets the goodbye message";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                return false;
            }

            if (args[1].Equals("enable", StringComparison.InvariantCultureIgnoreCase) || args[1].Equals("disable", StringComparison.InvariantCultureIgnoreCase))
            {
                return args.Length == 2;
            }
            else if (args[1].Equals("message", StringComparison.InvariantCultureIgnoreCase))
            {
                return args.Length <= 3;
            }
            else
            {
                return false;
            }
        }

        public override string CommandText()
        {
            return "server-goodbye";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args[1].Equals("enable", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Context.Instance.ServerSettings.GetGoodbyeMessage(messageSource.Server.Id) == string.Empty)
                {
                    return "The server does not have a goodbye message yet, please create a goodbye message";
                }
                else
                {
                    Context.Instance.ServerSettings.SetGoodbyeEnable(messageSource.Server.Id, true);
                    return "Server goodbye message is now enabled";
                }
            }
            else if (args[1].Equals("disable", StringComparison.InvariantCultureIgnoreCase))
            {
                Context.Instance.ServerSettings.SetGoodbyeEnable(messageSource.Server.Id, false);
                return "Server goodbye message is now disabled";
            }
            else //it is a message
            {
                if (args.Length == 3)
                {
                    //we actually want to re parse the raw message
                    var rawArgs = CommandParser.ParseCommand(messageSource.Message.RawText);
                    Context.Instance.ServerSettings.SetGoodByeMessage(messageSource.Server.Id, rawArgs[2]);
                    return "Server goodbye message is set, please enable server goodbye messages for it to work";
                }
                else
                {
                    return $"The current goodbye message is: {Context.Instance.ServerSettings.GetGoodbyeMessage(messageSource.Server.Id)}";
                }
            }
        }

        public override string UsageText()
        {
            return "<enable|disable|message> OPTIONAL(\"goodbye message\")";
        }
    }
}
