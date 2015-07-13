Test_01();
Test_mysql_01();
Test_mysql_cmd_01("show tables");
AssemblyResolve.Trace = true;
AssemblyResolve.Clear();



Test_mysql_cmd_01("show databases");
Test_mysql_cmd_01("show tables");
Test_mysql_cmd_01("select * from tm_authors");
Test_mysql_cmd_01("");
Test_mysql_cmd_01("");
