namespace pb.Web.Html
{
    // <div>           : OpenTag div, CloseTag div EndTag = false
    // <div ... >      : OpenTag div, Property ..., CloseTag div EndTag = false
    // <div ... />     : OpenTag div, Property ..., CloseTag div EndTag = true
    // <div />         : OpenTag div, CloseTag div EndTag = true
    // </div>          : EndTag div
    // <!DOCTYPE ... > : DocumentType

    public enum HtmlNodeType
    {
        DocumentType = 1,
        OpenTag,                     // <div /> (OpenTag + EndTag)
        CloseTag,                    // <div>   (OpenTag + CloseTag)
        EndTag,                      // </div>  (EndTag)
        Property,
        Comment,
        Text,
        Script
    }

    public class HtmlNode
    {
        public int Index;
        public HtmlNodeType Type;
        public int Line;
        public int Column;
        //private int _line;
        //private int _column;
        //public int Line { get { return _line; } set { } }
        //public int Column { get { return _column; } set { } }
    }

    public class HtmlNodeDocType : HtmlNode
    {
        public string DocType;

        public HtmlNodeDocType()
        {
            Type = HtmlNodeType.DocumentType;
        }
    }

    public class HtmlNodeOpenTag : HtmlNode
    {
        public string Name;
        public bool IsScript = false;

        public HtmlNodeOpenTag()
        {
            Type = HtmlNodeType.OpenTag;
        }
    }

    public class HtmlNodeCloseTag : HtmlNode
    {
        public string Name;
        //public bool EndTag = false;

        public HtmlNodeCloseTag()
        {
            Type = HtmlNodeType.CloseTag;
        }
    }

    public class HtmlNodeEndTag : HtmlNode
    {
        public string Name;

        public HtmlNodeEndTag()
        {
            Type = HtmlNodeType.EndTag;
        }
    }

    public class HtmlNodeProperty : HtmlNode
    {
        public string Name;
        public string Value;
        public char? Quote;

        public HtmlNodeProperty()
        {
            Type = HtmlNodeType.Property;
        }
    }

    public class HtmlNodeComment : HtmlNode
    {
        public string Comment;

        public HtmlNodeComment()
        {
            Type = HtmlNodeType.Comment;
        }
    }

    public class HtmlNodeText : HtmlNode
    {
        public string Text;
        public bool IsTextSeparator;

        public HtmlNodeText()
        {
            Type = HtmlNodeType.Text;
        }
    }

    public class HtmlNodeScript : HtmlNode
    {
        public string Script;

        public HtmlNodeScript()
        {
            Type = HtmlNodeType.Script;
        }
    }
}
