using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using pb.Text;

namespace pb.Data.Xml
{
    public enum XNodeFilter
    {
        SelectNode     = 0x0000,
        DontSelectNode = 0x0001,
        SkipNode       = 0x0002,
        Stop           = 0x0004
    }

    public enum NodeOpe
    {
        Source = 0,
        Child,
        Siblin,
        Close
    }

    public class XNodeInfo
    {
        public int Index;
        public XNode Node;
        public int Level;
        public NodeOpe Ope;
        public bool Selected;
        public bool Skip;
        public bool Stop;

        public override string ToString()
        {
            return ToString(100);
        }

        public string ToString(int width)
        {
            string s = string.Format("{0,5} ", Index); ;
            if (Level > 1)
                s += new string(' ', Level * 2);
            s += Node.zToString(attrib: true, close: Ope == NodeOpe.Close);
            if (s.Length < width)
                s += new string(' ', width - s.Length);
            s += "(" + Ope.ToString();
            if (Selected)
                s += ", selected";
            if (Skip)
                s += ", skip";
            if (Stop)
                s += ", stop";
            s += ")";
            return s;
        }
    }

    public static class XmlDescendant
    {
        private static bool __trace = false;

        public static bool Trace { get { return __trace; } set { __trace = value; } }

        public static IEnumerable<string> DescendantTexts(XElement element, Func<XNode, XNodeFilter> filter = null)
        {
            return DescendantTexts(new XElement[] { element }, filter);
        }

        public static IEnumerable<string> DescendantTexts(IEnumerable<XElement> elements, Func<XNode, XNodeFilter> filter = null)
        {
            return from xtext in DescendantNodes(elements, node => TextFilter(node, filter)) select ((XText)xtext).Value;
        }

        public static IEnumerable<XText> DescendantTextNodes(XElement element, Func<XNode, XNodeFilter> filter = null)
        {
            return DescendantTextNodes(new XElement[] { element }, filter);
        }

        public static IEnumerable<XText> DescendantTextNodes(IEnumerable<XElement> elements, Func<XNode, XNodeFilter> filter = null)
        {
            return from xtext in DescendantNodes(elements, node => TextFilter(node, filter)) select (XText)xtext;
        }

        public static IEnumerable<XElement> DescendantFormItems(XElement element, Func<XNode, XNodeFilter> filter = null)
        {
            return from node in DescendantNodes(new XElement[] { element }, node => FormItemFilter(node, filter)) select (XElement)node;
        }

        public static IEnumerable<XNode> DescendantNodes(XNode node, Func<XNode, XNodeFilter> filter = null)
        {
            return DescendantNodes(new XNode[] { node }, filter);
        }

