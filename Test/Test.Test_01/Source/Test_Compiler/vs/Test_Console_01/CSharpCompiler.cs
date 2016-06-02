using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;

// parse source 2 methods :
//   1) use Microsoft.CodeAnalysis.CSharp.SyntaxFactory (assembly Microsoft.CodeAnalysis.CSharp)
//      CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp5);
//      SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
//   2) use Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree (assembly Microsoft.CodeAnalysis.CSharp)

// doc
//   Microsoft.CodeAnalysis.CSharp.Syntax Namespace http://www.coderesx.com/roslyn/html/6C1EC978.htm
// source
//   Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree http://source.roslyn.io/#q=Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree

// to see :
//   Compilation.MetadataReference GetMetadataReference()
//   SemanticModel GetSemanticModel()

// VisualStudio C# REPL https://visualstudiogallery.msdn.microsoft.com/295fa0f6-37d1-49a3-b51d-ea4741905dc2

namespace Test_Roslyn_01
{
    public class CSharpCompiler
    {
        private string _assemblyName = null;
        private string _outputPath = null;

        private LanguageVersion _languageVersion = LanguageVersion.CSharp6;                           // CSharp1 ... CSharp6

        // CSharpCompilationOptions
        // OutputKind outputKind, bool reportSuppressedDiagnostics = false, string moduleName = null, string mainTypeName = null, string scriptClassName = null, IEnumerable<string> usings = null,
        // OptimizationLevel optimizationLevel = OptimizationLevel.Debug, bool checkOverflow = false, bool allowUnsafe = false, string cryptoKeyContainer = null, string cryptoKeyFile = null,
        // ImmutableArray<byte> cryptoPublicKey = default(ImmutableArray<byte>), bool? delaySign = default(bool?), Platform platform = Platform.AnyCpu, ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default,
        // int warningLevel = 4, IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions = null, bool concurrentBuild = true, bool deterministic = false, XmlReferenceResolver xmlReferenceResolver = null,
        // SourceReferenceResolver sourceReferenceResolver = null, MetadataReferenceResolver metadataReferenceResolver = null, AssemblyIdentityComparer assemblyIdentityComparer = null, StrongNameProvider strongNameProvider = null,
        // bool publicSign = false
        private OutputKind _outputKind = OutputKind.DynamicallyLinkedLibrary;                         // ConsoleApplication, WindowsApplication, DynamicallyLinkedLibrary, NetModule, WindowsRuntimeMetadata (.winmdobj file), WindowsRuntimeApplication (.exe that can run in an app container)
        private OptimizationLevel _optimizationLevel = OptimizationLevel.Debug;                       // Debug or Release
        private Platform _platform = Platform.AnyCpu;                                                 // AnyCpu, X86, X64, Itanium, AnyCpu32BitPreferred, Arm
        private ReportDiagnostic _generalDiagnosticOption = ReportDiagnostic.Default;                 // Report a diagnostic as : Default, Error, Warn, Info, Hidden, Suppress
        private int _warningLevel = 4;

        private IEnumerable<string> _sourceFiles = null;
        private IEnumerable<string> _assembliesFiles = null;

        public string AssemblyName { get { return _assemblyName; } set { _assemblyName = value; } }
        public string OutputPath { get { return _outputPath; } set { _outputPath = value; } }
        public LanguageVersion LanguageVersion { get { return _languageVersion; } set { _languageVersion = value; } }
        public OutputKind OutputKind { get { return _outputKind; } set { _outputKind = value; } }
        public OptimizationLevel OptimizationLevel { get { return _optimizationLevel; } set { _optimizationLevel = value; } }
        public Platform Platform { get { return _platform; } set { _platform = value; } }
        public ReportDiagnostic GeneralDiagnosticOption { get { return _generalDiagnosticOption; } set { _generalDiagnosticOption = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public IEnumerable<string> SourceFiles { get { return _sourceFiles; } set { _sourceFiles = value; } }
        public IEnumerable<string> AssembliesFiles { get { return _assembliesFiles; } set { _assembliesFiles = value; } }

        public void Compile()
        {
            //SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            //MetadataReference[] references = new MetadataReference[]
            //{
            //    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            //};

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
            CSharpParseOptions parseOptions = CSharpParseOptions.Default.WithLanguageVersion(_languageVersion);
            CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(_outputKind, optimizationLevel: _optimizationLevel, platform: _platform, generalDiagnosticOption: _generalDiagnosticOption, warningLevel: _warningLevel);

            CSharpCompilation compilation = CSharpCompilation.Create(
                _assemblyName,
                syntaxTrees: ParseSources2(parseOptions),
                references: GetAssemblyReferences(),
                options: compilationOptions);

            // EmitResult Emit(this CSharpCompilation compilation, string outputPath, string pdbPath = null, string xmlDocumentationPath = null, string win32ResourcesPath = null, IEnumerable<ResourceDescription> manifestResources = null,
            //   CancellationToken cancellationToken = default(CancellationToken));

            EmitResult result = compilation.Emit(_outputPath);

            WriteMessages(result.Diagnostics);

            if (!result.Success)
            {
                Console.WriteLine("error compiling");
            }
        }

        private IEnumerable<SyntaxTree> ParseSources1()
        {
            foreach (string sourceFile in _sourceFiles)
            {
                yield return CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFile));
            }
        }

        private IEnumerable<SyntaxTree> ParseSources2(CSharpParseOptions parseOptions)
        {
            string filename = "";
            foreach (string sourceFile in _sourceFiles)
            {
                //yield return CSharpSyntaxTree.ParseText(File.ReadAllText(source));
                string source = File.ReadAllText(sourceFile);
                SourceText stringText = SourceText.From(source, Encoding.UTF8);
                yield return SyntaxFactory.ParseSyntaxTree(stringText, parseOptions, filename);
            }
        }

        private IEnumerable<MetadataReference> GetAssemblyReferences()
        {
            foreach (string assemblyFile in _assembliesFiles)
            {
                // MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                yield return MetadataReference.CreateFromFile(assemblyFile);
            }
        }

        private static void WriteMessages(ImmutableArray<Diagnostic> diagnostics)
        {
            //IEnumerable<Diagnostic> failures = diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            foreach (Diagnostic diagnostic in diagnostics)
            {
                Console.Error.WriteLine("{0} : {1}", diagnostic.Id, diagnostic.GetMessage());
            }
        }
    }
}

//C:\pib\_dl\dev\vs\test\Test_csharp6_01\packages\Microsoft.CodeAnalysis.Common.1.2.2\lib\net45\Microsoft.CodeAnalysis.dll
//C:\pib\_dl\dev\vs\test\Test_csharp6_01\packages\Microsoft.CodeAnalysis.CSharp.1.2.2\lib\net45\Microsoft.CodeAnalysis.CSharp.dll
//C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\Microsoft.CSharp.dll

// http://stackoverflow.com/questions/18376313/setting-up-a-common-nuget-packages-folder-for-all-solutions-when-some-projects-a
// http://docs.nuget.org/release-notes/nuget-2.1
// Global nuget.config (%appdata%\NuGet\nuget.config)
// c:\Users\Pierre\AppData\Roaming\NuGet\