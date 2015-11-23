using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using pb;
using pb.Data.Mongo;
using pb.IO;
using pb.Text;

/***************************************************************************************************************************************************************
 *   add country manager : ex : / France "Psychologies N° 343 - Septembre 2014 / France"
 *   a letter after date : "Le Parisien + Journal de Paris du Lundi 25 Août 2014j"
***************************************************************************************************************************************************************/

namespace Print
{
    public class PrintTitleInfo
    {
        public string originalTitle = null;
        public string title = null;
        public string formatedTitle = null;
        public string name = null;
        public bool special = false;
        public MatchValues specialMatch = null;
        public string specialText = null;
        public int? number = null;
        public MatchValues numberMatch = null;
        public Date? date = null;
        public DateType dateType = DateType.Unknow;
        public MatchValues dateMatch = null;
        public MatchValues[] dateOtherMatchList = null;
        public string titleStructure = null;
        public string remainText = null;
        public string file = null;
    }

    public class PrintTitleRequest
    {
        public string originalTitle = null;
        public PrintSplitedTitle splitedTitle;
        public string title = null;
        public string formatedTitle = null;
        public string name = null;
        public bool special = false;
        public MatchValues specialMatch = null;
        public string specialText = null;
        public int? number = null;
        public MatchValues numberMatch = null;
        public Date? date = null;
        public DateType dateType = DateType.Unknow;
        public MatchValues dateMatch = null;
        public MatchValues[] dateOtherMatchList = null;
        public string titleStructure = null;
        public string remainText = null;
        public string file = null;
    }

    public class PrintSplitedTitle
    {
        private string _titlePart1 = null;
        private string _titlePart2 = null;

        public PrintSplitedTitle(string titlePart1, string titlePart2 = null)
        {
            _titlePart1 = titlePart1;
            _titlePart2 = titlePart2;
        }

        public string titlePart1 { get { return _titlePart1; } set { _titlePart1 = value; } }

        public string titlePart2
        {
            // if _titlePart2 is null use _titlePart1
            get { if (_titlePart2 != null) return _titlePart2; else return _titlePart1; }
            set { if (_titlePart2 != null) _titlePart2 = value; else _titlePart1 = value; }
        }

        public string realTitlePart2 { get { return _titlePart2; } set { _titlePart2 = value; } }

        //public string concatenatedTitle { get { return _titlePart1 + _titlePart2; } }
    }

