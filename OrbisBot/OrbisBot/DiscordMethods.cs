﻿using System;
using System.Linq;
using Discord;
using System.Configuration;
using System.Threading.Tasks;
using OrbisBot.Tasks;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.Events;

namespace OrbisBot
{
    class DiscordMethods
    {
        public static void LogMessage(object o, LogMessageEventArgs eventArgs)
        {
            Console.WriteLine($"[{eventArgs.Severity}] {eventArgs.Source}: {eventArgs.Message}");
        }

        public static async void OnMessageFailure(Exception ex, MessageEventArgs eventArgs)
        {
            try
            {
                var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]));

                var result = await Context.Instance.Client.SendMessage(loggingChannel, $"An exception has occurred in channel {eventArgs.Channel.Name} in server {eventArgs.Server.Name} with the message: {eventArgs.Message.Text}. \n The exception details are: {ex.Message}, {ex.ToString()}");
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
                var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]));

                var result = await Context.Instance.Client.SendMessage(loggingChannel, $"An event {eventForm.EventId} has failed to be dispatched for server {eventForm.ServerId}, channel {eventForm.ChannelId}, user {eventForm.UserId}. The event message is {eventForm.Message}. \n The exception details are: {ex.Message}, {ex.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static bool SendPrivateMessage(long userId, string message)
        {
            var client = Context.Instance.Client;

            var user = client.GetUser(client.AllServers.First(s => client.GetUser(s, userId) != null), userId);

            if (user == null)
            {
                return false;
            }

            var result = client.SendPrivateMessage(user, message);

            return true;
        }

        public static async void OnMessageReceived(object o, MessageEventArgs eventArgs)
        {
            var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]));

            if (!eventArgs.Message.IsAuthor)
            {
                try
                {
                    if (eventArgs.Channel.IsPrivate)
                    {
                        //private message, forward it to the private inbox
                            var result = await Context.Instance.Client.SendMessage(loggingChannel, $"User {eventArgs.User.Name}, {eventArgs.User.Id}: {eventArgs.Message.Text}");

                        var replyResult = await Context.Instance.Client.SendPrivateMessage(eventArgs.User, "Your message has been sent to a developer, he/she will get back to you shortly. The command trigger for the bot is '-'. To know more about the bot, try '-about'");

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
                        else if (eventArgs.Message.IsMentioningMe && !eventArgs.Message.MentionedRoles.Contains(eventArgs.Server.EveryoneRole))
                        {
                            var aboutTask = Context.Instance.Tasks[Constants.TRIGGER_CHAR + "bot-mention"];
                            aboutTask.RunTask(new string[] { "dummy" }, eventArgs);
                            //pass in a dummy string to bypass the NPE
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
                    var commandBuilder = new CustomCommandBuilder(server.WelcomeMsg, new string[] { }, eventArgs.User.Name, eventArgs.Server.Members);

                    var result = await Context.Instance.Client.SendMessage(channel, commandBuilder.GenerateCustomMessage());
                }
                catch (Exception e)
                {
                    try
                    {
                        var loggingChannel = Context.Instance.Client.GetChannel(Int64.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]));

                        var result = await Context.Instance.Client.SendMessage(loggingChannel, $"An exception has occurred in channel {channel.Name} in server {eventArgs.Server.Name} with the message: {server.WelcomeMsg}. \n The exception details are: {e.ToString()}");
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
        }
    }
}
