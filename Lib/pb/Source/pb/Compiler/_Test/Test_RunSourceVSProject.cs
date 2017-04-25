using pb.Data.VSProject;
using pb.Data.Xml;
using pb.IO;
using System.Collections.Generic;
using System.Linq;

namespace pb.Compiler.Test
{
    public static class Test_RunSourceVSProject
    {
        //public static void UpdateVSProject(string runsourceProject = null, string vsProject = null, VSProjectUpdateOptions options = VSProjectUpdateOptions.None)
        //public static void UpdateVSProject(string runsourceProject = null, VSProjectUpdateOptions options = VSProjectUpdateOptions.None)
        //{
        //    //if (runsourceProjects != null)
        //    //{
        //    //    //runsourceProject = RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(runsourceProject));
        //    //    List<string> projects = new List<string>();
        //    //    foreach (string runsourceProject in runsourceProjects)
        //    //        projects.Add(RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(runsourceProject)));
        //    //    runsourceProjects = projects;
        //    //}
        //    //else
        //    //    runsourceProjects = new string[] { RunSourceCommand.GetCurrentProject() };

        //    if (runsourceProject != null)
        //        runsourceProject = RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(runsourceProject));
        //    else
        //        runsourceProject = RunSourceCommand.GetCurrentProject();


        //    //string vsProject = GetVSProject(runsourceProject);
        //    //if (vsProject != null)
        //    //    vsProject = RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(vsProject));
        //    //else
        //    //    vsProject = GetVSProject(runsourceProjects.First());

        //    options |= VSProjectUpdateOptions.BackupVSProject;


        //    //RunSourceVSProjectManager.UpdateVSProject(vsProject, runsourceProjects, options);
        //    //RunSourceVSProjectManager.UpdateVSProject(runsourceProject, options);
        //    new RunSourceVSProjectManager().UpdateVSProject(runsourceProject, options);
        //}

        //public static void TraceRunSourceProject(string runsourceProject = null, bool traceProjectName = false)
        public static void TraceRunSourceProject(IEnumerable<string> runsourceProjects = null, bool traceProjectName = false)
        {
            if (runsourceProjects != null)
            {
                //runsourceProject = RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(runsourceProject));
                List<string> projects = new List<string>();
                foreach (string runsourceProject in runsourceProjects)
                    projects.Add(RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(runsourceProject)));
                runsourceProjects = projects;
            }
            else
                runsourceProjects = new string[] { RunSourceCommand.GetCurrentProject() };

            foreach (string runsourceProject in runsourceProjects)
            {
                if (!zFile.Exists(runsourceProject))
                {
                    Trace.WriteLine($"runsource project not found \"{runsourceProject}\"");
                    return;
                }
            }

            try
            {
                //RunSourceCommand.TraceDisableViewer();
                //RunSourceCommand.TraceSetWriter(WriteToFile.Create(zPath.Combine(zPath.GetDirectoryName(runsourceProject), zPath.GetFileNameWithoutExtension(runsourceProject) + ".trace.txt"), FileOption.RazFile), "trace");
                RunSourceCommand.TraceManager.DisableViewer();
                string project = runsourceProjects.First();
                RunSourceCommand.TraceManager.SetWriter(WriteToFile.Create(zPath.Combine(zPath.GetDirectoryName(project), zPath.GetFileNameWithoutExtension(project) + ".trace.txt"), FileOption.RazFile), "trace");

                //Trace.WriteLine($"runsource project \"{runsourceProject}\"");
                Trace.WriteLine($"runsource project {runsourceProjects.zToStringValues(runsourceProject => "\"" + zPath.GetFileName(runsourceProject) + "\"")}");

