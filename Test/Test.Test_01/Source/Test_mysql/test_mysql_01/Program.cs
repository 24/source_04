using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace test_mysql_01
{
    class Program
    {
        static void Main(string[] args)
        {
            Test_mysql_01();
        }

        public static void Test_mysql_01()
        {
            string server = "localhost";
            string uid = "root";
            string password = "beuzserv";
            //string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            string connectionString = "SERVER=" + server + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            Console.WriteLine("mysql connection : server \"{0}\" uid \"{1}\"", server, uid);
            MySqlConnection connection = new MySqlConnection(connectionString);
        }
    }
}
