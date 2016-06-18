using pb;
using pb.Compiler;
using pb.Data.Xml;

namespace pbc
{
    public static class Compile
    {
        private static string _runsourceSourceDirectory = null;

        public static void CompileFile(string file)
        {
            InitCompiler();
            string file2 = file.ToLowerInvariant();
            if (file2.EndsWith(".project.xml"))
                CompileProject(file);
            else if (file2.EndsWith(".projects.xml"))
                CompileProjects(file);
            else
                Trace.WriteLine("unknow file \"{0}\"", file);
        }

        public static void InitCompiler()
        {
            CompilerManager.Current.Init(XmlConfig.CurrentConfig.GetConfigElementExplicit("CompilerConfig"));
            CompilerManager.Current.AddCompiler("CSharp1", () => new CSharp1Compiler());
            CompilerManager.Current.AddCompiler("CSharp5", () => new CSharp5Compiler(CompilerManager.Current.FrameworkDirectories, CompilerManager.Current.MessageFilter));
            CompilerManager.Current.AddCompiler("JScript", () => new JScriptCompiler());
            //return new ResourceCompiler(CompilerManager.Current.ResourceCompiler);
            _runsourceSourceDirectory = XmlConfig.CurrentConfig.GetExplicit("RunsourceSourceDirectory");
        }

        public static void CompileProject(string projectFile)
        {
            //ResourceCompiler resourceCompiler = InitCompiler();
            Trace.WriteLine("Compile project \"{0}\"", projectFile);
            //ProjectCompiler projectCompiler = new ProjectCompiler(CompilerManager.Current.ResourceCompiler);
            ////compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), runCode: true);
            //CompilerProjectReader compilerProject = CompilerProjectReader.Create(new XmlConfig(projectFile).GetConfigElementExplicit("/AssemblyProject"));
            //projectCompiler.SetParameters(compilerProject);
            //projectCompiler.SetProjectCompilerFile(compilerProject.GetProjectCompilerFile());
            ProjectCompiler compiler = ProjectCompiler.Create(projectFile, CompilerManager.Current.Win32ResourceCompiler, CompilerManager.Current.ResourceCompiler);


            compiler.Compile();

            compiler.TraceMessages();

            //if (!compiler.HasError() && compiler.CopyRunSourceSourceFiles)
            //{
            //    string runsourceDirectory = GetRunSourceConfig().Get("UpdateRunSource/UpdateDirectory").zRootPath(zapp.GetEntryAssemblyDirectory());
            //    if (runsourceDirectory != null)
            //    {
            //        foreach (string directory in compiler.CopyOutputDirectories)
            //        {
            //            Trace.WriteLine("  copy runsource source files from \"{0}\" to \"{1}\"", runsourceDirectory, directory);
            //            foreach (string file in zDirectory.EnumerateFiles(runsourceDirectory, "*" + Compiler.ZipSourceFilename))
            //                zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
            //        }
            //    }
            //}

            string withError = !compiler.Success ? " with error(s)" : "";
            Trace.WriteLine($"  compiled{withError} : {compiler.OutputAssembly}");
        }

        public static void CompileProjects(string projectsFile)
        {
            Trace.WriteLine("Compile projects \"{0}\"", projectsFile);
            ProjectCompiler.CompileProjects(projectsFile, CompilerManager.Current.Win32ResourceCompiler, CompilerManager.Current.ResourceCompiler, _runsourceSourceDirectory,
                onCompiled: compiler => { compiler.TraceMessages(); });
        }
    }
}
