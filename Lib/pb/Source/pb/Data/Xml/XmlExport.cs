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
    }

    public class XmlValueExport
    {
        public string ElementName;
        public string ValueName;
        public MemberAccess MemberAccess;
        public object Value;
        public IEnumerator Enumerator;

        public object GetValue(object target)
        {
            return MemberAccess.GetValue(target);
        }
    }

    public static class XmlExport
    {
        //private Type _type = null;
        //private XmlExportDefinition _xmlDefinition = null;
        //private TypeAccess _typeAccess = null;
        //private XmlValueExport[] _valuesExport = null;

        public static void Export<T>(IEnumerable<T> dataList, string file, XmlExportDefinition xmlDefinition = null, bool detail = false)
        {
            //_type = typeof(T);

            if (xmlDefinition == null)
                xmlDefinition = new XmlExportDefinition();
            //_xmlDefinition = xmlDefinition;

            XmlValueExport[] valuesExport = CreateXmlValuesExport<T>(xmlDefinition);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = xmlDefinition.Encoding;
            settings.Indent = xmlDefinition.Indent;

            zfile.CreateFileDirectory(file);

            using (XmlWriter xwriter = XmlWriter.Create(file, settings))
            {
                xwriter.WriteStartElement(xmlDefinition.RootName);
                foreach (T data in dataList)
                {
                    bool first = true;
                    while (true)
                    {
                        if (!GetValues(data, valuesExport, detail, !first))
                            break;
                        xwriter.WriteStartElement(xmlDefinition.ElementName);
                        foreach (XmlValueExport valueExport in valuesExport)
                            WriteValue(xwriter, valueExport.ElementName, valueExport.Value, xmlDefinition);
                        xwriter.WriteEndElement();
                        first = false;


                        //bool detailValue = false;
                        //foreach (XmlValueExport valueExport in valuesExport)
                        //{
                        //    object value = null;
                        //    if (first)
                        //    {
                        //        value = valueExport.GetValue(data);
                        //        if (value is IEnumerable && !(value is string))
                        //        {
                        //            if (detail)
                        //            {
                        //                valueExport.Enumerator = ((IEnumerable)value).GetEnumerator();
                        //                //if (valueExport.Enumerator.MoveNext())
                        //                //{
                        //                //    value = valueExport.Enumerator.Current;
                        //                //    detailValue = true;
                        //                //}
                        //            }
                        //            else
                        //            {
                        //                IEnumerator enumerator  = ((IEnumerable)value).GetEnumerator();
                        //                if (enumerator.MoveNext())
                        //                    value = enumerator.Current;
                        //            }
                        //        }
                        //    }
                        //    if (valueExport.Enumerator != null)
                        //    {
                        //        if (valueExport.Enumerator.MoveNext())
                        //        {
                        //            value = valueExport.Enumerator.Current;
                        //            detailValue = true;
                        //        }
                        //    }

                        //    if (value is Date? && value != null)
                        //        value = (Date)value;
                        //    else if (value is DateTime? && value != null)
                        //        value = (DateTime)value;

                        //    string textValue = null;
                        //    if (value is Date)
                        //        textValue = ((Date)value).ToString(xmlDefinition.DateFormat);
                        //    else if (value is DateTime)
                        //        textValue = ((DateTime)value).ToString(xmlDefinition.DateTimeFormat);
                        //    else if (value != null)
                        //        textValue = value.ToString();

                        //    xwriter.zWriteElementText(valueExport.ElementName, textValue);
                        //}
                        //xwriter.WriteEndElement();
                        //if (!detail || !detailValue)
                        //    break;
                        //first = false;
                    }
                }
                xwriter.WriteEndElement();
            }
        }

        private static bool GetValues<T>(T data, XmlValueExport[] valuesExport, bool detail, bool onlyDetail)
        {
            bool foundValue = !onlyDetail;
            foreach (XmlValueExport valueExport in valuesExport)
            {
                object value = null;
                if (!onlyDetail)
                {
                    value = valueExport.GetValue(data);
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

        private static void WriteValue(XmlWriter xwriter, string elementName, object value, XmlExportDefinition xmlDefinition)
        {
            if (value is Date? && value != null)
                value = (Date)value;
            else if (value is DateTime? && value != null)
                value = (DateTime)value;

            string textValue = null;
            if (value is Date)
                textValue = ((Date)value).ToString(xmlDefinition.DateFormat);
            else if (value is DateTime)
                textValue = ((DateTime)value).ToString(xmlDefinition.DateTimeFormat);
            else if (value != null)
                textValue = value.ToString();

            xwriter.zWriteElementText(elementName, textValue);
        }

        private static XmlValueExport[] CreateXmlValuesExport<T>(XmlExportDefinition xmlDefinition)
        {
            XmlValueExport[] valuesExport;
            Type type = typeof(T);
            //_typeAccess = new TypeAccess(typeof(T));
            if (xmlDefinition.ValuesDefinition != null)
            {
                List<XmlValueExport> valueList = new List<XmlValueExport>();
                foreach (XmlValueDefinition valueDefinition in xmlDefinition.ValuesDefinition)
                {
                    MemberAccess memberAccess = MemberAccess.Create(type, valueDefinition.ValueName);
                    if (memberAccess != null)
                    {
                        valueList.Add(new XmlValueExport { ElementName = valueDefinition.ElementName, ValueName = valueDefinition.ValueName, MemberAccess = memberAccess });
                    }
                    else
                        Trace.WriteLine("warning xml export unknow variable \"{0}\" in type {1}", valueDefinition.ValueName, type.zGetTypeName());
                }
                valuesExport = valueList.ToArray();
            }
            else
            {
                valuesExport = MemberAccess.Create(type).Select(memberAccess => new XmlValueExport { ElementName = memberAccess.Name, ValueName = memberAccess.Name, MemberAccess = memberAccess })
                    .ToArray();
            }
            return valuesExport;
        }
    }

    public static partial class GlobalExtension
    {
        public static void zXmlExport<T>(this IEnumerable<T> dataList, string file, XmlExportDefinition xmlDefinition = null, bool detail = false)
        {
            XmlExport.Export(dataList, file, xmlDefinition, detail);
        }
    }

}
