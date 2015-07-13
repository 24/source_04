RunSource.CurrentRunSource.Compile_Project(@"Test_wcf_01\Service\Test_wcf_01_service_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"Test_wcf_01\Client\Test_wcf_01_client_project.xml");

RunSource.CurrentRunSource.Compile_Project(@"Test_wcf_02\Service\Test_wcf_02_service_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"Test_wcf_02\Client\Test_wcf_02_client_project.xml");

RunSource.CurrentRunSource.Compile_Project(@"Test_wcf_03\Service\Test_wcf_03_service_dll_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"Test_wcf_03\Service\Test_wcf_03_service_exe_project.xml");
RunSource.CurrentRunSource.Compile_Project(@"Test_wcf_03\Client\Test_wcf_03_client_project.xml");


Test_01();
