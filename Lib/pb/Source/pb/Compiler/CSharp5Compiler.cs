using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using pb.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// todo :
//   _generateInMemory
//   _compilerOptions to set define ("/define:DEBUG;TRACE")

namespace pb.Compiler
{
    public class EmitCompilerResults : ICompilerResult
    {
        private EmitResult _result = null;
        private string _assemblyFile = null;
        private Predicate<CompilerMessage> _messageFilter = null;

        public EmitCompilerResults(EmitResult result, string assemblyFile, Predicate<CompilerMessage> messageFilter)
        {
            _result = result;
            _assemblyFile = assemblyFile;
            _messageFilter = messageFilter;
        }

        public bool Success { get { return _result.Success; } }
        public int MessagesCount { get { return _result.Diagnostics.Length; } }
        //public Predicate<CompilerMessage> MessageFilter { get { return _messageFilter; } set { _messageFilter = value; } }

        public Assembly LoadAssembly()
        {
            return Assembly.LoadFile(_assemblyFile);
        }

        public string GetAssemblyFile()
        {
            return _assemblyFile;
        }

        public IEnumerable<CompilerMessage> GetMessages(Predicate<CompilerMessage> messageFilter = null)
        {
            foreach (Diagnostic diagnostic in _result.Diagnostics)
            {
                // Translating error diagnostic SourceSpan property to source Line and Column https://social.msdn.microsoft.com/Forums/vstudio/en-US/cebf07d6-f817-4c24-93f2-c66f05a23e20/translating-error-diagnostic-sourcespan-property-to-source-line-and-column?forum=roslyn
                //var lineSpan = Location.SourceTree.GetLineSpan(Location.SourceSpan, usePreprocessorDirectives: false);
                string fileName = null;
                int? column = null;
                int? line = null;
                Location location = diagnostic.Location;
                if (location != null && location.SourceTree != null)
                {
                    fileName = location.SourceTree.FilePath;
                    var lineSpan = location.SourceTree.GetLineSpan(location.SourceSpan);
                    column = lineSpan.StartLinePosition.Character + 1;
                    line = lineSpan.StartLinePosition.Line + 1;
                }
                //
                CompilerMessage message = new CompilerMessage { Id = diagnostic.Id, Message = diagnostic.GetMessage(), Severity = GetSeverity(diagnostic.Severity), FileName = fileName, Column = column, Line = line };
                if (messageFilter != null && !messageFilter(message))
                    continue;
                if (_messageFilter != null && !_messageFilter(message))
                    continue;
                yield return message;
            }
        }

        private CompilerMessageSeverity GetSeverity(DiagnosticSeverity severity)
        {
            switch (severity)
            {
                case DiagnosticSeverity.Hidden:
                    return CompilerMessageSeverity.Hidden;
                case DiagnosticSeverity.Info:
                    return CompilerMessageSeverity.Info;
                case DiagnosticSeverity.Warning:
                    return CompilerMessageSeverity.Warning;
                case DiagnosticSeverity.Error:
                    return CompilerMessageSeverity.Error;
                default:
                    throw new PBException("unknow DiagnosticSeverity {0}", severity);
            }
        }
    }

    public class CSharp5Compiler : ICompiler
    {
        private Dictionary<string, string> _frameworkDirectories = null;
        private Predicate<CompilerMessage> _messageFilter = null;
        private string _languageVersion = null;
        private string _frameworkVersion = null;
        private string _target = null;
        private string _platform = null;
        private bool _generateInMemory = false;
        private bool _debugInformation = false;
        private int _warningLevel = -1;
        private string _compilerOptions = null;
        private ReportDiagnostic _generalDiagnosticOption = ReportDiagnostic.Default;                 // Report a diagnostic as : Default, Error, Warn, Info, Hidden, Suppress
        private string _outputAssembly = null;
        private string _win32ResourceFile = null;
        private IEnumerable<string> _preprocessorSymbols = null;
        private IEnumerable<string> _sources = null;
        private IEnumerable<ReferencedAssembly> _referencedAssemblies = null;
        private IEnumerable<ResourceFile> _embeddedResources = null;

