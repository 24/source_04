using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace pb.Compiler
{
    public class ResourceCompilerResults
    {
        public bool HasError = false;
        public List<ResourceCompilerError> Errors = new List<ResourceCompilerError>();
    }

    public class CompilerError
    {
        public string ErrorNumber;
        public string ErrorText;
        public bool IsWarning;
        public string FileName;
        public int Column;
        public int Line;
    }

    public class ResourceCompilerError
    {
        public string FileName;
        public string ErrorText;
        public ResourceCompilerError(string FileName, string ErrorText)
        {
            this.FileName = FileName;
            this.ErrorText = ErrorText;
        }
    }

    public interface ICompilerResults
    {
        int ErrorsCount { get; }
        Assembly GetCompiledAssembly();
        string GetCompiledAssemblyPath();
        bool HasErrors();
        bool HasWarnings();
        IEnumerable<CompilerError> GetErrors();
    }

    public interface ICompiler
    {
        //string DefaultDir { get; set; }
        //IEnumerable<CompilerFile> SourceList { get; }
        //IEnumerable<CompilerFile> FileList { get; }
        //Dictionary<string, CompilerAssembly> Assemblies { get; }
        ///// <summary>CSharp, JScript</summary>
        //string Language { get; set; }
        //Dictionary<string, string> ProviderOption { get; }
        //string ResourceCompiler { get; set; }
        //bool GenerateInMemory { get; set; }
        //bool GenerateExecutable { get; set; }
        //bool DebugInformation { get; set; }
        //int WarningLevel { get; set; }
        //string OutputDir { get; set; }
        //string OutputAssembly { get; }
        //string CompilerOptions { get; set; }
        //ResourceCompilerResults ResourceResults { get; }
        //CompilerResults Results { get; }
        //IEnumerable<string> CopyOutputDirectories { get; }
        bool HasError();
        //void SetParameters(ICompilerProject project, bool dontSetOutput = false);
        //void SetOutputAssembly(string outputAssembly, ICompilerProject project = null);
        //void SetProviderOptions(IEnumerable<CompilerProviderOption> options, ICompilerProject project = null);
        //void AddCompilerOptions(IEnumerable<string> options);
        //void AddCompilerOption(string option);
        //void AddSources(IEnumerable<CompilerFile> sources);
        //void AddFiles(IEnumerable<CompilerFile> files);
        //void AddAssemblies(IEnumerable<CompilerAssembly> assemblies);
        //void AddAssembly(CompilerAssembly assembly);
        //void AddCopyOutputDirectories(IEnumerable<string> directories);
        //void Compile();
        //string[] CompileResources(CompilerFile[] resources);
        //string CompileResource(CompilerFile resource, string outputDir);
        DataTable GetCompilerMessagesDataTable();
        void CopyResultFilesToDirectory(string directory);
        //void TraceMessages();
    }
}
