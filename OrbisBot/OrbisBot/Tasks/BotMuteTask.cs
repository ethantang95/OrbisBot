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
    class BotMuteTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Mutes or unmutes the bot for the channel or server";
        }

        public override string CommandText()
        {
            return "bot-mute";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Admin, true);
        }

        public override string PermissionFileSource()
        {
            return Constants.BOT_CONTROL_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {

            if (args[1].Equals("mute", StringComparison.InvariantCultureIgnoreCase))//we are muting the currrent channel only
            {
                if (args.Length == 3) //we short this becuase we know the param is "server"
                {
                    var serverIds = messageSource.Server.Channels.Select(s => s.Id).ToList();
                    serverIds.ForEach(s => Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, s, true));

                    return $"{messageSource.Server.Name} is now muted from OrbisBot.";
                }
                Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, messageSource.Channel.Id, true);

                return $"{messageSource.Channel.Name} is now muted from OrbisBot.";
            }
            else //this must be unmute
            {
                if (args.Length == 3) //we short this becuase we know the param is "server"
                {
                    var serverIds = messageSource.Server.Channels.Select(s => s.Id).ToList();
                    serverIds.ForEach(s => Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, s, false));

                    return $"{messageSource.Server.Name} is now unmuted from OrbisBot.";
                }
                Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, messageSource.Channel.Id, false);

                return $"{messageSource.Channel.Name} is now unmuted from OrbisBot.";
            }
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return "An error has occurred when trying to mute the bot, the developers has been notified of this problem";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                return false;
            }

            if (args[1].Equals("mute", StringComparison.InvariantCultureIgnoreCase) || args[1].Equals("unmute", StringComparison.InvariantCultureIgnoreCase))
            {
                if (args.Length > 2)
                {
                    if (args[2].Equals("server", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public override string UsageText()
        {
            return "<mute|unmute> OPTIONAL<server>";
        }
    }
}
