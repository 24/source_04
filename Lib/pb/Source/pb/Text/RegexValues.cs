using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb.Data;
using pb.Data.Xml;

// problem MatchValues.GetAllValues()
// 

// remplacement de NamedValues1 par NamedValues<RegexValue> donc object devient RegexValue le 06/10/2013

namespace pb.Text
{
    public class MatchValuesInfos
    {
        public string Name;
        public NamedValues<ZValue> Values;
        public Dictionary<string, string> Attributes;
        public MatchInfo MatchInfo;
    }

    public class MatchInfo
    {
        public bool Success;
        public string Capture;
        public int Index;
        public int Length;
        public MatchInfo(Match match)
        {
            Success = match.Success;
            if (Success)
            {
                Index = match.Index;
                Length = match.Length;
                Capture = match.Value;
            }
        }
    }

    //[Obsolete]
    //public class FindText_old
    //{
    //    public bool found;
    //    public string text;
    //    public RegexValues regexValues;
    //}

    public enum RegexValueType
    {
        String = 1,
        StringArray,
        StringFirstValue,
        StringLastValue
    }

    //public enum RegexValueCaptureType
    //{
    //    FirstCapture = 1,
    //    LastCapture
    //}

    public class RegexValue<T>
    {
        public T Value;
        public int Index;
        public int Length;
    }

    public class RegexValueDefinition
    {
        public string Name;
        public RegexValueType Type;
        public int Index;

        public ZValue GetValue(Match match)
        {
            return GetValue(match.Groups[Index + 1]);
        }

        public RegexValue<ZValue> GetRegexValue(Match match)
        {
            Group group = match.Groups[Index + 1];
            return new RegexValue<ZValue> { Value = GetValue(group), Index = group.Index, Length = group.Length };
        }

        private ZValue GetValue(Group group)
        {
            switch (Type)
            {
                case RegexValueType.StringArray:
                    string[] values = new string[group.Captures.Count];
                    for (int i = 0; i < group.Captures.Count; i++)
                    {
                        if (group.Captures[i].Value != "")
                            values[i] = group.Captures[i].Value;
                    }
                    return new ZStringArray(values);
                case RegexValueType.StringFirstValue:
                    foreach (Capture capture in group.Captures)
                    {
                        if (capture.Value != null && capture.Value != "")
                            return new ZString(capture.Value);
                    }
                    return null;
                case RegexValueType.StringLastValue:
                    for (int i = group.Captures.Count - 1; i >= 0; i--)
                    {
                        string value = group.Captures[i].Value;
                        if (value != null && value != "")
                            return new ZString(value);
                    }
                    return null;
                default: // type == RegexValueType.String
                    if (group.Value != "") 
                        return new ZString(group.Value);
                    else
                        return null;
            }
        }

        public ZValue GetAllValues(Match match)
        {
            //Group group = match.Groups[Index + 1];
            return GetAllValues(match.Groups[Index + 1]);
        }

        public RegexValue<ZValue> GetAllRegexValues(Match match)
        {
            Group group = match.Groups[Index + 1];
            return new RegexValue<ZValue> { Value = GetAllValues(group), Index = group.Index, Length = group.Length };
        }

        private ZValue GetAllValues(Group group)
        {
            if (group.Captures.Count == 0)
                return null;
            else if (group.Captures.Count == 1)
                return new ZString(group.Value);
            else
            {
                string[] values = new string[group.Captures.Count];
                for (int i = 0; i < group.Captures.Count; i++)
                {
                    if (group.Captures[i].Value != "")
                        values[i] = group.Captures[i].Value;
                }
                return new ZStringArray(values);
            }
        }
    }

    // used to generate a RegexValues from model (ex: class Print and pt_config.xml)
    public class RegexValuesModel
    {
        private string _key;
        private string _name = null;
        private string _pattern = null;
        private string _options = null;
        private string _values = null;

        public RegexValuesModel(XElement xe)
        {
            _name = xe.zAttribValue("name");
            _key = xe.zAttribValue("key", _name);
            _pattern = xe.zAttribValue("regex");
            _options = xe.zAttribValue("options");
            _values = xe.zAttribValue("values");
        }

