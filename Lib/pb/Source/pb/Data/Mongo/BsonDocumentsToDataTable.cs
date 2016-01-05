using System;
using System.Collections.Generic;
using System.Data;
using MongoDB.Bson;

namespace pb.Data.Mongo
{
    // BsonArrayEnumerator IEnumerator<BsonValue>, IEnumerable<BsonValue>
    //public class BsonArrayToDataTable
    //{
    //    private string _name;
    //    private IEnumerator<BsonValue> _array;

    //    public BsonArrayToDataTable(string name, BsonArray array)
    //    {
    //        _name = name;
    //        _array = array.GetEnumerator();
    //    }
    //}

    public class BsonDocumentsToDataTable
    {
        private IEnumerable<BsonDocument> _documents;
        private DataTable _dt;
        private DataRow _row;

        public static DataTable ToDataTable(IEnumerable<BsonDocument> documents)
        {
            BsonDocumentsToDataTable documentsToDataTable = new BsonDocumentsToDataTable(documents);
            return documentsToDataTable.ToDataTable();
        }

        public static DataTable ToDataTable(BsonDocument document)
        {
            BsonDocumentsToDataTable documentsToDataTable = new BsonDocumentsToDataTable(new BsonDocument[] { document });
            return documentsToDataTable.ToDataTable();
        }

        public BsonDocumentsToDataTable(IEnumerable<BsonDocument> documents)
        {
            _documents = documents;
        }

        public DataTable ToDataTable()
        {
            _dt = new DataTable();

            foreach (BsonDocument document in _documents)
            {
                _row = _dt.NewRow();
                _dt.Rows.Add(_row);
                //AddToDataTable(null, document);
                //AddArraysToDataTable();
            }
            return _dt;
        }
    }

    public class BsonDocumentsToDataTable_v2
    {
        // generate a DataTable from a list of BsonDocument
        // each document and nested document fill one row in DataTable with values in column
        // if document contain array there is one row by array value, same for nested array

        //private IEnumerable<BsonDocument> _documents;
        private IEnumerable<BsonValue> _values;
        private DataTable _dt;
        private int _lastColumnIndex;
        private DataRow _row;
        private List<BsonArrayToDataTable> _arrays = new List<BsonArrayToDataTable>();

        //public static DataTable ToDataTable(IEnumerable<BsonDocument> documents)
        public static DataTable ToDataTable(IEnumerable<BsonValue> values, DataTable dt = null)
        {
            //BsonDocumentsToDataTable_v2 documentsToDataTable = new BsonDocumentsToDataTable_v2(documents);
            BsonDocumentsToDataTable_v2 documentsToDataTable = new BsonDocumentsToDataTable_v2(values);
            if (dt == null)
                dt = new DataTable();
            documentsToDataTable._dt = dt;
            documentsToDataTable.ToDataTable();
            return dt;
        }

        //public static DataTable ToDataTable(BsonDocument document)
        public static DataTable ToDataTable(BsonValue value, DataTable dt = null)
        {
            //BsonDocumentsToDataTable_v2 documentsToDataTable = new BsonDocumentsToDataTable_v2(new BsonDocument[] { document });
            BsonDocumentsToDataTable_v2 documentsToDataTable = new BsonDocumentsToDataTable_v2(new BsonValue[] { value });
            if (dt == null)
                dt = new DataTable();
            documentsToDataTable._dt = dt;
            documentsToDataTable.ToDataTable();
            return dt;
        }

        //public BsonDocumentsToDataTable_v2(IEnumerable<BsonDocument> documents)
        private BsonDocumentsToDataTable_v2(IEnumerable<BsonValue> values)
        {
            //_documents = documents;
            _values = values;
        }

        private void ToDataTable()
        {
            // from http://www.programingqa.com/post/Converting-MongoDB-query-result-to-C-ADONET-DataTable

            //_dt = new DataTable();

            //foreach (BsonDocument document in _documents)
            foreach (BsonValue value in _values)
            {
                _row = _dt.NewRow();
                _dt.Rows.Add(_row);
                _lastColumnIndex = -1;
                //AddToDataTable(null, document);
                AddToDataTable(null, value);
                AddArraysToDataTable();
            }
            //return _dt;
        }

        private void AddToDataTable(string name, BsonDocument document)
        {
            foreach (string key in document.Names)
            {
                if (key == "_t")
                    continue;
                AddToDataTable(name == null ? key : name + "." + key, document[key]);
            }
        }

