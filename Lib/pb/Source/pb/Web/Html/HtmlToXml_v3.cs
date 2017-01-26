using pb.Data.Xml;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace pb.Web.Html
{
    public class HtmlTable_v3
    {
        //public XNode Table;
        public XElement Table;
        public XElement ColGroup;
        public XElement Col;
        public XElement Body;
        public XElement Row;
        public XElement Data;
    }

    public class HtmlToXml_v3
    {
        private HtmlReader_v4 _htmlReader = null;
        private bool _generateXmlNodeOnly = false;  // si true ne crée pas les noeuds texte et commentaire
        private bool _normalizeXml = true;
        private bool _readCommentInText = false;

        private bool _noTag = false;
        private bool _body = false;
        private bool _title = false;
        private Stack<HtmlTable_v3> _tableStack = null;
        private HtmlTable_v3 _table = null;

        //private XDocument _xdocument = null;
        private XDocumentCreator _xdCreator = null;
        //private XNode _documentNode = null;
        //private XNode _currentNode = null;
        private XElement _currentNode = null;
        private XElement _currentTreeNode = null;
        private XElement _htmlNode = null;
        private XElement _headNode = null;
        private XElement _titleNode = null;
        private XElement _bodyNode = null;
        private Stack<XElement> _definitionListStack = null;
        private XElement _definitionList = null;

        // corect tag name and attribute name
        private static Regex _nameCorrection = new Regex(@"[/,;?@!<>\\\[\]\-\*\(\)\+\:\'" + "\\\"]", RegexOptions.Compiled);
        private static Regex _commentCorrection = new Regex("--+", RegexOptions.Compiled);

        public HtmlToXml_v3(HtmlReader_v4 htmlReader)
        {
            _htmlReader = htmlReader;
        }

        public bool GenerateXmlNodeOnly { get { return _generateXmlNodeOnly; } set { _generateXmlNodeOnly = value; } }
        public bool NormalizeXml { get { return _normalizeXml; } set { _normalizeXml = value; } }
        public bool ReadCommentInText { get { return _readCommentInText; } set { _readCommentInText = value; } }
        public IEnumerable<XLog> Log { get { return _xdCreator.Log; } }

        public XDocument CreateXml()
        {
            // ATTENTION HtmlReader_v4 dont manage ReadCommentInText
            //_htmlReader.ReadCommentInText = _readCommentInText;

            // need close tag
            //_htmlReader.GenerateCloseTag = true;
            //if (!_htmlReader.GenerateCloseTag)
            //    throw new PBException("html reader must have option GenerateCloseTag");

            //_xdocument = new XDocument();
            _xdCreator = new XDocumentCreator();
            //_documentNode = _xdocument;

            InitXml();

            _tableStack = new Stack<HtmlTable_v3>();
            _table = null;

            _definitionListStack = new Stack<XElement>();
            _definitionList = null;

            _noTag = false;
            _body = false;
            _title = false;

            foreach (HtmlNode htmlNode in _htmlReader.Read())
            {
                if (htmlNode.Type == HtmlNodeType.Text || htmlNode.Type == HtmlNodeType.Comment)
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
                            if (htmlNode.Type == HtmlNodeType.Text)
                                AddText(_currentNode, ((HtmlNodeText)htmlNode).Text);
                            else //if (htmlNode.Type == HtmlNodeType.Comment)
                                AddText(_currentNode, ((HtmlNodeComment)htmlNode).Comment);
                        }
                        else
                        {
                            if (htmlNode.Type == HtmlNodeType.Text)
                            {
                                AddText(_currentNode, ((HtmlNodeText)htmlNode).Text);
                            }
                            else //if (htmlNode.Type == HtmlNodeType.Comment)
                            {
                                string s = ((HtmlNodeComment)htmlNode).Comment;
                                s = _commentCorrection.Replace(s, "-");
                                if (s.EndsWith("-"))
                                    s += " ";
                                //AddComment(_currentNode, s);
                                _xdCreator.AddComment(_currentNode, s);
                            }
                        }
                    }
                }
                else if (htmlNode.Type == HtmlNodeType.Script)
                {
                    AddText(_currentNode, ((HtmlNodeScript)htmlNode).Script);
                }
                else if (htmlNode.Type == HtmlNodeType.DocumentType)
                {
                    //AddAttribute(_htmlNode, "doctype", ((HtmlNodeDocType)htmlNode).DocType);
                    _xdCreator.AddAttribute(_htmlNode, "doctype", ((HtmlNodeDocType)htmlNode).DocType);
                }
                else if (htmlNode.Type == HtmlNodeType.Property)
                {
                    if (_generateXmlNodeOnly || _noTag)
                        continue;
                    HtmlNodeProperty htmlNodeProperty = (HtmlNodeProperty)htmlNode;
                    try
                    {
                        string propertyName = htmlNodeProperty.Name;
                        propertyName = _nameCorrection.Replace(propertyName, "");
                        propertyName = propertyName.ToLower();
                        if (propertyName == "")
                            propertyName = "__value";

                        // modif le 28/01/2014
                        //   hexadecimal value 0x03, is an invalid character
                        //   found in http://www.reseau-gesat.com/Gesat/Yvelines,78/Fontenay-le-Fleury,31443/esat-cotra,e1596/
                        //   <html><head><meta name="keywords" content="Conditionnement, travaux &amp;agrave; fa&amp;ccedil;onToutes activit&amp;eacute;s en entreprise Entretien et cr&amp;eacute;ation despaces verts" />
                        string propertyValue = htmlNodeProperty.Value;
                        if (propertyValue != null)
                            propertyValue = propertyValue.Replace("\x03", "");
                        //AddAttribute(_currentNode, propertyName, propertyValue);
                        _xdCreator.AddAttribute(_currentNode, propertyName, propertyValue);
                        //if (_htmlReader.IsMarkBeginEnd)
                        //    TagEnd(_htmlReader.MarkName.ToLower());
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"error in HtmlToXml_v2.CreateXml() : line {htmlNode.Line} column {htmlNode.Column}");
                        Trace.WriteLine(ex.Message);
                    }
                }
                //else if (_htmlReader.IsMarkBeginEnd)
                //{
                //    string tagName = _htmlReader.MarkName.ToLower();
                //    tagName = _replace.Replace(tagName, "_");
                //    if (tagName == "") tagName = "_";

                //    TagBegin(tagName, true);
                //}
                //else if (_htmlReader.IsMarkBegin)
                else if (htmlNode.Type == HtmlNodeType.OpenTag)
                {
                    HtmlNodeOpenTag htmlNodeOpenTag = (HtmlNodeOpenTag)htmlNode;
                    string tagName = htmlNodeOpenTag.Name.ToLower();
                    tagName = _nameCorrection.Replace(tagName, "_");
                    if (tagName == "")
                        tagName = "_";

                    //TagBegin(tagName, false);
                    AddTagBegin(tagName);
                }
                //else if (htmlNode.Type == HtmlNodeType.CloseTag)
                //{
                //    HtmlNodeCloseTag htmlNodeCloseTag = (HtmlNodeCloseTag)htmlNode;
                //    string tagName = htmlNodeCloseTag.Name.ToLower();
                //    tagName = _nameCorrection.Replace(tagName, "_");
                //    if (tagName == "")
                //        tagName = "_";
                //    TagEnd(tagName);
                //}
                //else if (_htmlReader.IsMarkEnd)
                else if (htmlNode.Type == HtmlNodeType.EndTag)
                {
                    HtmlNodeEndTag htmlNodeEndTag = (HtmlNodeEndTag)htmlNode;
                    string tagName = htmlNodeEndTag.Name.ToLower();
                    tagName = _nameCorrection.Replace(tagName, "_");
                    if (tagName == "")
                        tagName = "_";
                    TagEnd(tagName);
                }
            }

            //return _xdocument;
            return _xdCreator.XDocument;
        }

        private void InitXml()
        {
            // création du tag xml
            //XNode element = CreateElement("xml");
            //AddElement(_documentNode, element);
            //_currentNode = _currentTreeNode = element;
            _currentNode = _currentTreeNode = _xdCreator.AddRootElement("xml");

            // inutile car AddText() n'ajoute le texte que si ce n'est pas un séparateur
            //if (!_generateXmlNodeOnly)
            //    AddText(_currentNode, "\r\n");

            if (_normalizeXml)
            {
                // création du tag html
                //element = CreateElement("html");
                //AddElement(_currentTreeNode, element);
                //_htmlNode = _currentNode = _currentTreeNode = element;
                _htmlNode = _currentNode = _currentTreeNode = _xdCreator.AddElement(_currentTreeNode, "html");

                // inutile car AddText() n'ajoute le texte que si ce n'est pas un séparateur
                //if (!_generateXmlNodeOnly)
                //    AddText(_currentNode, "\r\n");

                // création du tag head
                //element = CreateElement("head");
                //AddElement(_currentTreeNode, element);
                //_headNode = _currentNode = element;
                _headNode = _currentNode = _xdCreator.AddElement(_currentTreeNode, "head");

                // inutile car AddText() n'ajoute le texte que si ce n'est pas un séparateur
                //if (!_generateXmlNodeOnly)
                //    AddText(_currentNode, "\r\n");

                // création du tag title
                //element = CreateElement("title");
                //AddElement(_currentNode, element);
                //_titleNode = _currentNode = element;
                _titleNode = _xdCreator.AddElement(_currentNode, "title");

                // création du tag body
                //element = CreateElement("body");
                //AddElement(_currentTreeNode, element);
                //_bodyNode = _currentNode = element;
                _bodyNode = _currentNode = _xdCreator.AddElement(_currentTreeNode, "body");

                // inutile car AddText() n'ajoute le texte que si ce n'est pas un séparateur
                //if (!_generateXmlNodeOnly)
                //    AddText(_currentNode, "\r\n");

                _currentNode = _currentTreeNode = _headNode;
            }
        }

        //private XNode CreateElement(string name)
        //{
        //    return new XElement(name);
        //}

        //private static void AddElement(XNode parent, XNode child)
        //{
        //    if (parent is XElement)
        //        ((XElement)parent).Add(child);
        //    else if (parent is XDocument)
        //        ((XDocument)parent).Add(child);
        //    else
        //        throw new PBException("error generating XDocument node is neither a XElement nor a XDocument");
        //}

        //private static void AddAttribute(XNode parent, string name, string value)
        //{
        //    XElement xeParent = (XElement)parent;
        //    if (xeParent.Attribute(name) == null)
        //    {
        //        if (value == null)
        //            value = "";
        //        XAttribute attrib = new XAttribute(name, value);
        //        xeParent.Add(attrib);
        //    }
        //}

        //private static void AddText(XNode parent, string text)
        //{
        //    if (IsSeparator(text))
        //        return;
        //    ((XElement)parent).Add(new XText(text));
        //}

        private void AddText(XElement parent, string text)
        {
            if (IsSeparator(text))
                return;
            _xdCreator.AddText(parent, text);
        }

        //private static void AddComment(XNode parent, string comment)
        //{
        //    ((XElement)parent).Add(new XComment(comment));
        //}

        private static bool IsSeparator(string text)
        {
            foreach (char c in text)
            {
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                    return false;
            }
            return true;
        }

        // bool bTagEnd
        private void AddTagBegin(string tagName)
        {
            _noTag = false;

            HtmlTagType tagType = HtmlTags.GetHtmlTagType(tagName);
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
                        //if (!bTagEnd)
                        //{
                            _title = true;
                            _currentNode = _titleNode;
                        //}
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
            //_currentNode = CreateElement(tagName);
            if (_normalizeXml)
            {
                //if (tagType == HtmlTagType.Table && !bTagEnd)
                if (tagType == HtmlTagType.Table)
                {
                    //AddElement(_currentTreeNode, _currentNode);
                    //_currentTreeNode = _currentNode;
                    _currentTreeNode = _currentNode = _xdCreator.AddElement(_currentTreeNode, tagName);
                    if (_table != null)
                        _tableStack.Push(_table);
                    _table = new HtmlTable_v3();
                    _table.Table = _currentNode;
                    return;
                }
                //if (TagBeginTableCategory(tag, bTagEnd))
                if (TagBeginTableCategory(tag, tagName))
                    return;
                //if (tagType == HtmlTagType.DL && !bTagEnd)
                if (tagType == HtmlTagType.DL)
                {
                    //AddElement(_currentTreeNode, _currentNode);
                    //_currentTreeNode = _currentNode;
                    _currentTreeNode = _currentNode = _xdCreator.AddElement(_currentTreeNode, tagName);
                    if (_definitionList != null)
                        _definitionListStack.Push(_definitionList);
                    _definitionList = _currentNode;
                    return;
                }
                //if (TagBeginDefinitionListCategory(tag, bTagEnd))
                if (TagBeginDefinitionListCategory(tag, tagName))
                    return;

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
            //AddElement(_currentTreeNode, _currentNode);
            _currentNode = _xdCreator.AddElement(_currentTreeNode, tagName);
            //if (!bTagEnd && tag.EndBoundType != HtmlBoundType.Forbidden)
            if (tag.EndBoundType != HtmlBoundType.Forbidden)
                _currentTreeNode = _currentNode;
        }

        private void TagEnd(string tagName)
        {
            if (_normalizeXml)
            {
                HtmlTagType tagType = HtmlTags.GetHtmlTagType(tagName);
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
                        if (_table == null)
                            return;
                        //_currentNode = _currentTreeNode = GetParentXXNode(_table.Table);
                        _currentNode = _currentTreeNode = _table.Table.Parent;
                        _table = null;
                        if (_tableStack.Count != 0)
                            _table = _tableStack.Pop();
                        return;
                    case HtmlTagType.DL:
                        if (_definitionList == null)
                            return;
                        //_currentNode = _currentTreeNode = GetParentXXNode(_definitionList);
                        _currentNode = _currentTreeNode = _definitionList.Parent;
                        _definitionList = null;
                        if (_definitionListStack.Count != 0)
                            _definitionList = _definitionListStack.Pop();
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
                                //_currentNode = _currentTreeNode = GetParentXXNode(_table.Col);
                                _currentNode = _currentTreeNode = _table.Col.Parent;
                                _table.Col = null;
                            }
                            return;
                        case HtmlTagType.TR:
                            if (_table.Row != null)
                            {
                                //_currentNode = _currentTreeNode = GetParentXXNode(_table.Row);
                                _currentNode = _currentTreeNode = _table.Row.Parent;
                                _table.Row = null;
                            }
                            return;
                        case HtmlTagType.TH:
                        case HtmlTagType.TD:
                            if (_table.Data != null)
                            {
                                //_currentNode = _currentTreeNode = GetParentXXNode(_table.Data);
                                _currentNode = _currentTreeNode = _table.Data.Parent;
                                _table.Data = null;
                            }
                            return;
                    }
                }

            }
            XNode node = GetParentNodeByName(_currentTreeNode, tagName);
            if (node != null)
                //_currentTreeNode = GetParentXXNode(node);
                _currentTreeNode = node.Parent;
            _currentNode = _currentTreeNode;
        }

        //private static XXXNode GetParentXXNodeByName(XXXNode node, string name)
        private static XNode GetParentNodeByName(XNode node, string name)
        {
            //bool found = false;
            //XXXNode node2 = new XXXNode();
            XNode node2 = null;

            //XmlNode xmlNode = node.XmlNode;
            //while (xmlNode != null)
            //{
            //    if (xmlNode.Name == name)
            //    {
            //        node2.XmlNode = xmlNode;
            //        found = true;
            //        break;
            //    }
            //    xmlNode = xmlNode.ParentNode;
            //}

            //if (node.XNode != null)
            //{
            if (!(node is XElement))
                throw new PBException("error generating XDocument node is not a XElement");
            XElement element = (XElement)node;
            while (element != null)
            {
                if (element.Name == name)
                {
                    node2 = element;
                    //found = true;
                    break;
                }
                element = element.Parent;
            }
            //}

            //if (found)
            //    return node2;
            //else
            //    return null;
            return node2;
        }

        //private static XNode GetParentXXNode(XNode node)
        //{
        //    //XXXNode parentNode = new XXXNode();
        //    //if (node.XmlNode != null) parentNode.XmlNode = node.XmlNode.ParentNode;
        //    //if (node.XNode != null) parentNode.XNode = node.XNode.Parent;
        //    //return parentNode;
        //    return node.Parent;
        //}

        // bool bTagEnd
        private bool TagBeginTableCategory(HtmlTag tag, string tagName)
        {
            if (_table == null || tag.TagCategory != HtmlTagCategory.Table)
                return false;
            switch (tag.TagType)
            {
                case HtmlTagType.THead:
                case HtmlTagType.TBody:
                case HtmlTagType.TFoot:
                    //AddElement(_table.Table, _currentNode);
                    _currentNode = _xdCreator.AddElement(_table.Table, tagName);
                    //if (!bTagEnd)
                    //{
                    _table.Body = _currentNode;
                    _currentTreeNode = _currentNode;
                    //}
                    //else
                    //    _table.Body = null;
                    return true;
                case HtmlTagType.ColGroup:
                    //AddElement(_table.Table, _currentNode);
                    _currentNode = _xdCreator.AddElement(_table.Table, tagName);
                    //if (!bTagEnd)
                    //{
                    _table.ColGroup = _currentNode;
                    _currentTreeNode = _currentNode;
                    //}
                    //else
                    //    _table.ColGroup = null;
                    return true;
                case HtmlTagType.Col:
                    _currentTreeNode = _table.Table;
                    if (_table.ColGroup != null)
                        _currentTreeNode = _table.ColGroup;
                    //AddElement(_currentTreeNode, _currentNode);
                    _currentNode = _xdCreator.AddElement(_currentTreeNode, tagName);
                    //if (!bTagEnd)
                    //{
                    _table.Col = _currentNode;
                    _currentTreeNode = _currentNode;
                    //}
                    //else
                    //    _table.Col = null;
                    return true;
                case HtmlTagType.TR:
                    if (_table.Body == null)
                    {
                        //_table.Body = CreateElement("tbody");
                        //AddElement(_table.Table, _table.Body);
                        _table.Body = _xdCreator.AddElement(_table.Table, "tbody");
                    }
                    //AddElement(_table.Body, _currentNode);
                    _currentNode = _xdCreator.AddElement(_table.Body, tagName);
                    //if (!bTagEnd)
                    //{
                    _table.Row = _currentNode;
                    _currentTreeNode = _currentNode;
                    //}
                    //else
                    //    _table.Row = null;
                    return true;
                case HtmlTagType.TH:
                case HtmlTagType.TD:
                    if (_table.Row == null)
                    {
                        if (_table.Body == null)
                        {
                            //_table.Body = CreateElement("tbody");
                            //AddElement(_table.Table, _table.Body);
                            _table.Body = _xdCreator.AddElement(_table.Table, "tbody");
                        }
                        //_table.Row = CreateElement("tr");
                        //AddElement(_table.Body, _table.Row);
                        _table.Row = _xdCreator.AddElement(_table.Body, "tr");
                    }
                    //AddElement(_table.Row, _currentNode);
                    _currentNode = _xdCreator.AddElement(_table.Row, tagName);
                    //if (!bTagEnd)
                    //{
                    _table.Data = _currentNode;
                    _currentTreeNode = _currentNode;
                    //}
                    //else
                    //    _table.Data = null;
                    return true;
            }
            return false;
        }

        // bool bTagEnd
        private bool TagBeginDefinitionListCategory(HtmlTag tag, string tagName)
        {
            if (_definitionList == null || tag.TagCategory != HtmlTagCategory.DefinitionList)
                return false;
            switch (tag.TagType)
            {
                case HtmlTagType.DT:
                case HtmlTagType.DD:
                    //AddElement(_definitionList, _currentNode);
                    _currentNode = _xdCreator.AddElement(_definitionList, tagName);
                    //if (!bTagEnd)
                    _currentTreeNode = _currentNode;
                    return true;
            }
            return false;
        }
    }
}
