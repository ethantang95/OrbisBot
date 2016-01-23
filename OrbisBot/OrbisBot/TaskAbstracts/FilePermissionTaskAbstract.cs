using Discord;
using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskAbstracts
{
    abstract class FilePermissionTaskAbstract : TaskAbstract
    {
        public FilePermissionTaskAbstract()
        {
            PopulatePermissions();
        }

        private void PopulatePermissions()
        {
            try
            {
                var permission = FileHelper.GetObjectFromFile<CommandPermission>(PermissionFileSource());

                _commandPermission = permission;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem parsing the settings file, creating default");
                FileHelper.WriteObjectToFile(PermissionFileSource(), _commandPermission);
            }
        }

        public override bool AllowTaskExecution(MessageEventArgs messageEventArgs)
        {
            //here, we check for permissions, first, check the server permissions
            //default permission is always user
            var userPermission = GetUserPermission(messageEventArgs);

            //get the command permission now
            var commandPermission = GetCommandPermissionForChannel(messageEventArgs.Channel.Id);

            if (commandPermission > userPermission || userPermission == PermissionLevel.Restricted)
            {
                return false; //the user does not have the rights to perform this task
            }

            return true;
        }

        public override PermissionLevel GetCommandPermissionForChannel(long channelId)
        {
            if (_commandPermission.ChannelPermissionLevel.ContainsKey(channelId))
            {
                return _commandPermission.ChannelPermissionLevel[channelId].PermissionLevel;
            }
            return _commandPermission.DefaultLevel;
        }

        public override void SetCommandPermissionForChannel(long channelId, PermissionLevel newPermissionLevel)
        {
            //first check if such permissoin.
            if (_commandPermission.ChannelPermissionLevel.ContainsKey(channelId))
            {
                _commandPermission.ChannelPermissionLevel[channelId].PermissionLevel = newPermissionLevel;
            }
            else
            {
                _commandPermission.ChannelPermissionLevel.Add(channelId, new ChannelPermissionSetting(newPermissionLevel, _commandPermission.DefaultCoolDown));
            }
            FileHelper.WriteObjectToFile(PermissionFileSource(), _commandPermission);
        }

        private PermissionLevel GetUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return String.Empty;
        }

        public abstract string PermissionFileSource();
    }
}
