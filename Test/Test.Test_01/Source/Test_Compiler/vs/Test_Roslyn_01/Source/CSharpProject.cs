using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Test_Roslyn_01
{
    public class CSharpProject
    {
        private static int __traceLevel = 1;

        private string _projectFile = null;
        private string _projectDirectory = null;

        private XDocument _projectDocument = null;
        private LanguageVersion _languageVersion;
        private string _frameworkDirectory = null;
        private OutputKind _outputKind;
        private OptimizationLevel _optimizationLevel;
        private Platform _platform;
        private ReportDiagnostic _generalDiagnosticOption;
        private int _warningLevel;
        private string _assemblyName = null;
        private string _outputPath = null;
        private string _pdbPath = null;
        private string _win32ResourceFile = null;
        private IEnumerable<string> _preprocessorSymbols = null;
        private IEnumerable<string> _sourceFiles = null;
        private IEnumerable<ResourceFile> _resourceFiles = null;
        private IEnumerable<string> _assembliesFiles = null;

        public static int TraceLevel { get { return __traceLevel; } set { __traceLevel = value; } }
        public LanguageVersion LanguageVersion { get { return _languageVersion; } }
        public OutputKind OutputKind { get { return _outputKind; } }
        public OptimizationLevel OptimizationLevel { get { return _optimizationLevel; } }
        public Platform Platform { get { return _platform; } }
        public ReportDiagnostic GeneralDiagnosticOption { get { return _generalDiagnosticOption; } }
        public int WarningLevel { get { return _warningLevel; } }
        public string AssemblyName { get { return _assemblyName; } }
        public string OutputPath { get { return _outputPath; } }
        public string PdbPath { get { return _pdbPath; } }
        public string Win32ResourceFile { get { return _win32ResourceFile; } }
        public IEnumerable<string> PreprocessorSymbols { get { return _preprocessorSymbols; } }
        public IEnumerable<string> SourceFiles { get { return _sourceFiles; } }
        public IEnumerable<ResourceFile> ResourceFiles { get { return _resourceFiles; } }
        public IEnumerable<string> AssembliesFiles { get { return _assembliesFiles; } }

        public CSharpProject(string projectFile)
        {
            if (!File.Exists(projectFile))
                throw new Exception(string.Format("project file not found \"{0}\"", projectFile));
            WriteLine(1, "compile project \"{0}\"", projectFile);
            _projectFile = projectFile;
            _projectDirectory = Path.GetDirectoryName(projectFile);
            _projectDocument = XDocument.Load(projectFile);
            _frameworkDirectory = GetValue("FrameworkDirectory");
            WriteLine(2, "  framework directory               : \"{0}\"", _frameworkDirectory);
            _assemblyName = GetValue("AssemblyName");
            WriteLine(2, "  assembly name                     : \"{0}\"", _assemblyName);
            string outputDirectory = PathCombine(_projectDirectory, GetValue("OutputDirectory"));
            WriteLine(2, "  output directory                  : \"{0}\"", outputDirectory);
            _languageVersion = GetLanguageVersion(GetValue("LanguageVersion"));
            WriteLine(2, "  language version                  : \"{0}\"", _languageVersion);
            _outputKind = GetOutputKind(GetValue("OutputKind"));
            WriteLine(2, "  output kind                       : \"{0}\"", _outputKind);
            _optimizationLevel = GetOptimizationLevel(GetValue("OptimizationLevel"));
            WriteLine(2, "  optimization level                : \"{0}\"", _optimizationLevel);
            _platform = GetPlatform(GetValue("Platform"));
            WriteLine(2, "  platform                          : \"{0}\"", _platform);
            _generalDiagnosticOption = ReportDiagnostic.Default;
            WriteLine(2, "  general diagnostic option         : \"{0}\"", _generalDiagnosticOption);
            _warningLevel = 4;
            WriteLine(2, "  warning level                     : \"{0}\"", _warningLevel);
            _outputPath = PathCombine(outputDirectory, GetValue("OutputPath"));
            WriteLine(2, "  output path                       : \"{0}\"", _outputPath);
            _pdbPath = PathCombine(outputDirectory, GetValue("PdbPath"));
            WriteLine(2, "  pdb path                          : \"{0}\"", _pdbPath);
            _win32ResourceFile = PathCombine(_projectDirectory, GetValue("Win32ResourceFile"));
            WriteLine(2, "  win32 resource file               : \"{0}\"", _win32ResourceFile);
            _preprocessorSymbols = GetPreprocessorSymbols();
            _sourceFiles = GetSources();
            _resourceFiles = GetResourceFiles();
            _assembliesFiles = GetAssembliesFiles();
        }

        //private IEnumerable<string> GetPreprocessorSymbols()
        //{
        //    foreach (XElement source in _projectDocument.Root.XPathSelectElements("PreprocessorSymbol"))
        //    {
        //        string symbol = source.Attribute("value").Value;
        //        WriteLine(2, "  preprocessor symbol               : \"{0}\"", symbol);
        //        yield return symbol;
        //    }
        //}

        private IEnumerable<string> GetPreprocessorSymbols()
        {
            Write(2, "  preprocessor symbol               :");
            string symbols = GetValue("PreprocessorSymbol");
            if (symbols != null)
            {
                foreach (string symbol in symbols.Split(';'))
                {
                    string symbol2 = symbol.Trim();
                    if (symbol2 != "")
                    {
                        Write(2, " \"{0}\"", symbol2);
                        yield return symbol2;
                    }
                }
            }
            WriteLine(2);
        }

        private IEnumerable<string> GetSources()
        {
            foreach (XElement source in _projectDocument.Root.XPathSelectElements("Source"))
            {
                string file = source.Attribute("value").Value;
                WriteLine(2, "  source                            : \"{0}\"", file);
                yield return PathCombine(_projectDirectory, file);
            }
        }

        private IEnumerable<ResourceFile> GetResourceFiles()
        {
            foreach (XElement resource in _projectDocument.Root.XPathSelectElements("Resource"))
            {
                string file = resource.Attribute("file").Value;
                string @namespace = resource.Attribute("namespace").Value;
                WriteLine(2, "  resource                          : file \"{0}\" namespace \"{1}\"", file, @namespace);
                yield return new ResourceFile { File = PathCombine(_projectDirectory, file), Namespace = @namespace };
            }
        }

        private IEnumerable<string> GetAssembliesFiles()
        {
            foreach (XElement assembly in _projectDocument.Root.XPathSelectElements("FrameworkAssembly"))
            {
                string file = assembly.Attribute("value").Value;
                WriteLine(2, "  assembly                          : file \"{0}\"", file);
                yield return PathCombine(_frameworkDirectory, file);
            }
        }

        public void Trace()
        {
            Console.WriteLine("project                                     : \"{0}\"", _projectFile);
            Console.WriteLine("  project directory                         : \"{0}\"", _projectDirectory);
            Console.WriteLine("  language version                          : {0}", _languageVersion);
            Console.WriteLine("  framework directory                       : \"{0}\"", _frameworkDirectory);
            Console.WriteLine("  output kind                               : {0}", _outputKind);
            Console.WriteLine("  optimization level                        : {0}", _optimizationLevel);
            Console.WriteLine("  platform                                  : {0}", _platform);
            Console.WriteLine("  general diagnostic option                 : {0}", _generalDiagnosticOption);
            Console.WriteLine("  warning level                             : {0}", _warningLevel);
            Console.WriteLine("  assembly name                             : \"{0}\"", _assemblyName);
            Console.WriteLine("  output path                               : \"{0}\"", _outputPath);
            Console.WriteLine("  pdb path                                  : \"{0}\"", _pdbPath);
            Console.WriteLine("  win32 resource file                       : \"{0}\"", _win32ResourceFile);
            foreach (string source in _sourceFiles)
                Console.WriteLine("  source                                    : \"{0}\"", source);
            foreach (ResourceFile resource in _resourceFiles)
                Console.WriteLine("  resource                                  : file \"{0}\" namespace \"{1}\"", resource.File, resource.Namespace);
            foreach (string assembly in _assembliesFiles)
                Console.WriteLine("  assembly                                  : \"{0}\"", assembly);
        }

        private static LanguageVersion GetLanguageVersion(string languageVersion)
        {
            switch (languageVersion.ToLowerInvariant())
            {
                case "csharp1":
                    return LanguageVersion.CSharp1;
                case "csharp2":
                    return LanguageVersion.CSharp2;
                case "csharp3":
                    return LanguageVersion.CSharp3;
                case "csharp4":
                    return LanguageVersion.CSharp4;
                case "csharp5":
                    return LanguageVersion.CSharp5;
                case "csharp6":
                    return LanguageVersion.CSharp6;
                default:
                    throw new Exception(string.Format("unknow language version \"{0}\"", languageVersion));
            }
        }

        private static OutputKind GetOutputKind(string outputKind)
        {
            switch (outputKind.ToLowerInvariant())
            {
                case "consoleapplication":
                    return OutputKind.ConsoleApplication;
                case "windowsapplication":
                    return OutputKind.WindowsApplication;
                case "dynamicallylinkedlibrary":
                    return OutputKind.DynamicallyLinkedLibrary;
                case "netmodule":
                    return OutputKind.NetModule;
                case "windowsruntimemetadata":
                    return OutputKind.WindowsRuntimeMetadata;
                case "windowsruntimeapplication":
                    return OutputKind.WindowsRuntimeApplication;
                default:
                    throw new Exception(string.Format("unknow output kind \"{0}\"", outputKind));
            }
        }

        private static OptimizationLevel GetOptimizationLevel(string optimizationLevel)
        {
            switch (optimizationLevel.ToLowerInvariant())
            {
                case "debug":
                    return OptimizationLevel.Debug;
                case "release":
                    return OptimizationLevel.Release;
                default:
                    throw new Exception(string.Format("unknow optimization level \"{0}\"", optimizationLevel));
            }
        }

        private static Platform GetPlatform(string platform)
        {
            switch (platform.ToLowerInvariant())
            {
                case "anycpu":
                    return Platform.AnyCpu;
                case "x86":
                    return Platform.X86;
                case "x64":
                    return Platform.X64;
                case "itanium":
                    return Platform.Itanium;
                case "anycpu32bitpreferred":
                    return Platform.AnyCpu32BitPreferred;
                case "arm":
                    return Platform.Arm;
                default:
                    throw new Exception(string.Format("unknow platform \"{0}\"", platform));
            }
        }

        private string GetValue(string xpath)
        {
            return _projectDocument.Root.XPathSelectElement(xpath)?.Attribute("value")?.Value;
        }

        private static string PathCombine(string path1, string path2)
        {
            if (path1 == null)
                return path2;
            else if (path2 == null)
                return null;
            else
                return Path.Combine(path1, path2);
        }

        private static void Write(int level, string message, params object[] prm)
        {
            if (level > __traceLevel)
                return;
            if (prm.Length > 0)
                message = string.Format(message, prm);
            Console.Write(message);
        }

        private static void WriteLine(int level, string message, params object[] prm)
        {
            if (level > __traceLevel)
                return;
            if (prm.Length > 0)
                message = string.Format(message, prm);
            Console.WriteLine(message);
        }

        private static void WriteLine(int level)
        {
            if (level > __traceLevel)
                return;
            Console.WriteLine();
        }

        public static CSharpProject Create(string projectFile)
        {
            return new CSharpProject(projectFile);
        }
    }
}
