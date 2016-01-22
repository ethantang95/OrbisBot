using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot
{
    class Constants
    {
        public const string ORBIS_BOT_NAME = "OrbisBot";
        public const string COMMAND_DEFAULT = "Default";
        public const string COMMAND_DISABLED = "Disabled";
        public const string OPTIONS_FOLDER = "Options";
        public const string CHANNELS_OPTIONS_FOLDER = "ChannelsOption";
        public const string SERVER_OPTIONS_FOLDER = "ServerOption";
        public const string PROFILES_FOLDER = "Profile";
        public const string CUSTOM_COMMANDS_FOLDER = "CustomCommands";
        public const string REGISTERED_CHANNEL_FILE = "RegisteredChannel.txt";
        public const string REGISTERED_SERVER_FILE = "RegisteredServer.txt";

        public const string MAIN_CHANNEL_ID = "MainChannelID";
        public const string CHANNEL_ID = "ChannelID";
        public const string SERVER_ID = "ServerID";
        public const string CHANNEL_MUTED = "Muted";
        public const string COMMAND_OVERRIDE = "Override";
        public const string ENABLE_WELCOME = "EnableWelcome";
        public const string WELCOME_MSG = "WelcomeMSG";

        public const string COMMAND_NAME = "CommandName";
        public const string MAX_ARGS = "MaxArgs";
        public const string PERMISSION_LEVEL = "PermissionLevel";
        public const string RETURN_TEXT = "ReturnText";

        //permission setting files
        public const string WOLFRAMALPHA_SETTINGS_FILE = "WolframAlphaSettings.txt";
        public const string ABOUT_SETTINGS_FILE = "OrbisAbout.txt";
        public const string ADJUST_PERMISSION_FILE = "AdjustPermission.txt";
        public const string REGISTER_SELF_FILE = "RegisterFile.txt";
        public const string CHANGE_COMMAND_PERMISSION_FILE = "ChangeCommandPermission.txt";
        public const string LIST_COMMANDS_FILE = "ListCommands.txt";
        public const string META_INFO_FILE = "MetaInfo.txt";
        public const string CUSTOM_COMMAND_FILE = "CustomCommand.txt";
        public const string RANDOM_COMMAND_FILE = "RandomCommand.txt";
        public const string CHANGE_MAIN_CHANNEL_FILE = "ChangeMainChannel.txt";
        public const string BOT_CONTROL_FILE = "MuteBot.txt"; //mute and unmute will use the same file
        public const string CUTE_PICTURE_FILE = "CutePicture.txt";
        public const string REDDIT_FILE = "Reddit.txt";
        public const string EXCEPTION_TEST_FILE = "ExceptionTest.txt";
        public const string BOT_MENTION_FILE = "BotMention.txt";
        public const string FIFTY_SHADES_FIC_FILE = "FiftyShadesFic.txt";
        public const string INSULT_FILE = "Insult.txt";
        public const string ROLE_MENTION_FILE = "RoleMention.txt";

        public const string DISCORD_USERNAME_KEY = "DiscordLoginUserName";
        public const string DISCORD_PASSWORD_KEY = "DiscordLoginPassword";
        public const string IMGUR_CLIENT_ID = "ImgurClientID";
        public const string IMGUR_CLIENT_SECRET = "ImgurClientSecret";
        public const string COMMAND_CHANNEL = "CommandChannel";
        public const string DEVELOPERS = "Developers";

        public const string APP_VERSION = "0.2.0";

        public const string USAGE_INTRO = "This command's sytnax is:";
        public const string NO_PARAMS_USAGE = "This task takes in no additional parameters";

        public const string DISCORD_API_ENDPOINT = "https://discordapp.com/api/";

        public const string TRIGGER_CHAR = "-";
    }
}
