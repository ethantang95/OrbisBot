using OrbisBot.TaskAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.TaskPermissions;
using Discord;

namespace OrbisBot.Tasks.FunTasks
{
    class BlockifyTask : TaskAbstract
    {
        public BlockifyTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Returns the input as blocky letters";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2;
        }

        public override string CommandText()
        {
            return "blockify";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var text = args[1];

            var builder = new StringBuilder();

            foreach (var letter in text)
            {
                if (char.IsLetter(letter))
                {
                    var lowerLetter = char.ToLower(letter);
                    builder.Append($":regional_indicator_{lowerLetter}:");
                }
                else if (char.IsDigit(letter))
                {
                    switch (letter)
                    {
                        case '0':
                            builder.Append(":zero:");
                            break;
                        case '1':
                            builder.Append(":one:");
                            break;
                        case '2':
                            builder.Append(":two:");
                            break;
                        case '3':
                            builder.Append(":three:");
                            break;
                        case '4':
                            builder.Append(":four:");
                            break;
                        case '5':
                            builder.Append(":five:");
                            break;
                        case '6':
                            builder.Append(":six:");
                            break;
                        case '7':
                            builder.Append(":seven:");
                            break;
                        case '8':
                            builder.Append(":eight:");
                            break;
                        case '9':
                            builder.Append(":nine:");
                            break;
                    }
                }
                else
                {
                    builder.Append(letter);
                }   
            }

            return builder.ToString();
        }

        public override string UsageText()
        {
            return "(\"text\")";
        }
    }
}
