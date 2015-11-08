using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using pb.Data;
using pb.Reflection;
using pb.Text;

namespace pb
{
    public enum DateType
    {
        Unknow = 0,
        Day = 1,
        Month,
        Year
    }

    public class DateValues
    {
        public int? day = null;
        public int? month = null;
        public int? year = null;
        public int? dayNearCurrentDate = null;

        public static DateValues GetDateValues(NamedValues<ZValue> values)
        {
            DateValues dateValues = new DateValues();

            ZValue v;
            if (values.ContainsKey("day_near_current_date"))
            {
                v = values["day_near_current_date"];
                //if (v is ZStringArray)
                //    v = ((ZStringArray)v).SelectArrayValue();
                if (v is ZString)
                {
                    string s = (string)v;
                    s = s.Replace('o', '0');
                    s = s.Replace('O', '0');
                    int day;
                    if (int.TryParse(s, out day))
                        dateValues.dayNearCurrentDate = day;
                    else
                    {
                        //values.SetError("error creating Date day_near_current_date is'nt a number : \"{0}\"", v);
                        Trace.WriteLine("error creating Date day_near_current_date is'nt a number : \"{0}\"", v);
                        //return false;
                    }
                }
                else if (v is ZInt)
                    //day = (int)v;
                    dateValues.dayNearCurrentDate = (int)v;
                else
                {
                    //values.SetError("error creating Date day_near_current_date should be a string number or an int : {0}", v);
                    Trace.WriteLine("error creating Date day_near_current_date should be a string number or an int : {0}", v);
                    //return false;
                }
            }

            //string name = null;
            //if (values.ContainsKey("year"))
            //    name = "year";
            //else if (values.ContainsKey("year1"))
            //    name = "year1";
            //else if (values.ContainsKey("year2"))
            //    name = "year2";

            v = null;
            if (values.ContainsKey("year1"))
            {
                v = values["year1"];
                if (v is ZString && string.IsNullOrEmpty((string)v))
                    v = null;
            }
            if (v == null && values.ContainsKey("year2"))
            {
                v = values["year2"];
                if (v is ZString && string.IsNullOrEmpty((string)v))
                    v = null;
            }
            if (v == null && values.ContainsKey("year"))
            {
                v = values["year"];
                if (v is ZString && string.IsNullOrEmpty((string)v))
                    v = null;
            }

            if (v != null)
            {
                //if (v is ZStringArray)
                //    v = ((ZStringArray)v).SelectArrayValue();
                if (v is ZString)
                {
                    int year;
                    if (int.TryParse((string)v, out year))
                        dateValues.year = year;
                    else
                    {
                        //values.SetError("error creating Date year is'nt a number : \"{0}\"", v);
                        Trace.WriteLine("error creating Date year is'nt a number : \"{0}\"", v);
                        //return false;
                    }
                }
                else if (v is ZInt)
                    dateValues.year = (int)v;
                else if (v != null)
                {
                    //values.SetError("error creating Date year should be a string number or an int : {0}", v);
                    Trace.WriteLine("error creating Date year should be a string number or an int : {0}", v);
                    //return false;
                }
            }



            v = null;
            if (values.ContainsKey("month1"))
            {
                v = values["month1"];
                if (v is ZString && string.IsNullOrEmpty((string)v))
                    v = null;
            }
            if (v == null && values.ContainsKey("month2"))
            {
                v = values["month2"];
                if (v is ZString && string.IsNullOrEmpty((string)v))
                    v = null;
            }
            if (v == null && values.ContainsKey("month"))
            {
                v = values["month"];
                if (v is ZString && string.IsNullOrEmpty((string)v))
                    v = null;
            }

            if (v != null)
            {
                //if (v is ZStringArray)
                //    v = ((ZStringArray)v).SelectArrayValue();
                if (v is ZString)
                {
                    int month;
                    if (int.TryParse((string)v, out month))
                        dateValues.month = month;
                    else
                    {
                        month = zdate.GetMonthNumber((string)v);
                        if (month != 0)
                            dateValues.month = month;
                        else
                        {
                            //values.SetError("error creating Date invalid month : \"{0}\"", v);
                            Trace.WriteLine("error creating Date invalid month : \"{0}\"", v);
                            //return false;
                        }
                    }
                }
                else if (v is ZInt)
                    dateValues.month = (int)v;
                else if (v != null)
                {
                    //values.SetError("error creating Date month should be a string number or an int : {0}", v);
                    Trace.WriteLine("error creating Date month should be a string number or an int : {0}", v);
                    //return false;
                }
            }


            if (values.ContainsKey("day"))
            {
                v = values["day"];
                //if (v is ZStringArray)
                //    v = ((ZStringArray)v).SelectArrayValue();
                if (v is ZString)
                {
                    int day;
                    if ((string)v == "1er")
                        dateValues.day = 1;
                    else if (int.TryParse((string)v, out day))
                        dateValues.day = day;
                    else
                    {
                        //values.SetError("error creating Date day is'nt a number : \"{0}\"", v);
                        Trace.WriteLine("error creating Date day is'nt a number : \"{0}\"", v);
                        //return false;
                    }
                }
                else if (v is ZInt)
                {
                    dateValues.day = (int)v;
                }
                else if (v != null)
                {
                    //values.SetError("error creating Date day should be a string number or an int : {0}", v);
                    //Trace.WriteLine("error creating Date day should be a string number or an int : value {0} type {1}", v != null ? v : "(null)", v != null ? v.GetType().zName() : "(null)");
                    Trace.WriteLine("error creating Date day should be a string number or an int : value {0} type {1}", v, v.GetType().zGetTypeName());
                    //return false;
                }
            }



            return dateValues;
        }
    }

