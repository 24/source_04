using pb.Compiler;
using pb.Data.Mongo;
using pb.Data.OpenXml;
using pb.Web;
using System;
using System.Collections.Generic;

namespace WebData.BlogDemoor
{
    public static class Test_HtmlToDocx
    {
        public static void Test_01(string file)
        {
            string traceFile = file + ".01.trace";
            bool generateCloseTag = true;
            bool disableLineColumn = false;
            bool disableScriptTreatment = false;
            bool useReadAttributeValue_v2 = true;
            bool useTranslateChar = true;
            HtmlReader_v4.ReadFile(file, generateCloseTag: generateCloseTag, disableLineColumn: disableLineColumn, disableScriptTreatment: disableScriptTreatment,
                useReadAttributeValue_v2: useReadAttributeValue_v2, useTranslateChar: useTranslateChar)
                .zSave(traceFile);
        }

        public static void Test_02(string file)
        {
            string traceFile = file + ".02.trace";
            HtmlDocReader.ReadFile(file).zSave(traceFile);
        }

        public static void Test_03(string file)
        {
            string traceFile = file + ".03.trace";
            HtmlDocReader.ReadFile(file).zTraceToFile(traceFile);
        }

        public static void Test_04(string file)
        {
            string traceFile = file + ".04.trace";
            HtmlToDocxElements.ToDocXElements(HtmlDocReader.ReadFile(file)).zSave(traceFile);
        }

        public static void Test_05(string file)
        {
            zOpenXmlDoc.Create(file + ".docx", HtmlToDocxElements.ToDocXElements(HtmlDocReader.ReadFile(file)));
        }
    }

    public static class HtmlToDocxElements
    {
        private static string _pictureDir = @"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images\";
        //private IEnumerable<HtmlDocNode> _nodes = null;

        public static IEnumerable<zDocXElement> ToDocXElements(IEnumerable<HtmlDocNode> nodes)
        {
            foreach (HtmlDocNode node in nodes)
            {
                zDocXElement docXElement = null;
                switch (node.Type)
                {
                    case HtmlDocNodeType.BeginTag:
                        // <p> <a>
                        docXElement = BeginTag((HtmlDocNodeBeginTag)node);
                        break;
                    //case HtmlDocNodeType.EndTag:
                    //    //HtmlDocNodeEndTag
                    //    break;
                    case HtmlDocNodeType.Tag:
                        //
                        //HtmlDocNodeTagImg
                        // <br> <img>
                        docXElement = Tag((HtmlDocNodeTag)node);
                        break;
                    case HtmlDocNodeType.Text:
                        docXElement = new zDocXTextElement { Text = ((HtmlDocNodeText)node).Text };
                        break;
                }
                if (docXElement != null)
                    yield return docXElement;
            }
        }

        private static zDocXElement BeginTag(HtmlDocNodeBeginTag beginTag)
        {
            // <p> <a>
            if (beginTag.Tag == HtmlTagType.P)
                return new zDocXElement { Type = zDocXElementType.Paragraph };
            return null;
        }

        private static zDocXElement Tag(HtmlDocNodeTag tag)
        {
            // <br> <img>
            if (tag.Tag == HtmlTagType.BR)
                return new zDocXElement { Type = zDocXElementType.Line };
            else if (tag.Tag == HtmlTagType.Img)
            {
                HtmlDocNodeTagImg img = (HtmlDocNodeTagImg)tag;
                Uri uri = new Uri(img.Link);
                string file = uri.Segments[uri.Segments.Length - 1];
                // new zDocXPictureElement { File = _pictureDir + file, Width = 300, PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 21800 } }
                zDocXPictureElement pictureElement = new zDocXPictureElement { File = _pictureDir + file };
                foreach (string className in img.ClassList)
                    SetPictureClassParam(pictureElement, className);
                return pictureElement;
            }
            else
                return null;
        }

        private static void SetPictureClassParam(zDocXPictureElement pictureElement, string className)
        {
            // class : leftalign, nonealign, width300, width450
            switch (className.ToLower())
            {
                case "nonealign":
                    pictureElement.PictureDrawing = new zDocXPictureInlineDrawing();
                    break;
                case "leftalign":
                    pictureElement.PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 21800 };
                    break;
                case "width300":
                    pictureElement.Width = 300;
                    break;
                case "width450":
                    pictureElement.Width = 450;
                    break;
                default:
                    throw new pb.PBException($"unknow img class \"{className}\"");
            }
        }
    }
}
