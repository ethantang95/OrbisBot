using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.ServerSettings
{
    class ServerSettingsWrapper
    {
        public Dictionary<ulong, ServerSetting> ServerSettings { get; private set; }

        public ServerSettingsWrapper()
        {
            ServerSettings = new Dictionary<ulong, ServerSetting>();
            GetAllRegisteredServers();
        }

        private void GetAllRegisteredServers()
        {
            var servers = FileHelper.GetContentFromFile(Constants.REGISTERED_SERVER_FILE);
            if (servers != null)
            {
                servers.ForEach(s => SetServerSettings(UInt64.Parse(s)));
            }
        }

        private void SetServerSettings(ulong serverId)
        {
            var serverSetting = FileHelper.GetObjectFromFile<ServerSetting>(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()));

            if (serverSetting == null)
            {
                return;
            }

            ServerSettings.Add(serverId, serverSetting);
        }

        private void CheckAndCreateServer(ulong serverId)
        {
            if (!ServerSettings.ContainsKey(serverId))
            {
                var serverSetting = new ServerSetting(serverId, false, false, false, string.Empty, string.Empty, string.Empty);

                ServerSettings.Add(serverId, serverSetting);

                SaveServerID();
            }
        }

        private void SaveServerID()
        {
            FileHelper.WriteToFile(ServerSettings.Keys.Select(s => s.ToString()).ToList(), Constants.REGISTERED_SERVER_FILE);
        }

        public void SetWelcomeEnable(ulong serverId, bool enable)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.EnableWelcome = enable;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodbyeEnable(ulong serverId, bool enable)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.EnableGoodbyeMsgs = enable;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodByePmEnable(ulong serverId, bool enable)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.EnableGoodbyePms = enable;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetWelcomeMessage(ulong serverId, string message)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.WelcomeMsg = message;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodByeMessage(ulong serverId, string message)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.GoodbyeMsg = message;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodByePm(ulong serverId, string message)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.GoodbyePms = message;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public ServerSetting GetServerSettings(ulong serverId)
        {
            if (ServerSettings.ContainsKey(serverId))
            {
                return ServerSettings[serverId];
            }
            else
            {
                return new ServerSetting(0, false, false, false, string.Empty, string.Empty, string.Empty);
            }
        }

        public string GetWelcomeMessage(ulong serverId)
        {
            CheckAndCreateServer(serverId);
            return ServerSettings[serverId].WelcomeMsg;
        }

        public string GetGoodbyeMessage(ulong serverId)
        {
            CheckAndCreateServer(serverId);
            return ServerSettings[serverId].GoodbyeMsg;
        }

        public string GetGoodbyePm(ulong serverId)
        {
            CheckAndCreateServer(serverId);
            return ServerSettings[serverId].GoodbyePms;
        }

        private void SaveServerSettings(ulong serverId)
        {
            var server = ServerSettings[serverId];
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }
    }
}
