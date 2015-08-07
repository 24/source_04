using System;
using System.IO;
using pb;
using pb.IO;
using Print;

namespace Test.Test_Unit.Print
{
    public static class Test_Unit_Print
    {
        public static void Test_CalculatePrintDateNumber_03(PrintManager printManager, string directory)
        {
            //Test_CalculatePrintNumber_01(printManager["f1_racing"], directory, Date.Parse("2013-01-01"), 24);
            Test_CalculatePrintDateNumber_02(printManager["f1_racing"], directory, Date.Parse("2013-01-01"));
            //Test_CalculatePrintNumber_01(printManager["velo_magazine"], directory, Date.Parse("2013-01-01"), 24);
            Test_CalculatePrintDateNumber_02(printManager["velo_magazine"], directory, Date.Parse("2013-01-01"));
        }

        public static void Test_CalculatePrintDateNumber_01(PrintManager printManager, string directory)
        {
            // daily
            // pas de fonction zprint.GetDailyPrintDate() donc pas de Test_PrintDate_01 pour les quotodiens
            Test_CalculatePrintNumber_01(printManager["dernieres_nouvelles_d_alsace"], directory, Date.Parse("2013-07-01"), 60);
            Test_CalculatePrintNumber_01(printManager["l_equipe"], directory, Date.Parse("2012-01-01"), 1000);
            Test_CalculatePrintNumber_01(printManager["la_croix"], directory, Date.Parse("2013-04-28"), 30);
            Test_CalculatePrintNumber_01(printManager["le_figaro"], directory, Date.Parse("2013-04-16"), 60);
            Test_CalculatePrintNumber_01(printManager["le_monde"], directory, Date.Parse("2008-08-01"), 2000);
            Test_CalculatePrintNumber_01(printManager["le_parisien"], directory, Date.Parse("2013-04-01"), 90);
            Test_CalculatePrintNumber_01(printManager["les_echos"], directory, Date.Parse("2011-04-01"), 1300);
            Test_CalculatePrintNumber_01(printManager["liberation"], directory, Date.Parse("2011-02-01"), 1300);

            // weekly
            Test_CalculatePrintNumber_01(printManager["courrier_international"], directory, Date.Parse("2013-04-04"), 15);
            Test_CalculatePrintDate_01(printManager["courrier_international"], directory, 1170, 15);

            // Monthly
            Test_CalculatePrintNumber_01(printManager["l_expansion"], directory, Date.Parse("2011-11-01"), 30);
            Test_CalculatePrintDate_01(printManager["l_expansion"], directory, 769, 30);

            // Monthly
            Test_CalculatePrintNumber_01(printManager["les_cahiers_de_science_et_vie"], directory, Date.Parse("2012-01-01"), 36);
            Test_CalculatePrintDate_01(printManager["les_cahiers_de_science_et_vie"], directory, 122, 36);

            // Bimonthly
            Test_CalculatePrintNumber_01(printManager["le_monde_des_sciences"], directory, Date.Parse("2012-02-01"), 24);
            Test_CalculatePrintDate_01(printManager["le_monde_des_sciences"], directory, 1, 24);

            // Monthly
            Test_CalculatePrintNumber_01(printManager["montagnes_magazine"], directory, Date.Parse("2012-01-01"), 36);
            Test_CalculatePrintDate_01(printManager["montagnes_magazine"], directory, 374, 36);

            // Quarterly
            Test_CalculatePrintNumber_01(printManager["top_500_sites"], directory, Date.Parse("2013-02-01"), 4);
            Test_CalculatePrintDate_01(printManager["top_500_sites"], directory, 15, 4);
        }

        public static void Test_CalculatePrintDateNumber_02(PrintManager printManager, string directory)
        {
            Date date = Date.Parse("2013-01-01");
            foreach (global::Print.Print print in printManager.Prints.Values)
            {
                if (print.NoDateAndNumberCalculate)
                    continue;
                Test_CalculatePrintDateNumber_02(print, directory, date);
            }
        }

        public static void Test_CalculatePrintDateNumber_02(global::Print.Print print, string directory, Date date)
        {
            int printNumber = print.GetPrintNumber(date);
            if (printNumber <= 0)
                printNumber = 1;
            Date date2 = date;
            if (print.Frequency != PrintFrequency.Daily)
                date2 = print.GetPrintDate(printNumber);
            int nb = 0;
            switch (print.Frequency)
            {
                case PrintFrequency.Daily:
                    nb = 365 * 2;
                    break;
                case PrintFrequency.Weekly:
                    nb = 52 * 2;
                    break;
                case PrintFrequency.EveryTwoWeek:
                    nb = 26 * 2;
                    break;
                case PrintFrequency.Monthly:
                    nb = 12 * 2;
                    break;
                case PrintFrequency.Bimonthly:
                    nb = 6 * 2;
                    break;
                case PrintFrequency.Quarterly:
                    nb = 3 * 2;
                    break;
            }
            Test_CalculatePrintNumber_01(print, directory, date2, nb);
            if (print.Frequency != PrintFrequency.Daily)
                Test_CalculatePrintDate_01(print, directory, printNumber, nb);
        }

