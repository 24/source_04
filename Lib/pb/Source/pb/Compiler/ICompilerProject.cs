using System;
using System.Collections.Generic;

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
        public Dictionary<string, string> Attributes = new Dictionary<string, string>();

        public CompilerFile(string file)
        {
            this.File = file;
        }
    }

    public class CompilerAssembly
    {
        public string File = null;
        public bool Resolve = false;
        public string ResolveName = null;

        public CompilerAssembly(string file, bool resolve = false, string resolveName = null)
        {
            File = file;
            Resolve = resolve;
            ResolveName = resolveName;
        }
    }

    public interface ICompilerProject
    {
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
        string GetIcon();
        IEnumerable<ICompilerProject> GetIncludeProjects();
        IEnumerable<string> GetUsings();
        IEnumerable<CompilerFile> GetSources();
        IEnumerable<CompilerFile> GetFiles();
        IEnumerable<CompilerAssembly> GetAssemblies();
        IEnumerable<string> GetCopyOutputs();
    }
}
