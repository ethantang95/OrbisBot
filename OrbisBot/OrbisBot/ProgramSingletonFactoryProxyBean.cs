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
using OrbisBot.GlobalSettings;
using DatabaseConnector;
using OrbisBot.TaskPermissions.PermissionBuilders;
using OrbisBot.TaskPermissions.Implmentations;
using OrbisBot.TaskPermissions;
using OrbisBot.Tasks.BotTasks;
using OrbisBot.Events;
using OrbisBot.Tasks.EventTasks;
using OrbisBot.Tasks.ServerTasks;
using OrbisBot.Tasks.FunTasks;

namespace OrbisBot
{
    class Context
    {

        public DiscordClient Client { get; private set; }
        public Dictionary<string, TaskAbstract> Tasks { get; private set; }
        public ChannelPermissionsWrapper ChannelPermission { get; private set; }
        public ServerSettingsWrapper ServerSettings { get; private set; }
        public GlobalSetting GlobalSetting { get; private set; }
        public Dictionary<ulong, MultiInputTaskAbstract> InProgressStateTasks { get; private set; }
        public Dictionary<string, string> OAuthKeys { get; private set; }
        public DBAccessor DB { get; private set; }
        public EventManager EventManager { get; private set; }

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
            InProgressStateTasks = new Dictionary<ulong, MultiInputTaskAbstract>();
            OAuthKeys = new Dictionary<string, string>();
            ChannelPermission = new ChannelPermissionsWrapper();
            ServerSettings = new ServerSettingsWrapper();
            GlobalSetting = new GlobalSetting();
            DB = DBAccessor.GetAccessor(FileHelper.GetProgramFolder());
            EventManager = new EventManager(DB.EventDAO);
            PopulateTaskDictionary();
            PopulateCustomTasks();
            SetUpDiscordClient();
        }

        private void PopulateTaskDictionary()
        {
            //BotAdminTasks
            AddTask(new ExceptionThrowingTask(CreateDiscretePermission(false, false, PermissionLevel.Developer, 1)));
            AddTask(new ProxyPMTask(new CommandChannelTaskPermissionBuilder().BuildPermission()));
            AddTask(new RestartContextTask(new CommandChannelTaskPermissionBuilder().BuildPermission()));

            //BotTasks
            AddTask(new AboutTask(CreateFilePermission(Constants.ABOUT_SETTINGS_FILE)));
            AddTask(new BotHelpTask(CreateFilePermission(Constants.HELP_SETTINGS_FILE)));
            AddTask(new BotJoinTask(CreateDiscretePermission(false, false, PermissionLevel.User, 30)));
            AddTask(new BotMentionTask(CreateFilePermission(Constants.BOT_MENTION_FILE)));
            AddTask(new BotMuteTask(CreateFilePermission(Constants.BOT_CONTROL_FILE, false, true, PermissionLevel.Admin, 1)));
            AddTask(new BotPrivacyTask(CreateDiscretePermission(false, true, PermissionLevel.RestrictedUser, 1)));
            AddTask(new ListCommandsTask(CreateFilePermission(Constants.LIST_COMMANDS_FILE, false, true, PermissionLevel.RestrictedUser, 10)));
            AddTask(new RegisterTask(CreateDiscretePermission(false, true, PermissionLevel.RestrictedUser, 1)));

            //CustomCommandTasks
            AddTask(new CreateCustomTask(CreateFilePermission(Constants.CUSTOM_COMMAND_FILE, false, false, PermissionLevel.Moderator, 1)));
            AddTask(new MigrateCommandTask(CreateFilePermission(Constants.CUSTOM_COMMAND_FILE, false, true, PermissionLevel.Moderator, 1)));
            AddTask(new RemoveCustomTask(CreateFilePermission(Constants.CUSTOM_COMMAND_FILE, false, true, PermissionLevel.Moderator, 1)));
            AddTask(new UpdateCustomTask(CreateFilePermission(Constants.CUSTOM_COMMAND_FILE, false, true, PermissionLevel.Moderator, 1)));

            //EventTasks
            AddTask(new CreateEventTask(CreateFilePermission(Constants.EVENTS_FILE, false, false, PermissionLevel.Moderator, 1)));
            AddTask(new DevRemoveEventTask(new CommandChannelTaskPermissionBuilder().BuildPermission()));
            AddTask(new RemoveEventTask(CreateFilePermission(Constants.EVENTS_FILE, false, false, PermissionLevel.Moderator, 1)));
            AddTask(new SearchEventTask(CreateFilePermission(Constants.EVENTS_FILE, false, false, PermissionLevel.Moderator, 1)));
            AddTask(new SkipEventTask(CreateFilePermission(Constants.EVENTS_FILE, false, false, PermissionLevel.Moderator, 1)));

            //FunTasks
            AddTask(new ComicGeneratorTask(CreateFilePermission(Constants.RANDOM_COMIC_FILE)));
            AddTask(new CutePictureTask(CreateFilePermission(Constants.CUTE_PICTURE_FILE)));
            AddTask(new EdgeDrawerTask(CreateFilePermission(Constants.EDGE_IMAGE_FILE)));
            AddTask(new FiftyShadeFicTask(CreateFilePermission(Constants.FIFTY_SHADES_FIC_FILE)));
            AddTask(new ImageFetchTask(CreateFilePermission(Constants.PICTURE_COMMAND_FILE)));
            AddTask(new InsultTask(CreateFilePermission(Constants.INSULT_FILE)));
            AddTask(new RedditTask(CreateFilePermission(Constants.REDDIT_FILE)));
            AddTask(new WolframAlphaTask(CreateFilePermission(Constants.WOLFRAMALPHA_SETTINGS_FILE)));

            //PermissionTasks
            AddTask(new AdjustUserPermissionTask(CreateFilePermission(Constants.ADJUST_PERMISSION_FILE, false, true, PermissionLevel.Moderator, 1)));
            AddTask(new AutoRoleAssignTask(CreateDiscretePermission(false, true, PermissionLevel.Owner, 1)));
            AddTask(new ChangeCommandPermissionTask(CreateFilePermission(Constants.CHANGE_COMMAND_PERMISSION_FILE, false, true, PermissionLevel.Admin, 1)));
            AddTask(new ChangeCoolDownTask(CreateFilePermission(Constants.CHANGE_COOL_DOWN_FILE, false, true, PermissionLevel.Moderator, 1)));

            //ServerTasks
            AddTask(new ChangeMainChannelTask(CreateFilePermission(Constants.CHANGE_MAIN_CHANNEL_FILE, false, true, PermissionLevel.Admin, 1)));
            AddTask(new ServerBanNotificationSettingsTask(CreateDiscretePermission(false, true, PermissionLevel.Admin, 1)));
            AddTask(new ServerGoodByeMsgSettingsTask(CreateDiscretePermission(false, true, PermissionLevel.Admin, 1)));
            AddTask(new ServerGoodByePmSettingsTask(CreateDiscretePermission(false, true, PermissionLevel.Admin, 1)));
            AddTask(new ServerSetTriggerCharTask(CreateDiscretePermission(false, false, PermissionLevel.Owner, 1)));
            AddTask(new ServerWelcomeSettingsTask(CreateDiscretePermission(false, true, PermissionLevel.Admin, 1)));

            //ToolTasks
            AddTask(new MentionRoleTask(CreateFilePermission(Constants.ROLE_MENTION_FILE, false, true, PermissionLevel.Moderator, 1)));
            AddTask(new RandomNumberTask(CreateFilePermission(Constants.RANDOM_COMMAND_FILE)));
            AddTask(new UserInfoTask(CreateFilePermission(Constants.META_INFO_FILE)));
        }