        public string key { get { return _key; } }
        public string name { get { return _name; } }
        public string pattern { get { return _pattern; } }
        public string options { get { return _options; } }
        public string values { get { return _values; } }
    }

    // Regex with Pattern property and creation from xml
    public class XRegex : Regex
    {
        public XRegex(XElement xe, bool compileRegex = false)
            : base(xe.zAttribValue("regex"), GetRegexOptions(xe.zAttribValue("options"), compileRegex))
        {
        }

        public XRegex(string pattern, string options, bool compileRegex = false)
            : base(pattern, GetRegexOptions(options, compileRegex))
        {
        }

        public string Pattern { get { return pattern; } }

        public static RegexOptions GetRegexOptions(string s, bool compileRegex = false)
        {
            if (s == null)
                return RegexOptions.None;
            RegexOptions options = 0;
            foreach (string s2 in zsplit.Split(s, ',', true))
                options |= GetRegexOption(s2);
            if (compileRegex)
                options |= RegexOptions.Compiled;
            return options;
        }

        public static RegexOptions GetRegexOption(string s)
        {
            if (s == null)
                return RegexOptions.None;
            switch (s.ToLower())
            {
                case "cultureinvariant":
                    return RegexOptions.CultureInvariant;
                case "ecmascript":
                    return RegexOptions.ECMAScript;
                case "explicitcapture":
                    return RegexOptions.ExplicitCapture;
                case "ignorecase":
                    return RegexOptions.IgnoreCase;
                case "ignorepatternwhitespace":
                    return RegexOptions.IgnorePatternWhitespace;
                case "multiline":
                    return RegexOptions.Multiline;
                case "righttoleft":
                    return RegexOptions.RightToLeft;
                case "singleline":
                    return RegexOptions.Singleline;
                default:
                    throw new PBException("error unknow regex option \"{0}\"", s);
            }
        }
    }

    public class RegexCaptureValues
    {
        public string capture;
        public NamedValues<ZValue> values;

        public static RegexCaptureValues CreateRegexCaptureValues(MatchValues matchValues, bool allValues = false)
        {
            if (matchValues != null && matchValues.Success)
            {
                if (allValues)
                    return new RegexCaptureValues { capture = matchValues.Match.Value, values = matchValues.GetAllValues() };
                else
                    return new RegexCaptureValues { capture = matchValues.Match.Value, values = matchValues.GetValues() };
            }
            else
                return null;
        }

        public static RegexCaptureValues[] CreateRegexCaptureValuesList(MatchValues[] matchValuesList, bool allValues = false)
        {
            if (matchValuesList == null)
                return null;
            List<RegexCaptureValues> captureValuesList = new List<RegexCaptureValues>();
            foreach (MatchValues matchValues in matchValuesList)
                captureValuesList.Add(CreateRegexCaptureValues(matchValues, allValues));
            return captureValuesList.ToArray();
        }
    }

    public class RegexValues : XRegex
    {
        private string _key;
        private string _name = null;
        private Dictionary<string, RegexValueDefinition> _namedValuesDefinitions = null;
        private Dictionary<string, string> _attributes = new Dictionary<string, string>();  // constant named string other then "name", "key", "values", "regex", "options"
        //private Match _match = null;            // obsolete transfered to MatchValues
        //private string _input = null;           // obsolete transfered to MatchValues

        public RegexValues(XElement xe, bool compileRegex = false)
            : base(xe, compileRegex)
        {
            _name = xe.zAttribValue("name");
            _key = xe.zAttribValue("key", _name);
            InitValuesNames(xe.zAttribValue("values"));
            ReadAttributes(xe);
        }

        public RegexValues(string key, string name, string pattern, string options, string values, bool compileRegex = false)
            : base(pattern, options, compileRegex)
        {
            _key = key;
            _name = name;
            InitValuesNames(values);
        }

