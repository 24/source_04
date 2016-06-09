using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Download.Print
{
    //class LeFigaroCreate : IPrintCreate
    //{
    //    public string GetName() { return "le_figaro"; }

    //    public IPrint CreatePrint(Dictionary<string, object> values)
    //    {
    //        return new LeFigaro(values);
    //    }
    //}

    class LeFigaro // : IPrint
    {
        private Date _date;
        private int _printNumber = 0;
        private static Date _refPrintDate = new Date(2013, 4, 25);
        private static int _refPrintNumber = 21376;

        public LeFigaro(Date date)
        {
            _date = date;
        }

        //NamedValues1
        public LeFigaro(NamedValues<ZValue> values)
        {
            if (!values.ContainsKey("day_near_current_date"))
            {
                if (!values.ContainsKey("year"))
                    throw new PBException("error creating LeFigaro unknow year");
                if (!values.ContainsKey("month"))
                    throw new PBException("error creating LeFigaro unknow month");
                if (!values.ContainsKey("day"))
                    throw new PBException("error creating LeFigaro unknow day");
            }
            _date = zdate.CreateDate(values);
        }

        public string GetName() { return "le_figaro"; }

        public int GetPrintNumber()
        {
            if (_printNumber == 0)
                _printNumber = GetPrintNumber(_date);
            return _printNumber;
        }

        public static bool PrintExists(Date date)
        {
            // Le figaro - 2013-04-25 - no 21376.pdf
            // pas de journal le dimanche, pas de journal le 1er mai
            //if (date.DayOfWeek != DayOfWeek.Sunday && (date.Month != 5 || date.Day != 1) && (date.Month != 5 || date.Day != 8) && date != SpecialDays.GetAscensionThursdayDate(date.Year)
            //    && date != SpecialDays.GetPentecostMondayDate(date.Year))
            if (date.DayOfWeek != DayOfWeek.Sunday && (date.Month != 5 || date.Day != 1))
                return true;
            else
                return false;
        }

        public static int GetPrintNumber(Date date)
        {
            return zprint.GetPrintNumber(PrintFrequency.Daily, date, _refPrintNumber, _refPrintDate, new PrintExistsDelegate(PrintExists));
        }

        public string GetFilename(int index = 0)
        {
            // Le figaro - 2013-04-25 - no 21376.pdf
            string file = string.Format("Le figaro - {0:yyyy-MM-dd} - no {1}", _date, GetPrintNumber());
            if (index != 0) file += "_" + index.ToString(); 
            return file + ".pdf";
        }
    }
}
