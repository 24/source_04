using System;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Compiler
{
    public class ReferencedAssembly
    {
        public string File;
        public bool FrameworkAssembly;
    }

    public class ResourceFile
    {
        public string File;
        public string Namespace;
    }

    public interface ICompilerResult
    {
        bool Success { get; }
        int MessagesCount { get; }
        Assembly LoadAssembly();
        string GetAssemblyFile();
        //bool HasErrors();
        //bool HasWarnings();
        //IEnumerable<CompilerMessage> GetMessages();
        IEnumerable<CompilerMessage> GetMessages(Predicate<CompilerMessage> messageFilter = null);
    }

    public interface ICompiler
    {
        //Dictionary<string, string> ProviderOption { get; set; }
        string LanguageVersion { get; set; }
        string FrameworkVersion { get; set; }
        string Target { get; set; }
        string Platform { get; set; }
        string CompilerOptions { get; set; }
        bool GenerateInMemory { get; set; }
        //bool GenerateExecutable { get; set; }
        bool DebugInformation { get; set; }
        int WarningLevel { get; set; }
        string OutputAssembly { get; set; }

        ICompilerResult Compile();
        //void AddSources(IEnumerable<string> sources);
        void SetSources(IEnumerable<string> sources);
        //void AddReferencedAssembly(string referencedAssembly);
        //void SetReferencedAssemblies(IEnumerable<string> referencedAssemblies);
        void SetReferencedAssemblies(IEnumerable<ReferencedAssembly> referencedAssemblies);
        //void AddEmbeddedResource(string resourceFile);
        void SetEmbeddedResources(IEnumerable<ResourceFile> embeddedResources);
    }
}
