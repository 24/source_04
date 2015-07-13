using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Compiler;
using pb.Data.Xml;
using pb.IO;
using pb.Text;
using Print;

//namespace Print.download
namespace Download.Print
{
    public static class Print_Exe
    {
        public static void Test_utf8_01()
        {
            Trace.WriteLine("Test_utf8_01");
            string s;
            byte[] b = { 0xC3, 0xA9 }; s = Encoding.UTF8.GetString(b);
            //Trace.WriteLine("bytes {0} string \"{1}\"", b.zToStringValues(x => x.zToHex()), s);
            Trace.WriteLine("bytes {0} string \"{1}\" code {2}", b.zToStringValues(x => x.zToHex()), s, ((int)s[0]).zToHex());
            b = new byte[] { 0xD0, 0xBE }; s = Encoding.UTF8.GetString(b);
            //Trace.WriteLine("bytes {0} string \"{1}\"", b.zToStringValues(x => x.zToHex()), s);
            Trace.WriteLine("bytes {0} string \"{1}\" code {2}", b.zToStringValues(x => x.zToHex()), s, ((int)s[0]).zToHex());
            b = new byte[] { 0xD0, 0xB5 }; s = Encoding.UTF8.GetString(b);
            Trace.WriteLine("bytes {0} string \"{1}\" code {2}", b.zToStringValues(x => x.zToHex()), s, ((int)s[0]).zToHex());
            b = new byte[] { 0xD0, 0xB0 }; s = Encoding.UTF8.GetString(b);
            Trace.WriteLine("bytes {0} string \"{1}\" code {2}", b.zToStringValues(x => x.zToHex()), s, ((int)s[0]).zToHex());

            s = "é";
            Trace.WriteLine("string \"{0}\" code {1}", s, ((int)s[0]).zToHex());
            s = "o";
            Trace.WriteLine("string \"{0}\" code {1}", s, ((int)s[0]).zToHex());
            s = "e";
            Trace.WriteLine("string \"{0}\" code {1}", s, ((int)s[0]).zToHex());

            s = "R_C3_A9ponses_Photo_241_Avril_2012.pdf";
            Trace.WriteLine("string \"{0}\"", s);
            Trace.WriteLine("replace utf8 code \"{0}\"", zstr.ReplaceUTF8Code(s));
            s = "Rep_D0_BEns_D0_B5s_Photo_Decembre_249_2012.pdf";
            Trace.WriteLine("string \"{0}\"", s);
            Trace.WriteLine("replace utf8 code \"{0}\"", zstr.ReplaceUTF8Code(s));
        }

        public static void Test_SpecialDays_01()
        {
            string logFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), @"control\ctrl_SpecialDays_01.txt");
            Trace.WriteLine("write to file \"{0}\"", logFile);
            //Trace.CurrentTrace.AddTraceFile(logFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Print_Exe", WriteToFile.Create(logFile, FileOption.RazFile).Write);

