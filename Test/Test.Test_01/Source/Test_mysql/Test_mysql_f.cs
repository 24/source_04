using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using pb;
using pb.Compiler;

namespace Test_mysql
{
    static partial class w
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _rs = RunSource.CurrentRunSource;

        public static void Init()
        {
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static MySqlConnection GetMySqlConnection()
        {
            string server = "localhost";
            string uid = "root";
            string password = "beuzserv";
            //string database = null;
            string database = "tm";
            //string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            string connectionString = "SERVER=" + server + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            if (database != null)
                connectionString += "DATABASE=" + database + ";";
            _tr.WriteLine("mysql connection : server \"{0}\" uid \"{1}\" database \"{2}\" connection string \"{3}\"", server, uid, database, connectionString);
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }

        public static void Test_mysql_01()
        {
            _tr.WriteLine("Test_mysql_01");
            string server = "localhost";
            string uid = "root";
            string password = "beuzserv";
            string database = null;
            //string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            string connectionString = "SERVER=" + server + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            if (database != null)
                connectionString += "DATABASE=" + database + ";";
            _tr.WriteLine("mysql connection : server \"{0}\" uid \"{1}\" database \"{2}\" connection string \"{3}\"", server, uid, database, connectionString);
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            connection.Close();
        }

        public static void Test_mysql_cmd_01(string sql)
        {
            _tr.WriteLine("Test_mysql_02");
            using (MySqlConnection connection = GetMySqlConnection())
            {
                //string sql = "show databases";
                //string sql = "show tables";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                _rs.SetResult(dt);
            }
        }

    }
}
