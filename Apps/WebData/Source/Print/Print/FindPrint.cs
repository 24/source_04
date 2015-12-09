using pb;
using pb.IO;
using pb.Text;

namespace Print
{
    public class FindPrintInfo
    {
        public bool found;
        // source
        public string sourceTitle;
        public PrintType printType;
        // info
        public string name;
        public string title;
        public string DayTitle;
        public string directory;
        public Date? date;
        public DateType dateType = DateType.Unknow;
        public int? number;
        public bool special = false;
        public string specialText;
        public string label;
        public string file;
        public string remainText;
        public Print print;
        public FindPrintType findPrintType;
        public PrintTitleInfo titleInfo;
    }

    //// source
    //private string _sourceTitle = null;
    //private PrintType _printType = PrintType.Unknow;
    //// info
    //private string _name = null;
    //private string _title = null;
    //private string _directory = null;
    //private Date? _date = null;
    //private DateType _dateType = DateType.Unknow;
    //private int? _number = null;
    //private bool _special = false;
    //private string _specialText = null;
    //private string _label = null;
    //private string _file = null;
    //private string _remainText = null;
    //private Print _print = null;
    //private FindPrintType _findPrintType = FindPrintType.UnknowPrint;
    //private PrintTitleInfo _titleInfo = null;

    //public bool Found { get { return _found; } }
    //public string SourceTitle { get { return _sourceTitle; } }
    //public PrintType PrintType { get { return _printType; } }
    //public string Name { get { return _name; } }
    //public string Title { get { return _title; } }
    //public string Directory { get { return _directory; } }
    //public Date? Date { get { return _date; } }
    //public DateType DateType { get { return _dateType; } }
    //public int? Number { get { return _number; } }
    //public bool Special { get { return _special; } }
    //public string SpecialText { get { return _specialText; } }
    //public string Label { get { return _label; } }
    //public string File { get { return _file; } }
    //public string RemainText { get { return _remainText; } }
    //public Print Print { get { return _print; } }
    //public FindPrintType FindPrintType { get { return _findPrintType; } }
    //public PrintTitleInfo TitleInfo { get { return _titleInfo; } }

    public class FindPrint
    {
        private static bool __traceWarning = false;
        private static int __maxFilenameLength = 100;

        private FindPrintManager _findPrintManager = null;
        private string _sourceTitle;
        private PrintType _printType = PrintType.Unknow;
        private bool _forceSelect = false;
        private Date? _expectedDate = null;

        private bool _found = false;
        private string _name = null;
        private string _title = null;
        private string _dayTitle = null;
        private string _directory = null;
        private Date? _date = null;
        private DateType _dateType = DateType.Unknow;
        private int? _number = null;
        private bool _special = false;
        private string _specialText = null;
        private string _label = null;
        private string _file = null;
        private string _remainText = null;
        private Print _print = null;
        private FindText _findPrint = null;
        private FindPrintType _findPrintType = FindPrintType.UnknowPrint;
        private PrintTitleInfo _titleInfo = null;

        public static bool TraceWarning { get { return __traceWarning; } set { __traceWarning = value; } }

        public static FindPrintInfo Find(FindPrintManager findPrintManager, string title, PrintType printType = PrintType.Unknow, bool forceSelect = false, Date? expectedDate = null)
        {
            FindPrint findPrint = new FindPrint();
            findPrint._findPrintManager = findPrintManager;
            findPrint._sourceTitle = title;
            findPrint._printType = printType;
            findPrint._forceSelect = forceSelect;
            findPrint._expectedDate = expectedDate;
            findPrint._Find();
            return findPrint.GetFindPrintInfo();
        }

        private FindPrintInfo GetFindPrintInfo()
        {
            FindPrintInfo findPrintInfo = new FindPrintInfo();
            findPrintInfo.found = _found;
            findPrintInfo.sourceTitle = _sourceTitle;
            findPrintInfo.printType = _printType;

            findPrintInfo.name = _name;
            findPrintInfo.title = _title;
            findPrintInfo.DayTitle = _dayTitle;
            findPrintInfo.directory = _directory;
            findPrintInfo.date = _date;
            findPrintInfo.dateType = _dateType;
            findPrintInfo.number = _number;
            findPrintInfo.special = _special;
            findPrintInfo.specialText = _specialText;
            findPrintInfo.label = _label;
            findPrintInfo.file = _file;
            findPrintInfo.remainText = _remainText;
            findPrintInfo.print = _print;
            findPrintInfo.findPrintType = _findPrintType;
            findPrintInfo.titleInfo = _titleInfo;

            return findPrintInfo;
        }

