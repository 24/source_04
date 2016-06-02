using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Test_CSharpCodeProvider_01
{
    public static class Compiler_01
    {
        public static void Compile(string finalOutputAssembly, string[] sources, string[] referencedAssemblies, string compilerVersion, string compilerOptions = null)
        {
            //string finalOutputAssembly = "test";
            //string[] sources = new string[] { };
            //string[] referencedAssemblies = new string[] { };
            //string compilerOptions = null;

            Dictionary<string, string> providerOption = new Dictionary<string, string>();
            //"v4.0"
            providerOption.Add("CompilerVersion", compilerVersion);
            CSharpCodeProvider provider = new CSharpCodeProvider(providerOption);

            CompilerParameters options = new CompilerParameters();
            options.CompilerOptions = compilerOptions;
            options.GenerateInMemory = false;
            options.OutputAssembly = finalOutputAssembly;
            options.GenerateExecutable = true;
            options.IncludeDebugInformation = true;
            // WarningLevel : from http://msdn.microsoft.com/en-us/library/13b90fz7.aspx
            //   0 Turns off emission of all warning messages.
            //   1 Displays severe warning messages.
            //   2 Displays level 1 warnings plus certain, less-severe warnings, such as warnings about hiding class members.
            //   3 Displays level 2 warnings plus certain, less-severe warnings, such as warnings about expressions that always evaluate to true or false.
            //   4 (the default) Displays all level 3 warnings plus informational warnings.
            options.WarningLevel = 4;
            foreach (string referencedAssembly in referencedAssemblies)
                options.ReferencedAssemblies.Add(referencedAssembly);

            CompilerResults results = provider.CompileAssemblyFromFile(options, sources);
        }
    }
}
