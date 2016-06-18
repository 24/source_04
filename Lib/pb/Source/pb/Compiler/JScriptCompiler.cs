using Microsoft.JScript;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace pb.Compiler
{
    // zzJScriptCodeProvider
    public class JScriptCompiler : ICompiler
    {
        private string _target = null;
        private string _platform = null;
        private bool _generateInMemory = false;
        private bool _debugInformation = false;
        private int _warningLevel = -1;
        private string _compilerOptions = null;
        private string _outputAssembly = null; // _finalOutputAssembly
        private string _win32ResourceFile = null;
        private IEnumerable<string> _sources = null;
        //private IEnumerable<string> _referencedAssemblies = null;
        private IEnumerable<ReferencedAssembly> _referencedAssemblies = null;
        private IEnumerable<ResourceFile> _embeddedResources = null;

        //private bool _generateExecutable = false;
        //private List<string> _sources = null;
        //private List<string> _referencedAssemblies = null;
        //private List<string> _embeddedResourceResources = null;
        //private CompilerParameters _options = new CompilerParameters();


        public string LanguageVersion { get { return null; } set { } }
        public string FrameworkVersion { get { return null; } set { } }
        public string Target { get { return _target; } set { _target = value; } }
        public string Platform { get { return _platform; } set { _platform = value; } }
        public bool GenerateInMemory { get { return _generateInMemory; } set { _generateInMemory = value; } }
        public bool DebugInformation { get { return _debugInformation; } set { _debugInformation = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public string CompilerOptions { get { return _compilerOptions; } set { _compilerOptions = value; } }
        public string OutputAssembly { get { return _outputAssembly; } set { _outputAssembly = value; } }
        public string Win32ResourceFile { get { return _win32ResourceFile; } set { _win32ResourceFile = value; } }

        //public Dictionary<string, string> ProviderOption { get { throw new PBException("no ProviderOption with JScriptCodeProvider"); } set { throw new PBException("no ProviderOption with JScriptCodeProvider"); } }
        //public bool GenerateExecutable { get { return _generateExecutable; } set { _generateExecutable = value; } }


        //public void AddSources(IEnumerable<string> sources)
        //{
        //    _sources.AddRange(sources);
        //}

        public void SetPreprocessorSymbols(IEnumerable<string> preprocessorSymbols)
        {
        }

        public void SetSources(IEnumerable<string> sources)
        {
            _sources = sources;
        }

        //public void AddReferencedAssembly(string referencedAssembly)
        //{
        //    _options.ReferencedAssemblies.Add(referencedAssembly);
        //}

        //public void SetReferencedAssemblies(IEnumerable<string> referencedAssemblies)
        public void SetReferencedAssemblies(IEnumerable<ReferencedAssembly> referencedAssemblies)
        {
            _referencedAssemblies = referencedAssemblies;
        }

        //public void AddEmbeddedResource(string resourceFile)
        //{
        //    //_embeddedResourceResources.Add(resourceFile);
        //    _options.EmbeddedResources.Add(resourceFile);
        //}

        public void SetEmbeddedResources(IEnumerable<ResourceFile> embeddedResources)
        {
            _embeddedResources = embeddedResources;
        }

        private void AddCompilerOption(string option)
        {
            if (option == null || option == "")
                return;
            if (_compilerOptions != null)
                _compilerOptions += " " + option;
            else
                _compilerOptions = option;
        }

        public ICompilerResult Compile()
        {
            CompilerParameters options = new CompilerParameters();
            if (_target != null)
                AddCompilerOption("/target:" + _target);
            if (_platform != null)
                AddCompilerOption("/platform:" + _platform);
            //options.GenerateExecutable = _generateExecutable;
            options.GenerateExecutable = IsTargetExecutable(_target);
            options.GenerateInMemory = _generateInMemory;
            options.IncludeDebugInformation = _debugInformation;

            // WarningLevel : from http://msdn.microsoft.com/en-us/library/13b90fz7.aspx
            //   0 Turns off emission of all warning messages.
            //   1 Displays severe warning messages.
            //   2 Displays level 1 warnings plus certain, less-severe warnings, such as warnings about hiding class members.
            //   3 Displays level 2 warnings plus certain, less-severe warnings, such as warnings about expressions that always evaluate to true or false.
            //   4 (the default) Displays all level 3 warnings plus informational warnings.
            options.WarningLevel = _warningLevel;

            options.OutputAssembly = _outputAssembly;
            options.CompilerOptions = _compilerOptions;

            //foreach (string assembly in _referencedAssemblies)
            foreach (ReferencedAssembly assembly in _referencedAssemblies)
                options.ReferencedAssemblies.Add(assembly.File);

            foreach (ResourceFile resource in _embeddedResources)
                options.EmbeddedResources.Add(resource.File);

            CodeDomProvider provider = new JScriptCodeProvider();
            CompilerResults result = provider.CompileAssemblyFromFile(options, _sources.ToArray());
            provider.Dispose();

            return new CodeDomProviderCompilerResult(result);
        }

        private static bool IsTargetExecutable(string target)
        {
            if (target == null)
                return true;
            switch (target.ToLowerInvariant())
            {
                case "exe":
                case "winexe":
                    return true;
                case "library":
                case "module":
                    return false;
                //case "winruntimemetadata":
                //case "winruntimeexe":
                default:
                    throw new PBException("unknow target \"{0}\"", target);
            }
        }
    }
}
