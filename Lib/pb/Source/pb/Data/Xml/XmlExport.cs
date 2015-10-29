using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using pb.IO;

namespace pb.Data.Xml
{
    public class XmlExportDefinition
    {
        public string RootName = "Export";
        public string ElementName = "Data";
        public Encoding Encoding = Encoding.UTF8;
        public string DateFormat = "yyyy-MM-dd";
        public string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public bool Indent = true;
        public XmlValueDefinition[] ValuesDefinition;
    }

    public class XmlValueDefinition
    {
        public string ElementName;
        public string ValueName;
        public Func<object, object> TransformValue;
    }

    public class XmlValueExport
    {
        public string ElementName;
        public string ValueName;
        public Func<object, object> TransformValue;
        public MemberAccess MemberAccess;
        public object Value;
        public IEnumerator Enumerator;

        public object GetValue(object target)
        {
            return MemberAccess.GetValue(target);
        }
    }

    public class XmlExport<T>
    {
        //private Type _type = null;
        private XmlExportDefinition _xmlDefinition = null;
        //private TypeAccess _typeAccess = null;
        private XmlValueExport[] _valuesExport = null;
        private XmlWriter _xwriter = null;

        public XmlExport(XmlExportDefinition xmlDefinition = null)
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

            //using (XmlWriter xwriter = XmlWriter.Create(file, settings))
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
                        foreach (XmlValueExport valueExport in _valuesExport)
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
            foreach (XmlValueExport valueExport in _valuesExport)
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
                List<XmlValueExport> valueList = new List<XmlValueExport>();
                foreach (XmlValueDefinition valueDefinition in _xmlDefinition.ValuesDefinition)
                {
                    MemberAccess memberAccess = MemberAccess.Create(type, valueDefinition.ValueName);
                    if (memberAccess != null)
                    {
                        valueList.Add(new XmlValueExport { ElementName = valueDefinition.ElementName, ValueName = valueDefinition.ValueName,
                            TransformValue = valueDefinition.TransformValue, MemberAccess = memberAccess });
                    }
                    else
                        Trace.WriteLine("warning xml export unknow variable \"{0}\" in type {1}", valueDefinition.ValueName, type.zGetTypeName());
                }
                _valuesExport = valueList.ToArray();
            }
            else
            {
                _valuesExport = MemberAccess.Create(type).Select(memberAccess => new XmlValueExport { ElementName = memberAccess.Name, ValueName = memberAccess.Name, MemberAccess = memberAccess })
                    .ToArray();
            }
        }

        public static void Export(IEnumerable<T> dataList, string file, XmlExportDefinition xmlDefinition = null, bool detail = false)
        {
            XmlExport<T> xmlExport = new XmlExport<T>(xmlDefinition);
            xmlExport.Export(dataList, file, detail);
        }
    }

    public static partial class GlobalExtension
    {
        public static void zXmlExport<T>(this IEnumerable<T> dataList, string file, XmlExportDefinition xmlDefinition = null, bool detail = false)
        {
            XmlExport<T>.Export(dataList, file, xmlDefinition, detail);
        }
    }

}
