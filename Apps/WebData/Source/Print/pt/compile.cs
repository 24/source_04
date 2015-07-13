toto
RunSource.CurrentRunSource.Compile_Project("pt_project.xml");

File.Copy(@"bin\pt.exe", @"c:\pib\prog\tools\pt.exe", true);
File.Copy(@"bin\pt.pdb", @"c:\pib\prog\tools\pt.pdb", true);
File.SetAttributes(@"bin\pt_config.xml", FileAttributes.Archive);
File.Copy(@"pt_config.xml", @"bin\pt_config.xml", true);
File.SetAttributes(@"c:\pib\prog\tools\pt_config.xml", FileAttributes.Archive);
File.Copy(@"pt_config.xml", @"c:\pib\prog\tools\pt_config.xml", true);