    public static class zdate
    {
        private static Dictionary<string, int> _month;

        static zdate()
        {
            initMonth();
        }

        public static DateTime AddTime(DateTime dtDate, DateTime dtTime)
        {
            TimeSpan ts;

            ts = GetTimeSpan(dtTime);
            return dtDate.Add(ts);
        }

        public static TimeSpan GetTimeSpan(DateTime dt)
        {
            DateTime dt2;

            dt2 = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
            return dt.Subtract(dt2);
        }

        public static DateTime GetDate(DateTime dt)
        {
            return dt.Date;
        }

        public static DateTime GetTime(DateTime dt)
        {
            return new DateTime(dt.TimeOfDay.Ticks);
        }

        public static void GetFrenchWeekYearNumber(DateTime dt, out int iWeekNumber, out int iYear)
        {
            int n, iDay, d;

            // le no de semaine de dt = no de semaine du jeudi de la semaine de dt

            // recherche le jour de la semaine de dt
            iDay = GetFrenchDayOfWeek(dt) - 1;
            // calcul du décalage
            d = 3 - iDay;
            // décalage de dt pour avoir le jeudi
            dt = dt.AddDays(d);

            // no du jour de l'année base 0
            n = dt.DayOfYear - 1;
            // recherche le jour de la semaine du 01/01/yyyy
            iDay = GetFrenchDayOfWeek(new DateTime(dt.Year, 1, 1)) - 1;
            // calcul du décalage
            d = (iDay + 3) % 7 - 3;
            // décalage du jour de l'année
            n += d;
            // calcul du no de semaine
            iWeekNumber = n / 7 + 1;
            // calcul de l'année
            iYear = dt.Year;
        }

        public static int GetFrenchWeekNumber(DateTime dt)
        {
            int iWeekNumber, iYear;

            GetFrenchWeekYearNumber(dt, out iWeekNumber, out iYear);
            return iWeekNumber;
        }

