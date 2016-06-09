using System.Collections.Generic;
using System.Xml.Linq;
using pb.Data;

namespace pb.Text
{
    public class FindNumber
    {
        public bool found = false;
        public int? number = null;
        public MatchValues matchValues = null;
    }

    //[Obsolete]
    //public class FindNumber_old
    //{
    //    public bool found;
    //    public int? number;
    //    public RegexValues regexValues;
    //}

    public class FindNumberManager
    {
        private RegexValuesList _numberRegexList = null;

        public FindNumberManager(IEnumerable<XElement> xelements, bool compileRegex = false)
        {
            _numberRegexList = new RegexValuesList(xelements, compileRegex: compileRegex);
        }

        public RegexValuesList NumberRegexList { get { return _numberRegexList; } }

        public FindNumber Find(string text)
        {
            foreach (RegexValues rv in _numberRegexList.Values)
            {
                MatchValues matchValues = rv.Match(text);
                if (matchValues.Success)
                {
                    NamedValues<ZValue> values = matchValues.GetValues();
                    if (values.ContainsKey("number"))
                    {
                        int number;
                        ZValue value = values["number"];
                        if (value is ZString)
                        {
                            if (!int.TryParse((string)value, out number))
                            {
                                throw new PBException("error finding number text is'nt a number : \"{0}\"", (string)value);
                            }
                        }
                        else if (value is ZInt)
                        {
                            number = (int)value;
                        }
                        else
                        {
                            throw new PBException("error finding number value is'nt a number : {0}", value);
                        }
                        //return new FindNumber { found = true, number = number, remainText = rv.MatchReplace("_"), regexValues = rv };
                        return new FindNumber { found = true, number = number, matchValues = matchValues };
                    }
                }
            }
            return new FindNumber { found = false };
        }

        //[Obsolete]
        //public FindNumber_old Find_old(string text)
        //{
        //    foreach (RegexValues rv in _numberRegexList.Values)
        //    {
        //        rv.Match_old(text);
        //        if (rv.Success_old)
        //        {
        //            NamedValues<ZValue> values = rv.GetValues_old();
        //            if (values.ContainsKey("number"))
        //            {
        //                int number;
        //                ZValue value = values["number"];
        //                if (value is ZString)
        //                {
        //                    if (!int.TryParse((string)value, out number))
        //                    {
        //                        throw new PBException("error finding number text is'nt a number : \"{0}\"", (string)value);
        //                    }
        //                }
        //                else if (value is ZInt)
        //                {
        //                    number = (int)value;
        //                }
        //                else
        //                {
        //                    throw new PBException("error finding number value is'nt a number : {0}", value);
        //                }
        //                //return new FindNumber { found = true, number = number, remainText = rv.MatchReplace("_"), regexValues = rv };
        //                return new FindNumber_old { found = true, number = number, regexValues = rv };
        //            }
        //        }
        //    }
        //    return new FindNumber_old { found = false, number = null, regexValues = null };
        //}
    }
}
