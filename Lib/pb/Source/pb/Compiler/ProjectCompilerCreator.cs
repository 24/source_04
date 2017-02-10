using pb.Data.Xml;
using pb.IO;
using System;
using System.Xml.Linq;

namespace pb.Compiler
{
    partial class ProjectCompiler
    {
        public static ProjectCompiler Create(string projectFile, Win32ResourceCompiler win32ResourceCompiler, ResourceCompiler resourceCompiler = null)
        {
            ProjectCompiler compiler = new ProjectCompiler(win32ResourceCompiler, resourceCompiler);
            CompilerProjectReader projectReader = CompilerProjectReader.Create(new XmlConfig(projectFile).GetConfigElementExplicit("/AssemblyProject"));
            compiler.SetParameters(projectReader);
            //compiler.SetProjectCompilerFile(projectReader.GetProjectCompilerFile());
            return compiler;
        }

        public static bool CompileProjects(string projectsFile, Win32ResourceCompiler win32ResourceCompiler, ResourceCompiler resourceCompiler = null, string runsourceSourceDirectory = null, Action<IProjectCompiler> onCompiled = null)
        {
            Chrono chrono = new Chrono();
            chrono.Start();
            int nbProject = 0;
            try
            {
                if (!zFile.Exists(projectsFile))
                    throw new PBException("projects file dont exists \"{0}\"", projectsFile);
                XmlConfig projects = new XmlConfig(projectsFile);
                string projectsDirectory = zPath.GetDirectoryName(projectsFile);
                //string updateDir = _config.GetExplicit("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());

                string updateDirectory = projects.Get("UpdateDirectory");

                foreach (XElement project in projects.GetElements("Project"))
                {
                    string projectFile = project.zExplicitAttribValue("value").zRootPath(projectsDirectory);
                    Trace.WriteLine("Compile project \"{0}\"", projectFile);

                    ProjectCompiler compiler = ProjectCompiler.Create(projectFile, win32ResourceCompiler, resourceCompiler);
                    compiler.RunsourceSourceDirectory = runsourceSourceDirectory;
                    compiler.Compile();

                    //if (onCompiled != null)
                    //    onCompiled(compiler);
                    onCompiled?.Invoke(compiler);
                    if (!compiler.Success)
                        return false;
                    string copyOutput = project.zAttribValue("copyOutput").zRootPath(zapp.GetEntryAssemblyDirectory());
                    if (copyOutput != null)
                    {
                        //Trace.WriteLine("  copy result files to directory \"{0}\"", copyOutput);
                        compiler.CopyResultFilesToDirectory(copyOutput);
                    }
                    if (project.zAttribValue("copyToUpdateDirectory").zTryParseAs(false))
                    {
                        if (updateDirectory == null)
                            throw new PBException("update directory is not defined");
                        //Trace.WriteLine("  copy result files to directory \"{0}\"", updateDirectory);
                        compiler.CopyResultFilesToDirectory(updateDirectory);
                    }
                    nbProject++;
                }
            }
            catch (ProjectCompilerException ex)
            {
                Error.WriteMessage(ErrorOptions.TraceError, ex.Message);
            }
            finally
            {
                chrono.Stop();
                Trace.WriteLine("{0} project(s) compiled", nbProject);
                Trace.WriteLine("Process completed {0}", chrono.TotalTimeString);
            }
            return true;
        }
    }
}
