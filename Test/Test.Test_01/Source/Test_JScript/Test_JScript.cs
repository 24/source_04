RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_dll\irunsource_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_dll\runsource_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource_domain\runsourced32_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"..\..\runsource\runsource\runsource32_project.xml");


RunSource.CurrentRunSource.Compile_Project(@"Test_JScript_01_js\Test_JScript_01_na_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"Test_JScript_assembly_attributes\Test_JScript_assembly_attributes_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"Test_JScript_01_exe\Test_JScript_01_exe_project.xml");

RunSource.CurrentRunSource.Compile_Project("magazine3k_project.xml");

AssemblyResolve.Trace = true;
Compiler.TraceLevel = 2;
AssemblyResolve.Trace = false;
Compiler.TraceLevel = 1;
Test_01();
Test_JScript_01();
Test_SetAssemblyEvent_01();
Test_UnsetAssemblyEvent_01();
