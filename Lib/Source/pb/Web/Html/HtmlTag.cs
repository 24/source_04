using System;
using System.Collections.Generic;

namespace pb.Web
{
    public enum HtmlTagCategory
    {
        NoCategory = 0,
        Head = 1,
        Table,
        DefinitionList
    }

    public enum HtmlBoundType
    {
        Required = 1,
        Optional,
        Forbidden
    }

    public enum HtmlDTDType // Document Type Definition
    {
        NoDTD = 0,
        LooseDTD = 1,
        FramesetDTD
    }

    public enum HtmlTagType
    {
        Unknow = 0,
        Html = 1,
        Head,
        Body,
        Title,
        Meta,
        A,
        Abbr,
        Acronym,
        Address,
        Applet,
        Area,
        B,
        Base,
        BaseFont,
        Bdo,
        Big,
        Blockquote,
        BR,
        Button,
        Caption,
        Center,
        Cite,
        Code,
        Col,
        ColGroup,
        DD,
        Del,
        Dfn,
        Dir,
        Div,
        DL,
        DT,
        EM,
        FieldSet,
        Font,
        Form,
        Frame,
        FrameSet,
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
        Hr,
        I,
        IFrame,
        Img,
        Input,
        Ins,
        IsIndex,
        Kbd,
        Label,
        Legend,
        LI,
        Link,
        Map,
        Menu,
        NoFrames,
        NoScript,
        Object,
        OL,
        OptGroup,
        Option,
        P,
        Param,
        Pre,
        Q,
        S,
        Samp,
        Script,
        Select,
        Small,
        Span,
        Strike,
        Strong,
        Style,
        Sub,
        Sup,
        Table,
        TBody,
        TD,
        TextArea,
        TFoot,
        TH,
        THead,
        TR,
        TT,
        U,
        UL,
        Var
    }

    public class HtmlTag
    {
        public string TagName;
        public HtmlTagType TagType;
        public HtmlTagCategory TagCategory;
        public HtmlBoundType StartBoundType;
        public HtmlBoundType EndBoundType;
        public bool Empty;
        public bool Deprecated;                      // Deprecated = Obsolète
        public HtmlDTDType DTDType;                  // Document Type Definition
        //public bool Head;

        //public HtmlTag(string TagName, HtmlTagType TagType, HtmlBoundType StartTagType, HtmlBoundType EndTagType, bool Empty, bool Deprecated, HtmlDTDType DTDType)
        //{
        //    this.TagName = TagName;
        //    this.TagType = TagType;
        //    this.StartBoundType = StartTagType;
        //    this.EndBoundType = EndTagType;
        //    this.Empty = Empty;
        //    this.Deprecated = Deprecated;
        //    this.DTDType = DTDType;
        //    this.TagCategory = HtmlTagCategory.NoCategory;
        //}

        public HtmlTag(string TagName, HtmlTagType TagType, HtmlBoundType StartTagType, HtmlBoundType EndTagType, bool Empty, bool Deprecated, HtmlDTDType DTDType,
            HtmlTagCategory TagCategory = HtmlTagCategory.NoCategory)
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
    }

    public class HtmlTags
    {
        //private static SortedList<string, HtmlTagType> __tagTypes;
        private static Dictionary<string, HtmlTagType> __tagTypes;
        //private static SortedList<HtmlTagType, HtmlTag> __tags;
        private static Dictionary<HtmlTagType, HtmlTag> __tags;

        static HtmlTags()
        {
            InitTagType();
            InitTagList();
        }

        public static HtmlTagType GetHtmlTagType(string tagName)
        {
            //int i = __tagTypes.IndexOfKey(sTagName);
            //if (i != -1) return __tagTypes.Values[i];
            if (__tagTypes.ContainsKey(tagName))
                return __tagTypes[tagName];
            else
                return HtmlTagType.Unknow;
        }

        public static HtmlTag GetHtmlTag(HtmlTagType tagType)
        {
            //int i = __tags.IndexOfKey(TagType);
            //if (i != -1) return __tags.Values[i];
            if (__tags.ContainsKey(tagType))
                return __tags[tagType];
            else
                return null;
        }

