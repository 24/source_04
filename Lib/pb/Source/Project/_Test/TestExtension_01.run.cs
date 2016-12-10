Trace.WriteLine("toto");
"toto".zTrace();
"toto".zView();
"toto".zView_v2();
"toto".zView_v3();
"toto".zViewType();
"$Root$".zGetRunSourceProjectVariableValue().zTrace();

HttpManager.CurrentHttpManager.ExportResult = false;
HttpManager.CurrentHttpManager.ExportResult = true;
HttpManager.CurrentHttpManager.ExportDirectory = @"c:\pib\dev_data\exe\runsource\log\http";
Trace.WriteLine("ExportResult {0} ExportDirectory \"{1}\"", HttpManager.CurrentHttpManager.ExportResult, HttpManager.CurrentHttpManager.ExportDirectory);
HttpRun.Load("https://www.google.fr");
HtmlRun.Select("//div:.:EmptyRow");

RunSource.CurrentRunSource.CompileProject(@"$Root$\Lib\pb\Source\Project\Extension_01.project.xml");
HtmlRun.Test();
HtmlRun.Test2();
System.AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.zGetAssemblyInfo()).zView_v2();

