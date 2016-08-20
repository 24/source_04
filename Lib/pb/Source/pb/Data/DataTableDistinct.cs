using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

// How to dynamically create generic C# object using reflection? http://stackoverflow.com/questions/1151464/how-to-dynamically-create-generic-c-sharp-object-using-reflection

namespace pb.Data
{
    public class DataTableDistinct
    {
        private DataTable _dt;
        Dictionary<string, IDictionary> _values = new Dictionary<string, IDictionary>();

        public DataTableDistinct(DataTable dt)
        {
            _dt = dt;
        }

        private void CreateDistinctValues()
        {
            foreach (string name in _values.Keys)
            {
                DataColumn column = _dt.Columns[name];
                IDictionary dictionary = _values[name];
                foreach (DataRow row in _dt.Rows)
                {
                    object value = row[column];
                    dictionary[value] = value;
                }
            }
        }

        private DataTable CreateDistinctValuesDataTable()
        {
            DataTable dt = new DataTable();
            foreach (string name in _values.Keys)
            {
                DataColumn column = _dt.Columns[name];
                IDictionary dictionary = _values[name];
                dt.Columns.Add(column.ColumnName, column.DataType);
                DataColumn column2 = dt.Columns[name];
                IEnumerator rows = dt.Rows.GetEnumerator();
                foreach (object value in dictionary.Values)
                {
                    DataRow row;
                    if (rows != null && rows.MoveNext())
                    {
                        row = (DataRow)rows.Current;
                    }
                    else
                    {
                        rows = null;
                        row = dt.NewRow();
                        dt.Rows.Add(row);
                    }
                    row[column2] = value;
                }
            }
            return dt;
        }

        private void SelectColumns(IEnumerable<string> names)
        {
            foreach (string name in names)
            {
                if (!_dt.Columns.Contains(name))
                    throw new PBException($"unknow column {name}");
                Type type = _dt.Columns[name].DataType;
                IDictionary dictionary = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(type, type));
                _values.Add(name, dictionary);
            }
        }

        public static DataTable GetDistinctValues(DataTable dt, IEnumerable<string> names)
        {
            DataTableDistinct dtDistinct = new DataTableDistinct(dt);
            dtDistinct.SelectColumns(names);
            dtDistinct.CreateDistinctValues();
            return dtDistinct.CreateDistinctValuesDataTable();
        }
    }
}
