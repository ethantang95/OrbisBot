using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskHelpers.UserFinder;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class UserInfoTask : TaskAbstract
    {
        public UserInfoTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var mainChannel = messageSource.Server.TextChannels.FirstOrDefault(s => s.Id == Context.Instance.ChannelPermission.GetMainChannelForServer(messageSource.Server.Id));

            var returnText = new StringBuilder();

            User targetUser;

            if (args.Length == 2)
            {
                targetUser = UserFinderUtil.FindUser(messageSource.Server.Users, args[1], Context.Instance.GlobalSetting.HideList);
                if (targetUser == null)
                {
                    return "The user you are trying to find does not exist in this server";
                }
            }
            else
            {
                targetUser = messageSource.User;
            }

            var targetUserRole = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id, targetUser.Id);

            if (Context.Instance.ChannelPermission.IsDeveloper(messageSource.Channel.Id, messageSource.User.Id))
            {
                returnText.AppendLine($"Channel ID: {messageSource.Channel.Id}")
                .AppendLine($"Main Channel: {mainChannel?.Name}")
                .AppendLine($"Server ID: {messageSource.Server.Id}")
                .AppendLine($"Server Owner: {messageSource.Server.Owner.Name}")
                .AppendLine($"User ID: {targetUser.Id}");
            }

            returnText.AppendLine($"User Name: {targetUser.Name}")
                .AppendLine(
                    $"User role for bot: {targetUserRole}")
                .AppendLine($"User Avatar: {targetUser.AvatarUrl}");

            return returnText.ToString();
        }

        public override string CommandText()
        {
            return "user-info";
        }

        public override string AboutText()
        {
            return "Shows information about the user, if user is not specified, it will display information about yourself";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length <= 2;
        }

        public override string UsageText()
        {
            return "OPTIONAL(name)";
        }
    }
}
