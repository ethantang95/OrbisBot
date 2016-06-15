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
    class BotPrivacyTask : TaskAbstract
    {
        public BotPrivacyTask(DiscreteTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Hide yourself from the bot being able to search you from commands globally";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2 &&
                (args[1].Equals("hide", StringComparison.InvariantCultureIgnoreCase) ||
                args[1].Equals("unhide", StringComparison.InvariantCultureIgnoreCase));
        }

        public override string CommandText()
        {
            return "bot-privacy";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args[1].Equals("hide", StringComparison.InvariantCultureIgnoreCase))
            {
                Context.Instance.GlobalSetting.AddToHide(messageSource.User.Id);
                return "You are now hidden from being able to be mentioned by the bot";
            }
            else
            {
                Context.Instance.GlobalSetting.RemoveFromHide(messageSource.User.Id);
                return "You are now unhidden from being able to be mentioned by the bot";
            }
        }

        public override string UsageText()
        {
            return "<hide|unhide>";
        }
    }
}
