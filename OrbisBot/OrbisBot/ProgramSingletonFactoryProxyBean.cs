﻿using Discord;
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
using OrbisBot.GlobalSettings;
using DatabaseConnector;

namespace OrbisBot
{
    class Context
    {

        public DiscordClient Client { get; private set; }
        public Dictionary<string, TaskAbstract> Tasks { get; private set; }
        public ChannelPermissionsWrapper ChannelPermission { get; private set; }
        public ServerSettingsWrapper ServerSettings { get; private set; }
        public GlobalSetting GlobalSetting { get; private set; }
        public Dictionary<ulong, StateTaskAbstract> InProgressStateTasks { get; private set; }
        public Dictionary<string, string> OAuthKeys { get; private set; }
        public DBAccessor DB { get; private set; }

        private bool _restartToken;

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
            InProgressStateTasks = new Dictionary<ulong, StateTaskAbstract>();
            OAuthKeys = new Dictionary<string, string>();
            ChannelPermission = new ChannelPermissionsWrapper();
            ServerSettings = new ServerSettingsWrapper();
            GlobalSetting = new GlobalSetting();
            DB = DBAccessor.GetAccessor();
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

            AddTask(new RemoveCustomTask());
            AddTask(new ServerWelcomeSettingsTask());
            AddTask(new ChangeCoolDownTask());
            AddTask(new ProxyPMTask());
            AddTask(new UpdateCustomTask());

            AddTask(new BotPrivacyTask());
            AddTask(new MigrateCommandTask());
            AddTask(new ComicGeneratorTask());
            AddTask(new EdgeDrawerTask());
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

        public void SignalRestart()
        {
            _restartToken = true;
        }

        public void DestorySelf()
        {
            Client.Disconnect();
        }

        public void AddStateTask(ulong userId, StateTaskAbstract task)
        {
            if (!InProgressStateTasks.ContainsKey(userId))
            {
                InProgressStateTasks.Add(userId, task);
            }
            else
            {
                InProgressStateTasks[userId] = task;
            }
        }

        public void RemoveStateTask(ulong userId)
        {
            if (InProgressStateTasks.ContainsKey(userId))
            {
                InProgressStateTasks.Remove(userId);
            }
        }

        private void SetUpDiscordClient()
        {
            _restartToken = false;

            Client = new DiscordClient();

            //Display all log messages in the console
            Client.Log.Message += DiscordMethods.LogMessage;

            Client.MessageReceived += DiscordMethods.OnMessageReceived;

            Client.UserJoined += DiscordMethods.OnUserJoinsServer;

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            Client.ExecuteAndWait(async () =>
            {
                //Connect to the Discord server using our email and password
                var username = ConfigurationManager.AppSettings[Constants.DISCORD_USERNAME_KEY];
                var password = ConfigurationManager.AppSettings[Constants.DISCORD_PASSWORD_KEY];
                await Client.Connect(username, password);
                Client.SetGame($"AWS EC2 - {Constants.APP_VERSION}");
            });

            Client = null;
            Tasks = null;
            InProgressStateTasks = null;
            ChannelPermission = null;
            ServerSettings = null;
            GlobalSetting = null;
            OAuthKeys = null;
            DB = null;

            if (_restartToken)
            {
                Initalize();
            }
        }
    }
}
