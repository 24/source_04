using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using pb;
using pb.Compiler;
using pb.Data;
using pb.Data.Xml;
using pb.IO;
using pb.Text;

//namespace Print.download
namespace Download.Print.old
{
    public static class Test_Exe
    {
        public static void Test_Regex_01()
        {
            //TestRegex(string sRegex, string sInput, RegexOptions options)
            //private static Regex _rgInfo = new Regex(@"^(.*)\s+(.*)\s*|\s*(.*)\s*|\s*(.*)\s*|\s*(.*\s*mb)\s*(.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // L'Express(+ Supplément : L'ExpressStyles) 3160 - 25 au 31 Janvier 2012 French | 134+84 pages | PDF | 132+72 MB A la une dans ce numéro : 2007-2012 : le vrai bilanEt également : Afghanistan booster l'hiver Côte d'Ivoire Fendi Finlande Jean-Luc Mélenchon la peur du déclin Lagardère Paris Racing Lana Del Rey les saisons des émotions Michel Galabru Nicolas Sarkozy Patti Smith Petit Bateau Riom Spiegelman Vanessa Paradis Vincent Darré
            //string s = "L'Express(+ Supplément : L'ExpressStyles) 3160 - 25 au 31 Janvier 2012 French | 134+84 pages | PDF | 132+72 MB A la une dans ce numéro : 2007-2012 : le vrai bilanEt également : Afghanistan booster l'hiver Côte d'Ivoire Fendi Finlande Jean-Luc Mélenchon la peur du déclin Lagardère Paris Racing Lana Del Rey les saisons des émotions Michel Galabru Nicolas Sarkozy Patti Smith Petit Bateau Riom Spiegelman Vanessa Paradis Vincent Darré";
            //string s = "L'Express+ L'ExpressStyles 3176 - 16 au 22 Mai 2012 French | 172+88 pages | HQ PDF | 164+77 MB A la une dans ce numéro : Valérie Trierweiler : en fait-elle trop ?Et également : Afghanistan Cécile Duflot Facebook Festival de Cannes François Hollande Isabelle Huppert Jacques Audiard le juge Van Ruymbeke rescapé du goulag nord-coréen Valérie Trierweiler";
            //PB_Tools.Test.Test_Regex(@"^(.*)\s+([^\s]+)\s*\|\s*(.*)\s*\|\s*(.*)\s*\|\s*(.*?\s*mb)\s*(.*)$", s, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //string pattern = @"^Le monde - ([0-9]{4})-([0-9]{2})-([0-9]{2}) - no ([0-9]+) (:?(_quo)(:?\+(argent|culture|dossier|edu|géo|livres|mag|mode|science|sport|tv|éco|édu))*|-(:?argent|culture|dossier|edu|géo|livres|mag|mode|science|sport|tv|éco|édu))\.pdf$";
            //string pattern = @"^Le monde - ([0-9]{4})-([0-9]{2})-([0-9]{2}) - no ([0-9]+) (:?(_quo)(:?\+(argent|culture|dossier|edu|géo|livres|mag|mode|science|sport|tv|éco|édu))*|-(argent|culture|dossier|edu|géo|livres|mag|mode|science|sport|tv|éco|édu))\.pdf$";
            //string pattern = @"^Le monde - ([0-9]{4})-([0-9]{2})-([0-9]{2}) - no ([0-9]+) ((_quo)(:?\+(argent|culture|dossier|edu|géo|livres|mag|mode|science|sport|tv|éco|édu))*|-(argent|culture|dossier|edu|géo|livres|mag|mode|science|sport|tv|éco|édu))\.pdf$";

            //string s = "Le monde - 2013-02-03 - no 21164 _quo.pdf";
            //string s = "Le monde - 2013-02-03 - no 21164 _quo+tv.pdf";
            //string s = "Le monde - 2013-02-03 - no 21164 _quo+tv+argent.pdf";
            //string s = "Le monde - 2013-03-01 - no 21186 -livres.pdf";

            //string pattern = @"^courrier\s+international\s+n.?([0-9]{4})\s+du\s+([0-9]{1,2})\s+au\s+([0-9]{1,2})\s+(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)\s+([0-9]{4})\.pdf$";
            //string s = "COURRIER INTERNATIONAL N.1168 du 21 au 27 mars 2013.pdf";

            //"^le\s+monde(culture et idees|geo politique|magazine|science et technologie|sport et forme|televisions)(?:\s+(?:du|et|lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche))*\s+([0-9]{1,2})(:?\s+(:?au|et)\s+[0-9]{1,2})?\.([0-9]{1,2})\.?([0-9]{4})?(:?\s*\([0-9]+\))?\.pdf$"
            //string pattern = @"^le\s+monde(culture et idees|geo politique|magazine|science et technologie|sport et forme|televisions)(?:\s+(?:du|et|lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche))*\s+([0-9]{1,2})(:?\s+(:?au|et)\s+[0-9]{1,2})?\.([0-9]{1,2})\.?([0-9]{4})?(:?\s*\([0-9]+\))?\.pdf$";
            //string pattern = @"^le\s+monde\s*(culture et idees|geo politique|magazine|science et technologie|sport et forme|televisions)?(?:\s+(?:du|et|lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche))*\s+([0-9]{1,2})(?:\s+(?:au|et)\s+[0-9]{1,2})?\.([0-9]{1,2})\.?([0-9]{4})?(?:\s*\([0-9]+\))?\.pdf$";
            //string s = "Le Monde du dimanche et lundi 17 et 18.03.2013.pdf";
            //string s = "Le Monde culture et idees du 18.03.2013.pdf";

            // "^$v_print_code1$([0-9o]{1,2})(?:&amp;[0-9o]{1,2})?$v_print_code2$[£\$+]*\.pdf$"
            // v_print_code1="[£\$]pary?[£\$+]*" v_print_code2="[£\$+]eco"
            //string pattern = @"^[£\$]pary?[£\$+]*([0-9o]{1,2})(?:&amp;[0-9o]{1,2})?[£\$+]eco[£\$+]*\.pdf$";
            //string s = "£Pary10£eco$.pdf";

            //string pattern = @"^Le monde - ([0-9]{4})-([0-9]{2})-([0-9]{2}) - no ([0-9]+) ((_quo)(?:\+(argent|culture|dossier|éco|eco|édu|edu|festival|géo|geo|livres|mag|mode|science|sport|style|nyt|tv|document|élection|hebdo|sup))*|-(argent|culture|dossier|éco|eco|édu|edu|festival|géo|geo|livres|mag|mode|science|sport|style|nyt|tv|document|élection|hebdo|sup))\.pdf$";
            //string pattern = @"^Le monde - ([0-9]{4})-([0-9]{2})-([0-9]{2}) - no ([0-9]+) ((_quo)(?:\+(éco|eco))*|-(éco|eco))\.pdf$";
            //string s = "Le monde - 2013-07-07 - no 21295 -éco.pdf";
            //PB_Tools.Test.Test_Regex(pattern, s, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //s = "Le monde - 2013-07-07 - no 21295 _quo.pdf";
            //PB_Tools.Test.Test_Regex(pattern, s, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //s = "Le monde - 2013-07-07 - no 21295 _quo+éco.pdf";
            //PB_Tools.Test.Test_Regex(pattern, s, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            //string pattern = @"^01\s*net(?:\s+n°[0-9]+)?\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche)\s+([0-9]{1,2})\s+(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)\s+(?:au\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche)\s+(?:[0-9]{1,2})\s+(?:janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)?\s*)?([0-9]{4})\s*\.pdf$";
            //string pattern = @"^01\s*net(?:\s+n°[0-9]+)?\s+";
            //string pattern = @"^01\s*net(?:\s+n°[0-9]+)?\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?([0-9]{1,2})\s+(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)\s+(?:au\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?(?:[0-9]{1,2})\s+(?:janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)?\s*)?([0-9]{4})\s*\.pdf$";
            //string s = "01Net N°778 Du 11 Juillet au 07 Aout 2013.pdf";

            //string pattern = @"equipe\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?([0-9]{1,2})\s+(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)\s+(?:au\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?(?:[0-9]{1,2})\s+(?:janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)?\s*)?([0-9]{4})";
            //string pattern = @"equipe\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche)?\s*([0-9]{1,2})\s+(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)\s+(?:au\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?(?:[0-9]{1,2})\s+(?:janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)?\s*)?([0-9]{4})";
            //string pattern = @"equipe\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?([0-9]{1,2})\s+(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)";
            //string pattern = @"equipe\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?([0-9]{1,2})";
            //string s = "L' Equipe  du lundi 15 juillet 2013.pdf";

            //string pattern = GetPrintConfig().Element.zXPathValue("Print/FilenamesModel/FilenameModel[@name='code_date1']/@regex");
            //Dictionary<string, string> values = new Dictionary<string, string>();
            //values.Add("v_code", "humanite");
            //pattern = pattern.zSetTextValues(values);
            //// "humanite\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche)?\s*([0-9]{1,2})\s+(?:&\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche)\s*([0-9]{1,2})\s+)*(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)\s+(?:au\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?(?:[0-9]{1,2})\s+(?:janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)?\s*)?([0-9]{4})"
            ////  humanite\s+du\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche)?\s*([0-9]{1,2})\s+(?:&\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche)\s*([0-9]{1,2})\s+)*(janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)\s+(?:au\s+(?:lundi|mardi|mercredi|jeudi|vendredi|samedi|dimanche\s+)?(?:[0-9]{1,2})\s+(?:janvier|f[eé]vrier|mars|avril|mai|juin|juillet|ao[uû]t|septembre|octobre|novembre|d[eé]cembre)?\s*)?([0-9]{4})
            //string s = "L'Humanite du Vendredi 19 & Samedi 20 & Dimanche 21 Juillet 2013.pdf";

            //string pattern = @"(?:[\$s]ud\$?[\$0]u?es?t?\$?|sud)([0-9o]{1,2})";
            //string s = "Sud0ues20.pdf";

            //string pattern = GetPrintConfig().Element.zXPathValue("Print/FilenamesModel/FilenameModel[@name='code_date1']/@regex");
            //string s = "Sud0ues20.pdf";

            //string pattern = @"_(quo|arg|arh|aut|dos|edu|liv|mag|mde|mdv|nyt|peh|sch|scq|sph|sty|tel)\.pdf";
            //string s = "20130720_ARH.pdf";

            //string pattern = "(?<![0-9o])([0-9o]{1,2})(?![0-9o])";
            //string s = "123a45";

            //string pattern = @"du\s+(?:$weekdays$)?\s*([0-9]{1,2})\s+(?:&amp;\s+(?:$weekdays$)\s*(?:[0-9]{1,2})\s+)*($months$)\s+(?:au\s+(?:$weekdays$\s+)?(?:[0-9]{1,2})\s+(?:$months$)?\s*)?([0-9]{4})";
            //pattern = pattern.Replace("&amp;", "&");
            //XmlConfig config = GetPrintConfig();
            //TextValues textValues = new TextValues(config.GetTextValues());
            //pattern = textValues.SetTextValues(pattern);
            //string s = "Le Nouvel Observateur N°2542 Du 25 au 31 Juillet 2013.pdf";

            //string pattern = @"(?:Figaro|F[iy]ga?|^lfg_|^FI_)(?!.*histoire)";
            //string s = "LeFigaroHistoire8.pdf";

            //string pattern = @"([^\s=]+)=([^\s;]*)";
            //string s = "  skimlinks_enabled=1; bb_lastvisit=1367138318; bb_lastactivity=0 ";
            //string s = "skimlinks_enabled=1; bb_lastvisit=1367138318; bb_lastactivity=0; bb_userid=170354; bb_password=f2911aa1a0f89fd2a427ec51dc63b92c; bb_forum_view=3c2096874c61a68fb01f0a6e2e41d54fc89c6092a-5-%7Bi-174_i-1376444795_i-58_i-1376444534_i-230_i-1376444542_i-235_i-1376444624_i-216_i-1376444705_%7D; bb_thread_lastview=454b397cfcf2e2a388b263d9bf8df753a3c16469a-64-%7Bi-439694_i-1375442859_i-439634_i-1375393480_i-439610_i-1375391002_i-439581_i-1375368599_i-439879_i-1375553344_i-439973_i-1375555033_i-440080_i-1375618837_i-440074_i-1375616844_i-440073_i-1375616654_i-440068_i-1375615800_i-440059_i-1375610217_i-440057_i-1375610133_i-440056_i-1375610088_i-440055_i-1375610032_i-440047_i-1375605750_i-440009_i-1375576849_i-439996_i-1375568220_i-440257_i-1375699047_i-440273_i-1375693905_i-440272_i-1375693791_i-440270_i-1375693433_i-440269_i-1375693232_i-440215_i-1375657930_i-440150_i-1375653547_i-440182_i-1375642502_i-440181_i-1375641538_i-440169_i-1375639340_i-440168_i-1375639173_i-440478_i-1375794642_i-440449_i-1375787364_i-440448_i-1375783706_i-440443_i-1375783596_i-440440_i-1375783498_i-440433_i-1375783240_i-440415_i-1375767642_i-440367_i-1375736849_i-440357_i-1375730482_i-440676_i-1375881935_i-440668_i-1375877796_i-440649_i-1375870010_i-440579_i-1375868638_i-440970_i-1375972314_i-440926_i-1375971263_i-440893_i-1375967645_i-440921_i-1375964311_i-440906_i-1375962813_i-440922_i-1375957995_i-441237_i-1376061837_i-441200_i-1376055533_i-441090_i-1376031658_i-441032_i-1376000818_i-441332_i-1376146506_i-441515_i-1376204841_i-441717_i-1376307904_i-441664_i-1376262586_i-441661_i-1376262454_i-441626_i-1376247844_i-441625_i-1376247778_i-441835_i-1376379820_i-441834_i-1376379720_i-441815_i-1376348204_i-441745_i-1376315912_i-441873_i-1376461190_i-442236_i-1376461791_%7D; __utma=164418920.193645274.1367138295.1376462653.1376465584.74; __utmb=164418920.1.10.1376465584; __utmc=164418920; __utmz=164418920.1371282698.13.2.utmcsr=google|utmccn=(organic)|utmcmd=organic|utmctr=(not%20provided); vbseo_loggedin=yes; bb_sessionhash=8d2210ff76e837b72a39473bb3e35d19";
            //string s = "    bb_lastvisit=1367138318     ";

            //string pattern = @"^multi\s*/?$";
            //string s = "Multi /";

            string pattern = @"(aujourd'hui|hier),?";
            string s = "aujourd'hui, 10:08";
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match match = rg.Match(s);
            if (match.Success)
            {
                Trace.WriteLine("replace : \"{0}\"", match.zReplace(s, DateTime.Today.ToString("dd/MM/yyyy")));
            }
            //string pattern = @"";
            //string s = "";


            //Test.Test_Text.Test_Regex.Test(pattern, s, RegexOptions.IgnoreCase);
        }

