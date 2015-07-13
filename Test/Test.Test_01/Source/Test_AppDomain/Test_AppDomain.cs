toto
Test_01();
Test_AppDomain_01();
Test_AppDomain_02();
Test_AppDomain_03();
Test_AppDomain_04();

RunSource.CurrentDomainRunSource.Compile_Project(@"exe\Test_AppDomain_01_project.xml");
RunSource.CurrentDomainRunSource.Compile_Project(@"exe\Test_AppDomain_02_project.xml");
RunSource.CurrentDomainRunSource.Compile_Project(@"exe\Test_AppDomain_03_project.xml");
RunSource.CurrentDomainRunSource.Compile_Project(@"exe\Test_AppDomain_04_project.xml");
RunSource.CurrentDomainRunSource.Compile_Project(@"exe\Test_AppDomain_05_interface_dll_project.xml");
RunSource.CurrentDomainRunSource.Compile_Project(@"exe\Test_AppDomain_05_dll_project.xml");
RunSource.CurrentDomainRunSource.Compile_Project(@"exe\Test_AppDomain_05_project.xml");

