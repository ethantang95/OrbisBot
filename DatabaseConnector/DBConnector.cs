using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace DatabaseConnector
{ 

    public class DBConnector
    {
        private SQLiteConnection _connection;
        private string _database;
        
        public DBConnector(string dbName)
        {
            _database = dbName;
        }

        //Initialize values
        private void Initialize()
        {
            string connectionString;
            if (!File.Exists(FileLocation(_database)))
            {
                SQLiteConnection.CreateFile(FileLocation(_database));
            }
            connectionString = $"Data Source={_database};Version=3;FailIfMissing=True;";
            _connection = new SQLiteConnection(connectionString);
        }

        public int ExecuteNonQuery(SQLiteCommand command)
        {
            command.Connection = _connection;
            _connection.Open();
            var result = command.ExecuteNonQuery();
            _connection.Close();
            return result;
        }

        public object ExecuteScalar(SQLiteCommand command)
        {
            command.Connection = _connection;
            _connection.Open();
            var result = command.ExecuteScalar();
            _connection.Close();
            return result;
        }

        public delegate List<T> ReaderParser<T>(SQLiteDataReader reader);
        public List<T> ExecuteAndGetReader<T>(SQLiteCommand command, ReaderParser<T> parser) 
        {
            command.Connection = _connection;
            _connection.Open(); ;
            var reader = command.ExecuteReader();
            var result = parser(reader);
            _connection.Close();
            return result;
        }

        public static string FileLocation(string fileName)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.ORBIS_BOT_NAME, fileName);
        }
    }
}
