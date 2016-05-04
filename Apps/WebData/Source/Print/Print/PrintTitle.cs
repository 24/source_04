using System;
using System.Text;
using System.Text.RegularExpressions;
using pb;
using pb.Text;

//namespace Print
namespace Download.Print
{
    public class PrintTitleInfo
    {
        public string OriginalTitle = null;
        public string Title = null;
        public string FormatedTitle = null;
        public string Name = null;
        public bool Special = false;
        public MatchValues SpecialMatch = null;
        public string SpecialText = null;
        public int? Number = null;
        public MatchValues NumberMatch = null;
        public Date? Date = null;
        public DateType DateType = DateType.Unknow;
        public MatchValues DateMatch = null;
        //public MatchValues[] DateOtherMatchList = null;
        public string TitleStructure = null;
        public string RemainText = null;
    }

    public class PrintTitle
    {
        private static char[] __trimChars = new char[] { ' ', '-', '/', ',' };
        private static Regex __titleStructureName = new Regex(@"\$\$[a-z]+\$\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex __specialLabel = new Regex(@"\$\$special\$\$(.*?)(?=\$\$|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private static Regex __formatTitle = new Regex(@"^(?:([^\s_]+)(?:[\s_]+|$))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private static Regex __formatTitle = new Regex(@"^(?:([^\s_\.]+)(?:[\s_\.]+|$))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex __formatTitle = new Regex(@"^(?:([^\s_\.\(\)]+)(?:[\s_\.\(\)]+|$))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private PrintTitleManager _printTitleManager = null;
        private bool _splitTitle = false;
        private Date? _expectedDate = null;

        private string _title = null;
        private string _originalTitle = null;
        private string _formatedTitle = null;
        private string _name = null;

        private bool _special = false;
        private MatchValues _specialMatch = null;
        private string _specialText = null;

        private int? _number = null;
        private MatchValues _numberMatch = null;

        private bool _foundDate = false;
        private Date? _date = null;
        private DateType _dateType = DateType.Unknow;
        private MatchValues _dateMatch = null;
        //private MatchValues[] _dateOtherMatchList = null;

        private string _titleStructure = null;
        private string _remainText = null;
        //private string _file = null;

        public static PrintTitleInfo GetPrintTitleInfo(PrintTitleManager printTitleManager, string title, bool splitTitle, Date? expectedDate)
        {
            PrintTitle printTitle = new PrintTitle();
            printTitle._printTitleManager = printTitleManager;
            printTitle._title = title;
            printTitle._splitTitle = splitTitle;
            printTitle._expectedDate = expectedDate;
            printTitle.GetInfo();
            return printTitle.GetResult();
        }

        private void GetInfo()
        {
            // pourquoi split :
            //     "Le Parisien + Votre été du dimanche 24 août 2014"                               date = "été du dimanche 24 août 2014"
            //     "Le Parisien + Votre été du dimanche 24 août 2014"                               date = "été du dimanche 24 août 2014"
            //     "Le Parisien + Votre été (la France en fête) du dimanche 20 juillet 2014"        date = "été"
            //     "Le Monde + Eco&Entreprise + journal de 1994 du mardi 08 avril 2014"             date = "de 1994"
            //     "Le Monde de l'Image 84  - 2013"                                                 date not found
            //     "Le Monde de L'Intelligence 29 - Février-Mars 2013"                              date not found
            //     "Le Monde des Sciences 7 - Février-Mars 2013"                                    date = "7 - Février-Mars 2013"

            _originalTitle = _title;

            _title = ReplaceCharacters(_title);
            _title = GetFormatedText(_title);

            _foundDate = false;

            if (_splitTitle)
                SplitTitle();

            _title = FindSpecial(_title);

            _title = FindNumber(_title);

            if (!_foundDate)
                _title = FindDate(_title);

            _title = FindSpecial2(_title);

            int i = _title.IndexOf("$$");
            if (i != -1)
            {
                _titleStructure = _title.Substring(i);
                _title = _title.Substring(0, i).Trim(__trimChars);
            }
            else
            {
                _titleStructure = null;
            }

            _formatedTitle = GetFormatedText(_title);
            _name = GetName(_formatedTitle);
            if (_titleStructure != null)
                _remainText = __titleStructureName.Replace(_titleStructure, "").Trim(__trimChars);
        }

        private PrintTitleInfo GetResult()
        {
            return new PrintTitleInfo
            {
                OriginalTitle = _originalTitle,
                //title = titleRequest.Title,
                Title = _title,
                FormatedTitle = _formatedTitle,
                Name = _name,
                Special = _special,
                SpecialMatch = _specialMatch,
                SpecialText = _specialText,
                Number = _number,
                NumberMatch = _numberMatch,
                Date = _date,
                DateType = _dateType,
                DateMatch = _dateMatch,
                //DateOtherMatchList = _dateOtherMatchList,
                TitleStructure = _titleStructure,
                RemainText = _remainText,
                //File = _file
            };
        }

        private void SplitTitle()
        {
            // split d'abord avec "du" puis avec "-"
            int i1 = _title.LastIndexOf(" du ", StringComparison.InvariantCultureIgnoreCase);
            int i2 = _title.LastIndexOf("- ");
            int i3 = Math.Max(i1, i2);
            if (i3 != -1)
            {
                string title1 = _title.Substring(0, i3);
                string title2 = _title.Substring(i3);

                //FindDate findDate = _printTitleManager.FindDateManager.Find(title2, _expectedDate);
                //if (findDate.found)
                //{
                //    _date = findDate.date;
                //    _dateType = findDate.dateType;
                //    _dateMatch = findDate.matchValues;
                //    title2 = findDate.matchValues.Replace(" $$date$$ ");
                //    _title = title1 + title2;
                //    _foundDate = true;
                //}
                //_dateOtherMatchList = findDate.matchValuesList;
                title2 = FindDate(title2);
                _title = title1 + title2;
            }

            if (!_foundDate)
            {
                // puis split avec "-"
                i3 = Math.Min(i1, i2);
                i3 = _title.IndexOf("- ");
                if (i3 != -1)
                {
                    // attention i + 1 pour garder un espace en début de chaine
                    //return new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 1));

                    string title1 = _title.Substring(0, i3);
                    string title2 = _title.Substring(i3);

                    //FindDate findDate = _printTitleManager.FindDateManager.Find(title2, _expectedDate);
                    //if (findDate.found)
                    //{
                    //    _date = findDate.date;
                    //    _dateType = findDate.dateType;
                    //    _dateMatch = findDate.matchValues;
                    //    title2 = findDate.matchValues.Replace(" $$date$$ ");
                    //    _title = title1 + title2;
                    //    _foundDate = true;
                    //}
                    //_dateOtherMatchList = findDate.matchValuesList;
                    title2 = FindDate(title2);
                    _title = title1 + title2;
                }
            }
        }

        private string FindSpecial(string title)
        {
            FindText findSpecial = _printTitleManager.FindSpecial.Find(title);
            if (findSpecial.Found)
            {
                _special = true;
                _specialMatch = findSpecial.matchValues;
                title = findSpecial.matchValues.Replace(" $$special$$ ");
            }
            return title;
        }

        private string FindSpecial2(string title)
        {
            Match match = __specialLabel.Match(title);
            if (match.Success)
            {
                _specialText = match.Groups[1].Value.Trim(__trimChars);
                _specialText = GetFormatedText(_specialText);
                title = match.zReplace(title, "");
            }
            return title;
        }

        private string FindNumber(string title)
        {
            FindNumber findNumber = _printTitleManager.FindNumberManager.Find(title);
            if (findNumber.found)
            {
                _number = findNumber.number;
                _numberMatch = findNumber.matchValues;
                title = findNumber.matchValues.Replace(" $$number$$ ");
            }
            return title;
        }

        private string FindDate(string title)
        {
            FindDate findDate = _printTitleManager.FindDateManager.Find(title, _expectedDate);
            if (findDate.Found)
            {
                _date = findDate.Date;
                _dateType = findDate.DateType;
                _dateMatch = findDate.matchValues;
                title = findDate.matchValues.Replace(" $$date$$ ");
                _foundDate = true;
            }
            //_dateOtherMatchList = findDate.matchValuesList;
            return title;
        }

        //private string FindDay(string title)
        //{
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
        //}

        //private bool IsDayValid(int day, Date expectedDate)
        //{
        //    int expectedDay = expectedDate.Day;
        //    if (day == expectedDay)
        //        return true;
        //    int gap = _printTitleManager.GapDayBefore;
        //    if (gap > 0)
        //    {
        //        Date minDate = expectedDate.AddDays(-gap);
        //        int minDay = minDate.Day;
        //        if (minDay < expectedDay)
        //        {
        //            if (day >= minDay && day < expectedDay)
        //                return true;
        //        }
        //        else // previous month
        //        {
        //            if (day >=1 && day < expectedDay)
        //                return true;
        //        }
        //    }
        //}

        //private string GetFile()
        //{
        //    //string file = PrintIssue.GetStandardFilename(_formatedTitle, _special, _date, _dateType, _number, _specialText);
        //    //file = zfile.ReplaceBadFilenameChars(file, "_");
        //    //return _printDirectory + "\\" + file;
        //    return _printDirectory + "\\" + zfile.ReplaceBadFilenameChars(PrintIssue.GetStandardFilename(_formatedTitle, _special, _date, _dateType, _number, _specialText), "_");
        //}

        private static string ReplaceCharacters(string text)
        {
            //text = text.Replace("&amp;", " et ");
            text = zstr.DecodeHtmlSpecialCharacters(text);
            text = text.Replace("&", " et ");
            // apostrophe 2018 to 201B
            text = text.Replace('\u2019', '\'');
            // ¨
            text = text.Replace('¨', ' ');
            return text;
        }

        private static string GetFormatedText(string text)
        {
            if (text.StartsWith("z.", StringComparison.InvariantCultureIgnoreCase))
                text = text.Substring(2);
            Match match = __formatTitle.Match(text);
            if (match.Success)
            {
                StringBuilder sb = new StringBuilder();
                bool first = true;
                foreach (Capture capture in match.Groups[1].Captures)
                {
                    if (first)
                    {
                        sb.Append(capture.Value.zToFirstCharUpper());
                        first = false;
                    }
                    else
                    {
                        sb.Append(' ');
                        sb.Append(capture.Value.ToLowerInvariant());
                    }
                }
                text = sb.ToString();
            }
            else
                Trace.WriteLine("error unable to format title \"{0}\"", text);
            return text;
        }

        private static string GetName(string formatedTitle)
        {
            string name = formatedTitle.zToStringWithoutAccent();
            name = name.ToLowerInvariant();
            name = name.Replace(' ', '_');
            name = name.Replace('\'', '_');
            return name;
        }
    }
}
