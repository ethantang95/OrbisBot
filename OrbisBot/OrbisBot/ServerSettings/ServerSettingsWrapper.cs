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
        private Dictionary<ulong, ServerSetting> _serverSettings;

        public ServerSettingsWrapper()
        {
            _serverSettings = new Dictionary<ulong, ServerSetting>();
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

            _serverSettings.Add(serverId, serverSetting);
        }

        private void CheckAndCreateServer(ulong serverId)
        {
            if (!_serverSettings.ContainsKey(serverId))
            {
                var serverSetting = new ServerSetting(serverId);

                _serverSettings.Add(serverId, serverSetting);

                SaveServerID();
            }
        }

        private void SaveServerID()
        {
            FileHelper.WriteToFile(_serverSettings.Keys.Select(s => s.ToString()).ToList(), Constants.REGISTERED_SERVER_FILE);
        }

        public void SetWelcomeEnable(ulong serverId, bool enable)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.EnableWelcome = enable;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodbyeEnable(ulong serverId, bool enable)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.EnableGoodbyeMsgs = enable;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodByePmEnable(ulong serverId, bool enable)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.EnableGoodbyePms = enable;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetBanNotifEnable(ulong serverId, bool enable)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.EnableBanNotificaftion = enable;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetWelcomeMessage(ulong serverId, string message)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.WelcomeMsg = message;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodByeMessage(ulong serverId, string message)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.GoodbyeMsg = message;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetGoodByePm(ulong serverId, string message)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.GoodbyePms = message;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public void SetTriggerChar(ulong serverId, char triggerChar)
        {
            CheckAndCreateServer(serverId);
            var server = _serverSettings[serverId];
            server.TriggerChar = triggerChar;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }

        public ServerSetting GetServerSettings(ulong serverId)
        {
            if (_serverSettings.ContainsKey(serverId))
            {
                return _serverSettings[serverId];
            }
            else
            {
                return new ServerSetting(0);
            }
        }

        public string GetWelcomeMessage(ulong serverId)
        {
            CheckAndCreateServer(serverId);
            return _serverSettings[serverId].WelcomeMsg;
        }

        public string GetGoodbyeMessage(ulong serverId)
        {
            CheckAndCreateServer(serverId);
            return _serverSettings[serverId].GoodbyeMsg;
        }

        public string GetGoodbyePm(ulong serverId)
        {
            CheckAndCreateServer(serverId);
            return _serverSettings[serverId].GoodbyePms;
        }

        public char GetTriggerChar(ulong serverId)
        {
            CheckAndCreateServer(serverId);
            return _serverSettings[serverId].TriggerChar;
        }

        private void SaveServerSettings(ulong serverId)
        {
            var server = _serverSettings[serverId];
            FileHelper.WriteObjectToFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()), server);
        }
    }
}
