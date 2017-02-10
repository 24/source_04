﻿using pb.Data.VSProject;
using pb.Data.Xml;
using pb.IO;
using System.Linq;

namespace pb.Compiler
{
    public static class RunSourceVSProjectCommand
    {
        public static void UpdateVSProject(string runsourceProject = null, VSProjectUpdateOptions options = VSProjectUpdateOptions.AddSourceLink)
        {
            string currentRunsourceProject = RunSourceCommand.GetCurrentProject();

            if (runsourceProject != null)
                runsourceProject = RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(runsourceProject));
            else
                runsourceProject = currentRunsourceProject;

            string vsProject = GetVSProject(runsourceProject);
            if (!zFile.Exists(vsProject))
            {
                Trace.WriteLine($"visual studio project not found \"{vsProject}\"");
                return;
            }

            options |= VSProjectUpdateOptions.BackupVSProject;

            string label = "";
            if ((options & VSProjectUpdateOptions.Simulate) == VSProjectUpdateOptions.Simulate)
                label = "simulate";
            else
                label = "backup";
            Trace.WriteLine($"update visual studio project ({label}) \"{vsProject}\"");
            Trace.WriteLine($"  from runsource project \"{runsourceProject}\"");
            //Trace.WriteLine($"  backup visual studio project");
            string labelRemove = "";
            if ((options & VSProjectUpdateOptions.RemoveSource) == VSProjectUpdateOptions.RemoveSource)
                labelRemove = "source";
            if ((options & VSProjectUpdateOptions.RemoveSourceLink) == VSProjectUpdateOptions.RemoveSourceLink)
                labelRemove += (labelRemove != "" ? ", " : "") + "source link";
            if ((options & VSProjectUpdateOptions.RemoveAssemblyReference) == VSProjectUpdateOptions.RemoveAssemblyReference)
                labelRemove += (labelRemove != "" ? ", " : "") + "assembly reference";
            if (labelRemove != "")
                labelRemove = "remove " + labelRemove;
            //Trace.WriteLine($"  remove {label}");
            string labelAdd = "";
            if ((options & VSProjectUpdateOptions.AddSourceLink) == VSProjectUpdateOptions.AddSourceLink)
                labelAdd = "source";
            if ((options & VSProjectUpdateOptions.AddSourceLink) == VSProjectUpdateOptions.AddSourceLink)
                labelAdd += (labelAdd != "" ? ", " : "") + "source link";
            if ((options & VSProjectUpdateOptions.AddAssemblyReference) == VSProjectUpdateOptions.AddAssemblyReference)
                labelAdd += (labelAdd != "" ? ", " : "") + "assembly reference";
            //Trace.WriteLine($"  add {label}");
            if (labelAdd != "")
                labelAdd = "add " + labelAdd;
            label = labelRemove;
            if (labelAdd != "")
                label += (label != "" ? ", " : "") + labelAdd;
            Trace.WriteLine($"  operations : {label}");

            VSProjectUpdateResult result = RunSourceVSProjectManager.UpdateVSProject(vsProject, runsourceProject, options);

            label = "";
            if ((options & VSProjectUpdateOptions.RemoveSource) == VSProjectUpdateOptions.RemoveSource)
                label = $"{result.SourceRemovedCount} source removed";
            if ((options & VSProjectUpdateOptions.AddSource) == VSProjectUpdateOptions.AddSource)
                label += $"{(label != "" ? ", " : "")}{result.SourceAddedCount} source added";
            if ((options & VSProjectUpdateOptions.RemoveSourceLink) == VSProjectUpdateOptions.RemoveSourceLink)
                label += $"{(label != "" ? ", " : "")}{result.SourceLinkRemovedCount} source link removed";
            if ((options & VSProjectUpdateOptions.AddSourceLink) == VSProjectUpdateOptions.AddSourceLink)
                label += $"{(label != "" ? ", " : "")}{result.SourceLinkAddedCount} source link added";
            if ((options & VSProjectUpdateOptions.RemoveAssemblyReference) == VSProjectUpdateOptions.RemoveAssemblyReference)
                label += $"{(label != "" ? ", " : "")}{result.AssemblyReferenceRemovedCount} assembly reference removed";
            if ((options & VSProjectUpdateOptions.AddAssemblyReference) == VSProjectUpdateOptions.AddAssemblyReference)
                label += $"{(label != "" ? ", " : "")}{result.AssemblyReferenceAddedCount} assembly reference added";
            if (label != "")
                Trace.WriteLine($"  {label}");
        }

