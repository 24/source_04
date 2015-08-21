using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace pb.Compiler
{
    public class ResourceCompilerResults
    {
        public bool HasError = false;
        public List<ResourceCompilerError> Errors = new List<ResourceCompilerError>();
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

    //public class CopyOutputDirectory
    //{
    //    public string Directory = null;
    //    public List<CompilerFile> Files = new List<CompilerFile>();
    //}

    public interface ICompiler
    {
        string DefaultDir { get; set; }
        IEnumerable<CompilerFile> SourceList { get; }
        IEnumerable<CompilerFile> FileList { get; }
        IEnumerable<CompilerAssembly> AssemblyList { get; }
        /// <summary>CSharp, JScript</summary>
        string Language { get; set; }
        Dictionary<string, string> ProviderOption { get; }
        string ResourceCompiler { get; set; }
        bool GenerateInMemory { get; set; }
        bool GenerateExecutable { get; set; }
        bool DebugInformation { get; set; }
        int WarningLevel { get; set; }
        string OutputDir { get; set; }
        string OutputAssembly { get; set; }
        string CompilerOptions { get; set; }
        ResourceCompilerResults ResourceResults { get; }
        CompilerResults Results { get; }
        IEnumerable<string> CopyOutputDirectories { get; }
        bool HasError();
        void SetParameters(ICompilerProject project, bool includeProject = false);
        //void SetProviderOption(string name, string value);
        void SetProviderOptions(IEnumerable<CompilerProviderOption> options);
        void AddCompilerOptions(IEnumerable<string> options);
        void AddCompilerOption(string option);
        //void AddSources(IEnumerable<string> sources);
        //void AddSources(IEnumerable<XElement> sources);
        //void AddSource(string source);
        void AddSources(IEnumerable<CompilerFile> sources);
        //void AddFile(string file, string dir);
        //void AddFiles(IEnumerable<XElement> files, string dir = null);
        void AddFiles(IEnumerable<CompilerFile> files);
        //void AddAssemblies(IEnumerable<XElement> assemblies);
        //void AddAssembly(string assembly, bool resolve = false, string resolveName = null);
        void AddAssemblies(IEnumerable<CompilerAssembly> assemblies);
        void AddAssembly(CompilerAssembly assembly);
        //void AddLocalAssemblies(IEnumerable<XElement> assemblies);
        //void AddLocalAssembly(string assembly, bool resolve = false, string resolveName = null);
        void AddCopyOutputDirectories(IEnumerable<string> directories);
        void Compile();
        string[] CompileResources(CompilerFile[] resources);
        string CompileResource(CompilerFile resource, string outputDir);
        DataTable GetCompilerMessagesDataTable();
        void CopyResultFilesToDirectory(string directory);
        void TraceMessages();
    }
}