                //CompilerProjectReader runsourceProjectReader = CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject"));
                List<CompilerProjectReader> runsourceProjectsReaders = new List<CompilerProjectReader>();
                foreach (string runsourceProject in runsourceProjects)
                    runsourceProjectsReaders.Add(CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject")));

                Trace.WriteLine("  sources");
                foreach (RunSourceSource source in RunSourceVSProjectManager.GetRunSourceSources(runsourceProjectsReaders).OrderBy(source => source.RelativePath))
                {
                    //Trace.WriteLine($"    file \"{sourceLink.File}\" link \"{sourceLink.Link}\"");
                    //Trace.WriteLine($"    file \"{source.File}\" relative path \"{source.RelativePath}\" ({zPath.GetFileName(source.ProjectFile)})");
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\""),-140} ({zPath.GetFileName(source.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\"")}");
                }
                Trace.WriteLine("  sources links");
                foreach (RunSourceSource source in RunSourceVSProjectManager.GetRunSourceSourcesLinks(runsourceProjectsReaders).OrderBy(source => source.RelativePath))
                {
                    //Trace.WriteLine($"    file \"{sourceLink.File}\" link \"{sourceLink.Link}\"");
                    //Trace.WriteLine($"    file \"{source.File}\" relative path \"{source.RelativePath}\" ({zPath.GetFileName(source.ProjectFile)})");
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\""),-140} ({zPath.GetFileName(source.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\"")}");
                }
                Trace.WriteLine("  framework references");
                foreach (RunSourceReference reference in RunSourceVSProjectManager.GetRunSourceAssemblies(runsourceProjectsReaders).Where(reference => reference.FrameworkAssembly).OrderBy(reference => reference.Name))
                {
                    //Trace.WriteLine($"    file \"{assembly.File}\"{(assembly.FrameworkAssembly ? " (framework assembly)" : "")}{(assembly.RunSourceAssembly ? " (runsource assembly)" : "")} ({zPath.GetFileName(assembly.Project.ProjectFile)})");
                    //Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {(assembly.RelativePath != null ? "\"" + assembly.RelativePath + "\"" : ""),-140} {(assembly.FrameworkAssembly ? " (framework assembly)" : "")}{(assembly.RunSourceAssembly ? " (runsource assembly)" : "")} ({zPath.GetFileName(assembly.ProjectFile)})");
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + reference.Name + "\""),-70} ({zPath.GetFileName(reference.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + reference.Name + "\"")}");
                }
                Trace.WriteLine("  dll references");
                foreach (RunSourceReference assembly in RunSourceVSProjectManager.GetRunSourceAssemblies(runsourceProjectsReaders).Where(reference => !reference.FrameworkAssembly && !reference.RunSourceAssembly).OrderBy(reference => reference.Name))
                {
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\""),-140} ({zPath.GetFileName(assembly.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\"")}");
                }
                Trace.WriteLine("  runsource references");
                foreach (RunSourceReference assembly in RunSourceVSProjectManager.GetRunSourceAssemblies(runsourceProjectsReaders).Where(reference => reference.RunSourceAssembly).OrderBy(reference => reference.Name))
                {
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\""),-140} ({zPath.GetFileName(assembly.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\"")}");
                }
            }
            finally
            {
                //RunSourceCommand.TraceRemoveWriter("trace");
                //RunSourceCommand.TraceEnableViewer();
                RunSourceCommand.TraceManager.RemoveWriter("trace");
                RunSourceCommand.TraceManager.EnableViewer();
            }
        }

        public static void TraceVSProject(string vsProject = null)
        {
            if (vsProject != null)
                vsProject = RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(vsProject));
            else
                vsProject = GetVSProject(RunSourceCommand.GetCurrentProject());

            if (!zFile.Exists(vsProject))
            {
                Trace.WriteLine($"visual studio project not found \"{vsProject}\"");
                return;
            }

            try
            {
                //RunSourceCommand.TraceDisableViewer();
                ////RunSourceCommand.TraceSetWriter(WriteToFile.Create(zPath.Combine(zPath.GetDirectoryName(vsProject), zPath.GetFileName(vsProject) + ".trace.txt"), FileOption.RazFile), "trace");
                //RunSourceCommand.TraceSetWriter(WriteToFile.Create(vsProject + ".trace.txt", FileOption.RazFile), "trace");
                RunSourceCommand.TraceManager.DisableViewer();
                RunSourceCommand.TraceManager.SetWriter(WriteToFile.Create(vsProject + ".trace.txt", FileOption.RazFile), "trace");

                Trace.WriteLine($"visual studio project \"{vsProject}\"");
                VSProjectManager vsProjectManager = new VSProjectManager(vsProject);
                Trace.WriteLine("  sources");
                foreach (VSSource source in vsProjectManager.GetSources().Where(source => source.Link == null).OrderBy(source => source.File))
                {
                    //Trace.WriteLine($"    file \"{sourceLink.File}\" link \"{sourceLink.Link}\"");
                    //if (source.Link == null)
                    Trace.WriteLine($"    \"{source.File}\"");
                }
                Trace.WriteLine("  sources links");
                foreach (VSSource source in vsProjectManager.GetSources().Where(source => source.Link != null).OrderBy(source => source.File))
                {
                    //Trace.WriteLine($"    file \"{sourceLink.File}\" link \"{sourceLink.Link}\"");
                    //if (source.Link != null)
                    Trace.WriteLine($"    \"{source.File}\"");
                }
                Trace.WriteLine("  framework references");
                foreach (VSReference reference in vsProjectManager.GetReferences().Where(reference => reference.File == null).OrderBy(reference => reference.Name))
                {
                    Trace.WriteLine($"    \"{reference.Name}\"");
                }
                Trace.WriteLine("  dll references");
                foreach (VSReference reference in vsProjectManager.GetReferences().Where(reference => reference.File != null && !reference.ProjectReference).OrderBy(reference => reference.Name))
                {
                    Trace.WriteLine($"    {("\"" + reference.Name + "\""),-70} \"{reference.File}\"");
                    //if (reference.File == null)
                    //    Trace.WriteLine($"    \"{reference.Name}\"");
                    //else if (reference.ProjectReference)
                    //    //Trace.WriteLine($"    \"{reference.Name}\" \"{reference.File}\" project \"{reference.ProjectReference}\"");
                    //    Trace.WriteLine($"    \"{reference.Name,-70}\" \"{reference.File}\"");
                    //else
                    //    Trace.WriteLine($"    \"{reference.Name,-70}\" \"{reference.File}\"");
                }
                Trace.WriteLine("  project references");
                foreach (VSReference reference in vsProjectManager.GetReferences().Where(reference => reference.ProjectReference).OrderBy(reference => reference.Name))
                {
                    Trace.WriteLine($"    {("\"" + reference.Name + "\""),-70} \"{reference.File}\"");
                }
            }
            finally
            {
                //RunSourceCommand.TraceRemoveWriter("trace");
                //RunSourceCommand.TraceEnableViewer();
                RunSourceCommand.TraceManager.RemoveWriter("trace");
                RunSourceCommand.TraceManager.EnableViewer();
            }
        }

        public static void Test_BackupVSProject()
        {
            string project = RunSourceCommand.GetCurrentProject();
            Trace.WriteLine($"current project \"{project}\"");
            string vsProject = GetVSProject(project);
            Trace.WriteLine($"visual studio project \"{vsProject}\"");
            if (!zFile.Exists(vsProject))
            {
                Trace.WriteLine($"visual studio project not found \"{vsProject}\"");
                return;
            }
            // tv.project.xml tv.csproj
            RunSourceVSProjectManager.Test_BackupVSProject(vsProject);
        }

        //private static string GetVSProject(string runsourceProject)
        //{
        //    string vsProject = zPath.GetFileNameWithoutExtension(runsourceProject);
        //    if (vsProject.EndsWith(".project"))
        //        vsProject = vsProject.Substring(0, vsProject.Length - 8);
        //    vsProject += ".csproj";
        //    return zPath.Combine(zPath.GetDirectoryName(runsourceProject), vsProject);
        //}

        private static string GetVSProject(string runsourceProject)
        {
            return CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject")).GetVSProject();
        }
    }
}