        public static IEnumerable<XNode> DescendantNodes(IEnumerable<XNode> nodes, Func<XNode, XNodeFilter> filter = null)
        {
            bool stop = false;
            foreach (XNode node1 in nodes)
            {
                XNodeFilter xnodeFilter = XNodeFilter.SelectNode;
                if (filter != null)
                    xnodeFilter = filter(node1);

                if (__trace)
                    pb.Trace.WriteLine("DescendantNodes : source node {0}, filter {1}", node1.zGetPath(), xnodeFilter);

                if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                    break;

                if ((xnodeFilter & XNodeFilter.SkipNode) == XNodeFilter.SkipNode)
                    continue;

                if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                    yield return node1;

                if (__trace)
                    pb.Trace.WriteLine("DescendantNodes : source node {0}", node1.zGetPath());
                XNode node2 = node1;
                XNode sourceNode = node2;
                while (true)
                {

                    // get child
                    while (true)
                    {
                        if (!(node2 is XElement))
                            break;

                        // first child node
                        XNode childNode = ((XElement)node2).FirstNode;
                        if (childNode == null)
                            break;
                        xnodeFilter = XNodeFilter.SelectNode;
                        if (filter != null)
                            xnodeFilter = filter(childNode);
                        if (__trace)
                            pb.Trace.WriteLine("DescendantNodes : child node  {0}, filter {1}", childNode.zGetPath(), xnodeFilter);
                        if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                        {
                            stop = true;
                            break;
                        }
                        if ((xnodeFilter & XNodeFilter.SkipNode) == XNodeFilter.SkipNode)
                            break;

                        node2 = childNode;
                        //_level++;
                        if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                            yield return node2;
                        //else
                        //    continue;
                    }
                    if (stop)
                        break;

                    // get next sibling node or next sibling node of parent node
                    bool getChild = false;
                    bool stopNode = false;
                    while (true)
                    {
                        if (node2 == sourceNode)
                        {
                            stopNode = true;
                            break;
                        }

                        // next sibling node
                        XNode nextNode = node2.NextNode;
                        while (true)
                        {
                            if (nextNode == null)
                                break;

                            xnodeFilter = XNodeFilter.SelectNode;
                            if (filter != null)
                                xnodeFilter = filter(nextNode);

                            if (__trace)
                                pb.Trace.WriteLine("DescendantNodes : next node   {0}, filter {1}", nextNode.zGetPath(), xnodeFilter);

                            if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                            {
                                stop = true;
                                break;
                            }

                            if ((xnodeFilter & XNodeFilter.SkipNode) == 0)
                            {
                                node2 = nextNode;
                                if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                                    yield return node2;
                                //else
                                //{
                                    getChild = true;
                                    break;
                                //}
                            }
                            nextNode = nextNode.NextNode;
                        }
                        if (getChild)
                            break;

                        // parent node
                        node2 = node2.Parent;
                        //_level--;
                    }
                    if (stop || stopNode)
                        break;
                }
                if (stop)
                    break;
            }
        }

        public static IEnumerable<XNodeInfo> DescendantNodesInfos(XNode node, Func<XNode, XNodeFilter> filter = null)
        {
            return DescendantNodesInfos(new XNode[] { node }, filter);
        }

        public static IEnumerable<XNodeInfo> DescendantNodesInfos(IEnumerable<XNode> nodes, Func<XNode, XNodeFilter> filter = null)
        {
            bool stop = false;
            int level = 1;
            int index = 1;
            foreach (XNode node1 in nodes)
            {
                XNodeFilter xnodeFilter = XNodeFilter.SelectNode;
                if (filter != null)
                    xnodeFilter = filter(node1);

                if (__trace)
                    pb.Trace.WriteLine("DescendantNodesInfos : source node {0}, filter {1}", node1.zGetPath(), xnodeFilter);

                XNodeInfo nodeInfo = new XNodeInfo { Index = index++,  Node = node1, Level = level, Ope = NodeOpe.Source, Selected = false, Skip = false, Stop = false };

                if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                {
                    nodeInfo.Stop = true;
                    yield return nodeInfo;
                    break;
                }

                if ((xnodeFilter & XNodeFilter.SkipNode) == XNodeFilter.SkipNode)
                {
                    nodeInfo.Skip = true;
                    yield return nodeInfo;
                    continue;
                }

                if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                {
                    //yield return node1;
                    nodeInfo.Selected = true;
                    yield return nodeInfo;
                }
                else
                    yield return nodeInfo;

                if (__trace)
                    pb.Trace.WriteLine("DescendantNodesInfos : source node {0}", node1.zGetPath());
                XNode node2 = node1;
                XNode sourceNode = node2;
                while (true)
                {

                    // get child
                    while (true)
                    {
                        if (!(node2 is XElement))
                            break;

                        // first child node
                        XNode childNode = ((XElement)node2).FirstNode;

                        if (childNode == null)
                            break;

                        nodeInfo = new XNodeInfo { Index = index++, Node = childNode, Level = level + 1, Ope = NodeOpe.Child, Selected = false, Skip = false, Stop = false };

                        xnodeFilter = XNodeFilter.SelectNode;
                        if (filter != null)
                            xnodeFilter = filter(childNode);
                        if (__trace)
                            pb.Trace.WriteLine("DescendantNodesInfos : child node  {0}, filter {1}", childNode.zGetPath(), xnodeFilter);

                        if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                        {
                            stop = true;
                            nodeInfo.Stop = true;
                            yield return nodeInfo;
                            break;
                        }

                        if ((xnodeFilter & XNodeFilter.SkipNode) == XNodeFilter.SkipNode)
                        {
                            nodeInfo.Skip = true;
                            yield return nodeInfo;
                            break;
                        }

                        node2 = childNode;
                        if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                        {
                            //yield return node2;
                            level++;
                            nodeInfo.Selected = true;
                            yield return nodeInfo;
                        }
                        else
                            yield return nodeInfo;
                    }
                    if (stop)
                        break;

                    // get next sibling node or next sibling node of parent node
                    bool getChild = false;
                    bool stopNode = false;
                    while (true)
                    {
                        if (node2 == sourceNode)
                        {
                            stopNode = true;
                            break;
                        }

                        // next sibling node
                        XNode nextNode = node2.NextNode;
                        while (true)
                        {
                            if (nextNode == null)
                                break;

                            nodeInfo = new XNodeInfo { Index = index++, Node = nextNode, Level = level, Ope = NodeOpe.Siblin, Selected = false, Skip = false, Stop = false };

                            xnodeFilter = XNodeFilter.SelectNode;
                            if (filter != null)
                                xnodeFilter = filter(nextNode);

                            if (__trace)
                                pb.Trace.WriteLine("DescendantNodesInfos : next node   {0}, filter {1}", nextNode.zGetPath(), xnodeFilter);

                            if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                            {
                                stop = true;
                                nodeInfo.Stop = true;
                                yield return nodeInfo;
                                break;
                            }

                            if ((xnodeFilter & XNodeFilter.SkipNode) == 0)
                            {
                                node2 = nextNode;
                                if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                                {
                                    nodeInfo.Selected = true;
                                    yield return nodeInfo;
                                }
                                else
                                    yield return nodeInfo;
                                //else
                                //{
                                getChild = true;
                                break;
                                //}
                            }
                            else
                            {
                                nodeInfo.Skip = true;
                                yield return nodeInfo;
                            }
                            nextNode = nextNode.NextNode;
                        }
                        if (getChild)
                            break;

                        // parent node
                        node2 = node2.Parent;
                        level--;

                        nodeInfo = new XNodeInfo { Index = index++, Node = node2, Level = level, Ope = NodeOpe.Close, Selected = false, Skip = false, Stop = false };
                        yield return nodeInfo;
                    }
                    if (stop || stopNode)
                        break;
                }
                if (stop)
                    break;
            }
        }