        public static void Test_RegexValues_01()
        {
            Trace.WriteLine("Test_RegexValues_01");
            //string s = "Le monde - 2013-02-01 - no 21162 _quo.pdf";
            //string s = "Le monde - 2013-02-03 - no 21164 _quo+tv.pdf";
            string s = "Le monde - 2013-02-03 - no 21164 _quo+tv+argent.pdf";
            //string s = "Le monde - 2013-03-01 - no 21186 -livres.pdf";
            //string s = "20130315_LIV.pdf";
            Test_RegexValues(XmlConfig.CurrentConfig.GetElements("Print/Filenames/Filename"), s);
        }

        public static void Test_RegexValues(IEnumerable<XElement> elements, string input)
        {
            Trace.WriteLine("\"{0}\" : ", input);
            RegexValuesList rvs = new RegexValuesList(elements);
            //FindText_old findText = rvs.Find_old(input);
            FindText findText = rvs.Find(input);
            if (findText.found)
            {
                //NamedValues<ZValue> values = findText.regexValues.GetValues_old();
                NamedValues<ZValue> values = findText.matchValues.GetValues();
                //Trace.WriteLine("  found \"{0}\" {1} values", findText.regexValues.Name, values.Count);
                Trace.WriteLine("  found \"{0}\" {1} values", findText.matchValues.Name, values.Count);
                foreach (KeyValuePair<string, ZValue> value in values)
                {
                    if (value.Value is ZStringArray)
                        Trace.WriteLine("  value {0} = [{1}] {2}", value.Key, ((string[])value.Value).Length, ((string[])value.Value).zToStringValues(s => "\"" + s.zToStringOrNull() + "\""));
                    else
                        Trace.WriteLine("  value {0} = \"{1}\"", value.Key, value.Value.zToStringOrNull());
                }
            }
            else
                Trace.WriteLine("  not found");
            Trace.WriteLine();
        }

