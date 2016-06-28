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
            return !(args.Length != 4 || !Int32.TryParse(args[2], out maxParams)) ;
        }

        public override string CommandText()
        {
            return "commands-create";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //the first args will be what the command is called
            //the second args is how many (most) args will the commands take
            //the third args are the commands that will be randomly picked
            //commands can be specified with these replacement args
            //%1...N represents the args passed by the user calling the custom command
            //%r(a:b) represents getting a random number between the range of a to b
            //%v1...N(a) represents that the value inside there will be watched
            //%?(%ea op %eb)(a : b) represents a boolean evaluation where a or b are the return results
            //%e(eval) represents an evaluation of a mathematical expression
            //%u represents self

            int maxParams = int.Parse(args[2]);

            if (Context.Instance.Tasks.ContainsKey(args[1]) && Context.Instance.Tasks[args[1]].GetType() != typeof(CustomTask))
            {
                return "The name of the command already exist and is not a custom command";
            }

            //separate the commands
            var rawArgs = CommandParser.ParseCommand(messageSource.Message.RawText);
            var customReturns = CommandParser.ParseList(rawArgs[3]);

            //we are no longer validating the commands because it basically is a hard NP problem or even a halting problem, so we will fail on runtime
            var customCommand = new CustomCommandForm(args[1], maxParams, messageSource.Channel.Id, PermissionLevel.User, customReturns.ToList(), 30);

            var triggerChar = Context.Instance.ServerSettings.GetTriggerChar(messageSource.Server.Id);

            if (Context.Instance.Tasks.ContainsKey(args[1]))
            {
                var task = (CustomTask)Context.Instance.Tasks[args[1]];
                task.AddContent(customCommand);
                return $"The command {triggerChar}{task.CommandText()} has been added";
            }

            var customCommandList = new List<CustomCommandForm> { customCommand };

            var permission = new RegisteredChannelTaskPermissionBuilder()
                    .SetSaver(new SaveCustomCommands())
                    .SetPermissions(customCommandList)
                    .BuildPermission();

            var newTask = new CustomTask(args[1], customCommandList, permission);

            Context.Instance.AddTask(newTask);

            CustomCommandFileHandler.SaveCustomTask(newTask.GetCustomCommands());

            return $"The command {triggerChar}{newTask.CommandText()} has been added";
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
