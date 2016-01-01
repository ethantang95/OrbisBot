using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Utils
{
    class CommandBuilder
    {
        public static string InitInsertStatement(string table) 
        {
            return "INSERT INTO [" + table + "] VALUES ";
        }

        public static string InitUpdateStatement(string table)
        {
            return "UPDATE [" + table + "] SET ";
        }

        public static string InitSelectModelStatement(string table)
        {
            return "SELECT * FROM [" + table + "] ";
        }


        public static SqlCommand AddParameters(Dictionary<string, Tuple<SqlDbType, object>> parameters, SqlCommand command)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(new SqlParameter("@" + parameter.Key, parameter.Value.Item1));
                command.Parameters["@" + parameter.Key].Value = parameter.Value.Item2;
            }
            return command;
        }
    }
}
