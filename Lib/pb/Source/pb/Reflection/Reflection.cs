using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pb.Reflection
{
    public class MethodElements
    {
        public string MethodName;
        public string TypeName;
        public string QualifiedTypeName;
        public string AssemblyName;
    }

    public static class zReflection
    {
        public static string GetName(Type type)
        {
            if (type == null)
                return null;
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

        //public static IEnumerable<TreeValue<ValueInfo>> GetTypeAllValuesInfos_v1(Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public,
        //    Func<ValueInfo, TreeFilter> filter = null, bool verbose = false)
        //{
        //    if (type == null)
        //        yield break;

        //    int level = 1;
        //    int index = 1;

        //    ValueInfo valueInfo = new ValueInfo(type);

        //    TreeValue<ValueInfo> treeValue;
        //    TreeFilter treeFilter;

        //    if (verbose)
        //    {
        //        treeValue = new TreeValue<ValueInfo> { Index = index++, Value = valueInfo, Level = level, TreeOpe = TreeOpe.Source, Selected = false, Skip = false, Stop = false };

        //        treeFilter = TreeFilter.Select;
        //        if (filter != null)
        //            treeFilter = filter(valueInfo);

        //        if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
        //        {
        //            treeValue.Stop = true;
        //            yield return treeValue;
        //            yield break;
        //        }

        //        if ((treeFilter & TreeFilter.Skip) == TreeFilter.Skip)
        //        {
        //            treeValue.Skip = true;
        //            yield return treeValue;
        //            yield break;
        //        }

        //        if ((treeFilter & TreeFilter.DontSelect) == 0)
        //        {
        //            treeValue.Selected = true;
        //            yield return treeValue;
        //        }
        //        else
        //            yield return treeValue;
        //    }

        //    Stack<IEnumerator<ValueInfo>> stack = new Stack<IEnumerator<ValueInfo>>();
        //    IEnumerator<ValueInfo> enumerator = null;
        //    string treeName = null;
        //    while (true)
        //    {
        //        // get child
        //        while (true)
        //        {
        //            if (valueInfo.IsValueType)
        //                break;

        //            if (enumerator != null)
        //                stack.Push(enumerator);

        //            level++;
        //            enumerator = valueInfo.ValueType.zGetTypeValuesInfos(bindingFlags, MemberTypes.Field | MemberTypes.Property).GetEnumerator();
        //            if (!enumerator.MoveNext())
        //                break;

        //            treeName = valueInfo.TreeName;
        //            valueInfo = enumerator.Current;
        //            if (treeName != null)
        //            {
        //                valueInfo.TreeName = treeName + "." + valueInfo.TreeName;
        //                valueInfo.ParentName = treeName;
        //            }

        //            treeValue = new TreeValue<ValueInfo> { Index = index++, Value = valueInfo, Level = level, TreeOpe = TreeOpe.Child, Selected = false, Skip = false, Stop = false };

        //            treeFilter = TreeFilter.Select;
        //            if (filter != null)
        //                treeFilter = filter(valueInfo);

        //            if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
        //            {
        //                treeValue.Stop = true;
        //                yield return treeValue;
        //                yield break;
        //            }

        //            if ((treeFilter & TreeFilter.Skip) == TreeFilter.Skip)
        //            {
        //                treeValue.Skip = true;
        //                yield return treeValue;
        //                break;
        //            }

        //            if ((treeFilter & TreeFilter.DontSelect) == 0)
        //            {
        //                treeValue.Selected = true;
        //                yield return treeValue;
        //            }
        //            else
        //                yield return treeValue;
        //        }

        //        if (enumerator == null)
        //            break;

        //        // get next sibling node or next sibling node of parent node
        //        bool getChild = false;
        //        //bool stopNode = false;
        //        while (true)
        //        {
        //            // next sibling node
        //            while (true)
        //            {
        //                if (!enumerator.MoveNext())
        //                    break;

        //                valueInfo = enumerator.Current;
        //                if (treeName != null)
        //                {
        //                    valueInfo.TreeName = treeName + "." + valueInfo.TreeName;
        //                    valueInfo.ParentName = treeName;
        //                }
        //                treeValue = new TreeValue<ValueInfo> { Index = index++, Value = valueInfo, Level = level, TreeOpe = TreeOpe.Siblin, Selected = false, Skip = false, Stop = false };

        //                treeFilter = TreeFilter.Select;
        //                if (filter != null)
        //                    treeFilter = filter(valueInfo);

        //                if ((treeFilter & TreeFilter.Stop) == TreeFilter.Stop)
        //                {
        //                    treeValue.Stop = true;
        //                    yield return treeValue;
        //                    yield break;
        //                }

        //                if ((treeFilter & TreeFilter.Skip) == 0)
        //                {
        //                    if ((treeFilter & TreeFilter.DontSelect) == 0)
        //                    {
        //                        treeValue.Selected = true;
        //                        yield return treeValue;
        //                    }
        //                    else
        //                        yield return treeValue;
        //                    getChild = true;
        //                    break;
        //                }
        //                else
        //                {
        //                    treeValue.Skip = true;
        //                    yield return treeValue;
        //                }
        //            }
        //            if (getChild)
        //                break;

        //            level--;
        //            yield return new TreeValue<ValueInfo> { Index = index++, Value = null, Level = level, TreeOpe = TreeOpe.Parent, Selected = false, Skip = false, Stop = false };

        //            // parent node
        //            if (stack.Count == 0)
        //            {
        //                //stopNode = true;
        //                yield break;
        //            }

        //            enumerator = stack.Pop();
        //            treeName = enumerator.Current.ParentName;
        //            //yield return new TreeValue<ValueInfo> { Index = index++, Value = null, Level = level, TreeOpe = TreeOpe.Parent, Selected = false, Skip = false, Stop = false };
        //            //level--;
        //            //break;
        //        }
        //        //if (stopNode)
        //        //    break;
        //    }
        //}

        //private static void Error(string errorMessage, bool throwError, bool traceError)
        //{
        //    if (throwError)
        //        throw new PBException(errorMessage);
        //    if (traceError)
        //        Trace.WriteLine(errorMessage);
        //}

        public static Type GetNullableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return type.GetGenericArguments()[0];
            else
                return null;
        }

        public static Type GetEnumerableType(Type type)
        {
            foreach (Type intType in type.GetInterfaces())
            {
                if (intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return intType.GetGenericArguments()[0];
            }
            return null;
        }
    }

    public static partial class GlobalExtension
    {
        public static string zGetTypeName(this Type type)
        {
            return zReflection.GetName(type);
        }

        public static string zGetDefinition(this MethodInfo method)
        {
            return zReflection.GetDefinition(method);
        }

        public static Type zGetValueType(this MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).FieldType;
            else if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).PropertyType;
            else
                return null;
        }

        //public static ValueInfo zGetValueInfo(this MemberInfo memberInfo)
        //{
        //    Type valueType = memberInfo.zGetValueType();
        //    return new ValueInfo { Name = memberInfo.Name, ValueType = valueType, IsValueType = Reflection.IsValueType(valueType), DeclaringType = memberInfo.DeclaringType,
        //        ReflectedType = memberInfo.ReflectedType, MemberType = memberInfo.MemberType, MetadataToken = memberInfo.MetadataToken, Module = memberInfo.Module };
        //}


        //public static IEnumerable<TreeValue<ValueInfo>> zGetTypeAllValuesInfos_v1(this Type type, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public, Func<ValueInfo, TreeFilter> filter = null)
        //{
        //    return zReflection.GetTypeAllValuesInfos_v1(type, bindingFlags, filter);
        //}
    }
}
