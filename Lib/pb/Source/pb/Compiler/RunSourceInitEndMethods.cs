﻿using pb.Reflection;
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
        private bool _callInit = true;
        private Dictionary<string, MethodInfo> _initMethods = null;
        private Dictionary<string, MethodInfo> _endMethods = null;

        public bool CallInit { get { return _callInit; } set { _callInit = value; } }


        //bool forceCallInit
        public void CallInitMethods(IEnumerable<string> initMethodNames, IEnumerable<string> endMethodNames, Func<string, MethodInfo> getMethod)
        {
            Dictionary<string, MethodInfo> initMethods = CreateDictionary(initMethodNames);
            Dictionary<string, MethodInfo> endMethods = CreateDictionary(endMethodNames);

            bool callInit = _callInit;

            //if (forceCallInit)
            //    callInit = true;

            // check if init methods or end methods change
            if (!callInit && (!MethodsEquals(_initMethods, initMethods) || !MethodsEquals(_endMethods, endMethods)))
                callInit = true;


            if (callInit)
            {
                CallEndMethods();

                GetMethods(initMethods, getMethod);
                GetMethods(endMethods, getMethod);
                _initMethods = initMethods;
                _endMethods = endMethods;
                CallMethods(_initMethods, init: true);
                _callInit = false;
            }
        }

        public void CallEndMethods()
        {
            CallMethods(_endMethods, init: false);
            _endMethods = null;
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

        private void CallMethods(Dictionary<string, MethodInfo> methods, bool init)
        {
            if (methods == null)
                return;
            string s;
            if (init)
                s = "init";
            else
                s = "end";
            foreach (MethodInfo method in methods.Values)
            {
                try
                {
                    if (method != null)
                    {
                        Trace.WriteLine("call {0} methods \"{1}\"", s, method.zGetName());
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