        public static void Test_CalculatePrintNumber_01(global::Print.Print print, string directory, Date date, int nb)
        {
            string traceFile = zPath.Combine(directory, @"Print\CalculatePrintDateNumber", string.Format("Print_{0}_Number.txt", print.Name));
            Trace.WriteLine("print {0} frequency {1} calculate number from date {2:dd-MM-yyyy} nb {3} trace file \"{4}\"", print.Name, print.Frequency, date, nb, zPath.GetFileName(traceFile));
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_CalculatePrintNumber_01", WriteToFile.Create(traceFile, FileOption.RazFile).Write);
            try
            {
                Trace.WriteLine("print {0} frequency {1} calculate number from date {2:dd-MM-yyyy} nb {3}", print.Name, print.Frequency, date, nb);
                Trace.WriteLine();
                if (print.Frequency == PrintFrequency.Daily)
                {
                    for (int i = 0; i < nb; i++)
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                        date = date.AddDays(1);
                    }
                }
                else if (print.Frequency == PrintFrequency.Weekly)
                {
                    for (int i = 0; i < nb; i++)
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                        date = date.AddDays(7);
                    }
                }
                else if (print.Frequency == PrintFrequency.Monthly)
                {
                    for (int i = 0; i < nb; i++)
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                        date = date.AddMonths(1);
                    }
                }
                else if (print.Frequency == PrintFrequency.Bimonthly || print.Frequency == PrintFrequency.Quarterly)
                {
                    int lastNumber = 0;
                    for (int i = 0; i < nb; )
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        if (number != lastNumber)
                        {
                            Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                            lastNumber = number;
                            i++;
                        }
                        date = date.AddMonths(1);
                    }
                }
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_CalculatePrintNumber_01");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }

        public static void Test_CalculatePrintDate_01(global::Print.Print print, string directory, int printNumber, int nb)
        {
            string traceFile = zPath.Combine(directory, @"Print\CalculatePrintDateNumber", string.Format("Print_{0}_Date.txt", print.Name));
            Trace.WriteLine("print {0} frequency {1} calculate date from number {2} nb {3} trace file \"{4}\"", print.Name, print.Frequency, printNumber, nb, zPath.GetFileName(traceFile));
            //Trace.CurrentTrace.DisableBaseLog();
            Trace.CurrentTrace.DisableViewer = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_CalculatePrintDate_01", WriteToFile.Create(traceFile, FileOption.RazFile).Write);
            try
            {
                Trace.WriteLine("print {0} frequency {1} calculate date from number {2} nb {3}", print.Name, print.Frequency, printNumber, nb);
                Trace.WriteLine();
                for (int i = 0; i < nb; i++)
                {
                    PrintIssue issue = print.NewPrintIssue(printNumber);
                    Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, printNumber);
                    printNumber++;
                }
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_CalculatePrintDate_01");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }

        public static void Test_CalculatePrintNumber_02(global::Print.Print print, string directory, Date date, int nb)
        {
            string traceFile = zPath.Combine(zPath.Combine(directory, @"Print\CalculatePrintDateNumber"), string.Format("Print_{0}_Number.txt", print.Name));
            Trace.WriteLine("print {0} frequency {1} calculate number from date {2:dd-MM-yyyy} nb {3} trace file \"{4}\"", print.Name, print.Frequency, date, nb, zPath.GetFileName(traceFile));
            //Trace.CurrentTrace.DisableBaseLog();
            //Trace.CurrentTrace.DisableTraceView = true;
            //Trace.CurrentTrace.AddTraceFile(traceFile, LogOptions.RazLogFile);
            Trace.CurrentTrace.AddOnWrite("Test_CalculatePrintNumber_02", WriteToFile.Create(traceFile, FileOption.RazFile).Write);
            try
            {
                Trace.WriteLine("print {0} frequency {1} calculate number from date {2:dd-MM-yyyy} nb {3}", print.Name, print.Frequency, date, nb);
                Trace.WriteLine();
                if (print.Frequency == PrintFrequency.Daily)
                {
                    for (int i = 0; i < nb; i++)
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                        date = date.AddDays(1);
                    }
                }
                else if (print.Frequency == PrintFrequency.Weekly)
                {
                    for (int i = 0; i < nb; i++)
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                        date = date.AddDays(7);
                    }
                }
                else if (print.Frequency == PrintFrequency.Monthly)
                {
                    for (int i = 0; i < nb; i++)
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                        date = date.AddMonths(1);
                    }
                }
                else if (print.Frequency == PrintFrequency.Bimonthly || print.Frequency == PrintFrequency.Quarterly)
                {
                    int lastNumber = 0;
                    for (int i = 0; i < nb; )
                    {
                        PrintIssue issue = print.NewPrintIssue(date);
                        int number = issue.PrintNumber;
                        if (number != lastNumber)
                        {
                            Trace.WriteLine("{0:yyyy-MM-dd ddd} {1}", issue.Date, number);
                            lastNumber = number;
                            i++;
                        }
                        date = date.AddMonths(1);
                    }
                }
            }
            finally
            {
                //Trace.CurrentTrace.EnableBaseLog();
                //Trace.CurrentTrace.RemoveTraceFile(traceFile);
                Trace.CurrentTrace.RemoveOnWrite("Test_CalculatePrintNumber_02");
                Trace.CurrentTrace.DisableViewer = false;
            }
        }
    }
}
