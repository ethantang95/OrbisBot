using Discord;
using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrbisBot.Tasks
{
    abstract class TaskAbstract
    {
        private string _taskResult;
        private string[] _args;
        private MessageEventArgs _messageSource;
        protected CommandPermission _commandPermission;

        public TaskAbstract()
        {
            //we will populate the command permission first
            PopulatePermissions();
        }

        public void RunTask(string[] args, MessageEventArgs messageEventArgs)
        {
            //here, check if we will proceed based on the command and channel settings
            if (!proceedWithCommand(messageEventArgs))
            {
                return;
            }

            //here, we check for permissions, first, check the server permissions
            //default permission is always user
            var userPermission = getUserPermission(messageEventArgs);

            //get the command permission now
            var commandPermission = getCommandPermission(messageEventArgs);

            if (commandPermission > userPermission)
            {
                return; //the user does not have the rights to perform this task
            }

            _args = args;
            _messageSource = messageEventArgs;
            //do consider using a threadpool in the future to prevent request bombs
            Thread taskThread = new Thread(ExecuteTask);
            taskThread.Start();
        }

        private bool proceedWithCommand(MessageEventArgs messageEventArgs)
        {
            var proceed = true;
            if (Context.Instance.ChannelPermission.ContainsChannel(messageEventArgs.Channel.Id))
            {
                proceed &= !Context.Instance.ChannelPermission.ChannelPermissions[messageEventArgs.Channel.Id].Muted
                    || _commandPermission.OverrideMuting;
            }
            proceed &= !_commandPermission.Disabled;
            return proceed;
        }

        private PermissionLevel getUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }

        private PermissionLevel getCommandPermission(MessageEventArgs messageEventArgs)
        {
            var commandPermission = _commandPermission.DefaultLevel;

            if (_commandPermission.ChannelPermissionLevel.ContainsKey(messageEventArgs.Channel.Id))
            {
                commandPermission = _commandPermission.ChannelPermissionLevel[messageEventArgs.Channel.Id];
            }
            return commandPermission;
        }

        private void ExecuteTask()
        {
            _taskResult = TaskComponent(_args, _messageSource);
            PublishTask();
        }

        private void PublishTask()
        {
            var discordClient = Context.Instance.Client;
            discordClient.SendMessage(_messageSource.Channel, _taskResult);
        }

        private void PopulatePermissions()
        {

            var permissionListRaw = FileHelper.GetValuesFromFile(PermissionFileSource());

            try
            {
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
                _commandPermission = DefaultCommands();
                FileHelper.WriteValuesToFile(_commandPermission.toFileOutput(), PermissionFileSource());
            }
        }

        public void SetPermission(long channelID, PermissionLevel level)
        {
            //first check if such permissoin.
            if (_commandPermission.ChannelPermissionLevel.ContainsKey(channelID))
            {
                _commandPermission.ChannelPermissionLevel[channelID] = level;
            }
            else
            {
                _commandPermission.ChannelPermissionLevel.Add(channelID, level);
            }
            FileHelper.WriteValuesToFile(_commandPermission.toFileOutput(), PermissionFileSource());
        }

        public abstract string TaskComponent(string[] args, MessageEventArgs messageSource);

        public abstract string PermissionFileSource();

        public abstract CommandPermission DefaultCommands();

        public abstract string CommandText();
    }
}
