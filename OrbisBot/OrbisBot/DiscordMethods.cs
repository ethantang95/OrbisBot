using System;
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
                    if (Context.Instance.Tasks.ContainsKey(commandSplitted[0]))
                    {
                        var task = Context.Instance.Tasks[commandSplitted[0]];
                        var args = CommandParser.ParseCommand(eventArgs.Message.Text);
                        task.RunTask(args, eventArgs);
                    }
                    else if (eventArgs.Message.IsMentioningMe)
                    {
                        var aboutTask = new AboutTask();
                        aboutTask.RunTask(new string[] {"dummy"}, eventArgs);
                        //pass in a dummy string to bypass the NPE
                    }
                }
                catch (Exception ex)
                {
                    await Context.Instance.Client.SendMessage(eventArgs.Channel,
                        $"Error occurred, Exception: {ex.Message}; Message Received {eventArgs.Message.Text}");
                }
            }
        }

        public static async void OnUserJoinsServer(object o, UserEventArgs eventArgs)
        {
            var mainChannelID = Context.Instance.ChannelPermission.GetMainChannelForServer(eventArgs.Server.Id);

            var channels = eventArgs.Server.TextChannels;

            foreach (var channel in channels)
            {
                if (channel.Id == mainChannelID)
                {
                    await Context.Instance.Client.SendMessage(channel, $"Welcome {eventArgs.User.Name} to server {eventArgs.Server.Name}.");
                    break;
                }
            }
        }
    }
}
