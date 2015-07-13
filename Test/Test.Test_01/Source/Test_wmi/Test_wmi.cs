Trace.WriteLine("toto");

Test.Test_wmi.Test_wmi_f.Test_wmi_01();
Test.Test_wmi.Test_wmi_f.TraceWmiNamespaces("root");
Test.Test_wmi.Test_wmi_f.TraceWmiClassNames("root");

Trace.WriteLine(pb.Trace.CurrentTrace.GetLogFile());
Trace.WriteLine(pb.Data.Xml.XmlConfig.CurrentConfig.ConfigPath);
Trace.WriteLine(pb.Data.Xml.XmlConfig.CurrentConfig.ConfigLocalPath);
