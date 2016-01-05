using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.CSharp;
using Microsoft.JScript;
using pb;
using pb.Data.Xml;
using pb.IO;

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

namespace pb.Compiler
{
    public class CompilerException : Exception
    {
        public CompilerException(string sMessage) : base(sMessage) { }
        public CompilerException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public CompilerException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public CompilerException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }

    public class Compiler : ICompiler
    {
        public static int TraceLevel = 1;

        private CompilerFile _projectCompilerFile = null;
        private string _projectDirectory = null;
        private Dictionary<string, CompilerFile> _sourceList = new Dictionary<string, CompilerFile>();
        private CompilerFile _appConfig = null;
        private Dictionary<string, CompilerFile> _fileList = new Dictionary<string, CompilerFile>();
        private Dictionary<string, CompilerFile> _sourceFileList = new Dictionary<string, CompilerFile>();
        private Dictionary<string, CompilerAssembly> _assemblyList = new Dictionary<string, CompilerAssembly>();
        private string _language = null;           // CSharp, JScript
        private Dictionary<string, string> _providerOption = new Dictionary<string, string>();
        private string _resourceCompiler = null;
        private bool _generateInMemory = false;
        private bool _generateExecutable = false;
        private bool _debugInformation = false;
        private int _warningLevel = -1;
        private string _outputDir = null;
        private string _finalOutputDir = null;
        private string _CompileResourceSubDirectory = "resources";
        private string _outputAssembly = null;
        private string _finalOutputAssembly = null;
        private string _compilerOptions = null;
        private ResourceCompilerResults _resourceResults = new ResourceCompilerResults();
        private CompilerResults _results = null;
        private List<string> _copyOutputDirectories = new List<string>();
        private static string __zipSourceFilename = ".source.zip";
        private bool _copySourceFiles = false;
        private bool _copyRunSourceSourceFiles = false;
        private string _zipSourceFile = null;

        public Compiler()
        {
        }

