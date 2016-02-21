﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using System.Configuration;
using OrbisBot.TaskHelpers.AdminUtils;

namespace OrbisBot.TaskAbstracts
{
    abstract class CommandChannelTaskAbstract: TaskAbstract
    {
        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Developer, true, 1);
        }

        public override PermissionLevel GetCommandPermissionForChannel(long channelId)
        {
            return PermissionLevel.Developer;
        }

        public override bool AllowTaskExecution(MessageEventArgs eventArgs)
        {
            return GeneralAdminUtils.IsCommandChannel(eventArgs.Channel.Id) && Context.Instance.ChannelPermission.IsDeveloper(eventArgs.Channel.Id, eventArgs.User.Id);
        }

        public override void SetCommandPermissionForChannel(long channelId, PermissionLevel newPermissionLevel)
        {
            throw new UnauthorizedAccessException("This should not be called for command tasks");
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return $"Message crashed with the exception {ex.ToString()}";
        }

        public override void SetCoolDownForChannel(long channelId, int cooldown)
        {
            throw new UnauthorizedAccessException("This should not be called for command tasks");
        }
    }
}
