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
    public class RunSourceInitEndMethods
    {
        //private bool _initCalled = false;
        private bool _callInit = true;
        private Dictionary<string, MethodInfo> _initMethods = null;
        private Dictionary<string, MethodInfo> _endMethods = null;

        public bool CallInit { get { return _callInit; } set { _callInit = value; } }

        //public void OpenProject()
        //{
        //}

        //public void CloseProject()
        //{
        //}

        public void CallInitMethods(IEnumerable<string> initMethodNames, IEnumerable<string> endMethodNames, Func<string, MethodInfo> getMethod)
        {
            Dictionary<string, MethodInfo> initMethods = CreateDictionary(initMethodNames);
            Dictionary<string, MethodInfo> endMethods = CreateDictionary(endMethodNames);

            //if (_initCalled)
            //if (!_callInit)
            //{
            //    if (!MethodsEquals(_initMethods, initMethods) || !MethodsEquals(_endMethods, endMethods))
            //        // call end methods and set _callInit to true
            //        CallEndMethods();
            //}

            // check if init methods or end methods change
            if (!_callInit && (!MethodsEquals(_initMethods, initMethods) || !MethodsEquals(_endMethods, endMethods)))
                _callInit = true;


            //if (!_initCalled)
            if (_callInit)
            {
                CallEndMethods();

                GetMethods(initMethods, getMethod);
                GetMethods(endMethods, getMethod);
                _initMethods = initMethods;
                _endMethods = endMethods;
                CallMethods(_initMethods);
                //_initCalled = true;
                _callInit = false;
            }
        }

        public void CallEndMethods()
        {
            //if (_initCalled)
            //{
            CallMethods(_endMethods);
            _endMethods = null;
            //_initCalled = false;
            //_callInit = true;
            //}
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

        private void CallMethods(Dictionary<string, MethodInfo> methods)
        {
            if (methods == null)
                return;
            foreach (MethodInfo method in methods.Values)
            {
                try
                {
                    if (method != null)
                    {
                        Trace.WriteLine("call init methods \"{0}\"", method.zGetName());
                        method.Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    Trace.CurrentTrace.WriteError(ex);
                }
            }
        }
    }
}
