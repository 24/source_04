toto
wr.Compile_Project(@"..\PB_Library\PB_Library_project.xml");
wr.Compile_Project("Pib_project.xml");
wr.CompileAndRun_Project("Pib_project.xml", true);

wr.Compile_Project(@"..\Test\Test_Thread\Test_Thread2\Test_Thread_Windows_project.xml");
