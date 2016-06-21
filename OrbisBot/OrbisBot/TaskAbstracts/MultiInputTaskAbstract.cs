using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.TaskPermissions;

namespace OrbisBot.TaskAbstracts
{
    //for these type of tasks, long processing should not be done inside the
    //Task component as registering the person to the task is not synchronous
    //Long running tasks should be done in the state tasks
    abstract class MultiInputTaskAbstract : TaskAbstract
    {
        Dictionary<ulong, int> _userStates;

        public MultiInputTaskAbstract(TaskPermissionAbstract permission) : base(permission)
        {
            _userStates = new Dictionary<ulong, int>();
        }

        protected override void PostTaskExecution(bool success, MessageEventArgs messageEventArgs)
        {
            if (!success)
            {
                return;
            }

            Context.Instance.AddStateTask(messageEventArgs.User.Id, this);

            if (!_userStates.ContainsKey(messageEventArgs.User.Id))
            {
                _userStates.Add(messageEventArgs.User.Id, 0);
            }
            else
            {
                _userStates[messageEventArgs.User.Id] = 0;
            }
        }

        public void RunStateTask(string[] args, MessageEventArgs messageSource)
        {
            Task.Run(() => ExecuteStateTask(args, messageSource));
        }

        private async void ExecuteStateTask(string[] args, MessageEventArgs messageSource)
        {
            try {
                //consumes it from the listener
                Context.Instance.RemoveStateTask(messageSource.User.Id);
                var userState = _userStates[messageSource.User.Id];
                if (StateCheckArgs(userState, args))
                {
                    var newState = StateTaskComponent(userState, args, messageSource);
                    if (newState > 0)
                    {
                        _userStates[messageSource.User.Id] = newState;
                        Context.Instance.AddStateTask(messageSource.User.Id, this);
                    }
                    else
                    {
                        _userStates.Remove(messageSource.User.Id);
                        Context.Instance.RemoveStateTask(messageSource.User.Id);
                    }
                }
                else
                {
                    var result = await PublishMessage(StateUsageText(userState), messageSource);
                }
            }
            catch (Exception e)
            {
                var result = PublishMessage(ExceptionMessage(e, messageSource), messageSource);

                await DiscordMethods.OnMessageFailure(e, messageSource);
            }
        }

        public abstract bool StateCheckArgs(int state, string[] args);

        public abstract int StateTaskComponent(int state, string[] args, MessageEventArgs messageSource);

        public abstract string StateUsageText(int state);
    }
}
