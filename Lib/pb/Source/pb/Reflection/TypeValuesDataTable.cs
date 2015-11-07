using System;
using System.Collections.Generic;
using System.Data;

namespace pb.Reflection
{
    public partial class TypeValues<T>
    {
        public DataTable ToDataTable(T data)
        {
            DataTable dt = CreateDataTable();
            AddToDataTable(dt, data);
            return dt;
        }

        public DataTable ToDataTable(IEnumerable<T> dataList)
        {
            DataTable dt = CreateDataTable();
            foreach (T data in dataList)
                AddToDataTable(dt, data);
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

        public void AddToDataTable(DataTable dt, T data)
        {
            SetData(data);
            DataRow row = dt.NewRow();
            foreach (MemberValue memberValue in _memberValues.Values)
            {
                if (memberValue.MemberAccess.IsValueType)
                {
                    //columns.Add(memberValue.MemberAccess.TreeName, memberValue.MemberAccess.ValueType);
                    GetValue(memberValue);
                    if (memberValue.Value != null)
                        row[memberValue.MemberAccess.TreeName] = memberValue.Value;
                }
            }
            dt.Rows.Add(row);
            while (NextValues())
            {
                foreach (MemberValue memberValue in _memberValues.Values)
                {
                }
            }
        }
    }
}
