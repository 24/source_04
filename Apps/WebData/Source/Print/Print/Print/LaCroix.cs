using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Download.Print
{

    //class LaCroixCreate : IPrintCreate
    //{
    //    public string GetName() { return "la_croix"; }

    //    public IPrint CreatePrint(Dictionary<string, object> values)
    //    {
    //        return new LaCroix(values);
    //    }
    //}

    class LaCroix // : IPrint
    {
        private Date _date;
        private int _printNumber = 0;
        private static Date _refPrintDate = new Date(2013, 5, 2);
        private static int _refPrintNumber = 39571;

        public LaCroix(Date date)
        {
            _date = date;
        }

        //NamedValues1
        public LaCroix(NamedValues<ZValue> values)
        {
            // year          : mandatory, int or numeric string
            // day           : mandatory, int or numeric string
            // month         : mandatory, int or numeric string
            // number        : not used, int or numeric string
            if (!values.ContainsKey("day_near_current_date"))
            {
                if (!values.ContainsKey("year"))
                    throw new PBException("error creating LaCroix unknow year");
                if (!values.ContainsKey("month"))
                    throw new PBException("error creating LaCroix unknow month");
                if (!values.ContainsKey("day"))
                    throw new PBException("error creating LaCroix unknow day");
            }
            _date = zdate.CreateDate(values);
        }

        public string GetName() { return "la_croix"; }

        public int GetPrintNumber()
        {
            if (_printNumber == 0)
                _printNumber = GetPrintNumber(_date);
            return _printNumber;
        }

        public static bool PrintExists(Date date)
        {
            // La croix - 2013-05-02 - no 39571.pdf
            // pas de journal le dimanche, pas de journal le 1er mai, le 8 mai, lundi de pentecote
            if (date.DayOfWeek != DayOfWeek.Sunday && (date.Month != 5 || date.Day != 1) && (date.Month != 5 || date.Day != 8) && date != SpecialDayTools.GetAscensionThursdayDate(date.Year)
                && date != SpecialDayTools.GetPentecostMondayDate(date.Year))
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
            // La croix - 2013-04-30 - no 39570.pdf
            string file = string.Format("La croix - {0:yyyy-MM-dd} - no {1}", _date, GetPrintNumber());
            if (index != 0) file += "_" + index.ToString(); 
            return file + ".pdf";
        }
    }
}