        private void _Find()
        {
            GetTitleInfo();

            _FindPrint();

            if (!_findPrint.found)
            {
                FindDayAndPrint();
            }

            GetPrintInfo();

            if (!_found && _forceSelect)
                ForceSelect();

            if (_found)
                _file = GetPostTypeDirectory(_printType) + "\\" + _file;
        }

        private void GetTitleInfo()
        {
            _titleInfo = _findPrintManager.PrintTitleManager.GetPrintTitleInfo(_sourceTitle, expectedDate: _expectedDate);

            _date = _titleInfo.Date;
            _dateType = _titleInfo.DateType;
            _number = _titleInfo.Number;
            _special = _titleInfo.Special;
            _specialText = _titleInfo.SpecialText;
            _remainText = _titleInfo.RemainText;
        }

        private void _FindPrint()
        {
            if (_titleInfo.FormatedTitle != "")
                _findPrint = _findPrintManager.FindPrintList.Find(_titleInfo.FormatedTitle);
            else if (_titleInfo.Date != null && _titleInfo.RemainText != "")
                // pour fichier du monde 20150829_QUO.pdf formatedTitle=""
                _findPrint = _findPrintManager.FindPrintList.Find(_titleInfo.RemainText);
            else
                _findPrint = new FindText();   // not found
        }

        private void FindDayAndPrint()
        {
            if (!_findPrintManager.UseFindDay || _expectedDate == null || _titleInfo.FormatedTitle == "" || _date != null)
                return;

            if (__traceWarning)
                Trace.WriteLine("  search day \"{0}\"", _titleInfo.FormatedTitle);
            FindDay findDay = _findPrintManager.FindDayManager.Find(_titleInfo.FormatedTitle);
            while (findDay.Found)
            {
                int day = (int)findDay.Day;
                Date? date = zdate.GetDayInsideDateGap(day, (Date)_expectedDate, _findPrintManager.GapDayBefore, _findPrintManager.GapDayAfter);
                if (date != null)
                {
                    if (__traceWarning)
                        Trace.WriteLine("  day found {0} date {1} expected date {2}", day, date, _expectedDate);
                    //_dateMatch = findDay.MatchValues;
                    //title = findDay.MatchValues.Replace(" $$day$$ ");
                    //_foundDate = true;
                    string title = findDay.MatchValues.Replace("").Trim();
                    if (_dayTitle == null)
                    {
                        _dayTitle = title;
                        _date = date;
                        _dateType = DateType.Day;
                    }
                    if (__traceWarning)
                        Trace.WriteLine("  search print \"{0}\"", title);
                    _findPrint = _findPrintManager.FindPrintList.Find(title);
                    if (_findPrint.found)
                    {
                        _dayTitle = title;
                        _date = date;
                        _dateType = DateType.Day;
                        break;
                    }
                }
                else if (__traceWarning)
                    Trace.WriteLine("  wrong day found {0} expected date {1} in \"{2}\"", day, _expectedDate, _titleInfo.FormatedTitle);
                findDay.FindNext();
            }

            //    if (_expectedDate != null && _printTitleManager.UseFindDay)
            //    {
            //        FindDay findDay = _printTitleManager.FindDayManager.Find(title);
            //        //Trace.WriteLine("FindDay \"{0}\" : found {1} day {2}", title, findDay.Found, findDay.Day);
            //        if (findDay.Found)
            //        {
            //            int day = (int)findDay.Day;
            //            Date? date = zdate.GetDayInsideDateGap(day, (Date)_expectedDate, _printTitleManager.GapDayBefore, _printTitleManager.GapDayAfter);
            //            if (date != null)
            //            {
            //                _date = date;
            //                _dateType = DateType.Day;
            //                _dateMatch = findDay.MatchValues;
            //                title = findDay.MatchValues.Replace(" $$day$$ ");
            //                _foundDate = true;
            //            }
            //            else
            //                Trace.WriteLine("  wrong day found {0} expected date {1} in \"{2}\"", day, _expectedDate, title);
            //        }
            //    }
            //    return title;
        }

