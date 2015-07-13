using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

// Extract Text from PDF in C# (100% .NET) http://www.codeproject.com/Articles/14170/Extract-Text-from-PDF-in-C-100-NET
// iTextSharp http://itextpdf.com/
// iTextSharp http://sourceforge.net/projects/itextsharp/
// Prise en main rapide de ITextSharp http://dotnet.developpez.com/articles/itextsharp/

namespace Print.le_monde
{
    static partial class w
    {
        public static RunSource wr = null;

        public static void Init()
        {
            wr = RunSource.CurrentRunSource;
            wr.InitConfig("le_monde");
        }

        public static void Test_01()
        {
            wr.Print("Test_01");
        }

        public static void Test_02()
        {
            Date date = new Date(2012, 12, 10);
            for (int i = 0; i < 100; i++)
            {
                //wr.Print("{0,-8} {1} {2,6}", GetWeekDayName(date.DayOfWeek), date, GetLeMondePrintNumber(date));
                wr.Print("{0,-10:dddd} {1:yyyy-MM-dd} {2,6}", date, date, le_monde.GetPrintNumber(date));
                date = date.AddDays(-1);
            }
        }

        public static void Test_Regex_01()
        {
            //Test_Regex("([0-9]{4})([0-9]{2})([0-9]{2})_QUO.pdf", RegexOptions.IgnoreCase, "20110820_QUO.pdf");
            // LMD20111013 [WwW.VosBooks.com].pdf
            // LMD-20111014.pdf
            //Test_Regex(@"(?:le monde|lmd)[ -]*([0-9]{4})\.?([0-9]{2})\.?([0-9]{2}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled, "LMD20111013 [WwW.VosBooks.com].pdf");
            //string month = "janvier|f[ée]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[ée]cembre";
            //string month = "janvier|f[ée]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|oct(?:obre)?|novembre|d[ée]cembre";
            //Test_Regex(@"le[ \-_\.]monde[ \-_\.]+([0-9]{1,2})[ \-_\.](" + month + @")[ \-_\.]([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled, "Le.Monde.10.Juillet.2012.French.pdf");
            //Test_Regex(@"le[ \-_\.]monde[ \-_\.]+(?:du[ \-_\.])?([0-9]{1,2})[ \-_\.](" + month + @")[ \-_\.]([0-9]{2}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled, "Le-Monde-du-17-Oct-09.pdf");
            //Test_Regex(@"le[ \-_\.]?monde[ \-_\.]+(?:(?:edition|quotidien)[ \-_\.])?(?:du[ \-_\.])?([0-9]{2})(?:\-[0-9]{2})?[ \-_\.]?([0-9]{2})[ \-_\.]?([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled, "Le Monde Quotidien_08-09_11-2009.pdf");
            // Le Monde Quo. du 20 et 21-09-2009_pdf.pdf
            Test_Regex(@"le[ \-_\.]?monde[ \-_\.]+(?:(?:edition|quo(?:tidien)?)[ \-_\.]+)?(?:du[ \-_\.])?([0-9]{2})(?:[ \-_\.](?:et[ \-_\.])?[0-9]{2})?[ \-_\.]?([0-9]{2})[ \-_\.]?([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled, "Le Monde Quo. du 20 et 21-09-2009_pdf.pdf");
        }

        public static void Test_Regex(string pattern, RegexOptions options, string input)
        {
            Regex rg = new Regex(pattern, options);
            Match match = rg.Match(input);
            //wr.View(match);
            wr.Print("regex : \"{0}\"", pattern);
            wr.Print("input : \"{0}\"", input);
            wr.Print("match : {0}", match.Success);
            int i = 0;
            foreach (Group group in match.Groups)
            {
                wr.Print("group {0} : {1}", i++, group.Value);
            }
        }

        public static void Test_ControlLeMondePrintNumber()
        {
            bool r = true;
            if (!ControlLeMondePrintNumber("2008-08-31", 19781)) r = false; // ok  19781
            if (!ControlLeMondePrintNumber("2010-03-05", 20252)) r = false; // ok  20252
            if (!ControlLeMondePrintNumber("2011-04-12", 20596)) r = false; // ok  20596
            if (!ControlLeMondePrintNumber("2011-04-30", 20612)) r = false; // ok  20612 à vérifier samedi
            if (!ControlLeMondePrintNumber("2011-05-01", 20613)) r = false; // ok  20613 à vérifier dimanche
            if (!ControlLeMondePrintNumber("2011-05-03", 20614)) r = false; // ok  20614 à vérifier mardi
            if (!ControlLeMondePrintNumber("2011-05-06", 20617)) r = false; // ok  20617 à vérifier
            if (!ControlLeMondePrintNumber("2011-05-08", 20619)) r = false; // ok  20619 à vérifier
            if (!ControlLeMondePrintNumber("2011-05-13", 20623)) r = false; // ok  20623 à vérifier
            if (!ControlLeMondePrintNumber("2011-07-31", 20691)) r = false; // ok  20691
            if (!ControlLeMondePrintNumber("2011-08-31", 20717)) r = false; // ok  20717
            if (!ControlLeMondePrintNumber("2012-01-19", 20838)) r = false; // ok  20838
            if (!ControlLeMondePrintNumber("2012-02-25", 20870)) r = false; // ok  20870
            if (!ControlLeMondePrintNumber("2012-04-11", 20909)) r = false; // ok  20909
            if (!ControlLeMondePrintNumber("2012-04-18", 20915)) r = false; // ok  20915
            if (!ControlLeMondePrintNumber("2012-04-22", 20919)) r = false; // ok  20919
            if (!ControlLeMondePrintNumber("2012-04-29", 20925)) r = false; // ok  20925
            if (!ControlLeMondePrintNumber("2012-05-02", 20926)) r = false; // ok  20926
            if (!ControlLeMondePrintNumber("2012-05-11", 20934)) r = false; // ok  20934
            if (!ControlLeMondePrintNumber("2012-05-29", 20949)) r = false; // ok  20949
            if (!ControlLeMondePrintNumber("2012-07-10", 20985)) r = false; // ok  20985
            if (!ControlLeMondePrintNumber("2012-07-19", 20993)) r = false; // ok  20993
            if (!ControlLeMondePrintNumber("2012-09-11", 21039)) r = false; // ok  21039
            if (!r) wr.Print("Control failed"); else wr.Print("Control ok");
        }

        public static bool ControlLeMondePrintNumber(string date, int no)
        {
            Regex re = new Regex("([0-9]{4})-([0-9]{2})-([0-9]{2})", RegexOptions.Compiled);
            Match match = re.Match(date);
            if (!match.Success)
            {
                wr.Print("unknow date \"{0}\"", date);
                return false;
            }
            Date date2 = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            int no2 = le_monde.GetPrintNumber(date2);
            string s;
            if (no == no2) s = "ok"; else s = "failed";
            wr.Print("le monde du {0,-10:dddd} {1:yyyy-MM-dd} no {2,6} no calculé {3,6} : {4}", date2, date2, no, no2, s);
            return no == no2;
        }

        public static void Test_GetLeMondePrintNumber(string date)
        {
            // Le monde - 2012-07-19 - no 20993.pdf : ok
            Regex re = new Regex("([0-9]{4})-([0-9]{2})-([0-9]{2})", RegexOptions.Compiled);
            Match match = re.Match(date);
            if (!match.Success)
            {
                wr.Print("unknow date \"{0}\"", date);
                return;
            }
            //Date date2 = new Date(2012, 07, 19);
            Date date2 = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            wr.Print("{0,-10:dddd} {1:yyyy-MM-dd} {2,6}", date2, date2, le_monde.GetPrintNumber(date2));
        }

        public static void Test_RenameFilesQuotidien()
        {
            //string dir = @"c:\pib\media\print\Le monde\_new\new3";
            string dir = @"c:\pib\media\print\Le monde\_new\new\quo2";
            //string dir = @"c:\pib\media\print\Le monde\_new\new3";
            RenameFilesQuotidien(dir);
        }

        public static void Test_RenameFilesOther()
        {
            //string dir = @"c:\pib\media\print\Le monde\_new\new3";
            //string dir = @"c:\pib\media\print\Le monde\_new\new\autre";
            //string dir = @"c:\pib\media\print\Le monde\_new\new3";
            string dir = @"c:\pib\media\print\.01_quotidien\Le monde\_new\new\autre";
            RenameFilesOther(dir);
        }

        public static void RenameFilesOther(string dir)
        {
            // ARG : argent
            // ARH : culture
            // AUT : festival
            // DOS : dossier
            // EDU : éducation
            // LIV : des livres
            // MAG : magazine
            // MDE : économie
            // MDV : mode
            // NYT : the newyork times
            // PEH : géo-politique
            // SCH : science
            // SPH : sport
            // STY : style
            // TEL : tv
            Regex re_eco = new Regex(@"([0-9]{4})([0-9]{2})([0-9]{2})[_ ].*(arg|arh|aut|dos|edu|liv|mag|mde|mdv|nyt|peh|sch|sph|sty|tel)(?:[_ ].*)?\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            string[] files = Directory.GetFiles(dir, "*.pdf");
            foreach (string file in files)
            {
                bool success = false;
                string type = null;
                Date date = Date.MinValue;
                Match match = re_eco.Match(file);
                if (match.Success)
                {
                    try
                    {
                        date = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                        type = GetType(match.Groups[4].Value);
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                        {
                            success = true;
                        }
                    }
                    catch
                    {
                    }
                }
                if (success)
                {
                    //date = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                    int no = le_monde.GetPrintNumber(date);
                    // Le monde - 2012-12-01 - no 21109.pdf
                    // Le monde économie - 2012-12-01 - no 21109.pdf
                    //string file2 = string.Format("Le monde {0} - {1:yyyy-MM-dd} - no {2}", type, date, no);
                    string file2 = string.Format("Le monde - {0:yyyy-MM-dd} - no {1} -{2}", date, no, type);
                    file2 = cu.PathSetFile(file, file2);
                    if (file != file2)
                    {
                        string file3 = null;
                        for (int i = 2; File.Exists(file2); i++)
                        {
                            if (file3 == null) file3 = Path.GetFileNameWithoutExtension(file2);
                            file2 = cu.PathSetFile(file2, file3 + "_" + i.ToString());
                        }
                        wr.Print("rename \"{0}\" to \"{1}\"", Path.GetFileName(file), Path.GetFileName(file2));
                        File.Move(file, file2);
                    }
                    else
                        wr.Print("no change \"{0}\"", Path.GetFileName(file));
                }
                else
                    wr.Print("unknow file \"{0}\"", Path.GetFileName(file));

            }
        }

        public static void RenameFilesQuotidien(string dir)
        {
            // re1 : 20110820_QUO.pdf
            // re1 : 20090912 Le Monde QUO.pdf
            Regex re1 = new Regex(@"([0-9]{4})([0-9]{2})([0-9]{2})[_ ].*quo([_ ].*)?\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // re2 : 20121107_Le_Monde.pdf
            Regex re2 = new Regex(@"([0-9]{4})([0-9]{2})([0-9]{2})_le_monde\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // re3 : Le monde - 2011-08-20 - no 20707.pdf
            Regex re3 = new Regex(@"le monde - ([0-9]{4})-([0-9]{2})-([0-9]{2}) -.*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // Le Monde - 2012.01.24 (QUO).pdf
            // LMD20111013 [WwW.VosBooks.com].pdf
            // LMD-20111014.pdf
            Regex re4 = new Regex(@"(?:le monde|lmd)[ \-]*([0-9]{4})\.?([0-9]{2})\.?([0-9]{2}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // LMD 12102011[WwW.VosBooks.com].pdf
            Regex re9 = new Regex(@"(?:le monde|lmd)[ \-]*([0-9]{2})\.?([0-9]{2})\.?([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // Le Monde 03102009.pdf
            // Le_Monde_04092009.pdf
            // Le Monde du 05.03.2009.pdf
            // Le Monde du 07-08.02.2010.pdf
            // Le Monde du 04.03.2009.pdf
            // LeMonde_25022010.pdf
            // Le Monde Quotidien_08-09_11-2009.pdf
            // Le Monde Quo. du 20 et 21-09-2009_pdf.pdf
            Regex re5 = new Regex(@"le[ \-_\.]?monde[ \-_\.]+(?:(?:edition|quo(?:tidien)?)[ \-_\.]+)?(?:du[ \-_\.])?([0-9]{2})(?:[ \-_\.](?:et[ \-_\.])?[0-9]{2})?[ \-_\.]?([0-9]{2})[ \-_\.]?([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // Le.Monde.230908.pdf
            // Le.Monde.02.11.08.pdf
            // Le_Monde161009.pdf
            Regex re6 = new Regex(@"le[ \-_\.]monde[ \-_\.]*(?:du[ \-_\.])?([0-9]{2})\.?([0-9]{2})\.?([0-9]{2}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // Le Monde 04 Novembre 2008.pdf
            // Le.Monde.29.Aout.2008.French.pdf
            // Le.Monde.10.Juillet.2012.French.pdf
            // Le Monde Edition du 03 Septembre 2008 Pdf.pdf
            // Le Monde Quotidien du 2 mai 2012.pdf
            // LE MONDE Jeudi 22 octobre 2009.pdf
            // Journal LE MONDE - Dimanche 5 et Lundi 6 fevrier 2012.pdf
            string weekday = "lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche";
            string month = "janvier|f[ée]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|oct(?:obre)?|novembre|d[ée]cembre";
            //Regex re7 = new Regex(@"le[ \-_\.]monde[ \-_\.]+(?:(?:edition|quotidien) )?(?:du[ \-_\.])?([0-9]{1,2})[ \-_\.](" + month + @")[ \-_\.]([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            //Regex re7 = new Regex(@"le[ \-_\.]monde[ \-_\.]+(?:(?:edition|quotidien) )?(?:du[ \-_\.])?(?:(?:" + weekday + @")[ \-_\.]+)?([0-9]{1,2})[ \-_\.](" + month + @")[ \-_\.]([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Regex re7 = new Regex(@"le[ \-_\.]monde[ \-_\.]+(?:(?:edition|quotidien) )?(?:du[ \-_\.])?(?:(?:" + weekday + @")[ \-_\.]+)?([0-9]{1,2})[ \-_\.](?:et[ \-_\.]+(?:" + weekday + @")[ \-_\.]+[0-9]{1,2}[ \-_\.]+)?(" + month + @")[ \-_\.]([0-9]{4}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // Le-Monde-du-17-Oct-09.pdf
            Regex re8 = new Regex(@"le[ \-_\.]monde[ \-_\.]+(?:du[ \-_\.])?([0-9]{1,2})[ \-_\.](" + month + @")[ \-_\.]([0-9]{2}).*\.pdf", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string[] files = Directory.GetFiles(dir, "*.pdf");
            foreach (string file in files)
            {
                bool success = false;
                Date date = Date.MinValue;
                Match match = re1.Match(file);
                if (match.Success)
                {
                    try
                    {
                        date = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re2.Match(file);
                if (match.Success)
                {
                    try
                    {
                        date = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re3.Match(file);
                if (match.Success)
                {
                    try
                    {
                        date = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re4.Match(file);
                if (match.Success)
                {
                    try
                    {
                        date = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re9.Match(file);
                if (match.Success)
                {
                    try
                    {
                        date = new Date(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re5.Match(file);
                if (match.Success)
                {
                    try
                    {
                        date = new Date(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re6.Match(file);
                if (match.Success)
                {
                    try
                    {
                        int year = int.Parse(match.Groups[3].Value);
                        year = cu.ToFourDigitYear(year);
                        date = new Date(year, int.Parse(match.Groups[2].Value), int.Parse(match.Groups[1].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re7.Match(file);
                if (match.Success)
                {
                    try
                    {
                        int m = GetMonthNumber(match.Groups[2].Value);
                        date = new Date(int.Parse(match.Groups[3].Value), m, int.Parse(match.Groups[1].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (!success) match = re8.Match(file);
                if (match.Success)
                {
                    try
                    {
                        int m = GetMonthNumber(match.Groups[2].Value);
                        int year = int.Parse(match.Groups[3].Value);
                        year = cu.ToFourDigitYear(year);
                        date = new Date(year, m, int.Parse(match.Groups[1].Value));
                        if (date.Year >= 2000 && date.Year <= Date.Today.Year)
                            success = true;
                    }
                    catch
                    {
                    }
                }
                if (success)
                {
                    //date = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                    int no = le_monde.GetPrintNumber(date);
                    // Le monde - 2012-12-01 - no 21109.pdf
                    string file2 = string.Format("Le monde - {0:yyyy-MM-dd} - no {1}", date, no);
                    file2 = cu.PathSetFile(file, file2);
                    if (file != file2)
                    {
                        string file3 = null;
                        for (int i = 2; File.Exists(file2); i++)
                        {
                            if (file3 == null) file3 = Path.GetFileNameWithoutExtension(file2);
                            file2 = cu.PathSetFile(file2, file3 + "_" + i.ToString());
                        }
                        wr.Print("rename \"{0}\" to \"{1}\"", Path.GetFileName(file), Path.GetFileName(file2));
                        File.Move(file, file2);
                    }
                    else
                        wr.Print("no change \"{0}\"", Path.GetFileName(file));
                }
                else
                    wr.Print("unknow file \"{0}\"", Path.GetFileName(file));

            }
        }

        public static string GetWeekDayName(DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Monday:
                    return "lundi";
                case DayOfWeek.Tuesday:
                    return "mardi";
                case DayOfWeek.Wednesday:
                    return "mercredi";
                case DayOfWeek.Thursday:
                    return "jeudi";
                case DayOfWeek.Friday:
                    return "vendredi";
                case DayOfWeek.Saturday:
                    return "samedi";
                case DayOfWeek.Sunday:
                    return "dimanche";
            }
            return null;
        }

        public static int GetMonthNumber(string month)
        {
            //string month = "janvier|f[ée]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[ée]cembre";
            switch (month.ToLower())
            {
                case "janvier":
                    return 1;
                case "février":
                case "fevrier":
                    return 2;
                case "mars":
                    return 3;
                case "avril":
                    return 4;
                case "mai":
                    return 5;
                case "juin":
                    return 6;
                case "juillet":
                    return 7;
                case "août":
                case "aout":
                    return 8;
                case "septembre":
                    return 9;
                case "octobre":
                case "oct":
                    return 10;
                case "novembre":
                    return 11;
                case "décembre":
                case "decembre":
                    return 12;
            }
            throw new PBException("unknow month \"{0}\"", month);
        }

        public static string GetType(string code)
        {
            // ARG : argent
            // ARH : culture
            // AUT : festival
            // DOS : dossier
            // EDU : éducation
            // LIV : des livres
            // MAG : magazine
            // MDE : économie
            // MDV : mode
            // NYT : the newyork times
            // PEH : géo-politique
            // SCH : science
            // SPH : sport
            // STY : style
            // TEL : tv
            switch (code.ToLower())
            {
                case "arg":
                    return "argent";
                case "arh":
                    return "culture";
                case "aut":
                    return "festival";
                case "dos":
                    return "dossier";
                case "edu":
                    //return "éducation";
                    return "édu";
                case "liv":
                    //return "des livres";
                    return "livres";
                case "mag":
                    //return "magazine";
                    return "mag";
                case "mde":
                    //return "économie";
                    return "éco";
                case "mdv":
                    return "mode";
                case "nyt":
                    //return "the newyork times";
                    return "newyork_times";
                case "peh":
                    //return "géo-politique";
                    return "géo";
                case "sch":
                    return "science";
                case "sph":
                    return "sport";
                case "sty":
                    return "style";
                case "tel":
                    return "tv";
            }
            throw new PBException("unknow type \"{0}\"", code);
        }

    }
}
