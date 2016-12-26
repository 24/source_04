using pb.IO;
using pb.Text;
using System.Collections.Generic;
using System.IO;
using System.Text;

// used node : <p> <br> <a> <img> text
// open <p>
// text ""
// tag <br>
// close <p>
// open <a> href
// tag <img> src class
// close <a>

namespace pb.Web
{
    public enum HtmlDocNodeType
    {
        BeginTag = 1,
        EndTag,
        Tag,
        Text
    }

    public class HtmlDocNode
    {
        public int Index;
        public HtmlDocNodeType Type;
        public int Line;
        public int Column;
    }

    public class HtmlDocNodeBeginTag : HtmlDocNode
    {
        public HtmlTagType Tag;

        public HtmlDocNodeBeginTag()
        {
            Type = HtmlDocNodeType.BeginTag;
        }

        public override string ToString()
        {
            return $"{Index.ToString().PadLeft(3)} {Tag.ToString().ToLower()} (begin)";
        }
    }

    public class HtmlDocNodeBeginTagA : HtmlDocNodeBeginTag
    {
        public string Link;

        public HtmlDocNodeBeginTagA()
        {
            Tag = HtmlTagType.A;
        }

        public override string ToString()
        {
            return $"{Index.ToString().PadLeft(3)} {Tag.ToString().ToLower()} (begin) link \"{Link}\"";
        }
    }

    public class HtmlDocNodeEndTag : HtmlDocNode
    {
        public HtmlTagType Tag;

        public HtmlDocNodeEndTag()
        {
            Type = HtmlDocNodeType.EndTag;
        }

        public override string ToString()
        {
            return $"{Index.ToString().PadLeft(3)} {Tag.ToString().ToLower()} (end)";
        }
    }

    public class HtmlDocNodeTag : HtmlDocNode
    {
        public HtmlTagType Tag;

        public HtmlDocNodeTag()
        {
            Type = HtmlDocNodeType.Tag;
        }

        public override string ToString()
        {
            return $"{Index.ToString().PadLeft(3)} {Tag.ToString().ToLower()}";
        }
    }

    public class HtmlDocNodeTagImg : HtmlDocNodeTag
    {
        public string Link;
        public IEnumerable<string> ClassList;
        public int? Width;

        public HtmlDocNodeTagImg()
        {
            Tag = HtmlTagType.Img;
        }

        public override string ToString()
        {
            return $"{Index.ToString().PadLeft(3)} img link \"{Link}\" class {ClassList.zToStringValues()} width {Width}";
        }
    }

    public class HtmlDocNodeText : HtmlDocNode
    {
        public string Text;

        public HtmlDocNodeText()
        {
            Type = HtmlDocNodeType.Text;
        }

        public override string ToString()
        {
            return $"{Index.ToString().PadLeft(3)} text \"{Text}\"";
        }
    }

    public class HtmlDocReader
    {
        private IEnumerable<HtmlNode> _nodes = null;
        //private HtmlNode _node = null;
        private HtmlDocNode _node = null;
        private int _nodeIndex = 0;

        public IEnumerable<HtmlDocNode> Read()
        {
            foreach (HtmlNode node in _nodes)
            {
                switch (node.Type)
                {
                    case HtmlNodeType.OpenTag:
                        OpenTag((HtmlNodeOpenTag)node);
                        break;
                    case HtmlNodeType.CloseTag:
                        if (_node != null)
                        {
                            yield return _node;
                            _node = null;
                        }
                        break;
                    case HtmlNodeType.Property:
                        Property((HtmlNodeProperty)node);
                        break;
                    case HtmlNodeType.EndTag:
                        // for tag <img ... />
                        if (_node != null)
                        {
                            yield return _node;
                            _node = null;
                        }
                        EndTag((HtmlNodeEndTag)node);
                        if (_node != null)
                        {
                            yield return _node;
                            _node = null;
                        }
                        break;
                    case HtmlNodeType.Text:
                        HtmlNodeText nodeText = (HtmlNodeText)node;
                        if (!nodeText.IsTextSeparator)
                            yield return new HtmlDocNodeText() { Text = nodeText.Text, Index = ++_nodeIndex, Line = node.Line, Column = node.Column };
                        break;
                }
            }
        }