        private void GetPrintInfo()
        {
            //if (_findPrint == null || !_findPrint.found)
            if (!_findPrint.found)
                return;

            _found = true;
            MatchValues matchValues = _findPrint.matchValues;
            _name = matchValues.Name;

            _print = _findPrintManager.PrintManager[_name];

            if (_print == null)
            {
                _findPrintType = FindPrintType.PrintType1;
                if (matchValues.Attributes.ContainsKey("title"))
                    _title = matchValues.Attributes["title"];
                if (matchValues.Attributes.ContainsKey("directory"))
                    _directory = matchValues.Attributes["directory"];
                string directory;
                if (_directory != null)
                    directory = _directory;
                else
                    directory = _findPrintManager.DefaultPrintDirectory;
                _print = new Print(_name, _title, GetDirectory(directory));
            }
            else
                _findPrintType = FindPrintType.PrintType2;

            PrintIssue printIssue = _print.NewPrintIssue();

            if (_date != null)
            {
                printIssue.Date = _date;
                printIssue.DateType = _dateType;
                printIssue.Special = _special;
                printIssue.SpecialText = _specialText;

                if (_number != null)
                {
                    if (printIssue.CanCalculatePrintNumber())
                    {
                        int calculatedPrintNumber = printIssue.Print.GetPrintNumber((Date)printIssue.Date);
                        if (calculatedPrintNumber != (int)_number)
                        {
                            if (__traceWarning)
                                Trace.WriteLine("warning number in title {0} is different than calculated number {1}", (int)_number, calculatedPrintNumber);
                        }
                    }
                    // utilise de préférence le no du titre plutot que celui calculé
                    // sauf pour le monde ex : "Le Monde week-end + Magazine + 3 suppléments du samedi 30 aout 2014" le 3 n'est pas le bon numéro
                    printIssue.PrintNumber = (int)_number;
                }
            }
            else if (_number != null)
            {
                printIssue.PrintNumber = (int)_number;
                printIssue.Special = _special;
            }

            printIssue.TrySetValues(_findPrint.matchValues.GetAllValues());
            _label = printIssue.Label;

            _file = zPath.Combine(_print.Directory, zPath.GetFileNameWithoutExtension(printIssue.GetFilename()));
        }

        private void ForceSelect()
        {
            _found = true;
            _findPrintType = FindPrintType.UnknowPrint;
            _name = _titleInfo.Name;
            _title = _titleInfo.FormatedTitle;
            string directory = null;
            if (_printType == PrintType.Print)
                directory = _findPrintManager.UnknowPrintDirectory;

            _file = GetFile(directory);
        }

        private string GetDirectory()
        {
            string directory;
            if (_directory != null)
                directory = _directory;
            else
                directory = _findPrintManager.DefaultPrintDirectory;
            return GetDirectory(directory);
        }

        private string GetDirectory(string directory)
        {
            return GetDirectory(directory, GetTitleFileName());
        }

        private string GetDirectory(string directory, string titleFileName)
        {
            if (directory != null)
                directory += "\\";

            directory += titleFileName + "\\";

            return directory;
        }

        private string GetTitleFileName()
        {
            string title;
            if (_title != null)
                title = _title;
            else
                title = _name;
            title = zfile.ReplaceBadFilenameChars(title, "_");

            // truncate title to max file length
            if (__maxFilenameLength > 0 && title.Length > __maxFilenameLength)
                title = title.Substring(0, __maxFilenameLength);

            return title;
        }

        private string GetFile(string directory)
        {
            string titleFileName = GetTitleFileName();
            directory = GetDirectory(directory, titleFileName);

            if (_printType == PrintType.Print && _date == null && _number == null)
            {
                if (__traceWarning)
                    Trace.WriteLine("warning can't find date nor number in \"{0}\"", _sourceTitle);
            }

            string label = null;
            if (_specialText != null)
                label = zfile.ReplaceBadFilenameChars(_specialText, "_");
            string file = PrintIssue.GetStandardFilename(titleFileName, _special, _date, _dateType, _number, label);
            return directory + file;
        }

        private string GetPostTypeDirectory(PrintType postType)
        {
            if (_findPrintManager.PostTypeDirectories.ContainsKey(postType))
                return _findPrintManager.PostTypeDirectories[postType];
            else
            {
                if (__traceWarning)
                    Trace.WriteLine("warning directory of post type {0} is not defined use \"UndefinedPostTypeDirectory\" instead");
                return "UndefinedPostTypeDirectory";
            }
        }
    }
}
