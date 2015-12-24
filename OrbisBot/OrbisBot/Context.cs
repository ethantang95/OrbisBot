using Discord;
using OrbisBot.Permission;
using OrbisBot.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            AddTask(new WolframAlphaTask());
            AddTask(new AboutTask());
            AddTask(new RegisterSelfTask());
            AddTask(new AdjustUserPermissionTask());
            AddTask(new ChangeCommandPermissionTask());
            AddTask(new ListCommandsTask());
            AddTask(new MetaInfoTask());
        }

        private void AddTask(TaskAbstract toAdd)
        {
            Tasks.Add(toAdd.CommandTrigger(), toAdd);
        }

        private void SetUpDiscordClient()
        {
            Client = new DiscordClient();

            //Display all log messages in the console
            Client.LogMessage += DiscordMethods.LogMessage;

#pragma warning disable CS1998
            Client.MessageReceived += DiscordMethods.OnMessageReceived;

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            Client.Run(async () =>
            {
                //Connect to the Discord server using our email and password
                var username = ConfigurationManager.AppSettings[Constants.DISCORD_USERNAME_KEY];
                var password = ConfigurationManager.AppSettings[Constants.DISCORD_PASSWORD_KEY];
                await Client.Connect(username, password);
            });
        }
    }
}
