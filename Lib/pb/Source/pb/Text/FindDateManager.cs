using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace pb.Text
{
    public class FindDate
    {
        public bool found = false;
        public Date? date = null;
        public DateType dateType = DateType.Unknow;
        public MatchValues matchValues = null;
        public MatchValues[] matchValuesList = null;
    }

    public class FindDateManager
    {
        private RegexValuesList _dateRegexList = null;

        public FindDateManager(IEnumerable<XElement> xelements, bool compileRegex = false)
        {
            _dateRegexList = new RegexValuesList(xelements, compileRegex: compileRegex);
        }

        public RegexValuesList DateRegexList { get { return _dateRegexList; } }

        public FindDate Find(string text)
        {
            List<MatchValues> matchValuesList = new List<MatchValues>();
            foreach (RegexValues rv in _dateRegexList.Values)
            {
                MatchValues matchValues = rv.Match(text);
                while (matchValues.Success)
                {
                    //matchValuesList.Add(matchValues);
                    //Trace.WriteLine("date capture \"{0}\"", matchValues.Match.Value);
                    Date date;
                    DateType dateType;
                    if (zdate.TryCreateDate(matchValues.GetValues(), out date, out dateType))
                    {
                        return new FindDate { found = true, date = date, dateType = dateType, matchValues = matchValues, matchValuesList = matchValuesList.Count > 0 ? matchValuesList.ToArray() : null };
                    }
                    matchValuesList.Add(matchValues);
                    matchValues = matchValues.Next();
                }
            }
            return new FindDate { found = false, matchValuesList = matchValuesList.Count > 0 ? matchValuesList.ToArray() : null };
        }
    }

    public class FindDateManager_v1
    {
        private RegexValuesList _dateRegexList = null;

        public FindDateManager_v1(IEnumerable<XElement> xelements, bool compileRegex = false)
        {
            _dateRegexList = new RegexValuesList(xelements, compileRegex: compileRegex);
        }

        public RegexValuesList DateRegexList { get { return _dateRegexList; } }

        public FindDate Find(string text)
        {
            foreach (RegexValues rv in _dateRegexList.Values)
            {
                MatchValues matchValues = rv.Match(text);
                if (matchValues.Success)
                {
                    Date date;
                    DateType dateType;
                    if (zdate.TryCreateDate(matchValues.GetValues(), out date, out dateType))
                    {
                        //return new FindDate { found = true, date = date, dateType = dateType, remainText = rv.MatchReplace("_"), regexValues = rv };
                        //return new FindDate { found = true, date = date, dateType = dateType, matchValues = matchValues, matchValuesList = new MatchValues[] { matchValues } };
                        return new FindDate { found = true, date = date, dateType = dateType, matchValues = matchValues, matchValuesList = null };
                    }
                }
            }
            return new FindDate { found = false };
        }
    }
}
