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

    //public class CopyOutputDirectory
    //{
    //    public string Directory = null;
    //    public List<CompilerFile> Files = new List<CompilerFile>();
    //}

    public interface ICompiler
    {
        string DefaultDir { get; set; }
        IEnumerable<CompilerFile> SourceList { get; } // set;
        IEnumerable<CompilerFile> FileList { get; } // set;
        IEnumerable<CompilerAssembly> AssemblyList { get; } // set;
        /// <summary>CSharp, JScript</summary>
        string Language { get; set; }
        Dictionary<string, string> ProviderOption { get; } // set;
        string ResourceCompiler { get; set; }
        bool GenerateInMemory { get; set; }
        bool GenerateExecutable { get; set; }
        bool DebugInformation { get; set; }
        int WarningLevel { get; set; }
        string OutputDir { get; set; }
        //string FinalOutputDir { get; }
        string OutputAssembly { get; set; }
        //string FinalOutputAssembly { get; }
        //string CompiledAssemblyPath { get; }
        string CompilerOptions { get; set; }
        ResourceCompilerResults ResourceResults { get; }
        CompilerResults Results { get; }
        IEnumerable<string> CopyOutputDirectories { get; }
        //bool HasError { get; }
        bool HasError();
        //void SetCompilerParameters(string xmlFile);
        //void SetCompilerParameters(XElement xe);
        //void SetCompilerParameters(XmlConfigElement xe);
        void SetProviderOption(string name, string value);
        void AddCompilerOptions(IEnumerable<string> options);
        void AddCompilerOption(string option);
        void AddSources(IEnumerable<string> sources);
        void AddSources(IEnumerable<XElement> sources);
        void AddSource(string source);
        void AddFile(string file, string dir);
        void AddFiles(IEnumerable<XElement> files, string dir = null);
        void AddAssemblies(IEnumerable<XElement> assemblies);
        void AddAssembly(string assembly, bool resolve = false, string resolveName = null);
        void AddLocalAssemblies(IEnumerable<XElement> assemblies);
        void AddLocalAssembly(string assembly, bool resolve = false, string resolveName = null);
        void AddCopyOutputDirectories(IEnumerable<string> directories);
        //void CloseProcess();
        void Compile();
        string[] CompileResources(CompilerFile[] resources);
        string CompileResource(CompilerFile resource, string outputDir);
        DataTable GetCompilerMessagesDataTable();
        void CopyResultFilesToDirectory(string directory);
        void TraceMessages();
    }
}