        public static void Test_01()
        {
            string s = null;
            Trace.WriteLine("Test_01 {0}", s);
            //Directory.GetCurrentDirectory();
            //File.SetAttributes("", FileAttributes.Archive);
            //AppDomain.CurrentDomain.BaseDirectory
            //AppDomain.CurrentDomain.FriendlyName
            //AppDomain.CurrentDomain.RelativeSearchPath
            //Assembly.GetExecutingAssembly();
        }

        public static void Test_TextValues_01()
        {
            string s = XmlConfig.CurrentConfig.Get("Print/FilenamesModel/FilenameModel[2]/@regex");
            Trace.WriteLine("Print/FilenamesModel/FilenameModel[2]/@regex {0}", s);
        }

        public static void Test_Attribute_01()
        {
            Trace.WriteLine("Test_Attribute_01");
            Test01 t = new Test01();
            Type type = t.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                Attribute attrib = Attribute.GetCustomAttribute(field, typeof(ExportAttribute));
                if (attrib == null)
                    Trace.WriteLine("field {0} dont have Export attribute", field.Name);
                else
                    Trace.WriteLine("field {0} have Export attribute", field.Name);
            }
        }

        public static void Test_AddTraceFile_01()
        {
            Trace.WriteLine("Test_AddTraceFile_01");
            string logFile = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), @"test_log_01.txt");
            Trace.WriteLine("add trace file to \"{0}\"", logFile);
            //Trace.CurrentTrace.AddTraceFile(logFile, LogOptions.RazLogFile);
            if (logFile != null)
                Trace.CurrentTrace.AddOnWrite("Test_Exe", WriteToFile.Create(logFile, FileOption.RazFile).Write);
            Trace.WriteLine("Test trace");
            Trace.WriteLine("Test trace");
            Trace.WriteLine("Test trace");
            Trace.WriteLine("Test trace");
            //Trace.CurrentTrace.RemoveTraceFile(logFile);
            if (logFile != null)
                Trace.CurrentTrace.RemoveOnWrite("Test_Exe");
            Trace.WriteLine("remove trace file to \"{0}\"", logFile);
        }

        public static void Test_ViewResult_01()
        {
            Trace.WriteLine("Test_ViewResult_01");
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("t", typeof(string)));
            dt.Columns.Add(new DataColumn("i", typeof(int)));
            DataRow row = dt.NewRow();
            row["t"] = "text 01";
            row["i"] = 1;
            dt.Rows.Add(row);
            row = dt.NewRow();
            row["t"] = "text 02";
            row["i"] = 2;
            dt.Rows.Add(row);
            row = dt.NewRow();
            row["t"] = DBNull.Value;
            row["i"] = DBNull.Value;
            dt.Rows.Add(row);
            row = dt.NewRow();
            row["t"] = "text 03";
            row["i"] = 3;
            dt.Rows.Add(row);
            row = dt.NewRow();
            row["t"] = "text 04";
            row["i"] = 4;
            dt.Rows.Add(row);
            //            string xmlFormat = @"
            //                <def def='test'>
            //                    <col Name='t' Edit='Mask' NullText='--null--'/>
            //                    <col Name='i' Edit='Mask' NullText='--null--'/>
            //                </def>
            //            ";
            //_wr.SetResult(dt, xmlFormat);
            RunSource.CurrentRunSource.SetResult(dt);
        }

        public static void Test_ReadFileLines_01()
        {
            string file = zPath.Combine(XmlConfig.CurrentConfig.GetExplicit("DataDir"), @"control\print_filenames_01.txt");
            //RunSource.CurrentRunSource.View(zfile.ReadAllLines(file));
            zfile.ReadAllLines(file).zView();
        }

        public static void Test_linq_01()
        {
            Trace.WriteLine("Test_linq_01");
            XElement xe = null;
            xe.XPathSelectElements("");
            xe.zDescendantTexts();
            var q = from xe2 in xe.XPathSelectElements("") select (from s in xe2.zDescendantTexts() select s);
            var q2 = from xe2 in xe.XPathSelectElements("") from s in xe2.zDescendantTexts() select s;
        }

        public static void Test_ZValue_01()
        {
            Trace.WriteLine("Test_ZValue_01");
            ZString zs = null;
            Trace.WriteLine("ZString : \"{0}\"", (string)zs);
        }

        //public static void Test_XmlFilesXPathEvaluate_01(string xpath)
        //{
        //    Trace.WriteLine("Test_xml_01");
        //    string dir = XmlConfig.CurrentConfig.Get("TelechargementPlus/CacheDirectory");
        //    //string dir = @"c:\pib\dev_data\exe\wrun\test\telechargement-plus\test";
        //    //var q = from file in Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories) orderby file descending select from s in zxml.XmlFileXPathEvaluate(file, xpath) select new { file = file, image = s };
        //    //RunSource.CurrentRunSource.View(q.Flatten());
        //    //var q = zxml.XmlFilesXPathEvaluate(from file in Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories) orderby file descending select file, xpath);
        //    //var q = zxml.XmlFilesXPathEvaluate(Directory.EnumerateFiles(dir, "*.xml", SearchOption.AllDirectories).OrderByDescending(file => file).Take(5), xpath);
        //    var q = zxml.XmlFilesXPathEvaluate(zdir.Files(dir, "*.xml", SearchOption.AllDirectories, SortOption.Descending).Take(5), xpath);
        //    RunSource.CurrentRunSource.View(q);
        //}

        public static void Test_ConcatXmlFiles_01()
        {
            string dir = @"c:\pib\dev_data\exe\wrun\test\telechargement-plus\test";
            string xmlFile = dir + @"\posts.xml";
            IEnumerable<string> files = zdir.Files(dir, "*.xml");
            XmlWriterSettings wSettings = new XmlWriterSettings();
            wSettings.Encoding = Encoding.UTF8;
            wSettings.Indent = true;
            using (XmlWriter xw = XmlWriter.Create(xmlFile, wSettings))
            {
                xw.WriteStartElement("root");
                foreach (string file in files)
                {
                    if (file == xmlFile)
                        continue;
                    XmlReaderSettings rSettings = new XmlReaderSettings();
                    rSettings.IgnoreComments = true;
                    rSettings.IgnoreWhitespace = true;
                    using (XmlReader xr = XmlReader.Create(file, rSettings))
                    {
                        while (!xr.EOF)
                        {
                            xr.Read();
                            if (xr.NodeType == XmlNodeType.Element)
                                break;
                        }
                    }
                }
                xw.WriteEndElement();
            }
        }

        public static void Test_LoadXml_01()
        {
            string dir = @"c:\pib\dev_data\exe\wrun\test\telechargement-plus\test";
            //string xmlFile = dir + @"\posts.xml";
            string xmlFile = dir + @"\87003-kamasutra.xml";
            using (XmlTextReader xtr = new XmlTextReader(xmlFile))
            //using (XmlReader xr = XmlReader.Create(xmlFile))
            {
                //Trace.WriteLine("XmlReader type \"{0}\"", xr.GetType().FullName);
                XDocument xd = XDocument.Load(xtr);
            }
        }

        public static void Test_LoadXml_02()
        {
            string dir = @"c:\pib\dev_data\exe\wrun\test\telechargement-plus\test";
            string xmlFile = dir + @"\87003-kamasutra_2.xml";
            using (XmlTextReader xtr = new XmlTextReader(xmlFile))
            {

                while (xtr.Read())
                {
                    //xtr.IsEmptyElement
                    //xtr.IsStartElement();
                    //xtr.Name
                    //xtr.NodeType
                    //xtr.ReadState
                    //xtr.Value
                    //xtr.ValueType
                    if (xtr.NodeType != XmlNodeType.Whitespace)
                        Trace.WriteLine("node {0} \"{1}\" value \"{2}\" {3}", xtr.NodeType, xtr.Name, xtr.Value, xtr.ValueType);
                    
                }
            }
        }

        public static void Test_LoadXml_03()
        {
            string dir = @"c:\pib\dev_data\exe\wrun\test\telechargement-plus\test";
            string xmlFile = dir + @"\87003-kamasutra.xml";
            XmlReaderSettings rSettings = new XmlReaderSettings();
            rSettings.IgnoreComments = true;
            rSettings.IgnoreWhitespace = true;
            using (XmlReader xr = XmlReader.Create(xmlFile, rSettings))
            {

                while (xr.Read())
                {
                    Trace.Write("depth {0} nodetype {1} name \"{2}\"", xr.Depth, xr.NodeType, xr.Name);
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        Trace.Write(" isemptyelement {0} hasattributes {1}", xr.IsEmptyElement, xr.HasAttributes);
                        if (xr.HasAttributes)
                        {
                            while (xr.MoveToNextAttribute())
                            {
                                Trace.WriteLine();
                                Trace.Write("  depth {0} nodetype {1} name \"{2}\" value \"{3}\"", xr.Depth, xr.NodeType, xr.Name, xr.Value);
                            }
                            xr.MoveToElement();
                        }
                    }
                    Trace.WriteLine();
                }
            }
        }

        //public static void Test_DescendantNodes_01()
        //{
        //    XXElement xe = new XXElement(HtmlXmlReader.CurrentHtmlXmlReader.XDocument.Root);
        //    xe = xe.XPathElement("//div[@id='dle-content']");
        //    xe = xe.XPathElement(".//div[@class='maincont']//div[@class='binner']//div[@class='story-text']");
        //    foreach (XNode node in xe.XElement.DescendantNodes())
        //    {
        //        Trace.Write("node {0}", node.NodeType);
        //        if (node.NodeType == XmlNodeType.Element)
        //            Trace.Write(" {0}", ((XElement)node).Name);
        //        Trace.WriteLine();
        //    }
        //}

        //public static void Test_DescendantNodes_02()
        //{
        //    XXElement xe = new XXElement(HtmlXmlReader.CurrentHtmlXmlReader.XDocument.Root);
        //    xe = xe.XPathElement("//div[@id='dle-content']");
        //    xe = xe.XPathElement(".//div[@class='maincont']//div[@class='binner']//div[@class='story-text']");
        //    //foreach (XNode node in XNodeDescendants.DescendantNodes(xe.XElement, node => node is XElement && ((XElement)node).Name == "a" ? false : true))
        //    foreach (XNode node in xe.XElement.zDescendantNodes(node => node is XElement && ((XElement)node).Name == "a" ? XNodeFilter.SkipNode : XNodeFilter.SelectNode))
        //    {
        //        //Trace.Write("level {0} node {1} {2}", node.level, node.node.NodeType, node);
        //        Trace.Write("node {0}", node.NodeType);
        //        if (node.NodeType == XmlNodeType.Element)
        //            Trace.Write(" {0}", ((XElement)node).Name);
        //        Trace.WriteLine();
        //    }
        //}

        public static void Test_Dictionary_01()
        {
            Trace.WriteLine("Test_Dictionary_01");
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(1, "aaaa");
            dic.Add(3, "cccc");
            dic.Add(2, "bbbb");
            Trace.WriteLine("dic.Values");
            foreach (string s in dic.Values)
                Trace.WriteLine("\"{0}\"", s);
            Trace.WriteLine("dic");
            foreach (KeyValuePair<int, string> v in dic)
                Trace.WriteLine("{0} = \"{1}\"", v.Key, v.Value);
            //dic.Values
        }

    }

    public class XXNode2
    {
        public XNode node;
        public int level;
        public override string ToString()
        {
            if (node is XElement)
            {
                XElement xe = (XElement)node;
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("<{0}", xe.Name));
                foreach (XAttribute xa in xe.Attributes())
                    sb.Append(string.Format(" {0}=\"{1}\"", xa.Name, xa.Value));
                if (xe.FirstNode == null)
                    sb.Append("/");
                sb.Append(">");
                return sb.ToString();
            }
            else
                return node.ToString();
        }
    }

    public class ZXmlReader1 : XmlReader
    {
        public override int AttributeCount
        {
            get { throw new NotImplementedException(); }
        }

        public override string BaseURI
        {
            get { throw new NotImplementedException(); }
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EOF
        {
            get { throw new NotImplementedException(); }
        }

        public override string GetAttribute(int i)
        {
            throw new NotImplementedException();
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            throw new NotImplementedException();
        }

        public override string GetAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public override bool IsEmptyElement
        {
            get { throw new NotImplementedException(); }
        }

        public override string LocalName
        {
            get { throw new NotImplementedException(); }
        }

        public override string LookupNamespace(string prefix)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToAttribute(string name)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToElement()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToFirstAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNextAttribute()
        {
            throw new NotImplementedException();
        }

        public override XmlNameTable NameTable
        {
            get { throw new NotImplementedException(); }
        }

        public override string NamespaceURI
        {
            get { throw new NotImplementedException(); }
        }

        public override XmlNodeType NodeType
        {
            get { throw new NotImplementedException(); }
        }

        public override string Prefix
        {
            get { throw new NotImplementedException(); }
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }

        public override bool ReadAttributeValue()
        {
            throw new NotImplementedException();
        }

        public override ReadState ReadState
        {
            get { throw new NotImplementedException(); }
        }

        public override void ResolveEntity()
        {
            throw new NotImplementedException();
        }

        public override string Value
        {
            get { throw new NotImplementedException(); }
        }
    }
}