        public static void TraceRunSourceProject(string runsourceProject = null, bool traceProjectName = false)
        {
            if (runsourceProject != null)
                runsourceProject = RunSourceCommand.GetFilePath(RunSourceCommand.GetProjectVariableValue(runsourceProject));
            else
                runsourceProject = RunSourceCommand.GetCurrentProject();

            if (!zFile.Exists(runsourceProject))
            {
                Trace.WriteLine($"runsource project not found \"{runsourceProject}\"");
                return;
            }

            try
            {
                RunSourceCommand.TraceDisableViewer();
                RunSourceCommand.TraceSetWriter(WriteToFile.Create(zPath.Combine(zPath.GetDirectoryName(runsourceProject), zPath.GetFileNameWithoutExtension(runsourceProject) + ".trace.txt"), FileOption.RazFile), "trace");

                Trace.WriteLine($"runsource project \"{runsourceProject}\"");
                CompilerProjectReader runsourceProjectReader = CompilerProjectReader.Create(new XmlConfig(runsourceProject).GetConfigElement("/AssemblyProject"));
                Trace.WriteLine("  sources");
                //foreach (CompilerFile source in runsourceProjectReader.GetSources(recurse: true, withoutExtensionProject: true))
                foreach (RunSourceSource source in RunSourceVSProjectManager.GetRunSourceSources(runsourceProjectReader).OrderBy(source => source.RelativePath))
                {
                    //Trace.WriteLine($"    file \"{sourceLink.File}\" link \"{sourceLink.Link}\"");
                    //Trace.WriteLine($"    file \"{source.File}\" relative path \"{source.RelativePath}\" ({zPath.GetFileName(source.ProjectFile)})");
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\""),-140} ({zPath.GetFileName(source.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\"")}");
                }
                Trace.WriteLine("  sources links");
                //foreach (CompilerFile source in runsourceProjectReader.GetSourcesLinks(recurse: true, withoutExtensionProject: true))
                foreach (RunSourceSource source in RunSourceVSProjectManager.GetRunSourceSourcesLinks(runsourceProjectReader).OrderBy(source => source.RelativePath))
                {
                    //Trace.WriteLine($"    file \"{sourceLink.File}\" link \"{sourceLink.Link}\"");
                    //Trace.WriteLine($"    file \"{source.File}\" relative path \"{source.RelativePath}\" ({zPath.GetFileName(source.ProjectFile)})");
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\""),-140} ({zPath.GetFileName(source.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + source.RelativePath + "\"")}");
                }
                Trace.WriteLine("  framework references");
                //foreach (CompilerAssembly assembly in runsourceProjectReader.GetAssemblies(recurse: true, withoutExtensionProject: true))
                foreach (RunSourceReference reference in RunSourceVSProjectManager.GetRunSourceAssemblies(runsourceProjectReader).Where(reference => reference.FrameworkAssembly).OrderBy(reference => reference.Name))
                {
                    //Trace.WriteLine($"    file \"{assembly.File}\"{(assembly.FrameworkAssembly ? " (framework assembly)" : "")}{(assembly.RunSourceAssembly ? " (runsource assembly)" : "")} ({zPath.GetFileName(assembly.Project.ProjectFile)})");
                    //Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {(assembly.RelativePath != null ? "\"" + assembly.RelativePath + "\"" : ""),-140} {(assembly.FrameworkAssembly ? " (framework assembly)" : "")}{(assembly.RunSourceAssembly ? " (runsource assembly)" : "")} ({zPath.GetFileName(assembly.ProjectFile)})");
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + reference.Name + "\""),-70} ({zPath.GetFileName(reference.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + reference.Name + "\"")}");
                }
                Trace.WriteLine("  dll references");
                foreach (RunSourceReference assembly in RunSourceVSProjectManager.GetRunSourceAssemblies(runsourceProjectReader).Where(reference => !reference.FrameworkAssembly && !reference.RunSourceAssembly).OrderBy(reference => reference.Name))
                {
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\""),-140} ({zPath.GetFileName(assembly.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\"")}");
                }
                Trace.WriteLine("  runsource references");
                foreach (RunSourceReference assembly in RunSourceVSProjectManager.GetRunSourceAssemblies(runsourceProjectReader).Where(reference => reference.RunSourceAssembly).OrderBy(reference => reference.Name))
                {
                    if (traceProjectName)
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\""),-140} ({zPath.GetFileName(assembly.ProjectFile)})");
                    else
                        Trace.WriteLine($"    {("\"" + assembly.Name + "\""),-70} {("\"" + assembly.RelativePath + "\"")}");
                }
            }
            finally
            {
                RunSourceCommand.TraceRemoveWriter("trace");
                RunSourceCommand.TraceEnableViewer();
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
                RunSourceCommand.TraceDisableViewer();
                //RunSourceCommand.TraceSetWriter(WriteToFile.Create(zPath.Combine(zPath.GetDirectoryName(vsProject), zPath.GetFileName(vsProject) + ".trace.txt"), FileOption.RazFile), "trace");
                RunSourceCommand.TraceSetWriter(WriteToFile.Create(vsProject + ".trace.txt", FileOption.RazFile), "trace");

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
                RunSourceCommand.TraceRemoveWriter("trace");
                RunSourceCommand.TraceEnableViewer();
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

        private static string GetVSProject(string runsourceProject)
        {
            string vsProject = zPath.GetFileNameWithoutExtension(runsourceProject);
            if (vsProject.EndsWith(".project"))
                vsProject = vsProject.Substring(0, vsProject.Length - 8);
            vsProject += ".csproj";
            return zPath.Combine(zPath.GetDirectoryName(runsourceProject), vsProject);
        }
    }
}
