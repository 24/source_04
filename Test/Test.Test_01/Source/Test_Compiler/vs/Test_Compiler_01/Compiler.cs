using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;
using Microsoft.CSharp;

namespace Test_Compiler_01
{
    public static class Compiler
    {
        public static void Compile(string sourceName, string[] assemblies)
        {
            //string outputAssembly = "";
            FileInfo sourceFile = new FileInfo(sourceName);
            string outputAssembly = String.Format(@"{0}\{1}.exe", Environment.CurrentDirectory, sourceFile.Name.Replace(".", "_"));

            CompilerParameters options = new CompilerParameters();
            //options.CompilerOptions = "";
            options.GenerateInMemory = false;
            options.OutputAssembly = outputAssembly;
            options.GenerateExecutable = true;
            options.IncludeDebugInformation = true;
            foreach (string assembly in assemblies)
            {
                Console.WriteLine("assembly \"{0}\"", assembly);
                options.ReferencedAssemblies.Add(assembly);
            }

            CodeDomProvider provider = null;
            Dictionary<string, string> providerOption = new Dictionary<string, string>();
            providerOption.Add("CompilerVersion", "v4.0");
            provider = new CSharpCodeProvider(providerOption);
        }

        public static bool CompileExecutable(string sourceName, string[] assemblies)
        {
            // from http://msdn.microsoft.com/en-us/library/Microsoft.CSharp.CSharpCodeProvider(v=vs.100).aspx


            string compilerOptions = "/nostdlib+ /highentropyva- /filealign:512 /platform:x86";
            string[] standardAssemblies = { "mscorlib.dll", "System.dll" };

            FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;
            bool compileOk = false;

            Console.WriteLine("source \"{0}\"", sourceName);

            // Select the code provider based on the input file extension.
            if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
            {

                Dictionary<string, string> provOptions = new Dictionary<string, string>();
                //provOptions.Add("CompilerVersion", "v4");
                provOptions.Add("CompilerVersion", "v4.0");
                //provOptions.Add("CompilerVersion", "v3.5");
                //provOptions.Add("CompilerVersion", "v3.0");

                provider = CodeDomProvider.CreateProvider("CSharp", provOptions);
                Console.WriteLine("provider : \"{0}\"", provider.ToString());

                foreach (KeyValuePair<string, string> option in provOptions)
                {
                    Console.WriteLine("provider option : \"{0}\" = \"{1}\"", option.Key, option.Value);
                }
            }
            else if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".VB")
            {
                provider = CodeDomProvider.CreateProvider("VisualBasic");
            }
            else
            {
                Console.WriteLine("Source file must have a .cs or .vb extension");
            }

            if (provider != null)
            {

                // Format the executable file name.
                // Build the output assembly path using the current directory
                // and <source>_cs.exe or <source>_vb.exe.

                String exeName = String.Format(@"{0}\{1}.exe", Environment.CurrentDirectory, sourceFile.Name.Replace(".", "_"));
                Console.WriteLine("exeName  {0}", exeName);

                CompilerParameters cp = new CompilerParameters();

                // Generate an executable instead of 
                // a class library.
                cp.GenerateExecutable = true;

                // Specify the assembly file name to generate.
                cp.OutputAssembly = exeName;

                // Save the assembly as a physical file.
                cp.GenerateInMemory = false;

                cp.IncludeDebugInformation = true;

                // Set whether to treat all warnings as errors.
                cp.TreatWarningsAsErrors = false;

                cp.CompilerOptions = compilerOptions;

                Console.WriteLine("compiler options  \"{0}\"", compilerOptions);

                foreach (string assembly in standardAssemblies)
                {
                    Console.WriteLine("assembly \"{0}\"", assembly);
                    cp.ReferencedAssemblies.Add(assembly);
                }

                foreach (string assembly in assemblies)
                {
                    Console.WriteLine("assembly \"{0}\"", assembly);
                    cp.ReferencedAssemblies.Add(assembly);
                }

                // Invoke compilation of the source file.
                CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

                if (cr.Errors.Count > 0)
                {
                    // Display compilation errors.
                    Console.WriteLine("Errors building {0} into {1}", sourceName, cr.PathToAssembly);
                    foreach (CompilerError ce in cr.Errors)
                    {
                        Console.WriteLine("  {0}", ce.ToString());
                        Console.WriteLine();
                    }
                }
                else
                {
                    // Display a successful compilation message.
                    Console.WriteLine("Source {0} built into {1} successfully.", sourceName, cr.PathToAssembly);
                }

                // Return the results of the compilation.
                if (cr.Errors.Count > 0)
                {
                    compileOk = false;
                }
                else
                {
                    compileOk = true;
                }
            }
            return compileOk;
        }
    }
}
