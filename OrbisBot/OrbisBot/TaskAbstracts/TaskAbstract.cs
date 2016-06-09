using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskPermissions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace OrbisBot.TaskAbstracts
{
    abstract class TaskAbstract : IComparable<TaskAbstract>
    {
        public TaskPermissionAbstract TaskPermission { get; private set; }
        private Dictionary<ulong, DateTime> _lastUsed;
        private Dictionary<ulong, Dictionary<string, object>> _varDictionary;

        public TaskAbstract(TaskPermissionAbstract permission)
        {
            //to ensure non-nullability, we will always start the command permissions to start with default
            TaskPermission = permission;
            _lastUsed = new Dictionary<ulong, DateTime>();
            _varDictionary = new Dictionary<ulong, Dictionary<string, object>>();
            TaskPermission.CommandPermission.ChannelPermission.Keys.ToList().ForEach(s => _lastUsed.Add(s, new DateTime(0)));
        }

        public bool IsCommandDisabled()
        {
            return TaskPermission.CommandPermission.Disabled;
        }

        public virtual void RunTask(string[] args, MessageEventArgs messageEventArgs)
        {
            //here, check if we will proceed based on the command and channel settings
            if (!ProceedWithCommand(messageEventArgs) || !TaskPermission.AllowTaskExecution(messageEventArgs))
            {
                return;
            }

            Task.Run(() => ExecuteTask(args, messageEventArgs));
        }

        public string ExecuteTaskDirect(string[] args, MessageEventArgs messageEventArgs, int iterations = 0)
        {
            if (!ProceedWithCommand(messageEventArgs) || !TaskPermission.AllowTaskExecution(messageEventArgs))
            {
                throw new InvalidOperationException("You do not have permission to execute this task");
            }
            if (!CheckArgs(args))
            {
                throw new ArgumentException("The arguments passed into this command is not correct");
            }
            if (iterations > 10)
            {
                throw new NotFiniteNumberException("The command is referencing itself back again");
            }

            SetVariable(messageEventArgs.Channel.Id, "iterations", iterations);

            return TaskComponent(args, messageEventArgs);
        }

        private bool ProceedWithCommand(MessageEventArgs messageEventArgs)
        {
            //in this context, TaskAbstract knows that such properties exists and knows how to handle it
            //other about the list of channels should be handled by its respective inherited class
            var proceed = true;
            if (Context.Instance.ChannelPermission.ContainsChannel(messageEventArgs.Channel.Id))
            {
                proceed &= !Context.Instance.ChannelPermission.ChannelPermissions[messageEventArgs.Channel.Id].Muted
                    || (TaskPermission.CommandPermission.OverrideMuting 
                        && Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id, messageEventArgs.User.Id) >= PermissionLevel.Admin); //if a channel is muted, only an admin can proceed with override mute commands
            }
            proceed &= !TaskPermission.CommandPermission.Disabled;
            return proceed;
        }

        private async void ExecuteTask(string[] args, MessageEventArgs messageSource)
        {
            string taskResult;
            bool success = false;
            try
            {
                //check if it is for about, or if it's for activating the test
                if (args.Length > 1 && args[1].Equals("about", StringComparison.CurrentCultureIgnoreCase))
                {
                    taskResult = $"{CommandText()} - {AboutText()}. Permission level for this channel: {TaskPermission.GetCommandPermissionForChannel(messageSource.Channel.Id)}";
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
                            success = true;
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

                await DiscordMethods.OnMessageFailure(e, messageSource);
            }
            await PublishMessage(taskResult, messageSource);

            PostTaskExecution(success, messageSource);
        }

        protected virtual void PostTaskExecution(bool success, MessageEventArgs eventArgs)
        {
            return;
        }

        private int SecondsFromLastUsed(ulong channelId)
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

        private void UpdateCoolDown(ulong channelId)
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

        protected async Task<Message> PublishMessage(string message, MessageEventArgs messageSource)
        {
            if (message == "" || message == String.Empty)
            {
                return null;
            }
            var discordClient = Context.Instance.Client;
            try
            {
                return await messageSource.Channel.SendMessage(message);
            }
            catch (Exception ex)
            {
                await DiscordMethods.OnMessageFailure(ex, messageSource);
                return null;
            }
        }

        protected async Task<Message> PublishPrivateMessage(string message, MessageEventArgs messageSource)
        {
            var client = Context.Instance.Client;

            try
            {
                return await messageSource.User.PrivateChannel.SendMessage(message);
            }
            catch (Exception ex)
            {
                await DiscordMethods.OnPrivateMessageFailure(ex, messageSource.User, message);
                return null;
            }
        }

        protected async void PublishDevMessage(string message, MessageEventArgs messageSource)
        {
            try
            {
                await DiscordMethods.SendDevMessage(message, messageSource);
            }
            catch (Exception ex)
            {
                await DiscordMethods.OnMessageFailure(ex, messageSource);
            }
        }
        public void SetUserVariable(ulong channelId, ulong userId, string name, object obj)
        {
            SetVariable(channelId, userId + name, obj);
        }

        public void SetVariable(ulong channelId, string name, object obj)
        {
            if (!_varDictionary.ContainsKey(channelId))
            {
                _varDictionary.Add(channelId, new Dictionary<string, object>());
            }

            if (!_varDictionary[channelId].ContainsKey(name))
            {
                _varDictionary[channelId].Add(name, obj);
            }
            else
            {
                _varDictionary[channelId][name] = obj;
            }
        }

        public bool HasUserVariable(ulong channelId, ulong userId, string name)
        {
            return HasVariable(channelId, userId + name);
        }

        public bool HasVariable(ulong channelId, string name)
        {
            return (_varDictionary.ContainsKey(channelId) && _varDictionary[channelId].ContainsKey(name));
        }

        public object GetUserVariable(ulong channelId, ulong userId, string name)
        {
            return GetVariable(channelId, userId + name);
        }

        public object GetVariable(ulong channelId, string name)
        {
            if (!_varDictionary.ContainsKey(channelId))
            {
                throw new KeyNotFoundException("The variable dictionary does not exist for this channel yet");
            }

            if (!_varDictionary[channelId].ContainsKey(name))
            {
                throw new KeyNotFoundException($"The key {name} is not found in the variable dictionary");
            }

            return _varDictionary[channelId][name];
        }

        public int GetCoolDownTime(ulong channelId)
        {
            if (TaskPermission.CommandPermission.ChannelPermission.ContainsKey(channelId))
            {
                return TaskPermission.CommandPermission.ChannelPermission[channelId].CoolDown;
            }
            else
            {
                return TaskPermission.CommandPermission.DefaultCoolDown;
            }
        }

        public string CommandTrigger()
        {
            return Constants.TRIGGER_CHAR + CommandText();
        }

        public bool OverrideMuting()
        {
            return TaskPermission.CommandPermission.OverrideMuting;
        }

        public string CoolDownMesasgePrefix()
        {
            return "This command is currently in cooldown.";
        }

        public abstract bool CheckArgs(string[] args);

        public abstract string TaskComponent(string[] args, MessageEventArgs messageSource);

        public abstract string CommandText();

        public abstract string AboutText();

        public abstract string UsageText();

        public virtual string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return string.Empty;
        }

        public int CompareTo(TaskAbstract other)
        {
            return CommandText().CompareTo(other.CommandText());
        }
    }
}
