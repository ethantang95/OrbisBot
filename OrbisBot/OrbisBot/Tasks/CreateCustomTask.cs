using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class CreateCustomTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Allows you to create a custom task with its customized message. " + HelpText();
        }

        public override string CommandText()
        {
            return "createcommand";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Moderator, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.CUSTOM_COMMAND_FILE;
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
            int maxParams = 0;

            if (args.Length != 4 || !Int32.TryParse(args[2], out maxParams))
            {
                return HelpText();
            }

            if (Context.Instance.Tasks.ContainsKey(Constants.TRIGGER_CHAR + args[1]) && Context.Instance.Tasks[Constants.TRIGGER_CHAR + args[1]].GetType() != typeof(CustomTask))
            {
                return "The name of the command already exist and is not a custom command";
            }

            //separate the commands
            var customReturns = CommandParser.ParseList(args[3]);

            //now, we will test each args
            foreach (var customReturn in customReturns)
            {
                var fakeParams = Enumerable.Repeat("1", maxParams).ToArray();
                var validationBuilder = new CustomCommandBuilder(customReturn, fakeParams, messageSource.User.Name);
                var result = validationBuilder.GenerateString();
            }


            //here, the command successfully validates, because otherwise, we would've gotten an exception
            var customCommand = new CustomCommandForm(args[1], maxParams, messageSource.Channel.Id, PermissionLevel.User, customReturns.ToList());

            if (Context.Instance.Tasks.ContainsKey(Constants.TRIGGER_CHAR + args[1]))
            {
                var task = (CustomTask)Context.Instance.Tasks[Constants.TRIGGER_CHAR + args[1]];
                task.AddContent(customCommand);
                return $"The command {customCommand.CommandName} has been added";
            }

            var newTask = new CustomTask(args[1], new List<CustomCommandForm> { customCommand });

            Context.Instance.AddTask(newTask);

            CustomCommandFileHandler.SaveCustomTask(newTask.ToFileOutput());

            return $"The command {newTask.CommandTrigger()} has been added";
        }

        private string HelpText()
        {
            return "Allows you to create a custom command, currently still in development";
        }
    }
}
