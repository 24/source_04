using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using pb.Web;

#region a faire ...
    // - paramétrer WebRequest : http.Method, http.Referer
    // - WebRequest : ajouter répertoire pour générer les fichiers de trace
    // - XmlToDataTable.ToDataTable() : utiliser le schema pour générer la table avec les colonnes dans l'ordre
#endregion

namespace PB_Library
{
    #region class Xml2Data_Exception
    public class Xml2Data_Exception : Exception
    {
        public Xml2Data_Exception(string sMessage) : base(sMessage) { }
        public Xml2Data_Exception(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public Xml2Data_Exception(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public Xml2Data_Exception(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region class XmlSchemaElement
    public class XmlSchemaElement
    {
        public XName Name = null;
        public List<XmlSchemaElement> Childs = new List<XmlSchemaElement>();
        public XmlSchemaElement Parent = null;
        public List<XName> Attributes = new List<XName>();

        #region constructor
        #region XmlSchemaElement(XName name)
        public XmlSchemaElement(XName name)
        {
            Name = name;
        }
        #endregion

        #region XmlSchemaElement(XName name, XmlSchemaElement parent)
        public XmlSchemaElement(XName name, XmlSchemaElement parent)
        {
            Name = name;
            if (parent != null)
            {
                Parent = parent;
                parent.Childs.Add(this);
            }
        }
        #endregion
        #endregion

        #region Element
        public XmlSchemaElement Element(XName name)
        {
            foreach (XmlSchemaElement element in Childs)
                if (element.Name == name) return element;
            return null;
        }
        #endregion

        #region GetIndexOfAttribute
        public int GetIndexOfAttribute(XName name)
        {
            int index = 0;
            foreach (XName name2 in Attributes)
            {
                if (name2 == name) return index;
                index++;
            }
            return -1;
        }
        #endregion

        #region AddAttributes
        public void AddAttributes(XElement source)
        {
            int lastIndex = -1;
            foreach (XAttribute xa in source.Attributes().Reverse())
            {
                int i = GetIndexOfAttribute(xa.Name);
                if (i == -1)
                {
                    if (lastIndex == -1)
                    {
                        Attributes.Add(xa.Name);
                        lastIndex = Attributes.Count - 1;
                    }
                    else
                        Attributes.Insert(lastIndex, xa.Name);
                }
                else
                    lastIndex = i;
            }
        }
        #endregion

        #region ToXml
        public XDocument ToXml()
        {
            XDocument xd = new XDocument();
            xd.Add(_ToXml());
            return xd;
        }
        #endregion

        #region _ToXml
        public XElement _ToXml()
        {
            XElement xe = new XElement(Name);
            foreach (XName name in Attributes) xe.Add(new XAttribute(name, ""));
            foreach (XmlSchemaElement child in Childs)
                xe.Add(child._ToXml());
            return xe;
        }
        #endregion
    }
    #endregion

    #region class XmlSchema
    public class XmlSchema
    {
        private XElement gxeSource = null;
        private DataTable gdt = new DataTable();

        #region constructor
        public XmlSchema(XElement source)
        {
            gxeSource = source;
        }
        #endregion

        #region CreateXmlSchema
        public XDocument CreateXmlSchema()
        {
            XmlSchemaElement xeSchema = AddElement(null, gxeSource);
            return xeSchema.ToXml();
        }
        #endregion

        #region static AddElement
        public static XmlSchemaElement AddElement(XmlSchemaElement parentElement, XElement source)
        {
            XmlSchemaElement element = null;
            if (parentElement != null)
            {
                element = parentElement.Element(source.Name);
                if (element == null)
                    element = new XmlSchemaElement(source.Name, parentElement);
            }
            else
                element = new XmlSchemaElement(source.Name);
            element.AddAttributes(source);
            foreach (XElement child in source.Elements())
            {
                AddElement(element, child);
            }
            return element;
        }
        #endregion

        #region static CreateXmlSchema(XDocument source)
        public static XDocument CreateXmlSchema(XDocument source)
        {
            return CreateXmlSchema(source.Root);
        }
        #endregion

        #region static CreateXmlSchema(XElement source)
        public static XDocument CreateXmlSchema(XElement source)
        {
            XmlSchema ws = new XmlSchema(source);
            return ws.CreateXmlSchema();
        }
        #endregion

    }
    #endregion

    #region class XmlToDataTable
    public class XmlToDataTable
    {
        #region variable
        private XElement gxeElement = null;
        private DataTable gdtTable = null;
        private int giIndexLastColumn;
        #endregion

        #region constructor
        public XmlToDataTable(XElement xe)
        {
            gxeElement = xe;
        }
        #endregion

        #region ToDataTable
        public DataTable ToDataTable()
        {
            gdtTable = new DataTable();
            foreach (XElement xe in gxeElement.Elements())
            {
                ToDataRows(xe);
            }
            return gdtTable;
        }
        #endregion

        #region ToDataRows
        public void ToDataRows(XElement xe)
        {
            giIndexLastColumn = -1;
            List<DataRow> rows = new List<DataRow>();
            AddAttributeValues(rows, xe);

            AddChildsValues(rows, xe);

            foreach (DataRow row in rows) gdtTable.Rows.Add(row);
        }
        #endregion

        #region AddChildsValues
        public void AddChildsValues(List<DataRow> rows, XElement xe)
        {
            SortedList<string, string> childsElements = new SortedList<string, string>();
            foreach (XElement xe2 in xe.Elements())
            {
                AddAttributeValues(rows, xe2);
                if (!xe2.HasElements) continue;
                if (childsElements.ContainsKey(xe2.Name.LocalName))
                {
                    List<DataRow> rows2 = new List<DataRow>();
                    AddChildsValues(rows2, xe2);
                    rows.AddRange(rows2);
                }
                else
                {
                    AddChildsValues(rows, xe2);
                    childsElements.Add(xe2.Name.LocalName, null);
                }

            }
        }
        #endregion

        #region AddAttributeValues
        public void AddAttributeValues(List<DataRow> rows, XElement xe)
        {
            foreach (XAttribute xa in xe.Attributes())
            {
                string sName = xa.Name.LocalName;
                if (string.Compare(sName, "value", true) == 0)
                    sName = xe.Name.LocalName;
                AddValue(rows, sName, xa.Value);
            }
        }
        #endregion

        #region AddValue
        public void AddValue(List<DataRow> rows, string Name, string Value)
        {
            int iColumn = gdtTable.Columns.IndexOf(Name);
            if (iColumn == -1)
            {
                DataColumn column = gdtTable.Columns.Add(Name, typeof(string));
                if (giIndexLastColumn != -1) column.SetOrdinal(giIndexLastColumn + 1);
                iColumn = column.Ordinal;
            }
            giIndexLastColumn = iColumn;
            bool bValueAdded = false;
            foreach (DataRow row in rows)
            {
                if (row[iColumn] == DBNull.Value)
                {
                    bValueAdded = true;
                    row[iColumn] = Value;
                    break;
                }
            }
            if (!bValueAdded)
            {
                DataRow row = gdtTable.NewRow();
                row[iColumn] = Value;
                rows.Add(row);
            }
        }
        #endregion

        #region static ToDataTable(XDocument xd)
        public static DataTable ToDataTable(XDocument xd)
        {
            return ToDataTable(xd.Root);
        }
        #endregion

        #region static ToDataTable(XElement xe)
        public static DataTable ToDataTable(XElement xe)
        {
            XmlToDataTable xdt = new XmlToDataTable(xe);
            return xdt.ToDataTable();
        }
        #endregion

    }
    #endregion

    #region class ZReadMessage
    public class ZReadMessage
    {
        public string Message = null;
        public ZSchemaElement SchemaElement;
        public XElement SourceElement;
    }
    #endregion

    #region class ZRead
    public class ZRead
    {
        #region variable ...
        private ZSchema gSchema = null;
        private XElement gSource = null;
        private string gsSourcePath = null;
        private string gsXmlSourcePath = null;
        private SortedList<string, string> gConstants = null;
        private ZReadGroupValues gValues = null;
        private List<ZReadMessage> gMessages = new List<ZReadMessage>();
        private bool gbExportNullValue = false;
        #endregion

        #region property ...
        #region Schema
        public ZSchema Schema
        {
            get { return gSchema; }
            set { gSchema = value; }
        }
        #endregion

        #region Source
        public XElement Source
        {
            get { return gSource; }
            set { gSource = value; }
        }
        #endregion

        #region SourcePath
        public string SourcePath
        {
            get { return gsSourcePath; }
            set { gsSourcePath = value; }
        }
        #endregion

        #region XmlSourcePath
        public string XmlSourcePath
        {
            get { return gsXmlSourcePath; }
            set { gsXmlSourcePath = value; }
        }
        #endregion

        #region Constants
        public SortedList<string, string> Constants
        {
            get { return gConstants; }
            set { gConstants = value; }
        }
        #endregion

        #region Values
        public ZReadGroupValues Values
        {
            get { return gValues; }
        }
        #endregion

        #region Messages
        public List<ZReadMessage> Messages
        {
            get { return gMessages; }
        }
        #endregion

        #region ExportNullValue
        public bool ExportNullValue
        {
            get { return gbExportNullValue; }
            set { gbExportNullValue = value; }
        }
        #endregion
        #endregion

        #region Read()
        public bool Read()
        {
            gValues = new ZReadGroupValues();
            gValues.SchemaGroupValues = gSchema.RootValues;

            ZSchemaElement schemaElement = gSchema.RootElement;

            if (!schemaElement.Equals(gSource))
            {
                //cTrace.Trace("ZRead.Read() : l'élément source ne correspond pas à l'élément du schema, schema {0}, source {1}", schema, source.zToString());
                return false;
            }

            ZReadGroupValues groupValues = schemaElement.ReadValues(gSource, gConstants, gValues);

            //return Read(gSchema.RootElement, gSource, gValues);
            return Read(schemaElement, gSource, groupValues);
        }
        #endregion

        #region Read(ZSchemaElement schema, XElement source, ZReadGroupValues groupValues)
        public bool Read(ZSchemaElement schema, XElement source, ZReadGroupValues groupValues)
        {
            //if (!schema.HasChildElement) return;
            if (!schema.HasChildElement) return true;

            ZSchemaElement schemaElement;
            ZSchemaElement lastSchemaElement = null;

            //if (!schema.Equals(source))
            //{
            //    //cTrace.Trace("ZRead.Read() : l'élément source ne correspond pas à l'élément du schema, schema {0}, source {1}", schema, source.zToString());
            //    return false;
            //}

            //if (!schema.HasChildElement) return true;

            foreach (XElement xe in source.Elements())
            {
                schemaElement = GetNextSchemaElement(schema, lastSchemaElement, xe);
                if (schemaElement != null && !schemaElement.Exclude)
                {
                    lastSchemaElement = schemaElement;

                    ZReadGroupValues groupValues2 = schemaElement.ReadValues(xe, gConstants, groupValues);

                    Read(schemaElement, xe, groupValues2);
                }
            }
            if (lastSchemaElement != null)
                schemaElement = lastSchemaElement.NextElement;
            else
                schemaElement = schema.Childs[0];
            while (schemaElement != null)
            {
                if (!schemaElement.Optional && !schemaElement.Exclude)
                    gMessages.Add(new ZReadMessage() { Message = string.Format("Element not found in source : {0}", schemaElement), SchemaElement = schemaElement });
                schemaElement = schemaElement.NextElement;
            }
            return true;
        }
        #endregion

        #region GetNextSchemaElement
        /// <summary>
        /// Recherche de l'élément de schéma correspondant à source,
        /// Si lastSchemaElement n'est pas nul :
        ///   - si il est "Multiple" (li, dt, dd, tr, td) la recherche commence par lastSchemaElement,
        ///   - sinon la recherche commence par l'élément suivant lastSchemaElement
        ///   - si lastSchemaElement n'a pas d'élément suivant et que parentSchemaElement est "MultipleChild", la recherche commence par le 1er fils de parentSchemaElement
        /// Si lastSchemaElement est pas nul : la recherche commence par le 1er fils de parentSchemaElement
        /// si l'élément n'est pas trouvé le message "Element not found in schema" est généré
        /// si il y a des éléments non optionnel dans le schéma qui sont sautés, le message "Element not found in source" est généré
        /// </summary>
        public ZSchemaElement GetNextSchemaElement(ZSchemaElement parentSchemaElement, ZSchemaElement lastSchemaElement, XElement source)
        {
            ZSchemaElement firstElement = null;
            if (lastSchemaElement != null)
            {
                if (lastSchemaElement.Multiple)
                {
                    if (lastSchemaElement.Equals(source))
                        return lastSchemaElement;
                }
                firstElement = lastSchemaElement.NextElement;
                if (firstElement == null && parentSchemaElement.MultipleChild)
                    firstElement = parentSchemaElement.Childs[0];
            }
            else if (parentSchemaElement.Childs.Count > 0)
                firstElement = parentSchemaElement.Childs[0];

            ZSchemaElement elementFound = null;
            ZSchemaElement element = firstElement;
            while (element != null)
            {
                if (element.Equals(source))
                {
                    elementFound = element;
                    break;
                }
                element = element.NextElement;
            }
            if (elementFound == null)
            {
                if (gSchema.ControlElementNotFoundInSchema)
                    gMessages.Add(new ZReadMessage() { Message = string.Format("Element not found in schema : {0,-80} - {1}", parentSchemaElement, FormatXElement(source)), SchemaElement = parentSchemaElement, SourceElement = source });
                return null;
            }

            element = firstElement;
            while (element != elementFound)
            {
                if (!element.Optional && !element.Exclude)
                {
                    gMessages.Add(new ZReadMessage() { Message = string.Format("Element not found in source : {0}", element), SchemaElement = element });
                }
                element = element.NextElement;
            }

            return elementFound;
        }
        #endregion

        #region ResultToXml
        public XElement ResultToXml()
        {
            XElement xe = new XElement("Result");
            if (gValues == null) return xe;
            var values = from value in gValues.Values orderby value.ValueDefinition.OrderNumber select value;
            int i = SetAttribValues(xe, values);
            ValuesToXml(xe, values.Skip(i), false);
            return xe;
        }
        #endregion

        #region SetAttribValues
        private static int SetAttribValues(XElement xe, IEnumerable<ZReadValue> values)
        {
            ZSchemaElement schemaElement = null;
            int iNbValueToSkip = -1;
            int i = 0;
            foreach (ZReadValue value in values)
            {
                if (schemaElement == null)
                    schemaElement = value.ValueDefinition.SchemaElement;
                else if (schemaElement != value.ValueDefinition.SchemaElement)
                    break;
                if (!value.ValueDefinition.Hide && !value.ValueDefinition.NoValue)
                {
                    string s = value.Value;
                    if (s != null) xe.Add(new XAttribute(value.ValueDefinition.Name, s));
                }
                else if (value.ValueDefinition.NoValue && iNbValueToSkip == -1)
                    iNbValueToSkip = i;
                i++;
            }
            if (iNbValueToSkip == -1) iNbValueToSkip = i;
            return iNbValueToSkip;
        }
        #endregion

        #region MessageToXml
        public XElement MessageToXml()
        {
            XElement xeResult = new XElement("Result");
            bool bMessage = false;

            XElement xeValuesErrors = new XElement("ValuesErrors");
            if (ValuesErrorsToXml(xeValuesErrors, gValues.Values) > 0)
            {
                xeResult.Add(xeValuesErrors);
                bMessage = true;
            }

            foreach (ZReadMessage message in gMessages)
            {
                bMessage = true;
                XElement xeMessage = new XElement("Message");
                xeMessage.Add(new XElement("Message", message.Message));
                xeMessage.Add(new XElement("SchemaElement", message.SchemaElement.ToString()));
                if (message.SourceElement != null)
                    xeMessage.Add(new XElement("SourceElement", message.SourceElement));
                xeResult.Add(xeMessage);
            }

            if (!bMessage) return null;
            SetAttribValues(xeResult, gValues.Values);
            return xeResult;
        }
        #endregion

        #region ValuesToXml
        public void ValuesToXml(XElement xe, IEnumerable<ZReadValue> values, bool bExportAsAttrib)
        {
            foreach (ZReadValue value in values)
            {
                if (!value.ValueDefinition.Hide && !value.ValueDefinition.NoValue)
                {
                    string s = value.Value;
                    if (gbExportNullValue && s == null) s = "";
                    if (s != null)
                    {
                        object o;
                        if (bExportAsAttrib || value.ValueDefinition.ExportAsAttrib)
                            o = new XAttribute(value.ValueDefinition.Name, s);
                        else
                            o = new XElement(value.ValueDefinition.Name, new XAttribute("Value", s));
                        xe.Add(o);
                    }
                }
                if (value.ChildGroupValues != null)
                {
                    bool bExportAsAttrib2;
                    XElement xe2;
                    if (bExportAsAttrib)
                    {
                        xe2 = xe;
                        bExportAsAttrib2 = true;
                    }
                    else
                    {
                        xe2 = new XElement(value.ChildGroupValues.SchemaGroupValues.Name);
                        xe.Add(xe2);
                        bExportAsAttrib2 = value.ChildGroupValues.SchemaGroupValues.ExportAsAttrib;
                    }
                    var childValues = from v in value.ChildGroupValues.Values orderby v.ValueDefinition.OrderNumber select v;
                    ValuesToXml(xe2, childValues, bExportAsAttrib2);
                }
            }
        }
        #endregion

        #region ValuesErrorsToXml(XElement xe, List<ZReadValue> values)
        public static int ValuesErrorsToXml(XElement xe, List<ZReadValue> values)
        {
            int nbValuesErrors = 0;
            foreach (ZReadValue value in values)
            {
                string sParent = null;
                //if (values2.ParentValues != null)
                //{
                //    foreach (ZReadValue value in values2.ParentValues.Values)
                //    {
                //        if (sParent == null)
                //            sParent = value.ValueDefinition.Name;
                //        else
                //            sParent += " " + value.ValueDefinition.Name;
                //    }

                if (value.Value == null && !value.ValueDefinition.NoValue && !value.ValueDefinition.Optional)
                {
                    XElement xe2 = new XElement("ValueError");
                    ZValue valueDef = value.ValueDefinition;

                    xe2.Add(new XAttribute("Error", "Value not found"));
                    xe2.Add(new XAttribute("Name", valueDef.Name));
                    if (valueDef.Type != null) xe2.Add(new XAttribute("Type", valueDef.Type));
                    if (valueDef.IndexTextValue > 1) xe2.Add(new XAttribute("Index", valueDef.IndexTextValue));
                    if (valueDef.ElementName != null) xe2.Add(new XAttribute("Element", valueDef.ElementName));
                    if (valueDef.AttribName != null) xe2.Add(new XAttribute("Attrib", valueDef.AttribName));
                    if (sParent != null) xe2.Add(new XAttribute("Parent", sParent));
                    string sParentSchemaElement = valueDef.SchemaElement.ElementName;
                    if (sParentSchemaElement == null) sParentSchemaElement = "";
                    xe2.Add(new XAttribute("ParentElement", sParentSchemaElement));
                    xe.Add(xe2);
                    nbValuesErrors++;
                }
                if (value.ChildGroupValues != null)
                    nbValuesErrors += ValuesErrorsToXml(xe, value.ChildGroupValues.Values);
            }
            return nbValuesErrors;
        }
        #endregion

        #region Read(ZSchema schema, XDocument source)
        public static ZRead Read(ZSchema schema, XDocument source)
        {
            return Read(schema, source.Root, null);
        }
        #endregion

        #region Read(ZSchema schema, XDocument source, SortedList<string, string> constants)
        public static ZRead Read(ZSchema schema, XDocument source, SortedList<string, string> constants)
        {
            return Read(schema, source.Root, constants);
        }
        #endregion

        #region Read(ZSchema schema, XElement source)
        public static ZRead Read(ZSchema schema, XElement source)
        {
            return Read(schema, source, null);
        }
        #endregion

        #region Read(ZSchema schema, XElement source, SortedList<string, string> constants)
        public static ZRead Read(ZSchema schema, XElement source, SortedList<string, string> constants)
        {
            ZRead zr = new ZRead();
            zr.gSchema = schema;
            zr.gSource = source;
            zr.gConstants = constants;
            if (!zr.Read()) return null;
            return zr;
        }
        #endregion

        #region StringDeleteCRLF
        private static Regex grxDeleteCRLF = new Regex("[\r\n]+", RegexOptions.Compiled);
        public static string StringDeleteCRLF(string s)
        {
            return grxDeleteCRLF.Replace(s, "");
        }
        #endregion

        #region FormatXElement
        public static string FormatXElement(XElement xe)
        {
            string sSource = StringDeleteCRLF(xe.ToString());
            if (sSource.Length > 100) sSource = sSource.Substring(0, 100) + "...";
            return sSource;
        }
        #endregion
    }
    #endregion

    #region class ZEnumNode
    public class ZEnumNode : IEnumerable<XNode>, IEnumerator<XNode>
    {
        #region variable
        private ZSchemaElement gSchemaElement = null;
        private XElement gSourceElement = null;
        private ZSchemaElement gCurrentSchemaElement = null;
        private XNode gNodeOfCurrentSchemaElement = null;
        private XNode gCurrentNode = null;

        private bool gbTrimValue = false;
        private Regex gFilterValue = null;
        #endregion

        #region constructor
        #region ZEnumNode()
        public ZEnumNode()
        {
        }
        #endregion

        #region ZEnumNode(ZSchemaElement schema, XElement source)
        public ZEnumNode(ZSchemaElement schema, XElement source)
        {
            gSchemaElement = schema;
            gSourceElement = source;
            Reset();
        }
        #endregion

        #region ZEnumNode(ZEnumNode en)
        public ZEnumNode(ZEnumNode en)
        {
            if (en.Current is XElement)
                gSourceElement = (XElement)en.Current;
            if (en.NodeOfCurrentSchemaElement == en.Current)
                gSchemaElement = en.CurrentSchemaElement;
            Reset();
        }
        #endregion
        #endregion

        #region Dispose
        public void Dispose()
        {
        }
        #endregion

        #region Clone
        public ZEnumNode Clone()
        {
            ZEnumNode en = new ZEnumNode();
            en.gSchemaElement = gSchemaElement;
            en.gSourceElement = gSourceElement;
            en.gCurrentSchemaElement = gCurrentSchemaElement;
            en.gNodeOfCurrentSchemaElement = gNodeOfCurrentSchemaElement;
            en.gCurrentNode = gCurrentNode;
            en.gbTrimValue = gbTrimValue;
            en.gFilterValue = gFilterValue;
            return en;
        }
        #endregion

        #region property ...
        #region SchemaElement
        public ZSchemaElement SchemaElement
        {
            get { return gSchemaElement; }
        }
        #endregion

        #region SourceElement
        public XElement SourceElement
        {
            get { return gSourceElement; }
        }
        #endregion

        #region CurrentSchemaElement
        public ZSchemaElement CurrentSchemaElement
        {
            get { return gCurrentSchemaElement; }
        }
        #endregion

        #region NodeOfCurrentSchemaElement
        public XNode NodeOfCurrentSchemaElement
        {
            get { return gNodeOfCurrentSchemaElement; }
        }
        #endregion

        #region TrimValue
        public bool TrimValue
        {
            get { return gbTrimValue; }
            set { gbTrimValue = value; }
        }
        #endregion

        #region FilterValue
        public Regex FilterValue
        {
            get { return gFilterValue; }
            set { gFilterValue = value; }
        }
        #endregion
        #endregion

        #region GetEnumerator
        public IEnumerator<XNode> GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerable.GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region Current
        public XNode Current
        {
            get { return gCurrentNode; }
        }

        #endregion

        #region IEnumerator.Current
        object IEnumerator.Current
        {
            get { return gCurrentNode; }
        }
        #endregion

        #region MoveNext
        public bool MoveNext()
        {
            if (gCurrentNode == null) return false;
            if (gCurrentNode is XElement)
            {
                XNode xn1 = ((XElement)gCurrentNode).DescendantNodes().FirstOrDefault();
                if (xn1 != null)
                {
                    if (gNodeOfCurrentSchemaElement == gCurrentNode && xn1 is XElement)
                    {
                        ZSchemaElement ze = gCurrentSchemaElement.GetSchemaElement((XElement)xn1);
                        if (ze != null)
                        {
                            gCurrentSchemaElement = ze;
                            gNodeOfCurrentSchemaElement = xn1;
                        }
                    }
                    gCurrentNode = xn1;
                    if (gCurrentNode != gNodeOfCurrentSchemaElement || !gCurrentSchemaElement.Exclude)
                        return true;
                }
            }

            XNode xn = gCurrentNode;

            while (true)
            {
                while (true)
                {
                    XNode xn2 = xn.NextNode;
                    if (xn2 == null) break;

                    if (xn2 is XElement)
                    {
                        if (gNodeOfCurrentSchemaElement == xn)
                        {
                            ZSchemaElement ze = gCurrentSchemaElement.Parent.GetSchemaElement((XElement)xn2);
                            if (ze != null)
                            {
                                gCurrentSchemaElement = ze;
                                gNodeOfCurrentSchemaElement = xn2;
                            }
                            else
                            {
                                gCurrentSchemaElement = gCurrentSchemaElement.Parent;
                                gNodeOfCurrentSchemaElement = gNodeOfCurrentSchemaElement.Parent;
                            }
                        }
                        if (gNodeOfCurrentSchemaElement == xn.Parent)
                        {
                            ZSchemaElement ze = gCurrentSchemaElement.GetSchemaElement((XElement)xn2);
                            if (ze != null)
                            {
                                gCurrentSchemaElement = ze;
                                gNodeOfCurrentSchemaElement = xn2;
                            }
                        }
                    }
                    else if (gNodeOfCurrentSchemaElement != null && gNodeOfCurrentSchemaElement.Parent == xn.Parent)
                    {
                        gCurrentSchemaElement = gCurrentSchemaElement.Parent;
                        gNodeOfCurrentSchemaElement = gNodeOfCurrentSchemaElement.Parent;
                    }

                    gCurrentNode = xn2;
                    if (gCurrentNode != gNodeOfCurrentSchemaElement || !gCurrentSchemaElement.Exclude)
                        return true;
                    xn = xn2;
                }

                XElement xe = xn.Parent;
                if (xe == gSourceElement) return false;
                if (xe == null) return false;

                if (gNodeOfCurrentSchemaElement != null && gNodeOfCurrentSchemaElement.Parent == xe)
                {
                    gCurrentSchemaElement = gCurrentSchemaElement.Parent;
                    gNodeOfCurrentSchemaElement = xe;
                }

                xn = xe;
            }
        }
        #endregion

        #region Reset
        public void Reset()
        {
            gCurrentNode = gSourceElement;
            gCurrentSchemaElement = gSchemaElement;
            gNodeOfCurrentSchemaElement = gCurrentNode;
        }
        #endregion

        #region Descendant
        public XElement Descendant(XName name)
        {
            ZEnumNode en = new ZEnumNode(this);
            foreach (XNode node in en)
            {
                if (node is XElement)
                {
                    XElement xe = (XElement)node;
                    if (xe.Name == name)
                    {
                        gCurrentNode = en.Current;
                        if (en.NodeOfCurrentSchemaElement != null)
                        {
                            gNodeOfCurrentSchemaElement = en.NodeOfCurrentSchemaElement;
                            gCurrentSchemaElement = en.CurrentSchemaElement;
                        }
                        return xe;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Next
        public XElement Next(XName name)
        {
            ZEnumNode en = this.Clone();
            IEnumerator<XNode> enumerator = en.GetEnumerator();
            while (enumerator.MoveNext())
            {
                XNode node = enumerator.Current;
                if (node is XElement)
                {
                    XElement xe = (XElement)node;
                    if (xe.Name == name)
                    {
                        gCurrentNode = en.Current;
                        if (en.NodeOfCurrentSchemaElement != null)
                        {
                            gNodeOfCurrentSchemaElement = en.NodeOfCurrentSchemaElement;
                            gCurrentSchemaElement = en.CurrentSchemaElement;
                        }
                        return xe;
                    }
                }
            }
            return null;
        }
        #endregion

        #region FirstValue
        public string FirstValue(int IndexValue)
        {
            ZEnumNode nodes = new ZEnumNode(this);
            foreach (XNode node in nodes)
            {
                if (node is XText)
                {
                    string value = ((XText)node).Value;
                    value = ApplyTrimAndFilter(value);
                    if (value != null && value != "")
                    {
                        if (IndexValue-- == 0) return value;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Values
        public string Values()
        {
            //return ApplyTrimAndFilter(Values(new ZEnumNode(this)));
            return Values(new ZEnumNode(this));
        }
        #endregion

        #region AttribValue
        public string AttribValue(XName name)
        {
            if (!(gCurrentNode is XElement)) return null;
            return ((XElement)gCurrentNode).zAttribValue(name);
        }
        #endregion

        #region ApplyTrimAndFilter
        public string ApplyTrimAndFilter(string value)
        {
            if (value == null) return null;
            //if (gbTrimValue) value = value.Trim();
            if (gbTrimValue) value = zstr.RemoveMultipleSpace(value);
            if (gFilterValue != null)
            {
                Match match = gFilterValue.Match(value);
                if (match.Success)
                    value = match.Groups[1].Value;
            }
            return value;
        }
        #endregion

        #region Values
        public string Values(IEnumerable<XNode> nodes)
        {
            string s = null;
            foreach (XNode node in nodes)
            {
                if (node is XText)
                {
                    string s2 = ((XText)node).Value;
                    s2 = ApplyTrimAndFilter(s2);
                    if (s2 != "")
                    {
                        if (s == null)
                            s = s2;
                        else
                            s += " " + s2;
                    }
                }
            }
            return s;
        }
        #endregion
    }
    #endregion

    #region class ZSelect
    public class ZSelect
    {
        public string SchemaName = null;
        public string XPath = null;
        public ZSchema Schema = null;
    }
    #endregion

    #region class ZReadSchema
    public class ZReadSchema : IEnumerable<ZRead>, IEnumerator<ZRead>
    {
        #region variable
        private List<ZSelect> gSelects = new List<ZSelect>();
        private SortedList<string, ZSchema> gSchemas = new SortedList<string, ZSchema>();
        private XElement gXmlSource = null;
        private string gsSourcePath = null;
        private string gsXmlSourcePath = null;
        private SortedList<string, string> gConstants = null;
        private int giSelect = -1;
        private ZSelect gCurrentSelect = null;
        private IEnumerator<XElement> gEnumeratorSelectedElements = null;
        private int giResultIndex = 0;
        private ZRead gCurrentResult = null;
        private List<ZRead> gResults = new List<ZRead>();
        private bool gbExportNullValue = false;
        #endregion

        #region constructor
        public ZReadSchema(string pathSchema, string schemaName)
        {
            XDocument xd = XDocument.Load(pathSchema);
            XElement xe = (from s in xd.Root.Elements("schema") where s.zAttribValue("name") == schemaName select s).FirstOrDefault();
            if (xe == null) throw new Xml2Data_Exception("schema \"{0}\" doesn't exist in \"{1}\"", schemaName, pathSchema);
            Init(xe);
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
        }
        #endregion

        #region property ...
        #region Selects
        public List<ZSelect> Selects
        {
            get { return gSelects; }
        }
        #endregion

        #region Schemas
        public SortedList<string, ZSchema> Schemas
        {
            get { return gSchemas; }
        }
        #endregion

        #region Constants
        public SortedList<string, string> Constants
        {
            get { return gConstants; }
            set { gConstants = value; }
        }
        #endregion

        #region ResultIndex
        public int ResultIndex
        {
            get { return giResultIndex; }
        }
        #endregion

        #region Results
        public List<ZRead> Results
        {
            get { return gResults; }
        }
        #endregion

        #region ExportNullValue
        public bool ExportNullValue
        {
            get { return gbExportNullValue; }
            set { gbExportNullValue = value; }
        }
        #endregion
        #endregion

        #region Init
        public void Init(XElement xeSchema)
        {
            foreach (XElement xe in xeSchema.Elements("select"))
            {
                ZSelect sel = new ZSelect();
                sel.SchemaName = xe.zAttribValue("schema");
                sel.XPath = xe.zAttribValue("xpath");
                gSelects.Add(sel);
                int iSchema = gSchemas.IndexOfKey(sel.SchemaName);
                if (iSchema != -1)
                    sel.Schema = gSchemas.Values[iSchema];
                else
                {
                    XElement xeSchema2 = (from item in xeSchema.Elements("schema") where item.zAttribValue("name") == sel.SchemaName select item).FirstOrDefault();
                    if (xeSchema2 != null)
                    {
                        sel.Schema = ZSchema.CreateZSchema(xeSchema2);
                        gSchemas.Add(sel.SchemaName, sel.Schema);
                    }
                }
            }
        }
        #endregion

        #region SetSource(XDocument xmlSource, string sSourcePath, string sXmlSourcePath)
        public void SetSource(XDocument xmlSource, string sSourcePath, string sXmlSourcePath)
        {
            SetSource(xmlSource.Root, sSourcePath, sXmlSourcePath);
        }
        #endregion

        #region SetSource(XElement xmlSource, string sSourcePath, string sXmlSourcePath)
        public void SetSource(XElement xmlSource, string sSourcePath, string sXmlSourcePath)
        {
            gXmlSource = xmlSource;
            gsSourcePath = sSourcePath;
            gsXmlSourcePath = sXmlSourcePath;
            Reset();
        }
        #endregion

        #region Read(XDocument xmlSource, string sSourcePath, string sXmlSourcePath)
        public void Read(XDocument xmlSource, string sSourcePath, string sXmlSourcePath)
        {
            Read(xmlSource.Root, sSourcePath, sXmlSourcePath);
        }
        #endregion

        #region Read(XElement xmlSource, string sSourcePath, string sXmlSourcePath)
        public void Read(XElement xmlSource, string sSourcePath, string sXmlSourcePath)
        {
            SetSource(xmlSource, sSourcePath, sXmlSourcePath);
            foreach (ZRead zr in this)
                gResults.Add(zr);
        }
        #endregion

        #region ResultToXml
        public XDocument ResultToXml()
        {
            XDocument xd = new XDocument();
            xd.Add(new XElement("Results"));
            foreach (ZRead zr in gResults)
            {
                zr.ExportNullValue = gbExportNullValue;
                xd.Root.Add(zr.ResultToXml());
            }
            return xd;
        }
        #endregion

        #region SaveSourceFile
        public void SaveSourceFile(string sDir)
        {
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            if (!sDir.EndsWith("\\")) sDir += "\\";

            string sSourcePath = null;
            string sXmlSourcePath = null;
            int i = 1;
            int iSource = 1;
            foreach (ZRead zr in gResults)
            {
                bool bNewSource = false;
                if (sSourcePath != zr.SourcePath)
                {
                    sSourcePath = zr.SourcePath;
                    string sPath = sDir + string.Format("P{0:0000}_{1:0000}_Source{2}", iSource, i, zpath.PathGetExtension(sSourcePath));
                    File.Copy(sSourcePath, sPath);
                    bNewSource = true;
                }
                if (sXmlSourcePath != zr.XmlSourcePath)
                {
                    sXmlSourcePath = zr.XmlSourcePath;
                    string sPath = sDir + string.Format("P{0:0000}_{1:0000}_SourceXml{2}", iSource, i, zpath.PathGetExtension(sXmlSourcePath));
                    File.Copy(sXmlSourcePath, sPath);
                    bNewSource = true;
                }
                if (bNewSource) iSource++;
                i++;
            }
        }
        #endregion

        #region SaveXmlSource
        public void SaveXmlSource(string sDir)
        {
            if (!Directory.Exists(sDir)) Directory.CreateDirectory(sDir);
            if (!sDir.EndsWith("\\")) sDir += "\\";

            int i = 0;
            foreach (ZRead zr in gResults)
            {
                i++;
                string sPath = sDir + string.Format("XmlSource_{0:0000}.xml", i);
                zr.Source.Save(sPath);
            }
        }
        #endregion

        #region MessageToXml
        public XDocument MessageToXml()
        {
            XDocument xd = new XDocument();
            xd.Add(new XElement("Messages"));
            foreach (ZRead zr in gResults)
            {
                XElement xe = zr.MessageToXml();
                if (xe != null) xd.Root.Add(xe);
            }
            return xd;
        }
        #endregion

        #region GetEnumerator
        public IEnumerator<ZRead> GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerable.GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region Current
        public ZRead Current
        {
            get { return gCurrentResult; }
        }
        #endregion

        #region IEnumerator.Current
        object IEnumerator.Current
        {
            get { return gCurrentResult; }
        }
        #endregion

        #region MoveNext
        public bool MoveNext()
        {
            while (true)
            {
                if (gEnumeratorSelectedElements == null || !gEnumeratorSelectedElements.MoveNext())
                {
                    if (++giSelect >= gSelects.Count) return false;
                    gCurrentSelect = gSelects[giSelect];
                    IEnumerable<XElement> selectedElements = gXmlSource.XPathSelectElements(gCurrentSelect.XPath);
                    gEnumeratorSelectedElements = selectedElements.GetEnumerator();
                    if (!gEnumeratorSelectedElements.MoveNext()) return false;
                }
                XElement xe = gEnumeratorSelectedElements.Current;

                //if (gConstants != null && gConstants.ContainsKey("Index"))
                //    gConstants["Index"] = (giResultIndex + 1).ToString();
                if (gConstants == null) gConstants = new SortedList<string, string>();
                if (!gConstants.ContainsKey("Index")) gConstants.Add("Index", null);
                gConstants["Index"] = (giResultIndex + 1).ToString();

                gCurrentResult = ZRead.Read(gCurrentSelect.Schema, xe, gConstants);
                if (gCurrentResult == null) continue;
                giResultIndex++;
                gCurrentResult.SourcePath = gsSourcePath;
                gCurrentResult.XmlSourcePath = gsXmlSourcePath;
                return true;
            }
        }
        #endregion

        #region Reset
        public void Reset()
        {
            giSelect = -1;
            gEnumeratorSelectedElements = null;
            gCurrentResult = null;
        }
        #endregion
    }
    #endregion

    #region class ZReadGroupValues
    public class ZReadGroupValues
    {
        public ZSchemaGroupValues SchemaGroupValues = null;
        public List<ZReadValue> Values = new List<ZReadValue>();
        public ZReadGroupValues ParentValues = null;
        public List<ZReadGroupValues> ChildValues = new List<ZReadGroupValues>();
    }
    #endregion

    #region class ZReadValue
    public class ZReadValue
    {
        public string Name = null;
        public ZValue ValueDefinition = null;
        public ZReadGroupValues GroupValues = null;
        public ZReadGroupValues ChildGroupValues = null;
        public XElement Source = null;
        public string Value = null;
        public bool Attrib = false;
    }
    #endregion

    #region class ZSchema
    public class ZSchema
    {
        #region variable ...
        private string gsName = null;
        private bool gbControlElementNotFoundInSchema = false;
        private ZSchemaElement gRootElement = null; // arborescence du schema
        private ZSchemaGroupValues gRootValues = null; // arborescence des variables
        #endregion

        #region property ...
        #region Name
        public string Name
        {
            get { return gsName; }
            set { gsName = value; }
        }
        #endregion

        #region ControlElementNotFoundInSchema
        public bool ControlElementNotFoundInSchema
        {
            get { return gbControlElementNotFoundInSchema; }
            set { gbControlElementNotFoundInSchema = value; }
        }
        #endregion

        #region RootElement
        public ZSchemaElement RootElement
        {
            get { return gRootElement; }
        }
        #endregion

        #region RootValues
        public ZSchemaGroupValues RootValues
        {
            get { return gRootValues; }
        }
        #endregion
        #endregion

        #region CreateZSchema
        public static ZSchema CreateZSchema(XElement xeSchema)
        {
            // structure de xe
            // <schema name="company">
            //   <rootElement zname="schema element name" name="name" class="class" id="id" zoption="zoption">
            //      <Element zname="schema element name" name="name" class="class" id="id" zoption="zoption">
            //      </Element>
            //      <Element zname="schema element name" name="name" class="class" id="id" zoption="zoption">
            //          <Element zname="schema element name" name="name" class="class" id="id" zoption="zoption">
            //          </Element>
            //      </Element>
            //   </rootElement>
            // </schema>
            //
            // zoption : optional, exclude, MultipleChild

            if (xeSchema == null) return null;
            ZSchema schema = new ZSchema();
            schema.gsName = xeSchema.zAttribValue("name");
            string sOptions = xeSchema.zAttribValue("option");
            schema.SetOptions(sOptions);
            xeSchema = xeSchema.Elements().FirstOrDefault();
            schema.gRootValues = new ZSchemaGroupValues();
            schema.gRootElement = ZSchemaElement.CreateZSchemaElements(xeSchema, schema, schema.gRootValues);

            return schema;
        }
        #endregion

        #region SetOptions
        public void SetOptions(string sOptions)
        {
            if (sOptions == null) return;
            string[] sOptions2 = zsplit.Split(sOptions, ',', true);
            foreach (string s in sOptions2)
            {
                switch (s.ToLower())
                {
                    case "controlelementnotfoundinschema":
                        gbControlElementNotFoundInSchema = true;
                        break;
                }
            }
        }
        #endregion

        #region GetValues
        public IEnumerable<ZValue> GetValues()
        {
            return new ZSchemaEnumValue(this);
        }
        #endregion
    }
    #endregion

    #region class ZSchemaEnumValue
    public class ZSchemaEnumValue : IEnumerable<ZValue>, IEnumerator<ZValue>
    {
        private ZSchema gSchema = null;
        private ZValue gCurrentValue = null;

        #region constructor
        public ZSchemaEnumValue(ZSchema schema)
        {
            gSchema = schema;
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
        }
        #endregion

        #region GetEnumerator
        public IEnumerator<ZValue> GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerable.GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region Current
        public ZValue Current
        {
            get { return gCurrentValue; }
        }
        #endregion

        #region IEnumerator.Current
        object IEnumerator.Current
        {
            get { return gCurrentValue; }
        }
        #endregion

        #region MoveNext
        public bool MoveNext()
        {
            if (gCurrentValue == null)
            {
                if (gSchema.RootValues == null) return false;
                if (gSchema.RootValues.Values.Count == 0) return false;
                ZValue value = gSchema.RootValues.Values[0];
                while (true)
                {
                    //value.Hide
                    //value.NoValue
                }
            }
            return false;
        }
        #endregion

        #region Reset
        public void Reset()
        {
            gCurrentValue = null;
        }
        #endregion
    }
    #endregion

    #region class ZSchemaGroupValues
    public class ZSchemaGroupValues
    {
        public string Name = null;
        public bool ExportAsAttrib = false;
        public ZValue ValueDefinition = null;
        public ZSchemaElement SchemaElement = null;
        public List<ZValue> Values = new List<ZValue>();
        public ZSchemaGroupValues ParentGroupValues = null;
        public List<ZSchemaGroupValues> ChildsGroupValues = new List<ZSchemaGroupValues>();
    }
    #endregion

    #region class ZValue
    public class ZValue
    {
        #region variable ...
        private int giIndexValue = -1;
        private string gsName = null;
        private string gsType = null;
        private int giOrderNumber = 2000000000;
        private int giIndexTextValue = -1;
        private string gsElementName = null;
        private string gsAttribName = null;
        private string gsFilter = null;
        private Regex gFilter = null;
        private string gsLoad = null;
        private bool gbConcatValues = false;
        private bool gbMultiple = false; // permet de creer une variable pour chaque element présent dans la source, valable uniquement si gsElementName est défini
        private bool gbNoValue = false;
        private bool gbOptional = false;
        private bool gbHide = false;
        private bool gbConstant = false;
        private bool gbTextValue = false;
        private bool gbExportAsAttrib = false;

        private string gsValue = null;

        private ZSchemaElement gSchemaElement = null;
        private ZSchemaGroupValues gGroupValues = null;
        #endregion

        #region property ...
        #region IndexValue
        public int IndexValue
        {
            get { return giIndexValue; }
            set { giIndexValue = value; }
        }
        #endregion

        #region Name
        public string Name
        {
            get { return gsName; }
            set { gsName = value; }
        }
        #endregion

        #region Type
        public string Type
        {
            get { return gsType; }
            set { gsType = value; }
        }
        #endregion

        #region OrderNumber
        public int OrderNumber
        {
            get { return giOrderNumber; }
            set { giOrderNumber = value; }
        }
        #endregion

        #region IndexTextValue
        public int IndexTextValue
        {
            get { return giIndexTextValue; }
            set { giIndexTextValue = value; }
        }
        #endregion

        #region ElementName
        public string ElementName
        {
            get { return gsElementName; }
            set { gsElementName = value; }
        }
        #endregion

        #region AttribName
        public string AttribName
        {
            get { return gsAttribName; }
            set { gsAttribName = value; }
        }
        #endregion

        #region Multiple
        public bool Multiple
        {
            get { return gbMultiple; }
            set { gbMultiple = value; }
        }
        #endregion

        #region Filter
        public Regex Filter
        {
            get { return gFilter; }
            set { gFilter = value; }
        }
        #endregion

        #region Load
        public string Load
        {
            get { return gsLoad; }
            set { gsLoad = value; }
        }
        #endregion

        #region ConcatValues
        public bool ConcatValues
        {
            get { return gbConcatValues; }
            set { gbConcatValues = value; }
        }
        #endregion

        #region NoValue
        public bool NoValue
        {
            get { return gbNoValue; }
            set { gbNoValue = value; }
        }
        #endregion

        #region Optional
        public bool Optional
        {
            get { return gbOptional; }
            set { gbOptional = value; }
        }
        #endregion

        #region Hide
        public bool Hide
        {
            get { return gbHide; }
            set { gbHide = value; }
        }
        #endregion

        #region Constant
        public bool Constant
        {
            get { return gbConstant; }
            set { gbConstant = value; }
        }
        #endregion

        #region TextValue
        public bool TextValue
        {
            get { return gbTextValue; }
        }
        #endregion

        #region ExportAsAttrib
        public bool ExportAsAttrib
        {
            get { return gbExportAsAttrib; }
            set { gbExportAsAttrib = value; }
        }
        #endregion

        #region Value
        public string Value
        {
            get { return gsValue; }
            set { gsValue = value; }
        }
        #endregion

        #region SchemaElement
        public ZSchemaElement SchemaElement
        {
            get { return gSchemaElement; }
            set { gSchemaElement = value; }
        }
        #endregion

        #region GroupValues
        public ZSchemaGroupValues GroupValues
        {
            get { return gGroupValues; }
            set { gGroupValues = value; }
        }
        #endregion
        #endregion

        #region SetOptions
        public void SetOptions(string sOptions)
        {
            if (sOptions == null) return;
            string[] sOptions2 = zsplit.Split(sOptions, ',', true);
            foreach (string s in sOptions2)
            {
                switch (s.ToLower())
                {
                    case "concat":
                        gbConcatValues = true;
                        break;
                    case "multiple":
                        if (gsElementName == null) throw new Xml2Data_Exception("Une variable ne peut être \"multiple\" que si un élément est défini, {0}", gsName);
                        gbMultiple = true;
                        break;
                    case "novalue":
                        gbNoValue = true;
                        break;
                    case "optional":
                        gbOptional = true;
                        break;
                    case "hide":
                        gbHide = true;
                        break;
                    case "constant":
                        gbConstant = true;
                        break;
                    case "exportattrib":
                        gbExportAsAttrib = true;
                        break;
                }
            }
        }
        #endregion

        #region ReadValue
        public ZReadValue ReadValue(ZEnumNode source, ZReadGroupValues groupValues, SortedList<string, string> constants, bool nextValue)
        {
            ZReadValue value = new ZReadValue();
            value.Name = gsName;
            //value.GroupValues = GroupValues;
            value.ValueDefinition = this;
            value.Source = source.SourceElement;
            if (gbNoValue) return value;
            if (gbConstant)
            {
                if (constants != null)
                {
                    int i = constants.IndexOfKey(value.ValueDefinition.Name);
                    if (i != -1)
                        value.Value = constants.Values[i];
                }
                return value;
            }
            if (gbTextValue)
            {
                // pour optimiser la lecture les valeurs texte sont lues à l'avance
                value.Value = gsValue;
                return value;
            }
            if (gsElementName != null)
            {
                if (!nextValue)
                {
                    if (source.Descendant(gsElementName) == null) return value;
                }
                else
                {
                    if (source.Next(gsElementName) == null) return value;
                }
            }
            bool bTrimAndFilter = true;
            source.TrimValue = true;
            source.FilterValue = gFilter;
            if (gsAttribName != null)
            {
                value.Value = source.AttribValue(gsAttribName);
            }
            else if (gsLoad != null)
            {
                //w.wr.Print(gsLoad);
                foreach (ZReadValue value2 in groupValues.Values)
                {
                    if (value2.ValueDefinition.Name == "HoursInfoCode")
                    {
                        //string sUrl = "/trouverlesprofessionnels/infoHoraireAjax.do";
                        string sUrl = "http://www.pagesjaunes.fr/trouverlesprofessionnels/infoHoraireAjax.do";
                        //string sContent = "crypt=g4HT6Som0NKh1QaR7Z3GizLjmKU9dC6jxzcTih6Jvb5h3Ol+khOX/ajuovCR+DOk6tTRLgG1ioU1gCZdkWtKcw==";
                        string sContent = "crypt=" + value2.Value;

                        XDocument xl = WebRequest(sUrl, sContent);
                        value.Value = xl.Root.zValues();
                    }
                }

            }
            else
            {
                if (gbConcatValues)
                    value.Value = source.Values();
                else
                {
                    value.Value = source.FirstValue(giIndexTextValue);
                    bTrimAndFilter = false;
                }
            }
            if (bTrimAndFilter && value.Value != null)
            {
                //value.Value = value.Value.Trim();
                value.Value = zstr.RemoveMultipleSpace(value.Value);
                if (gFilter != null)
                {
                    Match match = gFilter.Match(value.Value);
                    if (match.Success)
                        value.Value = match.Groups[1].Value;
                }
            }
            return value;
        }
        #endregion

        #region WebRequest
        public static XLDocument WebRequest(string sUrl, string sContent)
        {
            //w.wr.Print("Load : {0}    {1}", sUrl, sContent);
            Trace.CurrentTrace.WriteLine("Load : POST {0}    {1}", sUrl, sContent);


            #region entete requete http
            // chargement des infos horaires
            //POST /trouverlesprofessionnels/infoHoraireAjax.do HTTP/1.1
            //Host: www.pagesjaunes.fr
            //Connection: keep-alive
            //User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.30 Safari/530.5
            //Referer: http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=059D94FB0E1D2E5E39DA6D7CE333BD19.yas07g?idContext=1408801085&portail=PJ
            //Content-Length: 94
            //Origin: http://www.pagesjaunes.fr
            //Content-Type: application/x-www-form-urlencoded
            //Accept: */*
            //Accept-Encoding: gzip,deflate,bzip2,sdch
            //Cookie: JSESSIONID=059D94FB0E1D2E5E39DA6D7CE333BD19.yas07g; RMID=513908194971ffb0; e=SXH-tsCoCh4AAFk97iY; VisitorID=XX-119190EF-14E32; myFormPref=S; ctr=1; rndNumber=15.067804977297783; crmseen=seen; crm_cookieEnabled=1; VisitorID=43124143876010160
            //Accept-Language: fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4
            //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
            //crypt=g4HT6Som0NKh1QaR7Z3GizLjmKU9dC6jxzcTih6Jvb5h3Ol+khOX/ajuovCR+DOk6tTRLgG1ioU1gCZdkWtKcw==



            //POST /trouverlesprofessionnels/infoHoraireAjax.do HTTP/1.1
            //Host: www.pagesjaunes.fr
            //Connection: keep-alive
            //User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/530.5 (KHTML, like Gecko) Chrome/2.0.172.33 Safari/530.5
            //Referer: http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheClassique.do;jsessionid=B885E74C457D793AD516DCA7611F57D3.yas03f?idContext=1591025492&portail=PJ
            //Content-Length: 82
            //Origin: http://www.pagesjaunes.fr
            //Content-Type: application/x-www-form-urlencoded
            //Accept: */*
            //Accept-Encoding: gzip,deflate,bzip2,sdch
            //Cookie: JSESSIONID=B885E74C457D793AD516DCA7611F57D3.yas03f; RMID=513908194971ffb0; e=SXH-tsCoCh4AAFk97iY; VisitorID=XX-119190EF-14E32; myFormPref=S; crmseen=seen; crm_cookieEnabled=1; ctr=1; VisitorID=43124143876010160; yesCodeLoc=L07505600
            //Accept-Language: fr-FR,fr;q=0.8,en-US;q=0.6,en;q=0.4
            //Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3
            //crypt=g4HT6Som0NKh1QaR7Z3Gi6OflvsH4rCT8Th0TSIYgS+2S7JsDxNQh2/8ThL6Ky0n8wubgGrxYAw=
            #endregion


            //afficherInfoHoraire('g4HT6Som0NKh1QaR7Z3GiwrdWMP8imqk7Gh539suglpMSWrfW4LfCu3cBeb5L0GGTFFALAJkRWcwIAFWMCikLw==', this);return false;
            //^afficherInfoHoraire\('(.*?)'


            // http://www.pagesjaunes.fr/trouverlesprofessionnels/rechercheAvance.do?idContext=373679288&portail=PJ
            Http http = new Http(sUrl);
            http.Method = HttpRequestMethod.Post;
            http.Referer = "http://www.pagesjaunes.fr/";
            http.RequestContentType = "application/x-www-form-urlencoded; charset=UTF-8"; // indispensable
            http.Content = sContent;
            //http.Cookies = w.wr.http.Cookies; attention ne pas mettre les cookies sinon impossible de charger la page suivante


            //////// $$pb refaire GetNewHtlmFileName(), il faut définir un répertoire de trace.
            ////////string sPath = w.wr.GetNewHtlmFileName(sUrl);
            ////////http.ExportPath = sPath;

            //XmlDocument xd = http.LoadXml();
            http.Load();
            XmlDocument xd = http.GetXmlDocumentResult();
            http.Dispose();
            XLDocument xl = XLDocument.Create(xd, sUrl);
            return xl;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            string s = gsName;
            if (gsType != null) s += " type=\"" + gsType + "\"";
            if (giIndexTextValue > 0) s += " index=" + giIndexTextValue.ToString();
            if (gsElementName != null) s += " element=\"" + gsElementName + "\"";
            if (gsAttribName != null) s += " attrib=\"" + gsAttribName + "\"";
            s += " parent element \"" + gSchemaElement.ElementName + "\"";
            return s;
        }
        #endregion

        #region CreateZValue
        public static ZValue CreateZValue(XElement xe)
        {
            ZValue value = new ZValue();
            value.gsName = xe.zAttribValue("name");
            value.gsType = xe.zAttribValue("type");
            string sOrderNumber = xe.zAttribValue("order");
            if (sOrderNumber != null)
            {
                int iOrderNumber;
                if (int.TryParse(sOrderNumber, out iOrderNumber))
                    value.giOrderNumber = iOrderNumber;
            }
            value.gsElementName = xe.zAttribValue("element");
            value.gsAttribName = xe.zAttribValue("attrib");
            string sOptions = xe.zAttribValue("option");
            value.gsFilter = xe.zAttribValue("filter");
            if (value.gsFilter != null)
                value.gFilter = new Regex(value.gsFilter, RegexOptions.Compiled | RegexOptions.Singleline);
            value.gsLoad = xe.zAttribValue("load");
            value.SetOptions(sOptions);
            value.gbTextValue = value.gsElementName == null && !value.gbConcatValues && !value.gbConstant && !value.gbNoValue && value.gsLoad == null;
            return value;
        }
        #endregion
    }
    #endregion

    #region class ZTextValues
    public class ZTextValues
    {
        public int LastIndexValue = -1;
        public int NbMandatoryValues = 0;
        public int NbOptionalValues = 0;
        public List<ZValue> Values = null;
        public List<bool> IndexValues = null;
    }
    #endregion

    #region class ZSchemaElement
    public class ZSchemaElement
    {
        #region variable ...
        private string gsElementType = null;
        private string gsElementName = null;
        private string gsName = null;
        private bool gbNameEqualsProcessed = false;
        private Regex gNameRegex = null;
        private string gsClass = null;
        private bool gbClassEqualsProcessed = false;
        private Regex gClassRegex = null;
        private string gsId = null;
        private bool gbIdEqualsProcessed = false;
        private Regex gIdRegex = null;
        private bool gbOptional = false;
        private bool gbMultiple = false;
        private bool gbMultipleChild = false;
        private bool gbExclude = false;

        private ZSchema gSchema = null;

        private ZSchemaElement gParent = null;
        private List<ZSchemaElement> gChilds = new List<ZSchemaElement>();
        private bool gbHasChildElement = false; // si true l'élément a des enfants qui ne sont pas du type "Exclude"
        private ZSchemaElement gPreviousElement = null;
        private ZSchemaElement gNextElement = null;

        private ZSchemaGroupValues gSchemaGroupValues = null;
        private List<ZValue> gValuesDefinition = new List<ZValue>();
        private ZTextValues gTextValues = null;
        #endregion

        #region property ...
        #region ElementType
        public string ElementType
        {
            get { return gsElementType; }
            set { gsElementType = value; }
        }
        #endregion

        #region ElementName
        public string ElementName
        {
            get { return gsElementName; }
            set { gsElementName = value; }
        }
        #endregion

        #region Name
        public string Name
        {
            get { return gsName; }
            set { gsName = value; }
        }
        #endregion

        #region Class
        public string Class
        {
            get { return gsClass; }
            set { gsClass = value; }
        }
        #endregion

        #region Id
        public string Id
        {
            get { return gsId; }
            set { gsId = value; }
        }
        #endregion

        #region Optional
        public bool Optional
        {
            get { return gbOptional; }
            set { gbOptional = value; }
        }
        #endregion

        #region Multiple
        public bool Multiple
        {
            get { return gbMultiple; }
            set { gbMultiple = value; }
        }
        #endregion

        #region MultipleChild
        public bool MultipleChild
        {
            get { return gbMultipleChild; }
            set { gbMultipleChild = value; }
        }
        #endregion

        #region Exclude
        public bool Exclude
        {
            get { return gbExclude; }
            set { gbExclude = value; }
        }
        #endregion

        #region Schema
        public ZSchema Schema
        {
            get { return gSchema; }
        }
        #endregion

        #region Parent
        public ZSchemaElement Parent
        {
            get { return gParent; }
        }
        #endregion

        #region Childs
        public List<ZSchemaElement> Childs
        {
            get { return gChilds; }
        }
        #endregion

        #region HasChildElement
        public bool HasChildElement
        {
            get { return gbHasChildElement; }
        }
        #endregion

        #region PreviousElement
        public ZSchemaElement PreviousElement
        {
            get { return gPreviousElement; }
        }
        #endregion

        #region NextElement
        public ZSchemaElement NextElement
        {
            get { return gNextElement; }
        }
        #endregion

        #region SchemaGroupValues
        public ZSchemaGroupValues SchemaGroupValues
        {
            get { return gSchemaGroupValues; }
        }
        #endregion

        #region ValuesDefinition
        public List<ZValue> ValuesDefinition
        {
            get { return gValuesDefinition; }
        }
        #endregion

        #region HasValue
        public bool HasValue
        {
            get { return gValuesDefinition.Count > 0; }
        }
        #endregion
        #endregion

        #region Add(ZSchemaElement ze)
        public void Add(ZSchemaElement ze)
        {
            ze.gParent = this;
            if (gChilds.Count > 0)
            {
                ZSchemaElement ze2 = gChilds[gChilds.Count - 1];
                ze2.gNextElement = ze;
                ze.gPreviousElement = ze2;
            }
            gChilds.Add(ze);
            if (!ze.Exclude) gbHasChildElement = true;
        }
        #endregion

        #region AddValues
        public ZSchemaGroupValues AddValues(XElement xe, ZSchemaGroupValues groupValues)
        {
            XElement xe2 = xe.Elements("values").FirstOrDefault();
            if (xe2 != null)
            {
                ZValue value = ZValue.CreateZValue(xe2);
                value.NoValue = true;
                value.SchemaElement = this;
                value.GroupValues = groupValues;
                groupValues.Values.Add(value);

                gSchemaGroupValues = new ZSchemaGroupValues();
                gSchemaGroupValues.ValueDefinition = value;
                gSchemaGroupValues.SchemaElement = this;
                gSchemaGroupValues.Name = value.Name;
                gSchemaGroupValues.ExportAsAttrib = value.ExportAsAttrib;
                gSchemaGroupValues.ParentGroupValues = groupValues;
                groupValues.ChildsGroupValues.Add(gSchemaGroupValues);

                groupValues = gSchemaGroupValues;
            }

            int iIndexTextValue = 0;
            int iIndexValue = 0;
            foreach (XElement xe3 in xe.Elements("value"))
            {
                ZValue value = ZValue.CreateZValue(xe3);
                value.IndexValue = iIndexValue++;
                if ((value.ElementName == null || value.AttribName == null) && !value.Constant && !value.NoValue)
                    value.IndexTextValue = iIndexTextValue++;
                value.SchemaElement = this;
                value.GroupValues = groupValues;
                groupValues.Values.Add(value);
                gValuesDefinition.Add(value);
            }

            gTextValues = GetZTextValues(gValuesDefinition);

            return groupValues;
        }
        #endregion

        #region GetZTextValues
        private static ZTextValues GetZTextValues(List<ZValue> values)
        {
            ZTextValues textValues = new ZTextValues();
            textValues.LastIndexValue = -1;
            textValues.NbMandatoryValues = 0;
            textValues.NbOptionalValues = 0;
            textValues.Values = new List<ZValue>();
            textValues.IndexValues = new List<bool>();
            foreach (ZValue value in values)
            {
                //if (value.ElementName != null || value.ConcatValues || value.Constant || value.NoValue) continue;
                //value.TextValue = true;
                if (!value.TextValue) continue;
                int index = value.IndexTextValue;
                if (index > textValues.LastIndexValue) textValues.LastIndexValue = index;
                if (value.Optional) textValues.NbOptionalValues++; else textValues.NbMandatoryValues++;
                for (int i = textValues.Values.Count; i <= index; i++) textValues.Values.Add(null);
                textValues.Values[index] = value;
                for (int i = textValues.IndexValues.Count; i <= index; i++) textValues.IndexValues.Add(false);
                textValues.IndexValues[index] = true;
            }
            return textValues;
        }
        #endregion

        #region SetOptions
        public void SetOptions(string sOptions)
        {
            if (sOptions == null) return;
            string[] sOptions2 = zsplit.Split(sOptions, ',', true);
            foreach (string s in sOptions2)
            {
                switch (s.ToLower())
                {
                    case "optional":
                        gbOptional = true;
                        break;
                    case "multiple":
                        gbMultiple = true;
                        break;
                    case "notmultiple":
                        gbMultiple = false;
                        break;
                    case "multiplechild":
                        gbMultipleChild = true;
                        break;
                    case "exclude":
                        gbExclude = true;
                        break;
                }
            }
        }
        #endregion

        #region ReadValues
        public ZReadGroupValues ReadValues(XElement source, SortedList<string, string> constants, ZReadGroupValues groupValues)
        {
            if (gSchemaGroupValues != null)
            {
                ZReadGroupValues groupValues2 = new ZReadGroupValues();
                groupValues2.ParentValues = groupValues;
                groupValues2.SchemaGroupValues = gSchemaGroupValues;
                groupValues.ChildValues.Add(groupValues2);

                ZReadValue value = new ZReadValue();
                value.Name = gSchemaGroupValues.Name;
                value.ValueDefinition = gSchemaGroupValues.ValueDefinition;
                value.Source = source;
                value.GroupValues = groupValues;
                value.ChildGroupValues = groupValues2;
                groupValues.Values.Add(value);

                groupValues = groupValues2;
            }

            if (gTextValues.LastIndexValue != -1)
            {
                // pour optimiser la lecture des valeurs texte
                List<string> textValues = ReadTextValues(gTextValues, new ZEnumNode(this, source));
                SetTextValues(textValues);
            }

            foreach (ZValue valueDefinition in gValuesDefinition)
            {
                ZEnumNode sourceEnum = new ZEnumNode(this, source);
                ZReadValue value = valueDefinition.ReadValue(sourceEnum, groupValues, constants, false);
                value.GroupValues = groupValues;
                groupValues.Values.Add(value);
                if (valueDefinition.Multiple)
                {
                    while (value.Value != null)
                    {
                        value = valueDefinition.ReadValue(sourceEnum, groupValues, constants, true);
                        value.GroupValues = groupValues;
                        groupValues.Values.Add(value);
                    }
                }
            }

            return groupValues;
        }
        #endregion

        #region ReadTextValues
        private static List<string> ReadTextValues(ZTextValues textValues, ZEnumNode nodes)
        {
            List<string> values = new List<string>();
            if (textValues.LastIndexValue == -1) return values;
            int i = 0;
            ZValue value = textValues.Values[i];
            bool bIndexValue = textValues.IndexValues[i];
            foreach (XNode node in nodes)
            {
                if (node is XText)
                {
                    string s = ((XText)node).Value;
                    s = ApplyTrimAndFilter(value, s);
                    if (s != null && s != "")
                    {
                        if (bIndexValue) values.Add(s);
                        if (++i > textValues.LastIndexValue) break;
                        value = textValues.Values[i];
                        bIndexValue = textValues.IndexValues[i];
                    }
                }
            }
            return values;
        }
        #endregion

        #region SetTextValues
        private void SetTextValues(List<string> textValues)
        {
            int nbValues = textValues.Count;
            int nbOptionalValues = nbValues - gTextValues.NbMandatoryValues;
            int i = 0;
            foreach (ZValue valueDefinition in gTextValues.Values)
            {
                valueDefinition.Value = null;
                if (valueDefinition == null) continue;
                if (i < nbValues)
                {
                    if (valueDefinition.Optional)
                    {
                        if (nbOptionalValues <= 0) continue;
                        nbOptionalValues--;
                    }
                    string sValue = textValues[i++];
                    valueDefinition.Value = sValue;
                }
            }
        }
        #endregion

        #region ApplyTrimAndFilter
        public static string ApplyTrimAndFilter(ZValue value, string s)
        {
            if (value == null) return s;
            if (s == null) return null;
            //s = s.Trim();
            s = zstr.RemoveMultipleSpace(s);
            if (value.Filter != null)
            {
                Match match = value.Filter.Match(s);
                if (match.Success)
                    s = match.Groups[1].Value;
            }
            return s;
        }
        #endregion

        #region GetSourceElement
        public XElement GetSourceElement(XElement source)
        {
            foreach (XElement xe in source.Elements())
            {
                if (Equals(xe))
                    return xe;
            }
            return null;
        }
        #endregion

        #region GetSchemaElement
        public ZSchemaElement GetSchemaElement(XElement source)
        {
            foreach (ZSchemaElement ze in gChilds)
            {
                if (ze.Equals(source))
                    return ze;
            }
            return null;
        }
        #endregion

        #region NameEquals
        public bool NameEquals(string sourceName)
        {
            if (gsName == null) return true;
            if (gsName == "" && (sourceName == "" || sourceName == null)) return true;
            if (sourceName == "" || sourceName == null) return false;
            if (!gbNameEqualsProcessed)
            {
                if (gsName.StartsWith("##"))
                    gNameRegex = new Regex(gsName.Substring(2, gsName.Length - 2), RegexOptions.Compiled);
                gbNameEqualsProcessed = true;
            }
            if (gNameRegex != null)
                return gNameRegex.IsMatch(sourceName);
            else
                return gsName.Equals(sourceName);
        }
        #endregion

        #region ClassEquals
        public bool ClassEquals(string sourceClass)
        {
            // modif du 05/11/2009 : si la class n'est pas spécifié (gsClass = null) alors pas de controle sur la class
            //if ((gsClass == null || gsClass == "") && (sourceClass == "" || sourceClass == null)) return true;
            if (gsClass == null || gsClass == "") return true;
            if (gsClass == "" && (sourceClass == "" || sourceClass == null)) return true;
            if (sourceClass == "" || sourceClass == null) return false;

            //if (gsClass == "*") return true;
            //if (gsClass == null) return false;
            if (!gbClassEqualsProcessed)
            {
                if (gsClass.StartsWith("##"))
                    gClassRegex = new Regex(gsClass.Substring(2, gsClass.Length - 2), RegexOptions.Compiled);
                gbClassEqualsProcessed = true;
            }
            if (gClassRegex != null)
                return gClassRegex.IsMatch(sourceClass);
            else
                return gsClass.Equals(sourceClass);
        }
        #endregion

        #region IdEquals
        public bool IdEquals(string sourceId)
        {
            if (gsId == null) return true;
            if (gsId == "" && (sourceId == "" || sourceId == null)) return true;
            if (sourceId == "" || sourceId == null) return false;
            if (!gbIdEqualsProcessed)
            {
                if (gsId.StartsWith("##"))
                    gIdRegex = new Regex(gsId.Substring(2, gsId.Length - 2), RegexOptions.Compiled);
                gbIdEqualsProcessed = true;
            }
            if (gIdRegex != null)
                return gIdRegex.IsMatch(sourceId);
            else
                return gsId.Equals(sourceId);
        }
        #endregion

        #region Equals
        public bool Equals(XElement sourceElement)
        {
            if (sourceElement.Name != gsElementType) return false;
            if (!ClassEquals(sourceElement.zAttribValue("class"))) return false;
            if (!NameEquals(sourceElement.zAttribValue("name"))) return false;
            if (!IdEquals(sourceElement.zAttribValue("id"))) return false;
            return true;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            string s;
            s = "ZSchemaElement ";
            s += gsElementType;
            if (gsElementName != null) s += " ZName=\"" + gsElementName + "\"";
            if (gsName != null) s += " Name=\"" + gsName + "\"";
            if (gsClass != null) s += " Class=\"" + gsClass + "\"";
            if (gsId != null) s += " Id=\"" + gsId + "\"";
            if (gValuesDefinition.Count > 0)
                s += " " + gValuesDefinition.zToStringValues();
            return s;
        }
        #endregion

        #region CreateZSchemaElement(XElement xe)
        public static ZSchemaElement CreateZSchemaElement(XElement xe)
        {
            ZSchemaElement ze = new ZSchemaElement();
            ze.gsElementType = xe.Name.LocalName;
            ze.gbMultiple = IsElementTypeMultiple(ze.gsElementType);
            ze.gsElementName = xe.zAttribValue("zname");
            string sOptions = xe.zAttribValue("zoption");
            ze.SetOptions(sOptions);
            ze.gsName = xe.zAttribValue("name");
            ze.gsId = xe.zAttribValue("id");
            ze.gsClass = xe.zAttribValue("class");
            return ze;
        }
        #endregion

        #region CreateZSchemaElements
        public static ZSchemaElement CreateZSchemaElements(XElement xeSchemaElement, ZSchema schema, ZSchemaGroupValues groupValues)
        {
            ZSchemaElement ze = ZSchemaElement.CreateZSchemaElement(xeSchemaElement);
            ze.gSchema = schema;
            groupValues = ze.AddValues(xeSchemaElement, groupValues);
            foreach (XElement xe in from e in xeSchemaElement.Elements() where e.Name != "value" && e.Name != "values" select e)
            {
                ze.Add(CreateZSchemaElements(xe, schema, groupValues));
            }
            return ze;
        }
        #endregion

        #region IsElementTypeMultiple
        public static bool IsElementTypeMultiple(string sElementType)
        {
            // li, dt, dd, tr, td
            //switch (sElementType.ToLower())
            //{
            //    case "li":
            //    case "dt":
            //    case "dd":
            //    case "tr":
            //    case "td":
            //        return true;
            //}
            return false;
        }
        #endregion
    }
    #endregion

    public static partial class XmlExtension
    {
        public static string zValues(this XElement xe)
        {
            if (xe == null) return null;
            string s = null;
            foreach (XNode node in xe.DescendantNodes())
            {
                if (node is XText)
                {
                    string s2 = ((XText)node).Value;
                    if (s == null)
                        s = s2;
                    else
                        s += " " + s2;
                }
            }
            return s;
        }
    }
}
