using System;
using System.Collections.Generic;
using pb.IO;

namespace pb.Compiler
{
    public class CompilerProviderOption
    {
        public string Name;
        public string Value;
    }

    public class CompilerFile
    {
        public string File = null;
        public string RelativePath = null;
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();
        public ICompilerProject Project = null;

        public CompilerFile(string file, string rootDirectory = null)
        {
            File = file;
            if (rootDirectory != null && file.StartsWith(rootDirectory))
            {
                RelativePath = file.Substring(rootDirectory.Length);
                if (RelativePath.StartsWith("\\"))
                    RelativePath = RelativePath.Substring(1);
            }
            else
                RelativePath = zPath.Combine("NoRoot", zPath.GetFileName(file));
            //Trace.WriteLine("  CompilerFile.File         : \"{0}\"", File);
            //Trace.WriteLine("  CompilerFile.RelativePath : \"{0}\"", RelativePath);
        }
    }

    public class CompilerAssembly
    {
        public string File = null;
        public bool Resolve = false;
        public string ResolveName = null;
        //public bool CopySource = false;
        public ICompilerProject Project = null;

        // bool copySource = false
        public CompilerAssembly(string file, bool resolve = false, string resolveName = null, ICompilerProject project = null)
        {
            File = file;
            Resolve = resolve;
            ResolveName = resolveName;
            //CopySource = copySource;
            Project = project;
        }
    }

    public interface ICompilerProject
    {
        string ProjectFile { get; }
        string ProjectDirectory { get; }
        bool IsIncludeProject { get; }

        string GetLanguage();
        IEnumerable<CompilerProviderOption> GetProviderOptions();
        string GetResourceCompiler();
        string GetOutputDir();
        string GetOutput();
        bool? GetGenerateExecutable();
        bool? GetGenerateInMemory();
        bool? GetDebugInformation();
        int? GetWarningLevel();
        IEnumerable<string> GetCompilerOptions();
        string GetKeyFile();
        string GetTarget();
        bool? GetCopySourceFiles();
        string GetIcon();
        IEnumerable<ICompilerProject> GetIncludeProjects();
        IEnumerable<string> GetUsings();
        IEnumerable<CompilerFile> GetSources();
        IEnumerable<CompilerFile> GetFiles();
        IEnumerable<CompilerFile> GetSourceFiles();
        IEnumerable<CompilerAssembly> GetAssemblies();
        IEnumerable<string> GetCopyOutputs();
    }
}
