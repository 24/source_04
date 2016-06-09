using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb.Text;

namespace pb.Data.Xml
{
    [Serializable]
    public class XValueFormat
    {
        public string XPath = null;
        public string Format = null;
        public List<XValueFormat> Values = null;
    }

    [Serializable]
    public class XFormat
    {
        #region variable
        private string gsName = null;
        private string gsFormat = null;
        private StringZone[] gFormats = null;
        private string gsStringFormat = null;
        private List<XValueFormat> gFormatValues = null;
        private bool gbHide = false;
        private List<int> giHideElementIndex = null;
        private bool gbHideChild = false;
        private bool gbNoChildCount = false;
        private bool gbExpandNode = false;
        private XFormat gParent = null;
        private List<XFormat> gChilds = new List<XFormat>();
        private XFormat gPreviousElement = null;
        private XFormat gNextElement = null;
        #endregion

        #region property ...
        #region Name
        public string Name
        {
            get { return gsName; }
        }
        #endregion

        #region OriginalFormat
        public string OriginalFormat
        {
            get { return gsFormat; }
        }
        #endregion

        #region Formats
        public StringZone[] Formats
        {
            get { return gFormats; }
        }
        #endregion

        #region StringFormat
        public string StringFormat
        {
            get { return gsStringFormat; }
        }
        #endregion

        #region Hide
        public bool Hide
        {
            get { return gbHide; }
        }
        #endregion

        #region HideElementIndex
        public List<int> HideElementIndex
        {
            get { return giHideElementIndex; }
        }
        #endregion

        #region HideChild
        public bool HideChild
        {
            get { return gbHideChild; }
        }
        #endregion

        #region NoChildCount
        public bool NoChildCount
        {
            get { return gbNoChildCount; }
        }
        #endregion

        #region ExpandNode
        public bool ExpandNode
        {
            get { return gbExpandNode; }
        }
        #endregion

        #region Parent
        public XFormat Parent
        {
            get { return gParent; }
        }
        #endregion

        #region Childs
        public List<XFormat> Childs
        {
            get { return gChilds; }
        }
        #endregion

        #region PreviousElement
        public XFormat PreviousElement
        {
            get { return gPreviousElement; }
        }
        #endregion

        #region NextElement
        public XFormat NextElement
        {
            get { return gNextElement; }
        }
        #endregion
        #endregion

        #region Add
        public XFormat Add(XElement xe)
        {
            XFormat xf = XFormat.CreateXFormat(xe);
            xf.gParent = this;
            if (gChilds.Count > 0)
            {
                XFormat xf2 = gChilds[gChilds.Count - 1];
                xf2.gNextElement = xf;
                xf.gPreviousElement = xf2;
            }
            gChilds.Add(xf);
            foreach (XElement xe2 in xe.Elements())
                xf.Add(xe2);
            return xf;
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
                    case "nochildcount":
                        gbNoChildCount = true;
                        break;
                    case "hide":
                        gbHide = true;
                        break;
                    case "hidechild":
                        gbHideChild = true;
                        break;
                    case "expandnode":
                        gbExpandNode = true;
                        break;
                    default:
                        StringZone[] zones = StringZones.SplitZone(s, new char[,] { { '(', ')' } });
                        if (zones.Length == 2 && zones[0].BeginZoneChar == (char)0 && zones[1].BeginZoneChar == '(')
                        {
                            if (string.Compare(zones[0].String, "hide", true) == 0)
                            {
                                giHideElementIndex = new List<int>();
                                foreach (string index in zsplit.Split(zones[1].ContentString, ',', true))
                                {
                                    int i;
                                    if (int.TryParse(index, out i))
                                        giHideElementIndex.Add(i - 1);
                                }
                            }
                        }
                        break;
                }
            }
        }
        #endregion

        #region Element
        public XFormat Element(string sName)
        {
            foreach (XFormat xf in gChilds)
                if (xf.gsName == sName) return xf;
            return null;
        }
        #endregion

        #region Format
        public string Format(XElement xe)
        {
            if (gFormatValues == null || xe == null) return null;
            string[] values = new string[gFormatValues.Count];
            int i = 0;
            foreach (XValueFormat valueFormat in gFormatValues)
            {
                if (valueFormat.Values != null)
                {
                    string s = "";
                    foreach (XElement xe2 in xe.zXPathElements(valueFormat.XPath))
                    {
                        string[] values2 = new string[valueFormat.Values.Count];
                        int i2 = 0;
                        foreach (XValueFormat valueFormat2 in valueFormat.Values)
                        {
                            values2[i2++] = xe2.zXPathValue(valueFormat2.XPath);
                        }
                        s += string.Format(valueFormat.Format, values2);
                    }
                    values[i++] = s;
                }
                else
                    values[i++] = xe.zXPathValue(valueFormat.XPath);
            }
            return string.Format(gsStringFormat, values);
        }
        #endregion

        #region CreateXFormat
        public static XFormat CreateXFormat(XElement xe)
        {
            XFormat xf = new XFormat();
            xf.gsName = xe.Name.LocalName;
            xf.gsFormat = xe.zAttribValue("format");
            if (xf.gsFormat != null)
            {
                xf.gFormats = StringZones.SplitZone(xf.gsFormat, new char[,] { { '{', '}' } });
                string sFormat = "";
                int i = 0;
                xf.gFormatValues = new List<XValueFormat>();
                foreach (StringZone format in xf.gFormats)
                {
                    if (format.BeginZoneChar == '{')
                    {
                        sFormat += "{" + i++.ToString() + "}";

                        XValueFormat valueFormat = null;

                        //{Node(Phone) - {@PhoneType} {@Phone}}

                        StringZone[] zones = StringZones.SplitZone(format.ContentString, new char[,] { { '(', ')' }, { '{', '}' } });
                        if (zones.Length >= 2 && zones[0].BeginZoneChar == (char)0 && zones[1].BeginZoneChar == '(')
                        {
                            if (zones[0].String.ToLower() == "node")
                            {
                                valueFormat = new XValueFormat();
                                valueFormat.XPath = zones[1].ContentString;
                                valueFormat.Values = new List<XValueFormat>();
                                string sFormat2 = "";
                                int i2 = 0;
                                for (int j = 2; j < zones.Length; j++)
                                {
                                    if (zones[j].BeginZoneChar == '{')
                                    {
                                        sFormat2 += "{" + i2++.ToString() + "}";
                                        valueFormat.Values.Add(new XValueFormat() { XPath = zones[j].ContentString });
                                    }
                                    else
                                        sFormat2 += zones[j].String;
                                }
                                valueFormat.Format = sFormat2;
                            }
                        }
                        else
                            valueFormat = new XValueFormat() { XPath = format.ContentString };
                        if (valueFormat != null) xf.gFormatValues.Add(valueFormat);
                    }
                    else
                        sFormat += format.String;
                }
                xf.gsStringFormat = sFormat;
            }
            xf.SetOptions(xe.zAttribValue("option"));
            return xf;
        }
        #endregion

        #region CreateXFormats
        public static XFormat CreateXFormats(XElement xe)
        {
            XFormat xf = CreateXFormat(xe);
            foreach (XElement xe2 in xe.Elements())
                xf.Add(xe2);
            return xf;
        }
        #endregion
    }
}