    public class PrintTitleManager
    {
        // '\t', '\n', '\r', ',', '»', '&', '+', '/', '|', '*', '=', '»', '_'
        private static char[] __trimChars = new char[] { ' ', '-', '/', ',' };
        //private static Regex __rgTitleStructure = new Regex(@"\$\$([a-z]+)\$\$(.*?)(?=\$\$|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex __rgTitleStructureName = new Regex(@"\$\$[a-z]+\$\$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex __rgSpecialLabel = new Regex(@"\$\$special\$\$(.*?)(?=\$\$|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private static Regex __rgFormatTitle = new Regex(@"^(?:([^\s\-\+_]+)(?:\s+|$))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // \-\+  garder le plus pour Santé+, pas d'intéret d'enlever le -
        private static Regex __rgFormatTitle = new Regex(@"^(?:([^\s_]+)(?:[\s_]+|$))*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //private FindDateManager_v1 _findDateManager_v1 = null;
        private FindDateManager _findDateManager = null;
        private FindNumberManager _findNumberManager = null;
        private RegexValuesList _findSpecial = null;
        private string _printDirectory = null;
        private bool _splitTitle = false;

        //public PrintTitleManager(FindDateManager_v1 findDateManager_v1, FindNumberManager findNumberManager, RegexValuesList findSpecial, string printDirectory)
        //{
        //    _findDateManager_v1 = findDateManager_v1;
        //    _findNumber = findNumberManager;
        //    _findSpecial = findSpecial;
        //    _printDirectory = printDirectory;
        //}

        //public PrintTitleManager(FindDateManager findDateManager, FindNumberManager findNumberManager, RegexValuesList findSpecial, string printDirectory, bool splitTitle = true)
        //{
        //    _findDateManager = findDateManager;
        //    _findNumber = findNumberManager;
        //    _findSpecial = findSpecial;
        //    _printDirectory = printDirectory;
        //    _splitTitle = splitTitle;
        //}

        public FindDateManager FindDateManager { get { return _findDateManager; } set { _findDateManager = value; } }
        public FindNumberManager FindNumberManager { get { return _findNumberManager; } set { _findNumberManager = value; } }
        public RegexValuesList FindSpecial { get { return _findSpecial; } set { _findSpecial = value; } }
        public string PrintDirectory { get { return _printDirectory; } set { _printDirectory = value; } }
        public bool SplitTitle { get { return _splitTitle; } set { _splitTitle = value; } }

        public PrintTitleInfo GetPrintTitleInfo(string title, Date? expectedDate = null)
        {
            //if (_findDateManager_new == null)
            //    return _GetPrintTitleInfo_v1(title);
            //else
            //    return _GetPrintTitleInfo(title, _splitTitle);
            return _GetPrintTitleInfo(title, _splitTitle, expectedDate);
        }

        //private PrintTitleInfo _GetPrintTitleInfo_v1(string title)
        //{
        //    PrintTitleInfo titleInfo = new PrintTitleInfo();
        //    titleInfo.originalTitle = title;
        //    string titleStructure = title;

        //    FindText findSpecial = _findSpecial.Find(titleStructure);
        //    if (findSpecial.found)
        //    {
        //        titleInfo.special = true;
        //        titleInfo.specialMatch = findSpecial.matchValues;
        //        titleStructure = findSpecial.matchValues.Replace(" $$special$$ ");
        //    }

        //    FindNumber findNumber = _findNumber.Find(titleStructure);
        //    if (findNumber.found)
        //    {
        //        titleInfo.number = findNumber.number;
        //        titleInfo.numberMatch = findNumber.matchValues;
        //        titleStructure = findNumber.matchValues.Replace(" $$number$$ ");
        //    }

        //    FindDate findDate = _findDateManager_v1.Find(titleStructure);
        //    if (findDate.found)
        //    {
        //        titleInfo.date = findDate.date;
        //        titleInfo.dateType = findDate.dateType;
        //        titleInfo.dateMatch = findDate.matchValues;
        //        titleStructure = findDate.matchValues.Replace(" $$date$$ ");
        //    }
        //    titleInfo.dateOtherMatchList = findDate.matchValuesList;

        //    Match match = __rgSpecialLabel.Match(titleStructure);
        //    if (match.Success)
        //    {
        //        titleInfo.specialText = match.Groups[1].Value.Trim(__trimChars);
        //        titleInfo.specialText = GetFormatedText(titleInfo.specialText);
        //        titleStructure = match.zReplace(titleStructure, "");
        //    }

        //    int i = titleStructure.IndexOf("$$");
        //    if (i != -1)
        //    {
        //        titleInfo.title = titleStructure.Substring(0, i).Trim(__trimChars);
        //        titleStructure = titleStructure.Substring(i);
        //    }
        //    else
        //        titleInfo.title = titleStructure;

        //    titleInfo.titleStructure = titleStructure;
        //    titleInfo.formatedTitle = GetFormatedText(titleInfo.title);
        //    titleInfo.name = GetName(titleInfo.formatedTitle);
        //    titleInfo.remainText = __rgTitleStructureName.Replace(titleStructure, "").Trim(__trimChars);

        //    titleInfo.file = GetFile(titleInfo);

        //    return titleInfo;
        //}

        /// not used
        //private PrintTitleInfo _GetPrintTitleInfo_v2(string title)
        //{
        //    PrintTitleRequest titleRequest = new PrintTitleRequest();

        //    titleRequest.originalTitle = title;

        //    PrintSplitedTitle splitedTitle = SplitTitle(title);

        //    FindText findSpecial = _findSpecial.Find(splitedTitle.titlePart1);
        //    if (findSpecial.found)
        //    {
        //        titleRequest.special = true;
        //        splitedTitle.titlePart1 = findSpecial.matchValues.Replace(" $$special$$ ");
        //    }

        //    FindNumber findNumber = _findNumber.Find(splitedTitle.titlePart1);
        //    if (findNumber.found)
        //    {
        //        titleRequest.number = findNumber.number;
        //        splitedTitle.titlePart1 = findNumber.matchValues.Replace(" $$number$$ ");
        //    }

        //    FindDate findDate = _findDateManager_new.Find(splitedTitle.titlePart2);
        //    if (findDate.found)
        //    {
        //        titleRequest.date = findDate.date;
        //        titleRequest.dateType = findDate.dateType;
        //        splitedTitle.titlePart2 = findDate.matchValues.Replace(" $$date$$ ");
        //    }

        //    Match match = __rgSpecialLabel.Match(splitedTitle.titlePart1);
        //    if (match.Success)
        //    {
        //        titleRequest.specialText = match.Groups[1].Value.Trim(__trimChars);
        //        titleRequest.specialText = GetFormatedText(titleRequest.specialText);
        //        splitedTitle.titlePart1 = match.zReplace(splitedTitle.titlePart1, "");
        //    }

        //    //Trace.WriteLine("titleRequest :");
        //    //Trace.WriteLine(titleRequest.zToJson());
        //    //Trace.WriteLine("splitedTitle :");
        //    //Trace.WriteLine(splitedTitle.zToJson());

        //    string concatenatedTitle = splitedTitle.titlePart1;
        //    if (!string.IsNullOrEmpty(splitedTitle.realTitlePart2))
        //        concatenatedTitle += " - " + splitedTitle.realTitlePart2;
        //    int i = concatenatedTitle.IndexOf("$$");
        //    if (i != -1)
        //    {
        //        titleRequest.title = concatenatedTitle.Substring(0, i).Trim(__trimChars);
        //        concatenatedTitle = concatenatedTitle.Substring(i);
        //    }
        //    else
        //    {
        //        titleRequest.title = concatenatedTitle;
        //        concatenatedTitle = null;
        //    }

        //    titleRequest.titleStructure = concatenatedTitle;
        //    titleRequest.formatedTitle = GetFormatedText(titleRequest.title);
        //    titleRequest.name = GetName(titleRequest.formatedTitle);
        //    if (concatenatedTitle != null)
        //        titleRequest.remainText = __rgTitleStructureName.Replace(concatenatedTitle, "").Trim(__trimChars);

        //    titleRequest.file = GetFile(titleRequest);

        //    PrintTitleInfo titleInfo = new PrintTitleInfo
        //        {
        //            originalTitle = titleRequest.originalTitle,
        //            title = titleRequest.title,
        //            formatedTitle = titleRequest.formatedTitle,
        //            name = titleRequest.name,
        //            special = titleRequest.special,
        //            specialText = titleRequest.specialText,
        //            number = titleRequest.number,
        //            date = titleRequest.date,
        //            dateType = titleRequest.dateType,
        //            titleStructure = titleRequest.titleStructure,
        //            remainText = titleRequest.remainText,
        //            file = titleRequest.file
        //        };
        //    return titleInfo;
        //}

        /// new split
        private PrintTitleInfo _GetPrintTitleInfo(string title, bool splitTitle, Date? expectedDate)
        {
            // pourquoi split :
            //     "Le Parisien + Votre été du dimanche 24 août 2014"                               date = "été du dimanche 24 août 2014"
            //     "Le Parisien + Votre été du dimanche 24 août 2014"                               date = "été du dimanche 24 août 2014"
            //     "Le Parisien + Votre été (la France en fête) du dimanche 20 juillet 2014"        date = "été"
            //     "Le Monde + Eco&Entreprise + journal de 1994 du mardi 08 avril 2014"             date = "de 1994"
            //     "Le Monde de l'Image 84  - 2013"                                                 date not found
            //     "Le Monde de L'Intelligence 29 - Février-Mars 2013"                              date not found
            //     "Le Monde des Sciences 7 - Février-Mars 2013"                                    date = "7 - Février-Mars 2013"

            PrintTitleRequest titleRequest = new PrintTitleRequest();

            titleRequest.originalTitle = title;

            title = ReplaceCharacters(title);

            // new le 11/08/2015
            //title = GetFormatedText(title);

            bool foundDate = false;

            if (splitTitle)
            {
                // split d'abord avec "du" puis avec "-"
                //int i1 = title.IndexOf(" du ", StringComparison.InvariantCultureIgnoreCase);
                //int i2 = title.IndexOf("- ");
                int i1 = title.LastIndexOf(" du ", StringComparison.InvariantCultureIgnoreCase);
                int i2 = title.LastIndexOf("- ");
                int i3 = Math.Max(i1, i2);
                if (i3 != -1)
                {
                    // attention i + 3 pour garder un espace en début de chaine
                    //PrintSplitedTitle splitedTitle = new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 3));
                    //return splitedTitle;

                    string title1 = title.Substring(0, i3);
                    string title2 = title.Substring(i3);

                    FindDate findDate = _findDateManager.Find(title2, expectedDate);
                    if (findDate.found)
                    {
                        titleRequest.date = findDate.date;
                        titleRequest.dateType = findDate.dateType;
                        titleRequest.dateMatch = findDate.matchValues;
                        title2 = findDate.matchValues.Replace(" $$date$$ ");
                        title = title1 + title2;
                        foundDate = true;
                    }
                    titleRequest.dateOtherMatchList = findDate.matchValuesList;
                }

                if (!foundDate)
                {
                    // puis split avec "-"
                    i3 = Math.Min(i1, i2);
                    i3 = title.IndexOf("- ");
                    if (i3 != -1)
                    {
                        // attention i + 1 pour garder un espace en début de chaine
                        //return new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 1));

                        string title1 = title.Substring(0, i3);
                        string title2 = title.Substring(i3);

                        FindDate findDate = _findDateManager.Find(title2, expectedDate);
                        if (findDate.found)
                        {
                            titleRequest.date = findDate.date;
                            titleRequest.dateType = findDate.dateType;
                            titleRequest.dateMatch = findDate.matchValues;
                            title2 = findDate.matchValues.Replace(" $$date$$ ");
                            title = title1 + title2;
                            foundDate = true;
                        }
                        titleRequest.dateOtherMatchList = findDate.matchValuesList;
                    }
                }
            }

            FindText findSpecial = _findSpecial.Find(title);
            if (findSpecial.found)
            {
                titleRequest.special = true;
                titleRequest.specialMatch = findSpecial.matchValues;
                title = findSpecial.matchValues.Replace(" $$special$$ ");
            }

            FindNumber findNumber = _findNumberManager.Find(title);
            if (findNumber.found)
            {
                titleRequest.number = findNumber.number;
                titleRequest.numberMatch = findNumber.matchValues;
                title = findNumber.matchValues.Replace(" $$number$$ ");
            }

            if (!foundDate)
            {
                FindDate findDate = _findDateManager.Find(title, expectedDate);
                //Trace.WriteLine("PrintTitleManager._GetPrintTitleInfo() : _findDateManager.Find(\"{0}\")", title);
                //Trace.WriteLine(findDate.zToJson());
                if (findDate.found)
                {
                    titleRequest.date = findDate.date;
                    titleRequest.dateType = findDate.dateType;
                    titleRequest.dateMatch = findDate.matchValues;
                    title = findDate.matchValues.Replace(" $$date$$ ");
                }
                titleRequest.dateOtherMatchList = findDate.matchValuesList;
            }

            Match match = __rgSpecialLabel.Match(title);
            if (match.Success)
            {
                titleRequest.specialText = match.Groups[1].Value.Trim(__trimChars);
                titleRequest.specialText = GetFormatedText(titleRequest.specialText);
                title = match.zReplace(title, "");
            }

            //Trace.WriteLine("titleRequest :");
            //Trace.WriteLine(titleRequest.zToJson());
            //Trace.WriteLine("splitedTitle :");
            //Trace.WriteLine(splitedTitle.zToJson());

            //string concatenatedTitle = splitedTitle.titlePart1;
            //if (!string.IsNullOrEmpty(splitedTitle.realTitlePart2))
            //    concatenatedTitle += " - " + splitedTitle.realTitlePart2;
            int i = title.IndexOf("$$");
            if (i != -1)
            {
                titleRequest.title = title.Substring(0, i).Trim(__trimChars);
                title = title.Substring(i);
            }
            else
            {
                titleRequest.title = title;
                title = null;
            }

            titleRequest.titleStructure = title;
            titleRequest.formatedTitle = GetFormatedText(titleRequest.title);
            titleRequest.name = GetName(titleRequest.formatedTitle);
            if (title != null)
                titleRequest.remainText = __rgTitleStructureName.Replace(title, "").Trim(__trimChars);

            titleRequest.file = GetFile(titleRequest);

            PrintTitleInfo titleInfo = new PrintTitleInfo
            {
                originalTitle = titleRequest.originalTitle,
                title = titleRequest.title,
                formatedTitle = titleRequest.formatedTitle,
                name = titleRequest.name,
                special = titleRequest.special,
                specialMatch = titleRequest.specialMatch,
                specialText = titleRequest.specialText,
                number = titleRequest.number,
                numberMatch = titleRequest.numberMatch,
                date = titleRequest.date,
                dateType = titleRequest.dateType,
                dateMatch = titleRequest.dateMatch,
                dateOtherMatchList = titleRequest.dateOtherMatchList,
                titleStructure = titleRequest.titleStructure,
                remainText = titleRequest.remainText,
                file = titleRequest.file
            };
            return titleInfo;
        }

        //private static PrintSplitedTitle SplitTitle_v1(string title)
        //{
        //    int i = title.IndexOf("- ");
        //    if (i != -1)
        //    {
        //        // attention i + 1 pour garder un espace en début de chaine
        //        return new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 1));
        //    }
        //    else
        //    {
        //        i = title.IndexOf(" du ", StringComparison.InvariantCultureIgnoreCase);
        //        if (i != -1)
        //        {
        //            // attention i + 3 pour garder un espace en début de chaine
        //            return new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 3));
        //        }
        //    }
        //    return new PrintSplitedTitle(title);
        //}

        /// not used
        private static PrintSplitedTitle _SplitTitle(string title)
        {
            //int i = title.IndexOf("- ");
            //if (i != -1)
            //{
            //    // attention i + 1 pour garder un espace en début de chaine
            //    return new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 1));
            //}
            //else
            //{
            //    i = title.IndexOf(" du ", StringComparison.InvariantCultureIgnoreCase);
            //    if (i != -1)
            //    {
            //        // attention i + 3 pour garder un espace en début de chaine
            //        return new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 3));
            //    }
            //}

            // split d'abord avec "du" puis avec "-"
            int i = title.IndexOf(" du ", StringComparison.InvariantCultureIgnoreCase);
            if (i != -1)
            {
                // attention i + 3 pour garder un espace en début de chaine
                PrintSplitedTitle splitedTitle = new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 3));
                return splitedTitle;
            }
            i = title.IndexOf("- ");
            if (i != -1)
            {
                // attention i + 1 pour garder un espace en début de chaine
                return new PrintSplitedTitle(title.Substring(0, i), title.Substring(i + 1));
            }
            return new PrintSplitedTitle(title);
        }

