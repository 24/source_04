using System;
using System.Reflection;
using pb;
using pb.Compiler;

// pb la dll généré par JScriptCodeProvider ne défini pas de culture ni de version de l'assembly
// dans les propriétés de la dll on a :
//  Language : Invariant language (Invariant country)
//  File version : 0.0.0.0
//  Product version : (vide)
// dans une dll csharp on a :
//  Language : Invariant Neutral
//  File version : 1.0.0.0
//  Product version : 1.0.0.0



namespace Test_JScript
{
    static partial class w
    {
        private static RunSource _rs = RunSource.CurrentRunSource;
        private static ITrace _tr = Trace.CurrentTrace;

        public static void Init()
        {
        }

        public static void End()
        {
        }

        public static void Test_01()
        {
            _tr.WriteLine("Test_01");
        }

        public static void Test_JScript_01()
        {
            _tr.WriteLine("Test_JScript_01");
            //string s = global::Test_JScript_01.test.test_01();
            //_tr.WriteLine("call jscript Test_JScript_01.test.test_01() : \"{0}\"", s);
        }

        public static void Test_SetAssemblyEvent_01()
        {
            Test_UnsetAssemblyEvent_01();
            _tr.WriteLine("Test_SetAssemblyEvent_01");
            ResolveEventHandler resolveEventHandler = new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            AssemblyLoadEventHandler assemblyLoadEventHandler = new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            _rs.Data.Add("ResolveEventHandler", resolveEventHandler);
            _rs.Data.Add("AssemblyLoadEventHandler", assemblyLoadEventHandler);
            AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;
            AppDomain.CurrentDomain.AssemblyLoad += assemblyLoadEventHandler;
        }

        public static void Test_UnsetAssemblyEvent_01()
        {
            if (_rs.Data.ContainsKey("ResolveEventHandler"))
            {
                ResolveEventHandler resolveEventHandler = (ResolveEventHandler)_rs.Data["ResolveEventHandler"];
                AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;
                _rs.Data.Remove("ResolveEventHandler");
            }
            if (_rs.Data.ContainsKey("AssemblyLoadEventHandler"))
            {
                AssemblyLoadEventHandler assemblyLoadEventHandler = (AssemblyLoadEventHandler)_rs.Data["AssemblyLoadEventHandler"];
                AppDomain.CurrentDomain.AssemblyLoad -= assemblyLoadEventHandler;
                _rs.Data.Remove("AssemblyLoadEventHandler");
            }
        }

        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            //AppDomain domain = sender as AppDomain;
            //string senderTxt = "-NA-";
            //if (domain != null)
            //    senderTxt = "domain " + domain.FriendlyName;
            //_tr.WriteLine("CurrentDomain_AssemblyLoad");
            //_tr.WriteLine("  sender                    {0}", senderTxt);
            //_tr.WriteLine("  args.LoadedAssembly       {0}", args.LoadedAssembly.FullName);
            _tr.WriteLine("Assembly loaded \"{0}\"", args.LoadedAssembly.FullName);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //AppDomain domain = sender as AppDomain;
            //string senderTxt = "-NA-";
            //if (domain != null)
            //    senderTxt = "domain " + domain.FriendlyName;
            //_tr.WriteLine("CurrentDomain_AssemblyResolve");
            //_tr.WriteLine("  sender                    {0}", senderTxt);
            //_tr.WriteLine("  args.Name                 {0}", args.Name);
            //_tr.WriteLine("  args.RequestingAssembly   {0}", args.RequestingAssembly.FullName);
            _tr.WriteLine("Assembly resolve \"{0}\" requested by \"{1}\"", args.Name, args.RequestingAssembly.FullName);
            if (args.Name == "Test_JScript_01_na, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                string file = @"c:\pib\dropbox\pbeuz\Dropbox\dev\project\Source\Source_01\Source\Test\Test_JScript\Test_JScript_01_js\bin\Test_JScript_01_na.dll";
                _tr.WriteLine("Assembly load from \"{0}\"", file);
                return Assembly.LoadFrom(file);
            }
            return null;
        }
    }
}
