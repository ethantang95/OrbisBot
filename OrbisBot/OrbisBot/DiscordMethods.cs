using System;
using System.Linq;
using Discord;
using System.Configuration;
using System.Threading.Tasks;
using OrbisBot.Tasks;

namespace OrbisBot
{
    class DiscordMethods
    {
        public static void LogMessage(object o, LogMessageEventArgs eventArgs)
        {
            Console.WriteLine($"[{eventArgs.Severity}] {eventArgs.Source}: {eventArgs.Message}");
        }

        public static async void OnMessageReceived(object o, MessageEventArgs eventArgs)
        {
            if (!eventArgs.Message.IsAuthor)
            {
                try
                {
                    var commandSplitted = eventArgs.Message.Text.Split(' ');
                    if (Context.Instance.Tasks.ContainsKey(commandSplitted[0].ToLower()))
                    {
                        var task = Context.Instance.Tasks[commandSplitted[0].ToLower()];
                        var args = CommandParser.ParseCommand(eventArgs.Message.Text);
                        task.RunTask(args, eventArgs);
                    }
                    else if (eventArgs.Message.IsMentioningMe && !eventArgs.Message.MentionedRoles.Contains(eventArgs.Server.EveryoneRole))
                    {
                        var aboutTask = Context.Instance.Tasks[Constants.TRIGGER_CHAR + "bot-mention"];
                        aboutTask.RunTask(new string[] {"dummy"}, eventArgs);
                        //pass in a dummy string to bypass the NPE
                    }
                }
                catch (Exception ex)
                {
                    await Context.Instance.Client.SendMessage(eventArgs.Channel,
                        $"Error occurred, Exception: {ex.Message}; Message Received {eventArgs.Message.Text}");

                    var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]));

                    await Context.Instance.Client.SendMessage(loggingChannel, $"An exception has occurred in channel {eventArgs.Channel.Name} in server {eventArgs.Server.Name} with the message: {eventArgs.Message.Text}. \n The exception details are: {ex.Message}, {ex.ToString()} \n Stacktrace is: {ex.StackTrace}");
                }
            }
        }

        public static async void OnUserJoinsServer(object o, UserEventArgs eventArgs)
        {
            var mainChannelID = Context.Instance.ChannelPermission.GetMainChannelForServer(eventArgs.Server.Id);

            var channel = eventArgs.Server.TextChannels.FirstOrDefault(s => s.Id == mainChannelID);

            if (channel != null)
            {
                await Context.Instance.Client.SendMessage(channel, $"Welcome {eventArgs.User.Name} to server {eventArgs.Server.Name}.");
            }
        }
    }
}
