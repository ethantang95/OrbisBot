﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class AdjustUserPermissionTask : FilePermissionTaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //remove the @ sign
            var name = args[1].Substring(1);

            if (!messageSource.Server.Users.Any(s => s.Name == name))
            {
                return "The user you have tried to change permission for does not exist, did you forget the @?";
            }

            PermissionLevel targetNewPermissionLevel;
            try
            {
                targetNewPermissionLevel = EnumParser.ParseString(args[2], true, PermissionLevel.User);
            }
            catch (Exception e)
            {
                return "The rank you have input is not recognized, the ranks are: Admin, Moderator, User, RestrictedUser";
            }

            //first, get the permissions of the user setting the permission, first, check if the current user has permissions
            if (!Context.Instance.ChannelPermission.IsUserInChannel(messageSource.Channel.Id, messageSource.User.Id))
            {
                return "You currently are not registered and have a role in this server, please try !Register to register";
            }

            var userPermission = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id,
                messageSource.User.Id);

            var targetUser = messageSource.Server.Users.First(s => s.Name == name);
            var targetPermission = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id,
                targetUser.Id);

            //if the target user's permission is higher or same, do not change
            if (userPermission <= targetPermission)
            {
                return "You do not have the power to change the role of this user";
            }

            //check if he is trying to set the role above his pay grade

            if (targetNewPermissionLevel >= userPermission)
            {
                return "You do not have the power to change the user to this role";
            }

            Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, targetUser.Id, targetNewPermissionLevel);

            return "New Role successfully set";
        }

        public override string PermissionFileSource()
        {
            return Constants.ADJUST_PERMISSION_FILE;
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Moderator, true, 1);
        }

        public override string CommandText()
        {
            return "user-permission";
        }

        public override string AboutText()
        {
            return "Changes a user's permission or role on the current channel";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return "An error has occurred when trying to change the person's permission, the developers has been notified of this problem";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 3;
        }

        public override string UsageText()
        {
            return $"\"@(user's name)\" (rank) \nThe ranks are Admin, Moderator, User, RestrictedUser";
        }
    }
}
