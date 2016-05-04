using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Text;

//namespace Print
namespace Download.Print
{
    public class Print1
    {
        protected static bool _trace = false;
        protected string _name;
        protected string _title;
        protected PrintFrequency _frequency;
        protected DayOfWeek? _weekday = null;
        protected string _directory;
        protected bool _noNumber = false;
        protected bool _noDateAndNumberCalculate = false;
        protected bool _specialNoDate = false;
        protected Date _refPrintDate;
        protected int _refPrintNumber;
        protected SpecialDay _noPrintDays;
        protected Month _noPrintMonths;
        protected Dictionary<int, int> _noPrintDates;
        protected Dictionary<int, int> _noPrintNumbers = new Dictionary<int,int>();
        protected Date? _date = null;
        protected int _printNumber = 0;
        protected bool _special = false;  // hors-série
        protected bool _specialMonth = false;  // hors-série
        protected string _label = null;
        protected int _index = 0;
        //NamedValues1
        protected NamedValues<ZValue> _printValues = new NamedValues<ZValue>();
        protected Date? _lastDate = null;
        protected int _lastPrintNumber = 0;
        //protected RegexValues _normalizedFilename = null;
        //protected RegexValues _normalizedSpecialFilename = null;

        public Print1(string name, string title, PrintFrequency frequency, DayOfWeek? weekday, string directory, Date refPrintDate, int refPrintNumber, SpecialDay noPrintDays)
        {
            _name = name;
            _title = title;
            _frequency = frequency;
            _weekday = weekday;
            _directory = directory;
            _refPrintDate = refPrintDate;
            _refPrintNumber = refPrintNumber;
            _noPrintDays = noPrintDays;
        }

        public Print1(XElement xe, string baseDirectory, Dictionary<string, RegexValuesModel> regexModels = null)
        {
            _name = xe.zXPathValue("Name");
            _title = xe.zXPathValue("Title");
            _frequency = GetFrequency(xe.zXPathValue("Frequency"));
            if (_frequency == PrintFrequency.Weekly || _frequency == PrintFrequency.EveryTwoWeek)
                _weekday = zdate.GetDayOfWeek(xe.zXPathValue("Weekday"));
            _directory = xe.zXPathValue("Directory").zRootPath(baseDirectory);
            //if (!Path.IsPathRooted(_directory))
            //    _directory = Path.Combine(baseDirectory, _directory);
            SetOption(xe.zXPathValue("Option"));
            if (!_noDateAndNumberCalculate)
            {
                string dateRef = xe.zXPathValue("DateReference");
                string numberRef = xe.zXPathValue("NumberReference");
                if (dateRef == null && numberRef != null)
                    throw new PBException("error missing reference number \"{0}\"", _name);
                if (dateRef != null && numberRef == null)
                    throw new PBException("error missing reference date \"{0}\"", _name);
                if (dateRef == null)
                    _noDateAndNumberCalculate = true;
                else
                {
                    _refPrintDate = Date.Parse(dateRef);
                    _refPrintNumber = int.Parse(numberRef);
                    GetNoPrintDays(xe.zXPathValues("NoPrintDays/NoPrintDay"));
                    GetNoPrintMonths(xe.zXPathValues("NoPrintMonths/NoPrintMonth"));
                }
            }
            //if (regexModels != null)
            //{
            //    string model;
            //    if (_frequency == PrintFrequency.Daily || _frequency == PrintFrequency.Weekly || _frequency == PrintFrequency.EveryTwoWeek)
            //    {
            //        if (!_noNumber)
            //            model = "name_day_number";
            //        else
            //            model = "name_day";
            //    }
            //    else
            //        model = "name_month_number";
            //    RegexValuesModel rvm = regexModels[model];
            //    Dictionary<string, string> textValues = new Dictionary<string, string>();
            //    textValues.Add("v_title", _title);
            //    XElement xe2 = xe.zXPathElement("NormalizedFilename");
            //    if (xe2 != null)
            //        (from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa).zAttribs(textValues);
            //    string pattern = rvm.pattern.zSetTextValues(textValues, true);
            //    string values = rvm.values.zSetTextValues(textValues, true);
            //    _normalizedFilename = new RegexValues(rvm.key, rvm.name, pattern, rvm.options, values);

            //    if (_specialNoDate)
            //        model = "special_name_number";
            //    else
            //        model = "special_name_month_number";
            //    rvm = regexModels[model];
            //    textValues = new Dictionary<string, string>();
            //    textValues.Add("v_title", _title);
            //    xe2 = xe.zXPathElement("NormalizedSpecialFilename");
            //    if (xe2 != null)
            //        (from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa).zAttribs(textValues);
            //    pattern = rvm.pattern.zSetTextValues(textValues, true);
            //    values = rvm.values.zSetTextValues(textValues, true);
            //    _normalizedSpecialFilename = new RegexValues(rvm.key, rvm.name, pattern, rvm.options, values);
            //}
        }

