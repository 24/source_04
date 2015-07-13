using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;

namespace pb.old
{
    public delegate string XmlTranslatePath(string sNodePath);

    #region XmlSelectException
    public class XmlSelectException : Exception
    {
        public XmlSelectException(string sMessage) : base(sMessage) { }
        public XmlSelectException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public XmlSelectException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public XmlSelectException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }
    #endregion

    #region doc
    ///
    /// Select(specific_xpath);
    ///   specific_xpath d�finit les noeuds xml � s�lectionner, les valeurs 
    ///   
    /// Select(string xpath_node, string xpath_value1, string xpath_value2, ...);
    ///   xpath_node et les xpath_value sont des specific_xpath
    ///   xpath_node d�finit les noeuds xml � s�lectionner
    ///   xpath_valueN d�finit les valeurs � rechercher, si aucun xpath_value n'est d�fini la valeur prise est celle du noeud
    /// 
    /// specific_xpath = [table_code] xpath [:.:xpath_function1:xpath_function2:...] ...
    /// 
    /// table_code     = T{level}.{row}.{col}
    /// 
    /// xpath          = xpath expression ( XmlNode.SelectNodes() )
    /// 
    /// xpath_function =
    ///   //(plus de convertion en int)  int                      : valeur int
    ///   EmptyRow                 : accepte les lignes sans valeurs
    ///   N(column_name)           : d�finit le nom de la colonne
    ///   NotNull                  : ne s�lectionne pas les lignes qui n'ont pas de valeur (value=null) pour cette colonne
    ///   NotEmpty                 : ne s�lectionne pas les lignes qui ont une valeur vide (value="") pour cette colonne
    ///   NoFilter                 : d�sactive le filtrage des valeurs,
    ///                              le filtrage consiste � supprimer les blancs et les tabulations en d�but et fin de ligne et � supprimer les changements de ligne
    ///   //Find(value)              : s�lectionne les lignes dont la valeur de cette colonne contient value,
    ///   //                           ignore la casse, il peut y avoir plusieurs Find pour la m�me colonne, dans ce cas il suffit que l'une des valeurs soit trouv�e, value est une expression r�guli�re
    ///   //                           si Find() est d�fini dans xpath_node pour chaque ligne il faut que au moins une colonne corresponde � un des Find()
    ///   Find(value) ou Find("value1", "value2", ...)
    ///                            : s�lectionne les lignes dont la valeur de cette colonne contient une des valeurs value1, value2, ...
    ///                              ignore la casse, il peut y avoir plusieurs Find pour la m�me colonne, dans ce cas il faut que toutes les valeurs soit trouv�e, value est une expression r�guli�re
    ///                              il peut y avoir plusieurs Find pour la m�me colonne, dans ce cas il faut que au moins une valeur de chaque Find() soit trouv�e,
    ///                              value est une expression r�guli�re, la casse est ignor�e
    ///   
    ///   FindAll(value)           : idem Find() sauf si il y a plusieurs FindAll pour la m�me colonne, il faut que toutes les valeurs soit trouv�e.
    ///                              si FindAll() est d�fini dans xpath_node pour chaque ligne il faut que au moins une colonne corresponde � tous les FindAll()
    ///   FindExact(value)         : s�lectionne les lignes dont la valeur de cette colonne est �gale � value,
    ///                              ignore la casse, il peut y avoir plusieurs FindExact pour la m�me colonne, dans ce cas il suffit que l'une des valeurs soit trouv�e
    ///   FindAttrib(attrib=value) : s�lectionne les noeuds qui ont un attribut attrib avec pour valeur value,
    ///                              ignore la casse, il peut y avoir plusieurs FindAttrib pour la m�me colonne, dans ce cas il faut que tous les attributs soient trouv�s,
    ///                              value est une expression r�guli�re
    ///   Skip(value)              : ne s�lectionne pas les lignes dont la valeur de cette colonne contient value,
    ///                              ignore la casse, il peut y avoir plusieurs Skip pour la m�me colonne, value est une expression r�guli�re
    ///   SkipValue(value)         : ne prend pas les valeurs qui contiennent 'value'
    ///                              ignore la casse, il peut y avoir plusieurs Skip pour la m�me colonne, value est une expression r�guli�re
    ///   Remove(value)            : permet de supprimer une partie de la valeur trouv�e. value est une expression r�guli�re, il peut y avoir plusieurs Remove pour la m�me colonne
    ///   Coords(value)            : r�cup�re une des valeurs d'un attribut coords d'un tag area, 
    ///                              les valeurs d�finis dans coords d�pendent du type de shape :
    ///                                shape = rect   : left-x, top-y, right-x, bottom-y
    ///                                shape = circle : center-x, center-y, radius
    ///                                shape = poly   : x1, y1, x2, y2, ..., xN, yN
    ///                              value peut prendre les valeurs suivantes : x1, y1, ..., xN, yN ou radius
    ///                              left-x = x1, top-y = y1, right-x = x2, bottom-y = y2
    ///                              center-x = x1, center-y = y1, radius = radius
    ///   Value(n)                 : r�cup�re la n i�me valeur
    ///   NodeValue                : pour obtenir la valeur du noeud
    ///   NodeName                 : nom du noeud
    ///   AllValues
    ///   Concat(separator) ou ConcatValue(separator)
    ///                            : fait la concat�nation des valeurs avec separator entre chaque valeur
    ///   Repeat                   : r�p�te la derni�re valeur non nulle de la colonne pour les lignes sans valeur
    ///   Url                      : change l'url relative en url absolue
    ///   NoSubNodes(value)        : permet d'�viter certain noeud dans la recherche d'une valeur, il peut y avoir plusieurs NoSubNodes pour la m�me colonne
    ///   record_no(no)            : 
    /// 
    ///   
    #endregion

