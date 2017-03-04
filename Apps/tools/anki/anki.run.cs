// $$info.test.regex

//*************************************************************************************************************************
//****                                                  anki.project.xml
//*************************************************************************************************************************

RunSourceCommand.SetProjectFromSource();
//RunSourceCommand.SetProject(@"$Root$\Apps\tools\tv\_Test\Test_TV.project.xml");
Trace.WriteLine("toto");

RunSourceCommand.CompileProject("AnkiJS.project.xml");


QuestionRun.WriteAnkiCard("UE3 - Activité électrique cardiaque");
QuestionRun.WriteAnkiCard("UE3 - Equilibre acido-basique");
QuestionRun.WriteAnkiCard("UE3 - Etat de la matiere et leur caracterisaion Etat d'agregation");
QuestionRun.WriteAnkiCard("UE3 - Etat de la matiere et leur caracterisation");
QuestionRun.WriteAnkiCard("UE3 - Etat de la matiere et leur caracterisation Proprietes colligatives");
QuestionRun.WriteAnkiCard("UE3 - Gaz, eau et solutions aqueuses");
QuestionRun.WriteAnkiCard("UE3 - pH et equilibre acido-basique");

//QuestionRun.Exec(@"UE3\UE3 - pH et equilibre acido-basique");
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateAnkiFileFromScanFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateAnkiFileFromQuestionFiles();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").CreateResponseFile();
QuestionRun.CreateQuestionsManager(@"UE3\UE3 - pH et equilibre acido-basique").ExportResponse();



QuestionRun.ReadResponse("UE3 - Etat de la matiere et leur caracterisaion Etat d'agregation");
QuestionRun.ReadResponse("UE3 - Etat de la matiere et leur caracterisation");
QuestionRun.ReadResponse("UE3 - Etat de la matiere et leur caracterisation Proprietes colligatives");
QuestionRun.ReadResponse("UE3 - Gaz, eau et solutions aqueuses");
QuestionRun.ReadResponse("UE3 - pH et equilibre acido-basique");
QuestionRun.ExportResponse(@"");

