using pb.Data;
using pb.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pb.Compiler
{
    //MethodParameters
    public class MethodInfoWithParam
    {
        public MethodInfo MethodInfo;
        //public object[] Parameters;
        public NamedValues<ZValue> NamedParameters;
        public object[] Parameters;
    }

    // les méthodes init() sont appelées à la première exécution du projet et les méthodes end() sont appelées à la fermeture du projet ou à la fermeture du programme
    // si on demande explicitement à appeler la méthode init() on appel d'abord la méthode end() puis la méthode init()
    // si il y a un changement dans la liste des méthodes init on appel 
    // v2 : utilisation de paramètres dans l'appel des fonctions init et end, remplacement de MethodInfo par MethodInfoWithParam
    public class RunSourceInitEndMethods_v2
    {
        private bool _traceRunOnce = false;
        private bool _traceRunAlways = false;
        private bool _callInitRunOnce = true;
        private Dictionary<string, MethodInfoWithParam> _initMethodsRunOnce = null;
        private Dictionary<string, MethodInfoWithParam> _endMethodsRunOnce = null;
        private Dictionary<string, MethodInfoWithParam> _endMethodsRunAlways = null;

        public bool TraceRunOnce { get { return _traceRunOnce; } set { _traceRunOnce = value; } }
        public bool TraceRunAlways { get { return _traceRunAlways; } set { _traceRunAlways = value; } }
        public bool CallInitRunOnce { get { return _callInitRunOnce; } set { _callInitRunOnce = value; } }

        // callInit
        public void CallInitMethods(IEnumerable<InitEndMethod> initMethods, IEnumerable<InitEndMethod> endMethods, bool callInitRunOnce, Func<string, MethodInfo> getMethod)
        {
            // .Select(method => method.Name)
            Dictionary<string, MethodInfoWithParam> initMethodsRunAlways = CreateDictionary(initMethods.Where(method => method.RunType == RunType.Always));
            Dictionary<string, MethodInfoWithParam> endMethodsRunAlways = CreateDictionary(endMethods.Where(method => method.RunType == RunType.Always));
            Dictionary<string, MethodInfoWithParam> initMethodsRunOnce = CreateDictionary(initMethods.Where(method => method.RunType == RunType.Once));
            Dictionary<string, MethodInfoWithParam> endMethodsRunOnce = CreateDictionary(endMethods.Where(method => method.RunType == RunType.Once));

            // check if run once init or end methods change
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

        //private Dictionary<string, MethodInfoWithParam> CreateDictionary(IEnumerable<string> methodNames)
        private Dictionary<string, MethodInfoWithParam> CreateDictionary(IEnumerable<InitEndMethod> methods)
        {
            Dictionary<string, MethodInfoWithParam> dicMethods = new Dictionary<string, MethodInfoWithParam>();
            foreach (InitEndMethod method in methods)
            {
                if (!dicMethods.ContainsKey(method.Name))
                    dicMethods.Add(method.Name, new MethodInfoWithParam { NamedParameters = method.Parameters });
            }
            return dicMethods;
        }

        private bool MethodsEquals(Dictionary<string, MethodInfoWithParam> methods1, Dictionary<string, MethodInfoWithParam> methods2)
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

        private void GetMethods(Dictionary<string, MethodInfoWithParam> methods, Func<string, MethodInfo> getMethod)
        {
            foreach (string methodName in methods.Keys.ToArray())
            {
                //MethodInfo method = getMethod(methodsName);
                //methods[methodsName] = method;
                MethodInfoWithParam mwp = methods[methodName];
                mwp.MethodInfo = getMethod(methodName);
                mwp.Parameters = MethodParameters.GetParameters(mwp.MethodInfo, mwp.NamedParameters, ErrorOptions.TraceWarning);
            }
        }

        private void CallMethods(Dictionary<string, MethodInfoWithParam> methods, bool always, bool init)
        {
            if (methods == null)
                return;
            foreach (MethodInfoWithParam method in methods.Values)
            {
                try
                {
                    if (method != null && method.MethodInfo != null)
                    {
                        if ((_traceRunOnce && !always) || (_traceRunAlways && always))
                            Trace.WriteLine("call {0} {1} methods \"{2}\"", always ? "always" : "once", init ? "init" : "end", method.MethodInfo.zGetName());
                        method.MethodInfo.Invoke(null, method.Parameters);
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
