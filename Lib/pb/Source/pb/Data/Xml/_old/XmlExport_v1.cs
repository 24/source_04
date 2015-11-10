using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using pb.IO;
using pb.Reflection;

namespace pb.Data.Xml
{
    public class XmlValueExport_v1
    {
        public string ElementName;
        public ValueAccess_v1 ValueAccess;
        public string ValueName;
        public Func<object, object> TransformValue;
        public TypeValueAccess TypeValueAccess;
        public object Value;
        public IEnumerator Enumerator;

        public object GetValue(object target)
        {
            return TypeValueAccess.GetValue(target);
        }
    }

    //public class XmlValueExport_v2
    //{
    //    public string ElementName;
    //    public ValueAccess ValueAccess;
    //    //public string ValueName;
    //    //public Func<object, object> TransformValue;
    //    //public MemberAccess MemberAccess;
    //    //public object Value;
    //    //public IEnumerator Enumerator;

    //    //public object GetValue(object target)
    //    //{
    //    //    return MemberAccess.GetValue(target);
    //    //}
    //}

    public class ValueAccess_v1
    {
        public string ValueName;
        public Func<object, object> TransformValue;
        public TypeValueAccess TypeValueAccess;
        public object Value;
        public IEnumerator Enumerator;
        public List<ValueAccess_v1> Childs = new List<ValueAccess_v1>();

        public object GetValue(object target)
        {
            return TypeValueAccess.GetValue(target);
        }
    }

    public class XmlExport_v1<T>
    {
        //private Type _type = null;
        private XmlExportDefinition _xmlDefinition = null;
        //private TypeAccess _typeAccess = null;
        private XmlValueExport_v1[] _valuesExport = null;
        private XmlWriter _xwriter = null;

        public XmlExport_v1(XmlExportDefinition xmlDefinition = null)
        {
            if (xmlDefinition == null)
                xmlDefinition = new XmlExportDefinition();
            _xmlDefinition = xmlDefinition;
        }

        public void Export(IEnumerable<T> dataList, string file, bool detail = false)
        {
            //_type = typeof(T);

            //_xmlDefinition = xmlDefinition;

            CreateXmlValuesExport();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = _xmlDefinition.Encoding;
            settings.Indent = _xmlDefinition.Indent;

            zfile.CreateFileDirectory(file);

            using (_xwriter = XmlWriter.Create(file, settings))
            {
                _xwriter.WriteStartElement(_xmlDefinition.RootName);
                foreach (T data in dataList)
                {
                    bool first = true;
                    while (true)
                    {
                        if (!GetValues(data, detail, !first))
                            break;
                        _xwriter.WriteStartElement(_xmlDefinition.ElementName);
                        foreach (XmlValueExport_v1 valueExport in _valuesExport)
                            WriteValue(valueExport.ElementName, valueExport.Value);
                        _xwriter.WriteEndElement();
                        first = false;
                    }
                }
                _xwriter.WriteEndElement();
            }
        }

        private bool GetValues(T data, bool detail, bool onlyDetail)
        {
            bool foundValue = !onlyDetail;
            foreach (XmlValueExport_v1 valueExport in _valuesExport)
            {
                object value = null;
                if (!onlyDetail)
                {
                    value = valueExport.GetValue(data);

                    if (valueExport.TransformValue != null)
                        value = valueExport.TransformValue(value);


                    if (value is IEnumerable && !(value is string))
                    {
                        if (detail)
                        {
                            valueExport.Enumerator = ((IEnumerable)value).GetEnumerator();
                        }
                        else
                        {
                            IEnumerator enumerator = ((IEnumerable)value).GetEnumerator();
                            if (enumerator.MoveNext())
                                value = enumerator.Current;
                        }
                    }
                }
                if (valueExport.Enumerator != null)
                {
                    if (valueExport.Enumerator.MoveNext())
                    {
                        value = valueExport.Enumerator.Current;
                        foundValue = true;
                    }
                }
                valueExport.Value = value;
            }
            return foundValue;
        }

        private void WriteValue(string elementName, object value)
        {
            if (value is Date? && value != null)
                value = (Date)value;
            else if (value is DateTime? && value != null)
                value = (DateTime)value;

            string textValue = null;
            if (value is Date)
                textValue = ((Date)value).ToString(_xmlDefinition.DateFormat);
            else if (value is DateTime)
                textValue = ((DateTime)value).ToString(_xmlDefinition.DateTimeFormat);
            else if (value != null)
                textValue = value.ToString();

            _xwriter.zWriteElementText(elementName, textValue);
        }

        private void CreateXmlValuesExport()
        {
            Type type = typeof(T);
            if (_xmlDefinition.ValuesDefinition != null)
            {
                List<XmlValueExport_v1> valueList = new List<XmlValueExport_v1>();
                foreach (XmlValueDefinition valueDefinition in _xmlDefinition.ValuesDefinition)
                {
                    TypeValueAccess typeValueAccess = TypeValueAccess.Create(type, valueDefinition.ValueName);
                    if (typeValueAccess != null)
                    {
                        valueList.Add(new XmlValueExport_v1 { ElementName = valueDefinition.ElementName, ValueName = valueDefinition.ValueName,
                            TransformValue = valueDefinition.TransformValue, TypeValueAccess = typeValueAccess });
                    }
                    else
                        Trace.WriteLine("warning xml export unknow variable \"{0}\" in type {1}", valueDefinition.ValueName, type.zGetTypeName());
                }
                _valuesExport = valueList.ToArray();
            }
            else
            {
                _valuesExport = TypeValueAccess.Create(type).Select(memberAccess => new XmlValueExport_v1 { ElementName = memberAccess.Name, ValueName = memberAccess.Name, TypeValueAccess = memberAccess })
                    .ToArray();
            }
        }

        public static void Export(IEnumerable<T> dataList, string file, XmlExportDefinition xmlDefinition = null, bool detail = false)
        {
            XmlExport_v1<T> xmlExport = new XmlExport_v1<T>(xmlDefinition);
            xmlExport.Export(dataList, file, detail);
        }
    }

    public static partial class GlobalExtension
    {
        public static void zXmlExport_v1<T>(this IEnumerable<T> dataList, string file, XmlExportDefinition xmlDefinition = null, bool detail = false)
        {
            XmlExport_v1<T>.Export(dataList, file, xmlDefinition, detail);
        }
    }
}
