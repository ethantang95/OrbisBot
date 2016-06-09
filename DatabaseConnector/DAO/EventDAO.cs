using DatabaseConnector.DAO;
using DatabaseConnector.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DatabaseConnector.Utils;

namespace DatabaseConnector.DAO
{
    public class EventDAO : DAOAbstract<EventModel>
    {
        public EventDAO(DBConnector connection) : base(connection)
        {
          
        }
        public override void CreateTable()
        {
            var sql = new StringBuilder($"CREATE TABLE IF NOT EXISTS {TableName()} (")
                .Append($"{Constants.ID_COLUMN} INTEGER PRIMARY KEY AUTOINCREMENT,")
                .Append($"{Constants.EVENT_NAME} VARCHAR(255),")
                .Append($"{Constants.USERID_COLUMN} BIGINT,")
                .Append($"{Constants.CHANNELID_COLUMN} BIGINT,")
                .Append($"{Constants.SERVERID_COLUMN} BIGINT,")
                .Append($"{Constants.MESSAGE_COLUMN} VARCHAR(2047),")
                .Append($"{Constants.TARGET_USER_JSON_COLUMN} VARCHAR(2047),")
                .Append($"{Constants.TARGET_ROLE_COLUMN} VARCHAR(255),")
                .Append($"{Constants.TARGET_EVERYONE_COLUMN} BOOLEAN,")
                .Append($"{Constants.NEXT_DISPATCH_COLUMN} BIGINT,")
                .Append($"{Constants.DISPATCH_DELAY_COLUMN} BIGINT,")
                .Append($"{Constants.EVENT_TYPE_COlUMN} VARCHAR(255));");

            _connection.ExecuteNonQuery(new SQLiteCommand(sql.ToString()));
        }

        public override bool DeleteObject(long id)
        {
            var sql = $"DELETE FROM {TableName()} WHERE {Constants.ID_COLUMN} = @{Constants.ID_COLUMN};";
            var sqlParams = CreateIDParam(id);
            var command = new SQLiteCommand(sql);
            command = CommandBuilder.AddParameters(sqlParams, command);
            var result = _connection.ExecuteNonQuery(command);
            return result > 0;
        }

        public override EventModel GetObjectById(long id)
        {
            var sql = $"SELECT * FROM {TableName()} WHERE {Constants.ID_COLUMN} = @{Constants.ID_COLUMN};";
            var sqlParam = CreateIDParam(id);
            var command = new SQLiteCommand(sql);
            command = CommandBuilder.AddParameters(sqlParam, command);
            var result = _connection.ExecuteReader(command, ReaderParser);
            return result.Count == 0 ? null : result[0];
        }

