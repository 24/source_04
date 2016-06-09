using System.Collections.Generic;
using System.Data;
using System.IO;
using pb.IO;
using System;

/*******************
 * C# Compiler Options Listed by Category http://msdn.microsoft.com/en-us/library/vstudio/6s2x2bzy.aspx
 * <Language                                  value = "" />  <!-- CSharp, JScript -->
 * <ProviderOption                            name  = "CompilerVersion" value = "v4.0" />
 * ...
 * <ResourceCompiler                          value = "c:\Program Files\Microsoft SDKs\Windows\v7.1\Bin\ResGen.exe"/>
 * <Target                                    value = "" /> <!-- exe (exe console default), library (dll), module (.netmodule), winexe (exe windows) -->
 * <KeyFile                                   value = "" />
 * <Icon                                      value = "" />
 * <OutputDir                                 value = "" />
 * <Output                                    value = "" />
 * <GenerateExecutable                        value = "" />
 * <GenerateInMemory                          value = "" />
 * <DebugInformation                          value = "" />
 * <WarningLevel                              value = "" />              Gets or sets the warning level at which the compiler aborts compilation. FAUX marche comme /warn:option
 * <CompilerOptions                           value = "" />              http://msdn.microsoft.com/en-us/library/2fdbz5xd.aspx   /define:DEBUG;TRACE
 *   /warn:option 0 Désactive l'émission de tous les messages d'avertissement, 1 Affiche les messages d'avertissement grave, 2 Affiche les avertissements de niveau 1 ainsi que quelques avertissements moins graves,
 *                2 Affiche les avertissements de niveau 2 ainsi que quelques avertissements moins graves, 4 Affiche tous les avertissements de niveau 3 plus les avertissements d'information
 * <IncludeProject                            value = "" />
 * ...
 * <Source                                    value = "" [namespace = ""] />
 * ...
 * <File                                      value = "" [destinationFile = ""] />
 * ...
 * <Assembly                                  value = "" resolve = "true" resolveName = "PcapDotNet.Base, Version=0.10.0.20588, Culture=neutral, PublicKeyToken=4b6f3e583145a652" />
 * ...
 * <LocalAssembly                             value = "" resolve = "true" resolveName = "PcapDotNet.Base, Version=0.10.0.20588, Culture=neutral, PublicKeyToken=4b6f3e583145a652" />
 * ...
 * <CopyOutput                                value = "" />
*******************/
// <CopyOutput value = "" />
//   < File                                    value = "" />
//
// UpdateDirectory : (_updateDirectories)
//   pas besoin de faire UpdateDirectory par exemple Extension_01.dll est copié dans runsource\exe\run donc on peut maj Extension_01.dll dans library\pib\Extension_01
//   <UpdateDirectory                  source = "$Root$\..\library\pib\Extension_01\new" destination = "$Root$\..\library\pib\Extension_01" />



// rename :
// ok  Compiler                                 ProjectCompiler
// ok  CompilerException                        to delete
// ok  ICompiler                                IProjectCompiler
// non ICompilerResults                         IProjectCompilerResults
// ok  CompilerProject                          CompilerProjectReader
// ok  ICompilerProject                         ICompilerProjectReader
//     CompilerProviderOption                   to delete
// ok  ICompilerZZ                              ICompiler
// ok  zzCSharpCodeProvider                     CSharp1Compiler
// ok  CSharpCodeProviderCompilerResults        CSharp1CompilerResults
// ok  zzJScriptCodeProvider                    JScriptCompiler



// project compiler :
//   ProjectCompiler          : project compiler
//   IProjectCompiler         : interface project compiler
//   CompilerProjectReader    : project reader
//   ICompilerProjectReader
//   CompilerFile
//   CompilerAssembly
// compiler result :
//   ResourceCompilerResults
//   CompilerError            : 
//   ResourceCompilerError
//   ICompilerResults
// compiler :
//   ICompiler                : interface compiler
//   CSharp1Compiler          : CSharp compiler until v4
//   CSharp1CompilerResults   : result of CSharp compiler until v4
//   CSharp5Compiler          : CSharp compiler from v5 (roslyn)
//   JScriptCompiler          : JScript compiler
// generate code
//   GenerateAssembly         : generate indexed assembly name
//   GenerateCSharpCode       : generate CSharp code
//   GenerateCSharpCodeResult
//   CSharpCodeWriter         : write CSharp code
//   ClassOptions             : class type
// other :
//   AssemblyResolve
// RunSource :
//   RunSource
//   IRunSource
//   EndRunCodeInfo
//   RemoteRunSource
//   RunCode
//   RunSourceInitEndMethods
//   CompilerGlobalExtension
//   RunSourceHtmlRun
//   RunSourceUpdate
//   RunSourceUpdateDirectory


namespace pb.Compiler
{
    public partial class ProjectCompiler : IProjectCompiler
    {
        public static int __traceLevel = 1;    // 0 no message, 1 default messages, 2 detailled messaged

        private CompilerFile _projectCompilerFile = null;
        private string _projectDirectory = null;
        private Dictionary<string, CompilerFile> _sourceList = new Dictionary<string, CompilerFile>();
        private CompilerFile _appConfig = null;
        private Dictionary<string, CompilerFile> _fileList = new Dictionary<string, CompilerFile>();
        private Dictionary<string, CompilerFile> _sourceFileList = new Dictionary<string, CompilerFile>();
        private Dictionary<string, CompilerAssembly> _assemblyList = new Dictionary<string, CompilerAssembly>();
        //private string _language = null;           // CSharp, JScript
        private CompilerLanguage _language = null;   // CSharp1 version v3.5, v4.0, CSharp5 version 1, 2, 3, 4, 5, 6, JScript
        private string _frameworkVersion = null;
        //private Dictionary<string, string> _providerOption = new Dictionary<string, string>();
        //private string _resourceCompiler = null;
        private ResourceCompiler _resourceCompiler = null;
        private string _target = null;
        private string _platform = null;
        private bool _generateInMemory = false;
        //private bool _generateExecutable = false;
        private bool _debugInformation = false;
        private int _warningLevel = -1;
        private string _compilerOptions = null;
        //private string _outputDir = null;
        private string _finalOutputDir = null;
        private string _CompileResourceSubDirectory = "resources";
        private string _outputAssembly = null;
        private string _finalOutputAssembly = null;
        //private ResourceCompilerResults _resourceResults = new ResourceCompilerResults();
        private bool _resourceError = false;
        private List<ResourceCompilerMessage> _resourceMessages = new List<ResourceCompilerMessage>();
        //private CompilerResults _results = null;
        private ICompilerResult _result = null;
        private List<string> _copyOutputDirectories = new List<string>();
        private static string __zipSourceFilename = ".source.zip";
        private bool _copySourceFiles = false;
        private bool _copyRunSourceSourceFiles = false;
        private string _runsourceSourceDirectory;
        private string _zipSourceFile = null;

