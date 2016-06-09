using System.Collections.Generic;
using pb;
using pb.Text;

namespace Download.Print
{
    public enum FindPrintType
    {
        UnknowPrint = 0,
        PrintType1,                  // print exist in print_config.xml
        PrintType2                   // print exist in print_list2.xml
    }

    public enum PrintType
    {
        Unknow = 0,
        Print,
        Book,
        Comics,
        UnknowEBook
    }

    public class FindPrintManager
    {
        private static bool __traceWarning = false;
        //private int _version = 1;
        private PrintTitleManager _printTitleManager = null;
        private bool _trySplitTitle = false;

        private FindDayManager _findDayManager = null;
        private bool _useFindDay = false;
        private int _gapDayBefore = 0;
        private int _gapDayAfter = 0;

        private RegexValuesList _findPrintList = null;
        private PrintManager _printManager = null;
        private Dictionary<PrintType, string> _postTypeDirectories = null;
        private string _defaultPrintDirectory = null;              // .05_new_print
        private string _unknowPrintDirectory = null;               // .06_unknow_print
        //private int _maxFilenameLength = 100;

        public static bool TraceWarning { get { return __traceWarning; } set { __traceWarning = value; } }
        //public int Version { get { return _version; } set { _version = value; } }
        public PrintTitleManager PrintTitleManager { get { return _printTitleManager; } set { _printTitleManager = value; } }
        public bool TrySplitTitle { get { return _trySplitTitle; } set { _trySplitTitle = value; } }
        public FindDayManager FindDayManager { get { return _findDayManager; } set { _findDayManager = value; } }
        public bool UseFindDay { get { return _useFindDay; } set { _useFindDay = value; } }
        public int GapDayBefore { get { return _gapDayBefore; } } // set { _gapDayBefore = value; }
        public int GapDayAfter { get { return _gapDayAfter; } } // set { _gapDayAfter = value; }
        public RegexValuesList FindPrintList { get { return _findPrintList; } set { _findPrintList = value; } }
        public PrintManager PrintManager { get { return _printManager; } set { _printManager = value; } }
        public Dictionary<PrintType, string> PostTypeDirectories { get { return _postTypeDirectories; } set { _postTypeDirectories = value; } }
        public string DefaultPrintDirectory { get { return _defaultPrintDirectory; } set { _defaultPrintDirectory = value; } }
        public string UnknowPrintDirectory { get { return _unknowPrintDirectory; } set { _unknowPrintDirectory = value; } }

        public void SetGapDayBefore(int gap)
        {
            _gapDayBefore = gap;
            if (_printTitleManager != null && _printTitleManager.FindDateManager != null)
                _printTitleManager.FindDateManager.GapDayBefore = gap;
        }

        public void SetGapDayAfter(int gap)
        {
            _gapDayAfter = gap;
            if (_printTitleManager != null && _printTitleManager.FindDateManager != null)
                _printTitleManager.FindDateManager.GapDayAfter = gap;
        }

        public FindPrintInfo Find(string title, PrintType printType = PrintType.Unknow, bool forceSelect = false, Date? expectedDate = null)
        {
            //if (_version == 1)
            //    return _Find(title, printType, forceSelect, expectedDate);
            //else
            return FindPrint.Find(this, title, printType, forceSelect, expectedDate);
        }

        //private FindPrintInfo _Find(string title, PrintType printType = PrintType.Unknow, bool forceSelect = false, Date? expectedDate = null)
        //{
        //    PrintTitleInfo titleInfo = _printTitleManager.GetPrintTitleInfo(title, expectedDate: expectedDate);

        //    FindPrintInfo findPrint = new FindPrintInfo { found = false };
        //    findPrint.sourceTitle = title;
        //    findPrint.printType = printType;
        //    findPrint.date = titleInfo.Date;
        //    findPrint.dateType = titleInfo.DateType;
        //    findPrint.number = titleInfo.Number;
        //    findPrint.special = titleInfo.Special;
        //    findPrint.specialText = titleInfo.SpecialText;
        //    findPrint.remainText = titleInfo.RemainText;
        //    findPrint.titleInfo = titleInfo;

        //    FindText findText = null;

        //    if (titleInfo.FormatedTitle != "")
        //        findText = _findPrintList.Find(titleInfo.FormatedTitle);
        //    else if (titleInfo.Date != null && titleInfo.RemainText != "")
        //        // pour fichier du monde 20150829_QUO.pdf formatedTitle=""
        //        findText = _findPrintList.Find(titleInfo.RemainText);

        //    GetPrintInfo(findPrint, findText);
        //    if (!findPrint.found && forceSelect)
        //    {
        //        ForceSelect(findPrint, titleInfo, printType);
        //    }
        //    if (findPrint.found)
        //        findPrint.file = GetPostTypeDirectory(findPrint.printType) + "\\" + findPrint.file;
        //    return findPrint;
        //}

        //private void ForceSelect(FindPrintInfo findPrint, PrintTitleInfo titleInfo, PrintType printType)
        //{
        //    findPrint.found = true;
        //    findPrint.findPrintType = FindPrintType.UnknowPrint;
        //    findPrint.name = titleInfo.Name;
        //    findPrint.title = titleInfo.FormatedTitle;
        //    string directory = null;
        //    if (printType == PrintType.Print)
        //        directory = _unknowPrintDirectory;

