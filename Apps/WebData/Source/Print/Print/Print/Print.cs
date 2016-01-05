using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using pb.old; // for zSetTextVariables() in pb\_old\TextVariables.cs

// A faire :
//   NoPrintDate pour les hebdo, ex 01 net 778 du 11/07/2013 puis 779 du 08/08/2013
//     pu.GetEveryTwoWeekPrintDate()
//   supprimer baseDirectory dans le constructeur

namespace Print
{
    public class PrintInfo
    {
        public string Title;
        public bool Special;
        public Date? Date;
        public DateType DateType;
        public int? Number;
        public string Label;
    }

    public class Print
    {
        protected static bool _trace = false;
        //private static ITrace _tr = pb.Trace.CurrentTrace;
        protected string _name;
        protected string _title;
        protected PrintFrequency _frequency;
        protected DayOfWeek? _weekday = null;
        protected string _directory;
        protected bool _noNumber = false;
        protected bool _noDateAndNumberCalculate = false;
        protected bool _specialNoDate = false;
        protected PrintDateNumberReferences _dateNumberReferences = null;
        //protected Date _refPrintDate;
        //protected int _refPrintNumber;
        protected SpecialDay _noPrintDays;
        protected Month _noPrintMonths;
        protected Dictionary<int, int> _noPrintDates;
        protected Dictionary<int, int> _noPrintNumbers;
        protected Date? _lastDate = null;
        protected int _lastPrintNumber = 0;
        protected RegexValues _normalizedFilename = null;
        protected RegexValues _normalizedSpecialFilename = null;

        //public Print(string name, string title, PrintFrequency frequency, string directory, Date refPrintDate, int refPrintNumber, DayOfWeek? weekday, SpecialDay noPrintDays)
        //{
        //    _name = name;
        //    _title = title;
        //    _frequency = frequency;
        //    _directory = directory;
        //    //_refPrintDate = refPrintDate;
        //    //_refPrintNumber = refPrintNumber;
        //    _weekday = weekday;
        //    _noPrintDays = noPrintDays;
        //}

        public Print(string name, string title, string directory)
        {
            _name = name;
            _title = title;
            _directory = directory;
            _frequency = PrintFrequency.Unknow;
            _noDateAndNumberCalculate = true;
        }

