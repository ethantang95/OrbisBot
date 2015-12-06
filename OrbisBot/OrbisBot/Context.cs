using Discord;
using OrbisBot.Permission;
using OrbisBot.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot
{
    class Context
    {

        public DiscordClient Client { get; private set; }
        public Dictionary<string, TaskAbstract> Tasks { get; private set; }
        public ChannelPermissionsWrapper ChannelPermission { get; private set; }

        private static Context _context;

        public static Context Instance
        {
            get
            {
                //theoretically, it should not be called in this program since
                //we have initialize to do this
                if (_context == null)
                {
                    _context = new Context();
                    _context.Initalize();
                }
                return _context;
            }
        }

        private Context()
        {

        }

        private void Initalize()
        {
            Client = new DiscordClient();
            Tasks = new Dictionary<string, TaskAbstract>();
            ChannelPermission = new ChannelPermissionsWrapper();
            PopulateTaskDictionary();
            SetUpDiscordClient();
        }

        private void PopulateTaskDictionary()
        {
            Tasks.Add("!Wolfram", new WolframAlphaTask());
        }

        private void SetUpDiscordClient()
        {
            Client = new DiscordClient();

            //Display all log messages in the console
            Client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

#pragma warning disable CS1998
            Client.MessageReceived += async (s, e) =>
#pragma warning restore CS1998
            {
                if (!e.Message.IsAuthor)
                {
                    try
                    {
                        var commandSplitted = e.Message.Text.Split(' ');
                        var command = e.Message.Text;
                        if (Context.Instance.Tasks.ContainsKey(commandSplitted[0]))
                        {
                            var task = Context.Instance.Tasks[commandSplitted[0]];
                            var args = CommandParser.ParseCommand(e.Message.Text);
                            task.RunTask(args, e);
                        }
                    }
                    catch (Exception ex)
                    {
                        await Client.SendMessage(e.Channel, string.Format("Error occurred, Exception: {0}; Message Received {1}", ex.Message, e.Message.Text));
                    }
                }
            };

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            Client.Run(async () =>
            {
                //Connect to the Discord server using our email and password
                await Client.Connect("ethan.tang95@outlook.com", "WfpZw0j7wrx7NJN4I3oxg5qDSs");
            });
        }
    }
}