        private void AddToDataTable(string name, BsonValue value)
        {
            if (value is BsonDocument)
            {
                AddToDataTable(name, (BsonDocument)value);
            }
            else if (value is BsonArray)
            {
                BsonArrayToDataTable arrayToDataTable = new BsonArrayToDataTable(this, name, (BsonArray)value);
                _arrays.Add(arrayToDataTable);
            }
            else
            {
                //string textValue;
                object dataValue;
                if (value is BsonTimestamp)
                {
                    dataValue = value.AsBsonTimestamp.ToLocalTime().ToString("s");
                }
                else if (value is BsonNull)
                {
                    //textValue = string.Empty;
                    dataValue = DBNull.Value;
                }
                else
                {
                    dataValue = value.ToString();
                }

                if (name == null)
                    name = "value";
                if (!_dt.Columns.Contains(name))
                {
                    DataColumn column = _dt.Columns.Add(name);
                    column.SetOrdinal(++_lastColumnIndex);
                }
                else
                    _lastColumnIndex = _dt.Columns[name].Ordinal;
                //if (name == null)
                //    name = "value";
                _row[name] = dataValue;
            }
        }

        private void AddArraysToDataTable()
        {
            object[] rowValues = _row.ItemArray;
            bool first = true;
            while (true)
            {
                bool findValue = false;

                for (int i = 0; i < _arrays.Count; )
                {
                    BsonArrayToDataTable arrayToDataTable = _arrays[i];
                    //if (arrayToDataTable.values.MoveNext())
                    //if (MoveNext(arrayToDataTable.values))
                    if (arrayToDataTable.AddNextValueToDataTable())
                    {
                        findValue = true;
                        //AddToDataTable(arrayToDataTable.values.Current, arrayToDataTable.name);
                        i++;
                    }
                    else
                        _arrays.RemoveAt(i);
                }

                if (!findValue)
                    break;

                if (!first)
                    _dt.Rows.Add(_row);
                first = false;

                _row = _dt.NewRow();
                _row.ItemArray = rowValues;
            }
        }

        //private bool MoveNext(IEnumerator<BsonValue> values)
        //{
        //    while (values.MoveNext())
        //    {
        //        //if (values.Current.GetType() == typeof(BsonNull))
        //        //    continue;
        //        return true;
        //    }
        //    return false;
        //}

        private class BsonArrayToDataTable
        {
            private BsonDocumentsToDataTable_v2 _parent;
            private string _name;
            private Stack<IEnumerator<BsonValue>> _arrayStack = new Stack<IEnumerator<BsonValue>>();

            public BsonArrayToDataTable(BsonDocumentsToDataTable_v2 parent, string name, BsonArray array)
            {
                _parent = parent;
                _name = name;
                _arrayStack.Push(array.GetEnumerator());
            }

            public bool AddNextValueToDataTable()
            {
                while (_arrayStack.Count > 0)
                {
                    while (_arrayStack.Peek().MoveNext())
                    {
                        BsonValue value = _arrayStack.Peek().Current;
                        if (value is BsonNull)
                            continue;
                        if (value is BsonArray)
                        {
                            _arrayStack.Push(((BsonArray)value).GetEnumerator());
                            continue;
                        }

                        _parent.AddToDataTable(_name, value);

                        if (value is BsonDocument)  // document values are added to columns, we continue to find a value
                            continue;

                        return true;
                    }
                    _arrayStack.Pop();
                }
                return false;
            }
        }

    }

    public class BsonDocumentsToDataTable_v1
    {
        private IEnumerable<BsonDocument> _documents;
        private DataTable _dt;
        private DataRow _row;

        public static DataTable ToDataTable(IEnumerable<BsonDocument> documents)
        {
            BsonDocumentsToDataTable_v1 documentsToDataTable = new BsonDocumentsToDataTable_v1(documents);
            return documentsToDataTable.ToDataTable();
        }

        public BsonDocumentsToDataTable_v1(IEnumerable<BsonDocument> documents)
        {
            _documents = documents;
        }

        public DataTable ToDataTable()
        {
            // from http://www.programingqa.com/post/Converting-MongoDB-query-result-to-C-ADONET-DataTable

            _dt = new DataTable();

            foreach (BsonDocument document in _documents)
            {
                _row = _dt.NewRow();
                AddToDataTable(document, null);
                _dt.Rows.Add(_row);
            }
            return _dt;
        }

