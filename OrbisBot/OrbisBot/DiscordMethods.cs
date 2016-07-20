using System;
using System.Linq;
using Discord;
using System.Configuration;
using System.Threading.Tasks;
using OrbisBot.Tasks;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.Events;
using OrbisBot.OrbScript;
using OrbisBot.TaskHelpers.ImageCreator;
using OrbisBot.TaskHelpers.AWS;

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
            return GetChannelFromID(ulong.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]));
        }

        public static void SetGame(string game)
        {
            Context.Instance.Client.SetGame(game);
        }

        public static async Task<Message> SendChannelMessage(Channel channel, string message)
        {
            if (message.Length > 2000)
            {
                var imageMsg = LongTextToImage.TextToImage(message);

                message = $"The message exceeded the 2000 character limit of discord. The result is now uploaded onto an image at: {AWSS3UploadUtil.UploadImage(imageMsg)}";
            }

            return await channel.SendMessage(message);
        }

        public static async Task<bool> SendPrivateMessage(ulong userId, string message)
        {
            var client = Context.Instance.Client;

            var user = client.Servers.First(s => s.GetUser(userId) != null).GetUser(userId);

            if (user == null)
            {
                return false;
            }
            try
            {
                var result = await SendChannelMessage(user.PrivateChannel, message);
            }
            catch (Exception e)
            {
                await OnPrivateMessageFailure(e, user, message);
            }

            return true;
        }

        public static async Task SendExceptionMsg(Exception ex)
        {
            try
            {
                var result = await SendChannelMessage(GetCommandChannel(), $"An exception has occurred with the details {ex.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static async Task SendDevMessage(string message, MessageEventArgs eventArgs)
        {
            try
            {
                var result = await SendChannelMessage(GetCommandChannel(), $"channel {eventArgs.Channel.Name} of {eventArgs.Server.Name} has logged the message of {message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static async Task OnMessageFailure(Exception ex, MessageEventArgs eventArgs)
        {
            try
            { 
                var result = await SendChannelMessage(GetCommandChannel(), $"An exception has occurred in channel {eventArgs.Channel.Name} in server {eventArgs.Server.Name} with the message: {eventArgs.Message.Text}. \n The exception details are: {ex.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static async Task OnPrivateMessageFailure(Exception ex, User user, string message)
        {
            try
            {
                var result = await SendChannelMessage(GetCommandChannel(), $"An exception has occurred in attempting to send private message to {user.Id} - {user.Name} with the message: {message}. \n The exception details are: {ex.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error has occurred when trying to log message. {e.ToString()}");
            }
        }

        public static async Task OnEventFailure(Exception ex, EventForm eventForm)
        {
            try
            {
                var result = await SendChannelMessage(GetCommandChannel(), $"An event {eventForm.EventId} has failed to be dispatched for server {eventForm.ServerId}, channel {eventForm.ChannelId}, user {eventForm.UserId}. The event message is {eventForm.Message}. \n The exception details are: {ex.ToString()}");
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
                    //first, check for the trigger char
                    var triggerChar = eventArgs.Channel.IsPrivate ?
                            '-' : Context.Instance.ServerSettings.GetTriggerChar(eventArgs.Server.Id);

                    //execute the task
                    if (eventArgs.Message.Text.Length != 0 && eventArgs.Message.Text[0] == triggerChar)
                    {
                        var command = eventArgs.Message.Text.Split(' ')[0].Substring(1);
                        if (Context.Instance.Tasks.ContainsKey(command.ToLower()))
                        {
                            var task = Context.Instance.Tasks[command.ToLower()];
                            var args = CommandParser.ParseCommand(eventArgs.Message.Text);
                            task.RunTask(args, eventArgs);
                        }
                        else if (eventArgs.Message.IsMentioningMe())
                        {
                            var aboutTask = Context.Instance.Tasks["bot-mention"];
                            aboutTask.RunTask(new string[] { "dummy" }, eventArgs);
                            //pass in a dummy string to bypass the NPE
                        }
                    }
                    else if (Context.Instance.InProgressStateTasks.ContainsKey(eventArgs.User.Id))
                    {
                        var task = Context.Instance.InProgressStateTasks[eventArgs.User.Id];
                        var args = CommandParser.ParseCommand(eventArgs.Message.Text);
                        task.RunStateTask(args, eventArgs);
                    }
                    else if (eventArgs.Channel.IsPrivate)
                    {
                        //private message, forward it to the private inbox
                        var result = await SendChannelMessage(GetCommandChannel(), $"User {eventArgs.User.Name}, {eventArgs.User.Id}: {eventArgs.Message.Text}");

                        var replyResult = await SendChannelMessage(eventArgs.User.PrivateChannel, "Your message has been sent to a developer, he/she will get back to you shortly. The command trigger for the bot is '-'. To know more about the bot, try '-about'. You may use commands inside a private channel with the bot, however, it is not guarenteed the commands will be stable as the bot was made for server usage. The developers are currently working on it");

                    }
                }
                catch (Exception ex)
                {
                    await OnMessageFailure(ex, eventArgs);
                }
            }
        }

        public static async void OnUserJoinsServer(object o, UserEventArgs eventArgs)
        {
            var mainChannelID = Context.Instance.ChannelPermission.GetMainChannelForServer(eventArgs.Server.Id);

            var channel = GetChannelFromID(mainChannelID);

            if (channel == null)
            {
                return;
            }

            var server = Context.Instance.ServerSettings.GetServerSettings(eventArgs.Server.Id);

            if (server.EnableWelcome)
            {
                try
                {
                    var engineConfig = new OrbScriptConfiger(OrbScriptBuildType.JoinLeave)
                        .SetRoleList(eventArgs.Server.Roles)
                        .SetUserList(eventArgs.Server.Users);

                    var engine = new OrbScriptEngine(engineConfig, eventArgs.User);

                    engine.SetArgs();

                    var message = engine.EvaluateString(server.WelcomeMsg);

                    var result = await SendChannelMessage(channel, message);
                }
                catch (Exception e)
                {
                    try
                    {
                        var result = await SendChannelMessage(GetCommandChannel(), $"An exception has occurred in channel {channel.Name} in server {eventArgs.Server.Name} with the message: {server.WelcomeMsg}. \n The exception details are: {e.ToString()}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error has occurred when trying to log message. {ex.ToString()}");
                    }
                }
            }
        }

        public static async void OnUserLeaveServer(object o, UserEventArgs eventArgs)
        {
            var mainChannelID = Context.Instance.ChannelPermission.GetMainChannelForServer(eventArgs.Server.Id);

            var channel = GetChannelFromID(mainChannelID);

            if (channel == null)
            {
                return;
            }

            var server = Context.Instance.ServerSettings.GetServerSettings(eventArgs.Server.Id);

            if (server.EnableGoodbyeMsgs)
            {
                try
                {
                    var engineConfig = new OrbScriptConfiger(OrbScriptBuildType.JoinLeave)
                        .SetRoleList(eventArgs.Server.Roles)
                        .SetUserList(eventArgs.Server.Users);

                    var engine = new OrbScriptEngine(engineConfig, eventArgs.User);

                    engine.SetArgs();

                    var message = engine.EvaluateString(server.GoodbyeMsg);

                    var result = await SendChannelMessage(channel, message);
                }
                catch (Exception e)
                {
                    try
                    {
                        var result = await SendChannelMessage(GetCommandChannel(), $"An exception has occurred in channel {channel.Name} in server {eventArgs.Server.Name} with the message: {server.GoodbyeMsg}. \n The exception details are: {e.ToString()}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error has occurred when trying to log message. {ex.ToString()}");
                    }
                }
            }

            if (server.EnableGoodbyePms)
            {
                try
                {
                    var engineConfig = new OrbScriptConfiger(OrbScriptBuildType.JoinLeave)
                        .SetRoleList(eventArgs.Server.Roles)
                        .SetUserList(eventArgs.Server.Users);

                    var engine = new OrbScriptEngine(engineConfig, eventArgs.User);

                    engine.SetArgs();

                    var message = engine.EvaluateString(server.GoodbyePms);

                    var result = await SendChannelMessage(eventArgs.User.PrivateChannel, message);
                }
                catch (Exception e)
                {
                    await OnPrivateMessageFailure(e, eventArgs.User, server.GoodbyePms);
                }
            }
        }

        public static async void OnUserBanned(object o, UserEventArgs eventArgs)
        {
            var mainChannelID = Context.Instance.ChannelPermission.GetMainChannelForServer(eventArgs.Server.Id);

            var channel = GetChannelFromID(mainChannelID);

            if (channel == null)
            {
                return;
            }

            var server = Context.Instance.ServerSettings.GetServerSettings(eventArgs.Server.Id);

            if (!server.EnableBanNotificaftion)
            {
                return;
            }

            try
            {
                var result = await SendChannelMessage(channel, $"User {eventArgs.User.Name} has been banned from the server");
            }
            catch (Exception e)
            {
                try
                {
                    var result = await SendChannelMessage(GetCommandChannel(), $"An exception has occurred in channel {channel.Name} in server {eventArgs.Server.Name}. \n The exception details are: {e.ToString()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error has occurred when trying to log message. {ex.ToString()}");
                }
            }

            try
            {
                var result = await SendChannelMessage(eventArgs.Server.Owner.PrivateChannel, $"User {eventArgs.User.Name} has been banned from the server");
            }
            catch (Exception e)
            {
                await OnPrivateMessageFailure(e, eventArgs.Server.Owner, $"User {eventArgs.User.Name} has been banned from the server");
            }
        }

        public static async void OnUserUnBanned(object o, UserEventArgs eventArgs)
        {
            var mainChannelID = Context.Instance.ChannelPermission.GetMainChannelForServer(eventArgs.Server.Id);

            var channel = GetChannelFromID(mainChannelID);

            if (channel == null)
            {
                return;
            }

            var server = Context.Instance.ServerSettings.GetServerSettings(eventArgs.Server.Id);

            if (!server.EnableBanNotificaftion)
            {
                return;
            }

            try
            {
                var result = await SendChannelMessage(channel, $"User {eventArgs.User.Name} has been unbanned from the server");
            }
            catch (Exception e)
            {
                try
                {
                    var result = await SendChannelMessage(GetCommandChannel(), $"An exception has occurred in channel {channel.Name} in server {eventArgs.Server.Name}. \n The exception details are: {e.ToString()}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error has occurred when trying to log message. {ex.ToString()}");
                }
            }

            try
            {
                var result = await SendChannelMessage(eventArgs.Server.Owner.PrivateChannel, $"User {eventArgs.User.Name} has been unbanned from the server");
            }
            catch (Exception e)
            {
                await OnPrivateMessageFailure(e, eventArgs.Server.Owner, $"User {eventArgs.User.Name} has been unbanned from the server");
            }
        }
    }
}
