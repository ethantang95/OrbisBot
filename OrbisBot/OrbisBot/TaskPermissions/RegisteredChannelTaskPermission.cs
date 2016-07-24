using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskPermissions.Interfaces;

namespace OrbisBot.TaskPermissions
{
    class RegisteredChannelTaskPermission<T> : TaskPermissionAbstract where T : ICommandPermissionForm
    {
        IPermissionSaver _saver;
        ICollection<T> _forms;
        public RegisteredChannelTaskPermission(CommandPermission permission, ICollection<T> permissionForms, IPermissionSaver saver) : base(permission)
        {
            _forms = permissionForms;
            _saver = saver;
        }
        public override bool AllowTaskExecution(MessageEventArgs eventArgs)
        {
            //here, we check for permissions, first, check the server permissions
            //default permission is always user
            var userPermission = GetUserPermission(eventArgs);

            //get the command permission now
            var commandPermission = GetCommandPermissionForChannel(eventArgs.Channel.Id);

            if (commandPermission > userPermission || userPermission == PermissionLevel.Restricted)
            {
                return false; //the user does not have the rights to perform this task
            }

            return true;
        }

        public override PermissionLevel GetCommandPermissionForChannel(ulong channelId)
        {
            if (CommandPermission.ChannelPermission.ContainsKey(channelId))
            {
                return CommandPermission.ChannelPermission[channelId].PermissionLevel;
            }
            return PermissionLevel.UsageDenied;
        }

        public override void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel)
        {
            if (!CommandPermission.ChannelPermission.ContainsKey(channelId))
            {
                throw new UnauthorizedAccessException("You cannot change the permission for this command as your channel do not have access to this command");
            }
            CommandPermission.ChannelPermission[channelId].PermissionLevel = newPermissionLevel;

            //update the forms
            _forms.First(s => s.Channel == channelId).PermissionLevel = newPermissionLevel;
            _saver.SaveSettings(_forms);
        }

        public override void SetCoolDownForChannel(ulong channelId, int cooldown)
        {
            if (!CommandPermission.ChannelPermission.ContainsKey(channelId))
            {
                throw new UnauthorizedAccessException("You cannot change the permission for this command as your channel do not have access to this command");
            }
            CommandPermission.ChannelPermission[channelId].CoolDown = cooldown;

            _forms.First(s => s.Channel == channelId).CoolDown = cooldown;
            _saver.SaveSettings(_forms);
        }

        private PermissionLevel GetUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }

        public override void AddPermission(ICommandPermissionForm permission)
        {
            _forms.Add((T)permission);
            _saver.SaveSettings(_forms);
            base.AddPermission(permission);
        }

        public override void RemovePermission(ulong channelId)
        {
            var form = _forms.First(s => s.Channel == channelId);
            _forms.Remove(form);
            _saver.SaveSettings(_forms);
            base.RemovePermission(channelId);
        }

        public override void UpdatePermission(ICommandPermissionForm permission)
        {
            RemovePermission(permission.Channel);
            AddPermission(permission);
        }
    }
}
