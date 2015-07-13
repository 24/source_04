using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using pb;
using pb.Compiler;

namespace Print.Tunit
{
    class Tunit_print_01
    {
        private static ITrace _tr = Trace.CurrentTrace;
        private static RunSource _wr = RunSource.CurrentRunSource;
        public static void Test_LeMonde_01()
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
            if (!r) _tr.WriteLine("Control failed"); else _tr.WriteLine("Control ok");
        }

        public static bool ControlLeMondePrintNumber(string date, int no)
        {
            Regex re = new Regex("([0-9]{4})-([0-9]{2})-([0-9]{2})", RegexOptions.Compiled);
            Match match = re.Match(date);
            if (!match.Success)
            {
                _tr.WriteLine("unknow date \"{0}\"", date);
                return false;
            }
            Date date2 = new Date(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            //int no2 = le_monde.GetPrintNumber(date2);
            int no2 = LeMonde0.GetPrintNumber(date2);
            string s;
            if (no == no2) s = "ok"; else s = "failed";
            _tr.WriteLine("le monde du {0,-10:dddd} {1:yyyy-MM-dd} no {2,6} no calculé {3,6} : {4}", date2, date2, no, no2, s);
            return no == no2;
        }
    }
}
