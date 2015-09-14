using System;
using System.Reflection;
using pb;
using pb.IO;

namespace Test.Test_Reflection
{
    public static class Test_Assembly
    {
        public static void Test_Assembly_01()
        {
            // calling assembly : RunSource_00003.dll
            Trace.WriteLine("calling assembly        : \"{0}\"", Assembly.GetCallingAssembly().Location);
            // entry assembly : runsource.runsource.exe
            Trace.WriteLine("entry assembly          : \"{0}\"", Assembly.GetEntryAssembly().Location);
            // executing assembly : RunSource_00003.dll
            Trace.WriteLine("executing assembly      : \"{0}\"", Assembly.GetExecutingAssembly().Location);
        }

        public static void Test_Assembly_02()
        {
            // entry assembly : runsource.runsource.exe
            Assembly assembly = Assembly.GetEntryAssembly();
            Trace.WriteLine("entry assembly          : \"{0}\"", assembly.Location);
            Trace.WriteLine("  assembly.GetReferencedAssemblies()");
            foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
            {
                Trace.WriteLine("    referenced assembly : Name \"{0}\" CodeBase \"{1}\" CultureName \"{2}\" FullName \"{3}\"", assemblyName.Name, assemblyName.CodeBase, assemblyName.CultureName, assemblyName.FullName);
            }
        }

        public static void Test_Assembly_Module_01()
        {
            // entry assembly : runsource.runsource.exe
            Assembly assembly = Assembly.GetEntryAssembly();
            Trace.WriteLine("entry assembly          : \"{0}\"", assembly.Location);
            Trace.WriteLine("  assembly.GetLoadedModules(true)");
            foreach (Module module in assembly.GetLoadedModules(true))
            {
                Trace.WriteLine("    module : Name \"{0}\" ScopeName \"{1}\" FullyQualifiedName \"{2}\"", module.Name, module.ScopeName, module.FullyQualifiedName);
            }
            Trace.WriteLine("  assembly.GetModules(true)");
            foreach (Module module in assembly.GetModules(true))
            {
                Trace.WriteLine("    module : Name \"{0}\" ScopeName \"{1}\" FullyQualifiedName \"{2}\"", module.Name, module.ScopeName, module.FullyQualifiedName);
            }

            Trace.WriteLine("  assembly.Modules");
            foreach (Module module in assembly.Modules)
            {
                Trace.WriteLine("    module : Name \"{0}\" ScopeName \"{1}\" FullyQualifiedName \"{2}\"", module.Name, module.ScopeName, module.FullyQualifiedName);
            }
        }

        public static void Test_Assembly_Type_01()
        {
            // entry assembly : runsource.runsource.exe
            Assembly assembly = Assembly.GetEntryAssembly();
            Trace.WriteLine("entry assembly          : \"{0}\"", assembly.Location);
            Trace.WriteLine("  assembly.GetTypes()");
            // assembly.GetTypes() and assembly.DefinedTypes return same type list
            foreach (Type type in assembly.GetTypes())
            {
                Trace.WriteLine("    assembly type : Name \"{0}\" FullName \"{1}\" AssemblyQualifiedName \"{2}\"", type.Name, type.FullName, type.AssemblyQualifiedName);
            }
            //assembly.DefinedTypes
        }

        public static void Test_Assembly_Type_02()
        {
            // entry assembly : runsource.runsource.exe
            Assembly assembly = Assembly.GetEntryAssembly();
            Trace.WriteLine("entry assembly          : \"{0}\"", assembly.Location);
            Trace.WriteLine("  assembly.DefinedTypes");
            // assembly.GetTypes() and assembly.DefinedTypes return same type list
            foreach (Type type in assembly.DefinedTypes)
            {
                Trace.WriteLine("    assembly type : Name \"{0}\" FullName \"{1}\" AssemblyQualifiedName \"{2}\"", type.Name, type.FullName, type.AssemblyQualifiedName);
            }
        }

        public static void Test_Assembly_Type_03(string filename)
        {
            Assembly assembly = GetAssembly(filename);
            Trace.WriteLine("assembly          : \"{0}\"", assembly.Location);
            Trace.WriteLine("  assembly.GetTypes()");
            // assembly.GetTypes() and assembly.DefinedTypes return same type list
            foreach (Type type in assembly.GetTypes())
            {
                Trace.WriteLine("    assembly type : Name \"{0}\" FullName \"{1}\" AssemblyQualifiedName \"{2}\"", type.Name, type.FullName, type.AssemblyQualifiedName);
            }
        }

        public static Assembly GetAssembly(string filename)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (zPath.GetFileName(assembly.Location) == filename)
                    return assembly;
            }
            return null;
        }

        public static void Test_AppDomain_Assembly_01()
        {
            Trace.WriteLine("AppDomain.CurrentDomain.GetAssemblies() :");
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic)
                    Trace.WriteLine("    assembly :         FullName \"{0}\" Location \"{1}\"", assembly.FullName, assembly.Location);
                else
                    //Trace.WriteLine("    assembly : is dynamic");
                    Trace.WriteLine("    assembly : dynamic FullName \"{0}\"", assembly.FullName);
            }
        }

        public static void Test_AppDomain_Assembly_02()
        {
            Trace.WriteLine("AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies() :");
            foreach (Assembly assembly in AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
            {
                Trace.WriteLine("    assembly : FullName \"{0}\" Location \"{1}\"", assembly.FullName, assembly.Location);
            }
        }

        public static void Test_GetType_01(string typeName)
        {
            Trace.WriteLine("Type.GetType() : \"{0}\"", typeName);
            Type type = Type.GetType(typeName);
            if (type != null)
                Trace.WriteLine("  assembly type : Name \"{0}\" FullName \"{1}\" AssemblyQualifiedName \"{2}\"", type.Name, type.FullName, type.AssemblyQualifiedName);
            else
                Trace.WriteLine("  type not found");
        }
    }
}