    public class XmlSelect : IEnumerable<XmlSelect>, IEnumerator<XmlSelect>
    {
        // param�tres
        private XmlNode gSourceNode = null;
        private XmlSelectParameters gSelectPrm = new XmlSelectParameters();
        private string gsSourceXPathNode = null;
        private XPath gXPathNode = null;
        private string[] gsSourceXPathValues = null;
        private XPath[] gXPathValues = new XPath[0];
        private Dictionary<string, int> gValueNames = null;

        // r�sultat
        private string gsPathSourceNode = null;
        private string gsTranslatedPathSourceNode = null;

        private XmlNode gCurrentNode = null;
        private string gsPathCurrentNode = null;
        private string gsTranslatedPathCurrentNode = null;
        private HtmlXmlTable gCurrentTable = null;
        private string[] gsValues = null;

        private XmlNode gLastValueNode = null;
        private string gsPathLastValueNode = null;
        private string gsTranslatedPathLastValueNode = null;
        private HtmlXmlTable gLastValueTable = null;

        // variable de travail
        private int[] giIndexNodes = null;
        private XmlNodeList gNodes = null; // liste des nodes s�lectionn�s avec gXPathNode
        private int giCurrentNode = -1;    // indice du node courant
        private int giRecordNo = 0;        // RecordNo 0, 1, ...

        private static Encoding gIsoEncoding = Encoding.GetEncoding("iso-8859-1");

        public void Dispose()
        {
        }

        public object this[int i]
        {
            get
            {
                return GetValue(i);
            }
        }

        public object this[string name]
        {
            get
            {
                return GetValue(name);
            }
        }

        public XmlNode SourceNode
        {
            get { return gSourceNode; }
            set { gSourceNode = value; }
        }

        public XmlSelectParameters SelectPrm
        {
            get { return gSelectPrm; }
            set { gSelectPrm = value; }
        }

        public string SourceXPathNode
        {
            get { return gsSourceXPathNode; }
            set
            {
                gsSourceXPathNode = value;
                gXPathNode = new XPath(gsSourceXPathNode, gSelectPrm);
            }
        }

        public XPath XPathNode
        {
            get { return gXPathNode; }
            set { gXPathNode = value; }
        }

        public string[] SourceXPathValues
        {
            get { return gsSourceXPathValues; }
            set
            {
                //gsSourceXPathValues = value;
                //gXPathValues = new XPath[gsSourceXPathValues.Length];
                //for (int i = 0; i < gsSourceXPathValues.Length; i++)
                //    gXPathValues[i] = new XPath(gsSourceXPathValues[i], gSelectPrm);
                SetXPathValues(value);
            }
        }

        private void SetXPathValues(string[] sourceXPathValues)
        {
            gsSourceXPathValues = sourceXPathValues;
            gXPathValues = new XPath[gsSourceXPathValues.Length];
            gValueNames = new Dictionary<string, int>();
            for (int i = 0; i < gsSourceXPathValues.Length; i++)
            {
                //gXPathValues[i] = new XPath(gsSourceXPathValues[i], gSelectPrm);
                XPath xpath = new XPath(gsSourceXPathValues[i], gSelectPrm);
                gXPathValues[i] = xpath;
                if (!gValueNames.ContainsKey(xpath.Name))
                    gValueNames.Add(xpath.Name, i);
            }
        }

        public XPath[] XPathValues
        {
            get { return gXPathValues; }
            set { gXPathValues = value; }
        }

        public string PathSourceNode
        {
            get
            {
                if (gsPathSourceNode == null)
                {
                    // Xml.GetNodePath(gSourceNode)
                    gsPathSourceNode = gSourceNode.zGetPath();
                    gsTranslatedPathSourceNode = null;
                }
                return gsPathSourceNode;
            }
        }

        public string TranslatedPathSourceNode
        {
            get
            {
                if (gsTranslatedPathSourceNode == null)
                    gsTranslatedPathSourceNode = XPathToTranslatedXPath(PathSourceNode, out gCurrentTable);
                return gsTranslatedPathSourceNode;
            }
        }

        public XmlNode CurrentNode
        {
            get { return gCurrentNode; }
        }

        public string PathCurrentNode
        {
            get
            {
                if (gsPathCurrentNode == null)
                {
                    // Xml.GetNodePath(gCurrentNode)
                    gsPathCurrentNode = gCurrentNode.zGetPath();
                    gsTranslatedPathCurrentNode = null;
                }
                return gsPathCurrentNode;
            }
        }

        public string TranslatedPathCurrentNode
        {
            get
            {
                if (gsTranslatedPathCurrentNode == null)
                    gsTranslatedPathCurrentNode = XPathToTranslatedXPath(PathCurrentNode, out gCurrentTable);
                return gsTranslatedPathCurrentNode;
            }
        }

