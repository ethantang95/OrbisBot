using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;
using OrbisBot.TaskPermissions.PermissionBuilders;
using OrbisBot.TaskPermissions.Implmentations;

namespace OrbisBot.Tasks
{
    class CreateCustomTask : TaskAbstract
    {
        public CreateCustomTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Create a custom command, for more information, try the command out";
        }

        public override bool CheckArgs(string[] args)
        {
            int maxParams;
            return !(args.Length != 4 || !int.TryParse(args[2], out maxParams)) ;
        }

        public override string CommandText()
        {
            return "commands-create";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var newCommandName = args[1];
            var maxParams = int.Parse(args[2]);
            var isUpperName = false;

            if (newCommandName.Any(s => char.IsUpper(s)))
            {
                newCommandName = newCommandName.ToLower();
                isUpperName = true;
            }

            if (Context.Instance.Tasks.ContainsKey(newCommandName) && Context.Instance.Tasks[newCommandName].GetType() != typeof(CustomTask))
            {
                return "The name of the command already exist and is not a custom command";
            }

            //separate the commands
            var rawArgs = CommandParser.ParseCommand(messageSource.Message.RawText);
            var customReturns = CommandParser.ParseList(rawArgs[3]);

            //we are no longer validating the commands because it basically is a hard NP problem or even a halting problem, so we will fail on runtime
            var customCommand = new CustomCommandForm(newCommandName, maxParams, messageSource.Channel.Id, PermissionLevel.User, customReturns.ToList(), 30);

            var triggerChar = Context.Instance.ServerSettings.GetTriggerChar(messageSource.Server.Id);

            if (Context.Instance.Tasks.ContainsKey(newCommandName))
            {
                var task = (CustomTask)Context.Instance.Tasks[newCommandName];
                task.AddContent(customCommand);
                return $"The command {triggerChar}{task.CommandText()} has been added";
            }

            var customCommandList = new List<CustomCommandForm> { customCommand };

            var permission = new RegisteredChannelTaskPermissionBuilder<CustomCommandForm>()
                    .SetSaver(new SaveCustomCommands())
                    .SetPermissions(customCommandList)
                    .BuildPermission();

            var newTask = new CustomTask(newCommandName, customCommandList, permission);

            Context.Instance.AddTask(newTask);

            CustomCommandFileHandler.SaveCustomTask(newTask.GetCustomCommands());

            var upperNotice = isUpperName ? "Your command contains an uppercase character, it has automatically been converted to all lower cases; however, you can still call the command with upper caes" : string.Empty;

            return $"The command {triggerChar}{newTask.CommandText()} has been added. {upperNotice}";
        }

        public override string UsageText()
        {
            return " (command name) (max number of params) [\"(possible return strings)\"]";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return $"This command failed to be created due to {ex.Message}";
        }
    }
}
