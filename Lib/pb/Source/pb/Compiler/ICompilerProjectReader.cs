using System.Collections.Generic;
using pb.IO;

namespace pb.Compiler
{
    public class CompilerLanguage
    {
        public string Name;
        public string Version;
    }

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
        public ICompilerProjectReader Project = null;

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
        public bool FrameworkAssembly = false;
        public bool Resolve = false;
        public string ResolveName = null;
        //public bool CopySource = false;
        public ICompilerProjectReader Project = null;

        // bool copySource = false
        //public CompilerAssembly(string file, bool resolve = false, string resolveName = null, ICompilerProjectReader project = null)
        //{
        //    File = file;
        //    Resolve = resolve;
        //    ResolveName = resolveName;
        //    //CopySource = copySource;
        //    Project = project;
        //}
    }

    //public class CompilerUpdateDirectory
    //{
    //    public string SourceDirectory = null;
    //    public string DestinationDirectory = null;
    //}

    // old name ICompilerProject
    public interface ICompilerProjectReader
    {
        string ProjectFile { get; }
        string ProjectDirectory { get; }
        bool IsIncludeProject { get; }

        //string GetLanguage();
        CompilerLanguage GetLanguage();
        string GetFrameworkVersion();
        string GetTarget();
        string GetPlatform();
        bool? GetGenerateInMemory();
        bool? GetDebugInformation();
        int? GetWarningLevel();
        IEnumerable<string> GetCompilerOptions();
        //IEnumerable<CompilerProviderOption> GetProviderOptions();
        //string GetResourceCompiler();
        //string GetOutputDir();
        string GetOutput();
        //bool? GetGenerateExecutable();
        string GetKeyFile();
        bool? GetCopySourceFiles();
        bool? GetCopyRunSourceSourceFiles();
        string GetIcon();
        IEnumerable<ICompilerProjectReader> GetIncludeProjects();
        IEnumerable<string> GetInitMethods();
        IEnumerable<string> GetUsings();
        IEnumerable<CompilerFile> GetSources();
        IEnumerable<CompilerFile> GetFiles();
        IEnumerable<CompilerFile> GetSourceFiles();
        IEnumerable<CompilerAssembly> GetAssemblies();
        IEnumerable<string> GetCopyOutputs();
        //IEnumerable<CompilerUpdateDirectory> GetUpdateDirectory();
    }
}
