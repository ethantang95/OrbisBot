﻿using System;
using System.Linq;
using Discord;
using System.Configuration;
using System.Threading.Tasks;
using OrbisBot.Tasks;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.Events;
using OrbisBot.TaskHelpers.CustomMessages;

namespace OrbisBot
{
    static class DiscordMethods
    {
        public static void LogMessage(object o, LogMessageEventArgs eventArgs)
        {
            Console.WriteLine($"[{eventArgs.Severity}] {eventArgs.Source}: {eventArgs.Message}");
        }

        public static User GetUserFromID(ulong id)
        {
            return Context.Instance.Client.Servers.FirstOrDefault(s => s.GetUser(id) != null)?.GetUser(id);
        }

        public static Channel GetChannelFromID(ulong id)
        {
            return Context.Instance.Client.GetChannel(id);
        }

        public static Server GetServerFromID(ulong id)
        {
            return Context.Instance.Client.Servers.FirstOrDefault(s => s.Id == id);
        }

        public static Channel GetCommandChannel()
        {
            return GetChannelFromID(UInt64.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]));
        }

        public static void SetGame(string game)
        {
            Context.Instance.Client.SetGame(game);
        }

        public static bool SendPrivateMessage(ulong userId, string message)
        {
            var client = Context.Instance.Client;

            var user = client.Servers.First(s => s.GetUser(userId) != null).GetUser(userId);

            if (user == null)
            {
                return false;
            }
            try
            {
                var result = user.PrivateChannel.SendMessage(message);
            }
            catch (Exception e)
            {
                OnPrivateMessageFailure(e, user, message);
            }

            return true;
        }

        public static async void OnMessageFailure(Exception ex, MessageEventArgs eventArgs)
        {
            try
            { 
                var result = await GetCommandChannel().SendMessage($"An exception has occurred in channel {eventArgs.Channel.Name} in server {eventArgs.Server.Name} with the message: {eventArgs.Message.Text}. \n The exception details are: {ex.Message}, {ex.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static async void OnPrivateMessageFailure(Exception ex, User user, string message)
        {
            try
            {
                var result = await GetCommandChannel().SendMessage($"An exception has occurred in attempting to send private message to {user.Id} - {user.Name} with the message: {message}. \n The exception details are: {ex.Message}, {ex.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static async void OnEventFailure(Exception ex, EventForm eventForm)
        {
            try
            {
                var result = await GetCommandChannel().SendMessage( $"An event {eventForm.EventId} has failed to be dispatched for server {eventForm.ServerId}, channel {eventForm.ChannelId}, user {eventForm.UserId}. The event message is {eventForm.Message}. \n The exception details are: {ex.Message}, {ex.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static async void OnMessageReceived(object o, MessageEventArgs eventArgs)
        {
            if (!eventArgs.Message.IsAuthor)
            {
                try
                {
                    if (eventArgs.Channel.IsPrivate)
                    {
                        //private message, forward it to the private inbox
                            var result = await GetCommandChannel().SendMessage($"User {eventArgs.User.Name}, {eventArgs.User.Id}: {eventArgs.Message.Text}");

                        var replyResult = await eventArgs.User.PrivateChannel.SendMessage("Your message has been sent to a developer, he/she will get back to you shortly. The command trigger for the bot is '-'. To know more about the bot, try '-about'");

                    }
                    else
                    {
                        var commandSplitted = eventArgs.Message.Text.Split(' ');
                        if (Context.Instance.Tasks.ContainsKey(commandSplitted[0].ToLower()))
                        {
                            var task = Context.Instance.Tasks[commandSplitted[0].ToLower()];
                            var args = CommandParser.ParseCommand(eventArgs.Message.Text);
                            task.RunTask(args, eventArgs);
                        }
                        else if (eventArgs.Message.IsMentioningMe())
                        {
                            var aboutTask = Context.Instance.Tasks[Constants.TRIGGER_CHAR + "bot-mention"];
                            aboutTask.RunTask(new string[] { "dummy" }, eventArgs);
                            //pass in a dummy string to bypass the NPE
                        }
                        else if (Context.Instance.InProgressStateTasks.ContainsKey(eventArgs.User.Id))
                        {
                            var task = Context.Instance.InProgressStateTasks[eventArgs.User.Id];
                            var args = CommandParser.ParseCommand(eventArgs.Message.Text);
                            task.RunStateTask(args, eventArgs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnMessageFailure(ex, eventArgs);
                }
            }
        }

        public static async void OnUserJoinsServer(object o, UserEventArgs eventArgs)
        {
            var mainChannelID = Context.Instance.ChannelPermission.GetMainChannelForServer(eventArgs.Server.Id);

            var channel = eventArgs.Server.TextChannels.FirstOrDefault(s => s.Id == mainChannelID);

            if (channel == null)
            {
                return;
            }

            var server = Context.Instance.ServerSettings.GetServerSettings(eventArgs.Server.Id);

            if (server.EnableWelcome)
            {
                try
                {
                    var commandBuilder = new CustomMessageBuilder(server.WelcomeMsg, new string[] { }, eventArgs.User, eventArgs.Server.Users, eventArgs.Server.Roles, Context.Instance.GlobalSetting.HideList);

                    var result = channel.SendMessage(commandBuilder.GenerateGeneralMessage().GetMessage());
                }
                catch (Exception e)
                {
                    try
                    {
                        var result = await GetCommandChannel().SendMessage($"An exception has occurred in channel {channel.Name} in server {eventArgs.Server.Name} with the message: {server.WelcomeMsg}. \n The exception details are: {e.ToString()}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error has occurred when trying to log message. {ex.ToString()}");
                    }
                }
            }
        }
    }
}
