using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.DAO
{
    interface IDAO<T>
    {
        
        void CreateTable();
        bool InsertObject(T obj);
        bool DeleteObject(T obj);
        bool UpdateObject(T obj);
        T GetObjectById(long id);

    }
}
