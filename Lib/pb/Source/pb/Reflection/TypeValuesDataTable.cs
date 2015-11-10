using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace pb.Reflection
{
    public partial class TypeValues<T>
    {
        public DataTable ToDataTable(T data, bool onlyNextValue = false)
        {
            DataTable dt = CreateDataTable();
            AddToDataTable(dt, data, onlyNextValue);
            return dt;
        }

        public DataTable ToDataTable(IEnumerable<T> dataList, bool onlyNextValue = false)
        {
            DataTable dt = CreateDataTable();
            foreach (T data in dataList)
                AddToDataTable(dt, data, onlyNextValue);
            return dt;
        }

        private DataTable CreateDataTable()
        {
            DataTable dt = new DataTable();
            DataColumnCollection columns = dt.Columns;
            foreach (TypeValue typeValue in GetTypeValues())
            {
                if (typeValue.IsValueType)
                    columns.Add(typeValue.TreeName, typeValue.ValueType);
            }
            return dt;
        }

        private void AddToDataTable(DataTable dt, T data, bool onlyNextValue = false)
        {
            SetData(data);
            DataRow row = dt.NewRow();
            foreach (TypeValue typeValue in GetValues())
            {
                if (typeValue.IsValueType)
                {
                    if (typeValue.Value != null)
                        row[typeValue.TreeName] = typeValue.Value;
                }
            }
            dt.Rows.Add(row);
            while (NextValues())
            {
                row = dt.NewRow();
                foreach (TypeValue typeValue in GetValues(onlyNextValue))
                {
                    if (typeValue.IsValueType)
                    {
                        if (typeValue.Value != null)
                            row[typeValue.TreeName] = typeValue.Value;
                    }
                }
                dt.Rows.Add(row);
            }
        }
    }

    public static partial class GlobalExtension
    {
        public static DataTable zToDataTable_v2<T>(this T value)
        {
            if (value is IEnumerable)
            {
                Type enumerableType = zReflection.GetEnumerableType(value.GetType());
                Type type = typeof(TypeValues<>).MakeGenericType(enumerableType);
                object typeValues = Activator.CreateInstance(type);

                //public void AddAllValues(MemberType memberType = MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property)
                MethodInfo methodInfo = type.GetRuntimeMethod("AddAllValues", new Type[] { typeof(MemberType) });
                if (methodInfo == null)
                    throw new PBException("method TypeValues<>.AddAllValues(MemberType) not found");
                methodInfo.Invoke(typeValues, new object[] { MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property });

                //public DataTable ToDataTable(IEnumerable<T> dataList, bool onlyNextValue = false)
                //methodInfo = type.GetRuntimeMethod("ToDataTable", new Type[] { typeof(IEnumerable<>), typeof(bool) });
                methodInfo = type.GetRuntimeMethod("ToDataTable", new Type[] { typeof(IEnumerable<>).MakeGenericType(enumerableType), typeof(bool) });
                if (methodInfo == null)
                    throw new PBException("method TypeValues<>.ToDataTable(IEnumerable<>, bool) not found");
                return (DataTable)methodInfo.Invoke(typeValues, new object[] { value, true  });
            }
            else
            {
                TypeValues<T> typeValues = new TypeValues<T>();
                typeValues.AddAllValues(MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property);
                return typeValues.ToDataTable(value, onlyNextValue: true);
            }
        }

        //public static void zTest<T>(this IEnumerable<T> enumerable)
        //{
        //    Trace.WriteLine("zTest<T> IEnumerable type {0} of {1}", enumerable.GetType().zGetTypeName(), zReflection.GetEnumerableType(enumerable.GetType()).zGetTypeName());
        //}

        //public static void zTest<T>(this T value)
        //{
        //    Trace.WriteLine("zTest<T> type {0}", value.GetType().zGetTypeName());
        //}
    }
}
