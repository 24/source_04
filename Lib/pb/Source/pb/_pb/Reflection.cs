using System;
using System.Linq;
using System.Reflection;

namespace pb
{
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

        public static Type GetTypeFromName(string type)
        {
            if (!type.Contains('.'))
                type += "System.";
            return Type.GetType(type, true, false);
        }
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
