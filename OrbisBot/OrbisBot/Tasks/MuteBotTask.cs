using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class MuteBotTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Mute the bot in this channel and optionally in this entire server. Commands that overrides muting however still works. Note: both muting and unmuting the bot uses the same permission file.";
        }

        public override string CommandText()
        {
            return "MuteBot";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Admin, true);
        }

        public override string PermissionFileSource()
        {
            return Constants.MUTE_BOT_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length > 2)
            {
                return SyntaxStructure();
            }

            if (args.Length == 1)//we are muting the currrent channel only
            {
                Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, messageSource.Channel.Id, true);

                return $"{messageSource.Channel.Name} is now muted from OrbisBot, to unmute, the command is UnmuteBot.";
            }
            else if (args[1].Equals("server", StringComparison.InvariantCultureIgnoreCase))
            {
                var serverIds = messageSource.Server.Channels.Select(s => s.Id).ToList();
                serverIds.ForEach(s => Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, s, true));

                return $"{messageSource.Server.Name} is now muted from OrbisBot, to unmute the server, the command is UnmuteBot server.";
            }

            return SyntaxStructure();
        }

        private string SyntaxStructure()
        {
            return $"{Constants.SYNTAX_INTRO} OPTIONAL Server. Type the word server after the command to mute the entire server";
        }
    }
}
