using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DatabaseConnector
{

    public class DBConnector
    {
        private SqlConnection _connection;
        private string _server = "";
        private string _database = "";
        private string _userid = "";
        private string _password = "";
        private int _timeout = 30;
        private bool _readerUsed = false;
        
        public DBConnector()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            string connectionString;
            connectionString = "user id=" + _userid + ";" +
                                       "password=" + _password + ";server=" + _server + ";" +
                                       "Trusted_Connection=false;Encrypt=True;Integrated Security=False;" +
                                       "database=" + _database + "; " +
                                       "connection timeout=" + _timeout;

            _connection = new SqlConnection(connectionString);
        }

        public int ExecuteNonQuery(SqlCommand command)
        {
            command.Connection = _connection;
            _connection.Open();
            var result = command.ExecuteNonQuery();
            _connection.Close();
            return result;
        }

        public object ExecuteScalar(SqlCommand command)
        {
            command.Connection = _connection;
            _connection.Open();
            var result = command.ExecuteScalar();
            _connection.Close();
            return result;
        }

        public SqlDataReader ExecuteAndGetReader(SqlCommand command) 
        {
            if (!_readerUsed)
            {
                var toReturn = new List<object>();
                command.Connection = _connection;
                _connection.Open(); ;
                var result = command.ExecuteReader();
                return result;
            }
            else 
            {
                throw new InvalidOperationException("reader has yet to be closed since last use");
            }
        }

        public void CloseConnection() 
        {
            if (_readerUsed)
            {
                _connection.Close();
            }
            _readerUsed = false;
        }
    }
}