        public string LanguageVersion { get { return _languageVersion; } set { _languageVersion = value; } }
        public string FrameworkVersion { get { return _frameworkVersion; } set { _frameworkVersion = value; } }
        public string Target { get { return _target; } set { _target = value; } }
        public string Platform { get { return _platform; } set { _platform = value; } }
        public bool GenerateInMemory { get { return _generateInMemory; } set { _generateInMemory = value; } }
        public bool DebugInformation { get { return _debugInformation; } set { _debugInformation = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public string CompilerOptions { get { return _compilerOptions; } set { _compilerOptions = value; } }
        public string OutputAssembly { get { return _outputAssembly; } set { _outputAssembly = value; } }
        public string Win32ResourceFile { get { return _win32ResourceFile; } set { _win32ResourceFile = value; } }

        public CSharp5Compiler(Dictionary<string, string> frameworkDirectories, Predicate<CompilerMessage> messageFilter)
        {
            _frameworkDirectories = frameworkDirectories;
            _messageFilter = messageFilter;
        }

        public void SetPreprocessorSymbols(IEnumerable<string> preprocessorSymbols)
        {
            _preprocessorSymbols = preprocessorSymbols;
        }

        public void SetSources(IEnumerable<string> sources)
        {
            _sources = sources;
        }

        //public void SetReferencedAssemblies(IEnumerable<string> referencedAssemblies)
        public void SetReferencedAssemblies(IEnumerable<ReferencedAssembly> referencedAssemblies)
        {
            _referencedAssemblies = referencedAssemblies;
        }

        public void SetEmbeddedResources(IEnumerable<ResourceFile> embeddedResources)
        {
            _embeddedResources = embeddedResources;
        }

        public ICompilerResult Compile()
        {
            // CSharpParseOptions :
            //   LanguageVersion languageVersion = LanguageVersion.CSharp6
            //   DocumentationMode documentationMode = DocumentationMode.Parse
            //   SourceCodeKind kind = SourceCodeKind.Regular
            //     Regular        : No scripting. Used for .cs/.vb file parsing.
            //     Script         : Allows top-level statements, declarations, and optional trailing expression. Used for parsing .csx/.vbx and interactive submissions.
            //     Interactive    : The same as Microsoft.CodeAnalysis.SourceCodeKind.Script.
            //   IEnumerable<string> preprocessorSymbols = null
            //   .WithFeatures(IEnumerable<KeyValuePair<string, string>> features);
            //     Enable some experimental language features for testing.
            CSharpParseOptions parseOptions = CSharpParseOptions.Default.WithLanguageVersion(GetLanguageVersion(_languageVersion));
            if (_preprocessorSymbols != null)
                parseOptions = parseOptions.WithPreprocessorSymbols(_preprocessorSymbols);

            // CSharpCompilationOptions :
            //   OutputKind outputKind
            //   bool reportSuppressedDiagnostics = false
            //   string moduleName = null
            //   string mainTypeName = null
            //   string scriptClassName = null
            //   IEnumerable<string> usings = null
            //   OptimizationLevel optimizationLevel = OptimizationLevel.Debug
            //   bool checkOverflow = false
            //   bool allowUnsafe = false
            //   string cryptoKeyContainer = null
            //   string cryptoKeyFile = null
            //   ImmutableArray<byte> cryptoPublicKey = default(ImmutableArray<byte>)
            //   bool? delaySign = default(bool?)
            //   Platform platform = Platform.AnyCpu
            //   ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default
            //   int warningLevel = 4
            //   IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions = null
            //   bool concurrentBuild = true
            //   bool deterministic = false
            //   XmlReferenceResolver xmlReferenceResolver = null
            //   SourceReferenceResolver sourceReferenceResolver = null
            //   MetadataReferenceResolver metadataReferenceResolver = null
            //   AssemblyIdentityComparer assemblyIdentityComparer = null
            //   StrongNameProvider strongNameProvider = null
            //   bool publicSign = false
            //   [Obsolete] CommonWithFeatures(ImmutableArray<string> features)

            OutputKind outputKind = GetOutputKind(_target);
            Platform platform = GetPlatform(_platform);
            OptimizationLevel optimizationLevel = OptimizationLevel.Release;
            if (_debugInformation)
                optimizationLevel = OptimizationLevel.Debug;

            CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(outputKind, optimizationLevel: optimizationLevel, platform: platform, generalDiagnosticOption: _generalDiagnosticOption, warningLevel: _warningLevel);

            CSharpCompilation compilation = CSharpCompilation.Create(zPath.GetFileNameWithoutExtension(_outputAssembly),
                syntaxTrees: ParseSources2(parseOptions),
                references: GetAssemblyReferences(),
                options: compilationOptions);

            // EmitResult Emit(this CSharpCompilation compilation, string outputPath, string pdbPath = null, string xmlDocumentationPath = null, string win32ResourcesPath = null, IEnumerable<ResourceDescription> manifestResources = null,
            //   CancellationToken cancellationToken = default(CancellationToken));


            //_generateInMemory, _compilerOptions

            string pdbPath = zpath.PathSetExtension(_outputAssembly, ".pdb");
            EmitResult result = compilation.Emit(_outputAssembly, pdbPath: pdbPath, win32ResourcesPath: _win32ResourceFile, manifestResources: GetResourceDescriptions());

            return new EmitCompilerResults(result, _outputAssembly, _messageFilter);
        }

        private static LanguageVersion GetLanguageVersion(string languageVersion)
        {
            int version = languageVersion.zTryParseAs(0);
            if (version < 1 || version > 6)
                throw new PBException("unknow language version \"{0}\"", languageVersion);
            return (LanguageVersion)version;
        }

        private static OutputKind GetOutputKind(string target)
        {
            if (target == null)
                return OutputKind.ConsoleApplication;
            switch (target.ToLowerInvariant())
            {
                case "exe":
                    return OutputKind.ConsoleApplication;
                case "library":
                    return OutputKind.DynamicallyLinkedLibrary;
                case "module":
                    return OutputKind.NetModule;
                case "winexe":
                    return OutputKind.WindowsApplication;
                case "winruntimeexe":
                    return OutputKind.WindowsRuntimeApplication;
                case "winruntimemetadata":
                    return OutputKind.WindowsRuntimeMetadata;
                default:
                    throw new PBException("unknow target \"{0}\"", target);
            }
        }

        private static Platform GetPlatform(string platform)
        {
            if (platform == null)
                return Microsoft.CodeAnalysis.Platform.AnyCpu;
            switch (platform.ToLowerInvariant())
            {
                case "anycpu":
                    return Microsoft.CodeAnalysis.Platform.AnyCpu;
                case "x86":
                    return Microsoft.CodeAnalysis.Platform.X86;
                case "x64":
                    return Microsoft.CodeAnalysis.Platform.X64;
                case "itanium":
                    return Microsoft.CodeAnalysis.Platform.Itanium;
                case "anycpu32bitpreferred":
                    return Microsoft.CodeAnalysis.Platform.AnyCpu32BitPreferred;
                case "arm":
                    return Microsoft.CodeAnalysis.Platform.Arm;
                default:
                    throw new PBException("unknow platform \"{0}\"", platform);
            }
        }

        private IEnumerable<SyntaxTree> ParseSources1()
        {
            foreach (string source in _sources)
            {
                if (!zFile.Exists(source))
                    throw new PBException($"file not found \"{source}\"");
                yield return CSharpSyntaxTree.ParseText(zFile.ReadAllText(source));
            }
        }

        private IEnumerable<SyntaxTree> ParseSources2(CSharpParseOptions parseOptions)
        {
            if (_sources != null)
            {
                foreach (string sourceFile in _sources)
                {
                    if (!zFile.Exists(sourceFile))
                        throw new PBException($"file not found \"{sourceFile}\"");
                    string source = zFile.ReadAllText(sourceFile);
                    SourceText stringText = SourceText.From(source, Encoding.UTF8);
                    yield return SyntaxFactory.ParseSyntaxTree(stringText, parseOptions, sourceFile);
                }
            }
        }

        private IEnumerable<MetadataReference> GetAssemblyReferences()
        {
            //string file = typeof(object).Assembly.Location;
            //Trace.WriteLine("  assembly mscorlib.dll : \"{0}\"", file);
            //yield return MetadataReference.CreateFromFile(file);

            if (_referencedAssemblies != null)
            {
                // _frameworkVersion _frameworkDirectories
                string frameworkDirectory = null;
                if (_frameworkVersion != null)
                {
                    if (!_frameworkDirectories.ContainsKey(_frameworkVersion))
                        throw new PBException("unknow framework version \"{0}\"", _frameworkVersion);
                    frameworkDirectory = _frameworkDirectories[_frameworkVersion];
                    //Trace.WriteLine("  compiler info : framework \"{0}\" - \"{1}\"", _frameworkVersion, frameworkDirectory);
                }
                else
                    Trace.WriteLine("  compiler warning : framework undefined");
                //foreach (string assembly in _referencedAssemblies)
                foreach (ReferencedAssembly assembly in _referencedAssemblies)
                {
                    string file = assembly.File;
                    //if (frameworkDirectory != null && zPath.GetDirectoryName(assembly2) == "")
                    //if (frameworkDirectory != null && !zPath.IsPathRooted(assembly2))
                    //    assembly2 = zPath.Combine(frameworkDirectory, assembly2);
                    if (assembly.FrameworkAssembly)
                        file = zPath.Combine(frameworkDirectory, file);
                    //Trace.WriteLine("  assembly \"{0}\" - \"{1}\"", assembly.File, file);
                    yield return MetadataReference.CreateFromFile(file);
                }
            }
        }

        private IEnumerable<ResourceDescription> GetResourceDescriptions()
        {
            // Including an embedded resource in a compilation made by Roslyn http://stackoverflow.com/questions/26851214/including-an-embedded-resource-in-a-compilation-made-by-roslyn
            //const string resourcePath = @"C:\Projects\...\Properties\Resources.resources";
            //var resourceDescription = new ResourceDescription(
            //                "[namespace].Resources.resources",
            //                () => File.OpenRead(resourcePath),
            //                true);

            if (_embeddedResources != null)
            {
                foreach (ResourceFile resourceFile in _embeddedResources)
                {
                    //Trace.WriteLine("  resource : namespace \"{0}\" file \"{1}\"", resourceFile.Namespace, resourceFile.File);
                    //yield return new ResourceDescription(resourceFile.Namespace + ".Resources.resources", () => zFile.OpenRead(resourceFile.File), true);
                    yield return new ResourceDescription(resourceFile.Namespace, () => zFile.OpenRead(resourceFile.File), true);
                }
            }
        }
    }
}
