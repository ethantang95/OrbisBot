using Discord;
using OrbisBot.OrbScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Events
{
    //event dispatch life cycle
    //an event is first sent from an event bank, which contains all the events in memory
    //it will call this abstract to dispatch the event
    //this abstract will first check the channel permissions to see if a dispatch is available
    //if not, the event will be discarded

    //if the event dispatch is available, the event will then call the main body
    //which is an abstract, and it will be dispatched with respect to its event type

    //an event object is not persistent as compared to tasks, it will get discarded after the
    //event has been executed

    static class EventDispatcher
    { 

        //maybe change this to a boolean in the future where an event failed to run, this way
        //it can saved and dispatched later
        public static async void Dispatch(EventForm eventForm) 
        {
            if (!ProceedWithCommand(eventForm))
            {
                return;
            }
            try
            {
                await EventDispatch(eventForm);
            }
            catch (Exception e)
            {
                await DiscordMethods.OnEventFailure(e, eventForm);
            }
        }

        private static bool ProceedWithCommand(EventForm eventForm)
        {
            //we will take the channel Id and see that if it is muted or not
            //future permissions will take place also
            if (Context.Instance.ChannelPermission.ContainsChannel(eventForm.ChannelId))
            {
                return !Context.Instance.ChannelPermission.ChannelPermissions[eventForm.ChannelId].Muted;
            }

            return true;
        }

        private static async Task PublishTask(string message, Channel channel)
        {
            if (message == "" || message == string.Empty)
            {
                return;
            }

            var result = await DiscordMethods.SendChannelMessage(channel, message);
        }

        private static async Task EventDispatch(EventForm eventForm)
        {
            switch (eventForm.EventType)
            {
                case EventType.ChannelEvent: await DispatchChannelEvent(eventForm);
                    break;
                case EventType.ServerEvent: await DispatchServerEvent(eventForm);
                    break;
                case EventType.UserEvent: await DispatchUserEvent(eventForm);
                    break;
                case EventType.InternalError: throw new InvalidOperationException($"{eventForm.EventId} has an undefined event type");
                default: throw new NotImplementedException($"An event type of {eventForm.EventType} has been tried to be executed");

            }
        }

        private static async Task DispatchChannelEvent(EventForm eventForm)
        {
            var channel = DiscordMethods.GetChannelFromID(eventForm.ChannelId);

            var user = DiscordMethods.GetUserFromID(eventForm.UserId);
            
            var server = DiscordMethods.GetServerFromID(eventForm.ServerId);

            if (server == null)
            {
                throw new Exception($"Server {eventForm.ServerId} cannot be found, the owner might have dismissed the bot from the server");
            }

            var mentionUsers = server.Users.Where(s => eventForm.TargetUsers.Contains(s.Id));

            var roles = Context.Instance.Client.Servers.First(s => s.Id == eventForm.ServerId).Roles;

            var config = new OrbScriptConfiger(user, OrbScriptBuildType.Events).SetRoleList(roles).SetUserList(mentionUsers);

            var engine = new OrbScriptEngine(config);

            engine.SetArgs();

            if (eventForm.TargetRole != 0)
            {
                engine.SetCustomArgs("role", eventForm.TargetRole.ToString());
            }

            var message = engine.EvaluateString($"$user reminds: {eventForm.Message}");

            await PublishTask(message, channel);
        }

        private static async Task DispatchServerEvent(EventForm eventForm)
        {
            var user = DiscordMethods.GetUserFromID(eventForm.UserId);

            var server = DiscordMethods.GetServerFromID(eventForm.ServerId);

            if (server == null)
            {
                throw new Exception($"Server {eventForm.ServerId} cannot be found, the owner might have dismissed the bot from the server");
            }

            var mainChannel = server.TextChannels.First(s => s.Id == Context.Instance.ChannelPermission.GetMainChannelForServer(server.Id));

            var mentionUsers = server.Users.Where(s => eventForm.TargetUsers.Contains(s.Id));

            var roles = Context.Instance.Client.Servers.First(s => s.Id == eventForm.ServerId).Roles;

            var config = new OrbScriptConfiger(user, OrbScriptBuildType.Events).SetRoleList            (roles).SetUserList(mentionUsers);

            var engine = new OrbScriptEngine(config);

            engine.SetArgs();

            if (eventForm.TargetRole != 0)
            {
                engine.SetCustomArgs("role", eventForm.TargetRole.ToString());
            }

            var message = engine.EvaluateString($"$user reminds: {eventForm.Message}");

            await PublishTask(message, mainChannel);
        }

        private static async Task DispatchUserEvent(EventForm eventForm)
        {
            //this will just pm the user
            var user = DiscordMethods.GetUserFromID(eventForm.UserId);

            var config = new OrbScriptConfiger(user, OrbScriptBuildType.PrivateMessage);

            var engine = new OrbScriptEngine(config);

            engine.SetArgs();

            var message = engine.EvaluateString($"Reminder: {eventForm.Message}");

            await DiscordMethods.SendChannelMessage(user.PrivateChannel, message);
        }
    }
}
