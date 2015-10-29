using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using pb.Data.Xml;

namespace pb.Windows.Forms
{
    public class TreeViewTool
    {
        #region //InitTreeView(TreeView treeView, DataRow row)
        //public static void InitTreeView(TreeView treeView, DataRow row)
        //{

        //    treeView.Nodes.Clear();
        //    if (row != null)
        //    {
        //        SetTreeViewData(treeView.Nodes, row);
        //        treeView.ExpandAll();
        //        if (treeView.Nodes.Count > 0) treeView.SelectedNode = treeView.Nodes[0];
        //    }
        //}
		#endregion

        public static void InitTreeView(TreeView treeView, XElement e)
        {

            treeView.Nodes.Clear();
            if (e != null)
            {
                SetTreeViewData(treeView.Nodes, e);
                treeView.ExpandAll();
                if (treeView.Nodes.Count > 0) treeView.SelectedNode = treeView.Nodes[0];
            }
        }

        #region //SetTreeViewData(TreeNodeCollection tns, DataRow row)
        //private static void SetTreeViewData(TreeNodeCollection tns, DataRow row)
        //{
        //    TreeNode tn = null;
        //    if (row.Table.Columns.Contains("val"))
        //    {
        //        object o = row["val"];
        //        if (o is string)
        //        {
        //            tn = new TreeNode((string)o);
        //            if (row.Table.Columns.Contains("key"))
        //                tn.Tag = row["key"];
        //        }
        //    }

        //    if (tn != null)
        //    {
        //        tns.Add(tn);
        //        tns = tn.Nodes;
        //    }
        //    if (row.Table.ChildRelations.Count > 0)
        //    {
        //        DataRelation dr = row.Table.ChildRelations[0];
        //        DataRow[] childRow = row.GetChildRows(dr);
        //        for (int i = 0; i < childRow.Length; i++)
        //        {
        //            SetTreeViewData(tns, childRow[i]);
        //        }
        //    }
        //}
		#endregion

        #region SetTreeViewData(TreeNodeCollection tns, XElement e)
        private static void SetTreeViewData(TreeNodeCollection tns, XElement xe)
        {
            XAttribute xa = xe.Attribute("val");
            if (xa != null)
            {
                TreeNode tn = new TreeNode(xa.Value);
                xa = xe.Attribute("key");
                if (xa != null) tn.Tag = xa.Value;
                tns.Add(tn);
                tns = tn.Nodes;
            }

            foreach (XElement xe2 in xe.Elements("level"))
                SetTreeViewData(tns, xe2);

        }
        #endregion
    }

    public static class TreeViewExtension
    {
        public static void zAddNode(this TreeView tree, string nodeName, XElement xmlElement, XFormat xFormat)
        {
            TreeNode node = tree.Nodes.Add(nodeName);
            TreeViewAdd(node, xmlElement, xFormat);
        }

        public static void TreeViewAdd(TreeNode node, XElement xeData, XFormat xFormat)
        {
            int nb = xeData.Elements().Count();
            if (nb == 0) return;
            if (xFormat != null && xFormat.HideChild) return;

            if (xFormat == null || !xFormat.NoChildCount)
                node.Text += " (" + nb.ToString() + ")";

            foreach (XElement xeChild in xeData.Elements())
            {
                XFormat xChildFormat = null;
                if (xFormat != null)
                    xChildFormat = xFormat.Element(xeChild.Name.LocalName);

                bool bHide = false;
                string s = null;

                if (xChildFormat != null)
                {
                    if (xChildFormat.Hide)
                        bHide = true;
                    else if (xChildFormat.HideElementIndex != null)
                    {
                        int i1 = xeChild.zGetElementIndex();
                        foreach (int i2 in xChildFormat.HideElementIndex)
                        {
                            if (i1 == i2)
                            {
                                bHide = true;
                                break;
                            }
                        }

                    }
                    if (!bHide)
                        s = xChildFormat.Format(xeChild);
                }

                if (!bHide && s == null)
                    //s = ElementToString(xeChild);
                    s = xeChild.zToString();

                if (!bHide)
                {
                    TreeNode nodeChild = node.Nodes.Add(s);
                    if (xChildFormat != null && xChildFormat.ExpandNode)
                        nodeChild.Expand();
                    TreeViewAdd(nodeChild, xeChild, xChildFormat);
                }
            }
        }
    }

    public static partial class GlobalExtension
    {
        public static string zToString(this XElement xe)
        {
            string s = xe.Name.LocalName + " :";

            bool bFirst = true;

            IEnumerable<XAttribute> attribs = xe.Attributes();
            if (attribs.Count() == 1)
            {
                XAttribute xa = attribs.First();
                if (!bFirst) s += ",";
                s += " " + xa.Value;
                bFirst = false;
            }
            else
            {
                foreach (XAttribute xa in attribs)
                {
                    if (!bFirst) s += ",";
                    s += " " + xa.Name + "=" + xa.Value;
                    bFirst = false;
                }
            }

            foreach (XText xt in xe.Nodes().OfType<XText>())
            {
                if (!bFirst) s += ",";
                s += ", " + xt.Value;
                bFirst = false;
            }

            return s;
        }
    }
}
