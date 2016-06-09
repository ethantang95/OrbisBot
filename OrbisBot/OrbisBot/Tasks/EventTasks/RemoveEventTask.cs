﻿using OrbisBot.TaskAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.TaskPermissions;
using Discord;
using OrbisBot.Events;

namespace OrbisBot.Tasks.EventTasks
{
    class RemoveEventTask : TaskAbstract
    {
        public RemoveEventTask(FileBasedTaskPermission permission) : base(permission)
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
            return "events-remove";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            long id;

            EventForm eventObj;

            if (long.TryParse(args[1], out id))
            {
                //first, validate that if this event belongs to the channel
                eventObj = Context.Instance.EventManager.EventDAOAccessor.FindEventById(id);
            }
            else
            {
                var candidates = Context.Instance.EventManager.EventDAOAccessor.FindEventByName(args[1], messageSource.Channel.Id);

                if (candidates.Count > 1)
                {
                    return "Found more than one event with that name, please search the event by name to confirm the event you want to delete";
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
                return "This event does not belong to this channel, the removal is not completed";
            }
            //this is an ID, we can just delete directly
            var result = Context.Instance.EventManager.EventDAOAccessor.RemoveEvent(id);

            if (result)
            {
                return $"Event {eventObj.EventName} has been removed";
            }
            else
            {
                PublishDevMessage($"Event {eventObj.EventName} with ID {eventObj.EventId} failed to be removed", messageSource);
                return $"Event {eventObj.EventName} did not remove successfully, the developers has been notified";
            }
        }

        public override string UsageText()
        {
            return "\"(event name)\"";
        }
    }
}
