using System;
using System.Xml;

namespace pb.Data.Xml
{
    public static class zxml1
    {
        public static string GetPath(XmlNode node)
        {
            if (node is XmlDocument) return node.Name;
            string sNodePath = "";
            do
            {
                if (node is XmlDocument) break;
                sNodePath = "/" + node.Name + "[" + GetNodeIndex(node).ToString() + "]" + sNodePath;
                if (node is XmlAttribute)
                    node = ((XmlAttribute)node).OwnerElement;
                else
                    node = node.ParentNode;
            }
            while (node != null);

            return sNodePath;
        }

        public static int GetNodeIndex(XmlNode node)
        {
            if (node.ParentNode == null) return 1;
            string sName = node.Name;
            int i = 1;
            foreach (XmlNode node2 in node.ParentNode.ChildNodes)
            {
                if (node2 == node) break;
                if (node2.Name == sName) i++;
            }
            return i;
        }

        public static string GetValue(XmlNode node, string xpath)
        {
            if (node == null)
                return null;
            if (xpath != null)
                node = node.SelectSingleNode(xpath);
            if (node != null)
            {
                if (node.Value != null)
                    return node.Value;
                foreach (XmlNode node2 in node.ChildNodes)
                {
                    if (node2 is XmlText)
                        return node2.Value;
                }
            }
            return null;
        }
    }

    public static partial class GlobalExtension
    {
        public static string zGetPath(this XmlNode node)
        {
            return zxml1.GetPath(node);
        }

        public static string zGetValue(this XmlNode node, string xpath)
        {
            return zxml1.GetValue(node, xpath);
        }
    }
}
