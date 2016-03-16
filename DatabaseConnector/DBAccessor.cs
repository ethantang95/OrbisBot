using DatabaseConnector.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public class DBAccessor
    {
        //unfortunately we cannot use a dictionary because it will be ambiguous with types

        public EventDAO EventDAO { get; private set; }

        DBAccessor() { }

        public static DBAccessor GetAccessor()
        {
            var accessor = new DBAccessor();
            return accessor;
        }

        private void CreateDAOs()
        {
            EventDAO = new EventDAO(new DBConnector(Constants.DB_NAME));
        }
    }
}
