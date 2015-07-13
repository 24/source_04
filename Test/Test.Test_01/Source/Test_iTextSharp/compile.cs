toto
WebRun.CurrentWebRun.Compile_Project("xpdf_project.xml");
File.Copy(@"bin\xpdf.exe", @"c:\pib\prog\tools\xpdf.exe", true);
WebRun.CurrentWebRun.CompileAndRun_Project("xpdf_project.xml", true);
