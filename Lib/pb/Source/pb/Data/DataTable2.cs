using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using pb.Reflection;

namespace pb.Data
{
    //public static class ToDataTable<T>
    public static class ToDataTable
    {
        //public static Dictionary<string, Func<DataTable, T>> _toDatablesFunctions;

        public static DataTable DefaultToDataTable<T>(T v)
        {
            DataTable dt = new DataTable();
            if (v is IEnumerable)
            {
                IEnumerator enumerator = ((IEnumerable)v).GetEnumerator();
                while (enumerator.MoveNext())
                    zdt.AddRow(dt, enumerator.Current);
            }
            else
            {
                zdt.AddColumns(dt, "name, value");
                TypeView view = new TypeView(v);
                foreach (NamedValue value in view.Values)
                {
                    string sName = value.Name; if (sName == "") sName = "value";
                    //dt.Rows.Add(sName, value.Value);
                    DataRow row = dt.NewRow();
                    row["name"] = sName;
                    row["value"] = value.Value;
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }
    }

    public static partial class zdt
    {
        public static DataTable Create(string sFieldDef)
        {
            return Create(new FieldList(sFieldDef));
        }

        public static DataTable Create(FieldList fields)
        {
            DataTable dt = new DataTable();
            foreach (Field field in fields)
            {
                Type type = field.Type;
                if (type.IsValueType && type.IsGenericType)
                {
                    Type[] types = type.GetGenericArguments();
                    if (types.Length > 0) type = types[0];
                }
                //dt.Columns.Add(field.Name, type);
                DataColumn col = new DataColumn(field.Name, type);
                col.DefaultValue = field.DefaultValue;
                col.AllowDBNull = field.IsNullable;
                dt.Columns.Add(col);
            }
            return dt;
        }
    }

    public static partial class GlobalExtension
    {
        public static DataTable zTypeToDataTable<T>(this T v)
        {
            DataTable dt = zdt.Create("type, name1, name2, value");
            Type t;
            if (v != null)
                t = v.GetType();
            else
                t = typeof(T);
            dt.Rows.Add(t, null, null, null);
            dt.Rows.Add(null, "Assembly", null, t.Assembly.FullName);
            dt.Rows.Add(null, "AssemblyQualifiedName", null, t.AssemblyQualifiedName);
            dt.Rows.Add(null, "Attributes", null, t.Attributes);
            dt.Rows.Add(null, "BaseType", null, t.BaseType);
            dt.Rows.Add(null, "ContainsGenericParameters", null, t.ContainsGenericParameters);
            dt.Rows.Add(null, "DeclaringType", null, t.DeclaringType);
            dt.Rows.Add(null, "FullName", null, t.FullName);

            MemberInfo[] members = t.GetDefaultMembers();
            dt.Rows.Add(null, "GetDefaultMembers()", "nb", members.Length);
            foreach (MemberInfo member in members)
                dt.Rows.Add(null, "DefaultMember", member.Name, member.MemberType);

            dt.Rows.Add(null, "GetElementType()", null, t.GetElementType());

            EventInfo[] events = t.GetEvents();
            dt.Rows.Add(null, "GetEvents()", "nb", events.Length);
            foreach (EventInfo e in events)
                dt.Rows.Add(null, "Event", e.Name, e);

            FieldInfo[] fields = t.GetFields();
            dt.Rows.Add(null, "GetFields()", "nb", fields.Length);
            foreach (FieldInfo field in fields)
                dt.Rows.Add(null, "Field", field.Name, field.MemberType);

            Type[] types = t.GetGenericArguments();
            dt.Rows.Add(null, "GetGenericArguments()", "nb", types.Length);
            foreach (Type type in types)
                dt.Rows.Add(null, "GenericArgument", null, type.FullName);

            //dt.Rows.Add(null, "GetGenericTypeDefinition", null, t.GetGenericTypeDefinition());

            types = t.GetInterfaces();
            dt.Rows.Add(null, "GetInterfaces()", "nb", types.Length);
            foreach (Type type in types)
                dt.Rows.Add(null, "Interface", null, type.FullName);

            members = t.GetMembers();
            dt.Rows.Add(null, "GetMembers()", "nb", members.Length);
            foreach (MemberInfo member in members)
                dt.Rows.Add(null, "Member", member.Name, member.MemberType);

            MethodInfo[] methods = t.GetMethods();
            dt.Rows.Add(null, "GetMethods()", "nb", methods.Length);
            foreach (MethodInfo method in methods)
                //dt.Rows.Add(null, "Method", method.zDefinition());
                dt.Rows.Add(null, "Method", method.zGetDefinition());

            types = t.GetNestedTypes();
            dt.Rows.Add(null, "GetNestedTypes()", "nb", types.Length);
            foreach (Type type in types)
                dt.Rows.Add(null, "NestedType", null, type.FullName);

            PropertyInfo[] properties = t.GetProperties();
            dt.Rows.Add(null, "GetProperties()", "nb", properties.Length);
            foreach (PropertyInfo property in properties)
                dt.Rows.Add(null, "Property", property.Name, property.PropertyType);

            dt.Rows.Add(null, "GUID", null, t.GUID);
            dt.Rows.Add(null, "HasElementType", null, t.HasElementType);
            dt.Rows.Add(null, "IsAbstract", null, t.IsAbstract);
            dt.Rows.Add(null, "IsAnsiClass", null, t.IsAnsiClass);
            dt.Rows.Add(null, "IsArray", null, t.IsArray);
            dt.Rows.Add(null, "IsAutoClass", null, t.IsAutoClass);
            dt.Rows.Add(null, "IsAutoLayout", null, t.IsAutoLayout);
            dt.Rows.Add(null, "IsByRef", null, t.IsByRef);
            dt.Rows.Add(null, "IsClass", null, t.IsClass);
            dt.Rows.Add(null, "IsCOMObject", null, t.IsCOMObject);
            dt.Rows.Add(null, "IsContextful", null, t.IsContextful);
            dt.Rows.Add(null, "IsEnum", null, t.IsEnum);
            dt.Rows.Add(null, "IsExplicitLayout", null, t.IsExplicitLayout);
            dt.Rows.Add(null, "IsGenericType", null, t.IsGenericType);
            dt.Rows.Add(null, "IsGenericTypeDefinition", null, t.IsGenericTypeDefinition);
            dt.Rows.Add(null, "IsImport", null, t.IsImport);
            dt.Rows.Add(null, "IsInterface", null, t.IsInterface);
            dt.Rows.Add(null, "IsLayoutSequential", null, t.IsLayoutSequential);
            dt.Rows.Add(null, "IsMarshalByRef", null, t.IsMarshalByRef);
            dt.Rows.Add(null, "IsNested", null, t.IsNested);
            dt.Rows.Add(null, "IsNestedAssembly", null, t.IsNestedAssembly);
            dt.Rows.Add(null, "IsNestedFamANDAssem", null, t.IsNestedFamANDAssem);
            dt.Rows.Add(null, "IsNestedFamily", null, t.IsNestedFamily);
            dt.Rows.Add(null, "IsNestedFamORAssem", null, t.IsNestedFamORAssem);
            dt.Rows.Add(null, "IsNestedPrivate", null, t.IsNestedPrivate);
            dt.Rows.Add(null, "IsNestedPublic", null, t.IsNestedPublic);
            dt.Rows.Add(null, "IsNotPublic", null, t.IsNotPublic);
            dt.Rows.Add(null, "IsPointer", null, t.IsPointer);
            dt.Rows.Add(null, "IsPrimitive", null, t.IsPrimitive);
            dt.Rows.Add(null, "IsPublic", null, t.IsPublic);
            dt.Rows.Add(null, "IsSealed", null, t.IsSealed);
            dt.Rows.Add(null, "IsSerializable", null, t.IsSerializable);
            dt.Rows.Add(null, "IsSpecialName", null, t.IsSpecialName);
            dt.Rows.Add(null, "IsUnicodeClass", null, t.IsUnicodeClass);
            dt.Rows.Add(null, "IsValueType", null, t.IsValueType);
            dt.Rows.Add(null, "IsVisible", null, t.IsVisible);
            dt.Rows.Add(null, "MemberType", null, t.MemberType);
            dt.Rows.Add(null, "MetadataToken", null, t.MetadataToken);
            dt.Rows.Add(null, "Module", null, t.Module);
            dt.Rows.Add(null, "Name", null, t.Name);
            dt.Rows.Add(null, "Namespace", null, t.Namespace);
            dt.Rows.Add(null, "ReflectedType", null, t.ReflectedType);
            dt.Rows.Add(null, "StructLayoutAttribute", null, t.StructLayoutAttribute);
            dt.Rows.Add(null, "TypeHandle", null, t.TypeHandle);
            dt.Rows.Add(null, "TypeInitializer", null, t.TypeInitializer);
            dt.Rows.Add(null, "UnderlyingSystemType", null, t.UnderlyingSystemType);

            dt.Rows.Add(null, "IsGenericParameter", null, t.IsGenericParameter);
            if (t.IsGenericParameter)
            {
                dt.Rows.Add(null, "GenericParameter", "DeclaringMethod", t.DeclaringMethod);
                dt.Rows.Add(null, "GenericParameter", "GenericParameterAttributes", t.GenericParameterAttributes);
                types = t.GetGenericParameterConstraints();
            }

            return dt;
        }
    }
}
