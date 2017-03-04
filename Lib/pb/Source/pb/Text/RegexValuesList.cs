using pb.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace pb.Text
{
    public class FindText_v2
    {
        private MatchValues _matchValues = null;
        private string _text = null;
        private int _matchIndex = 0;
        private int _matchLength = 0;
        private RegexValuesList _regexValuesList = null;

        //public FindText_v2()
        //{
        //}

        public FindText_v2(MatchValues matchValues, RegexValuesList regexValuesList)
        {
            _regexValuesList = regexValuesList;
            SetMatchValues(matchValues);
        }

        public string Text { get { return _text; } }
        public int MatchIndex { get { return _matchIndex; } }
        public int MatchLength { get { return _matchLength; } }
        public string Key { get { return _matchValues?.Key; } }
        public string Name { get { return _matchValues?.Name; } }
        public string Pattern { get { return _matchValues?.Pattern; } }
        public Dictionary<string, string> Attributes { get { return _matchValues?.Attributes; } }
        public bool Success { get { if (_matchValues == null) return false; else return _matchValues.Success; } }
        public Match Match { get { return _matchValues.Match; } }
        public string Input { get { return _matchValues?.Input; } }

        public bool Next()
        {
            if (_matchValues != null)
                return _matchValues.Next();
            else
                return false;
        }

        public bool FindNext(bool contiguous = false)
        {
            if (_matchValues != null && _regexValuesList != null)
            {
                //_matchValues = _regexValuesList.FindNext(_matchValues.Input, _matchValues.Match.Index + _matchValues.Match.Length, contiguous);
                //if (_matchValues != null)
                //{
                //    _text = _matchValues.Match.Value;
                //    return _matchValues.Success;
                //}
                SetMatchValues(_regexValuesList.FindNext(_matchValues.Input, _matchIndex + _matchLength, contiguous));
                return Success;
            }
            return false;
        }

        public string Replace(string replace)
        {
            if (_matchValues == null)
                throw new PBException("can't replace, no match found");
            return _matchValues.Replace(replace);
        }

        public NamedValues<ZValue> GetValues()
        {
            if (_matchValues == null)
                throw new PBException("can't get values, no match found");
            return _matchValues.GetValues();
        }

        public NamedValues<RegexValue<ZValue>> GetRegexValues()
        {
            if (_matchValues == null)
                throw new PBException("can't get values, no match found");
            return _matchValues.GetRegexValues();
        }

        public NamedValues<ZValue> GetAllValues()
        {
            if (_matchValues == null)
                throw new PBException("can't get all values, no match found");
            return _matchValues.GetAllValues();
        }

        public NamedValues<RegexValue<ZValue>> GetAllRegexValues()
        {
            if (_matchValues == null)
                throw new PBException("can't get all values, no match found");
            return _matchValues.GetAllRegexValues();
        }

        public string GetAttribute(string attribute)
        {
            if (_matchValues == null)
                throw new PBException("can't get attribute, no match found");
            return _matchValues.GetAttribute(attribute);
        }

        private void SetMatchValues(MatchValues matchValues)
        {
            _matchValues = matchValues;
            // test matchValues is null to keep _text, _matchIndex, _matchLength from last match
            if (matchValues != null)
            {
                _text = matchValues.Match.Value;
                _matchIndex = matchValues.Match.Index;
                _matchLength = matchValues.Match.Length;
            }
        }
    }

    //public class FindText
    //{
    //    public bool Found = false;
    //    public string Text = null;
    //    public MatchValues matchValues = null;
    //    //public MatchValuesInfos MatchValues = null;
    //}

    public class RegexValuesList : Dictionary<string, RegexValues>
    {
        private int _lastKeyNumber = 0;

        public RegexValuesList()
        {
        }

        public RegexValuesList(IEnumerable<XElement> xelements, bool compileRegex = false, bool renameDuplicateKey = true)
        {
            Add(xelements, compileRegex, renameDuplicateKey);
        }

        public void Add(IEnumerable<XElement> xelements, bool compileRegex = false, bool renameDuplicateKey = true)
        {
            foreach (XElement xe in xelements)
            {
                RegexValues rv = new RegexValues(xe, compileRegex);
                string key = rv.Key;
                if (key == null)
                {
                    key = "key" + (++_lastKeyNumber).ToString();
                    rv.Key = key;
                }
                if (renameDuplicateKey)
                {
                    string key0 = key;
                    int i = 1;
                    while (ContainsKey(key))
                    {
                        key = key0 + "_" + i++.ToString();
                    }
                    rv.Key = key;
                }
                Add(key, rv);
            }
        }

        //public FindText Find(string text)
        public FindText_v2 Find(string text, bool contiguous = false)
        {
            //foreach (RegexValues rv in this.Values)
            //{
            //    MatchValues matchValues = rv.Match(text);
            //    if (matchValues.Success)
            //    {
            //        //return new FindText { Found = true, Text = matchValues.Match.Value, matchValues = matchValues };
            //        //return new FindText { Found = true, Text = matchValues.Match.Value, MatchValues = matchValues.GetValuesInfos() };
            //        return new FindText_v2(matchValues);
            //    }
            //}
            ////return new FindText { Found = false };
            //return new FindText_v2();
            return new FindText_v2(FindNext(text, 0, contiguous), this);
        }

        public MatchValues FindNext(string text, int startat, bool contiguous = false)
        {
            foreach (RegexValues rv in this.Values)
            {
                MatchValues matchValues = rv.Match(text, startat);
                if (matchValues.Success)
                {
                    if (!contiguous || matchValues.Match.Index == startat)
                        return matchValues;
                }
            }
            return null;
        }

        public RegexValues[] GetRegexValuesListByName(string name)
        {
            List<RegexValues> list = new List<RegexValues>();
            foreach (RegexValues rv in this.Values)
            {
                if (rv.Name == name)
                    list.Add(rv);
            }
            return list.ToArray();
        }

        public NamedValues<ZValue> ExtractTextValues(ref string text)
        {
            NamedValues<ZValue> values = new NamedValues<ZValue>();
            foreach (RegexValues rv in this.Values)
            {
                MatchValues matchValues = rv.Match(text);
                if (matchValues.Success)
                {
                    values.SetValues(matchValues.GetValues());
                    text = matchValues.Replace("");
                }
            }
            return values;
        }
    }
}
