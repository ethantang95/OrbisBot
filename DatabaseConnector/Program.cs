using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            Initalize();
            Console.ReadLine();
        }

        public static void Initalize() 
        {
            SQLiteConnection.CreateFile($"{Constants.DB_NAME}");
            var dbConnection = new SQLiteConnection($"Data Source={Constants.DB_NAME};Version=3;FailIfMissing=True;");
            dbConnection.Open();
            
        }
    }
}
  