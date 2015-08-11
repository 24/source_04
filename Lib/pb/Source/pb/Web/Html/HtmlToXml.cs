using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace pb.Web
{
    public class XXXNode
    {
        public XmlNode XmlNode;
        public XNode XNode;
    }

    public class HtmlTable
    {
        public XXXNode Table;
        public XXXNode ColGroup;
        public XXXNode Col;
        public XXXNode Body;
        public XXXNode Row;
        public XXXNode Data;
    }

    public class HtmlToXml : IDisposable
    {
        // à faire :
        //  - comment gérer le PreserveWhitespace dans XDocument ?
        //  - les exceptions générées lors de la création d'un attribut sont capturées : mettre une option pour ne pas les capturées
        //  - pb dans TagBegin on appel HtmlTag.GetHtmlTag() et gCurrentTreeNode prend la valeur de gCurrentNode que si tag.EndBoundType != HtmlBoundTypeEnum.Forbidden
        //    mais dans TagEnd on recherche quand meme le parent.
        //  - pb le type de document <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
        //    est stocké sous forme de texte dans <head>
        //  - pb les attributs de <html>, <head>, <body> sont perdus
        //  - pb dans le xml les fin de ligne ne sont pas correctes
        //  - ajouter un paramètre pour supprimer ou pas les noeuds de texte ne contenant que des séparateurs IsSeparator() " \r\n\t"
        // $$pb


        private HtmlReader _htmlReader;
        private bool _generateXmlNodeOnly = false;  // si true ne crée pas les noeuds texte et commentaire
        private bool _normalizeXml = true;

        private bool _noTag = false;
        private bool _body = false;
        private bool _title = false;
        private Stack<HtmlTable> _tableStack = null;
        private HtmlTable _table = null;
        private bool _readCommentInText = false;

        private XmlDocument _xmlDocument = null;
        private XDocument _xDocument = null;
        private XXXNode _documentNode = null;

        private XXXNode _currentNode = null;
        private XXXNode _currentTreeNode = null;
        private XXXNode _htmlNode = null;
        private XXXNode _headNode = null;
        private XXXNode _bodyNode = null;
        private XXXNode _titleNode = null;
        private Stack<XXXNode> _definitionListStack = null;
        private XXXNode _definitionList = null;

        private static Regex _replace = new Regex(@"[/,;?@!<>\\\[\]\-\*\(\)\+\:\'" + "\\\"]", RegexOptions.Compiled);
        private static Regex _commentCorrection = new Regex("--+", RegexOptions.Compiled);

        public HtmlToXml(HtmlReader htmlReader)
        {
            _htmlReader = htmlReader;
        }

        public HtmlToXml(TextReader tr)
        {
            _htmlReader = new HtmlReader(tr);
        }

        public void Dispose()
        {
            if (_htmlReader != null)
            {
                _htmlReader.Dispose();
                _htmlReader = null;
            }
            _xmlDocument = null;
        }

        public bool GenerateXmlNodeOnly { get { return _generateXmlNodeOnly; } set { _generateXmlNodeOnly = value; } }
        public bool NormalizeXml { get { return _normalizeXml; } set { _normalizeXml = value; } }
        public bool ReadCommentInText { get { return _readCommentInText; } set { _readCommentInText = value; } }

        private XXXNode CreateElement(string name)
        {
            XXXNode node = new XXXNode();
            if (_xmlDocument != null)
            {
                XmlElement element = _xmlDocument.CreateElement(name);
                node.XmlNode = element;
            }
            if (_xDocument != null)
            {
                XElement element = new XElement(name);
                node.XNode = element;
            }
            return node;
        }

        private void AddElement(XXXNode parent, XXXNode child)
        {
            if (parent.XmlNode != null)
                parent.XmlNode.AppendChild(child.XmlNode);
            XNode xnode = parent.XNode;
            if (xnode != null)
            {
                if (xnode is XElement)
                    ((XElement)xnode).Add(child.XNode);
                else if (xnode is XDocument)
                    ((XDocument)xnode).Add(child.XNode);
                else
                    throw new PBException("error generating XDocument node is neither a XElement nor a XDocument");
            }
        }

        private void AddElement(XXXNode parent, string element)
        {
            if (parent.XmlNode != null)
            {
                XmlElement node = _xmlDocument.CreateElement(element);
                parent.XmlNode.AppendChild(node);
            }
            if (parent.XNode != null)
            {
                if (!(parent.XNode is XElement)) throw new PBException("error generating XDocument node is not a XElement");
                XElement node = new XElement(element);
                ((XElement)parent.XNode).Add(node);
            }
        }

        private void AddAttribute(XXXNode parent, string name, string value)
        {
            if (_xmlDocument != null)
            {
                if (parent.XmlNode.Attributes.GetNamedItem(name) == null)
                {
                    XmlAttribute attrib = _xmlDocument.CreateAttribute(name);
                    if (value == null) value = "";
                    attrib.Value = value;
                    parent.XmlNode.Attributes.Append(attrib);
                }
            }
            if (_xDocument != null)
            {
                XElement xeParent = (XElement)parent.XNode;
                if (xeParent.Attribute(name) == null)
                {
                    if (value == null) value = "";
                    XAttribute attrib = new XAttribute(name, value);
                    xeParent.Add(attrib);
                }
            }
        }

        private static bool IsSeparator(string text)
        {
            foreach (char c in text)
            {
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n') return false;
            }
            return true;
        }

        private void AddText(XXXNode parent, string text)
        {
            if (IsSeparator(text)) return;
            if (_xmlDocument != null)
            {
                XmlText node = _xmlDocument.CreateTextNode(text);
                parent.XmlNode.AppendChild(node);
            }
            if (_xDocument != null)
            {
                XText node = new XText(text);
                ((XElement)parent.XNode).Add(node);
            }
        }

        private void AddComment(XXXNode parent, string comment)
        {
            if (_xmlDocument != null)
            {
                XmlComment node = _xmlDocument.CreateComment(comment);
                parent.XmlNode.AppendChild(node);
            }
            if (_xDocument != null)
            {
                XComment node = new XComment(comment);
                ((XElement)parent.XNode).Add(node);
            }
        }

        private static XXXNode GetParentXXNode(XXXNode node)
        {
            XXXNode parentNode = new XXXNode();
            if (node.XmlNode != null) parentNode.XmlNode = node.XmlNode.ParentNode;
            if (node.XNode != null) parentNode.XNode = node.XNode.Parent;
            return parentNode;
        }

        private static XXXNode GetParentXXNodeByName(XXXNode node, string name)
        {
            bool found = false;
            XXXNode node2 = new XXXNode();

            XmlNode xmlNode = node.XmlNode;
            while (xmlNode != null)
            {
                if (xmlNode.Name == name)
                {
                    node2.XmlNode = xmlNode;
                    found = true;
                    break;
                }
                xmlNode = xmlNode.ParentNode;
            }

            if (node.XNode != null)
            {
                if (!(node.XNode is XElement)) throw new PBException("error generating XDocument node is not a XElement");
                XElement element = (XElement)node.XNode;
                while (element != null)
                {
                    if (element.Name == name)
                    {
                        node2.XNode = element;
                        found = true;
                        break;
                    }
                    element = element.Parent;
                }
            }

            if (found)
                return node2;
            else
                return null;
        }

        public XmlDocument GenerateXmlDocument()
        {
            _xmlDocument = new XmlDocument();
            _documentNode = new XXXNode();
            _documentNode.XmlNode = _xmlDocument;

            GenerateXml();
            return _xmlDocument;
        }

        public XDocument GenerateXDocument()
        {
            _xDocument = new XDocument();
            _documentNode = new XXXNode();
            _documentNode.XNode = _xDocument;

            GenerateXml();
            return _xDocument;
        }

        private void GenerateXml()
        {
            // gbNormalizeXml = true :
            //   - les tag html, head, title et body sont créés automatiquement
            //   - les tag html, head, title et body rencontrés ne sont pas pris en compte
            //   - seul les tag title et meta sont mis dans la partie head les autre tag sont mis dans la partie body
            //   - si un tag meta est placé après le début de la partie body, ce tag reste dans la partie body
            //   - seul le premier tag title est pris en compte et placé dans la partie head, les autre tag title ne sont pas pris en compte

            try
            {
                //cTrace.Trace("GenerateXml NewGenerateXml  : {0}", XmlConfig.CurrentConfig.Get("NewGenerateXml"));

                InitXml();

                _tableStack = new Stack<HtmlTable>();
                _table = null;

                _definitionListStack = new Stack<XXXNode>();
                _definitionList = null;

                _noTag = false;
                _body = false;
                _title = false;
                while (_htmlReader.Read())
                {
                    if (_htmlReader.IsText || _htmlReader.IsComment)
                    {
                        // $$pb modif le 11/01/2015
                        //if (_htmlReader.IsText && !_htmlReader.IsTextSeparator && !_body)
                        //if (_htmlReader.IsText && !_htmlReader.IsTextSeparator && !_htmlReader.IsScript && !_body)
                        //{
                        //    _body = true;
                        //    _currentNode = _currentTreeNode = _bodyNode;
                        //}
                        if (!_generateXmlNodeOnly)
                        {
                            if (_readCommentInText)
                            {
                                AddText(_currentNode, _htmlReader.Value);
                            }
                            else
                            {
                                if (_htmlReader.IsText)
                                {
                                    AddText(_currentNode, _htmlReader.Value);
                                }
                                else
                                {
                                    string s = _htmlReader.Value;
                                    s = _commentCorrection.Replace(s, "-");
                                    if (s.EndsWith("-")) s += " ";
                                    AddComment(_currentNode, s);
                                }
                            }
                        }
                    }
                    else if (_htmlReader.IsDocType)
                    {
                        AddAttribute(_htmlNode, "doctype", _htmlReader.DocType);
                    }
                    else if (_htmlReader.IsProperty)
                    {
                        if (_generateXmlNodeOnly || _noTag) continue;
                        try
                        {
                            string sPropertyName = _htmlReader.PropertyName;
                            //sPropertyName = sPropertyName.Replace("\"", "");
                            //sPropertyName = sPropertyName.Replace("/", "");
                            //sPropertyName = sPropertyName.Replace("\\", "");
                            //sPropertyName = sPropertyName.Replace("-", "");
                            //sPropertyName = sPropertyName.Replace(",", "");
                            sPropertyName = _replace.Replace(sPropertyName, "");
                            sPropertyName = sPropertyName.ToLower();
                            if (sPropertyName == "") sPropertyName = "__value";

                            // modif le 28/01/2014
                            //   hexadecimal value 0x03, is an invalid character
                            //   found in http://www.reseau-gesat.com/Gesat/Yvelines,78/Fontenay-le-Fleury,31443/esat-cotra,e1596/
                            //   <html><head><meta name="keywords" content="Conditionnement, travaux &amp;agrave; fa&amp;ccedil;onToutes activit&amp;eacute;s en entreprise Entretien et cr&amp;eacute;ation despaces verts" />
                            string propertyValue = _htmlReader.PropertyValue;
                            if (propertyValue != null)
                                propertyValue = propertyValue.Replace("\x03", "");
                            AddAttribute(_currentNode, sPropertyName, propertyValue);
                            if (_htmlReader.IsMarkBeginEnd) TagEnd(_htmlReader.MarkName.ToLower());
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("error in HtmlToXml.GenerateXml() :");
                            Trace.WriteLine(ex.Message);
                        }
                    }
                    else if (_htmlReader.IsMarkBeginEnd)
                    {
                        string sTagName = _htmlReader.MarkName.ToLower();
                        sTagName = _replace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagBegin(sTagName, true);
                    }
                    else if (_htmlReader.IsMarkBegin)
                    {
                        string sTagName = _htmlReader.MarkName.ToLower();
                        sTagName = _replace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagBegin(sTagName, false);
                    }
                    else if (_htmlReader.IsMarkEnd)
                    {
                        string sTagName = _htmlReader.MarkName.ToLower();
                        sTagName = _replace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagEnd(sTagName);
                    }
                }
            }
            finally
            {
                _htmlReader.Close();
            }
        }

        private void InitXml()
        {
            _htmlReader.ReadCommentInText = _readCommentInText;

            if (_xmlDocument != null)
            {
                if (!_generateXmlNodeOnly)
                    _xmlDocument.PreserveWhitespace = true;
                else
                    _xmlDocument.PreserveWhitespace = false;
            }

            // création du tag xml
            XXXNode element = CreateElement("xml");
            AddElement(_documentNode, element);
            _currentNode = _currentTreeNode = element;

            if (!_generateXmlNodeOnly)
                AddText(_currentNode, "\r\n");

            if (_normalizeXml)
            {
                // création du tag html
                element = CreateElement("html");
                AddElement(_currentTreeNode, element);
                _htmlNode = _currentNode = _currentTreeNode = element;

                if (!_generateXmlNodeOnly)
                    AddText(_currentNode, "\r\n");

                // création du tag head
                element = CreateElement("head");
                AddElement(_currentTreeNode, element);
                _headNode = _currentNode = element;

                if (!_generateXmlNodeOnly)
                    AddText(_currentNode, "\r\n");

                // création du tag title
                element = CreateElement("title");
                AddElement(_currentNode, element);
                _titleNode = _currentNode = element;

                // création du tag body
                element = CreateElement("body");
                AddElement(_currentTreeNode, element);
                _bodyNode = _currentNode = element;

                if (!_generateXmlNodeOnly)
                    AddText(_currentNode, "\r\n");

                _currentNode = _currentTreeNode = _headNode;
            }
        }

        private void TagBegin(string sTagName, bool bTagEnd)
        {
            _noTag = false;

            //sTagName = sTagName.Replace('-', '_');
            //sTagName = sTagName.Replace('!', '_');
            //sTagName = sTagName.Replace('[', '_');
            //sTagName = sTagName.Replace(']', '_');
            //sTagName = gReplace.Replace(sTagName, "_");
            //if (sTagName == "") sTagName = "_";
            HtmlTagType tagType = HtmlTags.GetHtmlTagType(sTagName);
            HtmlTag tag = HtmlTags.GetHtmlTag(tagType);
            if (_normalizeXml)
            {
                if (tagType == HtmlTagType.Html || tagType == HtmlTagType.Head)
                {
                    _noTag = true;
                    return;
                }
                if (tagType == HtmlTagType.Body)
                {
                    _noTag = true;
                    if (!_body)
                    {
                        _body = true;
                        _currentNode = _currentTreeNode = _bodyNode;
                    }
                    return;
                }
                if (tagType == HtmlTagType.Title)
                {
                    if (!_title)
                    {
                        if (!bTagEnd)
                        {
                            _title = true;
                            _currentNode = _titleNode;
                        }
                    }
                    else
                        _noTag = true;
                    return;
                }
                // $$pb modif le 11/01/2015
                //if (!_body && tag.TagCategory != HtmlTagCategory.Head)
                //{
                //    _body = true;
                //    _currentNode = _currentTreeNode = _bodyNode;
                //}
            }
            _currentNode = CreateElement(sTagName);
            if (_normalizeXml)
            {
                if (tagType == HtmlTagType.Table && !bTagEnd)
                {
                    if (_table != null) _tableStack.Push(_table);
                    _table = new HtmlTable();
                    _table.Table = _currentNode;
                    AddElement(_currentTreeNode, _currentNode);
                    _currentTreeNode = _currentNode;
                    return;
                }
                if (TagBeginTableCategory(tag, bTagEnd)) return;
                if (tagType == HtmlTagType.DL && !bTagEnd)
                {
                    if (_definitionList != null) _definitionListStack.Push(_definitionList);
                    _definitionList = _currentNode;
                    AddElement(_currentTreeNode, _currentNode);
                    _currentTreeNode = _currentNode;
                    return;
                }
                if (TagBeginDefinitionListCategory(tag, bTagEnd)) return;

                // $$pb à revérifier
                // il faut au moins annuler gLastPNode quand un des parents de gLastPNode se ferme
                //if (tagType == HtmlTagTypeEnum.P)
                //{
                //    // pour gérer une balise <p> qui n'a pas de fin de balise </p>
                //    if (gLastPNode != null)
                //    {
                //        gCurrentTreeNode = GetParentXXNode(gLastPNode);
                //        gLastPNode = null;
                //    }
                //    if (!bTagEnd) gLastPNode = gCurrentNode;
                //}

            }
            AddElement(_currentTreeNode, _currentNode);
            if (!bTagEnd && tag.EndBoundType != HtmlBoundType.Forbidden) _currentTreeNode = _currentNode;
        }

        private bool TagBeginTableCategory(HtmlTag tag, bool bTagEnd)
        {
            if (_table == null || tag.TagCategory != HtmlTagCategory.Table) return false;
            switch (tag.TagType)
            {
                case HtmlTagType.THead:
                case HtmlTagType.TBody:
                case HtmlTagType.TFoot:
                    AddElement(_table.Table, _currentNode);
                    if (!bTagEnd)
                    {
                        _table.Body = _currentNode;
                        _currentTreeNode = _currentNode;
                    }
                    else
                        _table.Body = null;
                    return true;
                case HtmlTagType.ColGroup:
                    AddElement(_table.Table, _currentNode);
                    if (!bTagEnd)
                    {
                        _table.ColGroup = _currentNode;
                        _currentTreeNode = _currentNode;
                    }
                    else
                        _table.ColGroup = null;
                    return true;
                case HtmlTagType.Col:
                    _currentTreeNode = _table.Table;
                    if (_table.ColGroup != null) _currentTreeNode = _table.ColGroup;
                    AddElement(_currentTreeNode, _currentNode);
                    if (!bTagEnd)
                    {
                        _table.Col = _currentNode;
                        _currentTreeNode = _currentNode;
                    }
                    else
                        _table.Col = null;
                    return true;
                case HtmlTagType.TR:
                    if (_table.Body == null)
                    {
                        _table.Body = CreateElement("tbody");
                        AddElement(_table.Table, _table.Body);
                    }
                    AddElement(_table.Body, _currentNode);
                    if (!bTagEnd)
                    {
                        _table.Row = _currentNode;
                        _currentTreeNode = _currentNode;
                    }
                    else
                        _table.Row = null;
                    return true;
                case HtmlTagType.TH:
                case HtmlTagType.TD:
                    if (_table.Row == null)
                    {
                        if (_table.Body == null)
                        {
                            _table.Body = CreateElement("tbody");
                            AddElement(_table.Table, _table.Body);
                        }
                        _table.Row = CreateElement("tr");
                        AddElement(_table.Body, _table.Row);
                    }
                    AddElement(_table.Row, _currentNode);
                    if (!bTagEnd)
                    {
                        _table.Data = _currentNode;
                        _currentTreeNode = _currentNode;
                    }
                    else
                        _table.Data = null;
                    return true;
            }
            return false;
        }

        private bool TagBeginDefinitionListCategory(HtmlTag tag, bool bTagEnd)
        {
            if (_definitionList == null || tag.TagCategory != HtmlTagCategory.DefinitionList) return false;
            switch (tag.TagType)
            {
                case HtmlTagType.DT:
                case HtmlTagType.DD:
                    AddElement(_definitionList, _currentNode);
                    if (!bTagEnd) _currentTreeNode = _currentNode;
                    return true;
            }
            return false;
        }

        private void TagEnd(string sTagName)
        {
            if (_normalizeXml)
            {
                HtmlTagType tagType = HtmlTags.GetHtmlTagType(sTagName);
                switch (tagType)
                {
                    case HtmlTagType.Html:
                    case HtmlTagType.Head:
                    case HtmlTagType.Body:
                        return;
                    case HtmlTagType.Title:
                        _currentNode = _currentTreeNode;
                        return;
                    case HtmlTagType.Table:
                        if (_table == null) return;
                        _currentNode = _currentTreeNode = GetParentXXNode(_table.Table);
                        _table = null;
                        if (_tableStack.Count != 0) _table = _tableStack.Pop();
                        return;
                    case HtmlTagType.DL:
                        if (_definitionList == null) return;
                        _currentNode = _currentTreeNode = GetParentXXNode(_definitionList);
                        _definitionList = null;
                        if (_definitionListStack.Count != 0) _definitionList = _definitionListStack.Pop();
                        return;
                }
                if (_table != null)
                {
                    switch (tagType)
                    {
                        case HtmlTagType.THead:
                        case HtmlTagType.TBody:
                        case HtmlTagType.TFoot:
                            _currentNode = _currentTreeNode = _table.Table;
                            _table.Body = null;
                            return;
                        case HtmlTagType.ColGroup:
                            _currentNode = _currentTreeNode = _table.Table;
                            _table.ColGroup = null;
                            return;
                        case HtmlTagType.Col:
                            if (_table.Col != null)
                            {
                                _currentNode = _currentTreeNode = GetParentXXNode(_table.Col);
                                _table.Col = null;
                            }
                            return;
                        case HtmlTagType.TR:
                            if (_table.Row != null)
                            {
                                _currentNode = _currentTreeNode = GetParentXXNode(_table.Row);
                                _table.Row = null;
                            }
                            return;
                        case HtmlTagType.TH:
                        case HtmlTagType.TD:
                            if (_table.Data != null)
                            {
                                _currentNode = _currentTreeNode = GetParentXXNode(_table.Data);
                                _table.Data = null;
                            }
                            return;
                    }
                }

            }
            XXXNode node = GetParentXXNodeByName(_currentTreeNode, sTagName);
            if (node != null)
                _currentTreeNode = GetParentXXNode(node);
            _currentNode = _currentTreeNode;
        }

        public static void Normalize(XmlDocument xml)
        {
            // ajout du tag tbody dans les tag table qui n'en ont pas
            XmlNodeList tables = xml.SelectNodes("//table");
            foreach (XmlNode table in tables)
            {
                NormalizeTable(table);
            }
        }

        private static void NormalizeTable(XmlNode table)
        {
            if (table.SelectSingleNode("./tbody") != null) return;
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node in table.ChildNodes)
            {
                if (node.Name == "caption") continue;
                if (node.Name == "thead") continue;
                if (node.Name == "tfoot") continue;
                if (node.Name == "col") continue;
                nodes.Add(node);
            }
            XmlElement tbody = table.OwnerDocument.CreateElement("tbody");
            table.AppendChild(tbody);
            foreach (XmlNode node in nodes)
            {
                table.RemoveChild(node);
                tbody.AppendChild(node);
            }
        }

        //public static HtmlXmlTables GetTables(XmlDocument xml)
        //{
        //    return new HtmlXmlTables(xml);
        //}

        //public static HtmlXmlTables GetTables(string[] tablesPath)
        //{
        //    return new HtmlXmlTables(tablesPath);
        //}
    }
}
