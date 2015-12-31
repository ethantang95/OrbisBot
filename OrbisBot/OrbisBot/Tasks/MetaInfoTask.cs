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
    class MetaInfoTask : FilePermissionTaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length > 2)
            {
                return $"{Constants.SYNTAX_INTRO} OPTIONAL \"<user's name>\"";
            }

            var mainChannel = messageSource.Server.Channels.FirstOrDefault(s => s.Id == Context.Instance.ChannelPermission.GetMainChannelForServer(messageSource.Server.Id));

            var returnText = new StringBuilder().AppendLine($"Channel ID: {messageSource.Channel.Id}")
                .AppendLine($"Main Channel: {mainChannel?.Name}")
                .AppendLine($"Server ID: {messageSource.Server.Id}")
                .AppendLine($"Server Owner: {messageSource.Server.Owner.Name}");

            User targetUser;

            if (args.Length == 2)
            {
                targetUser = messageSource.Server.Members.FirstOrDefault(s => s.Name == args[1]);
                if (targetUser == null)
                {
                    return "The user you are trying to find does not exist in this server";
                }
            }
            else
            {
                targetUser = messageSource.User;
            }

            var targetUserRole = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id,
                targetUser.Id);

            returnText.AppendLine($"User ID: {targetUser.Id}")
                .AppendLine(
                    $"User Role In Current Channel: {targetUserRole}")
                .AppendLine($"User Avatar: {Constants.DISCORD_API_ENDPOINT}{targetUser.AvatarUrl}");

            return returnText.ToString();
        }

        public override string PermissionFileSource()
        {
            return Constants.META_INFO_FILE;
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, true);
        }

        public override string CommandText()
        {
            return "userinfo";
        }

        public override string AboutText()
        {
            return "Shows information about the channel, server, and user, if user is not specified, it will display information about yourself";
        }
    }
}
