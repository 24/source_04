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

    public class MatchDate
    {
        public int Index;
        public Date? Date = null;
        public DateType DateType = DateType.Unknow;
        public MatchValues MatchValues = null;
    }

    public class FindDateManager
    {
        private RegexValuesList _dateRegexList = null;
        private bool _multipleSearch = false;
        //private static Regex __sixDigitDate = new Regex("[0-9]{6}", RegexOptions.Compiled);
        //private static RegexValues __sixDigitDate = new RegexValues("date", "date", "[0-9]{8}", null, null, compileRegex: true);
        private RegexValuesList _digitDateRegexList = null;
        private int _gapDayBefore = 0;
        private int _gapDayAfter = 0;

        public FindDateManager(IEnumerable<XElement> dates, IEnumerable<XElement> digitDates = null, bool compileRegex = false)
        {
            _dateRegexList = new RegexValuesList(dates, compileRegex: compileRegex);
            if (digitDates != null)
                _digitDateRegexList = new RegexValuesList(digitDates, compileRegex: compileRegex);
        }

        public RegexValuesList DateRegexList { get { return _dateRegexList; } }
        public bool MultipleSearch { get { return _multipleSearch; } set { _multipleSearch = value; } }
        public int GapDayBefore { get { return _gapDayBefore; } set { _gapDayBefore = value; } }
        public int GapDayAfter { get { return _gapDayAfter; } set { _gapDayAfter = value; } }

        public FindDate Find(string text, Date? expectedDate = null)
        {
            List<MatchValues> matchValuesList = new List<MatchValues>();
            List<MatchDate> matchDates = new List<MatchDate>();
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
                        if (!_multipleSearch)
                            return new FindDate { found = true, date = date, dateType = dateType, matchValues = matchValues, matchValuesList = matchValuesList.Count > 0 ? matchValuesList.ToArray() : null };
                        else
                            matchDates.Add(new MatchDate { Index = matchValuesList.Count, Date = date, DateType = dateType, MatchValues = matchValues });
                    }
                    matchValuesList.Add(matchValues);
                    matchValues = matchValues.Next();
                }
            }

            MatchDate selectedMatchDate = null;
            foreach (MatchDate matchDate in matchDates)
            {
                if (selectedMatchDate == null || zdate.GetDateTypeOrderNumber(matchDate.DateType) <= zdate.GetDateTypeOrderNumber(selectedMatchDate.DateType))
                {
                    selectedMatchDate = matchDate;
                }
            }

            // try to find a digit date corresponding to expectedDate : 20150820 20082015
            if (selectedMatchDate == null && expectedDate != null && _digitDateRegexList != null)
            {
                //string date1 = ((Date)expectedDate).ToString("yyyyMMdd");
                //string date2 = ((Date)expectedDate).ToString("ddMMyyyy");
                //MatchValues matchValues = __sixDigitDate.Match(text);
                //while (matchValues.Success)
                //{
                //    if (matchValues.Match.Value == date1 || matchValues.Match.Value == date2)
                //    {
                //        selectedMatchDate = new MatchDate { Date = (Date)expectedDate, DateType = DateType.Day, Index = -1, MatchValues = matchValues };
                //        break;
                //    }
                //    matchValues = matchValues.Next();
                //}
                foreach (RegexValues rv in _digitDateRegexList.Values)
                {
                    MatchValues matchValues = rv.Match(text);
                    while (matchValues.Success)
                    {
                        Date date;
                        DateType dateType;
                        //if (zdate.TryCreateDate(matchValues.GetValues(), out date, out dateType) && date == (Date)expectedDate)
                        if (zdate.TryCreateDate(matchValues.GetValues(), out date, out dateType) && IsDateCorrect(date, (Date)expectedDate))
                        {
                            return new FindDate { found = true, date = date, dateType = dateType, matchValues = matchValues, matchValuesList = matchValuesList.Count > 0 ? matchValuesList.ToArray() : null };
                        }
                        matchValues = matchValues.Next();
                    }
                }
            }

            if (selectedMatchDate != null)
            {
                if (selectedMatchDate.Index != -1)
                    matchValuesList.RemoveAt(selectedMatchDate.Index);
                return new FindDate { found = true, date = selectedMatchDate.Date, dateType = selectedMatchDate.DateType, matchValues = selectedMatchDate.MatchValues, matchValuesList = matchValuesList.Count > 0 ? matchValuesList.ToArray() : null };
            }
            else
                return new FindDate { found = false, matchValuesList = matchValuesList.Count > 0 ? matchValuesList.ToArray() : null };
        }

        private bool IsDateCorrect(Date date, Date expectedDate)
        {
            TimeSpan gap = expectedDate - date;
            if (gap.Days < 0)
            {
                if (-gap.Days <= _gapDayAfter)
                    return true;
            }
            else
            {
                if (gap.Days <= _gapDayBefore)
                    return true;
            }
            return false;
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