        //    findPrint.file = GetFile(findPrint, directory);
        //}

        //private void GetPrintInfo(FindPrintInfo findPrint, FindText findText)
        //{
        //    if (findText == null || !findText.Found)
        //        return;

        //    findPrint.found = true;
        //    MatchValues matchValues = findText.matchValues;
        //    //MatchValuesInfos matchValues = findText.MatchValues;
        //    findPrint.name = matchValues.Name;

        //    Print print = _printManager[findPrint.name];

        //    if (print == null)
        //    {
        //        findPrint.findPrintType = FindPrintType.PrintType1;
        //        if (matchValues.Attributes.ContainsKey("title"))
        //            findPrint.title = matchValues.Attributes["title"];
        //        if (matchValues.Attributes.ContainsKey("directory"))
        //            findPrint.directory = matchValues.Attributes["directory"];
        //        string directory;
        //        if (findPrint.directory != null)
        //            directory = findPrint.directory;
        //        else
        //            directory = _defaultPrintDirectory;
        //        print = new Print(findPrint.name, findPrint.title, GetDirectory(findPrint, directory));
        //    }
        //    else
        //        findPrint.findPrintType = FindPrintType.PrintType2;

        //    findPrint.print = print;

        //    PrintIssue printIssue = print.NewPrintIssue();

        //    if (findPrint.date != null)
        //    {
        //        printIssue.Date = findPrint.date;
        //        printIssue.DateType = findPrint.dateType;
        //        printIssue.Special = findPrint.special;
        //        printIssue.SpecialText = findPrint.specialText;

        //        if (findPrint.number != null)
        //        {
        //            if (printIssue.CanCalculatePrintNumber())
        //            {
        //                int calculatedPrintNumber = printIssue.Print.GetPrintNumber((Date)printIssue.Date);
        //                if (calculatedPrintNumber != (int)findPrint.number)
        //                {
        //                    if (__traceWarning)
        //                        Trace.WriteLine("warning number in title {0} is different than calculated number {1}", (int)findPrint.number, calculatedPrintNumber);
        //                }
        //            }
        //            // utilise de préférence le no du titre plutot que celui calculé
        //            // sauf pour le monde ex : "Le Monde week-end + Magazine + 3 suppléments du samedi 30 aout 2014" le 3 n'est pas le bon numéro
        //            printIssue.PrintNumber = (int)findPrint.number;
        //        }
        //    }
        //    else if (findPrint.number != null)
        //    {
        //        printIssue.PrintNumber = (int)findPrint.number;
        //        printIssue.Special = findPrint.special;
        //    }

        //    printIssue.TrySetValues(findText.matchValues.GetAllValues());
        //    //printIssue.TrySetValues(findText.MatchValues.GetAllValues());
        //    findPrint.label = printIssue.Label;

        //    findPrint.file = zPath.Combine(print.Directory, zPath.GetFileNameWithoutExtension(printIssue.GetFilename()));
        //}

        //private string GetTitleFileName(FindPrintInfo findPrint)
        //{
        //    string title;
        //    if (findPrint.title != null)
        //        title = findPrint.title;
        //    else
        //        title = findPrint.name;
        //    title = zfile.ReplaceBadFilenameChars(title, "_");

        //    // truncate title to max file length
        //    if (_maxFilenameLength > 0 && title.Length > _maxFilenameLength)
        //        title = title.Substring(0, _maxFilenameLength);

        //    return title;
        //}

        //private string GetDirectory(FindPrintInfo findPrint, string directory)
        //{
        //    return GetDirectory(findPrint, directory, GetTitleFileName(findPrint));
        //}

        //private string GetDirectory(FindPrintInfo findPrint, string directory, string titleFileName)
        //{
        //    if (directory != null)
        //        directory += "\\";
        //    directory += titleFileName + "\\";
        //    return directory;
        //}

        //private string GetFile(FindPrintInfo findPrint, string directory)
        //{
        //    string titleFileName = GetTitleFileName(findPrint);
        //    directory = GetDirectory(findPrint, directory, titleFileName);

        //    if (findPrint.printType == PrintType.Print && findPrint.date == null && findPrint.number == null)
        //    {
        //        if (__traceWarning)
        //            Trace.WriteLine("warning can't find date nor number in \"{0}\"", findPrint.sourceTitle);
        //    }

        //    string label = null;
        //    if (findPrint.specialText != null)
        //        label = zfile.ReplaceBadFilenameChars(findPrint.specialText, "_");
        //    string file = PrintIssue.GetStandardFilename(titleFileName, findPrint.special, findPrint.date, findPrint.dateType, findPrint.number, label);
        //    return directory + file;
        //}

        //private string GetPostTypeDirectory(PrintType postType)
        //{
        //    if (_postTypeDirectories.ContainsKey(postType))
        //        return _postTypeDirectories[postType];
        //    else
        //    {
        //        if (__traceWarning)
        //            Trace.WriteLine("warning directory of post type {0} is not defined use \"UndefinedPostTypeDirectory\" instead");
        //        return "UndefinedPostTypeDirectory";
        //    }
        //}
    }
}