        private void AddToDataTable(BsonDocument document, string parent)
        {
            // from http://www.programingqa.com/post/Converting-MongoDB-query-result-to-C-ADONET-DataTable
            // arrays means 1:M relation to parent, meaning we will have to fake multi levels by adding 1 more row foreach item in array.
            // i created the here because i want to add all new array rows after our main row.
            List<KeyValuePair<string, BsonArray>> arrays = new List<KeyValuePair<string, BsonArray>>();

            //DataTable dt = dr.Table;

            foreach (string key in document.Names) // this will loop thru all our json attributes.
            {
                //object value = document[key];
                BsonValue value = document[key]; // get the value of the current json attribute.

                string x; // for my specific needs, i need all values to be save in datatable as strings. you can implument to match your needs.

                // if our attribute is BsonDocument, means relation is 1:1. we can add values to current datarow and call the data column "parent.current".
                // we will use this recursive method to run thru all the child document.
                if (value is BsonDocument)
                {
                    string newParent = string.IsNullOrEmpty(parent) ? key : parent + "." + key;
                    AddToDataTable((BsonDocument)value, newParent);
                }
                // if our attribute is BsonArray, means relation is 1:N. we will need to add new rows, but not now.
                // we will save it in queue for later use.
                else if (value is BsonArray)
                {
                    // Save array to queue for later loop.
                    arrays.Add(new KeyValuePair<string, BsonArray>(key, (BsonArray)value));
                }
                // if our attribute is datatime i needed it in a spesific string format.
                else if (value is BsonTimestamp)
                {
                    //x = document[key].AsBsonTimestamp.ToLocalTime().ToString("s");
                    x = value.AsBsonTimestamp.ToLocalTime().ToString("s");
                }
                // if our attribute is null, i needed it converted to string.empty.
                else if (value is BsonNull)
                {
                    x = string.Empty;
                }
                else
                {
                    // for all other cases, just .ToString() it.
                    x = value.ToString();

                    // Make sure our datatable already contains column with the right name. if not - add it.
                    string colName = string.IsNullOrEmpty(parent) ? key : parent + "." + key;
                    if (!_dt.Columns.Contains(colName))
                        _dt.Columns.Add(colName);

                    // Add the value to the datarow in the right column.
                    _row[colName] = value;
                }
            }

            DataRow lastRow = _row;

            // loop thru all arrays when finish with standart fields.
            foreach (KeyValuePair<string, BsonArray> array in arrays)
            {
                // create column name that contains the parent name + child name.
                string newParent = string.IsNullOrEmpty(parent) ? array.Key : parent + "." + array.Key;

                // save the old - we will need it so we can add it existing values to the new row.
                //DataRow lastRow = _row;

                // loop thru all the BsonDocuments in the array
                foreach (BsonDocument document2 in array.Value)
                {
                    // Create new datarow for each item in array.
                    _row = _dt.NewRow();
                    _row.ItemArray = lastRow.ItemArray; // this will copy all the main row values to the new row - might not be needed for your use.
                    _dt.Rows.Add(_row); // the the new row to the datatable
                    AddToDataTable(document2, newParent); // fill the new datarow withh all the values for the BsonDocument in the array.
                }
                //_row = lastRow; // set the main data row back so we can use it values again.
            }

            _row = lastRow; // set the main data row back so we can use it values again.
        }
    }

    public static partial class GlobalExtension
    {
        //public static DataTable zToDataTable2(this IEnumerable<BsonDocument> documents)
        //{
        //    return BsonDocumentsToDataTable_v2.ToDataTable(documents);
        //}

        public static DataTable zToDataTable2(this IEnumerable<BsonValue> values, DataTable dt = null)
        {
            return BsonDocumentsToDataTable_v2.ToDataTable(values, dt);
        }

        //public static DataTable zToDataTable2(this BsonDocument document)
        //{
        //    return BsonDocumentsToDataTable_v2.ToDataTable(document);
        //}

        public static DataTable zToDataTable2(this BsonValue value, DataTable dt = null)
        {
            return BsonDocumentsToDataTable_v2.ToDataTable(value, dt);
        }

        public static DataTable zToDataTable2_old(this IEnumerable<BsonDocument> documents)
        {
            return BsonDocumentsToDataTable_v1.ToDataTable(documents);
        }
    }
}
