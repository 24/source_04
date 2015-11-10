using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using pb.IO;
using pb.Reflection;

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
        public bool OnlyNextValue = true;
        public XmlValueDefinition[] ValuesDefinition = null;
    }

    public class XmlValueDefinition
    {
        public string ElementName;
        public string ValueName;
        public Func<object, object> TransformValue;
        public bool NotEnumerable;
    }

    public class XmlValueExport
    {
        public string ElementName;
        public TypeValueNode TypeValueNode;
        public Func<object, object> TransformValue;
    }

    public class XmlExport<T>
    {
        private XmlExportDefinition _xmlDefinition = null;
        private XmlWriter _xwriter = null;
        private TypeValues<T> _typeValues = null;
        private XmlValueExport[] _valuesExport = null;

        public XmlExport(XmlExportDefinition xmlDefinition = null)
        {
            if (xmlDefinition == null)
                xmlDefinition = new XmlExportDefinition();
            _xmlDefinition = xmlDefinition;
        }

        public void Export(IEnumerable<T> dataList, string file, bool detail = false)
        {
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
                    _typeValues.SetData(data);
                    _xwriter.WriteStartElement(_xmlDefinition.ElementName);
                    foreach (XmlValueExport valueExport in _valuesExport)
                    {
                        object value = _typeValues.GetValue(valueExport.TypeValueNode);
                        if (valueExport.TransformValue != null)
                            value = valueExport.TransformValue(value);
                        WriteValue(valueExport.ElementName, value);
                    }
                    _xwriter.WriteEndElement();

                    if (detail)
                    {
                        while (_typeValues.NextValues())
                        {
                            _xwriter.WriteStartElement(_xmlDefinition.ElementName);
                            foreach (XmlValueExport valueExport in _valuesExport)
                            {
                                object value = _typeValues.GetValue(valueExport.TypeValueNode, _xmlDefinition.OnlyNextValue);
                                WriteValue(valueExport.ElementName, value);
                            }
                            _xwriter.WriteEndElement();
                        }
                    }
                }
                _xwriter.WriteEndElement();
            }
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
            _typeValues = new TypeValues<T>();
            List<XmlValueExport> valueList = new List<XmlValueExport>();
            if (_xmlDefinition.ValuesDefinition != null)
            {
                foreach (XmlValueDefinition valueDefinition in _xmlDefinition.ValuesDefinition)
                {
                    XmlValueExport xmlValueExport = new XmlValueExport();
                    xmlValueExport.ElementName = valueDefinition.ElementName;
                    xmlValueExport.TypeValueNode = _typeValues.AddValue(valueDefinition.ValueName, MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property,
                        valueDefinition.NotEnumerable);
                    xmlValueExport.TransformValue = valueDefinition.TransformValue;
                    valueList.Add(xmlValueExport);
                }
            }
            else
            {
                _typeValues.AddAllValues(MemberType.Instance | MemberType.Public | MemberType.Field | MemberType.Property);
                foreach (TypeValueNode typeValueNode in _typeValues.TypeValueNodes.Values)
                {
                    if (typeValueNode.TypeValueAccess.IsValueType)
                    {
                        XmlValueExport xmlValueExport = new XmlValueExport();
                        xmlValueExport.ElementName = typeValueNode.TypeValueAccess.TreeName;
                        xmlValueExport.TypeValueNode = typeValueNode;
                        valueList.Add(xmlValueExport);
                    }
                }
            }
            _valuesExport = valueList.ToArray();
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