        public ProjectCompiler(ResourceCompiler resourceCompiler)
        {
            _resourceCompiler = resourceCompiler;
        }

        public static int TraceLevel { get { return __traceLevel; } set { __traceLevel = value; } }
        //public string DefaultDirectory { get { return _defaultDirectory; } set { _defaultDirectory = value; } }
        public IEnumerable<CompilerFile> SourceList { get { return _sourceList.Values; } }
        public IEnumerable<CompilerFile> FileList { get { return _fileList.Values; } }
        public IEnumerable<CompilerFile> SourceFileList { get { return _sourceFileList.Values; } }
        public Dictionary<string, CompilerAssembly> Assemblies { get { return _assemblyList; } }
        public CompilerLanguage Language { get { return _language; } set { _language = value; } }
        //public Dictionary<string, string> ProviderOption { get { return _providerOption; } }
        //public string ResourceCompiler { get { return _resourceCompiler; } set { _resourceCompiler = value; } }
        public bool GenerateInMemory { get { return _generateInMemory; } set { _generateInMemory = value; } }
        //public bool GenerateExecutable { get { return _generateExecutable; } set { _generateExecutable = value; } }
        public bool DebugInformation { get { return _debugInformation; } set { _debugInformation = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public string CompilerOptions { get { return _compilerOptions; } set { _compilerOptions = value; } }
        //public string OutputDir { get { return _outputDir; } set { _outputDir = value; } }
        public string OutputAssembly { get { return _outputAssembly; } }
        //public ResourceCompilerResults ResourceResults { get { return _resourceResults; } }
        public ICompilerResult Results { get { return _result; } }
        public IEnumerable<string> CopyOutputDirectories { get { return _copyOutputDirectories; } }
        //public IEnumerable<CompilerUpdateDirectory> UpdateDirectories { get { return _updateDirectories; } }
        public static string ZipSourceFilename { get { return __zipSourceFilename; } }
        public bool CopyRunSourceSourceFiles { get { return _copyRunSourceSourceFiles; } }
        public string RunsourceSourceDirectory { get { return _runsourceSourceDirectory; } set { _runsourceSourceDirectory = value; } }

        // IProjectCompiler
        public bool Success { get { return !_resourceError && _result != null ? _result.Success : false; } }

        // IProjectCompiler
        //public bool HasError()
        //{
        //    //if ((_results != null && _results.HasErrors()) || _resourceResults.HasError)
        //    if ((_results != null && _results.HasErrors()) || _resourceError)
        //        return true;
        //    else
        //        return false;
        //}

        public void SetProjectCompilerFile(CompilerFile projectCompilerFile)
        {
            _projectCompilerFile = projectCompilerFile;
            _projectDirectory = zPath.GetDirectoryName(projectCompilerFile.File);
        }

        // runCode : true when executing code from runsource, true for CompilerDefaultValues and ProjectDefaultValues, otherwise false
        // bool dontSetOutput = false
        public void SetParameters(ICompilerProjectReader project, bool runCode = false, bool includeProject = false)
        {
            if (project == null)
                return;

            //string s = project.GetLanguage();
            //if (s != null)
            //{
            //    if (_language != null)
            //        WriteLine(1, "Compiler warning : redefine language \"{0}\" as \"{1}\" from project \"{2}\"", _language, s, project.ProjectFile);
            //    _language = s;
            //}
            //CompilerLanguage language = project.GetLanguage();
            //if (language != null)
            //{
            //    if (_language != null)
            //        WriteLine(1, "Compiler warning : redefine language \"{0}\"-\"{1}\" as \"{2}\"-\"{3}\" from project \"{4}\"", _language.Name, _language.Version, language.Name, language.Version, project.ProjectFile);
            //    _language = language;
            //}

            if (!includeProject)
            {
                SetLanguage(project.GetLanguage(), project);
                SetFrameworkVersion(project.GetFrameworkVersion(), project);
            }

            if (!runCode && !includeProject)
            {
                //s = project.GetTarget();
                //if (s != null)
                //    AddCompilerOption("/target:" + s);
                SetTarget(project.GetTarget(), project);
                SetPlatform(project.GetPlatform(), project);
            }

            bool? b;

            if (!runCode)
            {
                b = project.GetGenerateInMemory();
                if (b != null)
                    _generateInMemory = (bool)b;
            }

            b = project.GetDebugInformation();
            if (b != null)
                _debugInformation = (bool)b;

            int? i = project.GetWarningLevel();
            if (i != null)
                _warningLevel = (int)i;

            AddCompilerOptions(project.GetCompilerOptions());

            string s;

            if (!runCode && !includeProject)
            {
                s = project.GetOutput();
                if (s != null)
                    SetOutputAssembly(s, project);
            }

            if (!includeProject)
            {
                s = project.GetIcon();
                if (s != null)
                    AddCompilerOption("/win32icon:\"" + s + "\"");
            }

            if (!includeProject)
            {
                s = project.GetKeyFile();
                if (s != null)
                    AddCompilerOption("/keyfile:\"" + s + "\"");
            }

            // GetIncludeProjects() get include project recursively
            if (!includeProject)
            {
                foreach (ICompilerProjectReader project2 in project.GetIncludeProjects())
                {
                    SetParameters(project2, runCode: runCode, includeProject: true);
                }
            }

            AddSources(project.GetSources());

            AddFiles(project.GetFiles());

            AddSourceFiles(project.GetSourceFiles());

            AddAssemblies(project.GetAssemblies());

            if (!runCode && !includeProject)
            {
                b = project.GetCopySourceFiles();
                if (b != null)
                    _copySourceFiles = (bool)b;

                b = project.GetCopyRunSourceSourceFiles();
                if (b != null)
                    _copyRunSourceSourceFiles = (bool)b;
            }

            if (!runCode && !includeProject)
            {
                AddCopyOutputDirectories(project.GetCopyOutputs());
            }

            //foreach (XElement xe2 in xe.GetElements("ProviderOption"))
            //    compiler.SetProviderOption(xe2.zAttribValue("name"), xe2.zAttribValue("value"));
            //SetProviderOptions(project.GetProviderOptions(), project);

            //compiler.ResourceCompiler = xe.Get("ResourceCompiler", compiler.ResourceCompiler);
            //s = project.GetResourceCompiler();
            //if (s != null)
            //{
            //    if (_resourceCompiler != null)
            //        WriteLine(1, "Compiler warning : redefine resource compiler \"{0}\" as \"{1}\" from project \"{2}\"", _resourceCompiler, s, project.ProjectFile);
            //    _resourceCompiler = s;
            //}

            //if (!runCode)
            //{
            //    s = project.GetOutputDir();
            //    if (s != null)
            //    {
            //        if (_outputDir != null)
            //            WriteLine(1, "Compiler warning : redefine output directory \"{0}\" as \"{1}\" from project \"{2}\"", _outputDir, s, project.ProjectFile);
            //        _outputDir = s;
            //    }
            //}

            //if (s != null)
            //    _outputAssembly = s;
            //string ext = zPath.GetExtension(_outputAssembly);
            //if (ext != null)
            //{
            //    if (ext.ToLower() == ".exe")
            //        _generateExecutable = true;
            //    else if (ext.ToLower() == ".dll")
            //        _generateExecutable = false;
            //}

            //bool? b;
            //if (!runCode && !includeProject)
            //{
            //    b = project.GetGenerateExecutable();
            //    if (b != null)
            //        _generateExecutable = (bool)b;
            //}

            //compiler.AddLocalAssemblies(xe.GetElements("LocalAssembly"));

            //if (!runCode && !includeProject)
            //{
            //    AddUpdateDirectory(project.GetUpdateDirectory());
            //}
        }

        public void SetLanguage(CompilerLanguage language, ICompilerProjectReader project = null)
        {
            if (language != null)
            {
                if (_language != null)
                    WriteLine(1, "Compiler warning : redefine language \"{0}\"-\"{1}\" as \"{2}\"-\"{3}\" from project \"{4}\"", _language.Name, _language.Version, language.Name, language.Version, project.ProjectFile);
                //else
                //    WriteLine(1, "Compiler info    : set language \"{0}\"-\"{1}\" from project \"{2}\"", language.Name, language.Version, project.ProjectFile);
                _language = language;
            }
        }

        //GetFrameworkVersion()
        public void SetFrameworkVersion(string frameworkVersion, ICompilerProjectReader project = null)
        {
            if (frameworkVersion != null)
            {
                if (_frameworkVersion != null)
                    WriteLine(1, "Compiler warning : redefine framework version \"{0}\" as \"{1}\" from project \"{2}\"", _frameworkVersion, frameworkVersion, project != null ? project.ProjectFile : "(no project)");
                _frameworkVersion = frameworkVersion;
            }
        }

        public void SetTarget(string target, ICompilerProjectReader project = null)
        {
            if (target != null)
            {
                if (_target != null)
                    WriteLine(1, "Compiler warning : redefine target \"{0}\" as \"{1}\" from project \"{2}\"", _target, target, project != null ? project.ProjectFile : "(no project)");
                _target = target;
            }
        }

        public void SetPlatform(string platform, ICompilerProjectReader project = null)
        {
            if (platform != null)
            {
                if (_platform != null)
                    WriteLine(1, "Compiler warning : redefine platform \"{0}\" as \"{1}\" from project \"{2}\"", _platform, platform, project != null ? project.ProjectFile : "(no project)");
                _platform = platform;
            }
        }

        public void SetOutputAssembly(string outputAssembly, ICompilerProjectReader project = null)
        {
            if (outputAssembly != null)
            {
                if (_outputAssembly != null)
                    WriteLine(1, "Compiler warning : redefine output assembly \"{0}\" as \"{1}\" from project \"{2}\"", _outputAssembly, outputAssembly, project != null ? project.ProjectFile : "(no project)");
                _outputAssembly = outputAssembly;
                //string ext = zPath.GetExtension(outputAssembly);
                //if (ext != null)
                //{
                //    if (ext.ToLower() == ".exe")
                //        _generateExecutable = true;
                //    else if (ext.ToLower() == ".dll")
                //        _generateExecutable = false;
                //}
            }
        }

        //public void SetProviderOptions(IEnumerable<CompilerProviderOption> options, ICompilerProjectReader project = null)
        //{
        //    foreach (CompilerProviderOption option in options)
        //    {
        //        if (_providerOption.ContainsKey(option.Name))
        //        {
        //            WriteLine(1, "Compiler warning : redefine provider option \"{0}\" value \"{1}\" as \"{2}\" from project \"{3}\"", option.Name, _providerOption[option.Name], option.Value, project != null ? project.ProjectFile : "--no project--");
        //            _providerOption.Remove(option.Name);
        //        }
        //        _providerOption.Add(option.Name, option.Value);
        //    }
        //}

        public void AddCompilerOptions(IEnumerable<string> options)
        {
            foreach (string option in options)
                AddCompilerOption(option);
        }

        public void AddCompilerOption(string option)
        {
            if (option == null || option == "")
                return;
            if (_compilerOptions != null)
                _compilerOptions += " " + option;
            else
                _compilerOptions = option;
        }

        public void AddSources(IEnumerable<CompilerFile> sources)
        {
            foreach (CompilerFile source in sources)
                AddSource(source);
        }

        public void AddSource(CompilerFile source)
        {
            if (!_sourceList.ContainsKey(source.File))
                _sourceList.Add(source.File, source);
            //else
            //{
            //    WriteLine(1, "Compiler warning : duplicate source file \"{0}\" from project \"{1}\"", source.File, source.Project.ProjectFile);
            //    WriteLine(1, "  already loaded from project \"{0}\"", _sourceList[source.File].Project.ProjectFile);
            //}
        }

        public void AddFiles(IEnumerable<CompilerFile> files)
        {
            //_fileList.AddRange(files);
            foreach (CompilerFile file in files)
            {
                if (!_fileList.ContainsKey(file.File))
                    _fileList.Add(file.File, file);
                else
                {
                    WriteLine(1, "Compiler warning : duplicate file \"{0}\" from project \"{1}\"", file.File, file.Project.ProjectFile);
                    WriteLine(1, "  already loaded from project \"{0}\"", _fileList[file.File].Project.ProjectFile);
                }
            }
        }

        public void AddSourceFiles(IEnumerable<CompilerFile> files)
        {
            foreach (CompilerFile file in files)
            {
                if (!_sourceFileList.ContainsKey(file.File))
                    _sourceFileList.Add(file.File, file);
            }
        }

        public void AddAssemblies(IEnumerable<CompilerAssembly> assemblies)
        {
            foreach (CompilerAssembly assembly in assemblies)
                AddAssembly(assembly);
        }

        public void AddAssembly(CompilerAssembly assembly)
        {
            //if (!_assemblyList.ContainsKey(assembly.File))
            //    _assemblyList.Add(assembly.File, assembly);
            string filename = zPath.GetFileNameWithoutExtension(assembly.File).ToLowerInvariant();
            if (!_assemblyList.ContainsKey(filename))
                _assemblyList.Add(filename, assembly);
            else if (!assembly.Project.IsIncludeProject)
            {
                //CompilerAssembly assembly2 = _assemblyList[assembly.File];
                CompilerAssembly assembly2 = _assemblyList[filename];
                if (!assembly2.Project.IsIncludeProject)
                {
                    WriteLine(1, "Compiler warning : duplicate assembly \"{0}\" from project \"{1}\"", assembly.File, assembly.Project.ProjectFile);
                    WriteLine(1, "  already loaded from project \"{0}\"", assembly2.Project.ProjectFile);
                }
            }
        }

        public CompilerAssembly GetAssembly(string filename)
        {
            if (_assemblyList.ContainsKey(filename))
                return _assemblyList[filename];
            else
                return null;
        }

        public void AddCopyOutputDirectories(IEnumerable<string> directories)
        {
            _copyOutputDirectories.AddRange(directories);
        }

        //public void AddUpdateDirectory(IEnumerable<CompilerUpdateDirectory> updateDirectories)
        //{
        //    _updateDirectories.AddRange(updateDirectories);
        //}

        public void Compile()
        {
            if (!_generateInMemory && _outputAssembly == null)
                throw new PBException("output assembly is not defined");
            SetFinalOutputAssembly();
            if (_finalOutputDir != null)
                zDirectory.CreateDirectory(_finalOutputDir);
            WriteLine(2, "Compile \"{0}\"", _finalOutputAssembly);
            WriteLine(2, "  DebugInformation      {0}", _debugInformation);
            WriteLine(2, "  GenerateInMemory      {0}", _generateInMemory);
            //WriteLine(2, "  GenerateExecutable    {0}", _generateExecutable);
            WriteLine(2, "  WarningLevel          {0}", _warningLevel);
            WriteLine(2, "  CompilerOptions       \"{0}\"", _compilerOptions);

            _appConfig = GetCompilerFileName("app.config");
            if (_appConfig != null)
                WriteLine(2, "  app.config            \"{0}\"", _appConfig.File);

            //CompilerParameters options = new CompilerParameters();
            //options.CompilerOptions = _compilerOptions;
            //options.GenerateInMemory = _generateInMemory;
            //options.OutputAssembly = _finalOutputAssembly;
            //options.GenerateExecutable = _generateExecutable;
            //options.IncludeDebugInformation = _debugInformation;
            //// WarningLevel : from http://msdn.microsoft.com/en-us/library/13b90fz7.aspx
            ////   0 Turns off emission of all warning messages.
            ////   1 Displays severe warning messages.
            ////   2 Displays level 1 warnings plus certain, less-severe warnings, such as warnings about hiding class members.
            ////   3 Displays level 2 warnings plus certain, less-severe warnings, such as warnings about expressions that always evaluate to true or false.
            ////   4 (the default) Displays all level 3 warnings plus informational warnings.
            //options.WarningLevel = _warningLevel;
            //foreach (CompilerAssembly assembly in _assemblyList.Values)
            //{
            //    //WriteLine(2, "  Assembly              \"{0}\" resolve {1}", assembly.File, assembly.Resolve);
            //    options.ReferencedAssemblies.Add(assembly.File);
            //    // transfered to RunSource.RunCode_ExecuteCode()
            //    //if (assembly.Resolve)
            //    //    AssemblyResolve.Add(assembly.File, assembly.ResolveName);
            //}


            //CompilerFile[] resources = GetCompilerFilesType(".resx");
            //string[] compiledResources = CompileResources(resources);
            //foreach (string compiledResource in compiledResources)
            //{
            //    WriteLine(2, "  Resource              \"{0}\"", compiledResource);
            //    options.EmbeddedResources.Add(compiledResource);
            //}

            //WriteLine(2, "  Resource error        {0}", _resourceResults.Errors.Count);
            //if (_resourceResults.HasError)
            //    return;

            //CodeDomProvider provider = null;
            //string sourceExt = null;
            //// _language : CSharp, JScript
            //if (_language == null)
            //    throw new CompilerException("error undefined language");
            //string language = _language.ToLower();
            //if (language == "csharp")
            //{
            //    provider = new CSharpCodeProvider(_providerOption);
            //    sourceExt = ".cs";
            //}
            //else if (language == "jscript")
            //{
            //    provider = new JScriptCodeProvider();
            //    sourceExt = ".js";
            //}
            //else
            //    throw new CompilerException("error unknow language \"{0}\"", _language);
            //string[] sources = GetFilesType(sourceExt);
            //_results = provider.CompileAssemblyFromFile(options, sources);
            //WriteLine(2, "  Compile error warning {0}", _results.Errors.Count);
            //WriteLine(2, "  Compile has error     {0}", _results.Errors.HasErrors);
            //WriteLine(2, "  Compile has warning   {0}", _results.Errors.HasWarnings);
            //provider.Dispose();

            //ICompiler compiler = GetCompiler(_language);
            if (_language == null)
                throw new PBException("language not defined");
            ICompiler compiler = CompilerManager.Current.GetCompiler(_language.Name);
            compiler.LanguageVersion = _language.Version;
            compiler.FrameworkVersion = _frameworkVersion;
            compiler.Target = _target;
            compiler.Platform = _platform;
            compiler.GenerateInMemory = _generateInMemory;
            compiler.DebugInformation = _debugInformation;
            compiler.WarningLevel = _warningLevel;
            compiler.CompilerOptions = _compilerOptions;
            compiler.OutputAssembly = _finalOutputAssembly;


            //compiler.ProviderOption = _providerOption;
            //compiler.GenerateExecutable = _generateExecutable;

            //compiler.AddSources(GetFilesByType(GetSourceExtension(_language.Name)));
            compiler.SetSources(GetFilesByType(GetSourceExtension(_language.Name)));

            //foreach (CompilerAssembly assembly in _assemblyList.Values)
            //{
            //    WriteLine(2, "  Assembly              \"{0}\" resolve {1}", assembly.File, assembly.Resolve);
            //    compiler.AddReferencedAssembly(assembly.File);
            //}
            compiler.SetReferencedAssemblies(GetReferencedAssemblies());

            // compile resources files
            CompilerFile[] resources = GetCompilerFilesType(".resx");
            //Trace.WriteLine("resources.Length {0}", resources.Length);
            //string[] compiledResources = CompileResources(resources);
            ResourceFile[] compiledResources = CompileResources(resources);
            //Trace.WriteLine("compiledResources.Length {0}", compiledResources.Length);
            compiler.SetEmbeddedResources(GetCompiledResources(compiledResources));
            //foreach (string compiledResource in compiledResources)
            //{
            //    WriteLine(2, "  Resource              \"{0}\"", compiledResource);
            //    compiler.AddEmbeddedResource(compiledResource);
            //}
            //WriteLine(2, "  Resource error        {0}", _resourceResults.Errors.Count);
            WriteLine(2, "  Resource messages     {0}", _resourceMessages.Count);
            //if (_resourceResults.HasError)
            if (_resourceError)
                return;

            _result = compiler.Compile();

            WriteLine(2, "  Compile message       {0}", _result.MessagesCount);
            WriteLine(2, "  Compile success       {0}", _result.Success);
            //WriteLine(2, "  Compile has warning   {0}", _result.HasWarnings());


            if (_copySourceFiles)
                CopySourceFiles();

            //if (_results.PathToAssembly != null)
            if (_result.GetAssemblyFile() != null)
                CopyResultFilesToDirectory();

            CopyOutputToDirectories();
            _CopyRunSourceSourceFiles();
        }

        private static string GetSourceExtension(string language)
        {
            string language2 = language.ToLower();
            if (language2 == "csharp1" || language2 == "csharp5")
                return ".cs";
            else if (language == "jscript")
                return ".js";
            else
                throw new PBException("error unknow language \"{0}\"", language);
        }

        private void SetFinalOutputAssembly()
        {
            //string sOutputAssembly = gsOutputAssembly;
            _finalOutputAssembly = _outputAssembly;
            //string sOutputDir = null;
            _finalOutputDir = null;
            //if (_outputDir != null)
            //    _finalOutputDir = _outputDir;
            if (_finalOutputAssembly != null)
            {
                string sDir = zPath.GetDirectoryName(_finalOutputAssembly);
                _finalOutputAssembly = zPath.GetFileName(_finalOutputAssembly);
                if (sDir != null && sDir != "")
                    _finalOutputDir = sDir.zRootPath(_projectDirectory);
            }
            if (_finalOutputDir == null) _finalOutputDir = "bin".zRootPath(_projectDirectory);
            if (_finalOutputAssembly != null)
            {
                //if (_generateExecutable)
                //    _finalOutputAssembly = zpath.PathSetExtension(_finalOutputAssembly, ".exe");
                //else
                //    _finalOutputAssembly = zpath.PathSetExtension(_finalOutputAssembly, ".dll");
                _finalOutputAssembly = _finalOutputAssembly.zRootPath(_finalOutputDir);
            }
        }

        private CompilerFile GetCompilerFileName(string filename)
        {
            filename = filename.ToLower();
            foreach (CompilerFile source in _sourceList.Values)
            {
                string filename2 = zPath.GetFileName(source.File).ToLower();
                if (filename == filename2)
                    return source;
            }
            return null;
        }

        private CompilerFile[] GetCompilerFilesType(params string[] types)
        {
            List<CompilerFile> SourcesList = new List<CompilerFile>();
            for (int i = 0; i < types.Length; i++)
                types[i] = types[i].ToLower();
            foreach (CompilerFile source in _sourceList.Values)
            {
                string ext = zPath.GetExtension(source.File).ToLower();
                foreach (string type in types)
                {
                    if (ext == type)
                        SourcesList.Add(source);
                }
            }
            return SourcesList.ToArray();
        }

        //private string[] GetFilesType(params string[] types)
        //{
        //    List<string> SourcesList = new List<string>();
        //    for (int i = 0; i < types.Length; i++)
        //        types[i] = types[i].ToLower();
        //    foreach (CompilerFile source in _sourceList.Values)
        //    {
        //        string ext = zPath.GetExtension(source.File).ToLower();
        //        foreach (string type in types)
        //        {
        //            if (ext == type)
        //                SourcesList.Add(source.File);
        //        }
        //    }
        //    return SourcesList.ToArray();
        //}

        private IEnumerable<string> GetFilesByType(params string[] types)
        {
            for (int i = 0; i < types.Length; i++)
                types[i] = types[i].ToLower();
            foreach (CompilerFile source in _sourceList.Values)
            {
                string ext = zPath.GetExtension(source.File).ToLower();
                foreach (string type in types)
                {
                    if (ext == type)
                        yield return source.File;
                }
            }
        }

        private IEnumerable<ReferencedAssembly> GetReferencedAssemblies()
        {
            foreach (CompilerAssembly assembly in _assemblyList.Values)
            {
                WriteLine(2, "  Assembly              \"{0}\" framework assembly {1} resolve {2}", assembly.File, assembly.FrameworkAssembly, assembly.Resolve);
                //yield return assembly.File;
                yield return new ReferencedAssembly { File = assembly.File, FrameworkAssembly = assembly.FrameworkAssembly };
            }
        }

        //
        //private IEnumerable<string> GetCompiledResources(string[] compiledResources)
        private IEnumerable<ResourceFile> GetCompiledResources(ResourceFile[] compiledResources)
        {
            foreach (ResourceFile compiledResource in compiledResources)
            {
                WriteLine(2, "  Resource              \"{0}\" namespace \"{1}\"", compiledResource.File, compiledResource.Namespace);
                yield return compiledResource;
            }
        }

        //public string[] CompileResources(CompilerFile[] resources)
        public ResourceFile[] CompileResources(CompilerFile[] resources)
        {
            string outputDir = zPath.Combine(_finalOutputDir, _CompileResourceSubDirectory);
            //string[] compiledResources = new string[resources.Length];
            ResourceFile[] compiledResources = new ResourceFile[resources.Length];
            int i = 0;
            foreach (CompilerFile resource in resources)
            {
                compiledResources[i++] = CompileResource(resource, outputDir);
            }
            return compiledResources;
        }

        //public string CompileResource(CompilerFile resource, string outputDir)
        public ResourceFile CompileResource(CompilerFile resource, string outputDir)
        {
            // _resourceError _resourceMessages
            string nameSpace = null;
            if (resource.Attributes.ContainsKey("namespace"))
                nameSpace = resource.Attributes["namespace"];
            ResourceFile resourceFile = new ResourceFile { File = resource.File, Namespace = nameSpace };

            //ResourceCompilerResults results = _resourceCompiler.Compile(resource.File, resource.Attributes, outputDir);
            ResourceCompilerResult result = _resourceCompiler.Compile(resourceFile, outputDir);
            if (!result.Success)
                _resourceError = true;
            foreach (ResourceCompilerMessage message in result.Messages)
                _resourceMessages.Add(message);
            //return results.CompiledResourceFile;
            resourceFile.File = result.CompiledResourceFile;
            return resourceFile;
        }

        //public string CompileResource(CompilerFile resource, string outputDir)
        //{
        //    // accès au paramètres de compilation des ressources d'un projet
        //    // namespace : Microsoft.VisualStudio.VCProjectEngine
        //    // class : VCManagedResourceCompilerTool
        //    //string sPathCompiledResource = cu.PathSetDir(cu.PathSetExt(resource, ".resources"), outputDir);

        //    if (!zDirectory.Exists(outputDir))
        //        zDirectory.CreateDirectory(outputDir);

        //    string resourceFile = resource.File;
        //    string resourceFilename = zPath.GetFileNameWithoutExtension(resourceFile);
        //    string nameSpace = null;
        //    if (resource.Attributes.ContainsKey("namespace"))
        //        nameSpace = resource.Attributes["namespace"];
        //    string pathCompiledResource = resourceFilename + ".resources";
        //    if (nameSpace != null)
        //        pathCompiledResource = nameSpace + "." + pathCompiledResource;
        //    //"WRunSource.Class.PibLink."
        //    pathCompiledResource = zpath.PathSetDirectory(pathCompiledResource, outputDir);
        //    if (zFile.Exists(pathCompiledResource) && zFile.Exists(resourceFile))
        //    {
        //        var fiResource = zFile.CreateFileInfo(resourceFile);
        //        var fiCompiledResource = zFile.CreateFileInfo(pathCompiledResource);
        //        if (fiCompiledResource.LastWriteTime > fiResource.LastWriteTime)
        //            return pathCompiledResource;
        //    }
        //    if (_resourceCompiler == null)
        //        throw new PBException("error resource compiler is not defined");
        //    if (!zFile.Exists(_resourceCompiler))
        //        throw new PBException("error resource compiler cannot be found {0}", _resourceCompiler);
        //    ProcessStartInfo pi = new ProcessStartInfo();
        //    pi.FileName = _resourceCompiler;
        //    //    Resgen.exe PibLink_resource.resx PibLink.PibLink_resource.resources /str:cs,PibLink,PibLink_resource
        //    pi.Arguments = resourceFile + " " + pathCompiledResource;
        //    if (nameSpace != null)
        //        pi.Arguments += " /str:cs," + nameSpace + "," + resourceFilename;
        //    WriteLine(1, "  {0} {1}", _resourceCompiler, pi.Arguments);
        //    pi.UseShellExecute = false;
        //    pi.RedirectStandardError = true;
        //    pi.WorkingDirectory = zPath.GetDirectoryName(resourceFile);

        //    Process p = new Process();
        //    p.StartInfo = pi;
        //    gsResourceCompiling = resourceFile;
        //    p.ErrorDataReceived += new DataReceivedEventHandler(CompileResource_EventErrorDataReceived);
        //    p.Start();
        //    p.BeginErrorReadLine();
        //    while (!p.HasExited)
        //    {
        //    }
        //    if (p.ExitCode != 0)
        //    {
        //        _resourceResults.Errors.Add(new ResourceCompilerError(resourceFile, string.Format("error compiling resource, exit code {0}", p.ExitCode)));
        //        _resourceResults.HasError = true;
        //    }
        //    return pathCompiledResource;
        //}

        //private string gsResourceCompiling;
        //private void CompileResource_EventErrorDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    if (e.Data == null) return;
        //    string s = e.Data.Trim();
        //    if (s == "") return;
        //    _resourceResults.Errors.Add(new ResourceCompilerError(gsResourceCompiling, s));
        //}

        // IProjectCompiler
        public DataTable GetCompilerMessagesDataTable()
        {
            //if ((_results == null || _results.Errors.Count == 0) && !_resourceResults.HasError)
            //if ((_results == null || _results.ErrorsCount == 0) && !_resourceResults.HasError)
            if ((_result == null || _result.MessagesCount == 0) && !_resourceError)
                return null;
            DataTable dt = new DataTable();
            dt.Columns.Add("Severity", typeof(string));
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Source", typeof(string));
            dt.Columns.Add("Line", typeof(int));
            dt.Columns.Add("Column", typeof(int));
            //dt.Columns.Add("Error", typeof(bool));
            dt.Columns.Add("Message", typeof(string));
            //foreach (ResourceCompilerError err in _resourceResults.Errors)
            foreach (ResourceCompilerMessage err in _resourceMessages)
                dt.Rows.Add(null, zPath.GetFileName(err.File), null, null, true, err.Message);
            if (_result != null)
            {
                //foreach (CompilerError err in _results.Errors)
                foreach (CompilerMessage message in _result.GetMessages())
                    //dt.Rows.Add(err.Id, zPath.GetFileName(err.FileName), err.Line, err.Column, !err.IsWarning, err.Message);
                    dt.Rows.Add(message.Severity.ToString(), message.Id, zPath.GetFileName(message.FileName), message.Line, message.Column, message.Message);
            }
            return dt;
        }

        private void CopyOutputToDirectories()
        {
            foreach (string directory in _copyOutputDirectories)
                CopyResultFilesToDirectory(directory);
        }

        private void CopyResultFilesToDirectory()
        {
            CopyResultFilesToDirectory(null);
        }

        // IProjectCompiler
        // if directory = null copy only referenced assembly, files, app.config to output directory
        public void CopyResultFilesToDirectory(string directory)
        {
            //if (_results.PathToAssembly == null)
            if (_result.GetAssemblyFile() == null)
                return;

            if (directory != null)
                WriteLine(1, "  copy result files to directory \"{0}\"", directory);

            if (!_generateInMemory && directory != null)
            {
                //string file = _results.PathToAssembly;
                string file = _result.GetAssemblyFile();
                if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                    WriteLine(2, "    copy assembly \"{0}\" to \"{1}\"", file, directory);

                file = zpath.PathSetExtension(file, ".pdb");
                if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                    WriteLine(2, "    copy assembly \"{0}\" to \"{1}\"", file, directory);

                if (_zipSourceFile != null)
                    zfile.CopyFileToDirectory(_zipSourceFile, directory, options: CopyFileOptions.OverwriteReadOnly);
            }

            if (directory == null)
            {
                //directory = zPath.GetDirectoryName(_results.PathToAssembly);
                directory = zPath.GetDirectoryName(_result.GetAssemblyFile());
                WriteLine(2, "  copy result files to directory \"{0}\"", directory);
            }

            //string sOutputDir = zpath.PathGetDirectory(gResults.PathToAssembly);
            if (!_generateInMemory)
            {
                foreach (CompilerAssembly compilerAssembly in _assemblyList.Values)
                {
                    string file = compilerAssembly.File;

                    //if (!File.Exists(assembly))
                    //    continue;

                    // test when copy to output directory, ?? when copy output to directory
                    if (zPath.GetDirectoryName(file) == "")
                        continue;

                    //string assembly2 = zpath.PathSetDirectory(assembly, directory);
                    //if (File.Exists(assembly2))
                    //{
                    //    FileInfo fi1 = new FileInfo(assembly);
                    //    FileInfo fi2 = new FileInfo(assembly2);
                    //    if (fi1.LastWriteTime <= fi2.LastWriteTime)
                    //        continue;
                    //}
                    //WriteLine(2, "Copy assembly \"{0}\" to \"{1}\"", assembly, assembly2);
                    //File.Copy(assembly, assembly2, true);

                    if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                        WriteLine(2, "    copy assembly \"{0}\" to \"{1}\"", file, directory);

                    //copiedFiles.Add(assembly2);
                    //string pdb = zpath.PathSetExtension(assembly, ".pdb");
                    //string pdb2 = zpath.PathSetExtension(assembly2, ".pdb");
                    //if (File.Exists(pdb))
                    //{
                    //    WriteLine(2, "Copy assembly \"{0}\" to \"{1}\"", pdb, pdb2);
                    //    File.Copy(pdb, pdb2, true);
                    //    //copiedFiles.Add(pdb2);
                    //}

                    file = zpath.PathSetExtension(file, ".pdb");
                    if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                        WriteLine(2, "    copy assembly debug info \"{0}\" to \"{1}\"", file, directory);

                    if (_copySourceFiles)
                    {
                        file = zpath.PathSetExtension(file, __zipSourceFilename);
                        if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                            WriteLine(2, "    copy assembly source file \"{0}\" to \"{1}\"", file, directory);
                    }

                }
            }

            foreach (CompilerFile compilerFile in _fileList.Values)
            {
                string destinationFile = null;
                if (compilerFile.Attributes.ContainsKey("destinationFile"))
                {
                    destinationFile = compilerFile.Attributes["destinationFile"];
                    WriteLine(2, "    copy file \"{0}\" to \"{1}\" as \"{2}\"", compilerFile.File, directory, destinationFile);
                }
                else
                    WriteLine(2, "    copy file \"{0}\" to \"{1}\"", compilerFile.File, directory);
                //string path = zfile.CopyFileToDirectory(file.File, directory, destinationFile, true);
                string path = zfile.CopyFileToDirectory(compilerFile.File, directory, destinationFile, CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                //if (path != null)
                //    copiedFiles.Add(path);
            }

            if (_appConfig != null)
            {
                //string appFile = zPath.GetFileName(_results.PathToAssembly) + ".config";
                string appFile = zPath.GetFileName(_result.GetAssemblyFile()) + ".config";
                WriteLine(2, "    copy file \"{0}\" to \"{1}\" as \"{2}\"", _appConfig.File, directory, appFile);
                //string path = zfile.CopyFileToDirectory(_appConfig.File, directory, appFile, true);
                string path = zfile.CopyFileToDirectory(_appConfig.File, directory, appFile, CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                //if (path != null)
                //    copiedFiles.Add(path);
            }

            //foreach (CompilerAssembly assembly in _assemblyList.Values)
            //{
            //    if (assembly.CopySource)
            //    {
            //        string zipSourceFile = GetZipSourceFile(assembly.File);
            //        if (zFile.Exists(zipSourceFile))
            //        {
            //            WriteLine(1, "  copy source file \"{0}\" to \"{1}\"", zipSourceFile, directory);
            //            zfile.CopyFileToDirectory(zipSourceFile, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
            //        }
            //        else
            //            WriteLine(1, "  warning can't copy source file, file does'nt exists \"{0}\"", zipSourceFile);
            //    }
            //}

            //}
            //return copiedFiles;

            //if (directory != null)
            //    TraceLevel = 1;
        }

        public void CopySourceFiles(string destinationDirectory)
        {
            if (zDirectory.Exists(destinationDirectory))
                zDirectory.Delete(destinationDirectory, true);
            CopySourceFiles(destinationDirectory, _sourceList.Values);
            CopySourceFiles(destinationDirectory, _fileList.Values);
            CopySourceFiles(destinationDirectory, _sourceFileList.Values);
        }

        private void CopySourceFiles(string destinationDirectory, IEnumerable<CompilerFile> files)
        {
            foreach (CompilerFile file in files)
            {
                //WriteLine(1, "Compiler.CopySourceFiles CompilerFile.File         : \"{0}\"", file.File);
                //WriteLine(1, "Compiler.CopySourceFiles CompilerFile.RelativePath : \"{0}\"", file.RelativePath);
                if (zFile.Exists(file.File))
                {
                    string destinationFile = zPath.Combine(destinationDirectory, file.RelativePath);
                    //WriteLine(1, "Compiler.CopySourceFiles destinationFile           : \"{0}\"", destinationFile);
                    zfile.CreateFileDirectory(destinationFile);
                    zfile.CopyFile(file.File, destinationFile, CopyFileOptions.OverwriteReadOnly);
                }
                else
                    WriteLine(1, "Compiler warning can't copy source file : source file \"{0}\" does not exist from project \"{1}\"", file.File, file.Project.ProjectFile);
            }
        }

        private void CopySourceFiles()
        {
            // FileMode.Create will raz existing zip file
            if (_outputAssembly == null)
                throw new PBException("can't zip source files, output assembly is not defined");
            //_zipSourceFile = _zipSourceFilename;
            //if (_projectCompilerFile != null)
            //{
            //    string projectFile = zPath.GetFileNameWithoutExtension(_projectCompilerFile.File);
            //    if (projectFile.EndsWith(".project"))
            //        projectFile = projectFile.Substring(0, projectFile.Length - 8);
            //    _zipSourceFile = projectFile + "." + _zipSourceFile;
            //}
            //_zipSourceFile = _zipSourceFile.zRootPath(zPath.GetDirectoryName(_outputAssembly));
            _zipSourceFile = GetZipSourceFile(_outputAssembly);
            WriteLine(1, "  create zip source file \"{0}\"", _zipSourceFile);
            using (ZipArchive zipArchive = new ZipArchive(_zipSourceFile, FileMode.Create))
            {
                if (_projectCompilerFile != null && zFile.Exists(_projectCompilerFile.File))
                    zipArchive.AddFile(_projectCompilerFile.File, _projectCompilerFile.RelativePath);
                ZipSourceFiles(zipArchive, _sourceList.Values);
                ZipSourceFiles(zipArchive, _fileList.Values);
                ZipSourceFiles(zipArchive, _sourceFileList.Values);
            }
        }

        private void _CopyRunSourceSourceFiles()
        {
            if (_copyRunSourceSourceFiles && _runsourceSourceDirectory != null)
            {
                foreach (string directory in _copyOutputDirectories)
                {
                    Trace.WriteLine("  copy runsource source files from \"{0}\" to \"{1}\"", _runsourceSourceDirectory, directory);
                    foreach (string file in zDirectory.EnumerateFiles(_runsourceSourceDirectory, "*" + ProjectCompiler.ZipSourceFilename))
                        zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                }
            }
        }

        private static string GetZipSourceFile(string assemblyFile)
        {
            return zPath.Combine(zPath.GetDirectoryName(assemblyFile), zPath.GetFileNameWithoutExtension(assemblyFile) + __zipSourceFilename);
        }

        private void ZipSourceFiles(ZipArchive zipArchive, IEnumerable<CompilerFile> files)
        {
            foreach (CompilerFile file in files)
            {
                if (zFile.Exists(file.File))
                    zipArchive.AddFile(file.File, file.RelativePath);
                else
                    WriteLine(1, "Compiler warning can't copy source file : source file \"{0}\" does not exist from project \"{1}\"", file.File, file.Project.ProjectFile);
            }
        }

        public void TraceMessages(Predicate<CompilerMessage> messageFilter = null)
        {
            if (_result != null)
            {
                //foreach (CompilerError err in _results.Errors)
                foreach (CompilerMessage message in _result.GetMessages(messageFilter))
                    //Trace.WriteLine("{0} no {1,-6} source \"{2}\" line {3} col {4} \"{5}\"", message.IsWarning ? "warning" : "error", message.Id, zPath.GetFileName(message.FileName), message.Line, message.Column, message.Message);
                    Trace.WriteLine("{0} {1,-6} source \"{2}\" line {3} col {4} \"{5}\"", message.Severity, message.Id, zPath.GetFileName(message.FileName), message.Line, message.Column, message.Message);
            }
            //if (_resourceResults != null)
            //{
            //    foreach (ResourceCompilerError err in _resourceResults.Errors)
            //        Trace.WriteLine("source \"{0}\" \"{1}\"", zPath.GetFileName(err.FileName), err.ErrorText);
            //}
            foreach (ResourceCompilerMessage err in _resourceMessages)
                Trace.WriteLine("source \"{0}\" \"{1}\"", zPath.GetFileName(err.File), err.Message);
        }

        private void WriteLine(int traceLevel, string msg, params object[] prm)
        {
            //if (Trace == null || traceLevel > TraceLevel)
            if (traceLevel > __traceLevel)
                return;
            Trace.WriteLine(msg, prm);
        }
    }
}
