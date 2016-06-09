using System;
using System.Data;

namespace pb.Compiler
{
    public enum CompilerMessageSeverity
    {
        Hidden = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    public class CompilerMessage
    {
        public string Id;
        public string Message;
        //public bool IsWarning;
        public CompilerMessageSeverity Severity;
        public string FileName;
        public int? Column;
        public int? Line;
    }

    public interface IProjectCompiler
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
        //bool HasError();
        bool Success { get; }
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
        void TraceMessages(Predicate<CompilerMessage> messageFilter = null);
    }
}
