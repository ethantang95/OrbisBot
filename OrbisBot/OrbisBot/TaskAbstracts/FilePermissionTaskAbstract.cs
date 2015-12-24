using Discord;
using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Tasks
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
                var permissionListRaw = FileHelper.GetValuesFromFile(PermissionFileSource());

                _commandPermission = new CommandPermission(bool.Parse(permissionListRaw[Constants.COMMAND_DISABLED]),
                            PermissionEnumMethods.ParseString(permissionListRaw[Constants.COMMAND_DEFAULT]),
                            bool.Parse(permissionListRaw[Constants.COMMAND_OVERRIDE]));

                permissionListRaw.Remove(Constants.COMMAND_DISABLED);
                permissionListRaw.Remove(Constants.COMMAND_DEFAULT);
                permissionListRaw.Remove(Constants.COMMAND_OVERRIDE);

                var permissionList = permissionListRaw.ToDictionary(s => long.Parse(s.Key), s => PermissionEnumMethods.ParseString(s.Value));

                _commandPermission.ChannelPermissionLevel = permissionList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem parsing the settings file, creating default");
                FileHelper.WriteValuesToFile(_commandPermission.toFileOutput(), PermissionFileSource());
            }
        }

        public override bool AllowTaskExecution(MessageEventArgs messageEventArgs)
        {
            //here, we check for permissions, first, check the server permissions
            //default permission is always user
            var userPermission = GetUserPermission(messageEventArgs);

            //get the command permission now
            var commandPermission = GetCommandPermissionForChannel(messageEventArgs.Channel.Id);

            if (commandPermission > userPermission)
            {
                return false; //the user does not have the rights to perform this task
            }

            return true;
        }

        public override PermissionLevel GetCommandPermissionForChannel(long channelId)
        {
            if (_commandPermission.ChannelPermissionLevel.ContainsKey(channelId))
            {
                return _commandPermission.ChannelPermissionLevel[channelId];
            }
            return _commandPermission.DefaultLevel;
        }

        public override void SetCommandPermissionForChannel(long channelId, PermissionLevel level)
        {
            //first check if such permissoin.
            if (_commandPermission.ChannelPermissionLevel.ContainsKey(channelId))
            {
                _commandPermission.ChannelPermissionLevel[channelId] = level;
            }
            else
            {
                _commandPermission.ChannelPermissionLevel.Add(channelId, level);
            }
            FileHelper.WriteValuesToFile(_commandPermission.toFileOutput(), PermissionFileSource());
        }

        private PermissionLevel GetUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }

        public abstract string PermissionFileSource();
    }
}