        public static DayOfWeek GetDayOfWeek(string day)
        {
            switch (day.ToLower())
            {
                case "monday":
                    return DayOfWeek.Monday;
                case "tuesday":
                    return DayOfWeek.Tuesday;
                case "wednesday":
                    return DayOfWeek.Wednesday;
                case "thursday":
                    return DayOfWeek.Thursday;
                case "friday":
                    return DayOfWeek.Friday;
                case "saturday":
                    return DayOfWeek.Saturday;
                case "sunday":
                    return DayOfWeek.Sunday;
            }
            throw new PBException("unknow day of week \"{0}\"", day);
        }

        public static int GetFrenchDayOfWeek(DateTime dt)
        {
            // renvoie : 1=Lu, 2=Ma, 3=Me, 4=Je, 5=Ve, 6=Sa, 7=Di
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                //case DayOfWeek.Sunday:
                default:
                    return 7;
            }
        }

        public static Date GetNearestWeekday(Date date, DayOfWeek weekday)
        {
            int i = (int)weekday - (int)date.DayOfWeek;
            if (i == 0)
                return date;
            if (i > 3)
                return date.AddDays(i - 7);
            if (i < -3)
                return date.AddDays(i + 7);
            return date.AddDays(i);
        }

        public static int ToFourDigitYear(int iYear)
        {
            GregorianCalendar gc;

            gc = new GregorianCalendar(GregorianCalendarTypes.TransliteratedFrench);
            //Debug.WriteLine(string.Format("TwoDigitYearMax = {0}", gc.TwoDigitYearMax));
            return gc.ToFourDigitYear(iYear);
        }

        public static bool IsDateValid(int year, int month, int day)
        {
            // DateTime.MinValue 01/01/0001
            // DateTime.MaxValue 31/12/9999
            if (year < 1 || year > 9999 || month < 1 || month > 12 || day < 1 || day > Date.DaysInMonth(year, month))
                return false;
            else
                return true;
        }

        public static bool IsDateValid(Dictionary<string, ZValue> values)
        {
            int day, month, year;
            //object o;
            ZValue v;

            if (values.ContainsKey("year"))
            {
                v = values["year"];
                //if (o is string)
                if (v is ZString)
                {
                    if (!int.TryParse((string)v, out year))
                        throw new PBException("error in Date values year is'nt a number : \"{0}\"", v);
                }
                else if (v is ZInt)
                    year = (int)v;
                else
                    throw new PBException("error in Date values year should be a string number or an int : {0}", v);
                int currentYear = Date.Today.Year;

                if (year < 100)
                    year = ToFourDigitYear(year);

                if (year > currentYear + 1 || year < currentYear - 5)
                    return false;
            }
            else
                year = Date.Today.Year;

            if (!values.ContainsKey("month"))
                throw new PBException("error no month in Date values");
            v = values["month"];
            //if (o is string)
            if (v is ZString)
            {
                if (!int.TryParse((string)v, out month))
                {
                    month = GetMonthNumber((string)v);
                    if (month == 0)
                        throw new PBException("error in Date values invalid month : \"{0}\"", v);
                }
            }
            else if (v is ZInt)
                month = (int)v;
            else
                throw new PBException("error in Date values month should be a string number or an int : {0}", v);
            if (month < 1 || month > 12)
                return false;

            if (values.ContainsKey("day"))
            {
                v = values["day"];
                //if (o is string)
                if (v is ZString)
                {
                    if ((string)v == "1er")
                        day = 1;
                    else if (!int.TryParse((string)v, out day))
                        throw new PBException("error in Date values day is'nt a number : \"{0}\"", v);
                }
                else if (v is ZInt)
                    day = (int)v;
                else
                    throw new PBException("error in Date values day should be a string number or an int : {0}", v);
                if (day < 1 || day > Date.DaysInMonth(year, month))
                    return false;
            }

            return true;
        }

