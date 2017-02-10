using pb.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pb.Compiler
{
    // les méthodes init() sont appelées à la première exécution du projet et les méthodes end() sont appelées à la fermeture du projet ou à la fermeture du programme
    // si on demande explicitement à appeler la méthode init() on appel d'abord la méthode end() puis la méthode init()
    // si il y a un changement dans la liste des méthodes init on appel 
    public class RunSourceInitEndMethods_v1
    {
        private static bool __traceRunOnce = false;
        private static bool __traceRunAlways = false;
        private bool _callInitRunOnce = true;
        private Dictionary<string, MethodInfo> _initMethodsRunOnce = null;
        private Dictionary<string, MethodInfo> _endMethodsRunOnce = null;
        private Dictionary<string, MethodInfo> _endMethodsRunAlways = null;

        public static bool TraceRunOnce { get { return __traceRunOnce; } set { __traceRunOnce = value; } }
        public static bool TraceRunAlways { get { return __traceRunAlways; } set { __traceRunAlways = value; } }
        public bool CallInitRunOnce { get { return _callInitRunOnce; } set { _callInitRunOnce = value; } }


        //public void CallInitMethods(IEnumerable<string> initMethodNames, IEnumerable<string> endMethodNames, Func<string, MethodInfo> getMethod)
        //{
        //    Dictionary<string, MethodInfo> initMethods = CreateDictionary(initMethodNames);
        //    Dictionary<string, MethodInfo> endMethods = CreateDictionary(endMethodNames);

        //    bool callInit = _callInit;

        //    //if (forceCallInit)
        //    //    callInit = true;

        //    // check if init methods or end methods change
        //    if (!callInit && (!MethodsEquals(_initMethods, initMethods) || !MethodsEquals(_endMethods, endMethods)))
        //        callInit = true;


        //    if (callInit)
        //    {
        //        CallEndMethods();

        //        GetMethods(initMethods, getMethod);
        //        GetMethods(endMethods, getMethod);
        //        _initMethods = initMethods;
        //        _endMethods = endMethods;
        //        CallMethods(_initMethods, init: true);
        //        _callInit = false;
        //    }
        //}

        // callInit
        public void CallInitMethods(IEnumerable<InitEndMethod> initMethods, IEnumerable<InitEndMethod> endMethods, bool callInitRunOnce, Func<string, MethodInfo> getMethod)
        {
            Dictionary<string, MethodInfo> initMethodsRunAlways = CreateDictionary(initMethods.Where(method => method.RunType == RunType.Always).Select(method => method.Name));
            Dictionary<string, MethodInfo> endMethodsRunAlways = CreateDictionary(endMethods.Where(method => method.RunType == RunType.Always).Select(method => method.Name));
            Dictionary<string, MethodInfo> initMethodsRunOnce = CreateDictionary(initMethods.Where(method => method.RunType == RunType.Once).Select(method => method.Name));
            Dictionary<string, MethodInfo> endMethodsRunOnce = CreateDictionary(endMethods.Where(method => method.RunType == RunType.Once).Select(method => method.Name));

            //bool callInit = _callInit;

            // check if init methods or end methods change
            if (!callInitRunOnce && (!MethodsEquals(_initMethodsRunOnce, initMethodsRunOnce) || !MethodsEquals(_endMethodsRunOnce, endMethodsRunOnce)))
                callInitRunOnce = true;

            if (callInitRunOnce)
                CallEndMethodsRunOnce();

            CallEndMethodsRunAlways();

            if (callInitRunOnce)
            {
                GetMethods(initMethodsRunOnce, getMethod);
                GetMethods(endMethodsRunOnce, getMethod);
                _initMethodsRunOnce = initMethodsRunOnce;
                _endMethodsRunOnce = endMethodsRunOnce;
                CallMethods(_initMethodsRunOnce, always: false, init: true);
                _callInitRunOnce = false;
            }

            GetMethods(initMethodsRunAlways, getMethod);
            GetMethods(endMethodsRunAlways, getMethod);
            _endMethodsRunAlways = endMethodsRunAlways;
            CallMethods(initMethodsRunAlways, always: true, init: true);
        }

        public void CallEndMethods()
        {
            CallEndMethodsRunOnce();
            CallEndMethodsRunAlways();
        }

        private void CallEndMethodsRunOnce()
        {
            CallMethods(_endMethodsRunOnce, always: false, init: false);
            _endMethodsRunOnce = null;
        }

        private void CallEndMethodsRunAlways()
        {
            CallMethods(_endMethodsRunAlways, always: true, init: false);
            _endMethodsRunAlways = null;
        }

        private Dictionary<string, MethodInfo> CreateDictionary(IEnumerable<string> methodNames)
        {
            Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
            foreach (string methodName in methodNames)
            {
                if (!methods.ContainsKey(methodName))
                    methods.Add(methodName, null);
            }
            return methods;
        }

        private bool MethodsEquals(Dictionary<string, MethodInfo> methods1, Dictionary<string, MethodInfo> methods2)
        {
            if (methods1 == null && methods2 == null)
                return true;
            if (methods1 == null || methods2 == null)
                return false;
            if (methods1.Count != methods2.Count)
                return false;
            foreach (string methodsName in methods1.Keys)
            {
                if (!methods2.ContainsKey(methodsName))
                    return false;
            }
            return true;
        }

        private void GetMethods(Dictionary<string, MethodInfo> methods, Func<string, MethodInfo> getMethod)
        {
            foreach (string methodsName in methods.Keys.ToArray())
            {
                MethodInfo method = getMethod(methodsName);
                methods[methodsName] = method;
            }
        }

        private void CallMethods(Dictionary<string, MethodInfo> methods, bool always, bool init)
        {
            if (methods == null)
                return;
            foreach (MethodInfo method in methods.Values)
            {
                try
                {
                    if (method != null)
                    {
                        if ((__traceRunOnce && !always) || (__traceRunAlways && always))
                            Trace.WriteLine("call {0} {1} methods \"{2}\"", always ? "always" : "once", init ? "init" : "end", method.zGetName());
                        method.Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteError(ex);
                }
            }
        }
    }
}
