using System;
using System.Linq;
using System.Reflection;

namespace pb
{
    public class MethodElements
    {
        public string MethodName;
        public string TypeName;
        public string QualifiedTypeName;
        public string AssemblyName;
    }

    public static class Reflection
    {
        public static string GetName(Type type)
        {
            if (!type.IsGenericType)
                return type.FullName;
            string name = type.Namespace + "." + type.Name + "[";
            bool first = true;
            foreach (Type argument in type.GetGenericArguments())
            {
                if (!first)
                    name += ", ";
                name += "[" + GetName(argument) + "]";
                first = false;
            }
            name += "]";
            return name;
        }

        public static string GetDefinition(this MethodInfo method)
        {
            string s = "";
            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                if (s != "") s += ", ";
                //s += parameter.ParameterType.zName();
                s += parameter.ParameterType.zGetTypeName();
                if (parameter.Name != null)
                    s += " " + parameter.Name;
            }
            //return string.Format("{0} {1}({2})", method.ReturnType.zName(), method.Name, s);
            return string.Format("{0} {1}({2})", method.ReturnType.zGetTypeName(), method.Name, s);
        }

        public static bool IsQualifiedTypeName(string typeName)
        {
            return typeName.Contains(",");
        }

        // typeName :
        //   full name               : "System.Int32"
        //   assembly qualified name : "System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        //                             "pb.IO.zPath, runsource.irunsource, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
        public static Type GetType(string typeName, ErrorOptions errorOption = ErrorOptions.None)
        {
            if (typeName == null)
                throw new PBException("type name is null");
            //return Type.GetType(type, throwOnError: true, ignoreCase: false);
            Type type = Type.GetType(typeName);
            if (type == null)
                Error.WriteMessage(errorOption, "type not found \"{0}\"", typeName);
            return type;
        }

        public static Type GetType(Assembly assembly, string typeName, ErrorOptions errorOption = ErrorOptions.None)
        {
            if (assembly == null)
                throw new PBException("assembly is null");
            if (typeName == null)
                throw new PBException("type name is null");
            Type type = assembly.GetType(typeName);
            if (type == null)
                Error.WriteMessage(errorOption, "type not found \"{0}\" in assembly \"{1}\"", typeName, assembly.Location);
            return type;
        }

        public static Type GetType(string assemblyQualifiedName, string typeName, ErrorOptions errorOption = ErrorOptions.None)
        {
            if (typeName == null)
                throw new PBException("type name is null");
            Assembly assembly = GetAssembly(assemblyQualifiedName, errorOption);
            if (assembly != null)
                return GetType(assembly, typeName, errorOption);
            else
                return null;
        }

        public static Assembly GetAssembly(string assemblyQualifiedName, ErrorOptions errorOption = ErrorOptions.None)
        {
            if (assemblyQualifiedName == null)
                throw new PBException("assembly qualified name is null");
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(assembly2 => assembly2.FullName == assemblyQualifiedName).FirstOrDefault();
            if (assembly == null)
                Error.WriteMessage(errorOption, "assembly not found \"{0}\"", assemblyQualifiedName);
            return assembly;
        }

        public static string GetAssemblyName(string assemblyQualifiedName)
        {
            // assemblyQualifiedName : "ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
            int i = assemblyQualifiedName.IndexOf(',');
            if (i != -1)
                return assemblyQualifiedName.Substring(0, i).TrimEnd();
            else
                return assemblyQualifiedName;
        }

        //public static MethodInfo GetMethod(string typeName, string methodName, ErrorOptions errorOption = ErrorOptions.None)
        //{
        //    if (methodName == null)
        //        return null;
        //    return GetMethod(GetType(typeName, errorOption), methodName, errorOption);
        //}

        //public static MethodInfo GetMethod(Assembly assembly, string typeName, string methodName, ErrorOptions errorOption = ErrorOptions.None)
        //{
        //    if (methodName == null)
        //        return null;
        //    return GetMethod(GetType(assembly, typeName, errorOption), methodName, errorOption);
        //}

        public static MethodInfo GetMethod(Type type, string methodName, ErrorOptions errorOption = ErrorOptions.None)
        {
            if (type == null || methodName == null)
                return null;
            MethodInfo method = type.GetMethod(methodName);
            if (method == null)
                Error.WriteMessage(errorOption, "method not found \"{0}\" type \"{1}\" in assembly \"{2}\"", methodName, type.zGetTypeName(), type.Assembly.Location);
            return method;
        }

        public static MethodElements GetMethodElements(string methodName)
        {
            // methodName : "Init" => MethodName = "Init", TypeName = null, QualifiedTypeName = null, AssemblyName = null
            // methodName : "Test._RunCode.Init" => MethodName = "Init", TypeName = "Test._RunCode", QualifiedTypeName = null, AssemblyName = null
            // methodName : "Test._RunCode.Init, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
            //   => MethodName = "Init", TypeName = "Test._RunCode", QualifiedTypeName = "Test._RunCode, ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", AssemblyName = "ebook.download, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
            string assemblyName = null;
            int i = methodName.IndexOf(',');
            if (i != -1)
            {
                assemblyName = methodName.Substring(i + 1).TrimStart();
                methodName = methodName.Substring(0, i).TrimEnd();
            }
            string typeName = null;
            i = methodName.LastIndexOf('.');
            if (i != -1)
            {
                typeName = methodName.Substring(0, i).TrimEnd();
                methodName = methodName.Substring(i + 1).TrimStart();
            }
            return new MethodElements { MethodName = methodName, TypeName = typeName, QualifiedTypeName = assemblyName != null && typeName != null ? typeName + ", " + assemblyName : null, AssemblyName = assemblyName };
        }

        //private static void Error(string errorMessage, bool throwError, bool traceError)
        //{
        //    if (throwError)
        //        throw new PBException(errorMessage);
        //    if (traceError)
        //        Trace.WriteLine(errorMessage);
        //}
    }

    public static partial class GlobalExtension
    {
        public static string zGetTypeName(this Type type)
        {
            return Reflection.GetName(type);
        }

        public static string zGetDefinition(this MethodInfo method)
        {
            return Reflection.GetDefinition(method);
        }
    }
}
