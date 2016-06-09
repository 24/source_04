using pb.Text;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Download.Print
{
    public class FindDay
    {
        private bool _found = false;
        private int? _day = null;
        private MatchValues _matchValues = null;

        private IEnumerator<RegexValues> _enumerator = null;
        private string _text = null;

        public bool Found { get { return _found; } }
        public int? Day { get { return _day; } }
        public MatchValues MatchValues { get { return _matchValues; } }

        public bool FindNext()
        {
            _found = false;
            _day = null;
            _matchValues = null;
            while (_enumerator.MoveNext())
            {
                RegexValues rv = _enumerator.Current;
                MatchValues matchValues = rv.Match(_text);
                if (matchValues.Success)
                {
                    int day;
                    if (zDay.TryGetDay(matchValues.GetValues(), out day))
                    {
                        //return new FindDay { Found = true, Day = day, MatchValues = matchValues };
                        _found = true;
                        _day = day;
                        _matchValues = matchValues;
                        return true;
                    }
                }
            }
            //return new FindDay { Found = false, Day = null, MatchValues = null };
            return false;
        }

        public static FindDay Find(FindDayManager findDayManager, string text)
        {
            FindDay findDay = new FindDay();
            findDay._enumerator = findDayManager.DayRegexList.Values.GetEnumerator();
            findDay._text = text;
            findDay.FindNext();
            return findDay;
        }
    }

    public class FindDayManager
    {
        private RegexValuesList _dayRegexList = null;

        public FindDayManager(IEnumerable<XElement> days, bool compileRegex = false)
        {
            _dayRegexList = new RegexValuesList(days, compileRegex: compileRegex);
        }

        public RegexValuesList DayRegexList { get { return _dayRegexList; } }

        public FindDay Find(string text)
        {
            //foreach (RegexValues rv in _dayRegexList.Values)
            //{
            //    MatchValues matchValues = rv.Match(text);
            //    if (matchValues.Success)
            //    {
            //        int day;
            //        if (zDay.TryGetDay(matchValues.GetValues(), out day))
            //            return new FindDay { Found = true, Day = day, MatchValues = matchValues };
            //    }
            //}
            //return new FindDay { Found = false, Day = null, MatchValues = null };
            return FindDay.Find(this, text);
        }
    }
}
