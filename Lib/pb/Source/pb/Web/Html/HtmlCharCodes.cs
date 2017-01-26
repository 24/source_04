using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace pb.Web.Html
{
    public class HtmlCharCode
    {
        public string Name;
        public short Code;
        public char Char;
        public string Description;

        public HtmlCharCode(string Name, short Code, char Char, string Description)
        {
            this.Name = Name;
            this.Code = Code;
            this.Char = Char;
            this.Description = Description;
        }
    }

    public static class HtmlCharCodes
    {
        //private static SortedList<string, HtmlCharCode> _charCodes = new SortedList<string, HtmlCharCode>();
        private static Dictionary<string, HtmlCharCode> _charCodes = new Dictionary<string, HtmlCharCode>();

        static HtmlCharCodes()
        {
            Init();
        }

        public static HtmlCharCode GetHtmlChar(string name)
        {
            //int i = _charCodes.IndexOfKey(Name);
            //if (i == -1) return null;
            //return _charCodes.Values[i];
            if (_charCodes.ContainsKey(name))
                return _charCodes[name];
            else
                return null;
        }

        private static void Init()
        {
            HtmlCharCode[] charCodes =
                {
                    // Character latin 1
                    new HtmlCharCode("nbsp", 160, (char)160, "no-break space = non-breaking space"),
                    new HtmlCharCode("iexcl", 161, (char)161, "inverted exclamation mark"),
                    new HtmlCharCode("cent", 162, (char)162, "cent sign"),
                    new HtmlCharCode("pound", 163, (char)163, "pound sign"),
                    new HtmlCharCode("curren", 164, (char)164, "currency sign"),
                    new HtmlCharCode("yen", 165, (char)165, "yen sign = yuan sign"),
                    new HtmlCharCode("brvbar", 166, (char)166, "broken bar = broken vertical bar"),
                    new HtmlCharCode("sect", 167, (char)167, "section sign"),
                    new HtmlCharCode("uml", 168, (char)168, "diaeresis = spacing diaeresis"),
                    new HtmlCharCode("copy", 169, (char)169, "copyright sign"),
                    new HtmlCharCode("ordf", 170, (char)170, "feminine ordinal indicator"),
                    new HtmlCharCode("laquo", 171, (char)171, "left-pointing double angle quotation mark = left pointing guillemet"),
                    new HtmlCharCode("not", 172, (char)172, "not sign"),
                    new HtmlCharCode("shy", 173, (char)173, "soft hyphen = discretionary hyphen"),
                    new HtmlCharCode("reg", 174, (char)174, "registered sign = registered trade mark sign"),
                    new HtmlCharCode("macr", 175, (char)175, "macron = spacing macron = overline = APL overbar"),
                    new HtmlCharCode("deg", 176, (char)176, "degree sign"),
                    new HtmlCharCode("plusmn", 177, (char)177, "plus-minus sign = plus-or-minus sign"),
                    new HtmlCharCode("sup2", 178, (char)178, "superscript two = superscript digit two = squared"),
                    new HtmlCharCode("sup3", 179, (char)179, "superscript three = superscript digit three = cubed"),
                    new HtmlCharCode("acute", 180, (char)180, "acute accent = spacing acute"),
                    new HtmlCharCode("micro", 181, (char)181, "micro sign"),
                    new HtmlCharCode("para", 182, (char)182, "pilcrow sign = paragraph sign"),
                    new HtmlCharCode("middot", 183, (char)183, "middle dot = Georgian comma = Greek middle dot"),
                    new HtmlCharCode("cedil", 184, (char)184, "cedilla = spacing cedilla"),
                    new HtmlCharCode("sup1", 185, (char)185, "superscript one = superscript digit one"),
                    new HtmlCharCode("ordm", 186, (char)186, "masculine ordinal indicator"),
                    new HtmlCharCode("raquo", 187, (char)187, "right-pointing double angle quotation mark = right pointing guillemet"),
                    new HtmlCharCode("frac14", 188, (char)188, "vulgar fraction one quarter = fraction one quarter"),
                    new HtmlCharCode("frac12", 189, (char)189, "vulgar fraction one half = fraction one half"),
                    new HtmlCharCode("frac34", 190, (char)190, "vulgar fraction three quarters = fraction three quarters"),
                    new HtmlCharCode("iquest", 191, (char)191, "inverted question mark = turned question mark"),
                    new HtmlCharCode("Agrave", 192, (char)192, "latin capital letter A with grave = latin capital letter A grave"),
                    new HtmlCharCode("Aacute", 193, (char)193, "latin capital letter A with acute"),
                    new HtmlCharCode("Acirc", 194, (char)194, "latin capital letter A with circumflex"),
                    new HtmlCharCode("Atilde", 195, (char)195, "latin capital letter A with tilde"),
                    new HtmlCharCode("Auml", 196, (char)196, "latin capital letter A with diaeresis"),
                    new HtmlCharCode("Aring", 197, (char)197, "latin capital letter A with ring above = latin capital letter A ring"),
                    new HtmlCharCode("AElig", 198, (char)198, "latin capital letter AE = latin capital ligature AE"),
                    new HtmlCharCode("Ccedil", 199, (char)199, "latin capital letter C with cedilla"),
                    new HtmlCharCode("Egrave", 200, (char)200, "latin capital letter E with grave"),
                    new HtmlCharCode("Eacute", 201, (char)201, "latin capital letter E with acute"),
                    new HtmlCharCode("Ecirc", 202, (char)202, "latin capital letter E with circumflex"),
                    new HtmlCharCode("Euml", 203, (char)203, "latin capital letter E with diaeresis"),
                    new HtmlCharCode("Igrave", 204, (char)204, "latin capital letter I with grave"),
                    new HtmlCharCode("Iacute", 205, (char)205, "latin capital letter I with acute"),
                    new HtmlCharCode("Icirc", 206, (char)206, "latin capital letter I with circumflex"),
                    new HtmlCharCode("Iuml", 207, (char)207, "latin capital letter I with diaeresis"),
                    new HtmlCharCode("ETH", 208, (char)208, "latin capital letter ETH"),
                    new HtmlCharCode("Ntilde", 209, (char)209, "latin capital letter N with tilde"),
                    new HtmlCharCode("Ograve", 210, (char)210, "latin capital letter O with grave"),
                    new HtmlCharCode("Oacute", 211, (char)211, "latin capital letter O with acute"),
                    new HtmlCharCode("Ocirc", 212, (char)212, "latin capital letter O with circumflex"),
                    new HtmlCharCode("Otilde", 213, (char)213, "latin capital letter O with tilde"),
                    new HtmlCharCode("Ouml", 214, (char)214, "latin capital letter O with diaeresis"),
                    new HtmlCharCode("times", 215, (char)215, "multiplication sign"),
                    new HtmlCharCode("Oslash", 216, (char)216, "latin capital letter O with stroke = latin capital letter O slash"),
                    new HtmlCharCode("Ugrave", 217, (char)217, "latin capital letter U with grave"),
                    new HtmlCharCode("Uacute", 218, (char)218, "latin capital letter U with acute"),
                    new HtmlCharCode("Ucirc", 219, (char)219, "latin capital letter U with circumflex"),
                    new HtmlCharCode("Uuml", 220, (char)220, "latin capital letter U with diaeresis"),
                    new HtmlCharCode("Yacute", 221, (char)221, "latin capital letter Y with acute"),
                    new HtmlCharCode("THORN", 222, (char)222, "latin capital letter THORN"),
                    new HtmlCharCode("szlig", 223, (char)223, "latin small letter sharp s = ess-zed"),
                    new HtmlCharCode("agrave", 224, (char)224, "latin small letter a with grave = latin small letter a grave"),
                    new HtmlCharCode("aacute", 225, (char)225, "latin small letter a with acute"),
                    new HtmlCharCode("acirc", 226, (char)226, "latin small letter a with circumflex"),
                    new HtmlCharCode("atilde", 227, (char)227, "latin small letter a with tilde"),
                    new HtmlCharCode("auml", 228, (char)228, "latin small letter a with diaeresis"),
                    new HtmlCharCode("aring", 229, (char)229, "latin small letter a with ring above = latin small letter a ring"),
                    new HtmlCharCode("aelig", 230, (char)230, "latin small letter ae = latin small ligature ae"),
                    new HtmlCharCode("ccedil", 231, (char)231, "latin small letter c with cedilla"),
                    new HtmlCharCode("egrave", 232, (char)232, "latin small letter e with grave"),
                    new HtmlCharCode("eacute", 233, (char)233, "latin small letter e with acute"),
                    new HtmlCharCode("ecirc", 234, (char)234, "latin small letter e with circumflex"),
                    new HtmlCharCode("euml", 235, (char)235, "latin small letter e with diaeresis"),
                    new HtmlCharCode("igrave", 236, (char)236, "latin small letter i with grave"),
                    new HtmlCharCode("iacute", 237, (char)237, "latin small letter i with acute"),
                    new HtmlCharCode("icirc", 238, (char)238, "latin small letter i with circumflex"),
                    new HtmlCharCode("iuml", 239, (char)239, "latin small letter i with diaeresis"),
                    new HtmlCharCode("eth", 240, (char)240, "latin small letter eth"),
                    new HtmlCharCode("ntilde", 241, (char)241, "latin small letter n with tilde"),
                    new HtmlCharCode("ograve", 242, (char)242, "latin small letter o with grave"),
                    new HtmlCharCode("oacute", 243, (char)243, "latin small letter o with acute"),
                    new HtmlCharCode("ocirc", 244, (char)244, "latin small letter o with circumflex"),
                    new HtmlCharCode("otilde", 245, (char)245, "latin small letter o with tilde"),
                    new HtmlCharCode("ouml", 246, (char)246, "latin small letter o with diaeresis"),
                    new HtmlCharCode("divide", 247, (char)247, "division sign"),
                    new HtmlCharCode("oslash", 248, (char)248, "latin small letter o with stroke, = latin small letter o slash"),
                    new HtmlCharCode("ugrave", 249, (char)249, "latin small letter u with grave"),
                    new HtmlCharCode("uacute", 250, (char)250, "latin small letter u with acute"),
                    new HtmlCharCode("ucirc", 251, (char)251, "latin small letter u with circumflex"),
                    new HtmlCharCode("uuml", 252, (char)252, "latin small letter u with diaeresis"),
                    new HtmlCharCode("yacute", 253, (char)253, "latin small letter y with acute"),
                    new HtmlCharCode("thorn", 254, (char)254, "latin small letter thorn"),
                    new HtmlCharCode("yuml", 255, (char)255, "latin small letter y with diaeresis"),

                    // Character special
                    new HtmlCharCode("quot", 34, (char)34, "quotation mark = APL quote"),
                    new HtmlCharCode("amp", 38, (char)38, "ampersand"),
                    new HtmlCharCode("lt", 60, (char)60, "less-than sign"),
                    new HtmlCharCode("gt", 62, (char)62, "greater-than sign"),
                    new HtmlCharCode("OElig", 338, (char)338, "latin capital ligature OE"),
                    new HtmlCharCode("oelig", 339, (char)339, "latin small ligature oe"),
                    new HtmlCharCode("Scaron", 352, (char)352, "latin capital letter S with caron"),
                    new HtmlCharCode("scaron", 353, (char)353, "latin small letter s with caron"),
                    new HtmlCharCode("Yuml", 376, (char)376, "latin capital letter Y with diaeresis"),
                    new HtmlCharCode("circ", 710, (char)710, "modifier letter circumflex accent"),
                    new HtmlCharCode("tilde", 732, (char)732, "small tilde"),
                    new HtmlCharCode("ensp", 8194, (char)8194, "en space"),
                    new HtmlCharCode("emsp", 8195, (char)8195, "em space"),
                    new HtmlCharCode("thinsp", 8201, (char)8201, "thin space"),
                    new HtmlCharCode("zwnj", 8204, (char)8204, "zero width non-joiner"),
                    new HtmlCharCode("zwj", 8205, (char)8205, "zero width joiner"),
                    new HtmlCharCode("lrm", 8206, (char)8206, "left-to-right mark"),
                    new HtmlCharCode("rlm", 8207, (char)8207, "right-to-left mark"),
                    new HtmlCharCode("ndash", 8211, (char)8211, "en dash"),
                    new HtmlCharCode("mdash", 8212, (char)8212, "em dash"),
                    new HtmlCharCode("lsquo", 8216, (char)8216, "left single quotation mark"),
                    new HtmlCharCode("rsquo", 8217, (char)8217, "right single quotation mark"),
                    new HtmlCharCode("sbquo", 8218, (char)8218, "single low-9 quotation mark"),
                    new HtmlCharCode("ldquo", 8220, (char)8220, "left double quotation mark"),
                    new HtmlCharCode("rdquo", 8221, (char)8221, "right double quotation mark"),
                    new HtmlCharCode("bdquo", 8222, (char)8222, "double low-9 quotation mark"),
                    new HtmlCharCode("dagger", 8224, (char)8224, "dagger"),
                    new HtmlCharCode("Dagger", 8225, (char)8225, "double dagger"),
                    new HtmlCharCode("permil", 8240, (char)8240, "per mille sign"),
                    new HtmlCharCode("lsaquo", 8249, (char)8249, "single left-pointing angle quotation mark"),
                    new HtmlCharCode("rsaquo", 8250, (char)8250, "single right-pointing angle quotation mark"),
                    new HtmlCharCode("euro", 8364, (char)8364, "euro sign"),

                    // Character symbols
                    new HtmlCharCode("fnof", 402, (char)402, "latin small f with hook = function = florin"),
                    new HtmlCharCode("Alpha", 913, (char)913, "greek capital letter alpha"),
                    new HtmlCharCode("Beta", 914, (char)914, "greek capital letter beta"),
                    new HtmlCharCode("Gamma", 915, (char)915, "greek capital letter gamma"),
                    new HtmlCharCode("Delta", 916, (char)916, "greek capital letter delta"),
                    new HtmlCharCode("Epsilon", 917, (char)917, "greek capital letter epsilon"),
                    new HtmlCharCode("Zeta", 918, (char)918, "greek capital letter zeta"),
                    new HtmlCharCode("Eta", 919, (char)919, "greek capital letter eta"),
                    new HtmlCharCode("Theta", 920, (char)920, "greek capital letter theta"),
                    new HtmlCharCode("Iota", 921, (char)921, "greek capital letter iota"),
                    new HtmlCharCode("Kappa", 922, (char)922, "greek capital letter kappa"),
                    new HtmlCharCode("Lambda", 923, (char)923, "greek capital letter lambda"),
                    new HtmlCharCode("Mu", 924, (char)924, "greek capital letter mu"),
                    new HtmlCharCode("Nu", 925, (char)925, "greek capital letter nu"),
                    new HtmlCharCode("Xi", 926, (char)926, "greek capital letter xi"),
                    new HtmlCharCode("Omicron", 927, (char)927, "greek capital letter omicron"),
                    new HtmlCharCode("Pi", 928, (char)928, "greek capital letter pi"),
                    new HtmlCharCode("Rho", 929, (char)929, "greek capital letter rho"),
                    new HtmlCharCode("Sigma", 931, (char)931, "greek capital letter sigma"),
                    new HtmlCharCode("Tau", 932, (char)932, "greek capital letter tau"),
                    new HtmlCharCode("Upsilon", 933, (char)933, "greek capital letter upsilon"),
                    new HtmlCharCode("Phi", 934, (char)934, "greek capital letter phi"),
                    new HtmlCharCode("Chi", 935, (char)935, "greek capital letter chi"),
                    new HtmlCharCode("Psi", 936, (char)936, "greek capital letter psi"),
                    new HtmlCharCode("Omega", 937, (char)937, "greek capital letter omega"),
                    new HtmlCharCode("alpha", 945, (char)945, "greek small letter alpha"),
                    new HtmlCharCode("beta", 946, (char)946, "greek small letter beta"),
                    new HtmlCharCode("gamma", 947, (char)947, "greek small letter gamma"),
                    new HtmlCharCode("delta", 948, (char)948, "greek small letter delta"),
                    new HtmlCharCode("epsilon", 949, (char)949, "greek small letter epsilon"),
                    new HtmlCharCode("zeta", 950, (char)950, "greek small letter zeta"),
                    new HtmlCharCode("eta", 951, (char)951, "greek small letter eta"),
                    new HtmlCharCode("theta", 952, (char)952, "greek small letter theta"),
                    new HtmlCharCode("iota", 953, (char)953, "greek small letter iota"),
                    new HtmlCharCode("kappa", 954, (char)954, "greek small letter kappa"),
                    new HtmlCharCode("lambda", 955, (char)955, "greek small letter lambda"),
                    new HtmlCharCode("mu", 956, (char)956, "greek small letter mu"),
                    new HtmlCharCode("nu", 957, (char)957, "greek small letter nu"),
                    new HtmlCharCode("xi", 958, (char)958, "greek small letter xi"),
                    new HtmlCharCode("omicron", 959, (char)959, "greek small letter omicron"),
                    new HtmlCharCode("pi", 960, (char)960, "greek small letter pi"),
                    new HtmlCharCode("rho", 961, (char)961, "greek small letter rho"),
                    new HtmlCharCode("sigmaf", 962, (char)962, "greek small letter final sigma"),
                    new HtmlCharCode("sigma", 963, (char)963, "greek small letter sigma"),
                    new HtmlCharCode("tau", 964, (char)964, "greek small letter tau"),
                    new HtmlCharCode("upsilon", 965, (char)965, "greek small letter upsilon"),
                    new HtmlCharCode("phi", 966, (char)966, "greek small letter phi"),
                    new HtmlCharCode("chi", 967, (char)967, "greek small letter chi"),
                    new HtmlCharCode("psi", 968, (char)968, "greek small letter psi"),
                    new HtmlCharCode("omega", 969, (char)969, "greek small letter omega"),
                    new HtmlCharCode("thetasym", 977, (char)977, "greek small letter theta symbol"),
                    new HtmlCharCode("upsih", 978, (char)978, "greek upsilon with hook symbol"),
                    new HtmlCharCode("piv", 982, (char)982, "greek pi symbol"),
                    new HtmlCharCode("bull", 8226, (char)8226, "bullet = black small circle"),
                    new HtmlCharCode("hellip", 8230, (char)8230, "horizontal ellipsis = three dot leader"),
                    new HtmlCharCode("prime", 8242, (char)8242, "prime = minutes = feet"),
                    new HtmlCharCode("Prime", 8243, (char)8243, "double prime = seconds = inches"),
                    new HtmlCharCode("oline", 8254, (char)8254, "overline = spacing overscore"),
                    new HtmlCharCode("frasl", 8260, (char)8260, "fraction slash"),
                    new HtmlCharCode("weierp", 8472, (char)8472, "script capital P = power set = Weierstrass p"),
                    new HtmlCharCode("image", 8465, (char)8465, "blackletter capital I = imaginary part"),
                    new HtmlCharCode("real", 8476, (char)8476, "blackletter capital R = real part symbol"),
                    new HtmlCharCode("trade", 8482, (char)8482, "trade mark sign"),
                    new HtmlCharCode("alefsym", 8501, (char)8501, "alef symbol = first transfinite cardinal"),
                    new HtmlCharCode("larr", 8592, (char)8592, "leftwards arrow"),
                    new HtmlCharCode("uarr", 8593, (char)8593, "upwards arrow"),
                    new HtmlCharCode("rarr", 8594, (char)8594, "rightwards arrow"),
                    new HtmlCharCode("darr", 8595, (char)8595, "downwards arrow"),
                    new HtmlCharCode("harr", 8596, (char)8596, "left right arrow"),
                    new HtmlCharCode("crarr", 8629, (char)8629, "downwards arrow with corner leftwards = carriage return"),
                    new HtmlCharCode("lArr", 8656, (char)8656, "leftwards double arrow"),
                    new HtmlCharCode("uArr", 8657, (char)8657, "upwards double arrow"),
                    new HtmlCharCode("rArr", 8658, (char)8658, "rightwards double arrow"),
                    new HtmlCharCode("dArr", 8659, (char)8659, "downwards double arrow"),
                    new HtmlCharCode("hArr", 8660, (char)8660, "left right double arrow"),
                    new HtmlCharCode("forall", 8704, (char)8704, "for all"),
                    new HtmlCharCode("part", 8706, (char)8706, "partial differential"),
                    new HtmlCharCode("exist", 8707, (char)8707, "there exists"),
                    new HtmlCharCode("empty", 8709, (char)8709, "empty set = null set = diameter"),
                    new HtmlCharCode("nabla", 8711, (char)8711, "nabla = backward difference"),
                    new HtmlCharCode("isin", 8712, (char)8712, "element of"),
                    new HtmlCharCode("notin", 8713, (char)8713, "not an element of"),
                    new HtmlCharCode("ni", 8715, (char)8715, "contains as member"),
                    new HtmlCharCode("prod", 8719, (char)8719, "n-ary product = product sign"),
                    new HtmlCharCode("sum", 8721, (char)8721, "n-ary sumation"),
                    new HtmlCharCode("minus", 8722, (char)8722, "minus sign"),
                    new HtmlCharCode("lowast", 8727, (char)8727, "asterisk operator"),
                    new HtmlCharCode("radic", 8730, (char)8730, "square root = radical sign"),
                    new HtmlCharCode("prop", 8733, (char)8733, "proportional to"),
                    new HtmlCharCode("infin", 8734, (char)8734, "infinity"),
                    new HtmlCharCode("ang", 8736, (char)8736, "angle"),
                    new HtmlCharCode("and", 8743, (char)8743, "logical and = wedge"),
                    new HtmlCharCode("or", 8744, (char)8744, "logical or = vee"),
                    new HtmlCharCode("cap", 8745, (char)8745, "intersection = cap"),
                    new HtmlCharCode("cup", 8746, (char)8746, "union = cup"),
                    new HtmlCharCode("int", 8747, (char)8747, "integral"),
                    new HtmlCharCode("there4", 8756, (char)8756, "therefore"),
                    new HtmlCharCode("sim", 8764, (char)8764, "tilde operator = varies with = similar to"),
                    new HtmlCharCode("cong", 8773, (char)8773, "approximately equal to"),
                    new HtmlCharCode("asymp", 8776, (char)8776, "almost equal to = asymptotic to"),
                    new HtmlCharCode("ne", 8800, (char)8800, "not equal to"),
                    new HtmlCharCode("equiv", 8801, (char)8801, "identical to"),
                    new HtmlCharCode("le", 8804, (char)8804, "less-than or equal to"),
                    new HtmlCharCode("ge", 8805, (char)8805, "greater-than or equal to"),
                    new HtmlCharCode("sub", 8834, (char)8834, "subset of"),
                    new HtmlCharCode("sup", 8835, (char)8835, "superset of"),
                    new HtmlCharCode("nsub", 8836, (char)8836, "not a subset of"),
                    new HtmlCharCode("sube", 8838, (char)8838, "subset of or equal to"),
                    new HtmlCharCode("supe", 8839, (char)8839, "superset of or equal to"),
                    new HtmlCharCode("oplus", 8853, (char)8853, "circled plus = direct sum"),
                    new HtmlCharCode("otimes", 8855, (char)8855, "circled times = vector product"),
                    new HtmlCharCode("perp", 8869, (char)8869, "up tack = orthogonal to = perpendicular"),
                    new HtmlCharCode("sdot", 8901, (char)8901, "dot operator"),
                    new HtmlCharCode("lceil", 8968, (char)8968, "left ceiling = apl upstile"),
                    new HtmlCharCode("rceil", 8969, (char)8969, "right ceiling"),
                    new HtmlCharCode("lfloor", 8970, (char)8970, "left floor = apl downstile"),
                    new HtmlCharCode("rfloor", 8971, (char)8971, "right floor"),
                    new HtmlCharCode("lang", 9001, (char)9001, "left-pointing angle bracket = bra"),
                    new HtmlCharCode("rang", 9002, (char)9002, "right-pointing angle bracket = ket"),
                    new HtmlCharCode("loz", 9674, (char)9674, "lozenge"),
                    new HtmlCharCode("spades", 9824, (char)9824, "black spade suit"),
                    new HtmlCharCode("clubs", 9827, (char)9827, "black club suit = shamrock"),
                    new HtmlCharCode("hearts", 9829, (char)9829, "black heart suit = valentine"),
                    new HtmlCharCode("diams", 9830, (char)9830, "black diamond suit"),
                };

            foreach (HtmlCharCode charCode in charCodes)
            {
                _charCodes.Add(charCode.Name, charCode);
            }
        }

        private static Regex _translate1 = new Regex(@"&([a-zA-Z]+)\w*;?", RegexOptions.Compiled);
        private static Regex _translate2 = new Regex(@"&#([0-9]+);", RegexOptions.Compiled);
        private static Regex _translate3 = new Regex(@"&#x([a-fA-F0-9]+);", RegexOptions.Compiled);
        public static string TranslateCode(string value)
        {
            if (value == null)
                return null;
            // &gt;  &aaa;
            int i = 0;
            while (true)
            {
                Match match = _translate1.Match(value, i);
                if (!match.Success) break;
                string name = match.Groups[1].Value;
                HtmlCharCode htmlChar = HtmlCharCodes.GetHtmlChar(name);
                if (htmlChar != null)
                {
                    char c = htmlChar.Char;
                    value = value.Substring(0, match.Index) + c.ToString() + value.Substring(match.Index + match.Length, value.Length - match.Index - match.Length);
                }
                i = match.Index + 1;
            }

            // &#62; &#nnn;
            i = 0;
            while (true)
            {
                Match match = _translate2.Match(value, i);
                if (!match.Success) break;
                string codeString = match.Groups[1].Value;
                int code = int.Parse(codeString);
                char c = (char)code;
                value = value.Substring(0, match.Index) + c.ToString() + value.Substring(match.Index + match.Length, value.Length - match.Index - match.Length);
                i = match.Index + 1;
            }

            // &#xB7; &#xnn;
            i = 0;
            while (true)
            {
                Match match = _translate3.Match(value, i);
                if (!match.Success) break;
                string codeString = match.Groups[1].Value;
                int code = int.Parse(codeString, System.Globalization.NumberStyles.AllowHexSpecifier);
                char c = (char)code;
                value = value.Substring(0, match.Index) + c.ToString() + value.Substring(match.Index + match.Length, value.Length - match.Index - match.Length);
                i = match.Index + 1;
            }

            return value;
        }
    }
}
