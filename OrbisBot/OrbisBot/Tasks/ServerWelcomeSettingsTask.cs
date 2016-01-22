using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class ServerWelcomeSettingsTask : DiscretePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Enables or disables the welcome message for the server and sets the welcome message";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                return false;
            }

            if (args[1].Equals("enable", StringComparison.InvariantCultureIgnoreCase) || args[1].Equals("disable", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length != 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if (args[1].Equals("message", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string CommandText()
        {
            return "server-welcome";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Admin, true);
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args[1].Equals("enable", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Context.Instance.ServerSettings.GetWelcomeMessage(messageSource.Server.Id) == string.Empty)
                {
                    return "The server does not have a welcome message yet, please creates a welcome message";
                }
                else
                {
                    Context.Instance.ServerSettings.SetWelcomeEnable(messageSource.Server.Id, true);
                    return "Server welcome message is now enabled";
                }
            }
            else if (args[1].Equals("disable", StringComparison.InvariantCultureIgnoreCase))
            {
                Context.Instance.ServerSettings.SetWelcomeEnable(messageSource.Server.Id, true);
                return "Server welcome message is now disabled";
            }
            else //it is a message
            {
                if (args.Length == 3)
                {
                    Context.Instance.ServerSettings.SetWelcomeMessage(messageSource.Server.Id, args[2]);
                    return "Server welcome message is set, please enable server welcome messages for it to work";
                }
                else
                {
                    return $"The current welcome message is: {Context.Instance.ServerSettings.GetWelcomeMessage(messageSource.Server.Id)}";
                }
            }
           

            return "Server welcome message is now enabled";
        }

        public override string UsageText()
        {
            return "<enable|disable|message> OPTIONAL(\"welcome message\") \nThe message can use the tokens %s that is available from custom commands";
        }
    }
}
