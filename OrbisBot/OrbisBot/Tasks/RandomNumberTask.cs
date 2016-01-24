using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class RandomNumberTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Rolls a random number between specified, or you can flip a coin, roll a dice, or get an option from a list";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                return false;
            }
            if (args.Length == 3)
            {
                int a, b;
                return (Int32.TryParse(args[1], out a) && Int32.TryParse(args[2], out b));
            }
            return true;
        }

        public override string CommandText()
        {
            return "random";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false, 1);
        }

        public override string PermissionFileSource()
        {
            return Constants.RANDOM_COMMAND_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length == 2)
            {
                var number = 0;
                if (args[1].Equals("coin", StringComparison.InvariantCultureIgnoreCase))
                {
                    var flip = new Random().Next(2) == 1 ? "Heads" : "Tails";
                    return $"{flip}";
                }
                else if (args[1].Equals("dice", StringComparison.InvariantCultureIgnoreCase))
                {
                    var roll = new Random().Next(7);
                    return $"{roll}";
                }
                else if (Int32.TryParse(args[1], out number))
                {
                    var roll = new Random().Next(number + 1);
                    return $"{roll}";
                }
                else
                {
                    var options = CommandParser.ParseList(args[1]);
                    return options[new Random().Next(0, options.Length)];
                }
            }
            else
            {
                var roll = new Random().Next(Int32.Parse(args[1]), Int32.Parse(args[2]) + 1);
                return $"{roll}";
            }
        }

        public override string UsageText()
        {
            return "<coin|dice> OR (number 1) OR [\"(list of options)\"] OPTIONAL(number 2)";
        }
    }
}
