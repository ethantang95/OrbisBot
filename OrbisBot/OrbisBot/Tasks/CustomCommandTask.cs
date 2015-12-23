using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class CreateCustomTask : TaskAbstract
    {
        public override string AboutText()
        {
            return "Allows you to create a custom task with its customized message";
        }

        public override string CommandText()
        {
            return "CreateCommand";
        }

        public override CommandPermission DefaultCommands()
        {
            return new CommandPermission(false, PermissionLevel.Moderator, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.CUSTOM_COMMAND_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //the first args is how many (most) args will the commands take
            //the second args are the commands that will be randomly picked
            //commands can be specified with these replacement args
            //%1...N represents the args passed by the user calling the custom command
            //%r(a-b) represents getting a random number between the range of a to b
            //%e1...N(a) represents that the value inside there will be watched
            //%?(%ea op %eb)(a : b) represents a boolean evaluation where a or b are the return results
            //%e(eval) represents an evaluation of a mathematical expression
            //%s represents self
            return null;
        }
    }
}