        public static bool IsDayValid(Dictionary<string, ZValue> values)
        {
            int day;
            //object o = values["day_near_current_date"];
            ZValue v = values["day_near_current_date"];
            //if (o is string)
            if (v is ZString)
            {
                string s = (string)v;
                s = s.Replace('o', '0');
                s = s.Replace('O', '0');
                if (!int.TryParse(s, out day))
                    return false;
            }
            else if (v is ZInt)
                day = (int)v;
            else
                return false;
            Date current = Date.Today;
            Date previousMonth = current.AddMonths(-1);
            // date1 = day of current month, date2 = day of previous month
            return IsDateValid(current.Year, current.Month, day) || IsDateValid(previousMonth.Year, previousMonth.Month, day);
        }

        //public static Date CreateDate(NamedValues<ZValue> values, NamedValues<ZValue> param = null)
        public static Date CreateDate(NamedValues<ZValue> values)
        {
            Date date;
            DateType dateType;
            //if (TryCreateDate(values, out date, out dateType, param))
            if (TryCreateDate(values, out date, out dateType))
                return date;
            else
                throw new PBException(values.Error);
        }

        //public static bool TryCreateDate(NamedValues<ZValue> values, out Date date, out DateType dateType, NamedValues<ZValue> param = null)
        public static bool TryCreateDate(NamedValues<ZValue> values, out Date date, out DateType dateType)
        {
            date = Date.MinValue;
            dateType = DateType.Unknow;
            DateValues dateValues = DateValues.GetDateValues(values);
            //if (!values.ContainsKey("day_near_current_date") && !values.ContainsKey("month") && !values.ContainsKey("month1") && !values.ContainsKey("month2") && !values.ContainsKey("year"))
            if (dateValues.dayNearCurrentDate == null && dateValues.month == null && dateValues.year == null)
            {
                //values.SetError("error creating Date unknow day_near_current_date and month (month1 or month2)");
                //Trace.WriteLine("error creating Date unknow day_near_current_date unknow month (month1 and month2) and unknow year (year1 and year2)");
                return false;
            }

            //if (values.ContainsKey("day_near_current_date"))
            if (dateValues.dayNearCurrentDate != null)
            {
                //return TryCreateDateFromDay(values, out date, out dateType, param);
                return TryCreateDateFromDay(dateValues, out date, out dateType);
            }
            else
            {
                //return TryCreateDateFromYearMonthDay(values, out date, out dateType, param);
                return TryCreateDateFromYearMonthDay(dateValues, out date, out dateType);
            }
        }

        //public static bool TryCreateDateFromDay(NamedValues<ZValue> values, out Date date, out DateType dateType, NamedValues<ZValue> param = null)
        public static bool TryCreateDateFromDay(DateValues dateValues, out Date date, out DateType dateType, NamedValues<ZValue> param = null)
        {
            date = Date.MinValue;
            dateType = DateType.Unknow;
            //int day;
            //ZValue v = values["day_near_current_date"];
            //if (v is ZString)
            //{
            //    string s = (string)v;
            //    s = s.Replace('o', '0');
            //    s = s.Replace('O', '0');
            //    if (!int.TryParse(s, out day))
            //    {
            //        //throw new PBException("error creating Date day_near_current_date is'nt a number : \"{0}\"", o);
            //        values.SetError("error creating Date day_near_current_date is'nt a number : \"{0}\"", v);
            //        return false;
            //    }
            //}
            //else if (v is ZInt)
            //    day = (int)v;
            //else
            //{
            //    //throw new PBException("error creating Date day_near_current_date should be a string number or an int : {0}", o);
            //    values.SetError("error creating Date day_near_current_date should be a string number or an int : {0}", v);
            //    return false;
            //}

            //if (!TryGetDateFromDay(day, out date))
            if (!TryGetDateFromDay((int)dateValues.dayNearCurrentDate, out date))
            {
                //throw new PBException("error creating Date bad day_near_current_date : {0}", day);
                //values.SetError("error creating Date bad day_near_current_date : {0}", day);
                Trace.WriteLine("error creating Date bad day_near_current_date : {0}", dateValues.dayNearCurrentDate);
                return false;
            }
            dateType = DateType.Day;
            return true;
        }

