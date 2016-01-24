using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using System.Configuration;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    //really a task to make the other ones easier... mostly to register admins
    class RegisterTask : FilePermissionTaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var developers = ConfigurationManager.AppSettings[Constants.DEVELOPERS].Split(',').Select(s => s.Trim());
            if (developers.Contains(messageSource.User.Id.ToString())) 
            {
                Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, messageSource.User.Id, PermissionLevel.Developer);
                return $"Welcome {messageSource.User.Name}, you are now registered as a developer in this channel";
            }
            //we don't need channel registeration because channel permission does it when we register
            if (messageSource.Server.Owner.Id == messageSource.User.Id)
            {
                Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, messageSource.User.Id, PermissionLevel.Owner);
                return "You were detected as the owner of this channel, you have been granted the permission level of owner. Try the command -user-autopermission to automatically assign members to their proper role for this bot based on their server permissions";
            }

            if (Context.Instance.ChannelPermission.ContainsChannel(messageSource.Channel.Id) &&
                Context.Instance.ChannelPermission.IsUserInChannel(messageSource.Channel.Id, messageSource.User.Id))
            {
                return "You are already registered with this channel";
            }

            Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, messageSource.User.Id, PermissionLevel.User);

            return "Thank you for registering with this channel";
        }

        public override string PermissionFileSource()
        {
            return Constants.REGISTER_SELF_FILE;
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.RestrictedUser, true, 1);
        }

        public override string CommandText()
        {
            return "register";
        }

        public override string AboutText()
        {
            return "Registers yourself and the server to help with permissions and roles";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}
