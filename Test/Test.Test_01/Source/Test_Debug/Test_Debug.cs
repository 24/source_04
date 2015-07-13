Trace.WriteLine("toto");
Test_01();
Test_Exception_01();
Compiler.TraceLevel = 1;
Compiler.TraceLevel = 2;
Test_StackTrace_01();


RunSource.CurrentRunSource.Compile_Project(@"exe\Test_Debug_01_project.xml");