        public override bool InsertObject(EventModel obj)
        {
            var sql = new StringBuilder($"INSERT INTO {TableName()} (")
                .Append($"{Constants.EVENT_NAME}, ")
                .Append($"{Constants.USERID_COLUMN}, {Constants.CHANNELID_COLUMN}, ")
                .Append($"{Constants.SERVERID_COLUMN}, {Constants.MESSAGE_COLUMN}, ")
                .Append($"{Constants.TARGET_USER_JSON_COLUMN}, {Constants.TARGET_EVERYONE_COLUMN}, ")
                .Append($"{Constants.NEXT_DISPATCH_COLUMN}, {Constants.DISPATCH_DELAY_COLUMN}, ")
                .Append($"{Constants.EVENT_TYPE_COlUMN})")
                .Append($" VALUES (")
                .Append($"@{Constants.EVENT_NAME}, ")
                .Append($"@{Constants.USERID_COLUMN}, @{Constants.CHANNELID_COLUMN}, ")
                .Append($"@{Constants.SERVERID_COLUMN}, @{Constants.MESSAGE_COLUMN}, ")
                .Append($"@{Constants.TARGET_USER_JSON_COLUMN}, @{Constants.TARGET_EVERYONE_COLUMN}, ")
                .Append($"@{Constants.NEXT_DISPATCH_COLUMN}, @{Constants.DISPATCH_DELAY_COLUMN}, ")
                .Append($"@{Constants.EVENT_TYPE_COlUMN});");

            var sqlParams = new Dictionary<string, Tuple<DbType, object>>();
            sqlParams.Add(Constants.EVENT_NAME, new Tuple<DbType, object>(DbType.String, obj.Name));
            sqlParams.Add(Constants.USERID_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.UserID));
            sqlParams.Add(Constants.CHANNELID_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.ChannelID));
            sqlParams.Add(Constants.SERVERID_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.ServerID));
            sqlParams.Add(Constants.MESSAGE_COLUMN, new Tuple<DbType, object>(DbType.String, obj.Message));
            sqlParams.Add(Constants.TARGET_USER_JSON_COLUMN, new Tuple<DbType, object>(DbType.String, obj.TargetUsersJSON));
            sqlParams.Add(Constants.TARGET_EVERYONE_COLUMN, new Tuple<DbType, object>(DbType.Boolean, obj.TargetEveryone));
            sqlParams.Add(Constants.NEXT_DISPATCH_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.NextDispatch));
            sqlParams.Add(Constants.DISPATCH_DELAY_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.DispatchDelay));
            sqlParams.Add(Constants.EVENT_TYPE_COlUMN, new Tuple<DbType, object>(DbType.String, obj.EventType));

            var command = new SQLiteCommand(sql.ToString());
            command = CommandBuilder.AddParameters(sqlParams, command);

            var result = _connection.ExecuteNonQuery(command);

            return result > 0;
        }

        public override string TableName()
        {
            return Constants.EVENT_TABLE_NAME;
        }

        public override bool UpdateObject(EventModel obj)
        {
            

            var sql = new StringBuilder($"UPDATE {TableName()} SET ");
            var sqlParams = CreateIDParam(obj.ID);
            if (obj.Name != null && obj.Name != string.Empty)
            {
                sql.Append($"{Constants.EVENT_NAME} = @{Constants.EVENT_NAME},");
                sqlParams.Add(Constants.EVENT_NAME, new Tuple<DbType, object>(DbType.String, obj.Name));
            }
            if (obj.UserID != 0)
            {
                sql.Append($"{Constants.USERID_COLUMN} = @{Constants.USERID_COLUMN},");
                sqlParams.Add(Constants.USERID_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.UserID));
            }
            if (obj.ChannelID != 0)
            {
                sql.Append($"{Constants.CHANNELID_COLUMN} = @{Constants.CHANNELID_COLUMN},");
                sqlParams.Add(Constants.CHANNELID_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.ChannelID));
            }
            if (obj.ServerID != 0)
            {
                sql.Append($"{Constants.SERVERID_COLUMN} = @{Constants.SERVERID_COLUMN},");
                sqlParams.Add(Constants.SERVERID_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.ServerID));
            }
            if (obj.Message != null && obj.Message != string.Empty)
            {
                sql.Append($"{Constants.MESSAGE_COLUMN} = @{Constants.MESSAGE_COLUMN},");
                sqlParams.Add(Constants.MESSAGE_COLUMN, new Tuple<DbType, object>(DbType.String, obj.Message));
            }
            if (obj.TargetUsersJSON != null && obj.TargetUsersJSON != string.Empty)
            {
                sql.Append($"{Constants.TARGET_USER_JSON_COLUMN} = @{Constants.TARGET_USER_JSON_COLUMN},");
                sqlParams.Add(Constants.TARGET_USER_JSON_COLUMN, new Tuple<DbType, object>(DbType.String, obj.TargetUsersJSON));
            }
            if (obj.TargetEveryone != null)
            {
                sql.Append($"{Constants.TARGET_EVERYONE_COLUMN} = @{Constants.TARGET_EVERYONE_COLUMN},");
                sqlParams.Add(Constants.TARGET_EVERYONE_COLUMN, new Tuple<DbType, object>(DbType.String, obj.TargetEveryone));
            }
            if (obj.NextDispatch != 0)
            {
                sql.Append($"{Constants.NEXT_DISPATCH_COLUMN} = @{Constants.NEXT_DISPATCH_COLUMN},");
                sqlParams.Add(Constants.NEXT_DISPATCH_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.NextDispatch));
            }
            if (obj.DispatchDelay != 0)
            {
                sql.Append($"{Constants.DISPATCH_DELAY_COLUMN} = @{Constants.DISPATCH_DELAY_COLUMN},");
                sqlParams.Add(Constants.DISPATCH_DELAY_COLUMN, new Tuple<DbType, object>(DbType.Int64, obj.DispatchDelay));
            }
            if (obj.EventType != null && obj.EventType != string.Empty)
            {
                sql.Append($"{Constants.EVENT_TYPE_COlUMN} = @{Constants.EVENT_TYPE_COlUMN},");
                sqlParams.Add(Constants.EVENT_TYPE_COlUMN, new Tuple<DbType, object>(DbType.String, obj.EventType));
            }

            //return if there's nothing to be set
            if (sqlParams.Count == 0)
            {
                return false;
            }

            //removing trailing comma
            sql = new StringBuilder(sql.ToString().Remove(sql.ToString().Length-1));

            sql.Append($" WHERE {Constants.ID_COLUMN} = @{Constants.ID_COLUMN};");

            var command = new SQLiteCommand(sql.ToString());
            command = CommandBuilder.AddParameters(sqlParams, command);

            var result = _connection.ExecuteNonQuery(command);

            return result > 0;
            
        }

        public bool SetNextDispatch(long id, long delay)
        {
            var delay_name = "Delay";
            var sql = $"UPDATE {TableName()} SET {Constants.NEXT_DISPATCH_COLUMN} = {Constants.NEXT_DISPATCH_COLUMN} + @{delay_name} WHERE {Constants.ID_COLUMN} == @{Constants.ID_COLUMN};";

            var sqlParams = CreateIDParam(id);
            sqlParams.Add(delay_name, new Tuple<DbType, object>(DbType.Int64, delay));

            var command = new SQLiteCommand(sql);
            command = CommandBuilder.AddParameters(sqlParams, command);

            var result = _connection.ExecuteNonQuery(command);

            return result > 0;
        }

        public List<EventModel> FindObjectByName(string searchStr, long channelID)
        {
            var search_name = "Search";
            var sql = $"SELECT * FROM {Constants.EVENT_TABLE_NAME} WHERE {Constants.EVENT_NAME} LIKE @{search_name} AND {Constants.CHANNELID_COLUMN} == @{Constants.CHANNELID_COLUMN};";

            var sqlParams = new Dictionary<string, Tuple<DbType, object>>();
            sqlParams.Add(search_name, new Tuple<DbType, object>(DbType.String, $"%{searchStr}%"));
            sqlParams.Add(Constants.CHANNELID_COLUMN, new Tuple<DbType, object>(DbType.Int64, channelID));

            var command = new SQLiteCommand(sql);
            command = CommandBuilder.AddParameters(sqlParams, command);

            var result = _connection.ExecuteReader(command, ReaderParser);

            return result;
        }

        public List<EventModel> GetEventByTimeRange(long startTime, long endTime)
        {
            var start_name = "Start";
            var end_name = "end";

            var sql = $"SELECT * FROM {Constants.EVENT_TABLE_NAME} WHERE {Constants.NEXT_DISPATCH_COLUMN} >= @{start_name} AND {Constants.NEXT_DISPATCH_COLUMN} <= @{end_name};";

            var sqlParams = new Dictionary<string, Tuple<DbType, object>>();
            sqlParams.Add(start_name, new Tuple<DbType, object>(DbType.Int64, startTime));
            sqlParams.Add(end_name, new Tuple<DbType, object>(DbType.Int64, endTime));

            var command = new SQLiteCommand(sql);
            command = CommandBuilder.AddParameters(sqlParams, command);

            var result = _connection.ExecuteReader(command, ReaderParser);

            return result;
        }

        public bool UpdateEventMessage(long id, string message)
        {
            var model = new EventModel();
            model.ID = id;
            model.Message = message;
            model.TargetEveryone = null;
            return UpdateObject(model);
        }

        public bool UpdateTarget(long id, string targetUserJSON, bool targetEveryone)
        {
            var model = new EventModel();
            model.ID = id;
            model.TargetUsersJSON = targetUserJSON;
            model.TargetEveryone = targetEveryone;
            return UpdateObject(model);
        }

        public bool UpdateNextDispatch(long id, long nextDispatch)
        {
            var model = new EventModel();
            model.ID = id;
            model.NextDispatch = nextDispatch;
            model.TargetEveryone = null;
            return UpdateObject(model);
        }

        public bool UpdateDispatchDelay(long id, long dispatchDelay)
        {
            var model = new EventModel();
            model.ID = id;
            model.DispatchDelay = dispatchDelay;
            model.TargetEveryone = null;
            return UpdateObject(model);
        }

        private Dictionary<string, Tuple<DbType, object>> CreateIDParam(long id)
        {
            var sqlParams = new Dictionary<string, Tuple<DbType, object>>();
            sqlParams.Add(Constants.ID_COLUMN, new Tuple<DbType, object>(DbType.Int64, id));
            return sqlParams;
        }

        private List<EventModel> ReaderParser(SQLiteDataReader reader)
        {
            var toReturn = new List<EventModel>();

            while (reader.Read())
            {
                var model = new EventModel();
                model.ID = (long)reader[Constants.ID_COLUMN];
                model.Name = (string)reader[Constants.EVENT_NAME];
                model.UserID = (long)reader[Constants.USERID_COLUMN];
                model.ChannelID = (long)reader[Constants.CHANNELID_COLUMN];
                model.ServerID = (long)reader[Constants.SERVERID_COLUMN];
                model.Message = (string)reader[Constants.MESSAGE_COLUMN];
                model.TargetUsersJSON = (string)reader[Constants.TARGET_USER_JSON_COLUMN];
                model.TargetEveryone = (bool)reader[Constants.TARGET_EVERYONE_COLUMN];
                model.NextDispatch = (long)reader[Constants.NEXT_DISPATCH_COLUMN];
                model.DispatchDelay = (long)reader[Constants.DISPATCH_DELAY_COLUMN];
                model.EventType = (string)reader[Constants.EVENT_TYPE_COlUMN];

                toReturn.Add(model);
            }

            return toReturn;
        }
    }
}
