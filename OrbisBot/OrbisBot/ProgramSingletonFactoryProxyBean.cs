using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.Tasks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.ServerSettings;

namespace OrbisBot
{
    class Context
    {

        public DiscordClient Client { get; private set; }
        public Dictionary<string, TaskAbstract> Tasks { get; private set; }
        public ChannelPermissionsWrapper ChannelPermission { get; private set; }
        //in the future, maybe add a wrapper if it gets too big
        public ServerSettingsWrapper ServerSettings { get; private set; }
        public Dictionary<string, string> OAuthKeys { get; set; }

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

        public void Initalize()
        {
            Client = new DiscordClient();
            Tasks = new Dictionary<string, TaskAbstract>();
            OAuthKeys = new Dictionary<string, string>();
            ChannelPermission = new ChannelPermissionsWrapper();
            ServerSettings = new ServerSettingsWrapper();
            PopulateTaskDictionary();
            PopulateCustomTasks();
            SetUpDiscordClient();
        }

        private void PopulateTaskDictionary()
        {
            AddTask(new WolframAlphaTask());
            AddTask(new AboutTask());
            AddTask(new RegisterTask());
            AddTask(new AdjustUserPermissionTask());
            AddTask(new ChangeCommandPermissionTask());
            AddTask(new ListCommandsTask());
            AddTask(new UserInfoTask());
            AddTask(new CreateCustomTask());
            AddTask(new RandomNumberTask());
            AddTask(new ChangeMainChannelTask());
            AddTask(new BotMuteTask());
            AddTask(new CutePictureTask());
            AddTask(new RedditTask());
            AddTask(new ExceptionThrowingTask());
            AddTask(new AutoRoleAssignTask());
            AddTask(new RestartContextTask());
            AddTask(new BotMentionTask());
            AddTask(new FiftyShadeFicTask());
            AddTask(new InsultTask());
            AddTask(new MentionRoleTask());
            AddTask(new RemovveCustomTask());
            AddTask(new ServerWelcomeSettingsTask());
        }

        private void PopulateCustomTasks()
        {
            var files = FileHelper.GetAllFiles(Constants.CUSTOM_COMMANDS_FOLDER);
            files.ForEach(s =>
            {
                var customTaskContents = CustomCommandFileHandler.GetCustomTaskFromFile(s);
                var customTask = new CustomTask(customTaskContents[0].CommandName, customTaskContents);
                AddTask(customTask);
            });
        }

        public void AddTask(TaskAbstract toAdd)
        {
            Tasks.Add(toAdd.CommandTrigger(), toAdd);
        }

        public void DestorySelf()
        {
            Client.Disconnect();
            Client = null;
            Tasks = null;
            ChannelPermission = null;
            OAuthKeys = null;
        }

        private void SetUpDiscordClient()
        {
            Client = new DiscordClient();

            //Display all log messages in the console
            Client.LogMessage += DiscordMethods.LogMessage;

            Client.MessageReceived += DiscordMethods.OnMessageReceived;

            //Client.UserJoined += DiscordMethods.OnUserJoinsServer;
            //disabled until I manage to set server permissions and settings files

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
