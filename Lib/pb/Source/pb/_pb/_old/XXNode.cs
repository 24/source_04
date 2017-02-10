using System;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;

namespace pb.old
{
    public enum XXNodeType
    {
        Text = 1,
        Link,
        Image
    }

    public class XXNode
    {
        public XXNodeType type;
        public XNode node;
    }

    public class XXNodeText : XXNode
    {
        public XText xtext;
        public string text;

        public XXNodeText(XText xt)
        {
            type = XXNodeType.Text;
            xtext = xt;
            text = xt.Value;
        }

        public override string ToString()
        {
            return string.Format("text \"{0}\"", text);
        }
    }

    public class XXNodeLink : XXNode
    {
        public XElement link;
        public string url;
        public string text;
        public string relAttribute; // Specifies the relationship between the current document and the linked document; alternate, author, bookmark, help, license, next, nofollow, noreferrer, prefetch, prev, search, tag
        public string typeAttribute;

        public XXNodeLink(XElement xe)
        {
            if (xe.Name != "a")
                throw new PBException("error creating XXNodeLink with wrong node type \"{0}\"", xe.Name);
            type = XXNodeType.Link;
            link = xe;
            url = xe.zAttribValue("href");
            relAttribute = xe.zAttribValue("rel");
            typeAttribute = xe.zAttribValue("type");
            text = xe.Value;
        }

        public override string ToString()
        {
            return string.Format("link text \"{0}\" url \"{1}\" rel \"{2}\" type \"{3}\"", text, url, relAttribute, typeAttribute);
        }
    }

    public class XXNodeImage : XXNode
    {
        public XElement img;
        public string source;
        public string alt;
        public string title;
        public string className;
        public bool followingImage = false;

        public XXNodeImage(XElement xe)
        {
            if (xe.Name != "img")
                throw new PBException("error creating XXNodeImage with wrong node type \"{0}\"", xe.Name);
            type = XXNodeType.Image;
            img = xe;
            source = xe.zAttribValue("src");
            alt = xe.zAttribValue("alt");
            title = xe.zAttribValue("title");
            className = xe.zAttribValue("className");
        }

        public override string ToString()
        {
            return string.Format("image src \"{0}\" alt \"{1}\" title \"{2}\" class \"{3}\" following image {4}", source, alt, title, className, followingImage);
        }
    }
}