        //public string DefaultDirectory { get { return _defaultDirectory; } set { _defaultDirectory = value; } }
        public IEnumerable<CompilerFile> SourceList { get { return _sourceList.Values; } }
        public IEnumerable<CompilerFile> FileList { get { return _fileList.Values; } }
        public IEnumerable<CompilerFile> SourceFileList { get { return _sourceFileList.Values; } }
        public Dictionary<string, CompilerAssembly> Assemblies { get { return _assemblyList; } }
        public string Language { get { return _language; } set { _language = value; } }
        public Dictionary<string, string> ProviderOption { get { return _providerOption; } }
        public string ResourceCompiler { get { return _resourceCompiler; } set { _resourceCompiler = value; } }
        public bool GenerateInMemory { get { return _generateInMemory; } set { _generateInMemory = value; } }
        public bool GenerateExecutable { get { return _generateExecutable; } set { _generateExecutable = value; } }
        public bool DebugInformation { get { return _debugInformation; } set { _debugInformation = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public string OutputDir { get { return _outputDir; } set { _outputDir = value; } }
        public string OutputAssembly { get { return _outputAssembly; } }
        public string CompilerOptions { get { return _compilerOptions; } set { _compilerOptions = value; } }
        public ResourceCompilerResults ResourceResults { get { return _resourceResults; } }
        public CompilerResults Results { get { return _results; } }
        public IEnumerable<string> CopyOutputDirectories { get { return _copyOutputDirectories; } }
        public static string ZipSourceFilename { get { return __zipSourceFilename; } }
        public bool CopyRunSourceSourceFiles { get { return _copyRunSourceSourceFiles; } }

        // ICompiler
        public bool HasError()
        {
            if ((_results != null && _results.Errors.HasErrors) || _resourceResults.HasError)
                return true;
            else
                return false;
        }

        public void SetProjectCompilerFile(CompilerFile projectCompilerFile)
        {
            _projectCompilerFile = projectCompilerFile;
            _projectDirectory = zPath.GetDirectoryName(projectCompilerFile.File);
        }

        // runCode : true when executing code from runsource, true for CompilerDefaultValues and ProjectDefaultValues, otherwise false
        // bool dontSetOutput = false
        public void SetParameters(ICompilerProject project, bool runCode = false)
        {
            if (project == null)
                return;
            //if (!includeProject)
            //{
                //compiler.Language = xe.Get("Language", compiler.Language);
                string s = project.GetLanguage();
                if (s != null)
                {
                    if (_language != null)
                        WriteLine(1, "Compiler warning : redefine language \"{0}\" as \"{1}\" from project \"{2}\"", _language, s, project.ProjectFile);
                    _language = s;
                }

                //foreach (XElement xe2 in xe.GetElements("ProviderOption"))
                //    compiler.SetProviderOption(xe2.zAttribValue("name"), xe2.zAttribValue("value"));
                SetProviderOptions(project.GetProviderOptions(), project);

                //compiler.ResourceCompiler = xe.Get("ResourceCompiler", compiler.ResourceCompiler);
                s = project.GetResourceCompiler();
                if (s != null)
                {
                    if (_resourceCompiler != null)
                        WriteLine(1, "Compiler warning : redefine resource compiler \"{0}\" as \"{1}\" from project \"{2}\"", _resourceCompiler, s, project.ProjectFile);
                    _resourceCompiler = s;
                }

                //compiler.OutputDir = xe.Get("OutputDir", compiler.OutputDir);
                if (!runCode)
                {
                    s = project.GetOutputDir();
                    if (s != null)
                    {
                        if (_outputDir != null)
                            WriteLine(1, "Compiler warning : redefine output directory \"{0}\" as \"{1}\" from project \"{2}\"", _outputDir, s, project.ProjectFile);
                        _outputDir = s;
                    }
                }

                //compiler.OutputAssembly = xe.Get("Output", compiler.OutputAssembly);
                if (!runCode)
                {
                    s = project.GetOutput();
                    if (s != null)
                        SetOutputAssembly(s, project);
                }
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

                bool? b;
                //compiler.GenerateExecutable = xe.Get("GenerateExecutable").zTryParseAs(compiler.GenerateExecutable);
                if (!runCode)
                {
                    b = project.GetGenerateExecutable();
                    if (b != null)
                        _generateExecutable = (bool)b;
                }

                //compiler.GenerateInMemory = xe.Get("GenerateInMemory").zTryParseAs(compiler.GenerateInMemory);
                if (!runCode)
                {
                    b = project.GetGenerateInMemory();
                    if (b != null)
                        _generateInMemory = (bool)b;
                }

                //compiler.DebugInformation = xe.Get("DebugInformation").zTryParseAs(compiler.DebugInformation);
                b = project.GetDebugInformation();
                if (b != null)
                    _debugInformation = (bool)b;

                //compiler.WarningLevel = xe.Get("WarningLevel").zTryParseAs<int>(compiler.WarningLevel);
                int? i = project.GetWarningLevel();
                if (i != null)
                    _warningLevel = (int)i;

                //compiler.AddCompilerOptions(xe.GetValues("CompilerOptions"));
                AddCompilerOptions(project.GetCompilerOptions());

                if (!runCode)
                {
                    b = project.GetCopySourceFiles();
                    if (b != null)
                        _copySourceFiles = (bool)b;

                    b = project.GetCopyRunSourceSourceFiles();
                    if (b != null)
                        _copyRunSourceSourceFiles = (bool)b;
                }

                //string keyfile = xe.Get("KeyFile");
                s = project.GetKeyFile();
                if (s != null)
                    AddCompilerOption("/keyfile:\"" + s + "\"");

                //string target = xe.Get("Target");
                if (!runCode)
                {
                    s = project.GetTarget();
                    if (s != null)
                        AddCompilerOption("/target:" + s);
                }

                //string icon = xe.Get("Icon");
                s = project.GetIcon();
                if (s != null)
                    //AddCompilerOption("/win32icon:" + s);
                    AddCompilerOption("/win32icon:\"" + s + "\"");
            //}

            foreach (ICompilerProject project2 in project.GetIncludeProjects())
            {
                SetParameters(project2, runCode: runCode);
            }

            //compiler.AddSources(xe.GetElements("Source"));
            AddSources(project.GetSources());

            //compiler.AddFiles(xe.GetElements("File"));  // compiler.DefaultDir
            AddFiles(project.GetFiles());

            AddSourceFiles(project.GetSourceFiles());

            //compiler.AddAssemblies(xe.GetElements("Assembly"));
            AddAssemblies(project.GetAssemblies());

            //compiler.AddLocalAssemblies(xe.GetElements("LocalAssembly"));

            //compiler.AddCopyOutputDirectories(xe.GetValues("CopyOutput"));
            if (!runCode)
            {
                AddCopyOutputDirectories(project.GetCopyOutputs());
            }
        }

        public void SetOutputAssembly(string outputAssembly, ICompilerProject project = null)
        {
            if (outputAssembly != null)
            {
                if (_outputAssembly != null)
                    WriteLine(1, "Compiler warning : redefine output assembly \"{0}\" as \"{1}\" from project \"{2}\"", _outputAssembly, outputAssembly, project.ProjectFile);
                _outputAssembly = outputAssembly;
                string ext = zPath.GetExtension(outputAssembly);
                if (ext != null)
                {
                    if (ext.ToLower() == ".exe")
                        _generateExecutable = true;
                    else if (ext.ToLower() == ".dll")
                        _generateExecutable = false;
                }
            }
        }

        public void SetProviderOptions(IEnumerable<CompilerProviderOption> options, ICompilerProject project = null)
        {
            foreach (CompilerProviderOption option in options)
            {
                if (_providerOption.ContainsKey(option.Name))
                {
                    WriteLine(1, "Compiler warning : redefine provider option \"{0}\" value \"{1}\" as \"{2}\" from project \"{3}\"", option.Name, _providerOption[option.Name], option.Value, project != null ? project.ProjectFile : "--no project--");
                    _providerOption.Remove(option.Name);
                }
                _providerOption.Add(option.Name, option.Value);
            }
        }

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
            //_sourceList.AddRange(sources);
            foreach (CompilerFile source in sources)
                AddSource(source);
        }

        public void AddSource(CompilerFile source)
        {
            if (!_sourceList.ContainsKey(source.File))
                _sourceList.Add(source.File, source);
            else
            {
                WriteLine(1, "Compiler warning : duplicate source file \"{0}\" from project \"{1}\"", source.File, source.Project.ProjectFile);
                WriteLine(1, "  already loaded from project \"{0}\"", _sourceList[source.File].Project.ProjectFile);
            }
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

        public void Compile()
        {
            if (!_generateInMemory && _outputAssembly == null)
                throw new CompilerException("output assembly is not defined");
            SetFinalOutputAssembly();
            if (_finalOutputDir != null)
                zDirectory.CreateDirectory(_finalOutputDir);
            //WriteLine(1, "Compile \"{0}\"", gsFinalOutputAssembly);
            //_defaultDir
            ////WriteLine(1, "Compiler _defaultDir \"{0}\"", _defaultDir);
            WriteLine(2, "Compile \"{0}\"", _finalOutputAssembly);
            WriteLine(2, "  DebugInformation      {0}", _debugInformation);
            WriteLine(2, "  GenerateInMemory      {0}", _generateInMemory);
            WriteLine(2, "  GenerateExecutable    {0}", _generateExecutable);
            WriteLine(2, "  WarningLevel          {0}", _warningLevel);
            WriteLine(2, "  CompilerOptions       \"{0}\"", _compilerOptions);

            _appConfig = GetCompilerFileName("app.config");
            if (_appConfig != null)
                WriteLine(2, "  app.config            \"{0}\"", _appConfig.File);

            CompilerParameters options = new CompilerParameters();
            options.CompilerOptions = _compilerOptions;
            options.GenerateInMemory = _generateInMemory;
            options.OutputAssembly = _finalOutputAssembly;
            options.GenerateExecutable = _generateExecutable;
            options.IncludeDebugInformation = _debugInformation;
            // WarningLevel : from http://msdn.microsoft.com/en-us/library/13b90fz7.aspx
            //   0 Turns off emission of all warning messages.
            //   1 Displays severe warning messages.
            //   2 Displays level 1 warnings plus certain, less-severe warnings, such as warnings about hiding class members.
            //   3 Displays level 2 warnings plus certain, less-severe warnings, such as warnings about expressions that always evaluate to true or false.
            //   4 (the default) Displays all level 3 warnings plus informational warnings.
            options.WarningLevel = _warningLevel;
            //foreach (string s in gAssemblyList.Values)
            foreach (CompilerAssembly assembly in _assemblyList.Values)
            {
                WriteLine(2, "  Assembly              \"{0}\" resolve {1}", assembly.File, assembly.Resolve);
                options.ReferencedAssemblies.Add(assembly.File);
                if (assembly.Resolve)
                    AssemblyResolve.Add(assembly.File, assembly.ResolveName);
            }
            CompilerFile[] resources = GetCompilerFilesType(".resx");
            //string[] compiledResources = CompileResources(resources, gsFinalOutputDir);
            string[] compiledResources = CompileResources(resources);
            foreach (string compiledResource in compiledResources)
            {
                WriteLine(2, "  Resource              \"{0}\"", compiledResource);
                options.EmbeddedResources.Add(compiledResource);
            }

            WriteLine(2, "  Resource error        {0}", _resourceResults.Errors.Count);
            if (_resourceResults.HasError) return;

            //CSharpCodeProvider provider = new CSharpCodeProvider(gProviderOption);
            CodeDomProvider provider = null;
            string sourceExt = null;
            //gLanguage  CSharp, JScript
            if (_language == null)
                throw new CompilerException("error undefined language");
            string language = _language.ToLower();
            if (language == "csharp")
            {
                provider = new CSharpCodeProvider(_providerOption);
                sourceExt = ".cs";
            }
            else if (language == "jscript")
            {
                provider = new JScriptCodeProvider();
                sourceExt = ".js";
            }
            else
                throw new CompilerException("error unknow language \"{0}\"", _language);
            //string[] sSources = GetFilesType(".cs");
            string[] sources = GetFilesType(sourceExt);
            //string currentDirectory = zDirectory.GetCurrentDirectory();
            //Directory.SetCurrentDirectory(zapp.GetAppDirectory());
            //cTrace.Trace("Compiler.Compile() : change current directory to {0}", cu.GetAppDirectory());
            _results = provider.CompileAssemblyFromFile(options, sources);
            WriteLine(2, "  Compile error warning {0}", _results.Errors.Count);
            WriteLine(2, "  Compile has error     {0}", _results.Errors.HasErrors);
            WriteLine(2, "  Compile has warning   {0}", _results.Errors.HasWarnings);
            //Directory.SetCurrentDirectory(currentDirectory);
            //cTrace.Trace("Compiler.Compile() : restore current directory to {0}", currentDirectory);
            provider.Dispose();

            //CopyAssemblyToOutputDir();
            //if (gResults.PathToAssembly != null && !gbGenerateInMemory)
            //{
            //    List<string> copiedFiles = CopyReferencedAssembliesToDirectory(zpath.PathGetDirectory(gResults.PathToAssembly));
            //    _outputFiles.AddRange(copiedFiles);
            //}

            //CopyFileToOutputDir();
            //if (gResults.PathToAssembly != null)
            //{
            //    List<string> copiedFiles = CopyFilesToDirectory(zpath.PathGetDirectory(gResults.PathToAssembly));
            //    _outputFiles.AddRange(copiedFiles);
            //}

            if (_copySourceFiles)
                CopySourceFiles();

            if (_results.PathToAssembly != null)
                CopyResultFilesToDirectory();

            CopyOutputToDirectories();
        }

        private void SetFinalOutputAssembly()
        {
            //string sOutputAssembly = gsOutputAssembly;
            _finalOutputAssembly = _outputAssembly;
            //string sOutputDir = null;
            _finalOutputDir = null;
            if (_outputDir != null)
                //_finalOutputDir = _outputDir.zRootPath(_defaultDir);
                _finalOutputDir = _outputDir;
            else if (_finalOutputAssembly != null)
            {
                string sDir = zPath.GetDirectoryName(_finalOutputAssembly);
                _finalOutputAssembly = zPath.GetFileName(_finalOutputAssembly);
                if (sDir != null && sDir != "")
                    _finalOutputDir = sDir.zRootPath(_projectDirectory);
            }
            if (_finalOutputDir == null) _finalOutputDir = "bin".zRootPath(_projectDirectory);
            if (_finalOutputAssembly != null)
            {
                if (_generateExecutable)
                    _finalOutputAssembly = zpath.PathSetExtension(_finalOutputAssembly, ".exe");
                else
                    _finalOutputAssembly = zpath.PathSetExtension(_finalOutputAssembly, ".dll");
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

        private string[] GetFilesType(params string[] types)
        {
            List<string> SourcesList = new List<string>();
            for (int i = 0; i < types.Length; i++)
                types[i] = types[i].ToLower();
            foreach (CompilerFile source in _sourceList.Values)
            {
                string ext = zPath.GetExtension(source.File).ToLower();
                foreach (string type in types)
                {
                    if (ext == type)
                        SourcesList.Add(source.File);
                }
            }
            return SourcesList.ToArray();
        }

        public string[] CompileResources(CompilerFile[] resources)
        {
            string outputDir = zPath.Combine(_finalOutputDir, _CompileResourceSubDirectory);
            string[] compiledResources = new string[resources.Length];
            int i = 0;
            foreach (CompilerFile resource in resources)
            {
                compiledResources[i++] = CompileResource(resource, outputDir);
            }
            return compiledResources;
        }

        public string CompileResource(CompilerFile resource, string outputDir)
        {
            // Utilisation de Resgen.exe http://msdn.microsoft.com/fr-fr/library/ccec7sz1.aspx
            // resgen [parameters] [/compile]filename.extension [outputFilename.extension] [/str:lang[,namespace[,class[,file]]]]

            //   /compile
            //      Permet de spécifier plusieurs fichiers .resx ou texte à convertir en plusieurs fichiers .resources en une seule opération globale.
            //      Si vous omettez cette option, vous ne pouvez spécifier qu'un seul argument de fichier d'entrée.
            //      Cette option ne peut pas être utilisée avec l'option /str:.
            //   /publicClass
            //      Crée un type de ressources fortement typé comme classe public.
            //      Cette option est ignorée si l'option /str: n'est pas utilisée.
            //   /r:assembly
            //      Spécifie que les types doivent être chargés à partir d'un assembly.
            //      Si vous spécifiez cette option, un fichier .resx avec une version antérieure d'un type utilisera le type dans un assembly.
            //   /str:language[,namespace[,classname[,filename]]]
            //      Crée un fichier de classe de ressources fortement typé dans le langage de programmation (cs ou C# pour C#, vb ou visualbasic pour Visual Basic)
            //      spécifié dans l'option language.
            //      Vous pouvez utiliser l'option namespace pour spécifier l'espace de noms par défaut du projet,
            //      l'option classname pour spécifier le nom de la classe générée et l'option filename pour spécifier le nom du fichier de classe.
            //      Remarque	Dans le .NET Framework version 2.0, classname et filename sont ignorés si namespace n'est pas spécifié. 
            //      Un seul fichier d'entrée est autorisé lorsque l'option /str: est utilisée, afin qu'il ne puisse pas être utilisé avec l'option /compile.
            //      Si namespace est spécifié mais que classname ne l'est pas, le nom de la classe est dérivé du nom de fichier de sortie
            //      (par exemple, les traits de soulignement sont substitués pour les périodes).Les ressources fortement typées peuvent ne pas fonctionner correctement en conséquence.Pour éviter ce problème, spécifiez à la fois le nom de la classe et le nom du fichier de sortie.
            //   /usesourcepath
            //      Précise que le répertoire actif du fichier d'entrée sera utilisé pour résoudre des chemins d'accès de fichier relatif.
            //
            //  commande utilisée :
            //    Resgen.exe resource.resx resource.resources /str:cs,PibLink,PibLink_resource
            //    Resgen.exe PibLink_resource.resx WRunSource.Class.PibLink.PibLink_resource.resources /str:cs,WRunSource.Class.PibLink,PibLink_resource



            // accès au paramètres de compilation des ressources d'un projet
            // namespace : Microsoft.VisualStudio.VCProjectEngine
            // class : VCManagedResourceCompilerTool
            //string sPathCompiledResource = cu.PathSetDir(cu.PathSetExt(resource, ".resources"), outputDir);

            if (!zDirectory.Exists(outputDir))
                zDirectory.CreateDirectory(outputDir);

            string resourceFile = resource.File;
            string resourceFilename = zPath.GetFileNameWithoutExtension(resourceFile);
            string nameSpace = null;
            //int i = resource.Attributes.IndexOfKey("namespace");
            //if (i != -1) nameSpace = resource.Attributes.Values[i];
            if (resource.Attributes.ContainsKey("namespace"))
                nameSpace = resource.Attributes["namespace"];
            string sPathCompiledResource = resourceFilename + ".resources";
            if (nameSpace != null)
                sPathCompiledResource = nameSpace + "." + sPathCompiledResource;
            //"WRunSource.Class.PibLink."
            sPathCompiledResource = zpath.PathSetDirectory(sPathCompiledResource, outputDir);
            if (zFile.Exists(sPathCompiledResource) && zFile.Exists(resourceFile))
            {
                //FileInfo fiResource = new FileInfo(resourceFile);
                var fiResource = zFile.CreateFileInfo(resourceFile);
                //FileInfo fiCompiledResource = new FileInfo(sPathCompiledResource);
                var fiCompiledResource = zFile.CreateFileInfo(sPathCompiledResource);
                if (fiCompiledResource.LastWriteTime > fiResource.LastWriteTime)
                    return sPathCompiledResource;
            }
            if (_resourceCompiler == null)
                throw new CompilerException("error resource compiler is not defined");
            if (!zFile.Exists(_resourceCompiler))
                throw new CompilerException("error resource compiler cannot be found {0}", _resourceCompiler);
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = _resourceCompiler;
            //pi.Arguments = cu.PathGetFileWithExt(resource);
            //pi.Arguments = resource + " " + sPathCompiledResource;
            //    Resgen.exe PibLink_resource.resx PibLink.PibLink_resource.resources /str:cs,PibLink,PibLink_resource
            //pi.Arguments = resource + " " + sPathCompiledResource + " /str:cs";
            pi.Arguments = resourceFile + " " + sPathCompiledResource;
            if (nameSpace != null) pi.Arguments += " /str:cs," + nameSpace + "," + resourceFilename;
            //cTrace.Trace("{0} {1}", gsResourceCompiler, pi.Arguments);
            WriteLine(1, "  {0} {1}", _resourceCompiler, pi.Arguments);
            //pi.WorkingDirectory = cu.PathGetDir(resource);
            //pi.WorkingDirectory = outputDir;
            pi.UseShellExecute = false;
            pi.RedirectStandardError = true;
            //pi.WorkingDirectory = gsDefaultDir;
            pi.WorkingDirectory = zPath.GetDirectoryName(resourceFile);

            Process p = new Process();
            p.StartInfo = pi;
            gsResourceCompiling = resourceFile;
            p.ErrorDataReceived += new DataReceivedEventHandler(CompileResource_EventErrorDataReceived);
            p.Start();
            p.BeginErrorReadLine();
            while (!p.HasExited)
            {
            }
            if (p.ExitCode != 0)
            {
                _resourceResults.Errors.Add(new ResourceCompilerError(resourceFile, string.Format("error compiling resource, exit code {0}", p.ExitCode)));
                _resourceResults.HasError = true;
            }
            return sPathCompiledResource;
        }

        private string gsResourceCompiling;
        private void CompileResource_EventErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null) return;
            string s = e.Data.Trim();
            if (s == "") return;
            _resourceResults.Errors.Add(new ResourceCompilerError(gsResourceCompiling, s));
        }

        // ICompiler
        public DataTable GetCompilerMessagesDataTable()
        {
            if ((_results == null || _results.Errors.Count == 0) && !_resourceResults.HasError) return null;
            DataTable dt = new DataTable();
            dt.Columns.Add("ErrorNumber", typeof(string));
            dt.Columns.Add("Source", typeof(string));
            dt.Columns.Add("Line", typeof(int));
            dt.Columns.Add("Column", typeof(int));
            dt.Columns.Add("Error", typeof(bool));
            dt.Columns.Add("Message", typeof(string));
            foreach (ResourceCompilerError err in _resourceResults.Errors)
                dt.Rows.Add(null, zPath.GetFileName(err.FileName), null, null, true, err.ErrorText);
            if (_results != null)
            {
                foreach (CompilerError err in _results.Errors)
                {
                    //DataRow row = dt.NewRow();
                    //row["ErrorNumber"] = err.ErrorNumber;
                    //row["Source"] = zPath.GetFileName(err.FileName);
                    //row["Line"] = err.Line;
                    //row["Column"] = err.Column;
                    //row["Error"] = !err.IsWarning;
                    //row["Message"] = err.ErrorText;
                    //dt.Rows.Add(row);
                    dt.Rows.Add(err.ErrorNumber, zPath.GetFileName(err.FileName), err.Line, err.Column, !err.IsWarning, err.ErrorText);
                }
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

        // ICompiler
        public void CopyResultFilesToDirectory(string directory)
        {
            //List<string> copiedFiles = new List<string>();

            //if (gResults.PathToAssembly == null || gbGenerateInMemory)
            //    return;

            if (_results.PathToAssembly == null)
                return;

            if (directory != null)
                WriteLine(1, "  copy result files to directory \"{0}\"", directory);

            //if (directory != null)
            //    TraceLevel = 2;

            //if (_results.PathToAssembly != null)
            //{

            if (!_generateInMemory && directory != null)
            {
                string file = _results.PathToAssembly;
                if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                    WriteLine(2, "  copy assembly \"{0}\" to \"{1}\"", file, directory);

                file = zpath.PathSetExtension(file, ".pdb");
                if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                    WriteLine(2, "  copy assembly \"{0}\" to \"{1}\"", file, directory);

                if (_zipSourceFile != null)
                    zfile.CopyFileToDirectory(_zipSourceFile, directory, options: CopyFileOptions.OverwriteReadOnly);
            }

            if (directory == null)
                directory = zPath.GetDirectoryName(_results.PathToAssembly);

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
                        WriteLine(2, "  copy assembly \"{0}\" to \"{1}\"", file, directory);

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
                        WriteLine(2, "  copy assembly debug info \"{0}\" to \"{1}\"", file, directory);

                    if (_copySourceFiles)
                    {
                        file = zpath.PathSetExtension(file, __zipSourceFilename);
                        if (zfile.CopyFileToDirectory(file, directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer) != null)
                            WriteLine(2, "  copy assembly source file \"{0}\" to \"{1}\"", file, directory);
                    }

                }
            }

            foreach (CompilerFile compilerFile in _fileList.Values)
            {
                string destinationFile = null;
                if (compilerFile.Attributes.ContainsKey("destinationFile"))
                {
                    destinationFile = compilerFile.Attributes["destinationFile"];
                    WriteLine(2, "  copy file \"{0}\" to \"{1}\" as \"{2}\"", compilerFile.File, directory, destinationFile);
                }
                else
                    WriteLine(2, "  copy file \"{0}\" to \"{1}\"", compilerFile.File, directory);
                //string path = zfile.CopyFileToDirectory(file.File, directory, destinationFile, true);
                string path = zfile.CopyFileToDirectory(compilerFile.File, directory, destinationFile, CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
                //if (path != null)
                //    copiedFiles.Add(path);
            }

            if (_appConfig != null)
            {
                string appFile = zPath.GetFileName(_results.PathToAssembly) + ".config";
                WriteLine(2, "  copy file \"{0}\" to \"{1}\" as \"{2}\"", _appConfig.File, directory, appFile);
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
                throw new CompilerException("can't zip source files, output assembly is not defined");
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

        public void TraceMessages()
        {
            if (_results != null)
            {
                foreach (CompilerError err in _results.Errors)
                    pb.Trace.WriteLine("{0} no {1,-6} source \"{2}\" line {3} col {4} \"{5}\"", err.IsWarning ? "warning" : "error", err.ErrorNumber, zPath.GetFileName(err.FileName), err.Line, err.Column, err.ErrorText);
            }
            if (_resourceResults != null)
            {
                foreach (ResourceCompilerError err in _resourceResults.Errors)
                    pb.Trace.WriteLine("source \"{0}\" \"{1}\"", zPath.GetFileName(err.FileName), err.ErrorText);
            }
        }

        private void WriteLine(int traceLevel, string msg, params object[] prm)
        {
            //if (Trace == null || traceLevel > TraceLevel)
            if (traceLevel > TraceLevel)
                return;
            pb.Trace.WriteLine(msg, prm);
        }
    }
}
