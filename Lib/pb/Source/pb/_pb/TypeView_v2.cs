using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Xml.Linq;
using pb.Data.Xml;

namespace pb
{
    public class TypeView_v2
    {
        private object _variable = null;
        private Type _type = null;
        private int maxDepth = 10;
        //private bool _withList = false; // si true prend en compte les variables liste
        private List<NamedValue> _values = null;
        private Dictionary<string, NamedValue> _keyValues = null;

        public TypeView_v2(object var)
        {
            _variable = var;
        }

        public TypeView_v2(Type type)
        {
            _type = type;
        }

        public Dictionary<string, NamedValue> GetKeyValues()
        {
            if (_keyValues == null)
            {
                _keyValues = new Dictionary<string, NamedValue>();
                foreach (NamedValue value in GetValues())
                    _keyValues.Add(value.Name, value);
            }
            return _keyValues;
        }

        public IEnumerable<NamedValue> GetValues()
        {
            if (_values == null)
            {
                if (_variable != null)
                    GetValuesFromVariable();
                else if (_type != null)
                    GetValuesFromType();
                else
                    throw new PBException("no variable and no type defined");
            }
            return _values;
        }

        private void GetValuesFromVariable()
        {
            _values = new List<NamedValue>();
            GetValuesFromVariable(0, "", _variable, true);
        }

        private void GetValuesFromVariable(int depth, string name, object variable, bool path)
        {
            if (variable == null)
            {
                _values.Add(new NamedValue(name, variable));
                return;
            }
            if (depth++ > maxDepth)
                return;
            Type type = variable.GetType();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            if ((type.IsValueType && properties.Length == 0 && fields.Length == 0) || type.IsEnum || type == typeof(string) || type == typeof(DateTime) || type == typeof(pb.Date) || type == typeof(Bitmap))
                _values.Add(new NamedValue(name, variable));
            // désactivé le 03/03/2015 pour ne pas etre obligé d'ajouter FileReader au projet
            //else if (oVariable is pb.old.FileReader)
            //    GetValuesFromVariable(sName, oVariable as pb.old.FileReader);
            else if (variable is IEnumerable)
            {
                string name2;
                if (name == "")
                    name2 = "value"; else name2 = name + "_value";
                AddProperties(depth, name, variable, path, properties, true);
                AddFields(depth, name, variable, path, fields, true);
                GetValuesFromVariable(name2, variable as IEnumerable);
            }
            else if (variable is DataRow)
                GetValuesFromVariable(name, variable as DataRow);
            else if (variable is XElement)
                GetValuesFromVariable(name, variable as XElement, path);
            else if (variable is XText)
                GetValuesFromVariable(name, variable as XText, path);
            // modif le 18/09/2013 pour visualiser la class Test_02 de Test_xmlSerialize_f.cs
            //else if (iDepth > 1)
            //{
            //    gValues.Add(new NamedValue(sName, oVariable.ToString()));
            //}
            else
            {
                AddProperties(depth, name, variable, path, properties, false);
                AddFields(depth, name, variable, path, fields, false);
            }
        }

        private void GetValuesFromVariable(string name, IEnumerable list)
        {
            TypeView view = new TypeView();
            int i = 1;
            IEnumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string name2 = name + i++.ToString();
                view.Variable = enumerator.Current;
                foreach (NamedValue value in view.Values)
                {
                    if (value.Name == "")
                        value.Name = name2; else value.Name = name2 + "_" + value.Name;
                    _values.Add(value);
                }
            }
        }

        private void GetValuesFromVariable(string name, DataRow row)
        {
            if (name != "")
                name += "_";
            foreach (DataColumn col in row.Table.Columns)
            {
                _values.Add(new NamedValue(name + col.ColumnName, row[col]));
            }
        }

        private void GetValuesFromVariable(string name, XElement xe, bool path)
        {
            string s;
            if (path)
            {
                if (xe != null)
                    s = xe.zGetPath();
                else
                    s = null;
                _values.Add(new NamedValue(name + "_path", s));

                if (xe != null)
                    s = xe.Name.ToString();
                else
                    s = null;
                _values.Add(new NamedValue(name + "_node", s));

                name += "_value";
            }
            if (xe != null)
                s = xe.Value.Trim();
            else
                s = null;
            _values.Add(new NamedValue(name, s));
        }

        private void GetValuesFromVariable(string name, XText xt, bool path)
        {
            string s;
            if (path)
            {
                if (xt != null)
                    //s = xt.Parent.zGetPath();
                    s = xt.zGetPath();
                else
                    s = null;
                _values.Add(new NamedValue(name + "_path", s));

                if (xt != null)
                    s = xt.Parent.Name.ToString();
                else
                    s = null;
                _values.Add(new NamedValue(name + "_node", s));

                name += "_value";
            }
            if (xt != null)
                s = xt.Value.Trim();
            else
                s = null;
            _values.Add(new NamedValue(name, s));
        }

        private void AddProperties(int depth, string name, object variable, bool path, PropertyInfo[] properties, bool onlyVisible)
        {
            foreach (PropertyInfo property in properties)
            {
                if (onlyVisible && Attribute.GetCustomAttribute(property, typeof(VisibleAttribute)) == null)
                    continue;
                object value = null;
                try
                {
                    value = property.GetGetMethod().Invoke(variable, new object[0]);
                }
                catch (Exception ex)
                {
                    value = Error.GetErrorMessage(ex);
                }
                string name2;
                if (name == "")
                    name2 = property.Name;
                else
                    name2 = name + "_" + property.Name;
                GetValuesFromVariable(depth, name2, value, path);
                if (value is XElement)
                    path = false;
            }
        }

        private void AddFields(int depth, string name, object variable, bool path, FieldInfo[] fields, bool onlyVisible)
        {
            foreach (FieldInfo field in fields)
            {
                if (onlyVisible && Attribute.GetCustomAttribute(field, typeof(VisibleAttribute)) == null)
                    continue;
                object value = field.GetValue(variable);
                string name2;
                if (name == "")
                    name2 = field.Name;
                else
                    name2 = name + "_" + field.Name;
                GetValuesFromVariable(depth, name2, value, path);
                if (value is XElement)
                    path = false;
            }
        }

        private void GetValuesFromType()
        {
            if (_values != null) return;
            _values = new List<NamedValue>();
            FieldInfo[] fields = _type.GetFields();
            if (_type.IsValueType || _type == typeof(string))
            {
                _values.Add(new NamedValue("", _type));
            }
            else
            {
                PropertyInfo[] properties = _type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    _values.Add(new NamedValue(property.Name, property.PropertyType));
                }
                fields = _type.GetFields();
                foreach (FieldInfo field in fields)
                {
                    _values.Add(new NamedValue(field.Name, field.FieldType));
                }
            }
        }
    }
}
