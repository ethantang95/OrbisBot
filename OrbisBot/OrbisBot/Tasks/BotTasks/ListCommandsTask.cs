using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class ListCommandsTask : TaskAbstract
    {
        public ListCommandsTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var listCustom = args.Length == 2;

            //we will give back commands that are fit for the user's role
            var userPermission = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id, messageSource.User.Id);

            var channelMuted = Context.Instance.ChannelPermission.ContainsChannel(messageSource.Channel.Id) ? Context.Instance.ChannelPermission.ChannelPermissions[messageSource.Channel.Id].Muted : false;

            var availableCommands =
                Context.Instance.Tasks.Where(s =>
                    (s.Value.TaskPermission.GetCommandPermissionForChannel(messageSource.Channel.Id) <= userPermission) &&
                    !s.Value.IsCommandDisabled() &&
                    (!channelMuted ||
                        (s.Value.OverrideMuting() && userPermission >= PermissionLevel.Admin)));

            var returnMessage = new StringBuilder().AppendLine($"The commands you have available as a(n) {userPermission} on this channel are:");

            if (!availableCommands.Any())
            {
                returnMessage.AppendLine("You have no commands available to you");
            }
            else
            {
                var availableTasks = availableCommands.Select(s => s.Value).Where(s => (s is CustomTask) == listCustom).ToList();
                availableTasks.Sort();

                var prevPrefix = "";

                TaskAbstract prevTask = null;

                var triggerChar = Context.Instance.ServerSettings.GetTriggerChar(messageSource.Server.Id);

                availableTasks.ForEach(s =>
                {
                    if (prevTask == s)
                    {
                        return;
                    }

                    var prefix = s.CommandText().Split('-').First();
                    if (prevPrefix == prefix)
                    {
                        returnMessage.Append($"{triggerChar}{s.CommandText()} ");
                    }
                    else
                    {
                        prevPrefix = prefix;
                        returnMessage.AppendLine();
                        returnMessage.Append($"{triggerChar}{s.CommandText()} ");
                    }

                    //append additional triggers
                    if (s.AdditionalCommandTexts().Count() > 0)
                    {
                        returnMessage.Append("(");
                        returnMessage.Append(string.Join(" ", s.AdditionalCommandTexts().Select(r => $"{triggerChar}{s}")));
                        returnMessage.Append(") ");
                    }

                    prevTask = s;
                });
            }

            returnMessage.AppendLine();
            returnMessage.AppendLine("Type about after a command to see what it does or usage to see how to use it.");

            return returnMessage.ToString();
        }

        public override string CommandText()
        {
            return "commands";
        }

        public override string AboutText()
        {
            return "Lists all the commands you have access to in this server and its information";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 1 || (args.Length == 2 && args[1].Equals("custom", StringComparison.InvariantCultureIgnoreCase));
        }

        public override string UsageText()
        {
            return "OPTIONAL<custom>";
        }
    }
}
