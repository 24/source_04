using pb;
using pb.Compiler;
using pb.Data.Xml;

namespace pbc
{
    public static class Compile
    {
        public static void CompileProject(string projectFile)
        {
            Trace.WriteLine("Compile project \"{0}\"", projectFile);
            Compiler compiler = new Compiler();
            //compiler.SetParameters(GetRunSourceConfigCompilerDefaultValues(), runCode: true);
            CompilerProject compilerProject = CompilerProject.Create(new XmlConfig(projectFile).GetConfigElementExplicit("/AssemblyProject"));
            compiler.SetParameters(compilerProject);
            compiler.SetProjectCompilerFile(compilerProject.GetProjectCompilerFile());

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

            string withError = compiler.HasError() ? " with error(s)" : "";
            Trace.WriteLine($"  compiled{withError} : {compiler.OutputAssembly}");
        }
    }
}
