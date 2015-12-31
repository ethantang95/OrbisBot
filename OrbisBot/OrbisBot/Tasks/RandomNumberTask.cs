using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class RandomNumberTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Rolls a random number between specified, or you can do dice or coin to roll a dice or flip a coin";
        }

        public override string CommandText()
        {
            return "random";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.RANDOM_COMMAND_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //args can be either one word, coin or dice following, or 1 number representing max
            //or 2 numbers representing range
            if (args.Length < 2 || args.Length > 3)
            {
                return $"{Constants.SYNTAX_INTRO} coin/dice/<max number> or it can be \n {Constants.SYNTAX_INTRO} <min number> <max number>";
            }

            if (args.Length == 2)
            {
                if (args[1].Equals("coin", StringComparison.InvariantCultureIgnoreCase))
                {
                    var flip = new Random().Next(2) == 1 ? "Heads" : "Tails";
                    var imageNumber = new Random().Next(4);
                    string imageUrl;
                    switch (imageNumber)
                    {
                        case 0:
                            imageUrl = "https://images.rapgenius.com/744fcae41955504c15351a20d6a289ba.480x284x41.gif";
                            break;
                        case 1:
                            imageUrl = "https://media.giphy.com/media/10bv4HhibS9nZC/giphy.gif";
                            break;
                        case 2:
                            imageUrl = "http://i.giphy.com/5EQC8eAnEAAZG.gif";
                            break;
                        default:
                            imageUrl = "http://i.giphy.com/3o85xrIROugu8cWn4Y.gif";
                            break;
                    }
                    return $"{flip} \n{imageUrl}";
                }
                else if (args[1].Equals("dice", StringComparison.InvariantCultureIgnoreCase))
                {
                    var roll = new Random().Next(7);
                    var imageNumber = new Random().Next(4);
                    string imageUrl;
                    switch (imageNumber)
                    {
                        case 0:
                            imageUrl = "http://www.dreamincode.net/forums/uploads/monthly_06_2010/post-51745-12756374284438.gif";
                            break;
                        case 1:
                            imageUrl = "http://media1.giphy.com/media/b9Spyiqk0ZYJi/200.gif";
                            break;
                        case 2:
                            imageUrl = "http://bestanimations.com/Games/Dice/rolling-dice-gif-10.gif";
                            break;
                        default:
                            imageUrl = "http://searchengineland.com/figz/wp-content/seloads/2014/11/google-roll-dice.gif";
                            break;
                    }
                    return $"{roll} \n{imageUrl}";
                }
                else
                {
                    var roll = new Random().Next(Int32.Parse(args[1]) + 1);
                    return $"{roll}";
                }
            }
            else
            {
                var roll = new Random().Next(Int32.Parse(args[1]), Int32.Parse(args[2]) + 1);
                return $"{roll}";
            }
        }
    }
}
