using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
// Roslyn permet d'écrire un analyseur de Code direct pour votre API https://msdn.microsoft.com/fr-fr/magazine/dn879356.aspx
// Microsoft.Build.Tasks.CodeAnalysis/Csc.cs http://source.roslyn.io/#Microsoft.Build.Tasks.CodeAnalysis/Csc.cs
//   Microsoft.CodeAnalysis.BuildTasks.Csc
//   line 564 :  CheckHostObjectSupport(param = nameof(Win32Icon), cscHostObject.SetWin32Icon(Win32Icon));
// c# roslyn win32icon
// C# Compiler Options Listed by Category https://msdn.microsoft.com/fr-fr/library/6s2x2bzy.aspx

//var result = mutantCompilation.Emit(file, manifestResources: new[] { resourceDescription });


// roslyn source :

// c:\pib\_dl\dev\roslyn\roslyn-master\src\Compilers\Shared\Csc.cs
//   Microsoft.CodeAnalysis.CSharp.CommandLine.Csc
// c:\pib\_dl\dev\roslyn\roslyn-master\src\Compilers\Core\MSBuildTask\Shared\Csc.cs
//   Microsoft.CodeAnalysis.BuildTasks.Csc

// c:\pib\_dl\dev\roslyn\roslyn-master\src\Compilers\CSharp\csc\csc.csproj
//   ..\..\Shared\Csc.cs
// c:\pib\_dl\dev\roslyn\roslyn-master\src\Compilers\CSharp\CscCore\CscCore.csproj
//   ..\..\Shared\Csc.cs
// c:\pib\_dl\dev\roslyn\roslyn-master\src\Compilers\CSharp\Test\CommandLine\CSharpCommandLineTest.csproj
// c:\pib\_dl\dev\roslyn\roslyn-master\src\Compilers\Extension\CompilerExtension.csproj
// c:\pib\_dl\dev\roslyn\roslyn-master\src\Compilers\Server\VBCSCompilerTests\VBCSCompilerTests.csproj



namespace Test_Roslyn_01
{
    public class ResourceFile
    {
        public string File;
        public string Namespace;
    }

    public class CSharpCompiler
    {
        //Microsoft.CodeAnalysis.BuildTasks.Csc
        private string _assemblyName = null;
        private string _outputPath = null;
        private string _pdbPath = null;

        private LanguageVersion _languageVersion = LanguageVersion.CSharp6;                           // CSharp1 ... CSharp6

        private OutputKind _outputKind = OutputKind.DynamicallyLinkedLibrary;                         // ConsoleApplication, WindowsApplication, DynamicallyLinkedLibrary, NetModule, WindowsRuntimeMetadata (.winmdobj file), WindowsRuntimeApplication (.exe that can run in an app container)
        private OptimizationLevel _optimizationLevel = OptimizationLevel.Debug;                       // Debug or Release
        private Platform _platform = Platform.AnyCpu;                                                 // AnyCpu, X86, X64, Itanium, AnyCpu32BitPreferred, Arm
        private ReportDiagnostic _generalDiagnosticOption = ReportDiagnostic.Default;                 // Report a diagnostic as : Default, Error, Warn, Info, Hidden, Suppress
        private int _warningLevel = 4;

        private string _win32ResourceFile = null;
        private IEnumerable<string> _preprocessorSymbols = null;
        private IEnumerable<string> _sourceFiles = null;
        private IEnumerable<string> _assembliesFiles = null;
        private IEnumerable<ResourceFile> _resourceFiles = null;

        public string AssemblyName { get { return _assemblyName; } set { _assemblyName = value; } }
        public string OutputPath { get { return _outputPath; } set { _outputPath = value; } }
        public string PdbPath { get { return _pdbPath; } set { _pdbPath = value; } }
        public LanguageVersion LanguageVersion { get { return _languageVersion; } set { _languageVersion = value; } }
        public OutputKind OutputKind { get { return _outputKind; } set { _outputKind = value; } }
        public OptimizationLevel OptimizationLevel { get { return _optimizationLevel; } set { _optimizationLevel = value; } }
        public Platform Platform { get { return _platform; } set { _platform = value; } }
        public ReportDiagnostic GeneralDiagnosticOption { get { return _generalDiagnosticOption; } set { _generalDiagnosticOption = value; } }
        public int WarningLevel { get { return _warningLevel; } set { _warningLevel = value; } }
        public string Win32ResourceFile { get { return _win32ResourceFile; } set { _win32ResourceFile = value; } }
        public IEnumerable<string> PreprocessorSymbols { get { return _preprocessorSymbols; } set { _preprocessorSymbols = value; } }
        public IEnumerable<string> SourceFiles { get { return _sourceFiles; } set { _sourceFiles = value; } }
        public IEnumerable<string> AssembliesFiles { get { return _assembliesFiles; } set { _assembliesFiles = value; } }
        public IEnumerable<ResourceFile> ResourceFiles { get { return _resourceFiles; } set { _resourceFiles = value; } }