            //_wr.DisableMessage = true;
            CultureInfo culture = new CultureInfo("fr-FR");
            Trace.WriteLine("Mardi gras          Dimanche des        Dimanche de         Jeudi de            Dimanche de");
            Trace.WriteLine("                    Rameaux             Pâques              l'Ascension         Pentecôte");
            for (int year = 2013; year <= 2035; year++)
            {
                Date dateMardiGras = SpecialDayTools.GetMardiGrasDate(year);
                string dateMardiGrasString = string.Format(culture, "{0:dd}  {0,-7:MMMM} {0:yyyy}", dateMardiGras);

                Date datePalmSunday = SpecialDayTools.GetPalmSundayDate(year);
                string datePalmSundayString = string.Format(culture, "{0:dd}  {0,-7:MMMM} {0:yyyy}", datePalmSunday);

                Date dateEasterSunday = SpecialDayTools.GetEasterSundayDate(year);
                string dateEasterSundayString = string.Format(culture, "{0:dd}  {0,-7:MMMM} {0:yyyy}", dateEasterSunday);

                Date dateAscensionThursday = SpecialDayTools.GetAscensionThursdayDate(year);
                string dateAscensionThursdayString = string.Format(culture, "{0:dd}  {0,-7:MMMM} {0:yyyy}", dateAscensionThursday);

                Date datePentecostSunday = SpecialDayTools.GetPentecostSundayDate(year);
                string datePentecostSundayString = string.Format(culture, "{0:dd}  {0,-7:MMMM} {0:yyyy}", datePentecostSunday);

                Trace.WriteLine("{0}    {1}    {2}    {3}    {4}", dateMardiGrasString, datePalmSundayString, dateEasterSundayString, dateAscensionThursdayString, datePentecostSundayString);
            }
            //_wr.DisableMessage = false;
            //Trace.CurrentTrace.RemoveTraceFile(logFile);
            Trace.CurrentTrace.RemoveOnWrite("Print_Exe");
        }

        //public static XmlConfig GetPrintConfig()
        //{
        //    string file = zpath.GetPathFile(XmlConfig.CurrentConfig.GetExplicit("PrintConfig"));
        //    //Trace.WriteLine("current directory \"{0}\"", Directory.GetCurrentDirectory());
        //    //Trace.WriteLine("print config \"{0}\"", file);
        //    XmlConfig config = new XmlConfig(file);
        //    //Trace.WriteLine("print config path \"{0}\"", config.ConfigPath);
        //    return config;
        //}

        //public static PrintManager_v1 GetPrintManager_v1()
        //{
        //    return new PrintManager_v1(GetPrintConfig().GetElement("Print"));
        //}

        //public static PrintManager_v2 GetPrintManager_v2()
        //{
        //    return new PrintManager_v2(GetPrintConfig().GetElement("Print"));
        //}

        //public static void Test_PrintManager_01()
        //{
        //    //string file = "Le monde - 2013-07-07 - no 21295 _quo+éco.pdf";
        //    //string file = "Le monde - 2013-07-07 - no 21295 -éco.pdf";
        //    //string file = "Le parisien - 2013-05-24 - no 21369 -mag.pdf";
        //    //string file = "Le monde - 2013-05-30 - no 21262 _quo.pdf";
        //    //string file = "Le monde - 2013-05-30 - no 21262 -éco.pdf";
        //    //string file = "LeVifWeekEnd20130607.pdf";
        //    //string file = "325_AltertvEc0nomik325bkf.pdf";
        //    //string file = "L'Humanite du Vendredi 19 & Samedi 20 & Dimanche 21 Juillet 2013.pdf";
        //    //string file = "Sud0ues20.pdf";
        //    string file = "ECHOS du mardi 23 juillet 2013.pdf";
        //    //PrintManager pm = new PrintManager(XmlConfig.CurrentConfig.GetElement("Print"));
        //    PrintManager_v1 pm = GetPrintManager_v1();
        //    RegexValuesList rvl = pm.PrintRegexList;
        //    //RegexValues rv = rvl.Find(file);
        //    //FindText_old findText = rvl.Find_old(file);
        //    FindText findText = rvl.Find(file);
        //    Trace.WriteLine("search print \"{0}\"", file);
        //    //if (rv == null)
        //    if (!findText.found)
        //    {
        //        Trace.WriteLine("not found");
        //        return;
        //    }
        //    //Trace.WriteLine("found {0} - {1}", findText.regexValues.Name, findText.regexValues.Key);
        //    Trace.WriteLine("found {0} - {1}", findText.matchValues.Name, findText.matchValues.Key);
        //    //Trace.WriteLine("pattern \"{0}\"", findText.regexValues.Pattern);
        //    Trace.WriteLine("pattern \"{0}\"", findText.matchValues.Pattern);
        //    //RunSource.CurrentRunSource.View(findText.regexValues.GetValues_old());
        //    RunSource.CurrentRunSource.View(findText.matchValues.GetValues());
        //    //Print print = pm.Find(file);
        //    //if (print == null)
        //    //    Trace.WriteLine("print not found");
        //    //else
        //    //    Trace.WriteLine("print file \"{0}\"", print.GetFilename());
        //    //RunSource.CurrentRunSource.View(rv.GetValues());
        //}

        //public static void Test_PrintManager_02()
        //{
        //    //string file = "01Net N°778 Du 11 Juillet au 07 Aout 2013.pdf";
        //    //string file = "Le point - 2013-07-11 - no 2130_2.pdf";
        //    //string file = "LeVifWeekEnd20130607.pdf";
        //    //string file = "Psychologies magazine - 2013-07 - no 331.pdf";
        //    //string file = "L'Humanite du Vendredi 19 & Samedi 20 & Dimanche 21 Juillet 2013.pdf";
        //    string file = "Sud0ues20.pdf";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";

        //    //string key = "01_net2";
        //    PrintManager_v1 pm = GetPrintManager_v1();
        //    Trace.WriteLine("Find \"{0}\"", file);
        //    string error;
        //    Print1 print = pm.Find(file, out error);
        //    if (print == null)
        //        Trace.WriteLine("not found, error \"{0}\"", error);
        //    else
        //        Trace.WriteLine("found \"{0}\"", print.Name);
        //    //RegexValues rv = pm.PrintRegexList[key];
        //    //PB_Tools.Test.Test_Regex(rv.Pattern, file, rv.Options);
        //}

        //public static void Test_PrintManager_03()
        //{
        //    //string file = "01Net N°778 Du 11 Juillet au 07 Aout 2013.pdf";
        //    //string file = "Le point - 2013-07-11 - no 2130_2.pdf";
        //    //string file = "LeVifWeekEnd20130607.pdf";
        //    string file = "Psychologies magazine - 2013-07 - no 331.pdf";
        //    //string file = "L'Humanite du Vendredi 19 & Samedi 20 & Dimanche 21 Juillet 2013.pdf";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";

        //    //string key = "01_net2";
        //    PrintManager_v1 pm = GetPrintManager_v1();
        //    Trace.WriteLine("Find \"{0}\"", file);
        //    string error;
        //    Print1 print = pm.Find(file, out error);
        //    if (print == null)
        //        Trace.WriteLine("not found, error \"{0}\"", error);
        //    else
        //        Trace.WriteLine("found \"{0}\"", print.Name);
        //    //RegexValues rv = pm.PrintRegexList[key];
        //    //PB_Tools.Test.Test_Regex(rv.Pattern, file, rv.Options);
        //}

        //public static void Test_PrintManager_04()
        //{
        //    //string file = "01Net N°778 Du 11 Juillet au 07 Aout 2013.pdf";
        //    //string file = "Le point - 2013-07-11 - no 2130_2.pdf";
        //    //string key = "01_net2";
        //    //string name = "l_equipe";
        //    //string name = "l_humanite";
        //    //string name = "l_expansion";
        //    string name = "les_cahiers_de_science_et_vie";
        //    bool regexValues = false;
        //    PrintManager_v2 pm = GetPrintManager_v2();
        //    global::Print.Print print = pm.GetPrint(name);
        //    Trace.WriteLine("print \"{0}\"", name);
        //    Trace.WriteLine("  NoPrintMonths \"{0}\"", print.NoPrintMonths);
        //    foreach (int date in print.NoPrintDates.Values)
        //    {
        //        Trace.WriteLine("  NoPrintDate {0} - {1}", date, Date.CreateDateFromAbsoluteDay(date));
        //    }
        //    if (regexValues)
        //    {
        //        Trace.WriteLine("GetRegexValuesListByName \"{0}\"", name);
        //        RegexValues[] rvList = pm.PrintRegexList.GetRegexValuesListByName(name);
        //        Trace.WriteLine("found {0}", rvList.Length);
        //        foreach (RegexValues rv in rvList)
        //        {
        //            Trace.WriteLine("pattern \"{0}\"", rv.Pattern);
        //        }
        //    }
        //}

        //public static void Test_PrintManager_05()
        //{
        //    //string file = "01Net N°778 Du 11 Juillet au 07 Aout 2013.pdf";
        //    //string file = "Le point - 2013-07-11 - no 2130_2.pdf";
        //    //string file = "LeVifWeekEnd20130607.pdf";
        //    //string file = "Psychologies magazine - 2013-07 - no 331.pdf";
        //    //string file = "L'Humanite du Vendredi 19 & Samedi 20 & Dimanche 21 Juillet 2013.pdf";
        //    //string file = "Psychologies magazine - hors-série - 2012-10 - no 19.pdf";
        //    //string file = "20130720_ARH.pdf";
        //    //string file = "Figa20.pdf";
        //    //string file = "La croix du Mercredi 24 Juillet  2013.pdf";
        //    //string file = "merlib.240713.pdf";
        //    //string file = "SciencVi1151.pdf";
        //    //string file = "Psychologies_Magazine_Hors_S_rie_N_22_Juillet_Aout_Septembre_2013.pdf";
        //    //string file = "38CI1186.pdf";
        //    //string file = "Le_Nouvel_Observateur_2542_Juillet_2013.pdf";
        //    //string file = "Le Nouvel Observateur N°2542 Du 25 au 31 Juillet 2013.pdf";
        //    //string file = "36517Marion.846.pdf";
        //    //string file = "Invest27Juill02A0u.pdf";
        //    //string file = "L'express - hors-série - 2012-04 - no 13.pdf";
        //    //string file = "Dossier pour la science - 2013-01 - no 78 - vents et nuages, la physique du ciel.pdf";
        //    //string file = "36DossPolaScien_80.pdf";
        //    //string file = "Invest27Juill02A0u.pdf";
        //    //string file = "34665IntelligenceMonde31.pdf";
        //    //string file = "Le monde de l'intelligence - 2013-07 - no 31.pdf";
        //    //string file = "36571Ccervv0.PPssyycc0.58.pdf";
        //    //string file = "L'équipe - 2013-07-31 - no 21564.pdf";
        //    //string file = "L'essentiel cerveau et psycho - 2012-05 - no 10.pdf";
        //    //string file = "Le Figaro - Vendredi 2 Août 2013.pdf";
        //    //string file = "Archéothéma - 2013-07 - no 29_2.pdf";
        //    //string file = "Fémina n°32 du 04 aout 2013.pdf";
        //    //string file = "Rep_D0_BEns_D0_B5s_Photo_Decembre_249_2012.pdf";
        //    //string file = "LeFigaroHistoire8.pdf";
        //    //string file = "Cinema_Teaser_13_Avril_2012_28Collector_Avengers_29.pdf";
        //    //string file = "Trek Magazine N°149 Juin Juillet 2013.pdf";
        //    //string file = "Ciel_26_Espace_Hors_S_C3_A9rie_19_Juillet_2012.pdf";
        //    //string file = "Ciel_et_Esp_D0_B0ce_510_2012.pdf";
        //    //string file = "Montagnes_Magazine_Hors_S_C3_A9ie_380_Et_C3_A9_2012.pdf";
        //    //string file = "Bateaux Hors-Série N°2 (2010).pdf";
        //    string file = "Le monde - 2013-08-11 - no 21325 _quo+sport+tv.txt";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";
        //    //string file = "";

        //    //string key = "01_net2";
        //    PrintManager_v2.Trace = true;
        //    global::Print.Print.Trace = true;
        //    PrintIssue.Trace = true;
        //    PrintManager_v2 pm = GetPrintManager_v2();
        //    Trace.WriteLine("Create PrintManager ({0})", pm.PrintRegexList.Count);

        //    Trace.WriteLine("Find \"{0}\"", file);
        //    //string error;
        //    //PrintIssue issue = pm.Find(file, out error);
        //    PrintIssue issue = pm.Find(file);
        //    if (issue == null)
        //    {
        //        Trace.WriteLine("not found");
        //        return;
        //    }
        //    if (issue.Error != null)
        //    {
        //        Trace.WriteLine("not found, error \"{0}\"", issue.Error);
        //        return;
        //    }
        //    Trace.WriteLine("found \"{0}\"", issue.Print.Name);
        //    issue.PrintValues.zTrace();
        //    Trace.WriteLine("filename \"{0}\"", issue.GetFilename());
        //    Trace.WriteLine("SpecialMonth {0}", issue.SpecialMonth);
        //    //RegexValues rv = pm.PrintRegexList[key];
        //    //PB_Tools.Test.Test_Regex(rv.Pattern, file, rv.Options);
        //}

        //public static void Test_PrintManager_06()
        //{
        //    string name = "le_monde";
        //    Trace.WriteLine("Create PrintManager");
        //    PrintManager_v2 pm = GetPrintManager_v2();
        //    Trace.WriteLine("search pattern \"{0}\"", name);
        //    for (int i = 1; ; i++)
        //    {
        //        string key = name + i.ToString();
        //        if (!pm.PrintRegexList.ContainsKey(key))
        //            break;
        //        RegexValues rv = pm.PrintRegexList[key];
        //        Trace.WriteLine("{0} - \"{1}\"", key, rv.Pattern);
        //    }
        //    Trace.WriteLine("date patterns");
        //    foreach (RegexValues rv in pm.DateRegexList.Values)
        //    {
        //        Trace.WriteLine("\"{0}\"", rv.Pattern);
        //    }
        //}

        //public static void Test_LeParisien_01()
        //{
        //    LeParisien pari = new LeParisien("le_parisien", "Le parisien", PrintFrequency.Daily, null, null, new Date(2013, 5, 9), 21356, SpecialDay.Sunday);
        //    pari.NewIssue(new Date(2013, 6, 9));
        //    Trace.WriteLine(pari.GetFilename());

        //    Print1 print = new LeParisien("le_parisien", "Le parisien", PrintFrequency.Daily, null, null, new Date(2013, 5, 9), 21356, SpecialDay.Sunday);
        //    print.NewIssue(new Date(2013, 6, 9));
        //    Trace.WriteLine(print.GetFilename());

        //    Print1 print2 = print;
        //    Trace.WriteLine(print2.GetFilename());

        //    //PrintManager pm = new PrintManager(XmlConfig.CurrentConfig.GetElement("Print"));
        //    PrintManager_v1 pm = GetPrintManager_v1();
        //    print2 = pm.Get("le_parisien");
        //    print2.NewIssue(new Date(2013, 6, 9));
        //    Trace.WriteLine(print2.GetFilename());
        //}

        public static void Control_RegexValues_01()
        {
            Trace.WriteLine("Control_RegexValues_01");
            string logFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), @"control\ctrl_RegexValues_01.txt");
            Trace.WriteLine("write to file \"{0}\"", logFile);
            //Trace.CurrentTrace.AddTraceFile(logFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Print_Exe", WriteToFile.Create(logFile, FileOption.RazFile).Write);
            //_wr.DisableMessage = true;
            IEnumerable<XElement> els = XmlConfig.CurrentConfig.GetElements("Print/Filenames/Filename");
            //Test_RegexValues(els, "Le monde - 2013-02-01 - no 21162 _quo.pdf");
            //Test_RegexValues(els, "Le monde - 2013-02-03 - no 21164 _quo+tv.pdf");
            Test_Exe.Test_RegexValues(els, "Le monde - 2013-02-03 - no 21164 _quo+tv+argent.pdf");
            //Test_RegexValues(els, "Le monde - 2013-03-01 - no 21186 -livres.pdf");
            //Test_RegexValues(els, "La Croix du vendredi 24 Mai 2013.pdf");
            //Test_RegexValues(els, "20130315_LIV.pdf");
            //Test_RegexValues(els, );
            //_wr.DisableMessage = false;
            //Trace.CurrentTrace.RemoveTraceFile(logFile);
            Trace.CurrentTrace.RemoveOnWrite("Print_Exe");
        }

        public static void Control_RegexValues_02()
        {
            string file = @"control\print_filenames_01.txt";
            string logFile = @"control\print_filenames_01_regex.txt";
            logFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), logFile);
            Trace.WriteLine("write to file \"{0}\"", logFile);
            //Trace.CurrentTrace.AddTraceFile(logFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Print_Exe", WriteToFile.Create(logFile, FileOption.RazFile).Write);
            Trace.WriteLine("Control RegexValues :");
            Trace.WriteLine("  file : \"{0}\"", file);
            Trace.WriteLine("  log  : \"{0}\"", logFile);
            Trace.WriteLine();
            IEnumerable<XElement> els = XmlConfig.CurrentConfig.GetElements("Print/Filenames/Filename");
            string[] lines = zfile.ReadAllLines(Path.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), file));
            foreach (string line in lines)
            {
                if (line != "")
                    Test_Exe.Test_RegexValues(els, line);
                //else
                //    Trace.WriteLine();
            }
            //Trace.CurrentTrace.RemoveTraceFile(logFile);
            Trace.CurrentTrace.RemoveOnWrite("Print_Exe");
        }

        //public static void Control_PrintNumber_01()
        //{
        //    //Control_PrintNumber_01("la_croix", new Date(2013, 4, 28), 30);
        //    //Control_PrintNumber_01("le_figaro", new Date(2013, 4, 16), 60);
        //    //Control_PrintNumber_01("le_parisien", new Date(2013, 4, 1), 90);
        //    //Control_PrintNumber_01("le_monde", new Date(2008, 8, 1), 2000);
        //    //Control_PrintNumber_01("l_equipe", new Date(2012, 1, 1), 1000);
        //    //Control_PrintNumber_01("les_echos", new Date(2011, 4, 1), 1300);
        //    //Control_PrintNumber_01("liberation", new Date(2011, 2, 1), 1300);
        //    //Control_PrintNumber_01("courrier_international", new Date(2013, 4, 4), 15);
        //    //Control_PrintNumber_02("courrier_international", 1170, 15);
        //    //Control_PrintNumber_01("l_expansion", new Date(2011, 11, 1), 30);
        //    //Control_PrintNumber_02("l_expansion", 769, 30);
        //    //Control_PrintNumber_01("dernieres_nouvelles_d_alsace", new Date(2013, 7, 1), 60);
        //    //Control_PrintNumber_01("les_cahiers_de_science_et_vie", new Date(2012, 1, 1), 36);
        //    //Control_PrintNumber_02("les_cahiers_de_science_et_vie", 122, 36);
        //    //Control_PrintNumber_01("le_monde_des_sciences", new Date(2012, 2, 1), 24);
        //    //Control_PrintNumber_02("le_monde_des_sciences", 1, 24);
        //    //Control_PrintNumber_01("top_500_sites", new Date(2013, 2, 1), 4);
        //    //Control_PrintNumber_02("top_500_sites", 15, 4);
        //    Control_PrintNumber_01("montagnes_magazine", new Date(2012, 1, 1), 36);
        //    Control_PrintNumber_02("montagnes_magazine", 374, 36);
        //}

        //public static void Control_PrintNumber_01(string name, Date date, int nb)
        //{
        //    string logFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), string.Format(@"control\number\ctrl_{0}_number_01.txt", name));
        //    Trace.WriteLine("write to file \"{0}\"", logFile);
        //    Trace.CurrentTrace.AddTraceFile(logFile, LogOptions.RazLogFile);
        //    try
        //    {
        //        RunSource.CurrentRunSource.DisableMessage = true;
        //        PrintManager_v2 pm = GetPrintManager_v2();
        //        global::Print.Print print = pm.GetPrint(name);
        //        if (print.Frequency == PrintFrequency.Daily)
        //        {
        //            for (int i = 0; i < nb; i++)
        //            {
        //                PrintIssue issue = print.NewPrintIssue(date);
        //                int number = issue.PrintNumber;
        //                Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
        //                date = date.AddDays(1);
        //            }
        //        }
        //        else if (print.Frequency == PrintFrequency.Weekly)
        //        {
        //            for (int i = 0; i < nb; i++)
        //            {
        //                PrintIssue issue = print.NewPrintIssue(date);
        //                int number = issue.PrintNumber;
        //                Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
        //                date = date.AddDays(7);
        //            }
        //        }
        //        else if (print.Frequency == PrintFrequency.Monthly)
        //        {
        //            for (int i = 0; i < nb; i++)
        //            {
        //                PrintIssue issue = print.NewPrintIssue(date);
        //                int number = issue.PrintNumber;
        //                Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
        //                date = date.AddMonths(1);
        //            }
        //        }
        //        else if (print.Frequency == PrintFrequency.Bimonthly || print.Frequency == PrintFrequency.Quarterly)
        //        {
        //            int lastNumber = 0;
        //            for (int i = 0; i < nb; )
        //            {
        //                PrintIssue issue = print.NewPrintIssue(date);
        //                int number = issue.PrintNumber;
        //                if (number != lastNumber)
        //                {
        //                    Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
        //                    lastNumber = number;
        //                    i++;
        //                }
        //                date = date.AddMonths(1);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        RunSource.CurrentRunSource.DisableMessage = false;
        //        Trace.CurrentTrace.RemoveTraceFile(logFile);
        //    }
        //}

        //public static void Control_PrintNumber_02(string name, int printNumber, int nb)
        //{
        //    string logFile = Path.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), string.Format(@"control\number\ctrl_{0}_number_02.txt", name));
        //    Trace.WriteLine("write to file \"{0}\"", logFile);
        //    Trace.CurrentTrace.AddTraceFile(logFile, LogOptions.RazLogFile);
        //    try
        //    {
        //        RunSource.CurrentRunSource.DisableMessage = true;
        //        PrintManager_v2 pm = GetPrintManager_v2();
        //        global::Print.Print print = pm.GetPrint(name);
        //        for (int i = 0; i < nb; i++)
        //        {
        //            PrintIssue issue = print.NewPrintIssue(printNumber);
        //            Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, printNumber);
        //            printNumber++;
        //        }
        //    }
        //    finally
        //    {
        //        RunSource.CurrentRunSource.DisableMessage = false;
        //        Trace.CurrentTrace.RemoveTraceFile(logFile);
        //    }
        //}

        //public static void Test_Print_RenameFile_01(bool simulate = true, bool moveFile = false)
        //{
        //    bool recurseDir = false;
        //    Trace.WriteLine("Test_Print_RenameFile_01");
        //    //string dir = @"c:\pib\_dl\_jd\Various Files";
        //    //string dir = @"c:\pib\media\print\.01_quotidien\Le monde";
        //    //string dir = @"c:\pib\media\print\.01_quotidien\La croix";
        //    //string dir = @"c:\pib\media\print\.01_quotidien\Le figaro";
        //    //string dir = @"c:\pib\media\print\.03_mensuel\Cerveau et psycho";
        //    //string dir = @"c:\pib\_dl\_jd\Photo - ApresLeTrent le q3 I\Photo_.-.ApresLeTrent.le.q3.I.J\Bonne journée à vous ;)";
        //    //string dir = @"c:\pib\media\print\.03_mensuel\Le figaro histoire\";
        //    string dir = @"c:\pib\media\print\.01_quotidien\Le monde\a";
        //    //string dir = @"";
        //    //string dir = @"";
        //    //string dir = @"";
        //    //string dir = @"";
        //    Trace.WriteLine("dir : \"{0}\"", dir);
        //    //PrintManager pm = new PrintManager(XmlConfig.CurrentConfig.GetElement("Print"));
        //    PrintManager_v2.Trace = true;
        //    global::Print.Print.Trace = true;
        //    PrintIssue.Trace = true;
        //    PrintManager_v2 pm = GetPrintManager_v2();
        //    SearchOption option = SearchOption.TopDirectoryOnly;
        //    if (recurseDir) option = SearchOption.AllDirectories;
        //    string currentDir = Directory.GetCurrentDirectory();
        //    Directory.SetCurrentDirectory(dir);
        //    //string[] files = Directory.GetFiles(dir, "*.pdf", option);
        //    string[] files = Directory.GetFiles(".", "*.pdf", option);
        //    foreach (string file in files)
        //    {
        //        //if (RunSource.CurrentRunSource.Abort)
        //        if (RunSource.CurrentRunSource.IsExecutionAborted())
        //        {
        //            Trace.WriteLine("Abort function");
        //            break;
        //        }
        //        try
        //        {
        //            //Test_Print_RenameFile(pm, file, renameFile);
        //            zprint.RenameFile(pm, file, simulate, moveFile);
        //        }
        //        catch (Exception ex)
        //        {
        //            Trace.WriteLine(ex.Message);
        //        }
        //    }
        //    Directory.SetCurrentDirectory(currentDir);
        //}
    }
}