        private void OpenTag(HtmlNodeOpenTag openTag)
        {
            _node = null;
            // tag : <P> <br> <a> <img>
            switch (openTag.Name.ToLower())
            {
                case "p":
                    _node = new HtmlDocNodeBeginTag() { Tag = HtmlTagType.P, Index = ++_nodeIndex, Line = openTag.Line, Column = openTag.Column };
                    break;
                case "br":
                    _node = new HtmlDocNodeTag() { Tag = HtmlTagType.BR, Index = ++_nodeIndex, Line = openTag.Line, Column = openTag.Column };
                    break;
                case "a":
                    _node = new HtmlDocNodeBeginTagA() { Index = ++_nodeIndex, Line = openTag.Line, Column = openTag.Column };
                    break;
                case "img":
                    _node = new HtmlDocNodeTagImg() { Index = ++_nodeIndex, Line = openTag.Line, Column = openTag.Column };
                    break;
            }
        }

        private void Property(HtmlNodeProperty property)
        {
            if (_node is HtmlDocNodeBeginTagA)
            {
                if (property.Name.ToLower() == "href")
                    ((HtmlDocNodeBeginTagA)_node).Link = property.Value;
            }
            else if (_node is HtmlDocNodeTagImg)
            {
                if (property.Name.ToLower() == "src")
                    ((HtmlDocNodeTagImg)_node).Link = property.Value;
                else if (property.Name.ToLower() == "class")
                    ((HtmlDocNodeTagImg)_node).ClassList = zsplit.Split(property.Value, ' ', true);
                else if (property.Name.ToLower() == "width")
                {
                    int? width = property.Value.zTryParseAs<int?>();
                    if (width == null)
                        Trace.WriteLine($"unknow width \"{property.Value}\"");
                    ((HtmlDocNodeTagImg)_node).Width = width;
                }
            }
        }

        private void EndTag(HtmlNodeEndTag endTag)
        {
            _node = null;
            switch (endTag.Name.ToLower())
            {
                case "p":
                    _node = new HtmlDocNodeEndTag() { Tag = HtmlTagType.P, Index = ++_nodeIndex, Line = endTag.Line, Column = endTag.Column };
                    break;
                case "a":
                    _node = new HtmlDocNodeEndTag() { Tag = HtmlTagType.A, Index = ++_nodeIndex, Line = endTag.Line, Column = endTag.Column };
                    break;
            }
        }

        public static IEnumerable<HtmlDocNode> Read(TextReader textReader)
        {
            HtmlDocReader reader = new HtmlDocReader();
            bool disableLineColumn = false;
            bool disableScriptTreatment = false;
            bool useReadAttributeValue_v2 = true;
            bool useTranslateChar = true;
            reader._nodes = HtmlReader_v4.Read(textReader, generateCloseTag: true, disableLineColumn: disableLineColumn, disableScriptTreatment: disableScriptTreatment,
                useReadAttributeValue_v2: useReadAttributeValue_v2, useTranslateChar: useTranslateChar);
            return reader.Read();
        }

        public static IEnumerable<HtmlDocNode> ReadFile(string file, Encoding encoding = null)
        {
            using (StreamReader sr = zfile.OpenText(file, encoding))
            {
                // attention return Read() generate exception Cannot read from a closed TextReader. (System.ObjectDisposedException)
                //return Read(sr);
                foreach (HtmlDocNode node in Read(sr))
                {
                    yield return node;
                }
            }
        }

        public static IEnumerable<HtmlDocNode> ReadString(string html)
        {
            using (StringReader sr = new StringReader(html))
            {
                // attention return Read() generate exception Cannot read from a closed TextReader. (System.ObjectDisposedException)
                //return Read(sr);
                foreach (HtmlDocNode node in Read(sr))
                {
                    yield return node;
                }
            }
        }
    }
}