        public void Compile()
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
            CSharpParseOptions parseOptions = CSharpParseOptions.Default.WithLanguageVersion(_languageVersion);
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

            CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(_outputKind, optimizationLevel: _optimizationLevel, platform: _platform, generalDiagnosticOption: _generalDiagnosticOption, warningLevel: _warningLevel);

            CSharpCompilation compilation = CSharpCompilation.Create(
                _assemblyName,
                syntaxTrees: ParseSources2(parseOptions),
                references: GetAssemblyReferences(),
                options: compilationOptions);

            // EmitResult Emit(this CSharpCompilation compilation, string outputPath, string pdbPath = null, string xmlDocumentationPath = null, string win32ResourcesPath = null, IEnumerable<ResourceDescription> manifestResources = null,
            //   CancellationToken cancellationToken = default(CancellationToken));

            string directory = Path.GetDirectoryName(_outputPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            EmitResult result = compilation.Emit(_outputPath, pdbPath: _pdbPath, win32ResourcesPath: _win32ResourceFile, manifestResources: GetResourceDescriptions());

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
            if (_sourceFiles != null)
            {
                //string filename = "";
                foreach (string sourceFile in _sourceFiles)
                {
                    //yield return CSharpSyntaxTree.ParseText(File.ReadAllText(source));
                    string source = File.ReadAllText(sourceFile);
                    SourceText stringText = SourceText.From(source, Encoding.UTF8);
                    yield return SyntaxFactory.ParseSyntaxTree(stringText, parseOptions, sourceFile);
                }
            }
        }

        private IEnumerable<MetadataReference> GetAssemblyReferences()
        {
            if (_assembliesFiles != null)
            {
                foreach (string assemblyFile in _assembliesFiles)
                {
                    // MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
                    yield return MetadataReference.CreateFromFile(assemblyFile);
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

            if (_resourceFiles != null)
            {
                foreach (ResourceFile resourceFile in _resourceFiles)
                {
                    yield return new ResourceDescription(resourceFile.Namespace, () => File.OpenRead(resourceFile.File), true);
                }
            }
        }

        private static void WriteMessages(ImmutableArray<Diagnostic> diagnostics)
        {
            //IEnumerable<Diagnostic> failures = diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            foreach (Diagnostic diagnostic in diagnostics)
            {
                // Translating error diagnostic SourceSpan property to source Line and Column https://social.msdn.microsoft.com/Forums/vstudio/en-US/cebf07d6-f817-4c24-93f2-c66f05a23e20/translating-error-diagnostic-sourcespan-property-to-source-line-and-column?forum=roslyn
                Location location = diagnostic.Location;
                //var lineSpan = Location.SourceTree.GetLineSpan(Location.SourceSpan, usePreprocessorDirectives: false);
                var lineSpan = location.SourceTree.GetLineSpan(location.SourceSpan);
                Console.Error.WriteLine($"{location.SourceTree.FilePath}({lineSpan.StartLinePosition.Line + 1},{lineSpan.StartLinePosition.Character + 1}) : {diagnostic.Severity} ({diagnostic.WarningLevel}) {diagnostic.Id} : {diagnostic.GetMessage()}");

                // SourceFile([63..67))   --   SourceFile(test_form_01\Program.cs[63..67))
                //Console.Error.WriteLine(diagnostic.Location.ToString());
                // (3,14): error CS0234: The type or namespace name 'Linq' does not exist in the namespace 'System' (are you missing an assembly reference?)
                // test_form_01\Program.cs(5,22): error CS0234: The type or namespace name 'Forms' does not exist in the namespace 'System.Windows' (are you missing an assembly reference?)
                //Console.Error.WriteLine(diagnostic.ToString());
                //Console.Error.WriteLine();
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
