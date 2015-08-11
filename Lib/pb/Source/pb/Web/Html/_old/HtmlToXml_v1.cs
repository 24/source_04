using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace pb.Web
{
    public class HtmlToXml_v1 : IDisposable
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


        #region variable
        HtmlReader gHTMLReader;
        private bool gbGenerateXmlNodeOnly = false;  // si true ne crée pas les noeuds texte et commentaire
        private bool gbNormalizeXml = true;

        private bool gbNoTag = false;
        private bool gbBody = false;
        private bool gbTitle = false;
        private Stack<HtmlTable> gTableStack = null;
        private HtmlTable gTable = null;
        //private XXNode gLastPNode = null;
        private bool gbReadCommentInText = false;

        private XmlDocument gXmlDocument = null;
        private XDocument gXDocument = null;
        private XXXNode gDocumentNode = null;

        private XXXNode gCurrentNode = null;
        private XXXNode gCurrentTreeNode = null;
        private XXXNode gHtmlNode = null;
        private XXXNode gHeadNode = null;
        private XXXNode gBodyNode = null;
        private XXXNode gTitleNode = null;
        private Stack<XXXNode> gDefinitionListStack = null;
        private XXXNode gDefinitionList = null;

        private static Regex gReplace = new Regex(@"[/,;?@!<>\\\[\]\-\*\(\)\+\:\'" + "\\\"]", RegexOptions.Compiled);
        private static Regex gCommentCorrection = new Regex("--+", RegexOptions.Compiled);
        #endregion

        #region constructor
        #region HtmlXml(HTMLReader HTMLReader)
        public HtmlToXml_v1(HtmlReader HTMLReader)
        {
            gHTMLReader = HTMLReader;
        }
        #endregion

        #region HtmlXml(string sUrl_Path)
        //public HtmlXml(string sUrl_Path)
        //{
        //    gHTMLReader = new HtmlReader(sUrl_Path);
        //}
        #endregion

        #region HtmlXml(TextReader tr)
        public HtmlToXml_v1(TextReader tr)
        {
            gHTMLReader = new HtmlReader(tr);
        }
        #endregion
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (gHTMLReader != null)
            {
                gHTMLReader.Dispose();
                gHTMLReader = null;
            }
            gXmlDocument = null;
        }
        #endregion

        #region property ...
        #region GenerateXmlNodeOnly
        public bool GenerateXmlNodeOnly
        {
            get { return gbGenerateXmlNodeOnly; }
            set { gbGenerateXmlNodeOnly = value; }
        }
        #endregion

        #region NormalizeXml
        public bool NormalizeXml
        {
            get { return gbNormalizeXml; }
            set { gbNormalizeXml = value; }
        }
        #endregion

        #region ReadCommentInText
        public bool ReadCommentInText
        {
            get { return gbReadCommentInText; }
            set { gbReadCommentInText = value; }
        }
        #endregion
        #endregion

        #region CreateElement
        private XXXNode CreateElement(string name)
        {
            XXXNode node = new XXXNode();
            if (gXmlDocument != null)
            {
                XmlElement element = gXmlDocument.CreateElement(name);
                node.XmlNode = element;
            }
            if (gXDocument != null)
            {
                XElement element = new XElement(name);
                node.XNode = element;
            }
            return node;
        }
        #endregion

        #region AddElement
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
        #endregion

        #region AddElement
        private void AddElement(XXXNode parent, string element)
        {
            if (parent.XmlNode != null)
            {
                XmlElement node = gXmlDocument.CreateElement(element);
                parent.XmlNode.AppendChild(node);
            }
            if (parent.XNode != null)
            {
                if (!(parent.XNode is XElement)) throw new PBException("error generating XDocument node is not a XElement");
                XElement node = new XElement(element);
                ((XElement)parent.XNode).Add(node);
            }
        }
        #endregion

        #region AddAttribute
        private void AddAttribute(XXXNode parent, string name, string value)
        {
            if (gXmlDocument != null)
            {
                if (parent.XmlNode.Attributes.GetNamedItem(name) == null)
                {
                    XmlAttribute attrib = gXmlDocument.CreateAttribute(name);
                    if (value == null) value = "";
                    attrib.Value = value;
                    parent.XmlNode.Attributes.Append(attrib);
                }
            }
            if (gXDocument != null)
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
        #endregion

        #region IsSeparator
        private static bool IsSeparator(string text)
        {
            foreach (char c in text)
            {
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n') return false;
            }
            return true;
        }
        #endregion

        #region AddText
        private void AddText(XXXNode parent, string text)
        {
            if (IsSeparator(text)) return;
            if (gXmlDocument != null)
            {
                //XmlText node = gXmlDocument.CreateTextNode("text");
                //node.Value = text;
                XmlText node = gXmlDocument.CreateTextNode(text);
                parent.XmlNode.AppendChild(node);
            }
            if (gXDocument != null)
            {
                XText node = new XText(text);
                ((XElement)parent.XNode).Add(node);
            }
        }
        #endregion

        #region AddComment
        private void AddComment(XXXNode parent, string comment)
        {
            if (gXmlDocument != null)
            {
                XmlComment node = gXmlDocument.CreateComment(comment);
                parent.XmlNode.AppendChild(node);
            }
            if (gXDocument != null)
            {
                XComment node = new XComment(comment);
                ((XElement)parent.XNode).Add(node);
            }
        }
        #endregion

        #region GetParentXXNode
        private static XXXNode GetParentXXNode(XXXNode node)
        {
            XXXNode parentNode = new XXXNode();
            if (node.XmlNode != null) parentNode.XmlNode = node.XmlNode.ParentNode;
            if (node.XNode != null) parentNode.XNode = node.XNode.Parent;
            return parentNode;
        }
        #endregion

        #region GetParentXXNodeByName
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
        #endregion

        #region GenerateXmlDocument
        public XmlDocument GenerateXmlDocument()
        {
            gXmlDocument = new XmlDocument();
            gDocumentNode = new XXXNode();
            gDocumentNode.XmlNode = gXmlDocument;

            GenerateXml();
            return gXmlDocument;
        }
        #endregion

        #region GenerateXDocument
        public XDocument GenerateXDocument()
        {
            gXDocument = new XDocument();
            gDocumentNode = new XXXNode();
            gDocumentNode.XNode = gXDocument;

            GenerateXml();
            return gXDocument;
        }
        #endregion

        #region GenerateXml
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

                gTableStack = new Stack<HtmlTable>();
                gTable = null;

                gDefinitionListStack = new Stack<XXXNode>();
                gDefinitionList = null;

                gbNoTag = false;
                gbBody = false;
                gbTitle = false;
                while (gHTMLReader.Read())
                {
                    if (gHTMLReader.IsText || gHTMLReader.IsComment)
                    {
                        if (gHTMLReader.IsText && !gHTMLReader.IsTextSeparator && !gbBody)
                        {
                            gbBody = true;
                            gCurrentNode = gCurrentTreeNode = gBodyNode;
                        }
                        if (!gbGenerateXmlNodeOnly)
                        {
                            //if (string.Compare(XmlConfig.CurrentConfig.Get("NewGenerateXml"), "true", true) != 0)
                            if (gbReadCommentInText)
                            {
                                //XmlText text = gXmlDocument.CreateTextNode("text");
                                //text.Value = gHTMLReader.Value;
                                //gCurrentNode.AppendChild(text);
                                AddText(gCurrentNode, gHTMLReader.Value);
                            }
                            else
                            {
                                if (gHTMLReader.IsText)
                                {
                                    //XmlText text = gXmlDocument.CreateTextNode("text");
                                    //text.Value = gHTMLReader.Value;
                                    //gCurrentNode.AppendChild(text);
                                    AddText(gCurrentNode, gHTMLReader.Value);
                                }
                                else
                                {
                                    string s = gHTMLReader.Value;
                                    s = gCommentCorrection.Replace(s, "-");
                                    if (s.EndsWith("-")) s += " ";
                                    //XmlComment comment = gXmlDocument.CreateComment(s);
                                    //gCurrentNode.AppendChild(comment);
                                    AddComment(gCurrentNode, s);
                                }
                            }
                        }
                    }
                    else if (gHTMLReader.IsDocType)
                    {
                        //XmlAttribute attrib = gXmlDocument.CreateAttribute("doctype");
                        //attrib.Value = gHTMLReader.DocType;
                        //gHtmlNode.Attributes.Append(attrib);
                        AddAttribute(gHtmlNode, "doctype", gHTMLReader.DocType);
                    }
                    else if (gHTMLReader.IsProperty)
                    {
                        if (gbGenerateXmlNodeOnly || gbNoTag) continue;
                        try
                        {
                            string sPropertyName = gHTMLReader.PropertyName;
                            //sPropertyName = sPropertyName.Replace("\"", "");
                            //sPropertyName = sPropertyName.Replace("/", "");
                            //sPropertyName = sPropertyName.Replace("\\", "");
                            //sPropertyName = sPropertyName.Replace("-", "");
                            //sPropertyName = sPropertyName.Replace(",", "");
                            sPropertyName = gReplace.Replace(sPropertyName, "");
                            sPropertyName = sPropertyName.ToLower();
                            if (sPropertyName == "") sPropertyName = "__value";
                            //XmlAttribute attrib = gXmlDocument.CreateAttribute(sPropertyName);
                            //attrib.Value = gHTMLReader.PropertyValue;
                            //gCurrentNode.Attributes.Append(attrib);


                            // modif le 28/01/2014
                            //   hexadecimal value 0x03, is an invalid character
                            //   found in http://www.reseau-gesat.com/Gesat/Yvelines,78/Fontenay-le-Fleury,31443/esat-cotra,e1596/
                            //   <html><head><meta name="keywords" content="Conditionnement, travaux &amp;agrave; fa&amp;ccedil;onToutes activit&amp;eacute;s en entreprise Entretien et cr&amp;eacute;ation despaces verts" />
                            string propertyValue = gHTMLReader.PropertyValue;
                            if (propertyValue != null)
                                propertyValue = propertyValue.Replace("\x03", "");
                            //AddAttribute(gCurrentNode, sPropertyName, gHTMLReader.PropertyValue);
                            AddAttribute(gCurrentNode, sPropertyName, propertyValue);


                            if (gHTMLReader.IsMarkBeginEnd) TagEnd(gHTMLReader.MarkName.ToLower());
                        }
                        catch
                        {
                        }
                    }
                    else if (gHTMLReader.IsMarkBeginEnd)
                    {
                        //TagBegin(gHTMLReader.MarkName.ToLower(), true);

                        string sTagName = gHTMLReader.MarkName.ToLower();
                        sTagName = gReplace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagBegin(sTagName, true);
                    }
                    else if (gHTMLReader.IsMarkBegin)
                    {
                        //TagBegin(gHTMLReader.MarkName.ToLower(), false);

                        string sTagName = gHTMLReader.MarkName.ToLower();
                        sTagName = gReplace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagBegin(sTagName, false);
                    }
                    else if (gHTMLReader.IsMarkEnd)
                    {
                        //TagEnd(gHTMLReader.MarkName.ToLower());

                        string sTagName = gHTMLReader.MarkName.ToLower();
                        sTagName = gReplace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagEnd(sTagName);
                    }
                }
            }
            finally
            {
                gHTMLReader.Close();
            }
        }
        #endregion

        #region InitXml
        private void InitXml()
        {
            gHTMLReader.ReadCommentInText = gbReadCommentInText;

            //gXmlDocument = new XmlDocument();
            if (gXmlDocument != null)
            {
                if (!gbGenerateXmlNodeOnly)
                    gXmlDocument.PreserveWhitespace = true;
                else
                    gXmlDocument.PreserveWhitespace = false;
            }
            //XmlDeclaration declaration = gXml.CreateXmlDeclaration("1.0", "utf-8", null);
            //gXml.AppendChild(declaration);

            // création du tag xml
            //XmlElement element = gXmlDocument.CreateElement("xml");
            //gXmlDocument.AppendChild(element);
            //gCurrentNode = gCurrentTreeNode = (XmlNode)element;
            XXXNode element = CreateElement("xml");
            AddElement(gDocumentNode, element);
            gCurrentNode = gCurrentTreeNode = element;

            if (!gbGenerateXmlNodeOnly)
                AddText(gCurrentNode, "\r\n");

            if (gbNormalizeXml)
            {
                // création du tag html
                //element = gXmlDocument.CreateElement("html");
                //gCurrentTreeNode.AppendChild(element);
                //gHtmlNode = gCurrentNode = gCurrentTreeNode = (XmlNode)element;
                element = CreateElement("html");
                AddElement(gCurrentTreeNode, element);
                gHtmlNode = gCurrentNode = gCurrentTreeNode = element;

                if (!gbGenerateXmlNodeOnly)
                    AddText(gCurrentNode, "\r\n");

                // création du tag head
                //element = gXmlDocument.CreateElement("head");
                //gCurrentTreeNode.AppendChild(element);
                //gHeadNode = gCurrentNode = (XmlNode)element;
                element = CreateElement("head");
                AddElement(gCurrentTreeNode, element);
                gHeadNode = gCurrentNode = element;

                if (!gbGenerateXmlNodeOnly)
                    AddText(gCurrentNode, "\r\n");

                // création du tag title
                //element = gXmlDocument.CreateElement("title");
                //gCurrentNode.AppendChild(element);
                //gTitleNode = gCurrentNode = (XmlNode)element;
                element = CreateElement("title");
                AddElement(gCurrentNode, element);
                gTitleNode = gCurrentNode = element;

                // création du tag body
                //element = gXmlDocument.CreateElement("body");
                //gCurrentTreeNode.AppendChild(element);
                //gBodyNode = gCurrentNode = (XmlNode)element;
                element = CreateElement("body");
                AddElement(gCurrentTreeNode, element);
                gBodyNode = gCurrentNode = element;

                if (!gbGenerateXmlNodeOnly)
                    AddText(gCurrentNode, "\r\n");

                gCurrentNode = gCurrentTreeNode = gHeadNode;
            }
        }
        #endregion

        #region TagBegin
        private void TagBegin(string sTagName, bool bTagEnd)
        {
            gbNoTag = false;

            //sTagName = sTagName.Replace('-', '_');
            //sTagName = sTagName.Replace('!', '_');
            //sTagName = sTagName.Replace('[', '_');
            //sTagName = sTagName.Replace(']', '_');
            //sTagName = gReplace.Replace(sTagName, "_");
            //if (sTagName == "") sTagName = "_";
            HtmlTagType tagType = HtmlTags.GetHtmlTagType(sTagName);
            HtmlTag tag = HtmlTags.GetHtmlTag(tagType);
            if (gbNormalizeXml)
            {
                if (tagType == HtmlTagType.Html || tagType == HtmlTagType.Head)
                {
                    gbNoTag = true;
                    return;
                }
                if (tagType == HtmlTagType.Body)
                {
                    gbNoTag = true;
                    if (!gbBody)
                    {
                        gbBody = true;
                        gCurrentNode = gCurrentTreeNode = gBodyNode;
                    }
                    return;
                }
                if (tagType == HtmlTagType.Title)
                {
                    if (!gbTitle)
                    {
                        if (!bTagEnd)
                        {
                            gbTitle = true;
                            gCurrentNode = gTitleNode;
                        }
                    }
                    else
                        gbNoTag = true;
                    return;
                }
                if (!gbBody && tag.TagCategory != HtmlTagCategory.Head)
                {
                    gbBody = true;
                    gCurrentNode = gCurrentTreeNode = gBodyNode;
                }
            }
            //gCurrentNode = gXmlDocument.CreateElement(sTagName);
            gCurrentNode = CreateElement(sTagName);
            if (gbNormalizeXml)
            {
                if (tagType == HtmlTagType.Table && !bTagEnd)
                {
                    if (gTable != null) gTableStack.Push(gTable);
                    gTable = new HtmlTable();
                    gTable.Table = gCurrentNode;
                    //gCurrentTreeNode.AppendChild(gCurrentNode);
                    AddElement(gCurrentTreeNode, gCurrentNode);
                    gCurrentTreeNode = gCurrentNode;
                    return;
                }
                if (TagBeginTableCategory(tag, bTagEnd)) return;
                if (tagType == HtmlTagType.DL && !bTagEnd)
                {
                    if (gDefinitionList != null) gDefinitionListStack.Push(gDefinitionList);
                    gDefinitionList = gCurrentNode;
                    //gCurrentTreeNode.AppendChild(gCurrentNode);
                    AddElement(gCurrentTreeNode, gCurrentNode);
                    gCurrentTreeNode = gCurrentNode;
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
            //gCurrentTreeNode.AppendChild(gCurrentNode);
            AddElement(gCurrentTreeNode, gCurrentNode);
            if (!bTagEnd && tag.EndBoundType != HtmlBoundType.Forbidden) gCurrentTreeNode = gCurrentNode;
        }
        #endregion

        #region TagBeginTableCategory
        private bool TagBeginTableCategory(HtmlTag tag, bool bTagEnd)
        {
            if (gTable == null || tag.TagCategory != HtmlTagCategory.Table) return false;
            switch (tag.TagType)
            {
                case HtmlTagType.THead:
                case HtmlTagType.TBody:
                case HtmlTagType.TFoot:
                    //gTable.Table.AppendChild(gCurrentNode);
                    AddElement(gTable.Table, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Body = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Body = null;
                    return true;
                case HtmlTagType.ColGroup:
                    //gTable.Table.AppendChild(gCurrentNode);
                    AddElement(gTable.Table, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.ColGroup = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.ColGroup = null;
                    return true;
                case HtmlTagType.Col:
                    gCurrentTreeNode = gTable.Table;
                    if (gTable.ColGroup != null) gCurrentTreeNode = gTable.ColGroup;
                    //gCurrentTreeNode.AppendChild(gCurrentNode);
                    AddElement(gCurrentTreeNode, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Col = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Col = null;
                    return true;
                case HtmlTagType.TR:
                    if (gTable.Body == null)
                    {
                        //gTable.Body = gXmlDocument.CreateElement("tbody");
                        //gTable.Table.AppendChild(gTable.Body);
                        gTable.Body = CreateElement("tbody");
                        AddElement(gTable.Table, gTable.Body);
                    }
                    //gTable.Body.AppendChild(gCurrentNode);
                    AddElement(gTable.Body, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Row = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Row = null;
                    return true;
                case HtmlTagType.TH:
                case HtmlTagType.TD:
                    if (gTable.Row == null)
                    {
                        if (gTable.Body == null)
                        {
                            //gtable.body = gxmldocument.createelement("tbody");
                            //gtable.table.appendchild(gtable.body);
                            gTable.Body = CreateElement("tbody");
                            AddElement(gTable.Table, gTable.Body);
                        }
                        //gTable.Row = gXmlDocument.CreateElement("tr");
                        //gTable.Body.AppendChild(gTable.Row);
                        gTable.Row = CreateElement("tr");
                        AddElement(gTable.Body, gTable.Row);
                    }
                    //gTable.Row.AppendChild(gCurrentNode);
                    AddElement(gTable.Row, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Data = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Data = null;
                    return true;
            }
            return false;
        }
        #endregion

        #region TagBeginDefinitionListCategory
        private bool TagBeginDefinitionListCategory(HtmlTag tag, bool bTagEnd)
        {
            if (gDefinitionList == null || tag.TagCategory != HtmlTagCategory.DefinitionList) return false;
            switch (tag.TagType)
            {
                case HtmlTagType.DT:
                case HtmlTagType.DD:
                    //gDefinitionList.AppendChild(gCurrentNode);
                    AddElement(gDefinitionList, gCurrentNode);
                    if (!bTagEnd) gCurrentTreeNode = gCurrentNode;
                    return true;
            }
            return false;
        }
        #endregion

        #region TagEnd
        private void TagEnd(string sTagName)
        {
            if (gbNormalizeXml)
            {
                HtmlTagType tagType = HtmlTags.GetHtmlTagType(sTagName);
                switch (tagType)
                {
                    case HtmlTagType.Html:
                    case HtmlTagType.Head:
                    case HtmlTagType.Body:
                        return;
                    case HtmlTagType.Title:
                        gCurrentNode = gCurrentTreeNode;
                        return;
                    case HtmlTagType.Table:
                        if (gTable == null) return;
                        //gCurrentNode = gCurrentTreeNode = gTable.Table.ParentNode;
                        gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Table);
                        gTable = null;
                        if (gTableStack.Count != 0) gTable = gTableStack.Pop();
                        return;
                    case HtmlTagType.DL:
                        if (gDefinitionList == null) return;
                        //gCurrentNode = gCurrentTreeNode = gDefinitionList.ParentNode;
                        gCurrentNode = gCurrentTreeNode = GetParentXXNode(gDefinitionList);
                        gDefinitionList = null;
                        if (gDefinitionListStack.Count != 0) gDefinitionList = gDefinitionListStack.Pop();
                        return;
                }
                if (gTable != null)
                {
                    switch (tagType)
                    {
                        case HtmlTagType.THead:
                        case HtmlTagType.TBody:
                        case HtmlTagType.TFoot:
                            gCurrentNode = gCurrentTreeNode = gTable.Table;
                            gTable.Body = null;
                            return;
                        case HtmlTagType.ColGroup:
                            gCurrentNode = gCurrentTreeNode = gTable.Table;
                            gTable.ColGroup = null;
                            return;
                        case HtmlTagType.Col:
                            if (gTable.Col != null)
                            {
                                //gCurrentNode = gCurrentTreeNode = gTable.Col.ParentNode;
                                gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Col);
                                gTable.Col = null;
                            }
                            return;
                        case HtmlTagType.TR:
                            if (gTable.Row != null)
                            {
                                //gCurrentNode = gCurrentTreeNode = gTable.Row.ParentNode;
                                gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Row);
                                gTable.Row = null;
                            }
                            return;
                        case HtmlTagType.TH:
                        case HtmlTagType.TD:
                            if (gTable.Data != null)
                            {
                                //gCurrentNode = gCurrentTreeNode = gTable.Data.ParentNode;
                                gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Data);
                                gTable.Data = null;
                            }
                            return;
                    }
                }

            }
            //XmlNode node = gCurrentTreeNode;
            //while (node != null)
            //{
            //    if (node.Name == sTagName)
            //    {
            //        gCurrentTreeNode = node.ParentNode;
            //        break;
            //    }
            //    node = node.ParentNode;
            //}
            XXXNode node = GetParentXXNodeByName(gCurrentTreeNode, sTagName);
            if (node != null)
                gCurrentTreeNode = GetParentXXNode(node);
            gCurrentNode = gCurrentTreeNode;
        }
        #endregion

        #region static method
        #region Normalize
        public static void Normalize(XmlDocument xml)
        {
            // ajout du tag tbody dans les tag table qui n'en ont pas
            XmlNodeList tables = xml.SelectNodes("//table");
            foreach (XmlNode table in tables)
            {
                NormalizeTable(table);
            }
        }
        #endregion

        #region NormalizeTable
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
        #endregion

        #region LoadHtml
        //public static XmlDocument LoadHtml(string sUrl)
        //{
        //    HtmlXml hx = new HtmlXml(sUrl);
        //    return hx.GenerateXmlDocument();
        //}
        #endregion

        #region GetTables(XmlDocument xml)
        public static pb.old.HtmlXmlTables GetTables(XmlDocument xml)
        {
            return new pb.old.HtmlXmlTables(xml);
        }
        #endregion

        #region GetTables(string[] tablesPath)
        public static pb.old.HtmlXmlTables GetTables(string[] tablesPath)
        {
            return new pb.old.HtmlXmlTables(tablesPath);
        }
        #endregion
        #endregion
    }
}