        public Print(XElement xe, string baseDirectory = null, Dictionary<string, RegexValuesModel> regexModels = null)
        {
            _name = xe.zXPathValue("Name");
            _title = xe.zXPathValue("Title");
            _frequency = GetFrequency(xe.zXPathValue("Frequency"));

            _directory = xe.zXPathValue("Directory").zRootPath(baseDirectory);
            //if (!Path.IsPathRooted(_directory) && baseDirectory != null)
            //    _directory = Path.Combine(baseDirectory, _directory);
            SetOption(xe.zXPathValue("Option"));
            if (!_noDateAndNumberCalculate)
            {
                //string dateRef = xe.zXPathValue("DateReference");
                //string numberRef = xe.zXPathValue("NumberReference");
                //if (dateRef == null && numberRef != null)
                //    throw new PBException("error missing reference number \"{0}\"", _name);
                //if (dateRef != null && numberRef == null)
                //    throw new PBException("error missing reference date \"{0}\"", _name);
                //if (dateRef == null)
                //    _noDateAndNumberCalculate = true;
                //else
                //{
                //    _refPrintDate = Date.Parse(dateRef);
                //    _refPrintNumber = int.Parse(numberRef);
                //    GetNoPrintDates(xe.zXPathValues("NoPrintDate"));
                //    GetNoPrintNumbers(xe.zXPathValues("NoPrintNumber"));
                //}

                _dateNumberReferences = new PrintDateNumberReferences(xe.Elements("DateNumberReference"));
                if (_dateNumberReferences.Count == 0)
                    _noDateAndNumberCalculate = true;
                else
                {
                    GetNoPrintDates(xe.zXPathValues("NoPrintDate").ToArray());
                    GetNoPrintNumbers(xe.zXPathValues("NoPrintNumber").ToArray());
                }
            }
            if (_frequency == PrintFrequency.Weekly || _frequency == PrintFrequency.EveryTwoWeek)
                _weekday = zdate.GetDayOfWeek(xe.zXPathValue("Weekday"));
            if (regexModels != null)
            {
                string model;
                if (_frequency == PrintFrequency.Daily || _frequency == PrintFrequency.Weekly || _frequency == PrintFrequency.EveryTwoWeek)
                {
                    if (!_noNumber)
                        model = "name_day_number";
                    else
                        model = "name_day";
                }
                else
                    model = "name_month_number";
                RegexValuesModel rvm = regexModels[model];
                Dictionary<string, string> textValues = new Dictionary<string, string>();
                textValues.Add("v_title", _title);
                XElement xe2 = xe.zXPathElement("NormalizedFilename");
                if (xe2 != null)
                    //(from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa).zAttribs(textValues);
                    textValues.zAdd(from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa);
                string pattern = rvm.pattern.zSetTextVariables(textValues, true);
                string values = rvm.values.zSetTextVariables(textValues, true);
                _normalizedFilename = new RegexValues(rvm.key, rvm.name, pattern, rvm.options, values);

                if (_specialNoDate)
                    model = "special_name_number";
                else
                    model = "special_name_month_number";
                rvm = regexModels[model];
                textValues = new Dictionary<string, string>();
                textValues.Add("v_title", _title);
                xe2 = xe.zXPathElement("NormalizedSpecialFilename");
                if (xe2 != null)
                    //(from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa).zAttribs(textValues);
                    textValues.zAdd(from xa in xe2.Attributes() where xa.Name.ToString().StartsWith("v_") select xa);
                pattern = rvm.pattern.zSetTextVariables(textValues, true);
                values = rvm.values.zSetTextVariables(textValues, true);
                _normalizedSpecialFilename = new RegexValues(rvm.key, rvm.name, pattern, rvm.options, values);
            }
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

        private void GetNoPrintDates(string[] noPrintDates)
        {
            _noPrintDays = SpecialDay.NoDay;
            _noPrintMonths = Month.NoMonth;
            _noPrintDates = new Dictionary<int, int>();
            foreach (string noPrintDate in noPrintDates)
            {
                Date date;
                if (Date.TryParseExact(noPrintDate, "yyyy-MM-dd", CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    int d = date.AbsoluteDay;
                    if (!_noPrintDates.ContainsKey(d))
                        _noPrintDates.Add(d, d);
                }
                else if (Date.TryParseExact(noPrintDate, "yyyy-MM", CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    int d = date.AbsoluteDay;
                    if (!_noPrintDates.ContainsKey(d))
                        _noPrintDates.Add(d, d);
                }
                else
                {
                    SpecialDay day = SpecialDayTools.GetSpecialDay(noPrintDate);
                    if (day != SpecialDay.NoDay)
                        _noPrintDays |= SpecialDayTools.GetSpecialDay(noPrintDate);
                    else
                    {
                        Month month = zdate.GetMonth(noPrintDate);
                        if (month != Month.NoMonth)
                            _noPrintMonths |= zdate.GetMonth(noPrintDate);
                        else
                            throw new PBException("unknow NoPrintDate value \"{0}\"", noPrintDate);
                    }
                }
            }
        }

        private void GetNoPrintNumbers(string[] noPrintNumbers)
        {
            _noPrintNumbers = new Dictionary<int, int>();
            foreach (string noPrintNumber in noPrintNumbers)
            {
                int number;
                if (!int.TryParse(noPrintNumber, out number))
                    throw new PBException("unknow NoPrintNumber value \"{0}\"", noPrintNumber);
                if (!_noPrintNumbers.ContainsKey(number))
                    _noPrintNumbers.Add(number, number);
            }
        }

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public string Name { get { return _name; } }
        public string Title { get { return _title; } }
        public PrintFrequency Frequency { get { return _frequency; } }
        public DayOfWeek? Weekday { get { return _weekday; } }
        public string Directory { get { return _directory; } }
        public bool NoNumber { get { return _noNumber; } }
        public bool NoDateAndNumberCalculate { get { return _noDateAndNumberCalculate; } }
        public bool SpecialNoDate { get { return _specialNoDate; } }
        //public Date RefPrintDate { get { return _refPrintDate; } }
        //public int RefPrintNumber { get { return _refPrintNumber; } }
        public SpecialDay NoPrintDays { get { return _noPrintDays; } }
        public Dictionary<int, int> NoPrintDates { get { return _noPrintDates; } }
        public Dictionary<int, int> NoPrintNumbers { get { return _noPrintNumbers; } }
        public Month NoPrintMonths { get { return _noPrintMonths; } }
        public RegexValues NormalizedFilename { get { return _normalizedFilename; } }
        public RegexValues NormalizedSpecialFilename { get { return _normalizedSpecialFilename; } }

        public virtual PrintIssue NewPrintIssue()
        {
            return new PrintIssue(this);
        }

        public virtual PrintIssue NewPrintIssue(Date date)
        {
            return new PrintIssue(this, date);
        }

        public virtual PrintIssue NewPrintIssue(int printNumber)
        {
            return new PrintIssue(this, printNumber);
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

        //public virtual Date GetPrintDate(int printNumber)
        public virtual void GetPrintDate(int printNumber, out Date date, out DateType dateType)
        {
            if (_noDateAndNumberCalculate || printNumber == 0)
                throw new PBException("unable to calculate date \"{0}\" noDateAndNumberCalculate {1} printNumber {2}", _name, _noDateAndNumberCalculate, printNumber);
            // pas de fonction zprint.GetDailyPrintDate()
            if (_frequency == PrintFrequency.Weekly)
            {
                date = GetPeriodPrintDate(printNumber, 7);
                dateType = DateType.Day;
            }
            else if (_frequency == PrintFrequency.EveryTwoWeek)
            {
                date = GetPeriodPrintDate(printNumber, 14);
                dateType = DateType.Day;
            }
            else if (_frequency == PrintFrequency.Monthly)
            {
                date = GetMonthlyPrintDate(printNumber);
                dateType = DateType.Month;
            }
            else if (_frequency == PrintFrequency.Bimonthly)
            {
                date = GetMultiMonthlyPrintDate(printNumber, 2);
                dateType = DateType.Month;
            }
            else if (_frequency == PrintFrequency.Quarterly)
            {
                date = GetMultiMonthlyPrintDate(printNumber, 3);
                dateType = DateType.Month;
            }
            else
                throw new PBException("unable to calculate date \"{0}\" frequency {1}", _name, _frequency);
        }

        public virtual int GetPrintNumber(Date date, bool throwException = true)
        {
            if (_noDateAndNumberCalculate)
            {
                if (throwException)
                    throw new PBException("unable to calculate print number \"{0}\" noDateAndNumberCalculate {1}", _name, _noDateAndNumberCalculate);
                else
                    pb.Trace.WriteLine("unable to calculate print number \"{0}\" noDateAndNumberCalculate {1}", _name, _noDateAndNumberCalculate);
            }
            //int refPrintNumber = _refPrintNumber;
            //Date refPrintDate = _refPrintDate;
            //if (_lastDate != null)
            //{
            //    refPrintNumber = _lastPrintNumber;
            //    refPrintDate = (Date)_lastDate;
            //}
            int printNumber = 0;
            if (_frequency == PrintFrequency.Daily)
                //printNumber = zprint.GetDailyPrintNumber(date, refPrintNumber, refPrintDate, _noPrintDays, _noPrintDates, _noPrintNumbers);
                printNumber = GetDailyPrintNumber(date);
            else if (_frequency == PrintFrequency.Weekly)
                //printNumber = zprint.GetWeeklyPrintNumber(date, refPrintNumber, refPrintDate, (DayOfWeek)_weekday);
                printNumber = GetPeriodPrintNumber(date, 7);
            else if (_frequency == PrintFrequency.EveryTwoWeek)
                //printNumber = zprint.GetEveryTwoWeekPrintNumber(date, refPrintNumber, refPrintDate, (DayOfWeek)_weekday);
                printNumber = GetPeriodPrintNumber(date, 14);
            else if (_frequency == PrintFrequency.Monthly)
                //printNumber = zprint.GetMonthlyPrintNumber(date, refPrintNumber, refPrintDate, _noPrintMonths, _noPrintDates, _noPrintNumbers);
                printNumber = GetMonthlyPrintNumber(date);
            else if (_frequency == PrintFrequency.Bimonthly)
                //printNumber = zprint.GetMultiMonthlyPrintNumber(date, refPrintNumber, refPrintDate, _noPrintMonths, _noPrintDates, _noPrintNumbers, 2);
                printNumber = GetMultiMonthlyPrintNumber(date, 2);
            else if (_frequency == PrintFrequency.Quarterly)
                //printNumber = zprint.GetMultiMonthlyPrintNumber(date, refPrintNumber, refPrintDate, _noPrintMonths, _noPrintDates, _noPrintNumbers, 3);
                printNumber = GetMultiMonthlyPrintNumber(date, 3);
            else
            {
                if (throwException)
                    throw new PBException("error impossible to calculate print number {0}", _name);
                else
                    pb.Trace.WriteLine("error impossible to calculate print number {0}", _name);
            }

            //_lastDate = date;
            //_lastPrintNumber = printNumber;
            return printNumber;
        }

        //protected Date GetWeeklyPrintDate(int printNumber)
        protected Date GetPeriodPrintDate(int printNumber, int nbDaysInPeriod)
        {
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(printNumber);
            //return dateNumberReference.date.AddDays((printNumber - dateNumberReference.number) * 7);
            return dateNumberReference.date.AddDays((printNumber - dateNumberReference.number) * nbDaysInPeriod);
        }

        //protected Date GetEveryTwoWeekPrintDate(int printNumber)
        //{
        //    PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(printNumber);
        //    return dateNumberReference.date.AddDays((printNumber - dateNumberReference.number) * 14);
        //}

        protected Date GetMonthlyPrintDate(int printNumber)
        {
            //int no = refNumber;
            //Date date = refDate;
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(printNumber);
            int no = dateNumberReference.number;
            Date date = dateNumberReference.date;
            while (printNumber > no)
            {
                date = date.AddMonths(1);
                Month month = zdate.GetMonth(date.Month);
                if ((_noPrintMonths & month) != month && !_noPrintDates.ContainsKey(date.AbsoluteDay))
                {
                    do
                    {
                        no++;
                    } while (_noPrintNumbers.ContainsKey(no));
                }
            }
            while (printNumber < no)
            {
                date = date.AddMonths(-1);
                Month month = zdate.GetMonth(date.Month);
                if ((_noPrintMonths & month) != month && !_noPrintDates.ContainsKey(date.AbsoluteDay))
                {
                    do
                    {
                        no--;
                    } while (_noPrintNumbers.ContainsKey(no));
                }
            }
            return date;
        }

        protected Date GetMultiMonthlyPrintDate(int printNumber, int monthFreq)
        {
            //int no = refNumber;
            //Date date = refDate;
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(printNumber);
            int no = dateNumberReference.number;
            Date date = dateNumberReference.date;
            while (printNumber > no)
            {
                Month month;
                for (int i = 0; i < monthFreq; i++)
                {
                    do
                    {
                        date = date.AddMonths(1);
                        month = zdate.GetMonth(date.Month);
                    } while ((_noPrintMonths & month) == month || _noPrintDates.ContainsKey(date.AbsoluteDay));
                }
                do
                {
                    no++;
                } while (_noPrintNumbers.ContainsKey(no));
            }
            while (printNumber < no)
            {
                Month month;
                for (int i = 0; i < monthFreq; i++)
                {
                    do
                    {
                        date = date.AddMonths(-1);
                        month = zdate.GetMonth(date.Month);
                    } while ((_noPrintMonths & month) == month || _noPrintDates.ContainsKey(date.AbsoluteDay));
                }
                do
                {
                    no--;
                } while (_noPrintNumbers.ContainsKey(no));
            }
            return date;
        }

        protected int GetDailyPrintNumber(Date date)
        {
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(date);
            int no = dateNumberReference.number;
            Date date2 = dateNumberReference.date;
            while (date > date2)
            {
                date2 = date2.AddDays(1);
                if (!_noPrintDates.ContainsKey(date2.AbsoluteDay) && !SpecialDayTools.IsSpecialDay(_noPrintDays, date2))
                {
                    do
                    {
                        no++;
                    } while (_noPrintNumbers.ContainsKey(no));
                }
            }
            while (date < date2)
            {
                if (!_noPrintDates.ContainsKey(date2.AbsoluteDay) && !SpecialDayTools.IsSpecialDay(_noPrintDays, date2))
                {
                    do
                    {
                        no--;
                    } while (_noPrintNumbers.ContainsKey(no));
                }
                date2 = date2.AddDays(-1);
            }
            return no;
        }

        protected int GetPeriodPrintNumber(Date date, int nbDaysInPeriod)
        {
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(date);
            return dateNumberReference.number + (zdate.GetNearestWeekday(date, (DayOfWeek)_weekday).Subtract(dateNumberReference.date).Days / nbDaysInPeriod);
        }

        //protected int GetWeeklyPrintNumber(Date date)
        //{
        //    PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(date);
        //    return dateNumberReference.number + (zdate.GetNearestWeekday(date, (DayOfWeek)_weekday).Subtract(dateNumberReference.date).Days / 7);
        //}

        //protected int GetEveryTwoWeekPrintNumber(Date date)
        //{
        //    PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(date);
        //    return dateNumberReference.number + (zdate.GetNearestWeekday(date, (DayOfWeek)_weekday).Subtract(dateNumberReference.date).Days / 14);
        //}

        protected int GetMonthlyPrintNumber(Date date)
        {
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(date);
            int no = dateNumberReference.number;
            Date date2 = dateNumberReference.date;
            while (date > date2)
            {
                date2 = date2.AddMonths(1);
                Month month = zdate.GetMonth(date2.Month);
                if ((_noPrintMonths & month) != month && !_noPrintDates.ContainsKey(date2.AbsoluteDay))
                {
                    do
                    {
                        no++;
                    } while (_noPrintNumbers.ContainsKey(no));
                }
            }
            while (date < date2)
            {
                Month month = zdate.GetMonth(date2.Month);
                if ((_noPrintMonths & month) != month && !_noPrintDates.ContainsKey(date2.AbsoluteDay))
                {
                    do
                    {
                        no--;
                    } while (_noPrintNumbers.ContainsKey(no));
                }
                date2 = date2.AddMonths(-1);
            }
            return no;
        }

        protected int GetMultiMonthlyPrintNumber(Date date, int monthFreq)
        {
            //int no = refNumber;
            //Date date2 = refDate;
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(date);
            int no = dateNumberReference.number;
            Date date2 = dateNumberReference.date;
            while (date > date2)
            {
                Month month;
                for (int i = 0; i < monthFreq; i++)
                {
                    do
                    {
                        date2 = date2.AddMonths(1);
                        month = zdate.GetMonth(date2.Month);
                    } while ((_noPrintMonths & month) == month || _noPrintDates.ContainsKey(date2.AbsoluteDay));
                }
                do
                {
                    no++;
                } while (_noPrintNumbers.ContainsKey(no));
            }
            while (date < date2)
            {
                for (int i = 0; i < monthFreq; i++)
                {
                    Month month = zdate.GetMonth(date2.Month);
                    while ((_noPrintMonths & month) == month || _noPrintDates.ContainsKey(date2.AbsoluteDay))
                    {
                        date2 = date2.AddMonths(-1);
                        month = zdate.GetMonth(date2.Month);
                    }
                    date2 = date2.AddMonths(-1);
                }
                do
                {
                    no--;
                } while (_noPrintNumbers.ContainsKey(no));
            }
            return no;
        }

        protected static void WriteLine(string msg, params object[] prm)
        {
            if (_trace)
                pb.Trace.WriteLine(msg, prm);
        }
    }

    public class PrintIssue
    {
        protected static bool _trace = false;
        //private static ITrace _tr = pb.Trace.CurrentTrace;
        protected Print _print = null;
        protected Date? _date = null;
        protected DateType _dateType = DateType.Unknow;
        protected int _printNumber = 0;
        protected bool _special = false;  // hors-série
        protected bool _specialMonth = false;  // hors-série
        protected string _specialText = null;
        protected string _label = null;
        protected int _index = 0;
        //NamedValues1
        protected NamedValues<ZValue> _printValues = new NamedValues<ZValue>();
        protected string _error = null;

        public PrintIssue(Print print)
        {
            _print = print;
        }

        public PrintIssue(Print print, Date date)
        {
            _print = print;
            _date = date;
        }

        public PrintIssue(Print print, int printNumber)
        {
            _print = print;
            _printNumber = printNumber;
        }

        public static bool Trace { get { return _trace; } set { _trace = value; } }
        public Print Print { get { return _print; } }
        public Date? Date { get { TryGetPrintDate(); return _date; } set { _date = value; } }                 // 25/09/2014 set used in FindPrintManager.Find()
        public DateType DateType { get { return _dateType; } set { _dateType = value; } }                     // set used in FindPrintManager.Find()
        public int PrintNumber { get { return GetPrintNumber(); } set { _printNumber = value; } }
        public bool Special { get { return _special; } set { _special = value; } }
        public bool SpecialMonth { get { return _specialMonth; } }
        public string SpecialText { get { return _specialText; } set { _specialText = value; } }              // 25/09/2014 set used in FindPrintManager.Find()
        public string Label { get { return _label; } }
        public int Index { get { return _index; } }
        public NamedValues<ZValue> PrintValues { get { return _printValues; } }
        public string Error { get { return _error; } }

        public virtual bool Control()
        {
            if (!TryGetPrintDate())
                return false;
            string error;
            if (!ControlDate((Date)_date, out error))
            {
                SetError(error);
                return false;
            }
            return true;
        }

        public static bool ControlDate(Date date, out string error)
        {
            error = null;
            Date today = pb.Date.Today;
            if (date.Year < today.Year - 10)
            {
                error = string.Format("error date issue is to old {0}", date);
                return false;
            }
            if (date.Subtract(today).Days > 180)
            {
                error = string.Format("error date issue is inconsistent {0}", date);
                return false;
            }
            return true;
        }

        public virtual bool TrySetValues(NamedValues<ZValue> values)
        {
            //_error = null;
            //_printValues.SetValues(values);

            bool special = _special;
            if (values.ContainsKey("special"))
            {
                special = true;
                _printValues.SetValues(values, "special");
            }

            bool specialMonth = _specialMonth;
            if (values.ContainsKey("special_month"))
            {
                specialMonth = true;
                _printValues.SetValues(values, "special_month");
            }

            Date? date = _date;
            DateType dateType = _dateType;
            if (values.ContainsKey("day_near_current_date") || values.ContainsKey("month"))
            {
                Date date2;
                DateType dateType2;
                if (!zdate.TryCreateDate(values, out date2, out dateType2))
                {
                    //if (values.Error != null)
                    //    _error = values.Error;
                    return false;
                }
                string error;
                if (!ControlDate(date2, out error))
                    return false;
                date = date2;
                dateType = dateType2;
                _printValues.SetValues(values, "day_near_current_date", "day", "month", "year");
            }

            int printNumber = _printNumber;
            if (values.ContainsKey("number"))
            {
                if (!TryGetPrintNumber(values, out printNumber))
                    return false;
                _printValues.SetValues(values, "number");
            }

            if (date != null && printNumber != 0 && !_print.NoDateAndNumberCalculate && !special && !specialMonth)
            {
                if (_print.Frequency == PrintFrequency.Daily)
                    printNumber = 0;
                else
                {
                    date = null;
                    dateType = pb.DateType.Unknow;
                }
            }

            string label = _label;
            if (values.ContainsKey("label"))
            {
                label = (string)values["label"];
                _printValues.SetValues(values, "label");
            }

            int index = _index;
            if (values.ContainsKey("index"))
            {
                if (!TryGetIndex(values, out index))
                    return false;
                _printValues.SetValues(values, "index");
            }

            _error = null;
            _special = special;
            _specialMonth = specialMonth;
            _date = date;
            _dateType = dateType;
            _printNumber = printNumber;
            _label = label;
            _index = index;
            return true;
        }

        protected virtual bool TryGetPrintNumber(NamedValues<ZValue> values, out int number)
        {
            number = 0;
            if (!values.ContainsKey("number"))
            {
                SetError("error print number not found");
                return false;
            }
            int number2;
            if (!int.TryParse((string)values["number"], out number2))
            {
                SetError("error invalid print number \"{0}\"", values["number"]);
                return false;
            }
            if (_print.Frequency != PrintFrequency.Daily && !_special)
            {
                //Date date = _print.GetPrintDate(number2);
                Date date;
                DateType dateType;
                _print.GetPrintDate(number2, out date, out dateType);
                string error;
                if (!ControlDate(date, out error))
                {
                    return false;
                }
            }
            number = number2;
            return true;
        }

        protected virtual bool TryGetIndex(NamedValues<ZValue> values, out int index)
        {
            index = 0;
            if (!values.ContainsKey("index"))
            {
                SetError("error index not found");
                return false;
            }
            //string
            string s = (string)values["index"];
            if (s == null)
                return true;
            int index2;
            if (!int.TryParse(s, out index2))
            {
                SetError("error invalid index \"{0}\"", s);
                return false;
            }
            index = index2;
            return true;
        }

        protected virtual bool TryGetPrintDate()
        {
            if (_date == null)
            {
                if (_special)
                {
                    SetError("unable to calculate date for special issue \"{0}\"", _print.Name);
                    return false;
                }
                if (_specialMonth || _printNumber == 0)
                {
                    SetError("unable to calculate date \"{0}\" specialMonth {1} printNumber {2}", _print.Name, _specialMonth, _printNumber);
                    return false;
                }
                if (_print.Frequency == PrintFrequency.Daily)
                {
                    SetError("unable to calculate date of Daily print \"{0}\" from printNumber {1}", _print.Name, _printNumber);
                    return false;
                }
                //_date = _print.GetPrintDate(_printNumber);
                Date date;
                _print.GetPrintDate(_printNumber, out date, out _dateType);
                _date = date;
            }
            return true;
        }

        public virtual bool CanCalculatePrintNumber()
        {
            return !_print.NoDateAndNumberCalculate && _date != null && !_special && !_specialMonth;
        }

        protected virtual int GetPrintNumber(bool throwException = true)
        {
            if (_printNumber == 0)
            {
                //if (_date == null && !_print.NoDateAndNumberCalculate)
                //    throw new PBException("unknow date and number print \"{0}\" NoDateAndNumberCalculate {1}", _print.Name, _print.NoDateAndNumberCalculate);
                //if (_special || _specialMonth)
                //    throw new PBException("unable to calculate number \"{0}\" special {1} specialMonth {2}", _print.Name, _special, _specialMonth);
                //if (!_print.NoDateAndNumberCalculate && _date != null && !_special && !_specialMonth)
                if (CanCalculatePrintNumber())
                    _printNumber = _print.GetPrintNumber((Date)_date, throwException);
            }
            return _printNumber;
        }

        protected virtual string GetFilenameOption()
        {
            return null;
        }

        public virtual string GetFilename(int index = 0)
        {
            string file = _print.Title;
            if (_special || _specialMonth)
                file += " - hors-série";
            if (!_special || !_print.SpecialNoDate)
            {
                PrintFrequency frequency = _print.Frequency;
                //Date? date = Date;
                TryGetPrintDate();
                if (_date != null)
                {
                    //if ((frequency == PrintFrequency.Daily || frequency == PrintFrequency.Weekly || frequency == PrintFrequency.EveryTwoWeek) && !_specialMonth)
                    //    file += string.Format(" - {0:yyyy-MM-dd}", date);
                    //else
                    //    file += string.Format(" - {0:yyyy-MM}", date);
                    if (_dateType == pb.DateType.Day)
                        file += string.Format(" - {0:yyyy-MM-dd}", _date);
                    else
                        file += string.Format(" - {0:yyyy-MM}", _date);
                }
            }
            if (!_print.NoNumber)
            {
                int number = GetPrintNumber(throwException: false);
                if (number != 0)
                    //file += string.Format(" - no {0}", GetPrintNumber());
                    file += string.Format(" - no {0}", number);
            }
            file += GetFilenameOption();
            if (_specialText != null && _specialText != "")
                file += " - " + _specialText;
            if (_label != null && _label != "")
                file += " - " + _label;
            if (index != 0)
                file += "_" + index.ToString();
            return file + ".pdf";
        }

        private static Regex __rgPrintInfo = new Regex("^(.*?)( - hors-série)?( - (([0-9]{4}-[0-9]{2}-[0-9]{2})|([0-9]{4}-[0-9]{2})|([0-9]{4})))( - no ([0-9]+))?( - (.*))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        public static PrintInfo GetPrintInfo(string filename)
        {
            Match match = __rgPrintInfo.Match(filename);
            if (match.Success)
            {
                PrintInfo printInfo = new PrintInfo();
                if (match.Groups[1].Value != "")
                    printInfo.Title = match.Groups[1].Value;
                if (match.Groups[2].Value != "")
                    printInfo.Special = true;
                if (match.Groups[5].Value != "")
                {
                    printInfo.Date = (Date)DateTime.ParseExact(match.Groups[5].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    printInfo.DateType = DateType.Day;
                }
                if (match.Groups[6].Value != "")
                {
                    printInfo.Date = (Date)DateTime.ParseExact(match.Groups[6].Value, "yyyy-MM", CultureInfo.InvariantCulture);
                    printInfo.DateType = DateType.Month;
                }
                if (match.Groups[7].Value != "")
                {
                    printInfo.Date = (Date)DateTime.ParseExact(match.Groups[7].Value, "yyyy", CultureInfo.InvariantCulture);
                    printInfo.DateType = DateType.Year;
                }
                if (match.Groups[9].Value != "")
                    printInfo.Number = int.Parse(match.Groups[9].Value);
                if (match.Groups[11].Value != "")
                    printInfo.Label = match.Groups[11].Value;
                return printInfo;
            }
            else
                return null;
        }

        public static string GetStandardFilename(string title, bool special = false, Date? date = null, DateType dateType = DateType.Unknow, int? number = null, string label = null)
        {
            //string file = "";
            //if (findPrint.directory != null)
            //    file = findPrint.directory + "\\";
            //else if (_defaultDirectory != null)
            //    file = _defaultDirectory + "\\";

            //if (findPrint.title != null)
            //{
            //    file += findPrint.title + "\\";
            //    file += findPrint.title;
            //}
            //else
            //{
            //    file += findPrint.name + "\\";
            //    file += findPrint.name;
            //}

            string file = title;
            if (special)
                file += " - hors-série";

            if (date != null)
            {
                switch (dateType)
                {
                    case DateType.Day:
                        file += string.Format(@" - {0:yyyy-MM-dd}", date);
                        break;
                    case DateType.Month:
                        file += string.Format(@" - {0:yyyy-MM}", date);
                        break;
                    case DateType.Year:
                        file += string.Format(@" - {0:yyyy}", date);
                        break;
                    default:
                        throw new PBException("unknow DateType {0}", dateType);
                }
            }

            if (number != null)
                file += string.Format(@" - no {0}", number);

            //if (findPrint.date == null && findPrint.number == null)
            //{
            //    Trace.WriteLine("warning can't find date nor number in \"{0}\"", findPrint.text);
            //    file += zfile.ReplaceBadFilenameChars(findPrint.remainText, "");
            //}

            if (label != null && label != "")
                file += " - " + label;

            return file;
        }

        // bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad
        //public bool Match(RegexValues rv, string filename, Predicate<NamedValues<ZValue>> validValues = null)
        public MatchValues Match(RegexValues rv, string filename, Predicate<NamedValues<ZValue>> validValues = null)
        {
            //rv.Match_old(filename);
            //return MatchSetValues(rv, validValues);
            // bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad bad
            MatchValues matchValues = rv.Match(filename);
            if (matchValues.Success && MatchSetValues(matchValues, validValues))
                return matchValues;
            else
                return null;
        }

        //public bool MatchSetValues(RegexValues rv, Predicate<NamedValues<ZValue>> validValues = null)
        public bool MatchSetValues(MatchValues matchValues, Predicate<NamedValues<ZValue>> validValues = null)
        {
            //if (!rv.Success_old)
            if (!matchValues.Success)
                return false;
            //NamedValues<ZValue> values = rv.GetValues_old();
            NamedValues<ZValue> values = matchValues.GetValues();

            if (_trace)
                values.zTrace();

            if (validValues != null)
            {
                if (!validValues(values))
                    return false;
            }

            if (!TrySetValues(values))
            {
                //_error = string.Format("find \"{0}\" error \"{1}\"", _print.Name, values.Error);
                return false;
            }
            return true;
        }


        protected void SetError(string error, params object[] prm)
        {
            if (prm.Length > 0)
                error = string.Format(error, prm);
            _error = error;
            WriteLine(error);
        }

        protected static void WriteLine(string msg, params object[] prm)
        {
            if (_trace)
                pb.Trace.WriteLine(msg, prm);
        }

        //public static bool IsNumberValid(Dictionary<string, object> values)
        public static bool IsNumberValid(Dictionary<string, ZValue> values)
        {
            if (values.ContainsKey("number"))
            {
                int number;
                if (int.TryParse((string)values["number"], out number))
                {
                    if (number > 0)
                        return true;
                }
            }
            return false;
        }
    }

    public static partial class GlobalExtension
    {
        //public static Dictionary<string, string> zAttribs(this IEnumerable<XAttribute> xattribs, Dictionary<string, string> attribs = null)
        //{
        //    if (attribs == null)
        //        attribs = new Dictionary<string, string>();
        //    foreach (XAttribute xattrib in xattribs)
        //    {
        //        string name = xattrib.Name.LocalName.ToLower();
        //        if (!attribs.ContainsKey(name))
        //            attribs.Add(name, xattrib.Value);
        //        else
        //            attribs[name] = xattrib.Value;
        //    }
        //    return attribs;
        //}

        public static void zAdd(this Dictionary<string, string> dictionary, IEnumerable<XAttribute> xattribs)
        {
            foreach (XAttribute xattrib in xattribs)
            {
                string name = xattrib.Name.LocalName.ToLower();
                if (!dictionary.ContainsKey(name))
                    dictionary.Add(name, xattrib.Value);
                else
                    dictionary[name] = xattrib.Value;
            }
        }
    }
}
