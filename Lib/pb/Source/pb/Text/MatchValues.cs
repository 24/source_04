using pb.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace pb.Text
{
    public class MatchValues
    {
        private RegexValues _regexValues = null;
        private Match _match = null;
        private string _input = null;

        public MatchValues(RegexValues regexValues, string input, Match match)
        {
            _regexValues = regexValues;
            _input = input;
            _match = match;
        }

        public string Key { get { return _regexValues.Key; } }
        public string Name { get { return _regexValues.Name; } }
        public string Pattern { get { return _regexValues.Pattern; } }
        public Dictionary<string, string> Attributes { get { return _regexValues.Attributes; } }
        public bool Success { get { if (_match == null) return false; else return _match.Success; } }
        public Match Match { get { return _match; } }
        public string Input { get { return _input; } }

        //public MatchValues Next()
        public bool Next()
        {
            //if (_match == null)
            //    return null;
            //return new MatchValues(_regexValues, _input, _match.NextMatch());
            if (_match != null)
            {
                _match = _match.NextMatch();
                return _match.Success;
            }
            else
                return false;
        }

        public string Replace(string replace)
        {
            if (_match == null)
                throw new PBException("can't replace, no match found");
            return _match.zReplace(_input, replace);
        }

        public MatchValuesInfos GetValuesInfos()
        {
            return new MatchValuesInfos { Name = _regexValues.Name, Values = GetValues(), Attributes = _regexValues.Attributes, MatchInfo = new MatchInfo(_match) };
        }

        public NamedValues<ZValue> GetValues()
        {
            if (_match == null)
                throw new PBException("can't get values, no match found");
            NamedValues<ZValue> values = new NamedValues<ZValue>();
            if (_match.Success)
            {
                foreach (RegexValueDefinition value in _regexValues.NamedValuesDefinitions.Values)
                {
                    values.Add(value.Name, value.GetValue(_match));
                }
            }
            return values;
        }

        public NamedValues<RegexValue<ZValue>> GetRegexValues()
        {
            if (_match == null)
                throw new PBException("can't get values, no match found");
            NamedValues<RegexValue<ZValue>> values = new NamedValues<RegexValue<ZValue>>();
            if (_match.Success)
            {
                foreach (RegexValueDefinition value in _regexValues.NamedValuesDefinitions.Values)
                {
                    values.Add(value.Name, value.GetRegexValue(_match));
                }
            }
            return values;
        }

        public NamedValues<ZValue> GetAllValues()
        {
            if (_match == null)
                throw new PBException("can't get all values, no match found");
            NamedValues<ZValue> values = new NamedValues<ZValue>();
            if (_match.Success)
            {
                foreach (RegexValueDefinition value in _regexValues.NamedValuesDefinitions.Values)
                {
                    values.Add(value.Name, value.GetAllValues(_match));
                }
            }
            return values;
        }

        public NamedValues<RegexValue<ZValue>> GetAllRegexValues()
        {
            if (_match == null)
                throw new PBException("can't get all values, no match found");
            NamedValues<RegexValue<ZValue>> values = new NamedValues<RegexValue<ZValue>>();
            if (_match.Success)
            {
                foreach (RegexValueDefinition value in _regexValues.NamedValuesDefinitions.Values)
                {
                    values.Add(value.Name, value.GetAllRegexValues(_match));
                }
            }
            return values;
        }

        public string GetAttribute(string attribute)
        {
            if (_regexValues != null && _regexValues.Attributes.ContainsKey(attribute))
                return _regexValues.Attributes[attribute];
            else
                return null;
        }
    }
}
