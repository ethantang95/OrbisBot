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

                if (permission == null)
                {
                    FileHelper.WriteObjectToFile(PermissionFileSource(), DefaultCommandPermission());
                    _commandPermission = DefaultCommandPermission();
                }
                else
                {
                    _commandPermission = permission;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem parsing the settings file, creating default");
                FileHelper.WriteObjectToFile(PermissionFileSource(), DefaultCommandPermission());
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

        public override PermissionLevel GetCommandPermissionForChannel(ulong channelId)
        {
            if (_commandPermission.ChannelPermission.ContainsKey(channelId))
            {
                return _commandPermission.ChannelPermission[channelId].PermissionLevel;
            }
            return _commandPermission.DefaultLevel;
        }

        public override void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel)
        {
            //first check if such permissoin.
            if (_commandPermission.ChannelPermission.ContainsKey(channelId))
            {
                _commandPermission.ChannelPermission[channelId].PermissionLevel = newPermissionLevel;
            }
            else
            {
                _commandPermission.ChannelPermission.Add(channelId, new ChannelPermissionSetting(newPermissionLevel, _commandPermission.DefaultCoolDown));
            }
            FileHelper.WriteObjectToFile(PermissionFileSource(), _commandPermission);
        }

        public override void SetCoolDownForChannel(ulong channelId, int seconds)
        {
            if (_commandPermission.ChannelPermission.ContainsKey(channelId))
            {
                _commandPermission.ChannelPermission[channelId].CoolDown = seconds;
            }
            else
            {
                _commandPermission.ChannelPermission.Add(channelId, new ChannelPermissionSetting(DefaultCommandPermission().DefaultLevel, seconds));
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
