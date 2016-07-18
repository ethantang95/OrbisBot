using OrbisBot.TaskAbstracts;
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
    class DevRemoveEventTask : TaskAbstract
    {
        public DevRemoveEventTask(CommandChannelTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Remove a task of that name or ID from any channel";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2;
        }

        public override string CommandText()
        {
            return "dev-events-remove";
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
                return "This command can only remove events by an ID number";
            }

            //this is an ID, we can just delete directly
            Context.Instance.EventManager.RemoveEvent(eventObj);


            return $"Event {eventObj.EventName} has been removed";
        }

        public override string UsageText()
        {
            return "(ID)";
        }
    }
}
