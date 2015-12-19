using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    //really a task to make the other ones easier... mostly to register admins
    class RegisterSelfTask : TaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (messageSource.User.Id == 91325420921683968)  //Cyg
            {
                Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, messageSource.User.Id, PermissionLevel.Developer);
                return $"Welcome {messageSource.User.Name}, you are now registered as a developer in this channel";
            }
            //we don't need channel registeration because channel permission does it when we register
            if (messageSource.Server.Owner.Id == messageSource.User.Id)
            {
                Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, messageSource.User.Id, PermissionLevel.Owner);
                return "You were detected as the owner of this channel, you have been granted the permission level of owner";
            }

            if (Context.Instance.ChannelPermission.ContainsChannel(messageSource.Channel.Id) &&
                Context.Instance.ChannelPermission.IsUserInChannel(messageSource.Channel.Id, messageSource.User.Id))
            {
                return "You are already registered with this channel";
            }

            Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, messageSource.User.Id, PermissionLevel.User);

            Context.Instance.ChannelPermission.ChannelPermissions[messageSource.Channel.Id].toFileOutput();
            return "Thank you for registering with this channel";
        }

        public override string PermissionFileSource()
        {
            return Constants.REGISTER_SELF_FILE;
        }

        public override CommandPermission DefaultCommands()
        {
            return new CommandPermission(false, PermissionLevel.RestrictedUser, true);
        }

        public override string CommandText()
        {
            return "!Register";
        }

        public override string AboutText()
        {
            return "Registers yourself and the server to help with permissions and roles";
        }
    }
}
