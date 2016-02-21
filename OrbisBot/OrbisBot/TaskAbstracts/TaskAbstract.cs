using Discord;
using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace OrbisBot.TaskAbstracts
{
    abstract class TaskAbstract : IComparable<TaskAbstract>
    {
        protected CommandPermission _commandPermission;
        private Dictionary<long, DateTime> _lastUsed;

        public TaskAbstract()
        {
            //to ensure non-nullability, we will always start the command permissions to start with default
            _commandPermission = DefaultCommandPermission();
            _lastUsed = new Dictionary<long, DateTime>();
            _commandPermission.ChannelPermission.Keys.ToList().ForEach(s => _lastUsed.Add(s, new DateTime(0)));
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

            Task.Run(() => ExecuteTask(args, messageEventArgs));
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

        private async void ExecuteTask(string[] args, MessageEventArgs messageSource)
        {
            string taskResult;
            try
            {
                //check if it is for about, or if it's for activating the test
                if (args.Length > 1 && args[1].Equals("about", StringComparison.CurrentCultureIgnoreCase))
                {
                    taskResult = $"{CommandText()} - {AboutText()}. Permission level for this channel: {GetCommandPermissionForChannel(messageSource.Channel.Id)}";
                }
                else if (args.Length > 1 && args[1].Equals("usage", StringComparison.CurrentCultureIgnoreCase))
                {
                    taskResult = $"{CommandText()} {UsageText()}";
                }
                else
                {
                    if (CheckArgs(args))
                    {
                        int lastUsed = SecondsFromLastUsed(messageSource.Channel.Id);
                        if (lastUsed <= -1 || GetCoolDownTime(messageSource.Channel.Id) - lastUsed <= 0)
                        {
                            taskResult = TaskComponent(args, messageSource);
                            UpdateCoolDown(messageSource.Channel.Id);
                        }
                        else
                        {
                            taskResult = $"{CoolDownMesasgePrefix()} {GetCoolDownTime(messageSource.Channel.Id) - lastUsed} seconds remaining.";
                        }
                    }
                    else
                    {
                        taskResult = $"{Constants.USAGE_INTRO} {CommandText()} {UsageText()}";
                    }
                }
            }
            catch (Exception e)
            {
                taskResult = ExceptionMessage(e, messageSource);

                DiscordMethods.OnMessageFailure(e, messageSource);
            }
            await PublishTask(taskResult, messageSource);
        }

        private int SecondsFromLastUsed(long channelId)
        {
            //first, check the last used
            if (_lastUsed.ContainsKey(channelId))
            {
                var difference = (DateTime.Now - _lastUsed[channelId]);
                return (int)difference.TotalSeconds;
            }
            else
            {
                return -1;
            }
        }

        private void UpdateCoolDown(long channelId)
        {
            if (_lastUsed.ContainsKey(channelId))
            {
                _lastUsed[channelId] = DateTime.Now;
            }
            else
            {
                _lastUsed.Add(channelId, DateTime.Now);
            }
        }

        private async Task PublishTask(string message, MessageEventArgs messageSource)
        {
            if (message == "" || message == String.Empty)
            {
                return;
            }
            var discordClient = Context.Instance.Client;
            try
            {
                var result = await discordClient.SendMessage(messageSource.Channel, message);
            }
            catch (Exception ex)
            {
                DiscordMethods.OnMessageFailure(ex, messageSource);
            }
        }

        protected async void PublishIntermeditate(string message, MessageEventArgs messageSource)
        {
            if (message == "" || message == String.Empty)
            {
                return;
            }
            var discordClient = Context.Instance.Client;
            try
            {
                var result = await discordClient.SendMessage(messageSource.Channel, message);
            }
            catch (Exception ex)
            {
                DiscordMethods.OnMessageFailure(ex, messageSource);
            }
        }

        protected async void PublishPrivateMessage(string message, MessageEventArgs messageSource)
        {
            var client = Context.Instance.Client;

            try
            {
                var result = await client.SendPrivateMessage(messageSource.User, message);
            }
            catch (Exception ex)
            {
                DiscordMethods.OnMessageFailure(ex, messageSource);
            }
        }

        public int GetCoolDownTime(long channelId)
        {
            if (_commandPermission.ChannelPermission.ContainsKey(channelId))
            {
                return _commandPermission.ChannelPermission[channelId].CoolDown;
            }
            else
            {
                return DefaultCommandPermission().DefaultCoolDown;
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

        public string CoolDownMesasgePrefix()
        {
            return "This command is currently in cooldown.";
        }

        public abstract bool CheckArgs(string[] args);

        public abstract string TaskComponent(string[] args, MessageEventArgs messageSource);

        public abstract CommandPermission DefaultCommandPermission();

        public abstract string CommandText();

        public abstract string AboutText();

        public abstract string UsageText();

        public abstract string ExceptionMessage(Exception ex, MessageEventArgs eventArgs);

        public abstract bool AllowTaskExecution(MessageEventArgs eventArgs);

        public abstract PermissionLevel GetCommandPermissionForChannel(long channelId);

        public abstract void SetCommandPermissionForChannel(long channelId, PermissionLevel newPermissionLevel);

        public abstract void SetCoolDownForChannel(long channelId, int cooldown);

        public int CompareTo(TaskAbstract other)
        {
            return this.CommandText().CompareTo(other.CommandText());
        }
    }
}