        private void SetOption(string option)
        {
            if (option == null)
                return;
            switch (option.ToLower())
            {
                case "nonumber":
                    _noNumber = true;
                    _noDateAndNumberCalculate = true;
                    break;
                case "specialnodate":
                    _specialNoDate = true;
                    break;
            }
        }

        //private void GetNoPrintDays(string[] noPrintDays)
        private void GetNoPrintDays(IEnumerable<string> noPrintDays)
        {
            _noPrintDays = 0;
            _noPrintDates = new Dictionary<int, int>();
            foreach (string noPrintDay in noPrintDays)
            {
                Date date;
                if (Date.TryParseExact(noPrintDay, "yyyy-MM-dd", CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    int d = date.AbsoluteDay;
                    if (!_noPrintDates.ContainsKey(d))
                        _noPrintDates.Add(d, d);
                }
                else
                    _noPrintDays |= SpecialDayTools.GetSpecialDay(noPrintDay);
            }
        }

        //private void GetNoPrintMonths(string[] noPrintMonths)
        private void GetNoPrintMonths(IEnumerable<string> noPrintMonths)
        {
            foreach (string noPrintMonth in noPrintMonths)
                _noPrintMonths |= zdate.GetMonth(noPrintMonth);
        }

        public string Name { get { return _name; } }
        public string Title { get { return _title; } }
        public PrintFrequency Frequency { get { return _frequency; } }
        public DayOfWeek? Weekday { get { return _weekday; } }
        public string Directory { get { return _directory; } }
        public bool NoNumber { get { return _noNumber; } }
        public bool NoDateAndNumberCalculate { get { return _noDateAndNumberCalculate; } }
        public bool SpecialNoDate { get { return _specialNoDate; } }
        public Date RefPrintDate { get { return _refPrintDate; } }
        public int RefPrintNumber { get { return _refPrintNumber; } }
        public SpecialDay NoPrintDays { get { return _noPrintDays; } }
        public Dictionary<int, int> NoPrintDates { get { return _noPrintDates; } }
        public Date Date { get { return GetPrintDate(); } }
        public int PrintNumber { get { return GetPrintNumber(); } }
        public bool Special { get { return _special; } }
        public bool SpecialMonth { get { return _specialMonth; } }
        public string Label { get { return _label; } }
        public int Index { get { return _index; } }
        //NamedValues1
        public NamedValues<ZValue> PrintValues { get { return _printValues; } }
        //public RegexValues NormalizedFilename { get { return _normalizedFilename; } }
        //public RegexValues NormalizedSpecialFilename { get { return _normalizedSpecialFilename; } }

        //public string GetName() { return _name; }

        //public virtual bool IsMatchFilename(string filename)
        //{
        //    if (_regexPrint == null)
        //        return false;
        //    return _regexPrint.IsMatch(filename);
        //}

        //public virtual Match MatchFilename(string filename)
        //{
        //    if (_regexPrint == null)
        //        return null;
        //    return _regexPrint.Match(filename);
        //}

        public virtual void NewIssue()
        {
            //NamedValues1
            _printValues = new NamedValues<ZValue>();
            _date = null;
            _printNumber = 0;
            _special = false;
            _specialMonth = false;
            _label = null;
            _index = 0;
        }

        //public virtual void SetPrintDate(Date date)
        public virtual void NewIssue(Date date)
        {
            NewIssue();
            _date = date;
            _printNumber = 0;
        }

        //public virtual void SetPrintNumber(int printNumber)
        public virtual void NewIssue(int printNumber)
        {
            NewIssue();
            _printNumber = printNumber;
        }

        //NamedValues1
        public virtual bool TrySetValues(NamedValues<ZValue> values)
        {
            //_special
            //NewIssue();
            //if (values.ContainsKey("special_month"))
            //{
            //    _special_month = true;
            //    _date = date.CreateDate(values);
            //    _printNumber = GetPrintNumber(values);
            //}
            //else
            //{
            //    if (values.ContainsKey("day_near_current_date") || values.ContainsKey("year") || values.ContainsKey("month") || values.ContainsKey("day"))
            //        _date = date.CreateDate(values);
            //    if (values.ContainsKey("number"))
            //    {
            //        _printNumber = GetPrintNumber(values);
            //    }
            //}
            //_printValues = values;
            if (values.ContainsKey("special"))
                _special = true;
            if (values.ContainsKey("special_month"))
                _specialMonth = true;
            //bool setDate = false;
            //if (values.ContainsKey("day_near_current_date") || values.ContainsKey("year") || values.ContainsKey("month") || values.ContainsKey("day"))
            if (values.ContainsKey("day_near_current_date") || values.ContainsKey("month"))
            {
                //_date = date.CreateDate(values);
                //setDate = true;
                Date date;
                DateType dateType;
                if (!zdate.TryCreateDate(values, out date, out dateType))
                    return false;
                _date = date;
            }
            if (values.ContainsKey("number"))
            {
                //if (!setDate || _noDateAndNumberCalculate)
                //_printNumber = GetPrintNumber(values);
                if (!TryGetPrintNumber(values, out _printNumber))
                    return false;
            }
            if (_date != null && _printNumber != 0 && !_noDateAndNumberCalculate && !_special && !_specialMonth)
            {
                if (_frequency == PrintFrequency.Daily)
                    _printNumber = 0;
                else
                    _date = null;
            }

            if (values.ContainsKey("label"))
                _label = (string)values["label"];
            if (values.ContainsKey("index"))
            {
                //_index = GetIndex(values);
                if (!TryGetIndex(values, out _index))
                    return false;
            }
            _printValues.SetValues(values);
            return true;
        }

        //NamedValues1
        protected virtual bool TryGetPrintNumber(NamedValues<ZValue> values, out int number)
        {
            number = 0;
            if (!values.ContainsKey("number"))
            {
                //throw new PBException("error print number not found");
                values.SetError("error print number not found");
                return false;
            }
            int number2;
            if (!int.TryParse((string)values["number"], out number2))
            {
                //throw new PBException("error invalid print number \"{0}\"", values["number"]);
                values.SetError("error invalid print number \"{0}\"", values["number"]);
                return false;
            }
            //return printNumber;
            number = number2;
            return true;
        }

        //NamedValues1
        protected virtual bool TryGetIndex(NamedValues<ZValue> values, out int index)
        {
            index = 0;
            if (!values.ContainsKey("index"))
            {
                //throw new PBException("error index not found");
                values.SetError("error index not found");
                return false;
            }
            string s = (string)values["index"];
            if (s == null)
                //return 0;
                return true;
            int index2;
            if (!int.TryParse(s, out index2))
            {
                //throw new PBException("error invalid index \"{0}\"", s);
                values.SetError("error invalid index \"{0}\"", s);
                return false;
            }
            //return index;
            index = index2;
            return true;
        }

        protected virtual Date GetPrintDate()
        {
            if (_date == null)
                _date = GetPrintDate(_printNumber);
            return (Date)_date;
        }

        public virtual Date GetPrintDate(int printNumber)
        {
            if (_noDateAndNumberCalculate || _specialMonth || printNumber == 0 || (_special && _specialNoDate))
                throw new PBException("unable to calculate date \"{0}\"", _name);
            if (_frequency == PrintFrequency.Weekly)
                return zprint.GetWeeklyPrintDate(printNumber, _refPrintNumber, _refPrintDate);
            if (_frequency == PrintFrequency.EveryTwoWeek)
                return zprint.GetEveryTwoWeekPrintDate(printNumber, _refPrintNumber, _refPrintDate);
            if (_frequency == PrintFrequency.Monthly)
                return zprint.GetMonthlyPrintDate(printNumber, _refPrintNumber, _refPrintDate, _noPrintMonths, _noPrintDates, _noPrintNumbers);
            if (_frequency == PrintFrequency.Quarterly)
                return zprint.GetQuarterlyPrintDate(printNumber, _refPrintNumber, _refPrintDate);
            throw new PBException("unable to calculate date \"{0}\"", _name);
        }

        protected virtual int GetPrintNumber()
        {
            if (_printNumber == 0)
            {
                if (_date == null && !_noDateAndNumberCalculate)
                    throw new PBException("unknow date and number print \"{0}\"", _name);
                if (_special || _specialMonth)
                    throw new PBException("unable to calculate number \"{0}\"", _name);
                if (!_noDateAndNumberCalculate)
                    _printNumber = GetPrintNumber((Date)_date);
            }
            return _printNumber;
        }

        public virtual int GetPrintNumber(Date date)
        {
            if (_noDateAndNumberCalculate || _special || _specialMonth)
                throw new PBException("unable to calculate print number \"{0}\"", _name);
            int refPrintNumber = _refPrintNumber;
            Date refPrintDate = _refPrintDate;
            if (_lastDate != null)
            {
                refPrintNumber = _lastPrintNumber;
                refPrintDate = (Date)_lastDate;
            }
            int printNumber = 0;
            if (_frequency == PrintFrequency.Daily)
                printNumber = zprint.GetDailyPrintNumber(date, refPrintNumber, refPrintDate, _noPrintDays, _noPrintDates, _noPrintNumbers);
            else if (_frequency == PrintFrequency.Weekly)
                printNumber = zprint.GetWeeklyPrintNumber(date, refPrintNumber, refPrintDate, (DayOfWeek)_weekday);
            else if (_frequency == PrintFrequency.EveryTwoWeek)
                printNumber = zprint.GetEveryTwoWeekPrintNumber(date, refPrintNumber, refPrintDate, (DayOfWeek)_weekday);
            else if (_frequency == PrintFrequency.Monthly)
                printNumber = zprint.GetMonthlyPrintNumber(date, refPrintNumber, refPrintDate, _noPrintMonths, _noPrintDates, _noPrintNumbers);
            else if (_frequency == PrintFrequency.Quarterly)
                printNumber = zprint.GetQuarterlyPrintNumber(date, refPrintNumber, refPrintDate);
            else
                throw new PBException("error impossible to calculate print number {0}", _name);

            _lastDate = date;
            _lastPrintNumber = printNumber;
            return printNumber;
        }

        public virtual string GetFilename(int index = 0)
        {
            //string file = string.Format("{0} - {1:yyyy-MM-dd}", _title, _date);
            string file = _title;
            if (_special || _specialMonth)
                file += " - hors-série";
            if (!_special || !_specialNoDate)
            {
                if ((_frequency == PrintFrequency.Daily || _frequency == PrintFrequency.Weekly || _frequency == PrintFrequency.EveryTwoWeek) && !_specialMonth)
                    file += string.Format(" - {0:yyyy-MM-dd}", GetPrintDate());
                else
                    file += string.Format(" - {0:yyyy-MM}", GetPrintDate());
            }
            if (!_noNumber)
                file += string.Format(" - no {0}", GetPrintNumber());
            if (_label != null)
                file += " - " + _label;
            //if (index == 0)
            //    index = _index;
            if (index != 0)
                file += "_" + index.ToString();
            return file + ".pdf";
        }

        public static PrintFrequency GetFrequency(string frequency)
        {
            switch (frequency.ToLower())
            {
                case "daily":
                    return PrintFrequency.Daily;
                case "weekly":
                    return PrintFrequency.Weekly;
                case "everytwoweek":
                    return PrintFrequency.EveryTwoWeek;
                //case "twomonthly":
                //    return PrintFrequency.TwoMonthly;
                case "monthly":
                    return PrintFrequency.Monthly;
                case "bimonthly":
                    return PrintFrequency.Bimonthly;
                case "quarterly":
                    return PrintFrequency.Quarterly;
                default:
                    throw new PBException("error unknow frequency \"{0}\"", frequency);
            }
        }
    }
}