        //public static bool TryCreateDateFromYearMonthDay(NamedValues<ZValue> values, out Date date, out DateType dateType, NamedValues<ZValue> param = null)
        public static bool TryCreateDateFromYearMonthDay(DateValues dateValues, out Date date, out DateType dateType)
        {
            date = Date.MinValue;
            dateType = DateType.Unknow;
            int year;
            int month = 1;
            int day = 1;

            bool foundYear = false;
            bool foundMonth = false;
            bool foundDay = false;

            //ZValue v;
            //if (values.ContainsKey("year"))
            //{
            //    foundYear = true;
            //    v = values["year"];
            //    if (v is ZString)
            //    {
            //        if (!int.TryParse((string)v, out year))
            //        {
            //            values.SetError("error creating Date year is'nt a number : \"{0}\"", v);
            //            return false;
            //        }
            //    }
            //    else if (v is ZInt)
            //        year = (int)v;
            //    else
            //    {
            //        values.SetError("error creating Date year should be a string number or an int : {0}", v);
            //        return false;
            //    }
            //}
            //else
            //    year = Date.Today.Year;

            if (dateValues.year != null)
            {
                foundYear = true;
                year = (int)dateValues.year;
            }
            else
                year = Date.Today.Year;


            //v = null;
            //if (values.ContainsKey("month1"))
            //{
            //    v = values["month1"];
            //    if (v is ZString && string.IsNullOrEmpty((string)v))
            //        v = null;
            //}
            //if (v == null && values.ContainsKey("month2"))
            //{
            //    v = values["month2"];
            //    if (v is ZString && string.IsNullOrEmpty((string)v))
            //        v = null;
            //}
            //if (v == null && values.ContainsKey("month"))
            //{
            //    v = values["month"];
            //    if (v is ZString && string.IsNullOrEmpty((string)v))
            //        v = null;
            //}
            //if (v != null)
            //{
            //    foundMonth = true;
            //    if (v is ZString)
            //    {
            //        if (!int.TryParse((string)v, out month))
            //        {
            //            month = GetMonthNumber((string)v);
            //            if (month == 0)
            //            {
            //                values.SetError("error creating Date invalid month : \"{0}\"", v);
            //                return false;
            //            }
            //        }
            //    }
            //    else if (v is ZInt)
            //        month = (int)v;
            //    else
            //    {
            //        values.SetError("error creating Date month should be a string number or an int : {0}", v);
            //        return false;
            //    }
            //}

            if (dateValues.month != null)
            {
                foundMonth = true;
                month = (int)dateValues.month;
            }


            //if (values.ContainsKey("day"))
            //{
            //    foundDay = true;
            //    v = values["day"];
            //    if (v is ZString)
            //    {
            //        if (!int.TryParse((string)v, out day))
            //        {
            //            values.SetError("error creating Date day is'nt a number : \"{0}\"", v);
            //            return false;
            //        }
            //    }
            //    else if (v is ZInt)
            //    {
            //        day = (int)v;
            //    }
            //    else
            //    {
            //        values.SetError("error creating Date day should be a string number or an int : {0}", v);
            //        return false;
            //    }
            //}

            if (dateValues.day != null)
            {
                foundDay = true;
                day = (int)dateValues.day;
            }



            if (year < 100)
                year = zdate.ToFourDigitYear(year);

            if (day > DateTime.DaysInMonth(year, month))
                return false;

            Date date3 = new Date(year, month, day);
            if (!foundYear)
            {
                if (date3 > Date.Today && month > Date.Today.Month + 2)
                {
                    if (day > DateTime.DaysInMonth(year - 1, month))
                        return false;

                    date3 = new Date(year - 1, month, day);
                }
            }
            date = date3;

            if (foundDay)
                dateType = DateType.Day;
            else if (foundMonth)
                dateType = DateType.Month;
            else
                dateType = DateType.Year;

            return true;
        }

