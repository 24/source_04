using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb;

namespace Print.le_monde
{
    class le_monde
    {
        // GetLeMondePrintNumber
        public static int GetPrintNumber(Date date)
        {
            // Le monde - 2012-12-02 - no 21110.pdf
            // Le monde - 2012-10-12 - no 21066.pdf
            // Le monde - 2012-07-19 - no 20993.pdf
            // pas de journal le 1er mai sauf si c'est un dimanche, journal le dimanche 1er mai 2011
            // Test_GetLeMondePrintNumber("2012-04-29"); // ok  20925
            // Test_GetLeMondePrintNumber("2012-05-02"); // ok  20926
            Date dateRef = new Date(2012, 12, 2);
            int noRef = 21110;
            while (date > dateRef)
            {
                if (dateRef.DayOfWeek != DayOfWeek.Sunday && (dateRef.Month != 5 || dateRef.Day != 1 || dateRef.DayOfWeek == DayOfWeek.Sunday))
                    noRef++;
                dateRef = dateRef.AddDays(1);
            }
            while (date < dateRef)
            {
                if (dateRef.DayOfWeek != DayOfWeek.Monday && (dateRef.Month != 5 || dateRef.Day != 1 || dateRef.DayOfWeek == DayOfWeek.Sunday))
                    noRef--;
                dateRef = dateRef.AddDays(-1);
            }
            if (date != dateRef) throw new PBException("error date not found {0}", date.ToString());
            return noRef;
        }

        public static string GetPrintFilename(Date date, string info)
        {
            return string.Format("Le monde - {0:yyyy-MM-dd} - no {1} {2}", date, GetPrintNumber(date), info);
        }

    }
}
