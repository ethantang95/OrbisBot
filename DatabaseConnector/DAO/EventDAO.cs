using DatabaseConnector.DAO;
using DatabaseConnector.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DatabaseConnector.DAO
{
    class EventDAO : DAOAbstract<EventModel>
    {
        public EventDAO(DBConnector connection) : base(connection)
        {
          
        }
        public override void CreateTable()
        {
            throw new NotImplementedException();
        }

        public override bool DeleteObject(EventModel obj)
        {
            throw new NotImplementedException();
        }

        public override EventModel GetObjectById(long id)
        {
            throw new NotImplementedException();
        }

        public override bool InsertObject(EventModel obj)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateObject(EventModel obj)
        {
            throw new NotImplementedException();
        }

    }
}
