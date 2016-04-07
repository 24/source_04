using System;
using System.Collections.Generic;
using System.Reflection;

namespace pb.Compiler
{
    public static class AssemblyResolve
    {
        private static bool _traceAssemblyResolve = false;
        private static bool _traceAssemblyLoad = false;
        private static bool _active = false;
        private static Dictionary<string, string> _assemblies = new Dictionary<string, string>();

        public static bool TraceAssemblyResolve { get { return _traceAssemblyResolve; } set { _traceAssemblyResolve = value; } }
        public static bool TraceAssemblyLoad { get { return _traceAssemblyLoad; } set { _traceAssemblyLoad = value; } }

        // bool traceAssemblyResolve = false, bool traceAssemblyLoad = false
        public static void Start()
        {
            //_traceAssemblyResolve = traceAssemblyResolve;
            //_traceAssemblyLoad = traceAssemblyLoad;
            if (!_active)
            {
                _active = true;
                // AppDomain.AssemblyResolve : Occurs when the resolution of an assembly fails.    https://msdn.microsoft.com/en-us/library/system.appdomain.assemblyresolve(v=vs.110).aspx
                AppDomain.CurrentDomain.AssemblyResolve += _AssemblyResolve; //new ResolveEventHandler(_AssemblyResolve);
                AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoad;
                if (_traceAssemblyResolve)
                    Trace.WriteLine("AssemblyResolve : start assembly resolve");
            }
        }

        public static void Stop()
        {
            if (_active)
            {
                _active = false;
                AppDomain.CurrentDomain.AssemblyResolve -= _AssemblyResolve; //new ResolveEventHandler(_AssemblyResolve);
                AppDomain.CurrentDomain.AssemblyLoad -= AssemblyLoad;
                if (_traceAssemblyResolve)
                    Trace.WriteLine("AssemblyResolve : stop assembly resolve");
            }
        }

        private static Assembly _AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_assemblies.ContainsKey(args.Name))
            {
                if (_traceAssemblyResolve)
                    Trace.WriteLine("AssemblyResolve : resolve assembly \"{0}\" as \"{1}\"", args.Name, _assemblies[args.Name]);
                return Assembly.LoadFrom(_assemblies[args.Name]);
            }
            else if (_traceAssemblyResolve)
                Trace.WriteLine("AssemblyResolve : unable to resolve assembly \"{0}\"", args.Name);
            return null;
        }

        private static void AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (_traceAssemblyLoad)
                Trace.WriteLine("AssemblyResolve : load assembly event \"{0}\" \"{1}\"", args.LoadedAssembly.FullName, !args.LoadedAssembly.IsDynamic ? args.LoadedAssembly.Location : null);
        }

        public static void Clear()
        {
            _assemblies.Clear();
            if (_traceAssemblyResolve)
                Trace.WriteLine("AssemblyResolve : clear assembly list");
        }

        public static void Add(string file, string name)
        {
            if (name == null)
                throw new PBException("error you need to specify the name of assembly to resolve it (\"Test_dll, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\")");
            //string name = GetAssemblyName(file, version, culture, publicKeyToken);
            //if (name == null)
            //    name = string.Format("{0}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Path.GetFileNameWithoutExtension(file));
            if (_traceAssemblyResolve)
                Trace.WriteLine("AssemblyResolve : add assembly to resolve \"{0}\" as \"{1}\"", name, file);
            if (_assemblies.ContainsKey(name))
                _assemblies.Remove(name);
            _assemblies.Add(name, file);
        }

        //public static void Remove(string file, string version = null, string culture = null, string publicKeyToken = null)
        public static void Remove(string file, string name)
        {
            //string name = GetAssemblyName(file, version, culture, publicKeyToken);
            if (_traceAssemblyResolve)
                Trace.WriteLine("AssemblyResolve : remove assembly to resolve \"{0}\" as \"{1}\"", name, file);
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

        //private static void WriteLine(string msg, params object[] prm)
        //{
        //    if (_traceAssemblyResolve)
        //        pb.Trace.CurrentTrace.WriteLine(msg, prm);
        //}
    }
}
