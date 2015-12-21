using Discord;
using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public bool IsCommandDisabled()
        {
            return _commandPermission.Disabled;
        }

        public void RunTask(string[] args, MessageEventArgs messageEventArgs)
        {
            //here, check if we will proceed based on the command and channel settings
            if (!ProceedWithCommand(messageEventArgs))
            {
                return;
            }

            //here, we check for permissions, first, check the server permissions
            //default permission is always user
            var userPermission = GetUserPermission(messageEventArgs);

            //get the command permission now
            var commandPermission = GetCommandPermission(messageEventArgs.Channel.Id);

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

        private bool ProceedWithCommand(MessageEventArgs messageEventArgs)
        {
            var proceed = true;
            if (Context.Instance.ChannelPermission.ContainsChannel(messageEventArgs.Channel.Id))
            {
                proceed &= !Context.Instance.ChannelPermission.ChannelPermissions[messageEventArgs.Channel.Id].Muted
                    || (_commandPermission.OverrideMuting 
                        && Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id, messageEventArgs.User.Id) >= PermissionLevel.Admin); //if a channel is muted, only an admin can proceed with override mute commands
            }
            proceed &= !_commandPermission.Disabled;
            return proceed;
        }

        public PermissionLevel GetCommandPermission(long channelId)
        {
            if (_commandPermission.ChannelPermissionLevel.ContainsKey(channelId))
            {
                return _commandPermission.ChannelPermissionLevel[channelId];
            }
            return _commandPermission.DefaultLevel;
        }

        private PermissionLevel GetUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }

        private void ExecuteTask()
        {
            //check if it is for about, or if it's for activating the test
            if (_args.Length > 1 && _args[1].Equals("about", StringComparison.CurrentCultureIgnoreCase))
            {
                _taskResult = $"{CommandText()} - {AboutText()}";
            }
            else
            {
                _taskResult = TaskComponent(_args, _messageSource);
            }
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

        public void SetPermission(long channelId, PermissionLevel level)
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

        public abstract string TaskComponent(string[] args, MessageEventArgs messageSource);

        public abstract string PermissionFileSource();

        public abstract CommandPermission DefaultCommands();

        public abstract string CommandText();

        public abstract string AboutText();
    }
}
