using DatabaseConnector.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public class DBAccessor
    {
        //unfortunately we cannot use a dictionary because it will be ambiguous with types

        public EventDAO EventDAO { get; private set; }

        private DBConnector _dbConnector;

        DBAccessor(string path)
        {
            _dbConnector = new DBConnector(Path.Combine(path, Constants.DB_NAME));
            CreateDAOs();
        }

        public static DBAccessor GetAccessor(string path)
        {
            var accessor = new DBAccessor(path);
            return accessor;
        }

        private void CreateDAOs()
        {
            EventDAO = new EventDAO(_dbConnector);
        }
    }
}
