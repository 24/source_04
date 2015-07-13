using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using MongoDB.Bson;
using pb.Data.Mongo;
using pb.Data.Xml;

namespace pb.Data
{
    public static partial class zdt
    {
        public static DataRow AddRow(DataTable dt, object v)
        {
            DataRow row = dt.NewRow();
            AddToRow(row, v);
            dt.Rows.Add(row);
            return row;
        }

        public static void AddToRow(DataRow row, object v)
        {
            TypeView view = new TypeView(v);
            DataColumnCollection columns = row.Table.Columns;
            foreach (NamedValue value in view.Values)
            {
                string sName = value.Name; if (sName == "") sName = "value";
                int iCol = columns.IndexOf(sName);
                if (iCol == -1)
                {
                    Type type = typeof(string);
                    if (value.Value != null)
                    {
                        Type type2 = value.Value.GetType();
                        if (type2 == typeof(Bitmap))
                            type = type2;
                    }
                    DataColumn col = columns.Add(sName, type);
                    iCol = col.Ordinal;
                }
                row[iCol] = value.Value;
            }
        }

        public static void AddColumns(DataTable dt, string fieldDef)
        {
            AddColumns(dt, new FieldList(fieldDef));
        }

        public static void AddColumns(DataTable dt, FieldList fields)
        {
            foreach (Field field in fields)
            {
                if (dt.Columns.IndexOf(field.Name) != -1)
                    continue;
                Type type = field.Type;
                if (type.IsValueType && type.IsGenericType)
                {
                    Type[] types = type.GetGenericArguments();
                    if (types.Length > 0) type = types[0];
                }
                DataColumn col = new DataColumn(field.Name, type);
                col.DefaultValue = field.DefaultValue;
                col.AllowDBNull = field.IsNullable;
                dt.Columns.Add(col);
            }
        }

        public static string GetNewColumnName(DataTable dt, string columnName)
        {
            string columnName2 = columnName;
            int i = 1;
            while (dt.Columns.Contains(columnName2))
                columnName2 = columnName + i++.ToString();
            return columnName2;
        }

        public static string ToString(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            int rowNumber = 1;
            foreach (DataRow row in dt.Rows)
            {
                sb.AppendFormat("  row {0}", rowNumber++);
                sb.AppendLine();
                foreach (DataColumn column in dt.Columns)
                {
                    sb.AppendFormat("    {0} : {1}", column.ColumnName, row[column.ColumnName]);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    }

    public static partial class GlobalExtension
    {
        public static DataTable zToDataTable<T>(this T v)
        {
            if (v is BsonDocument)
                return (v as BsonDocument).zToDataTable2();
            //else if (v is IEnumerable<BsonDocument>)
            //    return (v as IEnumerable<BsonDocument>).zToDataTable2();
            else if (v is IEnumerable<BsonValue>)
                return (v as IEnumerable<BsonValue>).zToDataTable2();
            else
                return v.zToDataTable(new DataTable());
        }

        public static DataTable zToDataTable<T>(this T v, DataTable dt)
        {
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

        public static DataTable zToDataTable<T>(this IEnumerable<T> q)
        {
            DataTable dt = new DataTable();
            q.zToDataTable(dt);
            return dt;
        }

        public static DataTable zToDataTable<T>(this IEnumerable<T> q, DataTable dt)
        {
            foreach (T v in q)
                zdt.AddRow(dt, v);
            return dt;
        }

        public static DataTable zToDataTable(this IEnumerable<XNode> list)
        {
            return list.zToDataTable(new DataTable());
        }

        public static DataTable zToDataTable(this IEnumerable<XNode> list, DataTable dt)
        {
            zdt.AddColumns(dt, "path, node, type, value");
            foreach (var n in list)
            {
                DataRow row = dt.NewRow();
                if (n is XElement)
                {
                    XElement e = n as XElement;
                    //dt.Rows.Add(e.zGetPath(), e.Name, n.GetType(), e.Value);
                    row["path"] = e.zGetPath();
                    row["node"] = e.Name;
                    row["type"] = n.GetType();
                    row["value"] = e.Value;
                }
                else if (n is XText)
                {
                    XText t = n as XText;
                    //dt.Rows.Add(t.Parent.zGetPath(), null, n.GetType(), t.Value);
                    row["path"] = t.Parent.zGetPath();
                    row["type"] = n.GetType();
                    row["value"] = t.Value;
                }
                else
                {
                    //dt.Rows.Add(n.Parent.zGetPath(), null, n.GetType(), null);
                    row["path"] = n.Parent.zGetPath();
                    row["type"] = n.GetType();
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable zToDataTable(this IEnumerable<XElement> list)
        {
            return list.zToDataTable(new DataTable());
        }

        public static DataTable zToDataTable(this IEnumerable<XElement> list, DataTable dt)
        {
            zdt.AddColumns(dt, "path, node, value");
            foreach (var e in list)
            {
                DataRow row = dt.NewRow();
                row["path"] = e.zGetPath();
                row["node"] = e.Name;
                row["value"] = e.Value;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable zToDataTable(this IEnumerable<XAttribute> list)
        {
            return list.zToDataTable(new DataTable());
        }

        public static DataTable zToDataTable(this IEnumerable<XAttribute> list, DataTable dt)
        {
            zdt.AddColumns(dt, "path, node, value");
            foreach (var e in list)
            {
                DataRow row = dt.NewRow();
                row["path"] = e.zGetPath();
                row["node"] = e.Name;
                row["value"] = e.Value;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static string zToString(this DataTable dt)
        {
            return zdt.ToString(dt);
        }
    }
}
