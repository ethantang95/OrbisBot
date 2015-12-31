using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class ChangeMainChannelTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Set the current channel as the main channel for this server, all general messages by OrbisBot will be sent to this channel";
        }

        public override string CommandText()
        {
            return "setmainchannel";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Admin, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.CHANGE_MAIN_CHANNEL_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            Context.Instance.ChannelPermission.SetMainChannelForServer(messageSource.Server.Id, messageSource.Channel.Id);

            return $"Successfully changed the main channel to {messageSource.Channel.Name}";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return "An error has occurred when trying to change the main channel, the developers has been notified of this problem";
        }
    }
}
