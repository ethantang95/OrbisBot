using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.DAO
{
    public abstract class DAOAbstract<T> : IDAO<T>
    {
        protected DBConnector _connection;
        public DAOAbstract(DBConnector connection) 
        {
            _connection = connection;
            InitalizeTable();
        }

        private void InitalizeTable()
        {
            var sql = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name ='{TableName()}'";
            var result = _connection.ExecuteScalar(new SQLiteCommand(sql));
            if (result == null)
            {
                CreateTable();
            }
        }

        public abstract void CreateTable();
        public abstract string TableName();
        public abstract bool DeleteObject(long id);
        public abstract T GetObjectById(long id);
        public abstract bool InsertObject(T obj);
        public abstract bool UpdateObject(T obj);
    }
}