        private static string ReplaceCharacters(string text)
        {
            //text = text.Replace("&amp;", " et ");
            text = zstr.DecodeHtmlSpecialCharacters(text);
            text = text.Replace("&", " et ");
            // apostrophe 2018 to 201B
            text = text.Replace('\u2019', '\'');
            return text;
        }

        private static string GetFormatedText(string text)
        {
            Match match = __rgFormatTitle.Match(text);
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

        //private string GetFile(PrintTitleInfo titleInfo)
        //{
        //    //string file = "";
        //    //file = _printDirectory + "\\";
        //    //return file + PrintIssue.GetStandardFilename(titleInfo.formatedTitle, titleInfo.special, titleInfo.date, titleInfo.dateType, titleInfo.number, titleInfo.specialText);
        //    string file = PrintIssue.GetStandardFilename(titleInfo.formatedTitle, titleInfo.special, titleInfo.date, titleInfo.dateType, titleInfo.number, titleInfo.specialText);
        //    file = zfile.ReplaceBadFilenameChars(file, "_");
        //    return _printDirectory + "\\" + file;
        //}

        private string GetFile(PrintTitleRequest titleRequest)
        {
            //string file = "";
            //file = _printDirectory + "\\";
            //return file + PrintIssue.GetStandardFilename(titleRequest.formatedTitle, titleRequest.special, titleRequest.date, titleRequest.dateType, titleRequest.number, titleRequest.specialText);
            string file = PrintIssue.GetStandardFilename(titleRequest.formatedTitle, titleRequest.special, titleRequest.date, titleRequest.dateType, titleRequest.number, titleRequest.specialText);
            file = zfile.ReplaceBadFilenameChars(file, "_");
            return _printDirectory + "\\" + file;
        }
    }
}
