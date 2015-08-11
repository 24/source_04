using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pb
{
    public class DateException : Exception
    {
        public DateException(string sMessage) : base(sMessage) { }
        public DateException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
        public DateException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
        public DateException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
    }

    [Flags]
    public enum Month
    {
        NoMonth        = 0x0000,
        January        = 0x0001,
        February       = 0x0002,
        March          = 0x0004,
        April          = 0x0008,
        May            = 0x0010,
        June           = 0x0020,
        July           = 0x0040,
        August         = 0x0080,
        September      = 0x0100,
        October        = 0x0200,
        November       = 0x0400,
        December       = 0x0800
    }

    [Serializable]
    public struct Date : IComparable<Date>, IComparable, IEquatable<Date>, IFormattable, IConvertible
    {
        private DateTime _date;

        public static readonly Date MaxValue = new Date(DateTime.MaxValue);
        public static readonly Date MinValue = new Date(DateTime.MinValue);

        public Date(DateTime date)
        {
            this._date = date.Date;
        }

        public Date(string date)
        {
            this._date = DateTime.Parse(date).Date;
        }

        public Date(long ticks)
        {
            _date = new DateTime(ticks).Date;
        }

        public Date(long ticks, DateTimeKind kind)
        {
            _date = new DateTime(ticks, kind).Date;
        }

        public Date(int year, int month, int day)
        {
            _date = new DateTime(year, month, day);
        }

        public Date(int year, int month, int day, Calendar calendar)
        {
            _date = new DateTime(year, month, day, calendar);
        }

        public DateTime DateTime
        {
            get { return _date; }
        }

        public int Day
        {
            get { return _date.Day; }
        }

        public DayOfWeek DayOfWeek
        {
            get { return _date.DayOfWeek; }
        }

        public int AbsoluteDay
        {
            get
            {
                // 10 000 000 ticks = 1 second
                // number of ticks in one day : 24 * 60 * 60 * 10 000 000 = 86 400 * 10 000 000 = 864 000 000 000
                return (int)(_date.Ticks / 864000000000L);
            }
        }

        public int AbsoluteWeek
        {
            get
            {
                // 10 000 000 ticks = 1 second
                // number of ticks in one week : 7 * 24 * 60 * 60 * 10 000 000 = 7 * 86 400 * 10 000 000 = 6 048 000 000 000
                return (int)(_date.Ticks / 6048000000000L);
            }
        }

        public int AbsoluteMonth
        {
            get
            {
                // DateTime.MinValue = 01/01/0001
                return (_date.Year - 1) * 12 + _date.Month - 1;
            }
        }

        public int DayOfYear
        {
            get { return _date.DayOfYear; }
        }

        public int Month
        {
            get { return _date.Month; }
        }

        public int Year
        {
            get { return _date.Year; }
        }

        public static explicit operator Date(DateTime date)
        {
            return new Date(date);
        }

        public static explicit operator DateTime(Date date)
        {
            return date._date;
        }

        public static explicit operator Date(string date)
        {
            return new Date(date);
        }

        public static explicit operator string(Date date)
        {
            return date.ToString();
        }

        public static TimeSpan operator -(Date d1, Date d2)
        {
            return d1._date - d2._date;
        }

        public static Date operator -(Date d, TimeSpan t)
        {
            return new Date(d._date - t);
        }

        public static Date operator +(Date d, TimeSpan t)
        {
            return new Date(d._date + t);
        }

        public static bool operator !=(Date d1, Date d2)
        {
            return d1._date != d2._date;
        }

        public static bool operator <(Date d1, Date d2)
        {
            return d1._date < d2._date;
        }

        public static bool operator <=(Date d1, Date d2)
        {
            return d1._date <= d2._date;
        }

        public static bool operator ==(Date d1, Date d2)
        {
            return d1._date == d2._date;
        }

        public static bool operator >(Date d1, Date d2)
        {
            return d1._date > d2._date;
        }

        public static bool operator >=(Date d1, Date d2)
        {
            return d1._date >= d2._date;
        }

        public Date Add(TimeSpan value)
        {
            return new Date(_date.Add(value));
        }

        public Date AddDays(double value)
        {
            return new Date(_date.AddDays(value));
        }

        public Date AddMonths(int month)
        {
            return new Date(_date.AddMonths(month));
        }

        public Date AddYears(int year)
        {
            return new Date(_date.AddYears(year));
        }

        public TimeSpan Subtract(Date value)
        {
            return _date.Subtract(value._date);
        }

        public TimeSpan Subtract(DateTime value)
        {
            return _date.Subtract(value);
        }

        public Date Subtract(TimeSpan value)
        {
            return new Date(_date.Subtract(value));
        }

        public long ToBinary()
        {
            return _date.ToBinary();
        }

        public long ToFileTime()
        {
            return _date.ToFileTime();
        }

        public string ToLongDateString()
        {
            return _date.ToLongDateString();
        }

        public string ToShortDateString()
        {
            return _date.ToShortDateString();
        }

        public double ToOADate()
        {
            return _date.ToOADate();
        }

        public override string ToString()
        {
            return _date.ToString(FormatInfo.CurrentFormat.ShortDatePattern);
        }

        public string ToString(string format)
        {
            return _date.ToString(format);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) format = FormatInfo.CurrentFormat.ShortDatePattern;
            return _date.ToString(format, formatProvider);
        }

        public int CompareTo(Date other)
        {
            return _date.CompareTo(other._date);
        }

        public int CompareTo(object obj)
        {
            return _date.CompareTo(((Date)obj)._date);
        }

        public bool Equals(Date other)
        {
            return _date.Equals(other._date);
        }

        public override bool Equals(object other)
        {
            //if (other == null)
            //    throw new DateException("error parameter is null");
            //if (!(other is Date))
            //    throw new DateException("error parameter is not a Date");
            if (other is Date)
                return _date.Equals(((Date)other)._date);
            return false;
        }

        public override int GetHashCode()
        {
            return _date.GetHashCode();
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public static Date Today
        {
            get { return new Date(DateTime.Today); }
        }

        public static Date UtcNow
        {
            get { return new Date(DateTime.UtcNow); }
        }

        public static int Compare(Date d1, Date d2)
        {
            return DateTime.Compare(d1._date, d2._date);
        }

        public static int DaysInMonth(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        public static bool Equals(Date d1, Date d2)
        {
            return DateTime.Equals(d1._date, d2._date);
        }

        public static new bool Equals(object d1, object d2)
        {
            if (d1 == null || d2 == null)
                throw new DateException("error parameter is null");
            if (!(d1 is Date) || !(d2 is Date))
                throw new DateException("error parameter is not a Date");
            return DateTime.Equals(((Date)d1)._date, ((Date)d2)._date);
        }

        public static Date FromBinary(long dateData)
        {
            return new Date(DateTime.FromBinary(dateData));
        }

        public static Date FromFileTime(long fileTime)
        {
            return new Date(DateTime.FromFileTime(fileTime));
        }

        public static Date FromFileTimeUtc(long fileTime)
        {
            return new Date(DateTime.FromFileTimeUtc(fileTime));
        }

        public static Date FromOADate(double d)
        {
            return new Date(DateTime.FromOADate(d));
        }

        public static bool IsLeapYear(int year)
        {
            return DateTime.IsLeapYear(year);
        }

        public static Date Parse(string date)
        {
            return new Date(date);
        }

        public static Date Parse(string date, IFormatProvider provider)
        {
            return new Date(DateTime.Parse(date, provider));
        }

        public static Date Parse(string date, IFormatProvider provider, DateTimeStyles styles)
        {
            return new Date(DateTime.Parse(date, provider, styles));
        }

        public static Date ParseExact(string date, string format, IFormatProvider provider)
        {
            return new Date(DateTime.ParseExact(date, format, provider));
        }

        public static Date ParseExact(string date, string format, IFormatProvider provider, DateTimeStyles styles)
        {
            return new Date(DateTime.ParseExact(date, format, provider, styles));
        }

        public static Date ParseExact(string date, string[] formats, IFormatProvider provider, DateTimeStyles styles)
        {
            return new Date(DateTime.ParseExact(date, formats, provider, styles));
        }

        public static bool TryParse(string date, out Date result)
        {
            DateTime dt;
            if (DateTime.TryParse(date, out dt))
            {
                result = new Date(dt);
                return true;
            }
            else
            {
                result = new Date();
                return false;
            }
        }

        public static bool TryParse(string date, IFormatProvider provider, DateTimeStyles styles, out Date result)
        {
            DateTime dt;
            if (DateTime.TryParse(date, provider, styles, out dt))
            {
                result = new Date(dt);
                return true;
            }
            else
            {
                result = new Date();
                return false;
            }
        }

        public static bool TryParseExact(string date, string format, IFormatProvider provider, DateTimeStyles styles, out Date result)
        {
            DateTime dt;
            if (DateTime.TryParseExact(date, format, provider, styles, out dt))
            {
                result = new Date(dt);
                return true;
            }
            else
            {
                result = new Date();
                return false;
            }
        }

        public static bool TryParseExact(string date, string[] formats, IFormatProvider provider, DateTimeStyles styles, out Date result)
        {
            DateTime dt;
            if (DateTime.TryParseExact(date, formats, provider, styles, out dt))
            {
                result = new Date(dt);
                return true;
            }
            else
            {
                result = new Date();
                return false;
            }
        }

        public static Date CreateDateFromAbsoluteDay(int absoluteDay)
        {
            // 10 000 000 ticks = 1 second
            // number of ticks in one day : 24 * 60 * 60 * 10 000 000 = 86 400 * 10 000 000 = 864 000 000 000
            //return (int)(_date.Ticks / 864000000000L);
            return new Date(absoluteDay * 864000000000L);
        }
    }

    public static class DateTimeExtension
    {
        public static Date zDate(this DateTime dt)
        {
            return new Date(dt);
        }

        public static TimeSpan Subtract(this DateTime dt, Date value)
        {
            return dt.Subtract(value.DateTime);
        }
    }
}
