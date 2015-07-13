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
    public class VisibleAttribute : Attribute
    {
    }

    public class TypeView
    {
        private object _variable = null;
        private Type _type = null;
        private bool _withList = false; // si true prend en compte les variables liste
        private List<NamedValue> _values = null;

        public TypeView()
        {
        }

        public TypeView(object var)
        {
            _variable = var;
        }

        public TypeView(object var, bool bWithList)
        {
            _variable = var;
            _withList = bWithList;
        }

        public TypeView(Type type)
        {
            _type = type;
        }

        public object Variable
        {
            get { return _variable; }
            set
            {
                _values = null;
                _type = null;
                _variable = value;
            }
        }

        public Type type
        {
            get { return _type; }
            set
            {
                _values = null;
                _variable = null;
                _type = value;
            }
        }

        public bool WithList
        {
            get { return _withList; }
            set { _withList = value; }
        }

        public List<NamedValue> Values
        {
            get
            {
                GetValues();
                return _values;
            }
        }

        private void GetValues()
        {
            if (_variable != null)
                GetValuesFromVariable();
            else if (_type != null)
                GetValuesFromType();
            else
                _values = new List<NamedValue>();
        }

        private void GetValuesFromVariable()
        {
            if (_values != null) return;
            _values = new List<NamedValue>();
            GetValuesFromVariable(0, "", _variable, true);
        }

        private void GetValuesFromVariable(int iDepth, string sName, object oVariable, bool bPath)
        {
            if (oVariable == null)
            {
                _values.Add(new NamedValue(sName, oVariable));
                return;
            }
            if (iDepth++ > 10) return;
            Type type = oVariable.GetType();

            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            if ((type.IsValueType && properties.Length == 0 && fields.Length == 0) || type.IsEnum || type == typeof(string) || type == typeof(DateTime) || type == typeof(pb.Date) || type == typeof(Bitmap))
                _values.Add(new NamedValue(sName, oVariable));
            // désactivé le 03/03/2015 pour ne pas etre obligé d'ajouter FileReader au projet
            //else if (oVariable is pb.old.FileReader)
            //    GetValuesFromVariable(sName, oVariable as pb.old.FileReader);
            else if (oVariable is IEnumerable)
            {
                string sName2;
                if (sName == "") sName2 = "value"; else sName2 = sName + "_value";
                AddProperties(iDepth, sName, oVariable, bPath, properties, true);
                AddFields(iDepth, sName, oVariable, bPath, fields, true);
                GetValuesFromVariable(sName2, oVariable as IEnumerable);
            }
            else if (oVariable is DataRow)
                GetValuesFromVariable(sName, oVariable as DataRow);
            else if (oVariable is XElement)
                GetValuesFromVariable(sName, oVariable as XElement, bPath);
            else if (oVariable is XText)
                GetValuesFromVariable(sName, oVariable as XText, bPath);
            // modif le 18/09/2013 pour visualiser la class Test_02 de Test_xmlSerialize_f.cs
            //else if (iDepth > 1)
            //{
            //    gValues.Add(new NamedValue(sName, oVariable.ToString()));
            //}
            else
            {
                //foreach (PropertyInfo property in properties)
                //{
                //    object o = null;
                //    try
                //    {
                //        o = property.GetGetMethod().Invoke(oVariable, new object[0]);
                //    }
                //    catch (Exception ex)
                //    {
                //        o = cError.GetErrorMessage(ex);
                //    }
                //    string sName2;
                //    if (sName == "") sName2 = property.Name; else sName2 = sName + "_" + property.Name;
                //    GetValuesFromVariable(iDepth, sName2, o, bPath);
                //    if (o is XElement) bPath = false;
                //}
                AddProperties(iDepth, sName, oVariable, bPath, properties, false);
                //foreach (FieldInfo field in fields)
                //{
                //    object o = field.GetValue(oVariable);
                //    string sName2;
                //    if (sName == "") sName2 = field.Name; else sName2 = sName + "_" + field.Name;
                //    GetValuesFromVariable(iDepth, sName2, o, bPath);
                //    if (o is XElement) bPath = false;
                //}
                AddFields(iDepth, sName, oVariable, bPath, fields, false);
            }
        }

        private void AddProperties(int iDepth, string sName, object oVariable, bool bPath, PropertyInfo[] properties, bool onlyVisible)
        {
            foreach (PropertyInfo property in properties)
            {
                if (onlyVisible && Attribute.GetCustomAttribute(property, typeof(VisibleAttribute)) == null)
                    continue;
                object o = null;
                try
                {
                    o = property.GetGetMethod().Invoke(oVariable, new object[0]);
                }
                catch (Exception ex)
                {
                    o = Error.GetErrorMessage(ex);
                }
                string sName2;
                if (sName == "") sName2 = property.Name; else sName2 = sName + "_" + property.Name;
                GetValuesFromVariable(iDepth, sName2, o, bPath);
                if (o is XElement) bPath = false;
            }
        }

        private void AddFields(int iDepth, string sName, object oVariable, bool bPath, FieldInfo[] fields, bool onlyVisible)
        {
            foreach (FieldInfo field in fields)
            {
                if (onlyVisible && Attribute.GetCustomAttribute(field, typeof(VisibleAttribute)) == null)
                    continue;
                object o = field.GetValue(oVariable);
                string sName2;
                if (sName == "") sName2 = field.Name; else sName2 = sName + "_" + field.Name;
                GetValuesFromVariable(iDepth, sName2, o, bPath);
                if (o is XElement) bPath = false;
            }
        }

        private void GetValuesFromVariable(string sName, IEnumerable e)
        {
            TypeView view = new TypeView();
            int i = 1;
            IEnumerator enumerator = e.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string sName2 = sName + i++.ToString();
                view.Variable = enumerator.Current;
                foreach (NamedValue value in view.Values)
                {
                    if (value.Name == "") value.Name = sName2; else value.Name = sName2 + "_" + value.Name;
                    _values.Add(value);
                }
            }
        }

        private void GetValuesFromVariable(string sName, DataRow row)
        {
            if (sName != "") sName += "_";
            foreach (DataColumn col in row.Table.Columns)
            {
                _values.Add(new NamedValue(sName + col.ColumnName, row[col]));
            }
        }

        // désactivé le 03/03/2015 pour ne pas etre obligé d'ajouter FileReader au projet
        //private void GetValuesFromVariable(string sName, pb.old.FileReader fr)
        //{
        //    if (sName != "") sName += "_";
        //    foreach (pb.old.Field field in fr.Fields)
        //        _values.Add(new NamedValue(sName + field.Name, field.Value));
        //}

        private void GetValuesFromVariable(string sName, XElement v, bool bPath)
        {
            string s;
            if (bPath)
            {
                if (v != null) s = v.zGetPath(); else s = null;
                _values.Add(new NamedValue(sName + "_path", s));

                if (v != null) s = v.Name.ToString(); else s = null;
                _values.Add(new NamedValue(sName + "_node", s));

                sName += "_value";
            }
            if (v != null) s = v.Value.Trim(); else s = null;
            _values.Add(new NamedValue(sName, s));
        }

        private void GetValuesFromVariable(string sName, XText v, bool bPath)
        {
            string s;
            if (bPath)
            {
                if (v != null) s = v.Parent.zGetPath(); else s = null;
                _values.Add(new NamedValue(sName + "_path", s));

                if (v != null) s = v.Parent.Name.ToString(); else s = null;
                _values.Add(new NamedValue(sName + "_node", s));

                sName += "_value";
            }
            if (v != null) s = v.Value.Trim(); else s = null;
            _values.Add(new NamedValue(sName, s));
        }

        private void GetValuesFromType()
        {
            if (_values != null) return;
            _values = new List<NamedValue>();
            //if (gVariable == null)
            //{
            //    gValues.Add(new NamedValue("", gVariable));
            //    return;
            //}
            //Type type = gVariable.GetType();
            FieldInfo[] fields = _type.GetFields();
            if (_type.IsValueType || _type == typeof(string))
            {
                _values.Add(new NamedValue("", _type));
            }
            //else if (gVariable is IEnumerable)
            //{
            //    GetValuesFromVariable("value", gVariable as IEnumerable);
            //}
            //else if (gVariable is DataRow)
            //{
            //    GetValuesFromVariable("", gVariable as DataRow);
            //}
            else
            {
                PropertyInfo[] properties = _type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    _values.Add(new NamedValue(property.Name, property.PropertyType));
                }
                fields = type.GetFields();
                foreach (FieldInfo field in fields)
                {
                    _values.Add(new NamedValue(field.Name, field.FieldType));
                }
            }
        }
    }
}
