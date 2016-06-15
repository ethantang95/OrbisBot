using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using System.Text.RegularExpressions;
using OrbisBot.TaskPermissions;
using OrbisBot.Events;

namespace OrbisBot.Tasks.EventTasks
{
    class CreateEventTask : TaskAbstract
    {
        public CreateEventTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Creates an event which will notify you this message at later time";
        }

        public override bool CheckArgs(string[] args)
        {
            //event should have a name, message, target, time of dispatch, timezone, and an optional time of delay
            if (args.Length != 6 && args.Length != 7)
            {
                return false;
            }

            //regex check the date
            //date format is yyyy/mm/dd hh:mm
            var timeRegex = new Regex(@"\d{2,4}\/\d{1,2}\/\d{1,2} \d{1,2}:\d{2}");
            //delay format is dd hh:mm
            var delayRegex = new Regex(@"\d{1,2}.\d{1,2}:\d{2}");

            if (!timeRegex.IsMatch(args[4]))
            {
                return false;
            }

            if (args.Length == 7 && !delayRegex.IsMatch(args[6]))
            {
                return false;
            }

            return true;
        }

        public override string CommandText()
        {
            return "events-create";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            long a;
            if (long.TryParse(args[2], out a))
            {
                return "The name of the event cannot all be numbers";
            }

            var eventForm = new EventForm();

            //first, parse the time and delay values
            var timeString = args[4] + " " + CommonTools.TimezoneConvert(args[5]);
            var dispatchTime = DateTime.Parse(timeString);

            eventForm.EventName = args[1];
            eventForm.ServerId = messageSource.Server.Id;
            eventForm.ChannelId = messageSource.Channel.Id;
            eventForm.UserId = messageSource.User.Id;
            eventForm.Message = args[2];
            eventForm.TargetEveryone = false;
            eventForm.NextDispatchPeriod = -1;

            var mentioned = messageSource.Message.MentionedUsers;
            eventForm.TargetUsers = mentioned.Select(s => s.Id).ToList();

            //first, check to see if any roles are mentioned
            if (messageSource.Message.MentionedRoles.Count() > 0)
            {
                if (messageSource.Message.MentionedRoles.Count() > 1)
                {
                    return "You cannot mention more than one role in an event at the moment";
                }
                var role = messageSource.Message.MentionedRoles.First();

                if (role.IsEveryone)
                {
                    eventForm.TargetEveryone = true;
                }
                else
                {
                    eventForm.TargetRole = role.Id;
                }
            }

            eventForm.DispatchTime = dispatchTime.ToUniversalTime();

            if (args.Length == 7)
            {
                //there is a next dispatch period
                var nextDispatch = TimeSpan.Parse(args[6]);

                eventForm.NextDispatchPeriod = CommonTools.ToUnixMilliTime(nextDispatch);
            }

            eventForm.EventType = EventType.ChannelEvent;

            Context.Instance.EventManager.CreateEvent(eventForm);

            return $"Created Event {eventForm.EventName}";
        }

        public override string UsageText()
        {
            return "\"(name)\" \"(message)\" [\"(target users or roles)\"] \"(time of dispatch in yyyy/mm/dd hh:mm)\" (timezone abbreviation) *\"(Cycle Period in dd.hh:mm)\"";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            if (ex is FormatException)
            {
                return "There seems to be a bad format with the way the time is displayed, did you include a valid timezone symbol?";
            }
            else if (ex is NotSupportedException)
            {
                return "The current timezone you have entered is not supported";
            }

            return base.ExceptionMessage(ex, eventArgs);
        }
    }
}