        public HtmlXmlTable CurrentTable
        {
            get
            {
                //if (gsPathCurrentNode == null)
                //{
                //    gsPathCurrentNode = Xml.GetNodePath(gCurrentNode);
                //    gsTranslatedPathCurrentNode = null;
                //}
                string sPath;
                if (gsTranslatedPathCurrentNode == null)
                    //gsTranslatedPathCurrentNode = XPathToTranslatedXPath(gsPathCurrentNode, out gCurrentTable);
                    sPath = TranslatedPathCurrentNode;
                return gCurrentTable;
            }
        }

        public string[] Values
        {
            get { return gsValues; }
        }

        public XmlNode LastValueNode
        {
            get { return gLastValueNode; }
        }

        public string PathLastValueNode
        {
            get
            {
                if (gsPathLastValueNode == null)
                {
                    // Xml.GetNodePath(gLastValueNode)
                    gsPathLastValueNode = gLastValueNode.zGetPath();
                    gsTranslatedPathLastValueNode = null;
                }
                return gsPathLastValueNode;
            }
        }

        public string TranslatedPathLastValueNode
        {
            get
            {
                if (gsTranslatedPathLastValueNode == null)
                    gsTranslatedPathLastValueNode = XPathToTranslatedXPath(PathLastValueNode, out gLastValueTable);
                return gsTranslatedPathLastValueNode;
            }
        }

        public HtmlXmlTable LastValueTable
        {
            get
            {
                string sPath;
                if (gsTranslatedPathLastValueNode == null)
                    sPath = TranslatedPathLastValueNode;
                return gLastValueTable;
            }
        }

        //public int RecordNo
        //{
        //    get { return giRecordNo; }
        //}

        private void Init()
        {
            gsValues = new string[gXPathValues.Length];

            //giIndexNodes = new int[gXPathNode.NoSubNodes.Count];
            //string sXmlXPath = gXPathNode.XmlXPath;
            //string sPathSourceNode = PathSourceNode;
            //for (int i = 0; i < gXPathNode.NoSubNodes.Count; i++)
            //    giIndexNodes[i] = sPathSourceNode.LastIndexOf(gXPathNode.NoSubNodes[i]);
            giIndexNodes = InitNoSubNodes(gXPathNode.NoSubNodes, PathSourceNode);

            Trace.WriteLine("XmlSelect.Init() : gSourceNode.SelectNodes(\"{0}\")", gXPathNode.XmlXPath);
            gNodes = gSourceNode.SelectNodes(gXPathNode.XmlXPath);
            foreach (XmlNode node in gNodes)
                Trace.WriteLine("  node : type {0} name {1} value {2}", node.NodeType, node.Name, node.Value);
            giCurrentNode = -1;
        }

