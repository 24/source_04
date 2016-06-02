using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Compiler
{
    public class CSharpCodeProviderCompilerResults : ICompilerResults
    {
        private CompilerResults _results;

        public CSharpCodeProviderCompilerResults(CompilerResults results)
        {
            _results = results;
        }

        public int ErrorsCount { get { return _results.Errors.Count; } }

        public Assembly GetCompiledAssembly()
        {
            return _results.CompiledAssembly;
        }

        public string GetCompiledAssemblyPath()
        {
            return _results.PathToAssembly;
        }

        public bool HasErrors()
        {
            return _results.Errors.HasErrors;
        }

        public bool HasWarnings()
        {
            return _results.Errors.HasWarnings;
        }

        public IEnumerable<CompilerError> GetErrors()
        {
            foreach (System.CodeDom.Compiler.CompilerError error in _results.Errors)
            {
                yield return new CompilerError { ErrorNumber = error.ErrorNumber, ErrorText = error.ErrorText, IsWarning = error.IsWarning, FileName = error.FileName, Column = error.Column, Line = error.Line };
            }
        }
    }

    public interface ICompilerZZ
    {
        Dictionary<string, string> ProviderOption { get; set; }
        string CompilerOptions { get; set; }
        bool GenerateInMemory { get; set; }
        bool GenerateExecutable { get; set; }
        bool DebugInformation { get; set; }
        int WarningLevel { get; set; }
        string OutputAssembly { get; set; }

        ICompilerResults Compile();
        void AddSources(IEnumerable<string> sources);
        void AddReferencedAssembly(string referencedAssembly);
        void AddEmbeddedResource(string resourceFile);
    }

    public class zzCSharpCodeProvider : ICompilerZZ
    {
        private Dictionary<string, string> _providerOption = new Dictionary<string, string>();
        private string _compilerOptions = null;
        private bool _generateInMemory = false;
        private bool _generateExecutable = false;
        private bool _debugInformation = false;
        private int _warningLevel = -1;
        private string _outputAssembly = null; // _finalOutputAssembly
        private List<string> _sources = null;
        //private List<string> _referencedAssemblies = null;
        //private List<string> _embeddedResourceResources = null;
        private CompilerParameters _options = new CompilerParameters();


        public Dictionary<string, string> ProviderOption { get { return _providerOption; } set { _providerOption = value; } }
        public string CompilerOptions { get { return _compilerOptions; } set { _compilerOptions = value; } }
        public bool GenerateInMemory { get { return _generateInMemory; } set { _generateInMemory = value; } }
        public bool GenerateExecutable { get { return _generateExecutable; } set { _generateExecutable = value; } }
        public bool DebugInformation { get { return _debugInformation; } set { _debugInformation = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public string OutputAssembly { get { return _outputAssembly; } set { _outputAssembly = value; } }


        public void AddSources(IEnumerable<string> sources)
        {
            _sources.AddRange(sources);
        }

        public void AddReferencedAssembly(string referencedAssembly)
        {
            //_referencedAssemblies.Add(referencedAssembly);
            _options.ReferencedAssemblies.Add(referencedAssembly);
        }

        public void AddEmbeddedResource(string resourceFile)
        {
            //_embeddedResourceResources.Add(resourceFile);
            _options.EmbeddedResources.Add(resourceFile);
        }

        public ICompilerResults Compile()
        {
            //CompilerParameters options = new CompilerParameters();
            _options.CompilerOptions = _compilerOptions;
            _options.GenerateInMemory = _generateInMemory;
            _options.OutputAssembly = _outputAssembly;
            _options.GenerateExecutable = _generateExecutable;
            _options.IncludeDebugInformation = _debugInformation;

            // WarningLevel : from http://msdn.microsoft.com/en-us/library/13b90fz7.aspx
            //   0 Turns off emission of all warning messages.
            //   1 Displays severe warning messages.
            //   2 Displays level 1 warnings plus certain, less-severe warnings, such as warnings about hiding class members.
            //   3 Displays level 2 warnings plus certain, less-severe warnings, such as warnings about expressions that always evaluate to true or false.
            //   4 (the default) Displays all level 3 warnings plus informational warnings.
            _options.WarningLevel = _warningLevel;

            //foreach (string assembly in _referencedAssemblies)
            //    options.ReferencedAssemblies.Add(assembly);
            //foreach (string compiledResource in _embeddedResourceResources)
            //    options.EmbeddedResources.Add(compiledResource);

            CodeDomProvider provider = new CSharpCodeProvider(_providerOption);
            CompilerResults results = provider.CompileAssemblyFromFile(_options, _sources.ToArray());
            provider.Dispose();

            return new CSharpCodeProviderCompilerResults(results);
        }
    }
}