        private static void InitTagType()
        {
            __tagTypes = new Dictionary<string, HtmlTagType>();
            __tagTypes.Add("a", HtmlTagType.A);
            __tagTypes.Add("abbr", HtmlTagType.Abbr);
            __tagTypes.Add("acronym", HtmlTagType.Acronym);
            __tagTypes.Add("address", HtmlTagType.Address);
            __tagTypes.Add("applet", HtmlTagType.Applet);
            __tagTypes.Add("area", HtmlTagType.Area);
            __tagTypes.Add("b", HtmlTagType.B);
            __tagTypes.Add("base", HtmlTagType.Base);
            __tagTypes.Add("basefont", HtmlTagType.BaseFont);
            __tagTypes.Add("bdo", HtmlTagType.Bdo);
            __tagTypes.Add("big", HtmlTagType.Big);
            __tagTypes.Add("blockquote", HtmlTagType.Blockquote);
            __tagTypes.Add("body", HtmlTagType.Body);
            __tagTypes.Add("br", HtmlTagType.BR);
            __tagTypes.Add("button", HtmlTagType.Button);
            __tagTypes.Add("caption", HtmlTagType.Caption);
            __tagTypes.Add("center", HtmlTagType.Center);
            __tagTypes.Add("cite", HtmlTagType.Cite);
            __tagTypes.Add("code", HtmlTagType.Code);
            __tagTypes.Add("col", HtmlTagType.Col);
            __tagTypes.Add("colgroup", HtmlTagType.ColGroup);
            __tagTypes.Add("dd", HtmlTagType.DD);
            __tagTypes.Add("del", HtmlTagType.Del);
            __tagTypes.Add("dfn", HtmlTagType.Dfn);
            __tagTypes.Add("dir", HtmlTagType.Dir);
            __tagTypes.Add("div", HtmlTagType.Div);
            __tagTypes.Add("dl", HtmlTagType.DL);
            __tagTypes.Add("dt", HtmlTagType.DT);
            __tagTypes.Add("em", HtmlTagType.EM);
            __tagTypes.Add("fieldset", HtmlTagType.FieldSet);
            __tagTypes.Add("font", HtmlTagType.Font);
            __tagTypes.Add("form", HtmlTagType.Form);
            __tagTypes.Add("frame", HtmlTagType.Frame);
            __tagTypes.Add("frameset", HtmlTagType.FrameSet);
            __tagTypes.Add("h1", HtmlTagType.H1);
            __tagTypes.Add("h2", HtmlTagType.H2);
            __tagTypes.Add("h3", HtmlTagType.H3);
            __tagTypes.Add("h4", HtmlTagType.H4);
            __tagTypes.Add("h5", HtmlTagType.H5);
            __tagTypes.Add("h6", HtmlTagType.H6);
            __tagTypes.Add("head", HtmlTagType.Head);
            __tagTypes.Add("hr", HtmlTagType.Hr);
            __tagTypes.Add("html", HtmlTagType.Html);
            __tagTypes.Add("i", HtmlTagType.I);
            __tagTypes.Add("iframe", HtmlTagType.IFrame);
            __tagTypes.Add("img", HtmlTagType.Img);
            __tagTypes.Add("input", HtmlTagType.Input);
            __tagTypes.Add("ins", HtmlTagType.Ins);
            __tagTypes.Add("isindex", HtmlTagType.IsIndex);
            __tagTypes.Add("kbd", HtmlTagType.Kbd);
            __tagTypes.Add("label", HtmlTagType.Label);
            __tagTypes.Add("legend", HtmlTagType.Legend);
            __tagTypes.Add("li", HtmlTagType.LI);
            __tagTypes.Add("link", HtmlTagType.Link);
            __tagTypes.Add("map", HtmlTagType.Map);
            __tagTypes.Add("menu", HtmlTagType.Menu);
            __tagTypes.Add("meta", HtmlTagType.Meta);
            __tagTypes.Add("noframes", HtmlTagType.NoFrames);
            __tagTypes.Add("noscript", HtmlTagType.NoScript);
            __tagTypes.Add("object", HtmlTagType.Object);
            __tagTypes.Add("ol", HtmlTagType.OL);
            __tagTypes.Add("optgroup", HtmlTagType.OptGroup);
            __tagTypes.Add("option", HtmlTagType.Option);
            __tagTypes.Add("p", HtmlTagType.P);
            __tagTypes.Add("param", HtmlTagType.Param);
            __tagTypes.Add("pre", HtmlTagType.Pre);
            __tagTypes.Add("q", HtmlTagType.Q);
            __tagTypes.Add("s", HtmlTagType.S);
            __tagTypes.Add("samp", HtmlTagType.Samp);
            __tagTypes.Add("script", HtmlTagType.Script);
            __tagTypes.Add("select", HtmlTagType.Select);
            __tagTypes.Add("small", HtmlTagType.Small);
            __tagTypes.Add("span", HtmlTagType.Span);
            __tagTypes.Add("strike", HtmlTagType.Strike);
            __tagTypes.Add("strong", HtmlTagType.Strong);
            __tagTypes.Add("style", HtmlTagType.Style);
            __tagTypes.Add("sub", HtmlTagType.Sub);
            __tagTypes.Add("sup", HtmlTagType.Sup);
            __tagTypes.Add("table", HtmlTagType.Table);
            __tagTypes.Add("tbody", HtmlTagType.TBody);
            __tagTypes.Add("td", HtmlTagType.TD);
            __tagTypes.Add("textarea", HtmlTagType.TextArea);
            __tagTypes.Add("tfoot", HtmlTagType.TFoot);
            __tagTypes.Add("th", HtmlTagType.TH);
            __tagTypes.Add("thead", HtmlTagType.THead);
            __tagTypes.Add("title", HtmlTagType.Title);
            __tagTypes.Add("tr", HtmlTagType.TR);
            __tagTypes.Add("tt", HtmlTagType.TT);
            __tagTypes.Add("u", HtmlTagType.U);
            __tagTypes.Add("ul", HtmlTagType.UL);
            __tagTypes.Add("var", HtmlTagType.Var);
        }

