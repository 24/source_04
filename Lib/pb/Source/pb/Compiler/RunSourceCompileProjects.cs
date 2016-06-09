namespace pb.Compiler
{
    //public class RunSourceUpdateDirectory
    //{
    //    public string sourceDirectory;
    //    public string destinationDirectory;
    //}

    //class RunSourceUpdate
    //{
    //    public static IEnumerable<RunSourceUpdateDirectory> GetFromXml(string xml)
    //    {
    //        XDocument xdocument = XDocument.Parse(xml);
    //        foreach (XElement xe in xdocument.Root.zXPathElements("UpdateRunSource/Directory"))
    //        {
    //            yield return new RunSourceUpdateDirectory { sourceDirectory = xe.zAttribValue("source"), destinationDirectory = xe.zAttribValue("destination") };
    //        }
    //    }
    //}

    partial class RunSource
    {
        public bool CompileProjects(string projectsFile, string runsourceSourceDirectory = null)
        {
            return ProjectCompiler.CompileProjects(projectsFile, CompilerManager.Current.ResourceCompiler, runsourceSourceDirectory, onCompiled: compiler => { if (!compiler.Success) SetResult(compiler.GetCompilerMessagesDataTable()); });
        }

        //IEnumerable<XElement> projects
        //public bool CompileProjects(string projectsFile, string updateDirectory)
        //{
        //    Chrono chrono = new Chrono();
        //    chrono.Start();
        //    int nbProject = 0;
        //    try
        //    {
        //        //string updateDir = _config.GetExplicit("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());

        //        //foreach (XElement project in _config.GetElements("UpdateRunSource/Project"))
        //        foreach (XElement project in projects)
        //        {
        //            IProjectCompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
        //            //if (compiler.HasError())
        //            if (!compiler.Success)
        //                return false;
        //            string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
        //            if (copyOutput != null)
        //            {
        //                //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
        //                compiler.CopyResultFilesToDirectory(copyOutput);
        //            }
        //            if (project.zAttribValue("copyToUpdateDirectory").zTryParseAs(false))
        //            {
        //                //_trace.WriteLine("  copy result files to directory \"{0}\"", updateDir);
        //                compiler.CopyResultFilesToDirectory(updateDirectory);
        //            }
        //            nbProject++;
        //        }

        //        //foreach (XElement project in _config.GetElements("UpdateRunSource/ProjectRunSourceLaunch"))
        //        //{
        //        //    IProjectCompiler compiler = CompileProject(project.zExplicitAttribValue("value"));
        //        //    //if (compiler.HasError())
        //        //    if (!compiler.Success)
        //        //        return;
        //        //    string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
        //        //    if (copyOutput != null)
        //        //    {
        //        //        //_trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
        //        //        compiler.CopyResultFilesToDirectory(copyOutput);
        //        //    }
        //        //    nbProject++;
        //        //}
        //    }
        //    finally
        //    {
        //        chrono.Stop();
        //        Trace.WriteLine("{0} project(s) compiled", nbProject);
        //        Trace.WriteLine("Process completed {0}", chrono.TotalTimeString);
        //    }
        //    return true;
        //}
    }
}
