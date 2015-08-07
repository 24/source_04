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
 * <CompilerOptions                           value = "" />              http://msdn.microsoft.com/en-us/library/2fdbz5xd.aspx   /define:DEBUG;TRACE
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
    #region class CompilerException
    public class CompilerException : Exception
    {
        public CompilerException(string sMessage) : base(sMessage) { }
        public CompilerException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public CompilerException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public CompilerException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    public class Compiler : ICompiler
    {
        //public static ITrace Trace = pb.Trace.CurrentTrace;
        public static int TraceLevel = 1;

        private string _defaultDir = null;
        private List<CompilerFile> _sourceList = new List<CompilerFile>();
        private CompilerFile _appConfig = null;
        private List<CompilerFile> _fileList = new List<CompilerFile>();
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

        public Compiler()
        {
        }

        public string DefaultDir { get { return _defaultDir; } set { _defaultDir = value; } }
        public IEnumerable<CompilerFile> SourceList { get { return _sourceList; } }  // set { _sourceList = value; }
        public IEnumerable<CompilerFile> FileList { get { return _fileList; } }  //  set { _fileList = value; }
        //public Dictionary<string, CompilerAssembly> AssemblyList { get { return _assemblyList; } set { _assemblyList = value; } }
        public IEnumerable<CompilerAssembly> AssemblyList { get { return _assemblyList.Values; } }
        public string Language { get { return _language; } set { _language = value; } }
        public Dictionary<string, string> ProviderOption { get { return _providerOption; } }  // set { _providerOption = value; }
        public string ResourceCompiler { get { return _resourceCompiler; } set { _resourceCompiler = value; } }
        public bool GenerateInMemory { get { return _generateInMemory; } set { _generateInMemory = value; } }
        public bool GenerateExecutable { get { return _generateExecutable; } set { _generateExecutable = value; } }
        public bool DebugInformation { get { return _debugInformation; } set { _debugInformation = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public string OutputDir { get { return _outputDir; } set { _outputDir = value; } }
        //public string FinalOutputDir { get { SetFinalOutputAssembly(); return _finalOutputDir; } }
        public string OutputAssembly { get { return _outputAssembly; } set { _outputAssembly = value; } }
        //public string FinalOutputAssembly { get { SetFinalOutputAssembly(); return _finalOutputAssembly; } }
        //public string CompiledAssemblyPath { get { if (_results != null) return _results.PathToAssembly; else return null; } }
        public string CompilerOptions { get { return _compilerOptions; } set { _compilerOptions = value; } }
        public ResourceCompilerResults ResourceResults { get { return _resourceResults; } }
        public CompilerResults Results { get { return _results; } }
        public IEnumerable<string> CopyOutputDirectories { get { return _copyOutputDirectories; } }

        //public bool HasError { get { if ((_results != null && _results.Errors.HasErrors) || _resourceResults.HasError) return true; else return false; } }
        public bool HasError()
        {
            if ((_results != null && _results.Errors.HasErrors) || _resourceResults.HasError)
                return true;
            else
                return false;
        }

        //public void SetCompilerParameters(string xmlFile)
        //{
        //    XDocument xd = XDocument.Load(xmlFile);
        //    SetCompilerParameters(xd.Root);
        //}

        //public void SetCompilerParameters(XmlConfig config, string sXPathParameter)
        //{
        //    bool bDefaultDir = false;
        //    if (gsDefaultDir != null) bDefaultDir = true;
        //    if (!bDefaultDir) gsDefaultDir = zPath.GetDirectoryName(config.ConfigPath);
        //    SetCompilerParameters(config.XDocument.Root.XPathSelectElement(sXPathParameter));
        //    if (!bDefaultDir) gsDefaultDir = zPath.GetDirectoryName(config.ConfigLocalPath);
        //    SetCompilerParameters(config.LocalXDocument.Root.XPathSelectElement(sXPathParameter));
        //    if (!bDefaultDir) gsDefaultDir = null;
        //}

        //public void SetCompilerParameters(XElement xe)
        //{
        //    _outputDir = xe.zXPathValue("OutputDir", _outputDir);
        //    _outputAssembly = xe.zXPathValue("Output", _outputAssembly);
        //    string s = zPath.GetExtension(_outputAssembly);
        //    if (s != null)
        //    {
        //        if (s.ToLower() == ".exe")
        //            _generateExecutable = true;
        //        else if (s.ToLower() == ".dll")
        //            _generateExecutable = false;
        //    }
        //    _generateExecutable = xe.zXPathValue("GenerateExecutable").zTryParseAs(_generateExecutable);
        //    _generateInMemory = xe.zXPathValue("GenerateInMemory").zTryParseAs(_generateInMemory);
        //    _debugInformation = xe.zXPathValue("DebugInformation").zTryParseAs(_debugInformation);
        //    _warningLevel = xe.zXPathValue("WarningLevel").zTryParseAs<int>(_warningLevel);
        //    AddCompilerOptions(xe.zXPathValues("CompilerOptions"));
        //    string sKeyfile = xe.zXPathValue("KeyFile");
        //    if (sKeyfile != null)
        //        AddCompilerOption("/keyfile:\"" + zpath.PathMakeRooted(sKeyfile, _defaultDir) + "\"");
        //    string sTarget = xe.zXPathValue("Target");
        //    if (sTarget != null)
        //        AddCompilerOption("/target:" + sTarget);
        //    string sIcon = xe.zXPathValue("Icon");
        //    if (sIcon != null)
        //        AddCompilerOption("/win32icon:" + zpath.PathMakeRooted(sIcon, _defaultDir));
        //    if (xe != null)
        //        AddSources(xe.Elements("Source"));
        //    AddFiles(_fileList, xe.zXPathElements("File"), _defaultDir);
        //    AddAssemblies(xe.zXPathElements("Assembly"));
        //    AddLocalAssemblies(xe.zXPathElements("LocalAssembly"));

        //    _language = xe.zXPathValue("Language", _language);
        //    foreach (XElement xe2 in xe.zXPathElements("ProviderOption"))
        //        SetProviderOption(xe2.zAttribValue("name"), xe2.zAttribValue("value"));
        //    s = xe.zXPathValue("ResourceCompiler");
        //    if (s != null)
        //        _resourceCompiler = s;

        //    AddCopyOutputDirectories(xe.zXPathValues("CopyOutput"));
        //}

        //public void SetCompilerParameters(XmlConfigElement xe)
        //{
        //    _outputDir = xe.Get("OutputDir", _outputDir);
        //    _outputAssembly = xe.Get("Output", _outputAssembly);
        //    string s = zPath.GetExtension(_outputAssembly);
        //    if (s != null)
        //    {
        //        if (s.ToLower() == ".exe")
        //            _generateExecutable = true;
        //        else if (s.ToLower() == ".dll")
        //            _generateExecutable = false;
        //    }
        //    _generateExecutable = xe.Get("GenerateExecutable").zTryParseAs(_generateExecutable);
        //    _generateInMemory = xe.Get("GenerateInMemory").zTryParseAs(_generateInMemory);
        //    _debugInformation = xe.Get("DebugInformation").zTryParseAs(_debugInformation);
        //    _warningLevel = xe.Get("WarningLevel").zTryParseAs<int>(_warningLevel);
        //    AddCompilerOptions(xe.GetValues("CompilerOptions"));
        //    string keyfile = xe.Get("KeyFile");
        //    if (keyfile != null)
        //        AddCompilerOption("/keyfile:\"" + zpath.PathMakeRooted(keyfile, _defaultDir) + "\"");
        //    string target = xe.Get("Target");
        //    if (target != null)
        //        AddCompilerOption("/target:" + target);
        //    string icon = xe.Get("Icon");
        //    if (icon != null)
        //        AddCompilerOption("/win32icon:" + zpath.PathMakeRooted(icon, _defaultDir));
        //    if (xe != null)
        //        AddSources(xe.GetElements("Source"));
        //    AddFiles(_fileList, xe.GetElements("File"), _defaultDir);
        //    AddAssemblies(xe.GetElements("Assembly"));
        //    AddLocalAssemblies(xe.GetElements("LocalAssembly"));

        //    _language = xe.Get("Language", _language);
        //    foreach (XElement xe2 in xe.GetElements("ProviderOption"))
        //        SetProviderOption(xe2.zAttribValue("name"), xe2.zAttribValue("value"));
        //    s = xe.Get("ResourceCompiler");
        //    if (s != null)
        //        _resourceCompiler = s;

        //    AddCopyOutputDirectories(xe.GetValues("CopyOutput"));
        //}

        public void SetProviderOption(string name, string value)
        {
            if (_providerOption.ContainsKey(name)) _providerOption.Remove(name);
            _providerOption.Add(name, value);
        }

        //public void AddCompilerOptions(string[] options)
        public void AddCompilerOptions(IEnumerable<string> options)
        {
            foreach (string option in options)
                AddCompilerOption(option);
        }

        public void AddCompilerOption(string option)
        {
            if (option == null || option == "") return;
            if (_compilerOptions != null)
                _compilerOptions += " " + option;
            else
                _compilerOptions = option;
        }

        //public void AddSources(string[] sources)
        public void AddSources(IEnumerable<string> sources)
        {
            AddFiles(_sourceList, sources, _defaultDir);
        }

        public void AddSources(IEnumerable<XElement> sources)
        {
            AddFiles(_sourceList, sources, _defaultDir);
        }

        public void AddSource(string source)
        {
            AddFile(_sourceList, source, _defaultDir);
        }

        public void AddFile(string file, string dir)
        {
            AddFile(_fileList, file, dir);
        }

        public void AddFiles(IEnumerable<XElement> files, string dir = null)
        {
            AddFiles(_fileList, files, dir);
        }

        //private void AddFiles(List<CompilerFile> list, string[] files, string dir)
        private void AddFiles(List<CompilerFile> list, IEnumerable<string> files, string dir)
        {
            foreach (string file in files)
                AddFile(list, file, dir);
        }

        private void AddFiles(List<CompilerFile> list, IEnumerable<XElement> files, string dir = null)
        {
            foreach (XElement file in files)
                AddFile(list, file, dir);
        }

        private void AddFile(List<CompilerFile> list, string file, string dir)
        {
            list.Add(new CompilerFile(file.zRootPath(dir)));
        }

        private void AddFile(List<CompilerFile> list, XElement xeFile, string dir = null)
        {
            string file = xeFile.Attribute("value").Value;
            if (dir == null)
                dir = _defaultDir;
            if (dir != null)
                file = file.zRootPath(dir);
            CompilerFile cf = new CompilerFile(file);
            foreach (XAttribute xa in xeFile.Attributes())
            {
                if (xa.Name != "value")
                    cf.Attributes.Add(xa.Name.LocalName, xa.Value);
            }
            list.Add(cf);
        }

        public void AddAssemblies(IEnumerable<XElement> assemblies)
        {
            foreach (XElement assembly in assemblies)
            {
                //bool resolve = assembly.zAttribValueBool("resolve", false);
                bool resolve = assembly.zAttribValue("resolve").zTryParseAs<bool>(false);
                string resolveName = assembly.zAttribValue("resolveName");
                if (resolve && resolveName == null)
                    throw new PBException("error to resolve an assembly you must specify a resolveName (\"Test_dll, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")");
                AddAssembly(assembly.zAttribValue("value"), resolve, resolveName);
            }
        }

        public void AddAssembly(string assembly, bool resolve = false, string resolveName = null)
        {
            // sert a ajouter les assembly sans path : System.dll, System.Data.dll
            string dir = zPath.GetDirectoryName(assembly);
            //if (dir != "" && !zPath.IsPathRooted(assembly))
            //    assembly = zPath.Combine(_defaultDir, assembly);
            if (dir != "")
                assembly = assembly.zRootPath(_defaultDir);
            if (!_assemblyList.ContainsKey(assembly))
                _assemblyList.Add(assembly, new CompilerAssembly(assembly, resolve, resolveName));
        }

        public void AddLocalAssemblies(IEnumerable<XElement> assemblies)
        {
            foreach (XElement assembly in assemblies)
            {
                //bool resolve = assembly.zAttribValueBool("resolve", false);
                bool resolve = assembly.zAttribValue("resolve").zTryParseAs<bool>(false);
                string resolveName = assembly.zAttribValue("resolveName");
                if (resolve && resolveName == null)
                    throw new PBException("error to resolve an assembly you must specify a resolveName (\"Test_dll, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")");
                AddLocalAssembly(assembly.zAttribValue("value"), resolve, resolveName);
            }
        }

        public void AddLocalAssembly(string assembly, bool resolve = false, string resolveName = null)
        {
            assembly = assembly.zRootPath(_defaultDir);
            if (!zFile.Exists(assembly))
                return;
            AddAssembly(assembly, resolve, resolveName);
        }

        public void AddCopyOutputDirectories(IEnumerable<string> directories)
        {
            _copyOutputDirectories.AddRange(directories);
        }

        //public void AddCopyOutputDirectories(IEnumerable<XElement> elements)
        //{
        //    foreach (XElement element in elements)
        //    {
        //        CopyOutputDirectory cod = new CopyOutputDirectory();
        //        cod.Directory = zpath.PathMakeRooted(element.zAttribValue("value"), gsDefaultDir);
        //        AddFiles(cod.Files, element.zXPathElements("File"), gsDefaultDir);
        //        _copyOutputDirectories.Add(cod);
        //    }
        //}

        //public void CloseProcess()
        //{
        //    SetFinalOutputAssembly();
        //    if (_finalOutputAssembly == null) return;
        //    string processName = zpath.PathGetFileName(_finalOutputAssembly);
        //    foreach (Process process in Process.GetProcessesByName(processName))
        //    {
        //        if (string.Compare(process.MainModule.FileName, _finalOutputAssembly, true) != 0) continue;
        //        zprocess.CloseProcess(process);
        //    }
        //}

        public void Compile()
        {
            if (!_generateInMemory && _outputAssembly == null) throw new CompilerException("error output assembly is not defined");
            SetFinalOutputAssembly();
            if (_finalOutputDir != null)
                zDirectory.CreateDirectory(_finalOutputDir);
            //WriteLine(1, "Compile \"{0}\"", gsFinalOutputAssembly);
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
            string[] sSources = GetFilesType(sourceExt);
            //string currentDirectory = zDirectory.GetCurrentDirectory();
            //Directory.SetCurrentDirectory(zapp.GetAppDirectory());
            //cTrace.Trace("Compiler.Compile() : change current directory to {0}", cu.GetAppDirectory());
            _results = provider.CompileAssemblyFromFile(options, sSources);
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
                _finalOutputDir = _outputDir.zRootPath(_defaultDir);
            else if (_finalOutputAssembly != null)
            {
                string sDir = zPath.GetDirectoryName(_finalOutputAssembly);
                _finalOutputAssembly = zPath.GetFileName(_finalOutputAssembly);
                if (sDir != null && sDir != "")
                    _finalOutputDir = sDir.zRootPath(_defaultDir);
            }
            if (_finalOutputDir == null) _finalOutputDir = "bin".zRootPath(_defaultDir);
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
            foreach (CompilerFile source in _sourceList)
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
            foreach (CompilerFile source in _sourceList)
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
            foreach (CompilerFile source in _sourceList)
            {
                string ext = zPath.GetExtension(source.File).ToLower();
                foreach (string type in types)
                {
                    if (ext == type) SourcesList.Add(source.File);
                }
            }
            return SourcesList.ToArray();
        }

        //private void CopyCompileFilesToDirectory(string directory = null)
        //{
        //    CopyAssembliesToDirectory(directory);
        //    CopyFilesToDirectory(directory);
        //}

        //private void CopyFileToOutputDir()
        // List<string>
        // copy files and config file to directory
        //private void CopyFilesToDirectory(string directory = null)
        //{
        //    //List<string> copiedFiles = new List<string>();

        //    if (gResults.PathToAssembly != null)
        //    {
        //        //string sOutputDir = zpath.PathGetDirectory(gResults.PathToAssembly);

        //        if (directory == null)
        //            directory = zpath.PathGetDirectory(gResults.PathToAssembly);

        //        foreach (CompilerFile file in gFileList)
        //        {
        //            string destinationFile = null;
        //            if (file.Attributes.ContainsKey("destinationFile"))
        //            {
        //                destinationFile = file.Attributes["destinationFile"];
        //                WriteLine(2, "  Copy file \"{0}\" to \"{1}\" as \"{2}\"", file.File, directory, destinationFile);
        //            }
        //            else
        //                WriteLine(2, "  Copy file \"{0}\" to \"{1}\"", file.File, directory);
        //            //string path = zfile.CopyFileToDirectory(file.File, directory, destinationFile, true);
        //            string path = zfile.CopyFileToDirectory(file.File, directory, destinationFile, CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
        //            //if (path != null)
        //            //    copiedFiles.Add(path);
        //        }

        //        if (_appConfig != null)
        //        {
        //            string appFile = zPath.GetFileName(gResults.PathToAssembly) + ".config";
        //            WriteLine(2, "Copy file \"{0}\" to \"{1}\" as \"{2}\"", _appConfig.File, directory, appFile);
        //            //string path = zfile.CopyFileToDirectory(_appConfig.File, directory, appFile, true);
        //            string path = zfile.CopyFileToDirectory(_appConfig.File, directory, appFile, CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
        //            //if (path != null)
        //            //    copiedFiles.Add(path);
        //        }
        //    }

        //    //return copiedFiles;
        //}

        //private void CopyOutputToDirectories()
        //{
        //    foreach (CopyOutputDirectory cod in _copyOutputDirectories)
        //    {
        //        List<string> copiedFiles = CopyOutputToDirectory(cod);
        //        _copyOutputFiles.Add(new KeyValuePair<string, List<string>>(cod.Directory, copiedFiles));
        //    }
        //}

        //private void CopyOutputToDirectory(CopyOutputDirectory cod, List<string> files)
        //private List<string> CopyOutputToDirectory(CopyOutputDirectory cod)
        //{
        //    List<string> copiedFiles = new List<string>();

        //    if (gResults.PathToAssembly != null)
        //    {
        //        string outputDir = zpath.PathGetDirectory(gResults.PathToAssembly);

        //        WriteLine(1, "Copy output to \"{0}\"", cod.Directory);
        //        if (!Directory.Exists(cod.Directory))
        //            Directory.CreateDirectory(cod.Directory);

        //        // copy assembly
        //        string path = gResults.PathToAssembly;
        //        WriteLine(2, "  Copy file \"{0}\" to \"{1}\"", path, cod.Directory);
        //        //string path3 = zfile.CopyFileToDirectory(path, cod.Directory, null, true);
        //        string path3 = zfile.CopyFileToDirectory(path, cod.Directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);

        //        /* Unmerged change from project 'runsource.v2.dll'
        //        Before:
        //                        if (path3 != null)
        //                            copiedFiles.Add(path3);

        //                        // copy assembly pdb
        //                        string path2 = zpath.PathSetExtension(path, ".pdb");
        //                        WriteLine(2, "  Copy file \"{0}\" to \"{1}\"", path2, cod.Directory);
        //                        path3 = zfile.CopyFile(path2, cod.Directory, null, true);
        //                        if (path3 != null)
        //        After:
        //                        if (path3 != null)
        //        */
        //        if (path3 != null)
        //            copiedFiles.Add(path3);

        //        // copy assembly pdb
        //        string path2 = zpath.PathSetExtension(path, ".pdb");
        //        WriteLine(2, "  Copy file \"{0}\" to \"{1}\"", path2, cod.Directory);
        //        //path3 = zfile.CopyFileToDirectory(path2, cod.Directory, null, true);
        //        path3 = zfile.CopyFileToDirectory(path2, cod.Directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
        //        if (path3 != null)
        //            copiedFiles.Add(path3);

        //        // copy assembly pdb
        //        //string path2 = zpath.PathSetExtension(path, ".pdb");
        //        //WriteLine(2, "  Copy file \"{0}\" to \"{1}\"", path2, cod.Directory);
        //        ////path3 = zfile.CopyFileToDirectory(path2, cod.Directory, null, true);
        //        //path3 = zfile.CopyFileToDirectory(path2, cod.Directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
        //        //if (path3 != null)
        //        //    copiedFiles.Add(path3);

        //        // copy assembly config
        //        path2 = path + ".config";
        //        WriteLine(2, "  Copy file \"{0}\" to \"{1}\"", path2, cod.Directory);
        //        //path3 = zfile.CopyFileToDirectory(path2, cod.Directory, null, true);
        //        path3 = zfile.CopyFileToDirectory(path2, cod.Directory, options: CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
        //        if (path3 != null)
        //            copiedFiles.Add(path3);

        //        // copy referenced assemblies
        //        //foreach (CompilerAssembly ca in gAssemblyList.Values)
        //        //{
        //        //    string assembly = ca.File;
        //        //    if (!File.Exists(assembly)) continue;
        //        //    string assembly2 = zpath.PathSetDirectory(assembly, cod.Directory);
        //        //    if (File.Exists(assembly2))
        //        //    {
        //        //        FileInfo fi1 = new FileInfo(assembly);
        //        //        FileInfo fi2 = new FileInfo(assembly2);
        //        //        if (fi1.LastWriteTime <= fi2.LastWriteTime) continue;
        //        //    }
        //        //    WriteLine(2, "  Copy assembly \"{0}\" to \"{1}\"", assembly, assembly2);
        //        //    File.Copy(assembly, assembly2, true);
        //        //    files.Add(assembly2);
        //        //    path = zpath.PathSetExtension(assembly, ".pdb");
        //        //    assembly2 = zpath.PathSetExtension(assembly2, ".pdb");
        //        //    if (File.Exists(path))
        //        //    {
        //        //        WriteLine(2, "  Copy assembly \"{0}\" to \"{1}\"", path, assembly2);
        //        //        File.Copy(path, assembly2, true);
        //        //        files.Add(assembly2);
        //        //    }
        //        //}
        //        List<string> copiedFiles2 = CopyReferencedAssembliesToDirectory(cod.Directory);
        //        copiedFiles.AddRange(copiedFiles2);

        //        // copy files and config file to directory
        //        copiedFiles2 = CopyFilesToDirectory(cod.Directory);
        //        copiedFiles.AddRange(copiedFiles2);

        //        // copy files
        //        foreach (CompilerFile file in cod.Files)
        //        {
        //            path = file.File;
        //            if (!zPath.IsPathRooted(path))
        //                path = zPath.Combine(outputDir, path);
        //            string destinationFile = null;
        //            if (file.Attributes.ContainsKey("destinationFile"))
        //            {
        //                destinationFile = file.Attributes["destinationFile"];
        //                WriteLine(2, "  Copy file \"{0}\" to \"{1}\" as \"{2}\"", path, cod.Directory, destinationFile);
        //            }
        //            else
        //                WriteLine(2, "  Copy file \"{0}\" to \"{1}\"", path, cod.Directory);
        //            //path3 = zfile.CopyFileToDirectory(path, cod.Directory, destinationFile, true);
        //            path3 = zfile.CopyFileToDirectory(path, cod.Directory, destinationFile, CopyFileOptions.OverwriteReadOnly | CopyFileOptions.CopyOnlyIfNewer);
        //            if (path3 != null)
        //                copiedFiles.Add(path3);
        //        }
        //    }
        //    return copiedFiles;
        //}

        //public string[] CompileResources(CompilerFile[] resources, string outputDir)
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
            if (resource.Attributes.ContainsKey("namespace")) nameSpace = resource.Attributes["namespace"];
            string sPathCompiledResource = resourceFilename + ".resources";
            if (nameSpace != null) sPathCompiledResource = nameSpace + "." + sPathCompiledResource;
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

        //public string GetCompilerMessages()
        //{
        //    if ((gResults == null || gResults.Errors.Count == 0) && !gResourceResults.HasError) return null;
        //    string sError = "";
        //    if (gResourceResults.HasError)
        //    {
        //        sError += "Resource compiler error :\r\n";
        //        foreach (ResourceCompilerError error in gResourceResults.Errors)
        //            sError += string.Format("error in {0} : {1}", error.FileName, error.ErrorText) + "\r\n";
        //    }
        //    if (gResults != null && gResults.Errors.Count != 0)
        //    {
        //        sError += "Compiler error :\r\n";
        //        foreach (CompilerError error in gResults.Errors)
        //        {
        //            string s = "error"; if (error.IsWarning) s = "warning";
        //            sError += string.Format("{0} {1} in {2} line {3} column {4} : {5}\r\n", s, error.ErrorNumber, error.FileName, error.Line, error.Column, error.ErrorText);
        //        }
        //    }
        //    return sError;
        //}

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

        public void CopyResultFilesToDirectory(string directory)
        {
            //List<string> copiedFiles = new List<string>();

            //if (gResults.PathToAssembly == null || gbGenerateInMemory)
            //    return;

            if (_results.PathToAssembly == null)
                return;

            if (directory != null)
                WriteLine(2, "  copy result files to directory \"{0}\"", directory);

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
                        WriteLine(2, "  copy assembly \"{0}\" to \"{1}\"", file, directory);

                }
            }

            foreach (CompilerFile compilerFile in _fileList)
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
            //}
            //return copiedFiles;

            //if (directory != null)
            //    TraceLevel = 1;
        }

        //public void TraceMessages()
        //{
        //    if (Trace == null)
        //        return;
        //    if (_results != null)
        //    {
        //        foreach (CompilerError err in _results.Errors)
        //            Trace.WriteLine("{0} no {1,-6} source \"{2}\" line {3} col {4} \"{5}\"", err.IsWarning ? "warning" : "error", err.ErrorNumber, zPath.GetFileName(err.FileName), err.Line, err.Column, err.ErrorText);
        //    }
        //    if (_resourceResults != null)
        //    {
        //        foreach (ResourceCompilerError err in _resourceResults.Errors)
        //            Trace.WriteLine("source \"{0}\" \"{1}\"", zPath.GetFileName(err.FileName), err.ErrorText);
        //    }
        //}

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
