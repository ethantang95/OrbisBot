using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    static class Constants
    {
        public const string DB_NAME = "OrbisBot.sqlite";
        public const string ORBIS_BOT_NAME = "OrbisBot";

        public const string EVENT_TABLE_NAME = "Events";

        //Events table
        public const string ID_COLUMN = "ID";
        public const string EVENT_NAME = "Name";
        public const string USERID_COLUMN = "UserID";
        public const string CHANNELID_COLUMN = "ChannelID";
        public const string SERVERID_COLUMN = "ServerID";
        public const string MESSAGE_COLUMN = "Message";
        public const string TARGET_USER_JSON_COLUMN = "TargetUserJSON";
        public const string TARGET_ROLE_COLUMN = "TargetRole";
        public const string TARGET_EVERYONE_COLUMN = "TargetEveryone";
        public const string NEXT_DISPATCH_COLUMN = "NextDispatch";
        public const string DISPATCH_DELAY_COLUMN = "DispatchDelay";
        public const string EVENT_TYPE_COlUMN = "EventType";
    }
}