        public static bool TryGetDateFromDay(int day, out Date date)
        {
            date = Date.MinValue;
            //Date current = Date.Today;
            //Date? date1 = null;
            //Date? date2 = null;
            Date current = Date.Today;
            Date previousMonth = current.AddMonths(-1);
            // date1 = day of current month
            bool date1Valid = IsDateValid(current.Year, current.Month, day);
            // date2 = day of previous month
            bool date2Valid = IsDateValid(previousMonth.Year, previousMonth.Month, day);
            //try
            //{
            //    date1 = new Date(current.Year, current.Month, day);
            //}
            //catch
            //{
            //}
            //try
            //{
            //    Date date3 = current.AddMonths(-1);
            //    date2 = new Date(date3.Year, date3.Month, day);
            //}
            //catch
            //{
            //}
            //if (date1 == null && date2 == null)
            if (!date1Valid && !date2Valid)
            {
                //throw new PBException("error creating Date bad day_near_current_date : {0}", day);
                //values.SetError("error creating Date bad day_near_current_date : {0}", day);
                return false;
            }
            //else if (date1 == null)
            else if (!date1Valid)
            {
                //return (Date)date2;
                if (day > DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month))
                    return false;

                date = new Date(previousMonth.Year, previousMonth.Month, day);
                return true;
            }
            //else if (date2 == null)
            else if (!date2Valid)
            {
                //return (Date)date1;
                if (day > DateTime.DaysInMonth(current.Year, current.Month))
                    return false;

                date = new Date(current.Year, current.Month, day);
                return true;
            }
            else // date1 and date2 are valid, choose date nearest from current date
            {
                if (day > DateTime.DaysInMonth(current.Year, current.Month))
                    return false;
                if (day > DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month))
                    return false;

