using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace Print.download
{
    public class HtmlTag
    {
        #region variable
        private static SortedList<string, HtmlTagTypeEnum> gslTagType;
        private static SortedList<HtmlTagTypeEnum, HtmlTag> gslTagList;
        private static Trace _tr = Trace.CurrentTrace;

        public string TagName;
        public HtmlTagTypeEnum TagType;
        public HtmlTagCategoryEnum TagCategory;
        public HtmlBoundTypeEnum StartBoundType;
        public HtmlBoundTypeEnum EndBoundType;
        public bool Empty;
        public bool Deprecated;                      // Deprecated = Obsolète
        public HtmlDTDTypeEnum DTDType;              // Document Type Definition
        //public bool Head;
        #endregion

        public HtmlTag(string TagName, HtmlTagTypeEnum TagType, HtmlBoundTypeEnum StartTagType, HtmlBoundTypeEnum EndTagType, bool Empty, bool Deprecated, HtmlDTDTypeEnum DTDType)
        {
            this.TagName = TagName;
            this.TagType = TagType;
            this.StartBoundType = StartTagType;
            this.EndBoundType = EndTagType;
            this.Empty = Empty;
            this.Deprecated = Deprecated;
            this.DTDType = DTDType;
            this.TagCategory = HtmlTagCategoryEnum.NoCategory;
        }

        public HtmlTag(string TagName, HtmlTagTypeEnum TagType, HtmlBoundTypeEnum StartTagType, HtmlBoundTypeEnum EndTagType, bool Empty, bool Deprecated, HtmlDTDTypeEnum DTDType,
            HtmlTagCategoryEnum TagCategory)
        {
            this.TagName = TagName;
            this.TagType = TagType;
            this.StartBoundType = StartTagType;
            this.EndBoundType = EndTagType;
            this.Empty = Empty;
            this.Deprecated = Deprecated;
            this.DTDType = DTDType;
            this.TagCategory = TagCategory;
        }

        static HtmlTag()
        {
            _tr.WriteLine("HtmlTag static constructor");
            InitTagType();
            InitTagList();
        }

        public static HtmlTagTypeEnum GetHtmlTagType(string sTagName)
        {
            int i = gslTagType.IndexOfKey(sTagName);
            if (i != -1) return gslTagType.Values[i];
            return HtmlTagTypeEnum.Unknow;
        }

        public static HtmlTag GetHtmlTag(HtmlTagTypeEnum TagType)
        {
            int i = gslTagList.IndexOfKey(TagType);
            if (i != -1) return gslTagList.Values[i];
            return null;
        }

        public static void InitTagType()
        {
            gslTagType = new SortedList<string, HtmlTagTypeEnum>();
            gslTagType.Add("a", HtmlTagTypeEnum.A);
            gslTagType.Add("abbr", HtmlTagTypeEnum.Abbr);
            gslTagType.Add("acronym", HtmlTagTypeEnum.Acronym);
            gslTagType.Add("address", HtmlTagTypeEnum.Address);
            gslTagType.Add("applet", HtmlTagTypeEnum.Applet);
            gslTagType.Add("area", HtmlTagTypeEnum.Area);
            gslTagType.Add("b", HtmlTagTypeEnum.B);
            gslTagType.Add("base", HtmlTagTypeEnum.Base);
            gslTagType.Add("basefont", HtmlTagTypeEnum.BaseFont);
            gslTagType.Add("bdo", HtmlTagTypeEnum.Bdo);
            gslTagType.Add("big", HtmlTagTypeEnum.Big);
            gslTagType.Add("blockquote", HtmlTagTypeEnum.Blockquote);
            gslTagType.Add("body", HtmlTagTypeEnum.Body);
            gslTagType.Add("br", HtmlTagTypeEnum.BR);
            gslTagType.Add("button", HtmlTagTypeEnum.Button);
            gslTagType.Add("caption", HtmlTagTypeEnum.Caption);
            gslTagType.Add("center", HtmlTagTypeEnum.Center);
            gslTagType.Add("cite", HtmlTagTypeEnum.Cite);
            gslTagType.Add("code", HtmlTagTypeEnum.Code);
            gslTagType.Add("col", HtmlTagTypeEnum.Col);
            gslTagType.Add("colgroup", HtmlTagTypeEnum.ColGroup);
            gslTagType.Add("dd", HtmlTagTypeEnum.DD);
            gslTagType.Add("del", HtmlTagTypeEnum.Del);
            gslTagType.Add("dfn", HtmlTagTypeEnum.Dfn);
            gslTagType.Add("dir", HtmlTagTypeEnum.Dir);
            gslTagType.Add("div", HtmlTagTypeEnum.Div);
            gslTagType.Add("dl", HtmlTagTypeEnum.DL);
            gslTagType.Add("dt", HtmlTagTypeEnum.DT);
            gslTagType.Add("em", HtmlTagTypeEnum.EM);
            gslTagType.Add("fieldset", HtmlTagTypeEnum.FieldSet);
            gslTagType.Add("font", HtmlTagTypeEnum.Font);
            gslTagType.Add("form", HtmlTagTypeEnum.Form);
            gslTagType.Add("frame", HtmlTagTypeEnum.Frame);
            gslTagType.Add("frameset", HtmlTagTypeEnum.FrameSet);
            gslTagType.Add("h1", HtmlTagTypeEnum.H1);
            gslTagType.Add("h2", HtmlTagTypeEnum.H2);
            gslTagType.Add("h3", HtmlTagTypeEnum.H3);
            gslTagType.Add("h4", HtmlTagTypeEnum.H4);
            gslTagType.Add("h5", HtmlTagTypeEnum.H5);
            gslTagType.Add("h6", HtmlTagTypeEnum.H6);
            gslTagType.Add("head", HtmlTagTypeEnum.Head);
            gslTagType.Add("hr", HtmlTagTypeEnum.Hr);
            gslTagType.Add("html", HtmlTagTypeEnum.Html);
            gslTagType.Add("i", HtmlTagTypeEnum.I);
            gslTagType.Add("iframe", HtmlTagTypeEnum.IFrame);
            gslTagType.Add("img", HtmlTagTypeEnum.Img);
            gslTagType.Add("input", HtmlTagTypeEnum.Input);
            gslTagType.Add("ins", HtmlTagTypeEnum.Ins);
            gslTagType.Add("isindex", HtmlTagTypeEnum.IsIndex);
            gslTagType.Add("kbd", HtmlTagTypeEnum.Kbd);
            gslTagType.Add("label", HtmlTagTypeEnum.Label);
            gslTagType.Add("legend", HtmlTagTypeEnum.Legend);
            gslTagType.Add("li", HtmlTagTypeEnum.LI);
            gslTagType.Add("link", HtmlTagTypeEnum.Link);
            gslTagType.Add("map", HtmlTagTypeEnum.Map);
            gslTagType.Add("menu", HtmlTagTypeEnum.Menu);
            gslTagType.Add("meta", HtmlTagTypeEnum.Meta);
            gslTagType.Add("noframes", HtmlTagTypeEnum.NoFrames);
            gslTagType.Add("noscript", HtmlTagTypeEnum.NoScript);
            gslTagType.Add("object", HtmlTagTypeEnum.Object);
            gslTagType.Add("ol", HtmlTagTypeEnum.OL);
            gslTagType.Add("optgroup", HtmlTagTypeEnum.OptGroup);
            gslTagType.Add("option", HtmlTagTypeEnum.Option);
            gslTagType.Add("p", HtmlTagTypeEnum.P);
            gslTagType.Add("param", HtmlTagTypeEnum.Param);
            gslTagType.Add("pre", HtmlTagTypeEnum.Pre);
            gslTagType.Add("q", HtmlTagTypeEnum.Q);
            gslTagType.Add("s", HtmlTagTypeEnum.S);
            gslTagType.Add("samp", HtmlTagTypeEnum.Samp);
            gslTagType.Add("script", HtmlTagTypeEnum.Script);
            gslTagType.Add("select", HtmlTagTypeEnum.Select);
            gslTagType.Add("small", HtmlTagTypeEnum.Small);
            gslTagType.Add("span", HtmlTagTypeEnum.Span);
            gslTagType.Add("strike", HtmlTagTypeEnum.Strike);
            gslTagType.Add("strong", HtmlTagTypeEnum.Strong);
            gslTagType.Add("style", HtmlTagTypeEnum.Style);
            gslTagType.Add("sub", HtmlTagTypeEnum.Sub);
            gslTagType.Add("sup", HtmlTagTypeEnum.Sup);
            gslTagType.Add("table", HtmlTagTypeEnum.Table);
            gslTagType.Add("tbody", HtmlTagTypeEnum.TBody);
            gslTagType.Add("td", HtmlTagTypeEnum.TD);
            gslTagType.Add("textarea", HtmlTagTypeEnum.TextArea);
            gslTagType.Add("tfoot", HtmlTagTypeEnum.TFoot);
            gslTagType.Add("th", HtmlTagTypeEnum.TH);
            gslTagType.Add("thead", HtmlTagTypeEnum.THead);
            gslTagType.Add("title", HtmlTagTypeEnum.Title);
            gslTagType.Add("tr", HtmlTagTypeEnum.TR);
            gslTagType.Add("tt", HtmlTagTypeEnum.TT);
            gslTagType.Add("u", HtmlTagTypeEnum.U);
            gslTagType.Add("ul", HtmlTagTypeEnum.UL);
            gslTagType.Add("var", HtmlTagTypeEnum.Var);
        }

        public static void InitTagList()
        {
            gslTagList = new SortedList<HtmlTagTypeEnum, HtmlTag>();
            //gslTagList.Add(HtmlTagTypeEnum.Unknow, new HtmlTag("unknow", HtmlTagTypeEnum.Unknow, HtmlBoundTypeEnum.Optional, HtmlBoundTypeEnum.Forbidden, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Unknow, new HtmlTag("unknow", HtmlTagTypeEnum.Unknow, HtmlBoundTypeEnum.Optional, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.A, new HtmlTag("a", HtmlTagTypeEnum.A, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Abbr, new HtmlTag("abbr", HtmlTagTypeEnum.Abbr, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Acronym, new HtmlTag("acronym", HtmlTagTypeEnum.Acronym, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Address, new HtmlTag("address", HtmlTagTypeEnum.Address, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Applet, new HtmlTag("applet", HtmlTagTypeEnum.Applet, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Area, new HtmlTag("area", HtmlTagTypeEnum.Area, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.B, new HtmlTag("b", HtmlTagTypeEnum.B, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Base, new HtmlTag("base", HtmlTagTypeEnum.Base, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.BaseFont, new HtmlTag("basefont", HtmlTagTypeEnum.BaseFont, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Bdo, new HtmlTag("bdo", HtmlTagTypeEnum.Bdo, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Big, new HtmlTag("big", HtmlTagTypeEnum.Big, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Blockquote, new HtmlTag("blockquote", HtmlTagTypeEnum.Blockquote, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Body, new HtmlTag("body", HtmlTagTypeEnum.Body, HtmlBoundTypeEnum.Optional, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.BR, new HtmlTag("br", HtmlTagTypeEnum.BR, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Button, new HtmlTag("button", HtmlTagTypeEnum.Button, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Caption, new HtmlTag("caption", HtmlTagTypeEnum.Caption, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Center, new HtmlTag("center", HtmlTagTypeEnum.Center, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Cite, new HtmlTag("cite", HtmlTagTypeEnum.Cite, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Code, new HtmlTag("code", HtmlTagTypeEnum.Code, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Col, new HtmlTag("col", HtmlTagTypeEnum.Col, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.ColGroup, new HtmlTag("colgroup", HtmlTagTypeEnum.ColGroup, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.DD, new HtmlTag("dd", HtmlTagTypeEnum.DD, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.DefinitionList));
            gslTagList.Add(HtmlTagTypeEnum.Del, new HtmlTag("del", HtmlTagTypeEnum.Del, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Dfn, new HtmlTag("dfn", HtmlTagTypeEnum.Dfn, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Dir, new HtmlTag("dir", HtmlTagTypeEnum.Dir, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Div, new HtmlTag("div", HtmlTagTypeEnum.Div, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.DL, new HtmlTag("dl", HtmlTagTypeEnum.DL, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.DefinitionList));
            gslTagList.Add(HtmlTagTypeEnum.DT, new HtmlTag("dt", HtmlTagTypeEnum.DT, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.DefinitionList));
            gslTagList.Add(HtmlTagTypeEnum.EM, new HtmlTag("em", HtmlTagTypeEnum.EM, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.FieldSet, new HtmlTag("fieldset", HtmlTagTypeEnum.FieldSet, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Font, new HtmlTag("font", HtmlTagTypeEnum.Font, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Form, new HtmlTag("form", HtmlTagTypeEnum.Form, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Frame, new HtmlTag("frame", HtmlTagTypeEnum.Frame, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.FramesetDTD));
            gslTagList.Add(HtmlTagTypeEnum.FrameSet, new HtmlTag("frameset", HtmlTagTypeEnum.FrameSet, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.FramesetDTD));
            gslTagList.Add(HtmlTagTypeEnum.H1, new HtmlTag("h1", HtmlTagTypeEnum.H1, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.H2, new HtmlTag("h2", HtmlTagTypeEnum.H2, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.H3, new HtmlTag("h3", HtmlTagTypeEnum.H3, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.H4, new HtmlTag("h4", HtmlTagTypeEnum.H4, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.H5, new HtmlTag("h5", HtmlTagTypeEnum.H5, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.H6, new HtmlTag("h6", HtmlTagTypeEnum.H6, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Head, new HtmlTag("head", HtmlTagTypeEnum.Head, HtmlBoundTypeEnum.Optional, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Hr, new HtmlTag("hr", HtmlTagTypeEnum.Hr, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Html, new HtmlTag("html", HtmlTagTypeEnum.Html, HtmlBoundTypeEnum.Optional, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.I, new HtmlTag("i", HtmlTagTypeEnum.I, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.IFrame, new HtmlTag("iframe", HtmlTagTypeEnum.IFrame, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Img, new HtmlTag("img", HtmlTagTypeEnum.Img, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Input, new HtmlTag("input", HtmlTagTypeEnum.Input, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Ins, new HtmlTag("ins", HtmlTagTypeEnum.Ins, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.IsIndex, new HtmlTag("isindex", HtmlTagTypeEnum.IsIndex, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Kbd, new HtmlTag("kbd", HtmlTagTypeEnum.Kbd, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Label, new HtmlTag("label", HtmlTagTypeEnum.Label, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Legend, new HtmlTag("legend", HtmlTagTypeEnum.Legend, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.LI, new HtmlTag("li", HtmlTagTypeEnum.LI, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Link, new HtmlTag("link", HtmlTagTypeEnum.Link, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Map, new HtmlTag("map", HtmlTagTypeEnum.Map, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Menu, new HtmlTag("menu", HtmlTagTypeEnum.Menu, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Meta, new HtmlTag("meta", HtmlTagTypeEnum.Meta, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Head));
            gslTagList.Add(HtmlTagTypeEnum.NoFrames, new HtmlTag("noframes", HtmlTagTypeEnum.NoFrames, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.FramesetDTD));
            gslTagList.Add(HtmlTagTypeEnum.NoScript, new HtmlTag("noscript", HtmlTagTypeEnum.NoScript, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Object, new HtmlTag("object", HtmlTagTypeEnum.Object, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.OL, new HtmlTag("ol", HtmlTagTypeEnum.OL, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.OptGroup, new HtmlTag("optgroup", HtmlTagTypeEnum.OptGroup, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Option, new HtmlTag("option", HtmlTagTypeEnum.Option, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.P, new HtmlTag("p", HtmlTagTypeEnum.P, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Param, new HtmlTag("param", HtmlTagTypeEnum.Param, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Forbidden, true, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Pre, new HtmlTag("pre", HtmlTagTypeEnum.Pre, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Q, new HtmlTag("q", HtmlTagTypeEnum.Q, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.S, new HtmlTag("s", HtmlTagTypeEnum.S, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Samp, new HtmlTag("samp", HtmlTagTypeEnum.Samp, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Script, new HtmlTag("script", HtmlTagTypeEnum.Script, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Select, new HtmlTag("select", HtmlTagTypeEnum.Select, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Small, new HtmlTag("small", HtmlTagTypeEnum.Small, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Span, new HtmlTag("span", HtmlTagTypeEnum.Span, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Strike, new HtmlTag("strike", HtmlTagTypeEnum.Strike, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.Strong, new HtmlTag("strong", HtmlTagTypeEnum.Strong, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Style, new HtmlTag("style", HtmlTagTypeEnum.Style, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Head));
            gslTagList.Add(HtmlTagTypeEnum.Sub, new HtmlTag("sub", HtmlTagTypeEnum.Sub, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Sup, new HtmlTag("sup", HtmlTagTypeEnum.Sup, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Table, new HtmlTag("table", HtmlTagTypeEnum.Table, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.TBody, new HtmlTag("tbody", HtmlTagTypeEnum.TBody, HtmlBoundTypeEnum.Optional, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.TD, new HtmlTag("td", HtmlTagTypeEnum.TD, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.TextArea, new HtmlTag("textarea", HtmlTagTypeEnum.TextArea, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.TFoot, new HtmlTag("tfoot", HtmlTagTypeEnum.TFoot, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.TH, new HtmlTag("th", HtmlTagTypeEnum.TH, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.THead, new HtmlTag("thead", HtmlTagTypeEnum.THead, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.Title, new HtmlTag("title", HtmlTagTypeEnum.Title, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Head));
            gslTagList.Add(HtmlTagTypeEnum.TR, new HtmlTag("tr", HtmlTagTypeEnum.TR, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Optional, false, false, HtmlDTDTypeEnum.NoDTD, HtmlTagCategoryEnum.Table));
            gslTagList.Add(HtmlTagTypeEnum.TT, new HtmlTag("tt", HtmlTagTypeEnum.TT, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.U, new HtmlTag("u", HtmlTagTypeEnum.U, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, true, HtmlDTDTypeEnum.LooseDTD));
            gslTagList.Add(HtmlTagTypeEnum.UL, new HtmlTag("ul", HtmlTagTypeEnum.UL, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
            gslTagList.Add(HtmlTagTypeEnum.Var, new HtmlTag("var", HtmlTagTypeEnum.Var, HtmlBoundTypeEnum.Required, HtmlBoundTypeEnum.Required, false, false, HtmlDTDTypeEnum.NoDTD));
        }
    }

    public class HtmlXml : IDisposable
    {
        // à faire :
        //  - comment gérer le PreserveWhitespace dans XDocument ?
        //  - les exceptions générées lors de la création d'un attribut sont capturées : mettre une option pour ne pas les capturées
        //  - pb dans TagBegin on appel HtmlTag.GetHtmlTag() et gCurrentTreeNode prend la valeur de gCurrentNode que si tag.EndBoundType != HtmlBoundTypeEnum.Forbidden
        //    mais dans TagEnd on recherche quand meme le parent.
        //  - pb le type de document <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
        //    est stocké sous forme de texte dans <head>
        //  - pb les attributs de <html>, <head>, <body> sont perdus
        //  - pb dans le xml les fin de ligne ne sont pas correctes
        //  - ajouter un paramètre pour supprimer ou pas les noeuds de texte ne contenant que des séparateurs IsSeparator() " \r\n\t"


        #region variable
        HTMLReader gHTMLReader;
        private bool gbGenerateXmlNodeOnly = false;  // si true ne crée pas les noeuds texte et commentaire
        private bool gbNormalizeXml = true;

        private bool gbNoTag = false;
        private bool gbBody = false;
        private bool gbTitle = false;
        private Stack<HtmlTable> gTableStack = null;
        private HtmlTable gTable = null;
        //private XXNode gLastPNode = null;
        private bool gbReadCommentInText = false;

        private XmlDocument gXmlDocument = null;
        private XDocument gXDocument = null;
        private XXNode gDocumentNode = null;

        private XXNode gCurrentNode = null;
        private XXNode gCurrentTreeNode = null;
        private XXNode gHtmlNode = null;
        private XXNode gHeadNode = null;
        private XXNode gBodyNode = null;
        private XXNode gTitleNode = null;
        private Stack<XXNode> gDefinitionListStack = null;
        private XXNode gDefinitionList = null;

        private static Regex gReplace = new Regex(@"[/,;?@!<>\\\[\]\-\*\(\)\+\:\'" + "\\\"]", RegexOptions.Compiled);
        private static Regex gCommentCorrection = new Regex("--+", RegexOptions.Compiled);
        #endregion

        #region constructor
        #region HtmlXml(HTMLReader HTMLReader)
        public HtmlXml(HTMLReader HTMLReader)
        {
            gHTMLReader = HTMLReader;
        }
        #endregion

        #region HtmlXml(string sUrl_Path)
        public HtmlXml(string sUrl_Path)
        {
            gHTMLReader = new HTMLReader(sUrl_Path);
        }
        #endregion

        #region HtmlXml(TextReader tr)
        public HtmlXml(TextReader tr)
        {
            gHTMLReader = new HTMLReader(tr);
        }
        #endregion
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (gHTMLReader != null)
            {
                gHTMLReader.Dispose();
                gHTMLReader = null;
            }
            gXmlDocument = null;
        }
        #endregion

        #region property ...
        #region GenerateXmlNodeOnly
        public bool GenerateXmlNodeOnly
        {
            get { return gbGenerateXmlNodeOnly; }
            set { gbGenerateXmlNodeOnly = value; }
        }
        #endregion

        #region NormalizeXml
        public bool NormalizeXml
        {
            get { return gbNormalizeXml; }
            set { gbNormalizeXml = value; }
        }
        #endregion

        #region ReadCommentInText
        public bool ReadCommentInText
        {
            get { return gbReadCommentInText; }
            set { gbReadCommentInText = value; }
        }
        #endregion
        #endregion

        #region CreateElement
        private XXNode CreateElement(string name)
        {
            XXNode node = new XXNode();
            if (gXmlDocument != null)
            {
                XmlElement element = gXmlDocument.CreateElement(name);
                node.XmlNode = element;
            }
            if (gXDocument != null)
            {
                XElement element = new XElement(name);
                node.XNode = element;
            }
            return node;
        }
        #endregion

        #region AddElement
        private void AddElement(XXNode parent, XXNode child)
        {
            if (parent.XmlNode != null)
                parent.XmlNode.AppendChild(child.XmlNode);
            XNode xnode = parent.XNode;
            if (xnode != null)
            {
                if (xnode is XElement)
                    ((XElement)xnode).Add(child.XNode);
                else if (xnode is XDocument)
                    ((XDocument)xnode).Add(child.XNode);
                else
                    throw new PBException("error generating XDocument node is neither a XElement nor a XDocument");
            }
        }
        #endregion

        #region AddElement
        private void AddElement(XXNode parent, string element)
        {
            if (parent.XmlNode != null)
            {
                XmlElement node = gXmlDocument.CreateElement(element);
                parent.XmlNode.AppendChild(node);
            }
            if (parent.XNode != null)
            {
                if (!(parent.XNode is XElement)) throw new PBException("error generating XDocument node is not a XElement");
                XElement node = new XElement(element);
                ((XElement)parent.XNode).Add(node);
            }
        }
        #endregion

        #region AddAttribute
        private void AddAttribute(XXNode parent, string name, string value)
        {
            if (gXmlDocument != null)
            {
                if (parent.XmlNode.Attributes.GetNamedItem(name) == null)
                {
                    XmlAttribute attrib = gXmlDocument.CreateAttribute(name);
                    if (value == null) value = "";
                    attrib.Value = value;
                    parent.XmlNode.Attributes.Append(attrib);
                }
            }
            if (gXDocument != null)
            {
                XElement xeParent = (XElement)parent.XNode;
                if (xeParent.Attribute(name) == null)
                {
                    if (value == null) value = "";
                    XAttribute attrib = new XAttribute(name, value);
                    xeParent.Add(attrib);
                }
            }
        }
        #endregion

        #region IsSeparator
        private static bool IsSeparator(string text)
        {
            foreach (char c in text)
            {
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n') return false;
            }
            return true;
        }
        #endregion

        #region AddText
        private void AddText(XXNode parent, string text)
        {
            if (IsSeparator(text)) return;
            if (gXmlDocument != null)
            {
                //XmlText node = gXmlDocument.CreateTextNode("text");
                //node.Value = text;
                XmlText node = gXmlDocument.CreateTextNode(text);
                parent.XmlNode.AppendChild(node);
            }
            if (gXDocument != null)
            {
                XText node = new XText(text);
                ((XElement)parent.XNode).Add(node);
            }
        }
        #endregion

        #region AddComment
        private void AddComment(XXNode parent, string comment)
        {
            if (gXmlDocument != null)
            {
                XmlComment node = gXmlDocument.CreateComment(comment);
                parent.XmlNode.AppendChild(node);
            }
            if (gXDocument != null)
            {
                XComment node = new XComment(comment);
                ((XElement)parent.XNode).Add(node);
            }
        }
        #endregion

        #region GetParentXXNode
        private static XXNode GetParentXXNode(XXNode node)
        {
            XXNode parentNode = new XXNode();
            if (node.XmlNode != null) parentNode.XmlNode = node.XmlNode.ParentNode;
            if (node.XNode != null) parentNode.XNode = node.XNode.Parent;
            return parentNode;
        }
        #endregion

        #region GetParentXXNodeByName
        private static XXNode GetParentXXNodeByName(XXNode node, string name)
        {
            bool found = false;
            XXNode node2 = new XXNode();

            XmlNode xmlNode = node.XmlNode;
            while (xmlNode != null)
            {
                if (xmlNode.Name == name)
                {
                    node2.XmlNode = xmlNode;
                    found = true;
                    break;
                }
                xmlNode = xmlNode.ParentNode;
            }

            if (node.XNode != null)
            {
                if (!(node.XNode is XElement)) throw new PBException("error generating XDocument node is not a XElement");
                XElement element = (XElement)node.XNode;
                while (element != null)
                {
                    if (element.Name == name)
                    {
                        node2.XNode = element;
                        found = true;
                        break;
                    }
                    element = element.Parent;
                }
            }

            if (found)
                return node2;
            else
                return null;
        }
        #endregion

        #region GenerateXmlDocument
        public XmlDocument GenerateXmlDocument()
        {
            gXmlDocument = new XmlDocument();
            gDocumentNode = new XXNode();
            gDocumentNode.XmlNode = gXmlDocument;

            GenerateXml();
            return gXmlDocument;
        }
        #endregion

        #region GenerateXDocument
        public XDocument GenerateXDocument()
        {
            gXDocument = new XDocument();
            gDocumentNode = new XXNode();
            gDocumentNode.XNode = gXDocument;

            GenerateXml();
            return gXDocument;
        }
        #endregion

        #region GenerateXml
        private void GenerateXml()
        {
            // gbNormalizeXml = true :
            //   - les tag html, head, title et body sont créés automatiquement
            //   - les tag html, head, title et body rencontrés ne sont pas pris en compte
            //   - seul les tag title et meta sont mis dans la partie head les autre tag sont mis dans la partie body
            //   - si un tag meta est placé après le début de la partie body, ce tag reste dans la partie body
            //   - seul le premier tag title est pris en compte et placé dans la partie head, les autre tag title ne sont pas pris en compte

            try
            {
                //cTrace.Trace("GenerateXml NewGenerateXml  : {0}", XmlConfig.CurrentConfig.Get("NewGenerateXml"));

                InitXml();

                gTableStack = new Stack<HtmlTable>();
                gTable = null;

                gDefinitionListStack = new Stack<XXNode>();
                gDefinitionList = null;

                gbNoTag = false;
                gbBody = false;
                gbTitle = false;
                while (gHTMLReader.Read())
                {
                    if (gHTMLReader.IsText || gHTMLReader.IsComment)
                    {
                        if (gHTMLReader.IsText && !gHTMLReader.IsTextSeparator && !gbBody)
                        {
                            gbBody = true;
                            gCurrentNode = gCurrentTreeNode = gBodyNode;
                        }
                        if (!gbGenerateXmlNodeOnly)
                        {
                            //if (string.Compare(XmlConfig.CurrentConfig.Get("NewGenerateXml"), "true", true) != 0)
                            if (gbReadCommentInText)
                            {
                                //XmlText text = gXmlDocument.CreateTextNode("text");
                                //text.Value = gHTMLReader.Value;
                                //gCurrentNode.AppendChild(text);
                                AddText(gCurrentNode, gHTMLReader.Value);
                            }
                            else
                            {
                                if (gHTMLReader.IsText)
                                {
                                    //XmlText text = gXmlDocument.CreateTextNode("text");
                                    //text.Value = gHTMLReader.Value;
                                    //gCurrentNode.AppendChild(text);
                                    AddText(gCurrentNode, gHTMLReader.Value);
                                }
                                else
                                {
                                    string s = gHTMLReader.Value;
                                    s = gCommentCorrection.Replace(s, "-");
                                    if (s.EndsWith("-")) s += " ";
                                    //XmlComment comment = gXmlDocument.CreateComment(s);
                                    //gCurrentNode.AppendChild(comment);
                                    AddComment(gCurrentNode, s);
                                }
                            }
                        }
                    }
                    else if (gHTMLReader.IsDocType)
                    {
                        //XmlAttribute attrib = gXmlDocument.CreateAttribute("doctype");
                        //attrib.Value = gHTMLReader.DocType;
                        //gHtmlNode.Attributes.Append(attrib);
                        AddAttribute(gHtmlNode, "doctype", gHTMLReader.DocType);
                    }
                    else if (gHTMLReader.IsProperty)
                    {
                        if (gbGenerateXmlNodeOnly || gbNoTag) continue;
                        try
                        {
                            string sPropertyName = gHTMLReader.PropertyName;
                            //sPropertyName = sPropertyName.Replace("\"", "");
                            //sPropertyName = sPropertyName.Replace("/", "");
                            //sPropertyName = sPropertyName.Replace("\\", "");
                            //sPropertyName = sPropertyName.Replace("-", "");
                            //sPropertyName = sPropertyName.Replace(",", "");
                            sPropertyName = gReplace.Replace(sPropertyName, "");
                            sPropertyName = sPropertyName.ToLower();
                            if (sPropertyName == "") sPropertyName = "__value";
                            //XmlAttribute attrib = gXmlDocument.CreateAttribute(sPropertyName);
                            //attrib.Value = gHTMLReader.PropertyValue;
                            //gCurrentNode.Attributes.Append(attrib);
                            AddAttribute(gCurrentNode, sPropertyName, gHTMLReader.PropertyValue);
                            if (gHTMLReader.IsMarkBeginEnd) TagEnd(gHTMLReader.MarkName.ToLower());
                        }
                        catch
                        {
                        }
                    }
                    else if (gHTMLReader.IsMarkBeginEnd)
                    {
                        //TagBegin(gHTMLReader.MarkName.ToLower(), true);

                        string sTagName = gHTMLReader.MarkName.ToLower();
                        sTagName = gReplace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagBegin(sTagName, true);
                    }
                    else if (gHTMLReader.IsMarkBegin)
                    {
                        //TagBegin(gHTMLReader.MarkName.ToLower(), false);

                        string sTagName = gHTMLReader.MarkName.ToLower();
                        sTagName = gReplace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagBegin(sTagName, false);
                    }
                    else if (gHTMLReader.IsMarkEnd)
                    {
                        //TagEnd(gHTMLReader.MarkName.ToLower());

                        string sTagName = gHTMLReader.MarkName.ToLower();
                        sTagName = gReplace.Replace(sTagName, "_");
                        if (sTagName == "") sTagName = "_";

                        TagEnd(sTagName);
                    }
                }
            }
            finally
            {
                gHTMLReader.Close();
            }
        }
        #endregion

        #region InitXml
        private void InitXml()
        {
            gHTMLReader.ReadCommentInText = gbReadCommentInText;

            //gXmlDocument = new XmlDocument();
            if (gXmlDocument != null)
            {
                if (!gbGenerateXmlNodeOnly)
                    gXmlDocument.PreserveWhitespace = true;
                else
                    gXmlDocument.PreserveWhitespace = false;
            }
            //XmlDeclaration declaration = gXml.CreateXmlDeclaration("1.0", "utf-8", null);
            //gXml.AppendChild(declaration);

            // création du tag xml
            //XmlElement element = gXmlDocument.CreateElement("xml");
            //gXmlDocument.AppendChild(element);
            //gCurrentNode = gCurrentTreeNode = (XmlNode)element;
            XXNode element = CreateElement("xml");
            AddElement(gDocumentNode, element);
            gCurrentNode = gCurrentTreeNode = element;

            if (!gbGenerateXmlNodeOnly)
                AddText(gCurrentNode, "\r\n");

            if (gbNormalizeXml)
            {
                // création du tag html
                //element = gXmlDocument.CreateElement("html");
                //gCurrentTreeNode.AppendChild(element);
                //gHtmlNode = gCurrentNode = gCurrentTreeNode = (XmlNode)element;
                element = CreateElement("html");
                AddElement(gCurrentTreeNode, element);
                gHtmlNode = gCurrentNode = gCurrentTreeNode = element;

                if (!gbGenerateXmlNodeOnly)
                    AddText(gCurrentNode, "\r\n");

                // création du tag head
                //element = gXmlDocument.CreateElement("head");
                //gCurrentTreeNode.AppendChild(element);
                //gHeadNode = gCurrentNode = (XmlNode)element;
                element = CreateElement("head");
                AddElement(gCurrentTreeNode, element);
                gHeadNode = gCurrentNode = element;

                if (!gbGenerateXmlNodeOnly)
                    AddText(gCurrentNode, "\r\n");

                // création du tag title
                //element = gXmlDocument.CreateElement("title");
                //gCurrentNode.AppendChild(element);
                //gTitleNode = gCurrentNode = (XmlNode)element;
                element = CreateElement("title");
                AddElement(gCurrentNode, element);
                gTitleNode = gCurrentNode = element;

                // création du tag body
                //element = gXmlDocument.CreateElement("body");
                //gCurrentTreeNode.AppendChild(element);
                //gBodyNode = gCurrentNode = (XmlNode)element;
                element = CreateElement("body");
                AddElement(gCurrentTreeNode, element);
                gBodyNode = gCurrentNode = element;

                if (!gbGenerateXmlNodeOnly)
                    AddText(gCurrentNode, "\r\n");

                gCurrentNode = gCurrentTreeNode = gHeadNode;
            }
        }
        #endregion

        #region TagBegin
        private void TagBegin(string sTagName, bool bTagEnd)
        {
            gbNoTag = false;

            //sTagName = sTagName.Replace('-', '_');
            //sTagName = sTagName.Replace('!', '_');
            //sTagName = sTagName.Replace('[', '_');
            //sTagName = sTagName.Replace(']', '_');
            //sTagName = gReplace.Replace(sTagName, "_");
            //if (sTagName == "") sTagName = "_";
            HtmlTagTypeEnum tagType = HtmlTag.GetHtmlTagType(sTagName);
            HtmlTag tag = HtmlTag.GetHtmlTag(tagType);
            if (gbNormalizeXml)
            {
                if (tagType == HtmlTagTypeEnum.Html || tagType == HtmlTagTypeEnum.Head)
                {
                    gbNoTag = true;
                    return;
                }
                if (tagType == HtmlTagTypeEnum.Body)
                {
                    gbNoTag = true;
                    if (!gbBody)
                    {
                        gbBody = true;
                        gCurrentNode = gCurrentTreeNode = gBodyNode;
                    }
                    return;
                }
                if (tagType == HtmlTagTypeEnum.Title)
                {
                    if (!gbTitle)
                    {
                        if (!bTagEnd)
                        {
                            gbTitle = true;
                            gCurrentNode = gTitleNode;
                        }
                    }
                    else
                        gbNoTag = true;
                    return;
                }
                if (!gbBody && tag.TagCategory != HtmlTagCategoryEnum.Head)
                {
                    gbBody = true;
                    gCurrentNode = gCurrentTreeNode = gBodyNode;
                }
            }
            //gCurrentNode = gXmlDocument.CreateElement(sTagName);
            gCurrentNode = CreateElement(sTagName);
            if (gbNormalizeXml)
            {
                if (tagType == HtmlTagTypeEnum.Table && !bTagEnd)
                {
                    if (gTable != null) gTableStack.Push(gTable);
                    gTable = new HtmlTable();
                    gTable.Table = gCurrentNode;
                    //gCurrentTreeNode.AppendChild(gCurrentNode);
                    AddElement(gCurrentTreeNode, gCurrentNode);
                    gCurrentTreeNode = gCurrentNode;
                    return;
                }
                if (TagBeginTableCategory(tag, bTagEnd)) return;
                if (tagType == HtmlTagTypeEnum.DL && !bTagEnd)
                {
                    if (gDefinitionList != null) gDefinitionListStack.Push(gDefinitionList);
                    gDefinitionList = gCurrentNode;
                    //gCurrentTreeNode.AppendChild(gCurrentNode);
                    AddElement(gCurrentTreeNode, gCurrentNode);
                    gCurrentTreeNode = gCurrentNode;
                    return;
                }
                if (TagBeginDefinitionListCategory(tag, bTagEnd)) return;

                // $$pb à revérifier
                // il faut au moins annuler gLastPNode quand un des parents de gLastPNode se ferme
                //if (tagType == HtmlTagTypeEnum.P)
                //{
                //    // pour gérer une balise <p> qui n'a pas de fin de balise </p>
                //    if (gLastPNode != null)
                //    {
                //        gCurrentTreeNode = GetParentXXNode(gLastPNode);
                //        gLastPNode = null;
                //    }
                //    if (!bTagEnd) gLastPNode = gCurrentNode;
                //}

            }
            //gCurrentTreeNode.AppendChild(gCurrentNode);
            AddElement(gCurrentTreeNode, gCurrentNode);
            if (!bTagEnd && tag.EndBoundType != HtmlBoundTypeEnum.Forbidden) gCurrentTreeNode = gCurrentNode;
        }
        #endregion

        #region TagBeginTableCategory
        private bool TagBeginTableCategory(HtmlTag tag, bool bTagEnd)
        {
            if (gTable == null || tag.TagCategory != HtmlTagCategoryEnum.Table) return false;
            switch (tag.TagType)
            {
                case HtmlTagTypeEnum.THead:
                case HtmlTagTypeEnum.TBody:
                case HtmlTagTypeEnum.TFoot:
                    //gTable.Table.AppendChild(gCurrentNode);
                    AddElement(gTable.Table, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Body = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Body = null;
                    return true;
                case HtmlTagTypeEnum.ColGroup:
                    //gTable.Table.AppendChild(gCurrentNode);
                    AddElement(gTable.Table, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.ColGroup = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.ColGroup = null;
                    return true;
                case HtmlTagTypeEnum.Col:
                    gCurrentTreeNode = gTable.Table;
                    if (gTable.ColGroup != null) gCurrentTreeNode = gTable.ColGroup;
                    //gCurrentTreeNode.AppendChild(gCurrentNode);
                    AddElement(gCurrentTreeNode, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Col = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Col = null;
                    return true;
                case HtmlTagTypeEnum.TR:
                    if (gTable.Body == null)
                    {
                        //gTable.Body = gXmlDocument.CreateElement("tbody");
                        //gTable.Table.AppendChild(gTable.Body);
                        gTable.Body = CreateElement("tbody");
                        AddElement(gTable.Table, gTable.Body);
                    }
                    //gTable.Body.AppendChild(gCurrentNode);
                    AddElement(gTable.Body, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Row = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Row = null;
                    return true;
                case HtmlTagTypeEnum.TH:
                case HtmlTagTypeEnum.TD:
                    if (gTable.Row == null)
                    {
                        if (gTable.Body == null)
                        {
                            //gtable.body = gxmldocument.createelement("tbody");
                            //gtable.table.appendchild(gtable.body);
                            gTable.Body = CreateElement("tbody");
                            AddElement(gTable.Table, gTable.Body);
                        }
                        //gTable.Row = gXmlDocument.CreateElement("tr");
                        //gTable.Body.AppendChild(gTable.Row);
                        gTable.Row = CreateElement("tr");
                        AddElement(gTable.Body, gTable.Row);
                    }
                    //gTable.Row.AppendChild(gCurrentNode);
                    AddElement(gTable.Row, gCurrentNode);
                    if (!bTagEnd)
                    {
                        gTable.Data = gCurrentNode;
                        gCurrentTreeNode = gCurrentNode;
                    }
                    else
                        gTable.Data = null;
                    return true;
            }
            return false;
        }
        #endregion

        #region TagBeginDefinitionListCategory
        private bool TagBeginDefinitionListCategory(HtmlTag tag, bool bTagEnd)
        {
            if (gDefinitionList == null || tag.TagCategory != HtmlTagCategoryEnum.DefinitionList) return false;
            switch (tag.TagType)
            {
                case HtmlTagTypeEnum.DT:
                case HtmlTagTypeEnum.DD:
                    //gDefinitionList.AppendChild(gCurrentNode);
                    AddElement(gDefinitionList, gCurrentNode);
                    if (!bTagEnd) gCurrentTreeNode = gCurrentNode;
                    return true;
            }
            return false;
        }
        #endregion

        #region TagEnd
        private void TagEnd(string sTagName)
        {
            if (gbNormalizeXml)
            {
                HtmlTagTypeEnum tagType = HtmlTag.GetHtmlTagType(sTagName);
                switch (tagType)
                {
                    case HtmlTagTypeEnum.Html:
                    case HtmlTagTypeEnum.Head:
                    case HtmlTagTypeEnum.Body:
                        return;
                    case HtmlTagTypeEnum.Title:
                        gCurrentNode = gCurrentTreeNode;
                        return;
                    case HtmlTagTypeEnum.Table:
                        if (gTable == null) return;
                        //gCurrentNode = gCurrentTreeNode = gTable.Table.ParentNode;
                        gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Table);
                        gTable = null;
                        if (gTableStack.Count != 0) gTable = gTableStack.Pop();
                        return;
                    case HtmlTagTypeEnum.DL:
                        if (gDefinitionList == null) return;
                        //gCurrentNode = gCurrentTreeNode = gDefinitionList.ParentNode;
                        gCurrentNode = gCurrentTreeNode = GetParentXXNode(gDefinitionList);
                        gDefinitionList = null;
                        if (gDefinitionListStack.Count != 0) gDefinitionList = gDefinitionListStack.Pop();
                        return;
                }
                if (gTable != null)
                {
                    switch (tagType)
                    {
                        case HtmlTagTypeEnum.THead:
                        case HtmlTagTypeEnum.TBody:
                        case HtmlTagTypeEnum.TFoot:
                            gCurrentNode = gCurrentTreeNode = gTable.Table;
                            gTable.Body = null;
                            return;
                        case HtmlTagTypeEnum.ColGroup:
                            gCurrentNode = gCurrentTreeNode = gTable.Table;
                            gTable.ColGroup = null;
                            return;
                        case HtmlTagTypeEnum.Col:
                            if (gTable.Col != null)
                            {
                                //gCurrentNode = gCurrentTreeNode = gTable.Col.ParentNode;
                                gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Col);
                                gTable.Col = null;
                            }
                            return;
                        case HtmlTagTypeEnum.TR:
                            if (gTable.Row != null)
                            {
                                //gCurrentNode = gCurrentTreeNode = gTable.Row.ParentNode;
                                gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Row);
                                gTable.Row = null;
                            }
                            return;
                        case HtmlTagTypeEnum.TH:
                        case HtmlTagTypeEnum.TD:
                            if (gTable.Data != null)
                            {
                                //gCurrentNode = gCurrentTreeNode = gTable.Data.ParentNode;
                                gCurrentNode = gCurrentTreeNode = GetParentXXNode(gTable.Data);
                                gTable.Data = null;
                            }
                            return;
                    }
                }

            }
            //XmlNode node = gCurrentTreeNode;
            //while (node != null)
            //{
            //    if (node.Name == sTagName)
            //    {
            //        gCurrentTreeNode = node.ParentNode;
            //        break;
            //    }
            //    node = node.ParentNode;
            //}
            XXNode node = GetParentXXNodeByName(gCurrentTreeNode, sTagName);
            if (node != null)
                gCurrentTreeNode = GetParentXXNode(node);
            gCurrentNode = gCurrentTreeNode;
        }
        #endregion

        #region static method
        #region Normalize
        public static void Normalize(XmlDocument xml)
        {
            // ajout du tag tbody dans les tag table qui n'en ont pas
            XmlNodeList tables = xml.SelectNodes("//table");
            foreach (XmlNode table in tables)
            {
                NormalizeTable(table);
            }
        }
        #endregion

        #region NormalizeTable
        private static void NormalizeTable(XmlNode table)
        {
            if (table.SelectSingleNode("./tbody") != null) return;
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node in table.ChildNodes)
            {
                if (node.Name == "caption") continue;
                if (node.Name == "thead") continue;
                if (node.Name == "tfoot") continue;
                if (node.Name == "col") continue;
                nodes.Add(node);
            }
            XmlElement tbody = table.OwnerDocument.CreateElement("tbody");
            table.AppendChild(tbody);
            foreach (XmlNode node in nodes)
            {
                table.RemoveChild(node);
                tbody.AppendChild(node);
            }
        }
        #endregion

        #region LoadHtml
        public static XmlDocument LoadHtml(string sUrl)
        {
            HtmlXml hx = new HtmlXml(sUrl);
            return hx.GenerateXmlDocument();
        }
        #endregion

        #region GetTables(XmlDocument xml)
        public static HtmlXmlTables GetTables(XmlDocument xml)
        {
            return new HtmlXmlTables(xml);
        }
        #endregion

        #region GetTables(string[] tablesPath)
        public static HtmlXmlTables GetTables(string[] tablesPath)
        {
            return new HtmlXmlTables(tablesPath);
        }
        #endregion
        #endregion
    }

    public class Http : IDisposable
    {
        #region doc
        // - le fait de modifier un paramètre ferme la session en cours (appel de Close())
        // - les paramètres suivants sont remis à zéro à chaque fin de session (Close() appel ResetParameters())
        //   gMethod, gsReferer, gHeaders, gsRequestContentType, gsContent, gsExportPath
        #endregion

        #region variable
        // paramètre :
        private string gsUrl = null;
        //private string gsMethod = null;
        private HttpRequestMethod gMethod;
        private string gsAccept = null;
        private string gsReferer = null;
        private DecompressionMethods? gAutomaticDecompression = null;
        private NameValueCollection gHeaders = new NameValueCollection();
        private string gsRequestContentType = null;
        private string gsContent = null;
        //private Encoding gDefaultEncoding = Encoding.Default;
        private Encoding gEncoding = null;
        private bool gbUseWebClient = false;
        private int giLoadXmlRetryTimeout = 0; // timeout in seconds, 0 = no timeout, -1 = endless timeout
        private string gsTraceDirectory = null;
        private string gsTextExportPath = null;
        private string gsXmlExportPath = null;
        private bool gbReadCommentInText = false;

        // résultat de la requete :
        private string gsTextResult = null;
        private string gsContentType = null;
        private string gsCharset = null;
        private long gContentLength = -1;

        private WebClient gWebClient = null;

        private WebRequest gWebRequest = null;
        private WebResponse gWebResponse = null;
        private CookieContainer gCookies = null;

        private Progress gProgress = null;
        private StreamTransfer gStreamTransfer = null;
        private bool gAbortTransfer = false;
        private Stream gStream = null;
        private StreamReader gWebStream = null;
        private Encoding gWebStreamEncoding = null;
        private bool gbOpened = false;
        private bool gbResult = false;
        public delegate bool fnHttpRetry(Exception ex);
        /// <summary>
        /// valeur retournée : true pour recommencer, false pour arrêter
        /// </summary>
        public fnHttpRetry HttpRetry = null;

        private static Regex grxTranslate1 = new Regex(@"&([a-zA-Z]+)\w*;?", RegexOptions.Compiled);
        private static Regex grxTranslate2 = new Regex(@"&#([0-9]+);", RegexOptions.Compiled);
        private static Regex grxTranslate3 = new Regex(@"&#x([a-fA-F0-9]+);", RegexOptions.Compiled);
        #endregion

        #region constructor
        #region Http()
        public Http()
        {
            Init();
        }
        #endregion

        #region Http(string sUrl)
        public Http(string sUrl)
        {
            gsUrl = sUrl;
            Init();
        }
        #endregion

        #region Http(string sUrl, CookieContainer cookies)
        public Http(string sUrl, CookieContainer cookies)
        {
            gsUrl = sUrl;
            gCookies = cookies;
            Init();
        }
        #endregion
        #endregion

        #region Dispose
        public void Dispose()
        {
            Close();
        }
        #endregion

        #region property ...
        #region Url
        public string Url
        {
            get { return gsUrl; }
            set
            {
                Reset();
                gsUrl = GetUrl(gsUrl, value);
            }
        }
        #endregion

        #region Method
        public HttpRequestMethod Method
        {
            get { return gMethod; }
            set
            {
                Reset();
                gMethod = value;
            }
        }
        #endregion

        #region Accept
        public string Accept
        {
            get { return gsAccept; }
            set
            {
                Reset();
                gsAccept = value;
            }
        }
        #endregion

        #region Referer
        public string Referer
        {
            get { return gsReferer; }
            set
            {
                Reset();
                gsReferer = value;
            }
        }
        #endregion

        #region AutomaticDecompression
        public DecompressionMethods? AutomaticDecompression
        {
            get { return gAutomaticDecompression; }
            set
            {
                Reset();
                gAutomaticDecompression = value;
            }
        }
        #endregion

        #region Headers
        public NameValueCollection Headers
        {
            get { return gHeaders; }
            set
            {
                Reset();
                gHeaders = value;
            }
        }
        #endregion

        #region RequestContentType
        public string RequestContentType
        {
            get { return gsRequestContentType; }
            set
            {
                Reset();
                gsRequestContentType = value;
            }
        }
        #endregion

        #region Content
        public string Content
        {
            get { return gsContent; }
            set
            {
                Reset();
                gsContent = value;
            }
        }
        #endregion

        #region Encoding
        public Encoding Encoding
        {
            get { return gEncoding; }
            set
            {
                Reset();
                gEncoding = value;
            }
        }
        #endregion

        #region UseWebClient
        public bool UseWebClient
        {
            get { return gbUseWebClient; }
            set
            {
                Reset();
                gbUseWebClient = value;
            }
        }
        #endregion

        #region WebClient
        public WebClient WebClient
        {
            get
            {
                Open();
                return gWebClient;
            }
        }
        #endregion

        #region Request
        public WebRequest Request
        {
            get
            {
                Open();
                return gWebRequest;
            }
        }
        #endregion

        #region Response
        public WebResponse Response
        {
            get
            {
                Open();
                return gWebResponse;
            }
        }
        #endregion

        #region Cookies
        public CookieContainer Cookies
        {
            get
            {
                Open();
                return gCookies;
            }
            set
            {
                Reset();
                gCookies = value;
            }
        }
        #endregion

        #region WebStream
        public StreamReader WebStream
        {
            get
            {
                Open();
                CreateStreamReader();
                return gWebStream;
            }
        }
        #endregion

        #region WebStreamEncoding
        public Encoding WebStreamEncoding
        {
            get
            {
                //Open();
                return gWebStreamEncoding;
            }
        }
        #endregion

        #region TextResult
        public string TextResult
        {
            get { return gsTextResult; }
        }
        #endregion

        #region ContentType
        public string ContentType
        {
            get
            {
                Open();
                return gsContentType;
            }
        }
        #endregion

        #region Charset
        public string Charset
        {
            get
            {
                Open();
                return gsCharset;
            }
        }
        #endregion

        #region ContentLength
        public long ContentLength
        {
            get
            {
                Open();
                return gContentLength;
            }
        }
        #endregion

        #region LoadXmlRetryTimeout
        /// <summary>
        /// timeout in seconds, 0 = no timeout, -1 = endless timeout
        /// </summary>
        public int LoadXmlRetryTimeout
        {
            get { return giLoadXmlRetryTimeout; }
            set
            {
                Reset();
                giLoadXmlRetryTimeout = value;
            }
        }
        #endregion

        #region TraceDirectory
        public string TraceDirectory
        {
            get { return gsTraceDirectory; }
            set
            {
                Reset();
                gsTraceDirectory = value;
            }
        }
        #endregion

        #region TextExportPath
        public string TextExportPath
        {
            get { return gsTextExportPath; }
            set
            {
                Reset();
                gsTextExportPath = value;
            }
        }
        #endregion

        #region XmlExportPath
        public string XmlExportPath
        {
            get { return gsXmlExportPath; }
            set
            {
                Reset();
                gsXmlExportPath = value;
            }
        }
        #endregion

        #region ReadCommentInText
        public bool ReadCommentInText
        {
            get { return gbReadCommentInText; }
            set
            {
                Reset();
                gbReadCommentInText = value;
            }
        }
        #endregion

        #region Progress
        public Progress Progress
        {
            get { return gProgress; }
        }
        #endregion
        #endregion

        #region Init
        private void Init()
        {
            gProgress = new Progress();
            //gProgress.ProgressControlChanged += new Progress.ProgressControlChangedEventHandler(ProgressControlChanged);
        }
        #endregion

        #region //ProgressControlChanged
        //private void ProgressControlChanged(IProgressControl progressControl)
        //{
        //}
        #endregion

        #region AbortTransfer
        public void AbortTransfer()
        {
            gAbortTransfer = true;
            if (gStreamTransfer != null) gStreamTransfer.AbortTransfer();
        }
        #endregion

        #region CancelAbortTransfer
        public void CancelAbortTransfer()
        {
            gAbortTransfer = false;
            if (gStreamTransfer != null) gStreamTransfer.CancelAbortTransfer();
        }
        #endregion

        #region Load ...
        #region Load(string url)
        public void Load(string url)
        {
            //Load(url, HttpRequestMethod.Get, null, null);
            Reset();
            gsUrl = GetUrl(gsUrl, url);
            Load();
        }
        #endregion

        #region Load(string url, string method, string content)
        public void Load(string url, HttpRequestMethod method, string content)
        {
            //Load(url, method, content, null);
            Reset();
            gsUrl = GetUrl(gsUrl, url);
            gMethod = method;
            gsContent = content;
            Load();
        }
        #endregion

        #region Load(string url, string method, string content, string referer)
        public void Load(string url, HttpRequestMethod method, string content, string referer)
        {
            Reset();
            gsUrl = GetUrl(gsUrl, url);
            gMethod = method;
            gsContent = content;
            gsReferer = referer;
            Load();
        }
        #endregion

        #region Load()
        public void Load()
        {
            try
            {
                Open();
                if (gsContentType.StartsWith("text"))
                {
                    _LoadText();
                    if (gsTraceDirectory != null)
                        gsTextExportPath = GetNewHttpFileName(gsTraceDirectory, GetContentFileExtension(gsContentType));
                    else if (gsTextExportPath != null)
                    {
                        if (cu.PathGetExt(gsTextExportPath) == "")
                            gsTextExportPath = cu.PathSetExt(gsTextExportPath, GetContentFileExtension(gsContentType));
                    }
                    if (gsTextExportPath != null)
                        cu.WriteFile(gsTextExportPath, gsTextResult);
                }
            }
            finally
            {
                Close();
            }
        }
        #endregion
        #endregion

        #region LoadToFile ...
        #region LoadToFile(string path, string url)
        public bool LoadToFile(string path, string url)
        {
            return LoadToFile(path, url, HttpRequestMethod.Get, null, null);
        }
        #endregion

        #region LoadToFile(string path, string url, string method, string content)
        public bool LoadToFile(string path, string url, HttpRequestMethod method, string content)
        {
            return LoadToFile(path, url, method, content, null);
        }
        #endregion

        #region LoadToFile(string path, string url, string method, string content, string referer)
        public bool LoadToFile(string path, string url, HttpRequestMethod method, string content, string referer)
        {
            Reset();
            gsUrl = GetUrl(gsUrl, url);
            gMethod = method;
            gsContent = content;
            gsReferer = referer;
            return LoadToFile(path);
        }
        #endregion

        #region LoadToFile(string path)
        public bool LoadToFile(string path)
        {
            bool ret = false;
            FileStream fs = null;
            try
            {
                Open();
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);

                DateTime dtFirstCatch = new DateTime(0);
                while (true)
                {
                    try
                    {
                        //cu.StreamWrite(gStream, fs);
                        if (gAbortTransfer)
                        {
                            ret = false;
                            break;
                        }
                        //gStreamTransfer = new StreamTransfer(gProgress.ProgressControl);
                        gStreamTransfer = new StreamTransfer();
                        gStreamTransfer.SourceLength = gContentLength;
                        gStreamTransfer.Progress.ProgressChanged += new Progress.ProgressChangedEventHandler(StreamTransferProgressChange);
                        ret = gStreamTransfer.Transfer(gStream, fs);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ex is IOException)
                            throw;
                        if (ex is ThreadAbortException)
                            throw;
                        if (giLoadXmlRetryTimeout == 0)
                            throw;

                        if (dtFirstCatch.Ticks == 0)
                        {
                            dtFirstCatch = DateTime.Now;
                        }
                        else if (giLoadXmlRetryTimeout != -1)
                        {
                            dtFirstCatch = DateTime.Now;
                            TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                            if (ts.Seconds > giLoadXmlRetryTimeout) throw;
                        }
                        if (HttpRetry != null && !HttpRetry(ex)) throw;

                        Close();
                        Open();
                        FileStream fs2 = fs;
                        fs = null;
                        fs2.Close();
                        fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read);
                    }
                }
            }
            finally
            {
                gAbortTransfer = false;
                if (fs != null) fs.Close();
                Close();
            }
            return ret;
        }
        #endregion
        #endregion

        #region LoadToFileProgressEvent
        private void StreamTransferProgressChange(long current, long total)
        {
            gProgress.SetProgress(current, total);
        }
        #endregion

        #region _LoadText
        private void _LoadText()
        {
            DateTime dtFirstCatch = new DateTime(0);
            while (true)
            {
                try
                {
                    CreateStreamReader();
                    StreamReader sr = gWebStream;
                    gsTextResult = sr.ReadToEnd();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is IOException)
                        throw;
                    if (ex is ThreadAbortException)
                        throw;
                    if (giLoadXmlRetryTimeout == 0)
                        throw;

                    if (dtFirstCatch.Ticks == 0)
                    {
                        dtFirstCatch = DateTime.Now;
                    }
                    else if (giLoadXmlRetryTimeout != -1)
                    {
                        dtFirstCatch = DateTime.Now;
                        TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                        if (ts.Seconds > giLoadXmlRetryTimeout) throw;
                    }
                    if (HttpRetry != null && !HttpRetry(ex)) throw;

                    Close();
                    Open();
                }
            }
        }
        #endregion

        #region //GetTextResult
        //public string GetTextResult()
        //{
        //    if (gsTextResult == null)
        //        throw new PBException("Http error, there is no text result");
        //    return gsTextResult;
        //}
        #endregion

        #region GetXmlDocumentResult0
        public XmlDocument GetXmlDocumentResult0()
        {
            if (gsContentType == "text/html")
            {
                HtmlXml0 hx = new HtmlXml0(new StringReader(gsTextResult));
                hx.ReadCommentInText = gbReadCommentInText;
                return hx.GenerateXml();
            }
            else if (gsContentType == "text/xml")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(gsTextResult);
                return xml;
            }
            else
                throw new PBException("Error can't transform \"{0}\" content to xml", gsContentType);
        }
        #endregion

        #region GetXmlDocumentResult
        public XmlDocument GetXmlDocumentResult()
        {
            XmlDocument xml = null;
            if (gsContentType == "text/html")
            {
                HtmlXml hx = new HtmlXml(new StringReader(gsTextResult));
                hx.ReadCommentInText = gbReadCommentInText;
                xml = hx.GenerateXmlDocument();
            }
            else if (gsContentType == "text/xml")
            {
                xml = new XmlDocument();
                xml.LoadXml(gsTextResult);
                //return xml;
            }
            else
                throw new PBException("Error can't transform \"{0}\" content to xml", gsContentType);


            if (gsTraceDirectory != null)
                gsXmlExportPath = GetNewHttpFileName(gsTraceDirectory, ".xml");
            else if (gsXmlExportPath != null)
            {
                if (cu.PathGetExt(gsTextExportPath) == "")
                    gsXmlExportPath = cu.PathSetExt(gsTextExportPath, ".xml");
            }
            if (gsXmlExportPath != null)
                xml.Save(gsXmlExportPath);
            return xml;
        }
        #endregion

        #region GetXDocumentResult
        public XDocument GetXDocumentResult()
        {
            XDocument xml = null;
            if (gsContentType == "text/html")
            {
                HtmlXml hx = new HtmlXml(new StringReader(gsTextResult));
                hx.ReadCommentInText = gbReadCommentInText;
                xml = hx.GenerateXDocument();
            }
            else if (gsContentType == "text/xml")
            {
                xml = XDocument.Parse(gsTextResult, LoadOptions.PreserveWhitespace);
            }
            else
                throw new PBException("Error can't transform \"{0}\" content to xml", gsContentType);

            if (gsTraceDirectory != null)
                gsXmlExportPath = GetNewHttpFileName(gsTraceDirectory, ".xml");
            else if (gsXmlExportPath != null)
            {
                if (cu.PathGetExt(gsTextExportPath) == "")
                    gsXmlExportPath = cu.PathSetExt(gsTextExportPath, ".xml");
            }
            if (gsXmlExportPath != null)
                xml.Save(gsXmlExportPath);
            return xml;
        }
        #endregion

        #region SaveResult
        public void SaveResult(string path)
        {
            if (gsTextResult == null)
                throw new PBException("Http error, there is no result to save (file \"{0}\")", path);
            cu.WriteFile(path, gsTextResult);
        }
        #endregion

        #region Open
        private void Open()
        {
            if (gbOpened) return;
            if (gbUseWebClient)
                OpenWebClient();
            else
                OpenWebRequest();
        }
        #endregion

        #region OpenWebClient
        private void OpenWebClient()
        {
            if (gWebClient != null) return;
            if (gsUrl == null) return;
            gWebClient = new WebClient();
            gWebClient.Headers.Add(HttpRequestHeader.UserAgent, "pb");

            DateTime dtFirstCatch = new DateTime(0);
            while (true)
            {
                try
                {
                    gStream = gWebClient.OpenRead(gsUrl);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is IOException)
                        throw;
                    if (ex is ThreadAbortException)
                        throw;
                    if (giLoadXmlRetryTimeout == 0)
                        throw;

                    if (dtFirstCatch.Ticks == 0)
                    {
                        dtFirstCatch = DateTime.Now;
                    }
                    else if (giLoadXmlRetryTimeout != -1)
                    {
                        dtFirstCatch = DateTime.Now;
                        TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                        if (ts.Seconds > giLoadXmlRetryTimeout) throw;
                    }
                    if (HttpRetry != null && !HttpRetry(ex)) throw;
                }
            }

            GetWebClientHeaderValues();
            //CreateStreamReader();
            gbOpened = true;
        }
        #endregion

        #region OpenWebRequest
        //private int giOpenWebRequest = 1;
        private void OpenWebRequest()
        {
            if (gWebRequest != null) return;
            if (gsUrl == null) return;

            //cTrace.Trace("{0} Http.OpenWebRequest() : gWebRequest = WebRequest.Create()", giOpenWebRequest++);
            gWebRequest = WebRequest.Create(gsUrl);
            if (gWebRequest is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)gWebRequest;
                httpRequest.UserAgent = "pb";
                //if (gsMethod != null) httpRequest.Method = gsMethod;
                if (gMethod == HttpRequestMethod.Get)
                    httpRequest.Method = "GET";
                else
                    httpRequest.Method = "POST";
                if (gsAccept != null) httpRequest.Accept = gsAccept;
                if (gsReferer != null) httpRequest.Referer = gsReferer;
                httpRequest.Headers.Add(gHeaders);
                if (gAutomaticDecompression != null) httpRequest.AutomaticDecompression = (DecompressionMethods)gAutomaticDecompression;
                if (gCookies == null) gCookies = new CookieContainer();
                httpRequest.CookieContainer = gCookies;
                if (gsContent != null)
                {
                    //httpRequest.ContentType = gsRequestContentType;

                    //httpRequest.ContentLength = gsContent.Length;
                    ////Encoding encoding = Encoding.Default;
                    ////if (gEncoding != null) encoding = gEncoding;
                    ////httpRequest.ContentLength = encoding.GetByteCount(gsContent);
                    //Stream stream = httpRequest.GetRequestStream();
                    //StreamWriter sw = new StreamWriter(stream);
                    ////StreamWriter sw = new StreamWriter(stream, encoding);
                    //sw.Write(gsContent);
                    //sw.Flush();
                    //sw.Close();

                    httpRequest.ContentType = gsRequestContentType;
                    Encoding encoding;
                    if (gEncoding != null) encoding = gEncoding; else encoding = Encoding.Default;
                    byte[] bytes = encoding.GetBytes(gsContent);
                    httpRequest.ContentLength = bytes.LongLength;
                    Stream stream = httpRequest.GetRequestStream();
                    using (BinaryWriter w = new BinaryWriter(stream))
                    {
                        w.Write(bytes);
                    }
                }
            }

            DateTime dtFirstCatch = new DateTime(0);
            while (true)
            {
                try
                {
                    gWebResponse = gWebRequest.GetResponse();
                    gStream = gWebResponse.GetResponseStream();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex is IOException)
                        throw;
                    if (ex is ThreadAbortException)
                        throw;
                    if (ex is WebException)
                    {
                        //WebException wex = (WebException)ex;
                        //if (   wex.Status != WebExceptionStatus.ConnectFailure
                        //    && wex.Status != WebExceptionStatus.PipelineFailure
                        //    && wex.Status != WebExceptionStatus.ProtocolError
                        //    && wex.Status != WebExceptionStatus.ReceiveFailure
                        //    && wex.Status != WebExceptionStatus.SendFailure
                        //    && wex.Status != WebExceptionStatus.ServerProtocolViolation
                        //    && wex.Status != WebExceptionStatus.Timeout
                        //    && wex.Status != WebExceptionStatus.UnknownError
                        //    )
                        throw;
                    }

                    if (giLoadXmlRetryTimeout == 0)
                        throw;

                    if (dtFirstCatch.Ticks == 0)
                    {
                        dtFirstCatch = DateTime.Now;
                    }
                    else if (giLoadXmlRetryTimeout != -1)
                    {
                        dtFirstCatch = DateTime.Now;
                        TimeSpan ts = DateTime.Now.Subtract(dtFirstCatch);
                        if (ts.Seconds > giLoadXmlRetryTimeout) throw;
                    }
                    if (HttpRetry != null && !HttpRetry(ex)) throw;
                }
            }

            GetWebRequestHeaderValues();
            //CreateStreamReader();
            gbOpened = true;
            gbResult = true;
        }
        #endregion

        #region CreateStreamReader
        private void CreateStreamReader()
        {
            Encoding encoding = cu.GetEncoding(gsCharset);
            if (encoding == null)
            {
                if (gEncoding != null)
                    encoding = gEncoding;
                else
                    encoding = Encoding.Default;
            }
            gWebStreamEncoding = encoding;
            gWebStream = new StreamReader(gStream, encoding);
        }
        #endregion

        #region Reset()
        private void Reset()
        {
            Close();
            ResetParameters();
        }
        #endregion

        #region Close
        private void Close()
        {
            if (!gbOpened) return;
            if (gWebStream != null)
            {
                gWebStream.Close();
                gWebStream = null;
            }
            gWebStreamEncoding = null;
            if (gStream != null)
            {
                gStream.Close();
                gStream = null;
            }
            if (gWebClient != null)
            {
                gWebClient.Dispose();
                gWebClient = null;
            }
            if (gWebResponse != null)
            {
                gWebResponse.Close();
                gWebResponse = null;
            }
            gWebRequest = null;
            gbOpened = false;
        }
        #endregion

        #region ResetParameters
        public void ResetParameters()
        {
            if (gbResult)
            {
                gMethod = HttpRequestMethod.Get;
                //gsAccept = null;
                gsReferer = null;
                //gAutomaticDecompression = null;
                gHeaders = new NameValueCollection();
                gsRequestContentType = null;
                gsContent = null;
                //gEncoding = null;
                //gbUseWebClient = false;
                //giLoadXmlRetryTimeout = 0;
                //gsTraceDirectory = null;
                gsTextExportPath = null;
                //gbReadCommentInText = false;

                gsTextResult = null;
                gsContentType = null;
                gsCharset = null;

                gbResult = false;
            }
        }
        #endregion

        #region GetWebClientHeaderValues
        private void GetWebClientHeaderValues()
        {
            if (gsUrl.StartsWith("http:", StringComparison.InvariantCultureIgnoreCase))
            {
                string s = gWebClient.ResponseHeaders[HttpResponseHeader.ContentType].ToLower();
                string[] s2 = cu.Split(s, ';', true);
                if (s2.Length > 0)
                {
                    gsContentType = s2[0];
                }
                for (int i = 1; i < s2.Length; i++)
                {
                    string[] s3 = cu.Split(s2[i], '=', true);
                    if (s3.Length > 1)
                    {
                        if (s3[0] == "charset") gsCharset = s3[1];
                    }

                }
            }
            else if (gsUrl.StartsWith("file:", StringComparison.InvariantCultureIgnoreCase))
            {
                Uri uri = new Uri(gsUrl);
                string sExt = Path.GetExtension(uri.LocalPath).ToLower();
                switch (sExt)
                {
                    case ".xml":
                        gsContentType = "text/xml";
                        break;
                    case ".htm":
                    case ".html":
                        gsContentType = "text/html";
                        break;
                    case ".txt":
                        gsContentType = "text/txt";
                        break;
                    default:
                        if (sExt.Length > 1)
                            gsContentType = "/" + sExt.Substring(1);
                        break;
                }
            }
        }
        #endregion

        #region GetWebRequestHeaderValues
        private void GetWebRequestHeaderValues()
        {


            if (gWebResponse is HttpWebResponse)
            {

                HttpWebResponse httpResponse = (HttpWebResponse)gWebResponse;
                gsCharset = httpResponse.CharacterSet;
                if (gsCharset != null) gsCharset = gsCharset.ToLower();

                string s = httpResponse.ContentType.ToLower();
                string[] s2 = cu.Split(s, ';', true);
                if (s2.Length > 0) gsContentType = s2[0];

                //string contentLength = httpResponse.Headers[HttpResponseHeader.ContentLength];
                //int l = 0;
                //if (contentLength != null) l = int.Parse(contentLength);
                gContentLength = httpResponse.ContentLength;
            }
            else if (gWebResponse is FileWebResponse)
            {
                Uri uri = new Uri(gsUrl);
                string sExt = Path.GetExtension(uri.LocalPath).ToLower();
                switch (sExt)
                {
                    case ".xml":
                        gsContentType = "text/xml";
                        break;
                    case ".htm":
                    case ".html":
                        gsContentType = "text/html";
                        break;
                    case ".txt":
                        gsContentType = "text/txt";
                        break;
                    default:
                        if (sExt.Length > 1)
                            gsContentType = "/" + sExt.Substring(1);
                        break;
                }
            }
        }
        #endregion

        #region GetNewHttpFileName(string dir)
        public string GetNewHttpFileName(string dir)
        {
            return GetNewUrlFileName(dir, gsUrl);
        }
        #endregion

        #region GetNewHttpFileName(string dir, string ext)
        public string GetNewHttpFileName(string dir, string ext)
        {
            return GetNewUrlFileName(dir, gsUrl, ext);
        }
        #endregion

        #region GetInfo ...
        #region GetInfo
        public DataTable GetInfo()
        {
            DataTable dt = zdt.Create("From, Name, Value1, Value2");
            //if (wr.http == null)
            //{
            //    dt.Rows.Add("", "", "pas de page chargée");
            //    return;
            //}
            GetHttpInfo(dt);
            GetWebClientInfo(dt);
            GetWebRequestInfo(dt);
            GetWebResponseInfo(dt);
            return dt;
        }
        #endregion

        #region GetHttpInfo
        public void GetHttpInfo(DataTable dt)
        {
            dt.Rows.Add("Http input", "Accept", this.Accept);
            dt.Rows.Add("Http input", "AutomaticDecompression", this.AutomaticDecompression.ToString());
            dt.Rows.Add("Http input", "Content", this.Content);
            string s = ""; if (this.Encoding != null) s = this.Encoding.EncodingName;
            dt.Rows.Add("Http input", "Encoding", s);
            dt.Rows.Add("Http input", "LoadXmlRetryTimeout", this.LoadXmlRetryTimeout.ToString());
            dt.Rows.Add("Http input", "Method", this.Method);
            dt.Rows.Add("Http input", "Referer", this.Referer);
            dt.Rows.Add("Http input", "RequestContentType", this.RequestContentType);
            dt.Rows.Add("Http input", "Url", this.Url);
            dt.Rows.Add("Http input", "UseWebClient", this.UseWebClient.ToString());
            dt.Rows.Add("Http input", "Headers", "Count", this.Headers.Count);
            for (int i = 0; i < this.Headers.Count; i++)
                dt.Rows.Add("Http input", "Headers", this.Headers.Keys[i], this.Headers[i]);

            dt.Rows.Add("Http output", "Charset", this.Charset);
            dt.Rows.Add("Http output", "ContentType", this.ContentType);

            //dt.Rows.Add("Http", "", this.);
        }
        #endregion

        #region GetWebClientInfo()
        public DataTable GetWebClientInfo()
        {
            DataTable dt = zdt.Create("From, Name, Value1, Value2");
            GetWebClientInfo(dt);
            return dt;
        }
        #endregion

        #region GetWebClientInfo
        public void GetWebClientInfo(DataTable dt)
        {
            WebClient client = WebClient;
            if (client == null)
            {
                dt.Rows.Add("WebClient", "", "pas de WebClient");
                return;
            }
            dt.Rows.Add("WebClient", "BaseAddress", client.BaseAddress);
            dt.Rows.Add("WebClient", "IsBusy", client.IsBusy.ToString());
            //dt.Rows.Add("WebClient", "", client.);
            dt.Rows.Add("WebClient", "Headers", "Count", client.Headers.Count);
            for (int i = 0; i < client.Headers.Count; i++)
                dt.Rows.Add("WebClient", "Headers", client.Headers.Keys[i], client.Headers[i]);
            dt.Rows.Add("WebClient", "ResponseHeaders", "Count", client.ResponseHeaders.Count);
            for (int i = 0; i < client.ResponseHeaders.Count; i++)
                dt.Rows.Add("WebClient", "ResponseHeaders", client.ResponseHeaders.Keys[i], client.ResponseHeaders[i]);
        }
        #endregion

        #region GetWebRequestInfo()
        public DataTable GetWebRequestInfo()
        {
            DataTable dt = zdt.Create("From, Name, Value1, Value2");
            GetWebRequestInfo(dt);
            return dt;
        }
        #endregion

        #region GetWebRequestInfo(DataTable dt)
        public void GetWebRequestInfo(DataTable dt)
        {
            WebRequest request = Request;
            if (request == null)
            {
                dt.Rows.Add("WebRequest", "", "pas de WebRequest");
                return;
            }
            dt.Rows.Add("WebRequest", "ContentType", request.ContentType);
            dt.Rows.Add("WebRequest", "ContentLength", request.ContentLength.ToString());
            dt.Rows.Add("WebRequest", "ConnectionGroupName", request.ConnectionGroupName);
            dt.Rows.Add("WebRequest", "Method", request.Method);
            dt.Rows.Add("WebRequest", "RequestUri", request.RequestUri.AbsoluteUri);
            dt.Rows.Add("WebRequest", "Timeout", request.Timeout.ToString());
            //dt.Rows.Add("WebRequest", "", request.);
            //request.Headers;
            if (request is HttpWebRequest)
            {
                HttpWebRequest httpRequest = (HttpWebRequest)request;
                dt.Rows.Add("HttpWebRequest", "Accept", httpRequest.Accept);
                dt.Rows.Add("HttpWebRequest", "Address", httpRequest.Address.AbsoluteUri);
                dt.Rows.Add("HttpWebRequest", "AllowAutoRedirect", httpRequest.AllowAutoRedirect.ToString());
                dt.Rows.Add("HttpWebRequest", "AllowWriteStreamBuffering", httpRequest.AllowWriteStreamBuffering.ToString());
                dt.Rows.Add("HttpWebRequest", "AutomaticDecompression", httpRequest.AutomaticDecompression.ToString());
                dt.Rows.Add("HttpWebRequest", "Connection", httpRequest.Connection);
                dt.Rows.Add("HttpWebRequest", "ConnectionGroupName", httpRequest.ConnectionGroupName);
                dt.Rows.Add("HttpWebRequest", "ContentLength", httpRequest.ContentLength.ToString());
                dt.Rows.Add("HttpWebRequest", "ContentType", httpRequest.ContentType);
                dt.Rows.Add("HttpWebRequest", "Expect", httpRequest.Expect);
                dt.Rows.Add("HttpWebRequest", "HaveResponse", httpRequest.HaveResponse.ToString());
                dt.Rows.Add("HttpWebRequest", "IfModifiedSince", httpRequest.IfModifiedSince.ToString());
                dt.Rows.Add("HttpWebRequest", "KeepAlive", httpRequest.KeepAlive.ToString());
                dt.Rows.Add("HttpWebRequest", "MaximumAutomaticRedirections", httpRequest.MaximumAutomaticRedirections.ToString());
                dt.Rows.Add("HttpWebRequest", "MaximumResponseHeadersLength", httpRequest.MaximumResponseHeadersLength.ToString());
                dt.Rows.Add("HttpWebRequest", "MediaType", httpRequest.MediaType);
                dt.Rows.Add("HttpWebRequest", "Method", httpRequest.Method);
                dt.Rows.Add("HttpWebRequest", "Pipelined", httpRequest.Pipelined.ToString());
                dt.Rows.Add("HttpWebRequest", "PreAuthenticate", httpRequest.PreAuthenticate.ToString());
                dt.Rows.Add("HttpWebRequest", "ProtocolVersion", httpRequest.ProtocolVersion.ToString(2));
                dt.Rows.Add("HttpWebRequest", "ReadWriteTimeout", httpRequest.ReadWriteTimeout.ToString());
                dt.Rows.Add("HttpWebRequest", "Referer", httpRequest.Referer);
                dt.Rows.Add("HttpWebRequest", "RequestUri", httpRequest.RequestUri.AbsoluteUri);
                dt.Rows.Add("HttpWebRequest", "SendChunked", httpRequest.SendChunked.ToString());
                dt.Rows.Add("HttpWebRequest", "Timeout", httpRequest.Timeout.ToString());
                dt.Rows.Add("HttpWebRequest", "TransferEncoding", httpRequest.TransferEncoding);
                dt.Rows.Add("HttpWebRequest", "UnsafeAuthenticatedConnectionSharing", httpRequest.UnsafeAuthenticatedConnectionSharing.ToString());
                dt.Rows.Add("HttpWebRequest", "UseDefaultCredentials", httpRequest.UseDefaultCredentials.ToString());
                dt.Rows.Add("HttpWebRequest", "UserAgent", httpRequest.UserAgent);
                //dt.Rows.Add("HttpWebRequest", "", httpRequest.);
                WebHeaderCollection headers = httpRequest.Headers;
                dt.Rows.Add("HttpWebRequest", "Headers", "Count", headers.Count);
                for (int i = 0; i < headers.Count; i++)
                {
                    dt.Rows.Add("HttpWebRequest", "Headers", headers.GetKey(i), headers.Get(i));
                }
                //httpRequest.ClientCertificates;
                //httpRequest.ServicePoint
            }
        }
        #endregion

        #region GetWebResponseInfo()
        public DataTable GetWebResponseInfo()
        {
            DataTable dt = zdt.Create("From, Name, Value1, Value2");
            GetWebResponseInfo(dt);
            return dt;
        }
        #endregion

        #region GetWebResponseInfo(DataTable dt)
        public void GetWebResponseInfo(DataTable dt)
        {
            WebResponse response = Response;
            if (response == null)
            {
                dt.Rows.Add("WebResponse", "", "pas de WebResponse");
                return;
            }
            dt.Rows.Add("WebResponse", "ContentLength", response.ContentLength.ToString());
            dt.Rows.Add("WebResponse", "ContentType", response.ContentType);
            dt.Rows.Add("WebResponse", "IsFromCache", response.IsFromCache.ToString());
            dt.Rows.Add("WebResponse", "IsMutuallyAuthenticated", response.IsMutuallyAuthenticated.ToString());
            dt.Rows.Add("WebResponse", "ResponseUri", response.ResponseUri.AbsoluteUri);
            //dt.Rows.Add("WebResponse", "", response.);
            // response.Headers
            if (response is HttpWebResponse)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                dt.Rows.Add("HttpWebResponse", "CharacterSet", httpResponse.CharacterSet);
                dt.Rows.Add("HttpWebResponse", "ContentEncoding", httpResponse.ContentEncoding);
                dt.Rows.Add("HttpWebResponse", "ContentLength", httpResponse.ContentLength.ToString());
                dt.Rows.Add("HttpWebResponse", "ContentType", httpResponse.ContentType);
                dt.Rows.Add("HttpWebResponse", "IsFromCache", httpResponse.IsFromCache.ToString());
                dt.Rows.Add("HttpWebResponse", "IsMutuallyAuthenticated", httpResponse.IsMutuallyAuthenticated.ToString());
                dt.Rows.Add("HttpWebResponse", "LastModified", httpResponse.LastModified.ToString());
                dt.Rows.Add("HttpWebResponse", "Method", httpResponse.Method);
                dt.Rows.Add("HttpWebResponse", "ProtocolVersion", httpResponse.ProtocolVersion.ToString(2));
                dt.Rows.Add("HttpWebResponse", "ResponseUri", httpResponse.ResponseUri.AbsoluteUri);
                dt.Rows.Add("HttpWebResponse", "Server", httpResponse.Server);
                dt.Rows.Add("HttpWebResponse", "StatusCode", httpResponse.StatusCode.ToString());
                dt.Rows.Add("HttpWebResponse", "StatusDescription", httpResponse.StatusDescription);
                //dt.Rows.Add("HttpWebResponse", "", httpResponse.);
                WebHeaderCollection headers = httpResponse.Headers;
                dt.Rows.Add("HttpWebResponse", "Headers", "Count", headers.Count);
                for (int i = 0; i < headers.Count; i++)
                {
                    dt.Rows.Add("HttpWebResponse", "Headers", headers.GetKey(i), headers.Get(i));
                }
            }
        }
        #endregion
        #endregion

        #region static ...
        #region static GetNewUrlFileName(string dir, string url)
        public static string GetNewUrlFileName(string dir, string url)
        {
            return GetNewUrlFileName(dir, url, null);
        }
        #endregion

        #region static GetNewUrlFileName(string dir, string url, string ext)
        public static string GetNewUrlFileName(string dir, string url, string ext)
        {
            string sFile = UrlToFileName(url, ext);
            //return cu.GetNewIndexedFileName(dir, "{0:0000}") + "_" + sFile;
            return cu.GetNewIndexedFileName(Path.Combine(dir, "{0:0000}")) + "_" + sFile;
        }
        #endregion

        #region static UrlToFileName(string sUrl)
        public static string UrlToFileName(string sUrl)
        {
            return UrlToFileName(sUrl, null);
        }
        #endregion

        #region static UrlToFileName(string sUrl, string sExt)
        public static string UrlToFileName(string sUrl, string sExt)
        {
            return UrlToFileName(sUrl, sExt, UrlFileNameType.FileName);
        }
        #endregion

        #region static UrlToFileName(string sUrl, string sExt, HttpUriFileNameType type)
        public static string UrlToFileName(string sUrl, string sExt, UrlFileNameType type)
        {
            string sFile;
            Uri uri = new Uri(sUrl);
            if (type == UrlFileNameType.FileName)
            {
                sFile = uri.LocalPath;
                while (sFile.StartsWith("/")) sFile = sFile.Remove(0, 1);
                sFile = sFile.Replace("/", "_");
                if (sFile != "")
                    sFile = uri.Host + "_" + sFile;
                else
                    sFile = uri.Host;
                if (sExt != null) sFile = sFile + sExt;
            }
            else
            {
                //sFile = uri.AbsoluteUri;
                //sFile = sUrl;
                if (type == UrlFileNameType.Path)
                    sFile = uri.AbsolutePath;
                else if (type == UrlFileNameType.PathAndQuery)
                    sFile = uri.PathAndQuery;
                else if (type == UrlFileNameType.Query)
                    sFile = uri.Query;
                else
                    return null;
                int i = sFile.IndexOf("://");
                if (i != -1) sFile = cu.right(sFile, sFile.Length - i - 3);
                i = sFile.IndexOf('?');
                if (i != -1) sFile = cu.left(sFile, i);
                sFile = sFile.Replace('/', '_');
                //sFile = sFile.Replace('?', '_');
                sFile = sFile.Replace('%', '_');
                //sFile = cu.PathGetFile(sFile) + ".html";
                if (sExt != null) sFile = sFile + sExt;
            }
            sFile = Path.GetFileName(sFile);
            return sFile;
        }
        #endregion

        #region //LoadHttp0
        //public static void LoadHttp0(string sHttp, string sPath)
        //{
        //    WebClient web = new WebClient();
        //    web.Headers.Add(HttpRequestHeader.UserAgent, "pb");
        //    try
        //    {
        //        web.DownloadFile(sHttp, sPath);
        //    }
        //    finally
        //    {
        //        web.Dispose();
        //    }
        //}
        #endregion

        #region //static LoadToFile ...
        #region //static LoadToFile(string url, string path)
        //public static void LoadToFile(string url, string path)
        //{
        //    LoadToFile(url, path, null, false);
        //}
        #endregion

        #region //LoadHttp(string sHttp, string sPath, bool bUseWebClient)
        //public static void LoadHttp(string sHttp, string sPath, bool bUseWebClient)
        //{
        //    LoadHttp(sHttp, sPath, null, bUseWebClient);
        //}
        #endregion

        #region //LoadHttp
        //public static void LoadHttp(string sHttp, string sPath, CookieContainer cookies)
        //{
        //    LoadHttp(sHttp, sPath, cookies, false);
        //}
        #endregion

        #region //static LoadToFile(string url, string path, CookieContainer cookies, bool UseWebClient)
        //public static void LoadToFile(string url, string path, CookieContainer cookies, bool UseWebClient)
        //{
        //    Http http = new Http(url, cookies);
        //    http.UseWebClient = UseWebClient;
        //    try
        //    {
        //        http.Load();
        //        http.SaveResult(path);
        //    }
        //    finally
        //    {
        //        http.Dispose();
        //    }
        //}
        #endregion
        #endregion

        #region static LoadText ...
        #region static LoadText(string url)
        public static string LoadText(string url)
        {
            return LoadText(url, HttpRequestMethod.Get, null, null);
        }
        #endregion

        #region static LoadText(string url, string method, string content)
        public static string LoadText(string url, HttpRequestMethod method, string content)
        {
            return LoadText(url, method, content, null);
        }
        #endregion

        #region static LoadText(string url, HttpRequestMethod method, string content, string referer)
        public static string LoadText(string url, HttpRequestMethod method, string content, string referer)
        {
            Http http = new Http();
            http.Load(url, method, content, referer);
            return http.TextResult;
        }
        #endregion
        #endregion

        #region static LoadToXmlDocument0 ...
        #region static LoadToXmlDocument0(string url)
        public static XmlDocument LoadToXmlDocument0(string url)
        {
            return LoadToXmlDocument0(url, null, false);
        }
        #endregion

        #region static LoadToXmlDocument0(string url, CookieContainer cookies, bool UseWebClient)
        public static XmlDocument LoadToXmlDocument0(string url, CookieContainer cookies, bool UseWebClient)
        {
            Http http = new Http(url, cookies);
            http.UseWebClient = UseWebClient;
            try
            {
                http.Load();
                return http.GetXmlDocumentResult0();
            }
            finally
            {
                http.Dispose();
            }
        }
        #endregion
        #endregion

        #region static LoadToXmlDocument ...
        #region static LoadToXmlDocument(string url)
        public static XmlDocument LoadToXmlDocument(string url)
        {
            return LoadToXmlDocument(url, null, false);
        }
        #endregion

        #region static LoadToXmlDocument(string url, CookieContainer cookies, bool UseWebClient)
        public static XmlDocument LoadToXmlDocument(string url, CookieContainer cookies, bool UseWebClient)
        {
            Http http = new Http(url, cookies);
            http.UseWebClient = UseWebClient;
            try
            {
                http.Load();
                return http.GetXmlDocumentResult();
            }
            finally
            {
                http.Dispose();
            }
        }
        #endregion
        #endregion

        #region static LoadToXDocument ...
        #region static LoadToXDocument(string url)
        public static XDocument LoadToXDocument(string url)
        {
            return LoadToXDocument(url, null, false);
        }
        #endregion

        #region static LoadToXDocument(string url, CookieContainer cookies, bool UseWebClient)
        public static XDocument LoadToXDocument(string url, CookieContainer cookies, bool UseWebClient)
        {
            Http http = new Http(url, cookies);
            http.UseWebClient = UseWebClient;
            try
            {
                http.Load();
                return http.GetXDocumentResult();
            }
            finally
            {
                http.Dispose();
            }
        }
        #endregion
        #endregion

        #region static DownLoad ...
        #region static DownLoad(string path, string url)
        public static void DownLoad(string path, string url)
        {
            DownLoad(path, url, HttpRequestMethod.Get, null, null);
        }
        #endregion

        #region static DownLoad(string path, string url, string method, string content)
        public static void DownLoad(string path, string url, HttpRequestMethod method, string content)
        {
            DownLoad(path, url, method, content, null);
        }
        #endregion

        #region static DownLoad(string path, string url, string method, string content, string referer)
        public static void DownLoad(string path, string url, HttpRequestMethod method, string content, string referer)
        {
            Http http = new Http();
            http.LoadToFile(path, url, method, content, referer);
        }
        #endregion
        #endregion

        #region static GetContentFileExtension
        public static string GetContentFileExtension(string sContentType)
        {
            sContentType = sContentType.ToLower();
            //if (sContentType == "text/html")
            if (sContentType.EndsWith("/html"))
                return ".html";
            //else if (sContentType == "text/xml" || sContentType == "application/xml")
            else if (sContentType.EndsWith("/xml"))
                return ".xml";
            else
                return ".txt";
        }
        #endregion

        #region static UrlLocalPath
        public static string UrlLocalPath(string sUrl)
        {
            Uri uri = new Uri(sUrl);
            return uri.LocalPath;
        }
        #endregion

        #region static GetUrl(string sBaseUrl, string sUrl)
        public static string GetUrl(string sBaseUrl, string sUrl)
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
        #endregion

        #region static TranslateCode
        public static string TranslateCode(string sValue)
        {
            if (sValue == null) return null;
            // &gt;  &aaa;
            int i = 0;
            while (true)
            {
                Match match = grxTranslate1.Match(sValue, i);
                if (!match.Success) break;
                //string sName = match.Value.Substring(1, match.Value.Length - 2);
                string sName = match.Groups[1].Value;
                //char c = HtmlCharList.GetHtmlChar(sName).c;
                HtmlChar htmlChar = HtmlCharList.GetHtmlChar(sName);
                if (htmlChar != null)
                {
                    char c = htmlChar.c;
                    sValue = sValue.Substring(0, match.Index) + c.ToString() + sValue.Substring(match.Index + match.Length, sValue.Length - match.Index - match.Length);
                }
                i = match.Index + 1;
            }

            // &#62; &#nnn;
            i = 0;
            while (true)
            {
                Match match = grxTranslate2.Match(sValue, i);
                if (!match.Success) break;
                //string sCode = match.Value.Substring(2, match.Value.Length - 3);
                string sCode = match.Groups[1].Value;
                int iCode = int.Parse(sCode);
                char c = (char)iCode;
                sValue = sValue.Substring(0, match.Index) + c.ToString() + sValue.Substring(match.Index + match.Length, sValue.Length - match.Index - match.Length);
                i = match.Index + 1;
            }

            // &#xB7; &#xnn;
            i = 0;
            while (true)
            {
                Match match = grxTranslate3.Match(sValue, i);
                if (!match.Success) break;
                //string sCode = match.Value.Substring(3, match.Value.Length - 4);
                string sCode = match.Groups[1].Value;
                int iCode = int.Parse(sCode, System.Globalization.NumberStyles.AllowHexSpecifier);
                char c = (char)iCode;
                sValue = sValue.Substring(0, match.Index) + c.ToString() + sValue.Substring(match.Index + match.Length, sValue.Length - match.Index - match.Length);
                i = match.Index + 1;
            }

            return sValue;
        }
        #endregion

        #region static GetHttpRequestMethod
        public static HttpRequestMethod GetHttpRequestMethod(string method)
        {
            switch (method.ToLower())
            {
                case "get":
                    return HttpRequestMethod.Get;
                case "post":
                    return HttpRequestMethod.Post;
                default:
                    throw new PBException("Error unknow HttpRequestMethod \"{0}\"", method);
            }
        }
        #endregion
        #endregion
    }
}