        // bug il manque une boucle pour obtenir tous les enfants d'un noeud
        public static IEnumerable<XNode> DescendantNodes_bug(IEnumerable<XNode> nodes, Func<XNode, XNodeFilter> filter = null)
        {
            bool stop = false;
            foreach (XNode node1 in nodes)
            {
                XNodeFilter xnodeFilter = XNodeFilter.SelectNode;
                if (filter != null)
                    xnodeFilter = filter(node1);

                if (__trace)
                    pb.Trace.WriteLine("DescendantNodes : source node {0}, filter {1}", node1.zGetPath(), xnodeFilter);

                if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                    break;

                if ((xnodeFilter & XNodeFilter.SkipNode) == XNodeFilter.SkipNode)
                    continue;

                if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                    yield return node1;

                if (__trace)
                    pb.Trace.WriteLine("DescendantNodes : source node {0}", node1.zGetPath());
                XNode node2 = node1;
                XNode sourceNode = node2;
                while (true)
                {
                    // get child
                    if (node2 is XElement)
                    {
                        // first child node
                        XNode childNode = ((XElement)node2).FirstNode;
                        if (childNode != null)
                        {
                            xnodeFilter = XNodeFilter.SelectNode;
                            if (filter != null)
                                xnodeFilter = filter(childNode);
                            if (__trace)
                                pb.Trace.WriteLine("DescendantNodes : child node  {0}, filter {1}", childNode.zGetPath(), xnodeFilter);
                            if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                            {
                                stop = true;
                                break;
                            }
                            if ((xnodeFilter & XNodeFilter.SkipNode) == 0)
                            {
                                node2 = childNode;
                                //_level++;
                                if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                                    yield return node2;
                                else
                                    continue;
                            }
                        }
                    }

                    // get next sibling node or next sibling node of parent node
                    bool getChild = false;
                    bool stopNode = false;
                    while (true)
                    {
                        if (node2 == sourceNode)
                        {
                            stopNode = true;
                            break;
                        }

                        // next sibling node
                        XNode nextNode = node2.NextNode;
                        while (true)
                        {
                            if (nextNode == null)
                                break;

                            xnodeFilter = XNodeFilter.SelectNode;
                            if (filter != null)
                                xnodeFilter = filter(nextNode);

                            if (__trace)
                                pb.Trace.WriteLine("DescendantNodes : next node   {0}, filter {1}", nextNode.zGetPath(), xnodeFilter);

                            if ((xnodeFilter & XNodeFilter.Stop) == XNodeFilter.Stop)
                            {
                                stop = true;
                                break;
                            }

                            if ((xnodeFilter & XNodeFilter.SkipNode) == 0)
                            {
                                node2 = nextNode;
                                if ((xnodeFilter & XNodeFilter.DontSelectNode) == 0)
                                    yield return node2;
                                else
                                {
                                    getChild = true;
                                    break;
                                }
                            }
                            nextNode = nextNode.NextNode;
                        }
                        if (getChild)
                            break;

                        // parent node
                        node2 = node2.Parent;
                        //_level--;
                    }
                    if (stop || stopNode)
                        break;
                }
                if (stop)
                    break;
            }
        }