        private void InitValuesNames(string names)
        {
            _namedValuesDefinitions = new Dictionary<string, RegexValueDefinition>();
            if (names == null)
                return;
            int i = 0;
            foreach (string name in zsplit.Split(names, ',', true))
            {
                RegexValueDefinition def = new RegexValueDefinition();
                def.Index = i++;
                def.Name = name;
                def.Type = RegexValueType.String;
                if (name.EndsWith("[]"))
                {
                    def.Name = name.Substring(0, name.Length - 2).TrimEnd();
                    def.Type = RegexValueType.StringArray;
                }
                else if (name.EndsWith("[first]"))
                {
                    def.Name = name.Substring(0, name.Length - 7).TrimEnd();
                    def.Type = RegexValueType.StringFirstValue;
                }
                else if (name.EndsWith("[last]"))
                {
                    def.Name = name.Substring(0, name.Length - 6).TrimEnd();
                    def.Type = RegexValueType.StringLastValue;
                }
                if (def.Name != "")
                    _namedValuesDefinitions.Add(def.Name, def);
            }
        }

        private void ReadAttributes(XElement xe)
        {
            foreach (XAttribute xa in xe.Attributes())
            {
                switch (xa.Name.LocalName)
                {
                    case "name":
                    case "key":
                    case "values":
                    case "regex":
                    case "options":
                        break;
                    default:
                        _attributes.Add(xa.Name.LocalName, xa.Value);
                        break;
                }
            }
        }

        ////[Obsolete] ZValue this[string name]
        //[Obsolete]
        //public ZValue this[string name]
        //{
        //    get
        //    {
        //        if (_valuesNames.ContainsKey(name))
        //            return _valuesNames[name].GetValue(_match);
        //        else
        //            throw new PBException("error unknow value \"{0}\"", name);
        //    }
        //}

        public string Key { get { return _key; } set { _key = value; } }
        public string Name { get { return _name; } }
        public Dictionary<string, RegexValueDefinition> NamedValuesDefinitions { get { return _namedValuesDefinitions; } }
        //[Obsolete]
        //public bool Success_old { get { if (_match == null) return false; else return _match.Success; } }   // obsolete transfered to MatchValues
        //[Obsolete]
        //public Match MatchValue_old { get { return _match; } }   // obsolete transfered to MatchValues
        //[Obsolete]
        //public string MatchInput_old { get { return _input; } }   // obsolete transfered to MatchValues
        public Dictionary<string, string> Attributes { get { return _attributes; } }

        //[Obsolete]
        //public Match Match_old(string input)   // obsolete transfered to MatchValues
        //{
        //    _input = input;
        //    _match = base.Match(input);
        //    return _match;
        //}

        public new MatchValues Match(string input, int startat = 0)   // obsolete transfered to MatchValues
        {
            return new MatchValues(this, input, base.Match(input, startat));
        }

        //[Obsolete]
        //public Match Next_old()   // obsolete transfered to MatchValues
        //{
        //    if (_match == null)
        //        return null;
        //    _match = _match.NextMatch();
        //    return _match;
        //}

        //[Obsolete]
        //public string MatchReplace_old(string replace)   // obsolete transfered to MatchValues
        //{
        //    return _match.zReplace(_input, replace);
        //}

        //[Obsolete]
        //public NamedValues<ZValue> GetValues_old()   // obsolete transfered to MatchValues
        //{
        //    if (_match == null)
        //        throw new PBException("error you need to call Match() before GetValues()");
        //    NamedValues<ZValue> values = new NamedValues<ZValue>();
        //    if (_match.Success)
        //    {
        //        foreach (RegexValueDefinition value in _namedValuesDefinitions.Values)
        //        {
        //            values.Add(value.name, value.GetValue(_match));
        //        }
        //    }
        //    return values;
        //}
    }

    public static partial class GlobalExtension
    {
        public static void zTrace(this Dictionary<string, ZValue> values)
        {
            if (values.Count == 0)
                Trace.WriteLine("no value");
            else
            {
                bool first = true;
                foreach (KeyValuePair<string, ZValue> value in values)
                {
                    if (!first)
                        Trace.Write(", ");
                    first = false;
                    Trace.Write("{0}={1}", value.Key, value.Value);
                }
            }
        }
    }
}
