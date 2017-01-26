using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using pb.Data;
using pb.Data.Xml;
using pb.Web;
using pb.Web.Http;

namespace pb.old
{
    public class ImageHtml
    {
        public string Source;
        public string Alt;
        public string Title;
        public string Class;
        public string File;
        public int? ImageWidth;
        public int? ImageHeight;
        public Image Image;

        public ImageHtml()
        {
        }

        public ImageHtml(string source, string alt, string title, string className)
        {
            Source = source;
            Alt = alt;
            Title = title;
            Class = className;
        }

        public ImageHtml(XElement xe, string urlBase)
        {
            Source = zurl.GetUrl(urlBase, xe.zAttribValue("src"));
            Alt = xe.zAttribValue("alt");
            Title = xe.zAttribValue("title");
            Class = xe.zAttribValue("class");
        }

        public override string ToString()
        {
            return string.Format("src \"{0}\" alt \"{1}\" title \"{2}\" class \"{3}\"", Source, Alt, Title, Class);
        }
    }

    public static partial class GlobalExtension
    {
        public static void zResize(this IEnumerable<ImageHtml> htmlImages, int width = 0, int height = 0, bool preserveAspectRatio = true)
        {
            foreach (ImageHtml htmlImage in htmlImages)
                htmlImage.Image = zimg.ResizeImage(htmlImage.Image, width, height, preserveAspectRatio);
        }
    }
}
