using pb.Data.Mongo;
using System;
using System.Reflection;

namespace pb.Reflection.Test
{
    public static class Test_MethodInfo
    {
        public static void Test_01()
        {
            Trace.WriteLine("Test_MethodInfo.Test_01");
        }

        //string typeName
        public static string Test_GetMethod_01(string methodName, bool test = true)
        {
            // "pb.Reflection.Test.Test_MethodInfo.Test_01"
            MethodElements methodElements = zReflection.GetMethodElements(methodName);
            methodElements.zTraceJson();
            //Trace.WriteLine("Test_MethodInfo.Test_01");

            //Assembly.GetExecutingAssembly()
            //Assembly.GetCallingAssembly()
            //Assembly.GetEntryAssembly()

            Assembly assembly;
            if (methodElements.AssemblyName != null)
                assembly = zReflection.GetAssembly(methodElements.AssemblyName, ErrorOptions.None);
            else
                assembly = Assembly.GetExecutingAssembly();
            if (assembly != null)
                Trace.WriteLine($"assembly {assembly.FullName}");
            else
            {
                Trace.WriteLine("assembly not found");
                return null;
            }

            Type type = zReflection.GetType(assembly, methodElements.TypeName, ErrorOptions.TraceWarning);
            if (type != null)
                Trace.WriteLine($"type {type.AssemblyQualifiedName}");
            else
            {
                Trace.WriteLine("type not found");
                return null;
            }

            MethodInfo methodInfo = zReflection.GetMethod(type, methodElements.MethodName, ErrorOptions.TraceWarning);
            if (methodInfo != null)
                Trace.WriteLine($"method {methodInfo.zGetName()}");
            else
            {
                Trace.WriteLine("method not found");
                return null;
            }

            Trace.WriteLine($"method definition {zReflection.GetDefinition(methodInfo)}");

            ParameterInfo[] parameters = methodInfo.GetParameters();
            Trace.WriteLine($"method parameters : count {parameters.Length}");
            foreach (ParameterInfo parameter in parameters)
            {
                TraceParameterInfo(parameter);
                Trace.WriteLine();
            }

            Trace.WriteLine($"method return parameter :");
            TraceParameterInfo(methodInfo.ReturnParameter);

            return null;
        }

        private static void TraceParameterInfo(ParameterInfo parameter)
        {
            Trace.WriteLine($"  Position                {parameter.Position}");
            Trace.WriteLine($"  Name                    \"{parameter.Name}\"");
            Trace.WriteLine($"  Type                    \"{parameter.ParameterType.zGetTypeName()}\"");
            Trace.WriteLine($"  Attributes              {parameter.Attributes}");
            Trace.WriteLine($"  HasDefaultValue         {parameter.HasDefaultValue}");
            Trace.WriteLine("  DefaultValue            {0}", parameter.DefaultValue != null ? parameter.DefaultValue : "-null-");
        }
    }
}
