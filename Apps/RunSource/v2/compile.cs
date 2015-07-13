Trace.WriteLine("toto");



RunSource.CurrentRunSource.Compile_Project(@"runsource.irunsource\runsource.irunsource.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"runsource.dll\runsource.dll.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"runsource.runsource\runsource.runsource.project.xml");
RunSource.CurrentRunSource.Compile_Project(@"runsource.launch\runsource.launch.project.xml");


pb.Data.Xml.XmlConfig config = new pb.Data.Xml.XmlConfig(@"c:\pib\prog\tools\runsource\exe\test.config.xml");
Trace.WriteLine(config.ConfigPath);
Trace.WriteLine(config.ConfigLocalPath);
Trace.WriteLine("value01 : \"{0}\"", config.Get("value01"));
foreach (string s in config.GetValues("tata/value02"))
	Trace.WriteLine("tata/value02 : \"{0}\"", s);