        private void PopulateCustomTasks()
        {
            var files = FileHelper.GetAllFiles(Constants.CUSTOM_COMMANDS_FOLDER);
            files.ForEach(s =>
            {
                var customTaskContents = CustomCommandFileHandler.GetCustomTaskFromFile(s);
                var permission = new RegisteredChannelTaskPermissionBuilder()
                        .SetSaver(new SaveCustomCommands())
                        .SetPermissions(customTaskContents)
                        .BuildPermission();
                
                var customTask = new CustomTask(customTaskContents[0].CommandName, customTaskContents, permission);
                AddTask(customTask);
            });
        }

        private FileBasedTaskPermission CreateFilePermission(string fileName, bool disabled = false, bool overrideMuting = false, PermissionLevel level = PermissionLevel.User, int cooldown = 30)
        {
            var builder = new FileBasedTaskPermissionBuilder();
            builder.SetFileSource(fileName)
                    .SetDisabled(disabled)
                    .SetOverrideMuting(overrideMuting)
                    .SetDefaultLevel(level)
                    .SetDefaultCooldown(cooldown);
            return builder.BuildPermission();
        }

        private DiscreteTaskPermission CreateDiscretePermission(bool disabled, bool overrideMuting, PermissionLevel level, int cooldown)
        {
            var builder = new DiscreteTaskPermissionBuilder();
            builder.SetDefaultLevel(level)
                    .SetDisabled(disabled)
                    .SetOverrideMuting(overrideMuting)
                    .SetDefaultLevel(level)
                    .SetDefaultCooldown(cooldown);
            return builder.BuildPermission();
        }

        public void AddTask(TaskAbstract toAdd)
        {
            Tasks.Add(toAdd.CommandText(), toAdd);

            foreach (var trigger in toAdd.AdditionalCommandTexts())
            {
                Tasks.Add(trigger, toAdd);
            }
        }

        public void SignalRestart()
        {
            _restartToken = true;
        }

        public void DestorySelf()
        {
            Client.Disconnect();
        }

        public void AddStateTask(ulong userId, MultiInputTaskAbstract task)
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

            Client.UserLeft += DiscordMethods.OnUserLeaveServer;

            Client.UserBanned += DiscordMethods.OnUserBanned;

            Client.UserUnbanned += DiscordMethods.OnUserUnBanned;

            //Convert our sync method to an async one and block the Main function until the bot disconnects
            try
            {
                Client.ExecuteAndWait(async () =>
                {
                //Connect to the Discord server using our token
#if DEBUG
                    var token = ConfigurationManager.AppSettings["DiscordDevLoginToken"];
#else
                    var token = ConfigurationManager.AppSettings[Constants.DISCORD_TOKEN_KEY];
#endif
                    await Client.Connect(token);
                    Client.SetGame($"AWS EC2 - {Constants.APP_VERSION}");
                    await Task.Delay(5000); //wait for 5 seconds to connect
                    EventManager.GetEvents();
                });
            }
            catch (Exception e)
            { 
                //loop it to restart... yeah this might not be a good idea
                Console.WriteLine($"An error has occurred {e.ToString()}");
                _restartToken = true;
            }

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
