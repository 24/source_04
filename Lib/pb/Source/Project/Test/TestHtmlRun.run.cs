Trace.WriteLine("toto");
pb.Compiler.RunSource.CurrentRunSource.CompileProject(@"$Root$\Lib\pb\Source\Project\HtmlRun.project.xml");

Trace.WriteLine(System.IO.Directory.GetCurrentDirectory());
HttpManager.CurrentHttpManager.ExportResult = false;
HttpManager.CurrentHttpManager.ExportResult = true;
HttpManager.CurrentHttpManager.ExportDirectory = @"c:\pib\dev_data\exe\runsource\log\http";
Trace.WriteLine("ExportResult {0} ExportDirectory \"{1}\"", HttpManager.CurrentHttpManager.ExportResult, HttpManager.CurrentHttpManager.ExportDirectory);
HttpRun.Load("https://www.google.fr");
HtmlRun.Select("//div:.:EmptyRow");

