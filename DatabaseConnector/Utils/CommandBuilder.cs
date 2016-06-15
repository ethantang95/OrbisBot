using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Utils
{
    class CommandBuilder
    {
        public static SQLiteCommand AddParameters(Dictionary<string, Tuple<DbType, object>> parameters, SQLiteCommand command)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(new SQLiteParameter("@" + parameter.Key, parameter.Value.Item1));
                command.Parameters["@" + parameter.Key].Value = parameter.Value.Item2;
            }
            return command;
        }
    }
}