        private static void InitTagList()
        {
            __tags = new Dictionary<HtmlTagType, HtmlTag>();
            //__tags.Add(HtmlTagTypeEnum.Unknow, new HtmlTag("unknow", HtmlTagTypeEnum.Unknow, HtmlBoundTypeEnum.Optional, HtmlBoundTypeEnum.Forbidden, false, false, HtmlDTDTypeEnum.NoDTD));
            __tags.Add(HtmlTagType.Unknow, new HtmlTag("unknow", HtmlTagType.Unknow, HtmlBoundType.Optional, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.A, new HtmlTag("a", HtmlTagType.A, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Abbr, new HtmlTag("abbr", HtmlTagType.Abbr, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Acronym, new HtmlTag("acronym", HtmlTagType.Acronym, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Address, new HtmlTag("address", HtmlTagType.Address, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Applet, new HtmlTag("applet", HtmlTagType.Applet, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Area, new HtmlTag("area", HtmlTagType.Area, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.B, new HtmlTag("b", HtmlTagType.B, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            // HtmlTagCategory.Head
            __tags.Add(HtmlTagType.Base, new HtmlTag("base", HtmlTagType.Base, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD, HtmlTagCategory.Head));
            __tags.Add(HtmlTagType.BaseFont, new HtmlTag("basefont", HtmlTagType.BaseFont, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Bdo, new HtmlTag("bdo", HtmlTagType.Bdo, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Big, new HtmlTag("big", HtmlTagType.Big, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Blockquote, new HtmlTag("blockquote", HtmlTagType.Blockquote, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Body, new HtmlTag("body", HtmlTagType.Body, HtmlBoundType.Optional, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.BR, new HtmlTag("br", HtmlTagType.BR, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Button, new HtmlTag("button", HtmlTagType.Button, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Caption, new HtmlTag("caption", HtmlTagType.Caption, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Center, new HtmlTag("center", HtmlTagType.Center, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Cite, new HtmlTag("cite", HtmlTagType.Cite, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Code, new HtmlTag("code", HtmlTagType.Code, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Col, new HtmlTag("col", HtmlTagType.Col, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.ColGroup, new HtmlTag("colgroup", HtmlTagType.ColGroup, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.DD, new HtmlTag("dd", HtmlTagType.DD, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.DefinitionList));
            __tags.Add(HtmlTagType.Del, new HtmlTag("del", HtmlTagType.Del, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Dfn, new HtmlTag("dfn", HtmlTagType.Dfn, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Dir, new HtmlTag("dir", HtmlTagType.Dir, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Div, new HtmlTag("div", HtmlTagType.Div, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.DL, new HtmlTag("dl", HtmlTagType.DL, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.DefinitionList));
            __tags.Add(HtmlTagType.DT, new HtmlTag("dt", HtmlTagType.DT, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.DefinitionList));
            __tags.Add(HtmlTagType.EM, new HtmlTag("em", HtmlTagType.EM, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.FieldSet, new HtmlTag("fieldset", HtmlTagType.FieldSet, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Font, new HtmlTag("font", HtmlTagType.Font, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Form, new HtmlTag("form", HtmlTagType.Form, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Frame, new HtmlTag("frame", HtmlTagType.Frame, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.FramesetDTD));
            __tags.Add(HtmlTagType.FrameSet, new HtmlTag("frameset", HtmlTagType.FrameSet, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.FramesetDTD));
            __tags.Add(HtmlTagType.H1, new HtmlTag("h1", HtmlTagType.H1, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.H2, new HtmlTag("h2", HtmlTagType.H2, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.H3, new HtmlTag("h3", HtmlTagType.H3, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.H4, new HtmlTag("h4", HtmlTagType.H4, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.H5, new HtmlTag("h5", HtmlTagType.H5, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.H6, new HtmlTag("h6", HtmlTagType.H6, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Head, new HtmlTag("head", HtmlTagType.Head, HtmlBoundType.Optional, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Hr, new HtmlTag("hr", HtmlTagType.Hr, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Html, new HtmlTag("html", HtmlTagType.Html, HtmlBoundType.Optional, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.I, new HtmlTag("i", HtmlTagType.I, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.IFrame, new HtmlTag("iframe", HtmlTagType.IFrame, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Img, new HtmlTag("img", HtmlTagType.Img, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Input, new HtmlTag("input", HtmlTagType.Input, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Ins, new HtmlTag("ins", HtmlTagType.Ins, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.IsIndex, new HtmlTag("isindex", HtmlTagType.IsIndex, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Kbd, new HtmlTag("kbd", HtmlTagType.Kbd, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Label, new HtmlTag("label", HtmlTagType.Label, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Legend, new HtmlTag("legend", HtmlTagType.Legend, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.LI, new HtmlTag("li", HtmlTagType.LI, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD));
            // HtmlTagCategory.Head
            __tags.Add(HtmlTagType.Link, new HtmlTag("link", HtmlTagType.Link, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD, HtmlTagCategory.Head));
            __tags.Add(HtmlTagType.Map, new HtmlTag("map", HtmlTagType.Map, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Menu, new HtmlTag("menu", HtmlTagType.Menu, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Meta, new HtmlTag("meta", HtmlTagType.Meta, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD, HtmlTagCategory.Head));
            __tags.Add(HtmlTagType.NoFrames, new HtmlTag("noframes", HtmlTagType.NoFrames, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.FramesetDTD));
            __tags.Add(HtmlTagType.NoScript, new HtmlTag("noscript", HtmlTagType.NoScript, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Object, new HtmlTag("object", HtmlTagType.Object, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.OL, new HtmlTag("ol", HtmlTagType.OL, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.OptGroup, new HtmlTag("optgroup", HtmlTagType.OptGroup, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Option, new HtmlTag("option", HtmlTagType.Option, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.P, new HtmlTag("p", HtmlTagType.P, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Param, new HtmlTag("param", HtmlTagType.Param, HtmlBoundType.Required, HtmlBoundType.Forbidden, true, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Pre, new HtmlTag("pre", HtmlTagType.Pre, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Q, new HtmlTag("q", HtmlTagType.Q, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.S, new HtmlTag("s", HtmlTagType.S, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Samp, new HtmlTag("samp", HtmlTagType.Samp, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            // HtmlTagCategory.Head
            __tags.Add(HtmlTagType.Script, new HtmlTag("script", HtmlTagType.Script, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Head));
            __tags.Add(HtmlTagType.Select, new HtmlTag("select", HtmlTagType.Select, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Small, new HtmlTag("small", HtmlTagType.Small, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Span, new HtmlTag("span", HtmlTagType.Span, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Strike, new HtmlTag("strike", HtmlTagType.Strike, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.Strong, new HtmlTag("strong", HtmlTagType.Strong, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Style, new HtmlTag("style", HtmlTagType.Style, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Head));
            __tags.Add(HtmlTagType.Sub, new HtmlTag("sub", HtmlTagType.Sub, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Sup, new HtmlTag("sup", HtmlTagType.Sup, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Table, new HtmlTag("table", HtmlTagType.Table, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.TBody, new HtmlTag("tbody", HtmlTagType.TBody, HtmlBoundType.Optional, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.TD, new HtmlTag("td", HtmlTagType.TD, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.TextArea, new HtmlTag("textarea", HtmlTagType.TextArea, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.TFoot, new HtmlTag("tfoot", HtmlTagType.TFoot, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.TH, new HtmlTag("th", HtmlTagType.TH, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.THead, new HtmlTag("thead", HtmlTagType.THead, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.Title, new HtmlTag("title", HtmlTagType.Title, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Head));
            __tags.Add(HtmlTagType.TR, new HtmlTag("tr", HtmlTagType.TR, HtmlBoundType.Required, HtmlBoundType.Optional, false, false, HtmlDTDType.NoDTD, HtmlTagCategory.Table));
            __tags.Add(HtmlTagType.TT, new HtmlTag("tt", HtmlTagType.TT, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.U, new HtmlTag("u", HtmlTagType.U, HtmlBoundType.Required, HtmlBoundType.Required, false, true, HtmlDTDType.LooseDTD));
            __tags.Add(HtmlTagType.UL, new HtmlTag("ul", HtmlTagType.UL, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
            __tags.Add(HtmlTagType.Var, new HtmlTag("var", HtmlTagType.Var, HtmlBoundType.Required, HtmlBoundType.Required, false, false, HtmlDTDType.NoDTD));
        }
    }
}
