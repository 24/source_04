using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pb;

//namespace Print
namespace Download.Print
{
    public delegate bool PrintExistsDelegate(Date date);

    public partial class zprint
    {
        private static ITrace _tr = Trace.CurrentTrace;

        public static int GetDailyPrintNumber(Date date, int refNumber, Date refDate, SpecialDay noPrintDays, Dictionary<int, int> noPrintDates, Dictionary<int, int> noPrintNumbers)
        {
            int no = refNumber;
            Date date2 = refDate;
            while (date > date2)
            {
                date2 = date2.AddDays(1);
                if (!noPrintDates.ContainsKey(date2.AbsoluteDay) && !SpecialDayTools.IsSpecialDay(noPrintDays, date2))
                {
                    do
                    {
                        no++;
                    } while (noPrintNumbers.ContainsKey(no));
                }
            }
            while (date < date2)
            {
                if (!noPrintDates.ContainsKey(date2.AbsoluteDay) && !SpecialDayTools.IsSpecialDay(noPrintDays, date2))
                {
                    do
                    {
                        no--;
                    } while (noPrintNumbers.ContainsKey(no));
                }
                date2 = date2.AddDays(-1);
            }
            return no;
        }

        public static int GetDailyPrintNumber(Date date, int refNumber, Date refDate, PrintExistsDelegate printExists)
        {
            int no = refNumber;
            Date date2 = refDate;
            while (date > date2)
            {
                date2 = date2.AddDays(1);
                if (printExists(date2))
                    no++;
            }
            while (date < date2)
            {
                if (printExists(date2))
                    no--;
                date2 = date2.AddDays(-1);
            }
            return no;
        }

        public static int GetWeeklyPrintNumber(Date date, int refNumber, Date refDate, DayOfWeek weekday)
        {
            return refNumber + (zdate.GetNearestWeekday(date, weekday).Subtract(refDate).Days / 7);
        }

        public static int GetEveryTwoWeekPrintNumber(Date date, int refNumber, Date refDate, DayOfWeek weekday)
        {
            return refNumber + (zdate.GetNearestWeekday(date, weekday).Subtract(refDate).Days / 14);
        }

        public static int GetMonthlyPrintNumber(Date date, int refNumber, Date refDate, Month noPrintMonths, Dictionary<int, int> noPrintDates, Dictionary<int, int> noPrintNumbers)
        {
            int no = refNumber;
            Date date2 = refDate;
            while (date > date2)
            {
                date2 = date2.AddMonths(1);
                Month month = zdate.GetMonth(date2.Month);
                if ((noPrintMonths & month) != month && !noPrintDates.ContainsKey(date2.AbsoluteDay))
                {
                    do
                    {
                        no++;
                    } while (noPrintNumbers.ContainsKey(no));
                }
            }
            while (date < date2)
            {
                Month month = zdate.GetMonth(date2.Month);
                if ((noPrintMonths & month) != month && !noPrintDates.ContainsKey(date2.AbsoluteDay))
                {
                    do
                    {
                        no--;
                    } while (noPrintNumbers.ContainsKey(no));
                }
                date2 = date2.AddMonths(-1);
            }
            return no;
        }

        public static int GetMultiMonthlyPrintNumber(Date date, int refNumber, Date refDate, Month noPrintMonths, Dictionary<int, int> noPrintDates, Dictionary<int, int> noPrintNumbers, int monthFreq)
        {
            int no = refNumber;
            Date date2 = refDate;
            while (date > date2)
            {
                Month month;
                for (int i = 0; i < monthFreq; i++)
                {
                    do
                    {
                        date2 = date2.AddMonths(1);
                        month = zdate.GetMonth(date2.Month);
                    } while ((noPrintMonths & month) == month || noPrintDates.ContainsKey(date2.AbsoluteDay));
                }
                do
                {
                    no++;
                } while (noPrintNumbers.ContainsKey(no));
            }
            while (date < date2)
            {
                for (int i = 0; i < monthFreq; i++)
                {
                    Month month = zdate.GetMonth(date2.Month);
                    while ((noPrintMonths & month) == month || noPrintDates.ContainsKey(date2.AbsoluteDay))
                    {
                        date2 = date2.AddMonths(-1);
                        month = zdate.GetMonth(date2.Month);
                    }
                    date2 = date2.AddMonths(-1);
                }
                do
                {
                    no--;
                } while (noPrintNumbers.ContainsKey(no));
            }
            return no;
        }

        public static int GetQuarterlyPrintNumber(Date date, int refNumber, Date refDate)
        {
            return refNumber + (date.AbsoluteMonth - refDate.AbsoluteMonth) / 3;
        }

        public static Date GetWeeklyPrintDate(int printNumber, int refNumber, Date refDate)
        {
            return refDate.AddDays((printNumber - refNumber) * 7);
        }

        public static Date GetEveryTwoWeekPrintDate(int printNumber, int refNumber, Date refDate)
        {
            return refDate.AddDays((printNumber - refNumber) * 14);
        }

        public static Date GetMonthlyPrintDate(int printNumber, int refNumber, Date refDate, Month noPrintMonths, Dictionary<int, int> noPrintDates, Dictionary<int, int> noPrintNumbers)
        {
            int no = refNumber;
            Date date = refDate;
            while (printNumber > no)
            {
                date = date.AddMonths(1);
                Month month = zdate.GetMonth(date.Month);
                if ((noPrintMonths & month) != month && !noPrintDates.ContainsKey(date.AbsoluteDay))
                {
                    do
                    {
                        no++;
                    } while (noPrintNumbers.ContainsKey(no));
                }
            }
            while (printNumber < no)
            {
                date = date.AddMonths(-1);
                Month month = zdate.GetMonth(date.Month);
                if ((noPrintMonths & month) != month && !noPrintDates.ContainsKey(date.AbsoluteDay))
                {
                    do
                    {
                        no--;
                    } while (noPrintNumbers.ContainsKey(no));
                }
            }
            return date;
        }

        public static Date GetMultiMonthlyPrintDate(int printNumber, int refNumber, Date refDate, Month noPrintMonths, Dictionary<int, int> noPrintDates, Dictionary<int, int> noPrintNumbers, int monthFreq)
        {
            int no = refNumber;
            Date date = refDate;
            while (printNumber > no)
            {
                Month month;
                for (int i = 0; i < monthFreq; i++)
                {
                    do
                    {
                        date = date.AddMonths(1);
                        month = zdate.GetMonth(date.Month);
                    } while ((noPrintMonths & month) == month || noPrintDates.ContainsKey(date.AbsoluteDay));
                }
                do
                {
                    no++;
                } while (noPrintNumbers.ContainsKey(no));
            }
            while (printNumber < no)
            {
                Month month;
                for (int i = 0; i < monthFreq; i++)
                {
                    do
                    {
                        date = date.AddMonths(-1);
                        month = zdate.GetMonth(date.Month);
                    } while ((noPrintMonths & month) == month || noPrintDates.ContainsKey(date.AbsoluteDay));
                }
                do
                {
                    no--;
                } while (noPrintNumbers.ContainsKey(no));
            }
            return date;
        }

        public static Date GetQuarterlyPrintDate(int printNumber, int refNumber, Date refDate)
        {
            return refDate.AddMonths((printNumber - refNumber) * 3);
        }
    }
}
