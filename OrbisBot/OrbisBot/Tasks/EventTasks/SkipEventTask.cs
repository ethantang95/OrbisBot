using Discord;
using OrbisBot.Events;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Tasks.EventTasks
{
    class SkipEventTask : TaskAbstract
    {
        public SkipEventTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Removes a task of that name or ID";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2;
        }

        public override string CommandText()
        {
            return "events-skip";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            long id;

            EventForm eventObj;

            if (long.TryParse(args[1], out id))
            {
                //first, validate that if this event belongs to the channel
                eventObj = Context.Instance.EventManager.EventDAOAccessor.FindEventById(id);

                if (eventObj == null)
                {
                    return $"Cannot find event with the ID of {id}";
                }
            }
            else
            {
                var candidates = Context.Instance.EventManager.EventDAOAccessor.FindEventByName(args[1], messageSource.Channel.Id);

                if (candidates.Count > 1)
                {
                    return "Found more than one event with that name, please search the event by name to confirm the event you want to skip";
                }
                else if (candidates.Count == 0)
                {
                    return $"Cannot find event by the name of {args[1]}";
                }
                else
                {
                    eventObj = candidates[0];
                }
            }

            if (messageSource.Channel.Id != eventObj.ChannelId)
            {
                return "This event does not belong to this channel, the skip is not completed";
            }
            //this is an ID, we can just delete directly
            Context.Instance.EventManager.SkipEvent(id);

            return $"Event {eventObj.EventName} has been skipped and will dispatch in the next occurance";
        }

        public override string UsageText()
        {
            return "\"(event name)\"";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            if (ex is EventOperationsException)
            {
                var eventEx = (EventOperationsException)ex;
                PublishDevMessage($"Event {eventEx.Form.EventName} with ID {eventEx.Form.EventId} failed to be skipped", eventArgs);
                return $"Event did not skip successfully, the developers has been notified";
            }
            return base.ExceptionMessage(ex, eventArgs);
        }
    }
}
