using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.DAO
{
    abstract class DAOAbstract<T> : IDAO<T>
    {
        protected DBConnector _connection;
        public DAOAbstract(DBConnector connection) 
        {
            _connection = connection;
        }

        public abstract void CreateTable();
        public abstract bool DeleteObject(T obj);
        public abstract T GetObjectById(long id);
        public abstract bool InsertObject(T obj);
        public abstract bool UpdateObject(T obj);
    }
}