QuestionTest.Test_Find_01("2016", QuestionRun.GetQuestionRegexValuesList()).zTraceJson();
QuestionTest.Test_Find_02("Q 24 : Q51 :  Q51 :  Q 60 : Q51 :", QuestionRun.GetResponseRegexValuesList());
QuestionTest.Test_FindInFile_01(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-012.txt", QuestionRun.GetQuestionRegexValuesList());
QuestionTest.Test_FindInFile_01(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-022.txt", QuestionRun.GetResponseRegexValuesList());

QuestionReader.Read(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-009.txt", QuestionRun.GetQuestionRegexValuesList()).zTraceJson();
QuestionReader.Read(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-012_02.txt", QuestionRun.GetQuestionRegexValuesList()).zTraceJson();
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Activité électrique cardiaque\scan\UE3 - Activité électrique cardiaque-page-022_02.txt", QuestionRun.GetResponseRegexValuesList()).zTraceJson();
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Activité électrique cardiaque\scan\UE3 - Activité électrique cardiaque-page-022_02.txt", QuestionRun.GetResponseRegexValuesList()).zSave(@"c:\pib\drive\google\valentin\UE3\UE3 - Activité électrique cardiaque\responses.json");
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Equilibre acido-basique\scan\UE3 - Equilibre acido-basique-page-016_02.txt", QuestionRun.GetResponseRegexValuesList()).zTraceJson();
ResponseReader.Read(@"c:\pib\drive\google\valentin\UE3\UE3 - Equilibre acido-basique\scan\UE3 - Equilibre acido-basique-page-016_02.txt", QuestionRun.GetResponseRegexValuesList()).zSave(@"c:\pib\drive\google\valentin\UE3\UE3 - Equilibre acido-basique\responses.json");
QuestionRun.WriteAnkiFile(@"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-009.anki.txt", @"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-009.txt", @"c:\pib\_dl\valentin\UE3 - Activité électrique cardiaque-page-022_02.txt");

QuestionRun.Test_01();
QuestionRun.Test_02();

//*************************************************************************************************************************
//****                                   VSProject
//*************************************************************************************************************************

// VSProjectUpdateOptions.AddSource | VSProjectUpdateOptions.RemoveSource | VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
// VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference
// VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate

// simulate no remove
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.Simulate);
// save vs project no remove
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.AddAssemblyReference);
// simulate
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference | VSProjectUpdateOptions.Simulate);
// save vs project
RunSourceVSProjectCommand.UpdateVSProject(options: VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.RemoveSourceLink
  | VSProjectUpdateOptions.AddAssemblyReference | VSProjectUpdateOptions.RemoveAssemblyReference);

// simulate - runsource.irunsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.irunsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml", options: VSProjectUpdateOptions.All);
// simulate - runsource.dll.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.dll.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
RunSourceVSProjectCommand.TraceVSProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.csproj");
RunSourceVSProjectCommand.TraceRunSourceProject(@"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml");
// simulate - runsource.command.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.command.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.project.xml", options: VSProjectUpdateOptions.All);
RunSourceVSProjectCommand.TraceVSProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.csproj");
RunSourceVSProjectCommand.TraceRunSourceProject(@"$Root$\Apps\RunSource\v2\runsource.command\runsource.command.project.xml");
// simulate - runsource.launch.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.launch\runsource.launch.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.launch.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.launch\runsource.launch.project.xml", options: VSProjectUpdateOptions.All);
// simulate - runsource.runsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.runsource\runsource.runsource.project.xml", options: VSProjectUpdateOptions.All | VSProjectUpdateOptions.Simulate);
// save vs project - runsource.runsource.project.xml
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Apps\RunSource\v2\runsource.runsource\runsource.runsource.project.xml", options: VSProjectUpdateOptions.All);


RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Lib\pb\Source\Project\XmlConfig.project.xml", VSProjectUpdateOptions.AddSourceLink | VSProjectUpdateOptions.Simulate);
RunSourceVSProjectCommand.UpdateVSProject(@"$Root$\Lib\pb\Source\Project\XmlConfig.project.xml", VSProjectUpdateOptions.AddSourceLink);
RunSourceVSProjectCommand.TraceVSProject();
RunSourceVSProjectCommand.TraceRunSourceProject();
RunSourceVSProjectCommand.Test_BackupVSProject();

VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts_test_08.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\WebData.project.xml".zGetRunSourceProjectPath());

VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\Basic.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Apps\RunSource\v2\runsource.irunsource\runsource.irunsource.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Apps\RunSource\v2\runsource.dll\runsource.dll.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\RunSourceExtension.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\MongoExtension.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\MongoDefaultSerializer.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\MongoWebHeaderSerializer.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\HtmlRun.project.xml".zGetRunSourceProjectPath());
VSProject.AddCompilerProject(@"$Root$\Apps\Damien\hts.csproj".zGetRunSourceProjectPath(), @"$Root$\Lib\pb\Source\Project\WebData.project.xml".zGetRunSourceProjectPath());

FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata.txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata(1).txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata[1].txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata_1.txt").zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata.txt").GetPath(2).zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata(1).txt").GetPath(2).zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata[1].txt").GetPath(2).zTraceJson();
FilenameNumberInfo.GetFilenameNumberInfo(@"c:\toto\tata_1.txt").GetPath(2).zTraceJson();

//*************************************************************************************************************************
//****                                   Test Project
//*************************************************************************************************************************

Trace.WriteLine("toto");
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_Basic.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RegexValues.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RegexValuesList.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_RunSourceExtension.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_MongoExtension.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_MongoSerializer.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_HttpRun.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_HtmlRun.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_Extension_01.dll.project.xml");
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\Project\_Test\Test_VSProject.project.xml");



using(FileStream fs = zFile.Open(@"c:\pib\_dl\test_01.txt", FileMode.Append, FileAccess.Write, FileShare.Read))
{
    fs.Write(new byte[] { 0x31, 0x32, 0x33, 0x34, 0x35 }, 0, 5);
}

//*************************************************************************************************************************
//****                                   $$info.test.regex
//*************************************************************************************************************************

Trace.WriteLine("toto");
Trace.WriteLine(RunSource.CurrentRunSource.ProjectFile);
RunSourceCommand.SetProjectFromSource();
RunSourceCommand.SetProject(@"$Root$\Lib\pb\Source\pb\Text\_Test\Test_Text.project.xml");

Test_Regex.Test(@"(?<=(?:^|\s))([a-e]{1,4})(?=(?:\s|$))", " abc ");

("aa" + ((char)0x2072)).zTrace();
("aa" + ((char)0x2082)).zTrace();
("aa" + ((char)0x207A)).zTrace();
("aa" + ((char)0x208A)).zTrace();


Test_RegexValues.Test2(QuestionRun.GetResponseRegexValuesList(), "Q 37 : D Q 26 : D Q 25 : E  Q 24 : C", contiguous: true);
Test_Regex.Test(@"\s*([a-e]{1,4})", "  AB ");


RunSourceCommand.CompileProject(@"$Root$\Lib\pb\Source\pb\NodeJS\_Test\Test_NodeJS_01.project.xml");
RunSourceCommand.CompileProject(@"$Root$\Lib\pb\Source\Project\NodeJSLib.project.xml");