        public static XNodeFilter TextFilter(XNode node, Func<XNode, XNodeFilter> filter = null)
        {
            XNodeFilter xnodeFilter = XNodeFilter.SelectNode;
            if (filter != null)
                xnodeFilter = filter(node);
            if (!(node is XText))
                xnodeFilter |= XNodeFilter.DontSelectNode;
            return xnodeFilter;
        }

        public static XNodeFilter ImageFilter(XNode node, Func<XNode, XNodeFilter> filter = null)
        {
            XNodeFilter xnodeFilter = XNodeFilter.SelectNode;
            if (filter != null)
                xnodeFilter = filter(node);
            if (!(node is XElement) || ((XElement)node).Name != "img")
                xnodeFilter |= XNodeFilter.DontSelectNode;
            return xnodeFilter;
        }

        public static XNodeFilter FormItemFilter(XNode node, Func<XNode, XNodeFilter> filter = null)
        {
            XNodeFilter xnodeFilter = XNodeFilter.SelectNode;
            if (filter != null)
                xnodeFilter = filter(node);
            if (!(node is XElement))
                xnodeFilter |= XNodeFilter.DontSelectNode;
            else
            {
                XElement xe = (XElement)node;
                // return only element : input, select, button
                if (xe.Name != "input" && xe.Name != "select" && xe.Name != "button")
                    xnodeFilter |= XNodeFilter.DontSelectNode;
            }
            return xnodeFilter;
        }
    }

    public static partial class GlobalExtension
    {
        public static IEnumerable<string> zDescendantTexts(this XElement element, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantTexts(element, filter);
        }

        public static IEnumerable<string> zDescendantTexts(this IEnumerable<XElement> elements, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantTexts(elements, filter);
        }

        public static IEnumerable<XText> zDescendantTextNodes(this XElement element, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantTextNodes(element, filter);
        }

        public static IEnumerable<XText> zDescendantTextNodes(this IEnumerable<XElement> elements, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantTextNodes(elements, filter);
        }

        public static IEnumerable<XNode> zDescendantNodes(this XNode node, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantNodes(node, filter);
        }

        public static IEnumerable<XNode> zDescendantNodes(this IEnumerable<XNode> nodes, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantNodes(nodes, filter);
        }

        public static IEnumerable<XNodeInfo> zDescendantNodesInfos(this XNode node, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantNodesInfos(node, filter);
        }

        public static IEnumerable<XNodeInfo> zDescendantNodesInfos(this IEnumerable<XNode> nodes, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantNodesInfos(nodes, filter);
        }

        public static IEnumerable<XElement> zDescendantFormItems(this XElement element, Func<XNode, XNodeFilter> filter = null)
        {
            return XmlDescendant.DescendantFormItems(element, filter);
        }
    }
}
