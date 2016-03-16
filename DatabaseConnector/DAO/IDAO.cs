using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.DAO
{
    public interface IDAO<T>
    {
        bool InsertObject(T obj);
        bool DeleteObject(long id);
        bool UpdateObject(T obj);
        T GetObjectById(long id);

    }
}
