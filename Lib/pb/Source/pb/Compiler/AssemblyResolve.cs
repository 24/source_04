using System;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Compiler
{
    public class AssemblyResolve
    {
        private static bool _trace = false;
        private static bool _active = false;
        private static Dictionary<string, string> _assemblies = new Dictionary<string, string>();

        public static bool Trace { get { return _trace; } set { _trace = value; } }

        public static void Start()
        {
            if (!_active)
            {
                _active = true;
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(_AssemblyResolve);
            }
        }

        public static void Stop()
        {
            if (_active)
            {
                _active = false;
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(_AssemblyResolve);
            }
        }

        private static Assembly _AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_assemblies.ContainsKey(args.Name))
            {
                WriteLine("Resolve assembly \"{0}\" as \"{1}\"", args.Name, _assemblies[args.Name]);
                return Assembly.LoadFrom(_assemblies[args.Name]);
            }
            else
                WriteLine("Unable to resolve assembly \"{0}\"", args.Name);
            return null;
        }

        public static void Clear()
        {
            _assemblies.Clear();
        }

        //public static void Add(string file, string version = null, string culture = null, string publicKeyToken = null)
        public static void Add(string file, string name)
        {
            if (name == null)
                throw new PBException("error you need to specify the name of assembly to resolve it (\"Test_dll, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")");
            //string name = GetAssemblyName(file, version, culture, publicKeyToken);
            //if (name == null)
            //    name = string.Format("{0}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Path.GetFileNameWithoutExtension(file));
            WriteLine("Add assembly to resolve \"{0}\" as \"{1}\"", name, file);
            if (_assemblies.ContainsKey(name))
                _assemblies.Remove(name);
            _assemblies.Add(name, file);
        }

        //public static void Remove(string file, string version = null, string culture = null, string publicKeyToken = null)
        public static void Remove(string file, string name)
        {
            //string name = GetAssemblyName(file, version, culture, publicKeyToken);
            if (_assemblies.ContainsKey(name))
                _assemblies.Remove(name);
        }

        //private static string GetAssemblyName(string file, string version = null, string culture = null, string publicKeyToken = null)
        //{
        //    // "Test_JScript_01_na, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
        //    if (version == null)
        //        version = "1.0.0.0";
        //    if (culture == null)
        //        culture = "neutral";
        //    if (publicKeyToken == null)
        //        publicKeyToken = "null";
        //    return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", Path.GetFileNameWithoutExtension(file), version, culture, publicKeyToken);
        //}

        private static void WriteLine(string msg, params object[] prm)
        {
            if (_trace)
                pb.Trace.CurrentTrace.WriteLine(msg, prm);
        }
    }
}
