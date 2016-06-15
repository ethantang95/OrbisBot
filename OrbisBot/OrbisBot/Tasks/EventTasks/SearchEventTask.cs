using OrbisBot.TaskAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.TaskPermissions;
using Discord;

namespace OrbisBot.Tasks.EventTasks
{
    class SearchEventTask : TaskAbstract
    {
        public SearchEventTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Search for tasks by name in this channel";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2;
        }

        public override string CommandText()
        {
            return "events-find";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var events = Context.Instance.EventManager.EventDAOAccessor.FindEventByName(args[1], messageSource.Channel.Id);

            var builder = new StringBuilder();

            events.ForEach(s =>
            {
                builder.AppendLine($"ID: {s.EventId}")
                        .AppendLine($"Name: {s.EventName}")
                        .AppendLine($"Message: {s.Message}")
                        .AppendLine($"Next Dispatch: {s.DispatchTime}");

                if (s.NextDispatchPeriod > 0)
                {
                    builder.AppendLine($"Dispatch period: {new TimeSpan(s.NextDispatchPeriod).ToString()}");
                }
            });

            if (events.Count == 0)
            {
                builder.AppendLine($"There are no events found for the name {args[1]}");
            }

            return builder.ToString();
        }

        public override string UsageText()
        {
            return "\"(event name)\"";
        }
    }
}
