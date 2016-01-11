using Discord;
using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrbisBot.TaskAbstracts
{
    abstract class TaskAbstract : IComparable<TaskAbstract>
    {
        private string _taskResult;
        private string[] _args;
        private MessageEventArgs _messageSource;
        protected CommandPermission _commandPermission;

        public TaskAbstract()
        {
            //to ensure non-nullability, we will always start the command permissions to start with default
            _commandPermission = DefaultCommandPermission();
        }

        public bool IsCommandDisabled()
        {
            return _commandPermission.Disabled;
        }

        public void RunTask(string[] args, MessageEventArgs messageEventArgs)
        {
            //here, check if we will proceed based on the command and channel settings
            if (!ProceedWithCommand(messageEventArgs) || !AllowTaskExecution(messageEventArgs))
            {
                return;
            }

            _args = args;
            _messageSource = messageEventArgs;
            //do consider using a threadpool in the future to prevent request bombs
            Task.Run(() => ExecuteTask());
        }

        private bool ProceedWithCommand(MessageEventArgs messageEventArgs)
        {
            //in this context, TaskAbstract knows that such properties exists and knows how to handle it
            //other about the list of channels should be handled by its respective inherited class
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

        private async void ExecuteTask()
        {
            try
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
            }
            catch (Exception e)
            {
                try
                {
                    _taskResult = ExceptionMessage(e, _messageSource);

                    var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.LOGGING_CHANNEL]));

                    var result = await Context.Instance.Client.SendMessage(loggingChannel, $"An exception has occurred in channel {_messageSource.Channel.Name} in server {_messageSource.Server.Name} with the message: {_messageSource.Message.Text}. \n The exception details are: {e.ToString()}");
                }
                catch (Exception ex)
                {
                    //I'm about my wits end.. in case stuff screws up even harder
                    //prolly just return is better
                    return;
                }
            }
            PublishTask();
        }

        protected async void PublishIntermeditate(string message)
        {
            if (message == "" || message == String.Empty)
            {
                return;
            }
            var discordClient = Context.Instance.Client;
            try
            {
                var result = await discordClient.SendMessage(_messageSource.Channel, message);
            }
            catch (Exception ex)
            {
                var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.LOGGING_CHANNEL]));

                await Context.Instance.Client.SendMessage(loggingChannel, $"An exception has occurred publising intermeditate message in channel {_messageSource.Channel.Name} in server {_messageSource.Server.Name} with the message: {_messageSource.Message.Text}. \n The exception details are: {ex.ToString()}");
            }
        }

        private async void PublishTask()
        {
            if (_taskResult == "" || _taskResult == String.Empty)
            {
                return;
            }
            var discordClient = Context.Instance.Client;
            try
            {
                var result = await discordClient.SendMessage(_messageSource.Channel, _taskResult);
            }
            catch (Exception ex)
            {
                var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.LOGGING_CHANNEL]));

                await Context.Instance.Client.SendMessage(loggingChannel, $"An exception has occurred publishing task in channel {_messageSource.Channel.Name} in server {_messageSource.Server.Name} with the message: {_messageSource.Message.Text}. \n The exception details are: {ex.ToString()} \n Stacktrace is: {ex.StackTrace}");
            }
        }

        public string CommandTrigger()
        {
            return Constants.TRIGGER_CHAR + CommandText();
        }

        public bool OverrideMuting()
        {
            return _commandPermission.OverrideMuting;
        }

        public abstract string TaskComponent(string[] args, MessageEventArgs messageSource);

        public abstract CommandPermission DefaultCommandPermission();

        public abstract string CommandText();

        public abstract string AboutText();

        public abstract string ExceptionMessage(Exception ex, MessageEventArgs eventArgs);

        public abstract bool AllowTaskExecution(MessageEventArgs eventArgs);

        public abstract PermissionLevel GetCommandPermissionForChannel(long channelId);

        public abstract void SetCommandPermissionForChannel(long channelId, PermissionLevel newPermissionLevel);

        public int CompareTo(TaskAbstract other)
        {
            return this.CommandText().CompareTo(other.CommandText());
        }
    }
}