                Date date1 = new Date(current.Year, current.Month, day);
                Date date2 = new Date(previousMonth.Year, previousMonth.Month, day);
                TimeSpan ts1 = current.Subtract(date1);
                TimeSpan ts2 = current.Subtract(date2);
                if (Math.Abs(ts1.Days) <= Math.Abs(ts2.Days) && date1 <= current.AddDays(1))
                {
                    //return (Date)date1;
                    date = date1;
                    return true;
                }
                else
                {
                    //return (Date)date2;
                    date = date2;
                    return true;
                }
            }
        }

        private static void initMonth()
        {
            _month = new Dictionary<string, int>();
            _month.Add("janvier", 1);
            _month.Add("janv", 1);
            _month.Add("jan", 1);
            _month.Add("février", 2);
            _month.Add("fevrier", 2);
            _month.Add("fév", 2);
            _month.Add("fev", 2);
            _month.Add("mars", 3);
            _month.Add("mar", 3);
            _month.Add("avril", 4);
            _month.Add("avr", 4);
            _month.Add("mai", 5);
            _month.Add("juin", 6);
            _month.Add("juillet", 7);
            _month.Add("juill", 7);
            _month.Add("juil", 7);
            _month.Add("août", 8);
            _month.Add("a0ût", 8);
            _month.Add("aout", 8);
            _month.Add("a0ut", 8);
            _month.Add("aoû", 8);
            _month.Add("a0û", 8);
            _month.Add("aou", 8);
            _month.Add("a0u", 8);
            _month.Add("septembre", 9);
            _month.Add("sptembre", 9);
            _month.Add("sept", 9);
            _month.Add("octobre", 10);
            _month.Add("oct", 10);
            _month.Add("novembre", 11);
            _month.Add("nov", 11);
            _month.Add("décembre", 12);
            _month.Add("decembre", 12);
            _month.Add("déc", 12);
            _month.Add("dec", 12);
            _month.Add("january", 1);
            _month.Add("february", 2);
            _month.Add("march", 3);
            _month.Add("april", 4);
            _month.Add("may", 5);
            _month.Add("june", 6);
            _month.Add("july", 7);
            _month.Add("august", 8);
            _month.Add("september", 9);
            _month.Add("october", 10);
            _month.Add("november", 11);
            _month.Add("december", 12);
            _month.Add("printemps", 4);  // du 20 ou 21 mars au 19 ou 20 juin
            _month.Add("ete", 7);        // du 20 ou 21 juin au 21 ou 22 septembre
            _month.Add("eté", 7);
            _month.Add("été", 7);
            _month.Add("automne", 10);    // du 22 ou 23 septembre au 20 ou 21 décembre
            _month.Add("hiver", 1);      // du 21 ou 22 décembre au 19 ou 20 mars
        }

        public static int GetMonthNumber(string month)
        {
            month = month.ToLower();
            if (_month.ContainsKey(month))
                return _month[month];
            else
                return 0;
        }

        public static Month GetMonth(string month)
        {
            switch (month.ToLower())
            {
                case "january":
                    return Month.January;
                case "february":
                    return Month.February;
                case "march":
                    return Month.March;
                case "april":
                    return Month.April;
                case "may":
                    return Month.May;
                case "june":
                    return Month.June;
                case "july":
                    return Month.July;
                case "august":
                    return Month.August;
                case "september":
                    return Month.September;
                case "october":
                    return Month.October;
                case "november":
                    return Month.November;
                case "december":
                    return Month.December;
                default:
                    return Month.NoMonth;
            }
            //throw new PBException("unknow month \"{0}\"", month);
        }

        public static Month GetMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return Month.January;
                case 2:
                    return Month.February;
                case 3:
                    return Month.March;
                case 4:
                    return Month.April;
                case 5:
                    return Month.May;
                case 6:
                    return Month.June;
                case 7:
                    return Month.July;
                case 8:
                    return Month.August;
                case 9:
                    return Month.September;
                case 10:
                    return Month.October;
                case 11:
                    return Month.November;
                case 12:
                    return Month.December;
            }
            throw new PBException("unknow month number {0}", month);
        }

        public static Date GetNearestYearDate(int day, int month, Date? refDate = null)
        {
            if (month < 1 || month > 12)
                throw new PBException("wrong month number {0}", month);
            if (day < 1)
                throw new PBException("wrong day number {0}", day);
            if (refDate == null)
                refDate = Date.Today;

            int year = refDate.Value.Year;
            int m = refDate.Value.Month - month;
            if (m < -6)
                year--;
            else if (m > 6)
                year++;
            if (day > DateTime.DaysInMonth(year, month))
                throw new PBException("wrong date day {0} month {1}", day, month);
            return new Date(year, month, day);
        }

        private static Regex _rgDate = new Regex(@"(aujourd['\s]?hui|hier)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static DateTime? ParseDateTimeLikeToday(string date, DateTime todayDate, params string[] formats)
        {
            // Aujourd'hui, 17:13
            if (date == null)
                return null;
            Match match = _rgDate.Match(date);
            if (match.Success)
            {
                string s = null;
                switch (match.Groups[1].Value.ToLower())
                {
                    case "aujourd'hui":
                    case "aujourd hui":
                    case "aujourdhui":
                        s = todayDate.ToString("dd-MM-yyyy");
                        break;
                    case "hier":
                        s = todayDate.AddDays(-1).ToString("dd-MM-yyyy");
                        break;
                }
                date = match.zReplace(date, s);
            }
            DateTime dt;
            // "d-M-yyyy, HH:mm" work with "9-9-2014, 23:25" and "09-09-2014, 23:25"
            //if (DateTime.TryParseExact(date, @"d-M-yyyy, HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
            if (DateTime.TryParseExact(date, formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                return dt;
            else
                return null;
        }

        private static DateTime __epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();  // 
        public static int DateTimeToUnixTimeStamp(DateTime dt)
        {
            return (int)(dt.ToLocalTime() - __epoch).TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(int timestamp)
        {
            return __epoch.AddSeconds(timestamp);
        }
    }
}