        public bool Get()
        {
            bool bTrace = gSelectPrm.TraceFunction;

            //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 01");
            if (gNodes == null)
            {
                //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 02 Init");
                Init();
                //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 02 Init");
            }

            //if (bTrace) cTrace.Trace("XmlSelect.Get : {0} nodes", gNodes.Count);
            for (giCurrentNode++; giCurrentNode < gNodes.Count; giCurrentNode++)
            {
                //if (bTrace && giCurrentNode != 0) cTrace.StopNestedLevel("XmlSelect.Get : 03 for ...");
                //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 03 for ...");

                gCurrentNode = gNodes[giCurrentNode];
                gsPathCurrentNode = null;
                gsTranslatedPathCurrentNode = null;
                gCurrentTable = null;

                gLastValueNode = null;
                gsPathLastValueNode = null;
                gsTranslatedPathLastValueNode = null;
                gLastValueTable = null;

                if (!XPath_FindAttrib(gXPathNode.FindAttrib, gCurrentNode)) continue;

                if (gXPathNode.NoSubNodes.Count != 0)
                {
                    //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 04 NoSubNodes()");
                    // Xml.GetNodePath(gCurrentNode)
                    gsPathCurrentNode = gCurrentNode.zGetPath();
                    //if (!NoSubNodes(gXPathNode.NoSubNodes, giIndexNodes, gsPathNode)) continue;
                    bool b = NoSubNodes(gXPathNode.NoSubNodes, giIndexNodes, gsPathCurrentNode);
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 03 for ...");
                    if (!b) continue;
                }
                bool bAddRow = true;
                bool bEmptyRow = true;
                // bFind_Value : permet de savoir si l'un des Find() d�finis dans gXPathNode a �t� trouv� pour une des valeurs
                bool bFind_Value = false;
                if (gXPathNode.Find.Count == 0) bFind_Value = true;
                // bFindAll_Value : permet de savoir si tous les FindAll() d�finis dans gXPathNode a �t� trouv� pour une des valeurs
                bool bFindAll_Value = false;
                if (gXPathNode.FindAll.Count == 0) bFindAll_Value = true;
                for (int i = 0; i < gXPathValues.Length; i++)
                {
                    XPath xp = gXPathValues[i];
                    string s = null;
                    if (xp.NodeName && xp.XmlXPath == "")
                    {
                        s = gCurrentNode.Name;
                    }
                    else if (xp.RecordNo)
                    {
                        s = (xp.FirstRecordNo + giRecordNo).ToString();
                    }
                    else if (xp.NodeValue)
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 05 NodeValue");
                        if (gCurrentNode.Value != null)
                            s = gCurrentNode.Value;
                        else
                            // Xml.GetValue(gCurrentNode, "text()")
                            s = gCurrentNode.zGetValue("text()");
                        gLastValueNode = gCurrentNode;
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 05 NodeValue");
                    }
                    else if (!xp.AllValues && xp.IndexValue <= 0)
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 06 Value");
                        XmlNodeList nodes2 = gCurrentNode.SelectNodes(xp.XmlXPath);


                        //int[] iIndexNodes = new int[xp.NoSubNodes.Count];
                        //string sXmlXPath = xp.XmlXPath;
                        //for (int i = 0; i < xp.NoSubNodes.Count; i++)
                        //    iIndexNodes[i] = sXmlXPath.LastIndexOf(xp.NoSubNodes[i]);
                        int[] iIndexNodes = InitNoSubNodes(xp.NoSubNodes, PathCurrentNode);

                        s = "";
                        foreach (XmlNode node3 in nodes2)
                        {
                            //string sNodePath2 = Xml.GetNodePath(node3);
                            if (xp.NodeName)
                            {
                                s = node3.Name;
                                break;
                            }
                            if (!NoSubNodes(xp.NoSubNodes, iIndexNodes, node3)) continue;
                            string s2 = node3.Value;
                            //s2 = Http.TranslateCode(s2);
                            if (gXPathNode.FilterValue && xp.FilterValue)
                                s2 = FilterValue(s2);
                            if (!XPath_FindAttrib(xp.FindAttrib, node3)) continue;
                            if (XPath_Skip(gXPathNode.SkipValue, s2)) continue;
                            if (XPath_Skip(xp.SkipValue, s2)) continue;
                            gLastValueNode = node3;
                            if (s2 != null && s2 != "")
                            {
                                if (!xp.ConcatValue)
                                {
                                    s = s2;
                                    break;
                                }
                                if (s != "" && xp.ConcatSeparator != null) s += xp.ConcatSeparator;
                                s += s2;
                            }
                        }
                        if (s == "") s = null;
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 06 Value");
                    }
                    else
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 07 Value");
                        string[] s2 = XPath_GetValues(gCurrentNode, xp);
                        if (xp.AllValues)
                        {
                            if (s2.Length > 0) s = s2[0];
                        }
                        else if (s2.Length > xp.IndexValue)
                            s = s2[xp.IndexValue];
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 07 Value");
                        gLastValueNode = gCurrentNode;
                    }
                    //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 08 TranslateCode");
                    //s = Http.TranslateCode(s);
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 08 TranslateCode");
                    if (xp.CoordsValue != null)
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 09 GetCoordsValue");
                        s = GetCoordsValue(xp.CoordsValue, s);
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 09 GetCoordsValue");
                    }
                    if (gXPathNode.FilterValue && xp.FilterValue)
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 10 FilterValue");
                        s = FilterValue(s);
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 10 FilterValue");
                    }
                    if (s != null && s != "") bEmptyRow = false;
                    if (gXPathNode.Repeat || xp.Repeat)
                    {
                        if (s == null || s == "")
                            s = xp.Value;
                        else
                            xp.Value = s;
                    }
                    //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 11 XPath_Remove");
                    s = XPath_Remove(xp.Remove, s);
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 11 XPath_Remove");
                    //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 12 XPath_FindExact");
                    if (!XPath_FindExact(xp.FindExact, s))
                        bAddRow = false;
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 12 XPath_FindExact");
                    if (xp.Url) s = XPath_GetUrl(gSelectPrm.BaseUrl, s);
                    if (((gXPathNode.NotNull || xp.NotNull) && s == null) || ((gXPathNode.NotEmpty || xp.NotEmpty) && (s == null || s == "")))
                        bAddRow = false;
                    if (bAddRow)
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 13 XPath_Find");
                        if (!XPath_Find(xp.Find, s)) bAddRow = false;
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 13 XPath_Find");
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 14 XPath_FindAll");
                        if (!XPath_FindAll(xp.FindAll, s)) bAddRow = false;
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 14 XPath_FindAll");
                    }
                    //if (bAddRow)
                    //{
                    //    if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 14 XPath_FindAttrib");
                    //    if (!XPath_FindAttrib(xp.FindAttrib, s)) bAddRow = false;
                    //    if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 14 XPath_FindAttrib");
                    //}
                    if (bAddRow)
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 15 XPath_Skip");
                        if (XPath_Skip(gXPathNode.Skip, s)) bAddRow = false;
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 15 XPath_Skip");
                    }
                    if (bAddRow)
                    {
                        //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 16 XPath_Skip");
                        if (XPath_Skip(xp.Skip, s)) bAddRow = false;
                        //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 16 XPath_Skip");
                    }
                    if (!bAddRow) break;

                    //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 17 XPath_Find");
                    if (XPath_Find(gXPathNode.Find, s)) bFind_Value = true;
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 17 XPath_Find");

                    //if (bTrace) cTrace.StartNestedLevel("XmlSelect.Get : 18 XPath_FindAll");
                    if (XPath_FindAll(gXPathNode.FindAll, s)) bFindAll_Value = true;
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 18 XPath_FindAll");

                    gsValues[i] = s;
                }
                if (!bFind_Value || !bFindAll_Value || (bEmptyRow && !gXPathNode.EmptyRow)) bAddRow = false;
                if (bAddRow)
                {
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 03 for ...");
                    //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 01");
                    giRecordNo++;
                    return true;
                }
            }
            //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 03 for ...");
            //if (bTrace) cTrace.StopNestedLevel("XmlSelect.Get : 01");
            return false;
        }

        public object GetValue(int i)
        {
            string s = gsValues[i];
            //object v = zconvert.TryParse(gXPathValues[i].TypeValue, s);
            //if (v != null) return v;
            return s;
        }

        public object GetValue(string name)
        {
            return GetValue(gValueNames[name]);
        }

        public IEnumerator<XmlSelect> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public XmlSelect Current
        {
            get { return this; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this; }
        }

        public bool MoveNext()
        {
            return Get();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private static string[] XPath_GetValues(XmlNode node, XPath xp)
        {
            if (node == null || xp.XmlXPath == null) return new string[0];
            XmlNodeList nodes = node.SelectNodes(xp.XmlXPath);
            List<string> al = new List<string>();
            foreach (XmlNode node2 in nodes)
            {
                if (!XPath_FindAttrib(xp.FindAttrib, node2)) continue;
                if (node2.Value != null) al.Add(node2.Value);
            }
            string[] sValues = new string[al.Count];
            for (int i = 0; i < al.Count; i++) sValues[i] = (string)al[i];
            return sValues;
        }

        private static int[] InitNoSubNodes(List<string> NoSubNodes, string sPathNode)
        {
            int[] iIndexNodes = new int[NoSubNodes.Count];
            for (int i = 0; i < NoSubNodes.Count; i++)
                iIndexNodes[i] = sPathNode.LastIndexOf(NoSubNodes[i]);
            return iIndexNodes;
        }

        private static bool NoSubNodes(List<string> noSubNodes, int[] iIndexNodes, XmlNode node)
        {
            if (noSubNodes.Count == 0) return true;
            // Xml.GetNodePath(node)
            return NoSubNodes(noSubNodes, iIndexNodes, node.zGetPath());
        }

        private static bool NoSubNodes(List<string> noSubNodes, int[] iIndexNodes, string sNodePath)
        {
            for (int i = 0; i < noSubNodes.Count; i++)
            {
                int iIndexNode = sNodePath.LastIndexOf(noSubNodes[i]);
                if (iIndexNode != iIndexNodes[i]) return false;
            }
            return true;
        }

        //private static bool XPath_Find(List<Regex> find, string s)
        //{
        //    if (find.Count == 0) return true;
        //    if (s == null) return false;
        //    foreach (Regex regex in find)
        //    {
        //        if (regex.IsMatch(s)) return true;
        //    }
        //    return false;
        //}

        private static bool XPath_Find(List<List<Regex>> finds, string s)
        {
            if (finds.Count == 0) return true;
            if (s == null) return false;

            foreach (List<Regex> finds2 in finds)
            {
                bool bFind = false;
                foreach (Regex regex in finds2)
                {
                    if (regex.IsMatch(s))
                    {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind) return false;
            }
            return true;
        }

        private static bool XPath_FindAll(List<Regex> find, string s)
        {
            if (find.Count == 0) return true;
            if (s == null) return false;
            foreach (Regex regex in find)
            {
                if (!regex.IsMatch(s)) return false;
            }
            return true;
        }

        private static bool XPath_FindExact(List<string> find, string s)
        {
            if (find.Count == 0) return true;
            if (s == null) return false;
            foreach (string sFind in find)
            {
                if (string.Compare(s, sFind, true) == 0) return true;
            }
            return false;
        }

        private static bool XPath_FindAttrib(List<GClass2<string, Regex>> findAttribs, XmlNode node)
        {
            foreach (var findAttrib in findAttribs)
            {
                bool bFind = false;
                foreach (XmlAttribute attrib in node.Attributes)
                {
                    if (string.Compare(attrib.Name, findAttrib.Value1, true) == 0)
                    {
                        if (findAttrib.Value2.IsMatch(attrib.Value)) { bFind = true; break; }
                    }
                }
                if (!bFind) return false;
            }
            return true;
        }

        private static bool XPath_Skip(List<Regex> miss, string s)
        {
            foreach (Regex regex in miss)
            {
                if (regex.IsMatch(s))
                    return true;
            }
            return false;
        }
        
        private static string XPath_Remove(List<Regex> RemoveList, string s)
        {
            foreach (Regex regex in RemoveList)
            {
                Match match = regex.Match(s);
                while (match.Success)
                {
                    s = s.Remove(match.Index, match.Length);
                    match = match.NextMatch();
                }
            }
            return s;
        }
        
        public static string XPath_GetUrl(string sBaseUrl, string sUrl)
        {
            if (sUrl == null) return null;
            Uri uri;
            if (sBaseUrl != null)
            {
                Uri baseUri = new Uri(sBaseUrl);
                uri = new Uri(baseUri, sUrl);
            }
            else
                uri = new Uri(sUrl);
            return uri.AbsoluteUri;
        }
        
        public static string FilterValue(string sValue)
        {
            if (sValue == null) return null;
            char[] cLine = new char[] { '\r', '\n' };
            while (true)
            {
                int i1 = sValue.IndexOfAny(cLine);
                if (i1 == -1) break;
                int i2;
                for (i2 = i1 + 1; i2 < sValue.Length; i2++)
                {
                    char c = sValue[i2];
                    if (c != ' ' && c != '\t' && c != '\r' && c != '\n')
                        break;
                }
                sValue = sValue.Remove(i1, i2 - i1);
                sValue = sValue.Insert(i1, " ");
            }
            sValue = sValue.Trim();
            return sValue;
        }
        
        private static string GetCoordsValue(string sCoordsValue, string sValue)
        {
            // sCoordsValue = x1, y1, ..., xN, yN ou radius
            string[] sValues = zsplit.Split(sValue, ',', true);
            if (sCoordsValue.Length < 2) return null;
            char c = sCoordsValue[0];
            if (c == 'x' || c == 'y')
            {
                int i;
                if (int.TryParse(sCoordsValue.Substring(1), out i))
                {
                    i = (i - 1) * 2;
                    if (c == 'y') i++;
                    if (sValues.Length > i) return sValues[i];
                }
            }
            else if (sCoordsValue == "radius")
            {
                if (sValues.Length > 2) return sValues[2];
            }
            return null;
        }
        
        private string XPathToTranslatedXPath(string sNodePath, out HtmlXmlTable table)
        {
            return XPathToTranslatedXPath(sNodePath, gSelectPrm.Tables, out table);
        }
        
        public static string XPathToTranslatedXPath(string sNodePath, HtmlXmlTables tables)
        {
            HtmlXmlTable table;
            return XPathToTranslatedXPath(sNodePath, tables, out table);
        }
        
        public static string XPathToTranslatedXPath(string sNodePath, HtmlXmlTables tables, out HtmlXmlTable table)
        {
            table = null;
            if (tables == null) return sNodePath;
            return tables.XPathToTranslatedXPath(sNodePath, out table);
        }
        
        //public static string XPathToTranslatedXPath(string sNodePath, HtmlXmlTables tables, out HtmlXmlTable table)
        //{
        //    table = null;
        //    if (tables == null) return sNodePath;

        //    int i = sNodePath.LastIndexOf("table", StringComparison.InvariantCultureIgnoreCase);
        //    if (i != -1)
        //    {
        //        i = sNodePath.IndexOf(']', i + 5);
        //        if (i != -1)
        //        {
        //            table = tables[sNodePath.Substring(0, i + 1)];
        //            string sNodePath2 = table.Code + "(" + table.Id + ")";
        //            if (sNodePath.Length > i + 1)
        //                sNodePath2 += sNodePath.Substring(i + 1, sNodePath.Length - i - 1);
        //            return sNodePath2;
        //        }
        //    }
        //    return sNodePath;
        //}

        public static string GetTranslatedNodePath(XmlNode node, HtmlXmlTables tables)
        {
            HtmlXmlTable table;
            return GetTranslatedNodePath(node, tables, out table);
        }
        
        public static string GetTranslatedNodePath(XmlNode node, HtmlXmlTables tables, out HtmlXmlTable table)
        {
            // Xml.GetNodePath(node)
            string sPath = node.zGetPath();
            return XPathToTranslatedXPath(sPath, tables, out table);
        }

        public static XmlSelect Select(XmlNode node, string xpath, params string[] values)
        {
            if (node == null)
                throw new PBException("error no xml data");
            //HtmlXmlTables t = null;
            //if (_nodePathWithTableCode)
            //{
            //    if (_htmlXmlTables == null) _htmlXmlTables = HtmlXml.GetTables(XmlDocument);
            //    t = _htmlXmlTables;
            //}
            if (values.Length == 0)
                values = new string[] { ":.:NodeValue" };

            XmlSelect xSelect = new XmlSelect();
            xSelect.SourceNode = node;
            //xSelect.SelectPrm = new XmlSelectParameters(t, _url, _traceFunction);
            xSelect.SelectPrm = new XmlSelectParameters();
            xSelect.SourceXPathNode = xpath;
            xSelect.SourceXPathValues = values;
            return xSelect;
        }
    }
    
    #region class XmlSelectParameters
    public class XmlSelectParameters
    {
        //public XmlTranslatePath TranslatePath = null;
        public string BaseUrl = null;
        public HtmlXmlTables Tables = null;

        // defaut pour XPath
        public bool EmptyRow = false;
        public bool NotNull = false;
        public bool NotEmpty = false;
        public bool FilterValue = true;
        public bool NodeValue = false;
        public bool AllValues = false;
        public bool ConcatValue = false;
        public bool Repeat = false;
        public bool Url = false;

        public bool TraceFunction = false;

        public XmlSelectParameters()
        {
        }

        public XmlSelectParameters(HtmlXmlTables tables, string sBaseUrl, bool bTraceFunction)
        {
            Tables = tables;
            BaseUrl = sBaseUrl;
            TraceFunction = bTraceFunction;
        }
    }
    #endregion

    #region class XPath
    public class XPath
    {
        #region variable
        //public XmlSelect xmlSelect = null;
        public string XPathSource = null;
        public string XmlXPath = null;
        public string Name = null;
        public bool IsNameDefined = false;
        // modif le 04/02/2015 la gestion de TypeValue n'est pas bonne et ne sert qu'� convertir en int
        //public Type TypeValue = null;
        public bool EmptyRow = false;
        public bool NotNull = false;
        public bool NotEmpty = false;
        public bool FilterValue = true;
        public bool NodeValue = false;
        public bool NodeName = false;
        public bool AllValues = false;
        public bool ConcatValue = false;
        public bool RecordNo = false;
        public int FirstRecordNo = 1;
        public string ConcatSeparator = null;
        public int IndexValue = -1;
        //public string FindExact = null;
        public List<string> FindExact = new List<string>();
        //public List<Regex> Find = new List<Regex>();
        public List<List<Regex>> Find = new List<List<Regex>>();
        public List<Regex> FindAll = new List<Regex>();
        public List<GClass2<string, Regex>> FindAttrib = new List<GClass2<string, Regex>>();
        public List<Regex> Skip = new List<Regex>();
        public List<Regex> SkipValue = new List<Regex>();
        public bool Repeat = false;
        public bool Url = false;
        public List<Regex> Remove = new List<Regex>();
        public List<string> NoSubNodes = new List<string>();
        public string CoordsValue = null;
        public string Value = null;
        private XmlSelectParameters gSelectPrm = null;
        #endregion

        #region constructor
        public XPath(string sXPath, XmlSelectParameters prm)
        {
            //this.xmlSelect = xmlSelect;
            gSelectPrm = prm;
            SetDefault();
            Create(sXPath);
        }
        #endregion

        #region //Value
        //public string Value
        //{
        //    get
        //    {
        //        if (RecordNo)
        //            return xmlSelect.re;
        //        else
        //            return gsValue;
        //    }
        //    set { gsValue = value; }
        //}
        #endregion

        #region SetDefault
        private void SetDefault()
        {
            EmptyRow = gSelectPrm.EmptyRow;
            NotNull = gSelectPrm.NotNull;
            NotEmpty = gSelectPrm.NotEmpty;
            FilterValue = gSelectPrm.FilterValue;
            NodeValue = gSelectPrm.NodeValue;
            AllValues = gSelectPrm.AllValues;
            ConcatValue = gSelectPrm.ConcatValue;
            Repeat = gSelectPrm.Repeat;
            Url = gSelectPrm.Url;
        }
        #endregion

        #region Create
        private void Create(string sXPath)
        {
            char[] cTrim = { ' ', '\t', '\r', '\n' };
            char[,] cCharZone = { { '(', ')' }, { '[', ']' }, { '{', '}' }, { '\'', '\'' }, { '"', '"' } };

            XPathSource = sXPath;
            sXPath = TranslatedXPathToXPath(sXPath);

            //string[] sValues = cu.Split(sXPath, ':', cCharZone, true, false, false);
            string[] sValues = zsplit.Split(sXPath, ":.:", false, false);

            XmlXPath = sValues[0];
            if (sValues.Length > 1)
                sValues = zsplit.Split(sValues[1], ':', cCharZone, true, false, false);

            Name = GetName(XmlXPath);
            //for (int i = 1; i < sValues.Length; i++)
            for (int i = 0; i < sValues.Length; i++)
            {
                string sFunction = sValues[i].Trim(cTrim);
                string sParam = null;
                int i2 = sFunction.IndexOf('(');
                if (i2 != -1)
                {
                    int i3 = sFunction.LastIndexOf(')');
                    if (i3 == -1) throw new PBException("incorrect XPath expression \"{0}\"", sXPath);
                    sParam = sFunction.Substring(i2 + 1, i3 - i2 - 1);
                    sFunction = sFunction.Substring(0, i2).Trim(cTrim);
                }
                switch (sFunction.ToLower())
                {
                    // ligne
                    case "emptyrow":
                        EmptyRow = true;
                        break;
                    case "notnull":
                        NotNull = true;
                        break;
                    case "notempty":
                        NotEmpty = true;
                        break;
                    case "find":
                        //if (sParam == null) throw new PBException("missing expression in function Find {0}", sXPath);
                        //Find.Add(new Regex(sParam, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                        Create_Find(sParam, sXPath);
                        break;
                    case "findall":
                        if (sParam == null) throw new PBException("missing expression in function FindAll {0}", sXPath);
                        FindAll.Add(new Regex(sParam, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                        break;
                    case "findexact":
                        if (sParam == null) throw new PBException("missing expression in function FindExact {0}", sXPath);
                        FindExact.Add(sParam);
                        break;
                    case "findattrib":
                        if (sParam == null) throw new PBException("missing expression in function FindAttrib {0}", sXPath);
                        string[] sParams = zsplit.Split(sParam, '=', true, false, true);
                        if (sParams.Length != 2) throw new PBException("bad expression in function FindAttrib({0}) {1}", sParams, sXPath);
                        FindAttrib.Add(new GClass2<string, Regex>() { Value1 = sParams[0], Value2 = new Regex(sParams[1], RegexOptions.Compiled | RegexOptions.IgnoreCase) });
                        break;
                    case "skip":
                        if (sParam == null) throw new PBException("missing expression in function miss {0}", sXPath);
                        Skip.Add(new Regex(sParam, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                        break;
                    // value
                    case "nofilter":
                        FilterValue = false;
                        break;
                    case "skipvalue":
                        if (sParam == null) throw new PBException("missing expression in function SkipValue {0}", sXPath);
                        SkipValue.Add(new Regex(sParam, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                        break;
                    case "remove":
                        if (sParam == null) throw new PBException("missing expression in function remove {0}", sXPath);
                        Remove.Add(new Regex(sParam, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                        break;
                    case "coords":
                        if (sParam == null) throw new PBException("missing expression in function Coords {0}", sXPath);
                        CoordsValue = sParam.ToLower();
                        break;
                    case "value":
                        int iIndexValue;
                        if (!int.TryParse(sParam, out iIndexValue)) throw new PBException("invalid argument in function value {0}", sXPath);
                        IndexValue = iIndexValue - 1;
                        break;
                    case "nodevalue":
                        NodeValue = true;
                        if (Name == "") Name = "text";
                        break;
                    case "nodename":
                        NodeName = true;
                        //if (Name == "") Name = "text";
                        break;
                    case "allvalue":
                        AllValues = true;
                        break;
                    case "concatvalue":
                    case "concat":
                        ConcatValue = true;
                        ConcatSeparator = sParam;
                        break;
                    case "repeat":
                        Repeat = true;
                        break;
                    case "url":
                        Url = true;
                        break;
                    case "nosubnode":
                        if (sParam == null) throw new PBException("missing expression in function NoSubNode {0}", sXPath);
                        sParam = sParam.ToLower();
                        if (!sParam.StartsWith("/")) sParam = "/" + sParam;
                        NoSubNodes.Add(sParam);
                        break;
                    // column
                    case "n":
                        Name = sParam;
                        IsNameDefined = true;
                        break;
                    //case "int":
                    //    TypeValue = typeof(int);
                    //    break;
                    case "record_no":
                        RecordNo = true;
                        if (Name == "") Name = "record_no";
                        if (sParam != null)
                        {
                            int iFirstRecordNo;
                            if (int.TryParse(sParam, out iFirstRecordNo))
                                FirstRecordNo = iFirstRecordNo;
                        }
                        break;
                }
            }
        }
        #endregion

        #region Create_Find
        private void Create_Find(string sParam, string sXPath)
        {
            if (sParam == null) throw new PBException("missing expression in function Find {0}", sXPath);
            List<Regex> find = new List<Regex>();
            sParam = sParam.Trim();
            if (sParam.Length == 0) throw new PBException("missing expression in function Find {0}", sXPath);
            char cZone = sParam[0];
            if (cZone == '\"' || cZone == '\'')
            {
                //char[,] cCharZone = new char[,] { { '\"', '\"' } };
                char[,] cCharZone = new char[,] { { cZone, cZone } };
                string[] sFinds = zsplit.Split(sParam, ',', cCharZone, true, false, true);
                foreach (string sFind in sFinds)
                {
                    string sFind2 = sFind;
                    if (sFind2[0] == cZone) sFind2 = sFind2.Remove(0, 1);
                    if (sFind2.Length > 0 && sFind2[sFind2.Length - 1] == cZone) sFind2 = sFind2.Remove(sFind2.Length - 1, 1);
                    if (sFind2.Length == 0) throw new PBException("missing expression in function Find {0}", sXPath);
                    find.Add(new Regex(sFind2, RegexOptions.Compiled | RegexOptions.IgnoreCase));
                }
            }
            else
                find.Add(new Regex(sParam, RegexOptions.Compiled | RegexOptions.IgnoreCase));
            Find.Add(find);
        }
        #endregion

        #region GetName
        private static string GetName(string sXPath)
        {
            char[] cTrim = { '@', '_', '.', '(', ')' };
            string sName = sXPath;
            sName = sName.Replace("//", "_");
            sName = sName.Replace("/", "_");
            sName = sName.Trim(cTrim);
            return sName;
        }
        #endregion

        #region TranslatedXPathToXPath
        private static Regex grxTableCode = new Regex(@"^t[0-9]+\.[0-9]+\.[0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string TranslatedXPathToXPath(string sXPath)
        {
            if (gSelectPrm.Tables == null) return sXPath;
            //Regex rx = new Regex(@"^t[0-9]+\.[0-9]+\.[0-9]+", RegexOptions.IgnoreCase);
            Match match = grxTableCode.Match(sXPath);
            if (!match.Success) return sXPath;
            string sTableCode = sXPath.Substring(0, match.Length);
            HtmlXmlTable xmlTable = gSelectPrm.Tables.GetTable(sTableCode);
            //if (xmlTable == null) return null;
            if (xmlTable == null)
                throw new XmlSelectException("table \"{0}\" doesn't exist in \"{1}\"", sTableCode, gSelectPrm.BaseUrl);
            string sTablePath = xmlTable.AbsoluteTablePath;
            //sXPath = sTablePath + sXPath.Substring(match.Length, sXPath.Length - match.Length);
            int i = sXPath.IndexOf('/');
            if (i == -1)
                sXPath = sTablePath;
            else
                sXPath = sTablePath + sXPath.Substring(i, sXPath.Length - i);
            return sXPath;
        }
        #endregion
    }
    #endregion
}
