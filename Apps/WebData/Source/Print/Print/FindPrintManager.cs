using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using pb;
using pb.IO;
using pb.Text;

namespace Print
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

    public class FindPrint
    {
        public bool found;
        // source
        public string sourceTitle;
        public PrintType printType;
        // info
        public string name;
        public string title;
        public string directory;
        public Date? date;
        public DateType dateType = DateType.Unknow;
        public int? number;
        public bool special = false;
        public string specialText;
        public string file;
        public string remainText;
        public Print print;
        public FindPrintType findPrintType;
        public PrintTitleInfo titleInfo;
    }

    public class FindPrintManager
    {
        private static bool __traceWarning = false;
        private PrintTitleManager _printTitleManager = null;
        private RegexValuesList _findPrint = null;
        private PrintManager _printManager = null;
        private Dictionary<PrintType, string> _postTypeDirectories = null;
        private string _defaultPrintDirectory = null;              // .05_new_print
        private string _unknowPrintDirectory = null;               // .06_unknow_print
        private int _maxFilenameLength = 100;

        //public FindPrintManager(IEnumerable<XElement> xelements, PrintTitleManager printTitleManager, PrintManager printManager, bool compileRegex = true,
        //    Dictionary<PrintType, string> postTypeDirectories = null, string defaultPrintDirectory = null, string unknowPrintDirectory = null)
        //{
        //    _findPrint = new RegexValuesList(xelements, compileRegex: compileRegex);
        //    _printTitleManager = printTitleManager;
        //    _printManager = printManager;
        //    _postTypeDirectories = postTypeDirectories;
        //    _defaultPrintDirectory = defaultPrintDirectory;
        //    _unknowPrintDirectory = unknowPrintDirectory;
        //}

        public static bool TraceWarning { get { return __traceWarning; } set { __traceWarning = value; } }
        public PrintTitleManager PrintTitleManager { get { return _printTitleManager; } set { _printTitleManager = value; } }
        public RegexValuesList FindPrint { get { return _findPrint; } set { _findPrint = value; } }
        public PrintManager PrintManager { get { return _printManager; } set { _printManager = value; } }
        public Dictionary<PrintType, string> PostTypeDirectories { get { return _postTypeDirectories; } set { _postTypeDirectories = value; } }
        public string DefaultPrintDirectory { get { return _defaultPrintDirectory; } set { _defaultPrintDirectory = value; } }
        public string UnknowPrintDirectory { get { return _unknowPrintDirectory; } set { _unknowPrintDirectory = value; } }

        // bool isPrint
        public FindPrint Find(string title, PrintType printType = PrintType.Unknow, bool forceSelect = false)
        {
            PrintTitleInfo titleInfo = _printTitleManager.GetPrintTitleInfo(title);

            FindPrint findPrint = new FindPrint { found = false };
            findPrint.sourceTitle = title;
            findPrint.printType = printType;
            findPrint.date = titleInfo.date;
            findPrint.dateType = titleInfo.dateType;
            findPrint.number = titleInfo.number;
            findPrint.special = titleInfo.special;
            findPrint.specialText = titleInfo.specialText;
            findPrint.remainText = titleInfo.remainText;
            findPrint.titleInfo = titleInfo;

            //FindText_old findText = _printRegexList.Find_old(titleInfo.formatedTitle);
            FindText findText = _findPrint.Find(titleInfo.formatedTitle);
            if (findText.found)
            {
                findPrint.found = true;
                //RegexValues regexValues = findText.regexValues;
                MatchValues matchValues = findText.matchValues;
                findPrint.name = matchValues.Name;

                Print print = _printManager[findPrint.name];

                if (print != null)
                {
                    findPrint.findPrintType = FindPrintType.PrintType2;
                    findPrint.print = print;

                        PrintIssue printIssue = print.NewPrintIssue();

                        if (findPrint.date != null)
                        {
                            //PrintIssue printIssue = print.NewPrintIssue((Date)findPrint.date);
                            printIssue.Date = findPrint.date;
                            printIssue.Special = findPrint.special;
                            printIssue.SpecialText = findPrint.specialText;

                            if (findPrint.number != null)
                            {
                                if (printIssue.CanCalculatePrintNumber())
                                {
                                    int calculatedPrintNumber = printIssue.Print.GetPrintNumber((Date)printIssue.Date);
                                    if (calculatedPrintNumber != (int)findPrint.number)
                                    {
                                        if (__traceWarning)
                                            Trace.WriteLine("warning number in title {0} is different than calculated number {1}", (int)findPrint.number, calculatedPrintNumber);
                                    }
                                }
                                // utilise de préférence le no du titre plutot que celui calculé
                                // sauf pour le monde ex : "Le Monde week-end + Magazine + 3 suppléments du samedi 30 aout 2014" le 3 n'est pas le bon numéro
                                printIssue.PrintNumber = (int)findPrint.number;
                            }
                            //findPrint.file = zPath.Combine(print.Directory, zPath.GetFileNameWithoutExtension(printIssue.GetFilename()));
                        }
                        else if (findPrint.number != null)
                        {
                            //PrintIssue printIssue = print.NewPrintIssue((int)findPrint.number);
                            printIssue.PrintNumber = (int)findPrint.number;
                            printIssue.Special = findPrint.special;
                            //findPrint.file = zPath.Combine(print.Directory, zPath.GetFileNameWithoutExtension(printIssue.GetFilename()));
                        }

                        findPrint.file = zPath.Combine(print.Directory, zPath.GetFileNameWithoutExtension(printIssue.GetFilename()));
                }
                else
                {
                    findPrint.findPrintType = FindPrintType.PrintType1;
                    if (matchValues.Attributes.ContainsKey("title"))
                        findPrint.title = matchValues.Attributes["title"];
                    if (matchValues.Attributes.ContainsKey("directory"))
                        findPrint.directory = matchValues.Attributes["directory"];
                    string directory;
                    if (findPrint.directory != null)
                        directory = findPrint.directory;
                    else
                        directory = _defaultPrintDirectory;
                    findPrint.file = GetFile(findPrint, directory);
                }
            }
            else if (forceSelect)
            {
                findPrint.found = true;
                findPrint.findPrintType = FindPrintType.UnknowPrint;
                findPrint.name = titleInfo.name;
                findPrint.title = titleInfo.formatedTitle;
                string directory = null;
                if (printType == PrintType.Print)
                    directory = _unknowPrintDirectory;

                findPrint.file = GetFile(findPrint, directory);
            }
            if (findPrint.found)
                findPrint.file = GetPostTypeDirectory(findPrint.printType) + "\\" + findPrint.file;
            return findPrint;
        }

        private string GetFile(FindPrint findPrint, string directory)
        {
            if (directory != null)
                directory += "\\";

            //directory = GetPostTypeDirectory(findPrint.printType) + "\\" + directory;

            string title;
            if (findPrint.title != null)
                title = findPrint.title;
            else
                title = findPrint.name;
            title = zfile.ReplaceBadFilenameChars(title, "_");

            // truncate title to max file length
            if (_maxFilenameLength > 0 && title.Length > _maxFilenameLength)
                title = title.Substring(0, _maxFilenameLength);

            directory += title + "\\";

            if (findPrint.printType == PrintType.Print && findPrint.date == null && findPrint.number == null)
            {
                if (__traceWarning)
                    Trace.WriteLine("warning can't find date nor number in \"{0}\"", findPrint.sourceTitle);
            }

            string label = null;
            if (findPrint.specialText != null)
                label = zfile.ReplaceBadFilenameChars(findPrint.specialText, "_");
            string file = PrintIssue.GetStandardFilename(title, findPrint.special, findPrint.date, findPrint.dateType, findPrint.number, label);
            return directory + file;
        }

        private string GetPostTypeDirectory(PrintType postType)
        {
            if (_postTypeDirectories.ContainsKey(postType))
                return _postTypeDirectories[postType];
            else
            {
                if (__traceWarning)
                    Trace.WriteLine("warning directory of post type {0} is not defined use \"UndefinedPostTypeDirectory\" instead");
                return "UndefinedPostTypeDirectory";
            }
        }
    }

    //public class FindPrintManager
    //{
    //    private static char[] __trimChars = new char[] { ' ', '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '-', '_' };
    //    private RegexValuesList _printRegexList = null;
    //    private FindDateManager _findDate = null;
    //    private FindNumberManager _findNumber = null;
    //    private RegexValuesList _findSpecial = null;
    //    private string _defaultDirectory = null;

    //    public FindPrintManager(IEnumerable<XElement> xelements, FindDateManager findDateManager, FindNumberManager findNumberManager, RegexValuesList findSpecial, bool compileRegex = true, string defaultDirectory = null)
    //    {
    //        _findDate = findDateManager;
    //        _findNumber = findNumberManager;
    //        _findSpecial = findSpecial;
    //        _printRegexList = new RegexValuesList(xelements, compileRegex: compileRegex);
    //        _defaultDirectory = defaultDirectory;
    //    }

    //    public RegexValuesList PrintRegexList { get { return _printRegexList; } }
    //    public FindDateManager FindDate { get { return _findDate; } }
    //    public FindNumberManager FindNumber { get { return _findNumber; } }
    //    public RegexValuesList FindSpecial { get { return _findSpecial; } }
    //    public string DefaultDirectory { get { return _defaultDirectory; } }

    //    public FindPrint Find(string text)
    //    {
    //        foreach (RegexValues rv in _printRegexList.Values)
    //        {
    //            //rv.Match_old(text);
    //            MatchValues matchValues = rv.Match(text);
    //            //if (rv.Success_old)
    //            if (matchValues.Success)
    //            {
    //                FindPrint findPrint = new FindPrint { found = true, sourceTitle = text, name = rv.Name, title = rv.Attributes.ContainsKey("title") ? rv.Attributes["title"] : null,
    //                    directory = rv.Attributes.ContainsKey("directory") ? rv.Attributes["directory"] : null };
    //                //findPrint.remainText = rv.MatchReplace_old("_");
    //                findPrint.remainText = matchValues.Replace("_");
    //                //FindDate_old findDate = _findDate.Find_old(findPrint.remainText);
    //                FindDate findDate = _findDate.Find(findPrint.remainText);
    //                if (findDate.found)
    //                {
    //                    findPrint.date = findDate.date;
    //                    findPrint.dateType = findDate.dateType;
    //                    //findPrint.remainText = findDate.regexValues.MatchReplace_old("_");
    //                    findPrint.remainText = findDate.matchValues.Replace("_");
    //                }
    //                //FindNumber_old findNumber = _findNumber.Find_old(findPrint.remainText);
    //                FindNumber findNumber = _findNumber.Find(findPrint.remainText);
    //                if (findNumber.found)
    //                {
    //                    findPrint.number = findNumber.number;
    //                    //findPrint.remainText = findNumber.regexValues.MatchReplace_old("_");
    //                    findPrint.remainText = findNumber.matchValues.Replace("_");
    //                }
    //                //FindText_old findSpecial = _findSpecial.Find_old(findPrint.remainText);
    //                FindText findSpecial = _findSpecial.Find(findPrint.remainText);
    //                if (findSpecial.found)
    //                {
    //                    findPrint.special = true;
    //                    //findPrint.remainText = findSpecial.regexValues.MatchReplace_old("_");
    //                    findPrint.remainText = findSpecial.matchValues.Replace("_");
    //                }
    //                findPrint.remainText = findPrint.remainText.Trim(__trimChars);
    //                findPrint.file = GetFile(findPrint);
    //                return findPrint;
    //            }
    //        }
    //        return new FindPrint { found = false, sourceTitle = text, name = null, title = null, directory = null, date = null, file = null, remainText = null };
    //    }

    //    private string GetFile(FindPrint findPrint)
    //    {
    //        string file = "";
    //        if (findPrint.directory != null)
    //            file = findPrint.directory + "\\";
    //        else if (_defaultDirectory != null)
    //            file = _defaultDirectory + "\\";

    //        string title;
    //        if (findPrint.title != null)
    //            title = findPrint.title;
    //        else
    //            title = findPrint.name;
    //        //file += title + "\\" + title;
    //        file += title + "\\";

    //        //if (findPrint.special)
    //        //    file += " - hors-série";

    //        //if (findPrint.date != null)
    //        //{
    //        //    switch (findPrint.dateType)
    //        //    {
    //        //        case DateType.Day:
    //        //            file += string.Format(@" - {0:yyyy-MM-dd}", findPrint.date);
    //        //            break;
    //        //        case DateType.Month:
    //        //            file += string.Format(@" - {0:yyyy-MM}", findPrint.date);
    //        //            break;
    //        //        case DateType.Year:
    //        //            file += string.Format(@" - {0:yyyy}", findPrint.date);
    //        //            break;
    //        //    }
    //        //}

    //        //if (findPrint.number != null)
    //        //{
    //        //    file += string.Format(@" - no {0}", findPrint.number);
    //        //}

    //        string label = null;
    //        if (findPrint.date == null && findPrint.number == null)
    //        {
    //            Trace.WriteLine("warning can't find date nor number in \"{0}\"", findPrint.sourceTitle);
    //            label = zfile.ReplaceBadFilenameChars(findPrint.remainText, "");
    //        }

    //        return file + PrintIssue.GetStandardFilename(title, findPrint.special, findPrint.date, findPrint.dateType, findPrint.number, label);
    //    }

    //    //private void FindDate(SelectPost selectPost)
    //    //{
    //    //    foreach (RegexValues rv in _dateRegexList.Values)
    //    //    {
    //    //        //rv.Match(title);
    //    //        rv.Match(selectPost.remainText);
    //    //        if (rv.Success)
    //    //        {
    //    //            Date date;
    //    //            if (zdate.TryCreateDate(rv.GetValues(), out date))
    //    //            {
    //    //                selectPost.date = date;
    //    //                selectPost.remainText = rv.MatchReplace("_");
    //    //            }
    //    //        }
    //    //    }
    //    //}
    //}
}
