using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace pb
{
    public static class zconvert
    {
        private static SortedList<string, Encoding> gEncodings = null;
        public static Encoding GetEncoding(string sEncoding)
        {
            if (sEncoding == null) return null;
            InitEncodings();
            int i = gEncodings.IndexOfKey(sEncoding.ToLower());
            if (i == -1) return null;
            return gEncodings.Values[i];
        }

        public static void InitEncodings()
        {
            if (gEncodings != null) return;
            gEncodings = new SortedList<string, Encoding>();
            EncodingInfo[] encodings = Encoding.GetEncodings();
            foreach (EncodingInfo encoding in encodings)
            {
                string sName = encoding.Name.ToLower();
                if (!gEncodings.ContainsKey(sName))
                    gEncodings.Add(sName, encoding.GetEncoding());
            }
        }

        public static short LowValue(int value)
        {
            return (short)value;
        }

        public static short HightValue(int value)
        {
            return (short)(value >> 16);
        }

        public static int MakeValue(int lowValue, int hightValue)
        {
            return MakeValue((short)lowValue, (short)hightValue);
        }

        public static int MakeValue(short lowValue, short hightValue)
        {
            return (int)((uint)(hightValue << 16) + (ushort)lowValue);
        }

        //public static string s(object o)
        //{
        //    if (o == null) return "";
        //    switch (Type.GetTypeCode(o.GetType()))
        //    {
        //        case TypeCode.Boolean:
        //            return s((Boolean)o);
        //        //case TypeCode.Byte:
        //        //case TypeCode.Char:
        //        case TypeCode.DateTime:
        //            return s((DateTime)o);
        //        case TypeCode.DBNull:
        //            return "";
        //        //case TypeCode.Decimal:
        //        //case TypeCode.Double:
        //        //case TypeCode.Empty:
        //        //case TypeCode.Int16:
        //        case TypeCode.Int32:
        //            return s((int)o);
        //        //case TypeCode.Int64:
        //        //case TypeCode.Object:
        //        //case TypeCode.SByte:
        //        //case TypeCode.Single:
        //        //case TypeCode.String:
        //        //case TypeCode.UInt16:
        //        //case TypeCode.UInt32:
        //        //case TypeCode.UInt64:
        //    }
        //    return null;
        //}

        //public static string s(object o, string sFormat)
        //{
        //    return s(o, sFormat, "");
        //}

        //public static string s(object o, string sFormat, string sFmtNull)
        //{
        //    if (o == null) return sFmtNull;
        //    switch (Type.GetTypeCode(o.GetType()))
        //    {
        //        case TypeCode.Boolean:
        //            return s((Boolean)o, sFormat);
        //        //case TypeCode.Byte:
        //        //case TypeCode.Char:
        //        case TypeCode.DateTime:
        //            return s((DateTime)o, sFormat);
        //        case TypeCode.DBNull:
        //            return sFmtNull;
        //        //case TypeCode.Decimal:
        //        //case TypeCode.Double:
        //        //case TypeCode.Empty:
        //        //case TypeCode.Int16:
        //        case TypeCode.Int32:
        //            return s((int)o, sFormat);
        //        case TypeCode.Int64:
        //            return s((long)o, sFormat);
        //        //case TypeCode.Object:
        //        //case TypeCode.SByte:
        //        //case TypeCode.Single:
        //        //case TypeCode.String:
        //        //case TypeCode.UInt16:
        //        //case TypeCode.UInt32:
        //        //case TypeCode.UInt64:
        //        default:
        //            return string.Format("{0}", o);
        //    }
        //}

        //public static string s(object o, string sFmtDateTime, string sFmtBool, string sFmtInt, string sFmtNull)
        //{
        //    if (o == null) return sFmtNull;
        //    switch (Type.GetTypeCode(o.GetType()))
        //    {
        //        case TypeCode.Boolean:
        //            return s((Boolean)o, sFmtBool);
        //        //case TypeCode.Byte:
        //        //case TypeCode.Char:
        //        case TypeCode.DateTime:
        //            return s((DateTime)o, sFmtDateTime);
        //        case TypeCode.DBNull:
        //            return sFmtNull;
        //        //case TypeCode.Decimal:
        //        //case TypeCode.Double:
        //        //case TypeCode.Empty:
        //        //case TypeCode.Int16:
        //        case TypeCode.Int32:
        //            return s((int)o, sFmtInt);
        //        case TypeCode.Int64:
        //            return s((long)o, sFmtInt);
        //        //case TypeCode.Object:
        //        //case TypeCode.SByte:
        //        //case TypeCode.Single:
        //        //case TypeCode.String:
        //        //case TypeCode.UInt16:
        //        //case TypeCode.UInt32:
        //        //case TypeCode.UInt64:
        //        default:
        //            return o.ToString();
        //    }
        //}

        //public static string s(int i)
        //{
        //    return s(i, null);
        //}

        //public static string s(int i, string sFormat)
        //{
        //    if (sFormat == null) sFormat = "G";
        //    return i.ToString(sFormat);
        //}

        //public static string s(long l)
        //{
        //    return s(l, null);
        //}

        //public static string s(long l, string sFormat)
        //{
        //    if (sFormat == null) sFormat = "G";
        //    return l.ToString(sFormat);
        //}

        //public static string s(bool b)
        //{
        //    return s(b, null);
        //}

        //public static string s(bool b, bool bBool01)
        //{
        //    if (bBool01)
        //    {
        //        if (b) return "1"; else return "0";
        //    }
        //    else
        //        return b.ToString();
        //}

        //public static string s(bool b, string sFormat)
        //{
        //    return s(b, sFormat == "01");
        //}

        //public static string s(DateTime Date)
        //{
        //    return s(Date, null);
        //}

        //public static string s(DateTime Date, string sFormat)
        //{
        //    IFormatProvider format;

        //    if (sFormat == null) sFormat = "dd/MM/yyyy";
        //    format = new CultureInfo("fr-FR", true);
        //    return Date.ToString(sFormat, format);
        //}

        //public static long Long(string s, long Defaut)
        //{
        //    return Int64(s, Defaut);
        //}

        //public static long Long(string s)
        //{
        //    return Int64(s, 0);
        //}

        //public static int Int(string s, int Defaut)
        //{
        //    return Int32(s, Defaut);
        //}

        //public static int Int(string s)
        //{
        //    return Int32(s, 0);
        //}

        //public static Int64 Int64(string s, Int64 iDefaut)
        //{
        //    if (s == null) return iDefaut;
        //    Int64 i;
        //    if (System.Int64.TryParse(s, out i)) return i;
        //    return iDefaut;
        //}

        //public static Int64 Int64(string s)
        //{
        //    return Int64(s, 0);
        //}

        //public static Int32 Int32(string s, Int32 iDefaut)
        //{
        //    if (s == null) return iDefaut;
        //    Int32 i;
        //    if (System.Int32.TryParse(s, out i))
        //        return i;
        //    return iDefaut;
        //}

        //public static Int32 Int32(string s)
        //{
        //    return Int32(s, 0);
        //}

        //public static Int16 Int16(string s, Int16 iDefaut)
        //{
        //    Int16 i;
        //    if (System.Int16.TryParse(s, out i)) return i;
        //    return iDefaut;
        //}

        //public static Int16 Int16(string s)
        //{
        //    return Int16(s, 0);
        //}

        //public static object Date(string sDate)
        //{
        //    return Datetime(sDate, "dd/MM/yyyy");
        //}

        //public static object Date(string sDate, string sFormat)
        //{
        //    return Datetime(sDate, sFormat);
        //}

        //public static object Date(string sDate, string[] sFormat)
        //{
        //    return Datetime(sDate, sFormat);
        //}

        //public static object Time(string sTime)
        //{
        //    return Time(sTime, "HH:mm");
        //}

        //public static object Time(string sTime, string sFormat)
        //{
        //    object o;

        //    o = Datetime(sTime, sFormat);
        //    if (o == null) return null;
        //    return ((DateTime)o).TimeOfDay;
        //}

        //public static object Datetime(string sDateTime, string sFormat)
        //{
        //    DateTime dt;
        //    IFormatProvider format;

        //    format = new CultureInfo("fr-FR", true);
        //    if (DateTime.TryParseExact(sDateTime, sFormat, format, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out dt))
        //        return dt;
        //    return null;
        //}

        //public static object Datetime(string sDateTime, string[] sFormat)
        //{
        //    DateTime dt;
        //    IFormatProvider format;

        //    format = new CultureInfo("fr-FR", true);
        //    if (DateTime.TryParseExact(sDateTime, sFormat, format, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out dt))
        //        return dt;
        //    return null;
        //}

        //public static DateTime DiffDateTime(DateTime d1, DateTime d2)
        //{
        //    return new DateTime(d2.Ticks - d1.Ticks);
        //}

        //public static char[] ArrayByteToChar(byte[] tby)
        //{
        //    char[] tc;
        //    int i, nb;

        //    nb = tby.GetUpperBound(0) + 1;
        //    tc = new Char[nb];
        //    for (i = 0; i < nb; i++) tc[i] = (char)tby[i];
        //    return tc;
        //}

        //public static string ArrayByteToString(byte[] tby)
        //{
        //    char[] tc;
        //    int i, nb;
        //    string s;

        //    nb = tby.GetUpperBound(0) + 1;
        //    tc = new Char[nb];
        //    for (i = 0; i < nb; i++) tc[i] = (char)tby[i];
        //    s = new string(tc);
        //    return s;
        //}

        public static bool IsInt(string s)
        {
            int i;

            for (i = 0; i < s.Length; i++) if (!char.IsDigit(s, i)) return false;
            return true;
        }

        //public static int GetInt(string s)
        //{
        //    return GetInt(s, 0);
        //}

        //public static int GetInt(string s, int iNoInt)
        //{
        //    int i;
        //    for (i = 0; i < s.Length; i++) if (!char.IsDigit(s, i)) break;
        //    if (i == 0) return iNoInt;
        //    return int.Parse(s.Substring(0, i));
        //}

        public static object DefaultValue(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return false;
                case TypeCode.Byte:
                    return (byte)0;
                case TypeCode.Char:
                    return (char)0;
                case TypeCode.DateTime:
                    return new DateTime(1900, 1, 1);
                case TypeCode.Decimal:
                    return (decimal)0;
                case TypeCode.Double:
                    return (double)0;
                case TypeCode.Int16:
                    return (short)0;
                case TypeCode.Int32:
                    return (int)0;
                case TypeCode.Int64:
                    return (long)0;
                case TypeCode.SByte:
                    return (sbyte)0;
                case TypeCode.Single:
                    return (float)0;
                case TypeCode.String:
                    return "";
                case TypeCode.UInt16:
                    return (ushort)0;
                case TypeCode.UInt32:
                    return (uint)0;
                case TypeCode.UInt64:
                    return (ulong)0;
                //case TypeCode.Object:
                //case TypeCode.DBNull:
                //case TypeCode.Empty:
            }
            return null;
        }

        //public static bool Parse(string sValue, bool bDefault)
        //{
        //    return Parse(sValue, bDefault, null);
        //}

        //public static bool Parse(string sValue, bool bDefault, string sFormat)
        //{
        //    bool bValue;
        //    if (TryParse(sValue, out bValue, sFormat))
        //        return bValue;
        //    else
        //        return bDefault;
        //}

        //public static bool TryParse(string stringValue, out bool boolValue)
        //{
        //    return TryParse(stringValue, out boolValue, null);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="boolValue"></param>
        /// <param name="format">"01" où "tf", si sFormat n'a pas de valeur les 2 formats sont possible</param>
        /// <returns></returns>
        //public static bool TryParse(string stringValue, out bool boolValue, string format)
        //{
        //    if (stringValue == null)
        //    {
        //        boolValue = false;
        //        return false;
        //    }
        //    int i;
        //    if (format == "01")
        //    {
        //        if (int.TryParse(stringValue, out i))
        //        {
        //            if (i == 0) { boolValue = false; return true; }
        //            if (i == 1) { boolValue = true; return true; }
        //        }
        //    }
        //    else
        //    {
        //        if (string.Compare(stringValue, bool.FalseString, true) == 0) { boolValue = false; return true; }
        //        if (string.Compare(stringValue, bool.TrueString, true) == 0) { boolValue = true; return true; }
        //        if (format == "" || format == null)
        //        {
        //            if (int.TryParse(stringValue, out i))
        //            {
        //                if (i == 0) { boolValue = false; return true; }
        //                if (i == 1) { boolValue = true; return true; }
        //            }
        //        }
        //    }
        //    boolValue = false;
        //    return false;
        //}

        //public static bool TryParse(string stringValue, out TimeSpan timeSpanValue, string format)
        //{
        //    timeSpanValue = new TimeSpan();
        //    DateTime dt;
        //    //IFormatProvider formatProvider = new CultureInfo("fr-FR", true);
        //    if (format == null)
        //    {
        //        if (!DateTime.TryParse(stringValue, CultureInfo.CurrentCulture,
        //            DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out dt))
        //            return false;
        //    }
        //    else
        //    {
        //        if (!DateTime.TryParseExact(stringValue, format, CultureInfo.CurrentCulture,
        //            DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault, out dt))
        //            return false;
        //    }
        //    timeSpanValue = dt.TimeOfDay;
        //    return true;
        //}

        // modif $$pb fonction à supprimer
        //public static object TryParse(Type type, string sValue)
        //{
        //    switch (Type.GetTypeCode(type))
        //    {
        //        case TypeCode.Boolean:
        //            bool b;
        //            if (TryParse(sValue, out b)) return b;
        //            break;
        //        case TypeCode.Byte:
        //            //return byte.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            byte by;
        //            if (byte.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out by)) return by;
        //            break;
        //        case TypeCode.Char:
        //            //return char.Parse(sValue);
        //            char c;
        //            if (char.TryParse(sValue, out c)) return c;
        //            break;
        //        case TypeCode.DateTime:
        //            //return DateTime.Parse(sValue, DateTimeFormatInfo.InvariantInfo);
        //            DateTime dt;
        //            if (DateTime.TryParse(sValue, DateTimeFormatInfo.InvariantInfo,
        //                DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces, out dt))
        //                return dt;
        //            break;
        //        case TypeCode.Decimal:
        //            //return Decimal.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            Decimal dec;
        //            if (Decimal.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out dec)) return dec;
        //            break;
        //        case TypeCode.Double:
        //            //return Double.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            Double d;
        //            if (Double.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out d)) return d;
        //            break;
        //        case TypeCode.Int16:
        //            //return short.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            short sh;
        //            if (short.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out sh)) return sh;
        //            break;
        //        case TypeCode.Int32:
        //            //return int.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            int i;
        //            if (int.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out i)) return i;
        //            break;
        //        case TypeCode.Int64:
        //            //return long.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            long l;
        //            if (long.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out l)) return l;
        //            break;
        //        case TypeCode.SByte:
        //            //return sbyte.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            sbyte sby;
        //            if (sbyte.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out sby)) return sby;
        //            break;
        //        case TypeCode.Single:
        //            //return float.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            float f;
        //            if (float.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out f)) return f;
        //            break;
        //        case TypeCode.String:
        //            return sValue;
        //        case TypeCode.UInt16:
        //            //return ushort.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            ushort ush;
        //            if (ushort.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out ush)) return ush;
        //            break;
        //        case TypeCode.UInt32:
        //            //return uint.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            uint ui;
        //            if (uint.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out ui)) return ui;
        //            break;
        //        case TypeCode.UInt64:
        //            //return ulong.Parse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo);
        //            ulong ul;
        //            if (ulong.TryParse(sValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out ul)) return ul;
        //            break;
        //        //return Convert.ToByte(sValue);
        //        //case TypeCode.Object:
        //        //case TypeCode.DBNull:
        //        //case TypeCode.Empty:
        //    }
        //    return null;
        //}

        //public static string ToHex(byte? v)
        //{
        //    if (v == null)
        //        return null;
        //    else
        //        return ToHex((byte)v);
        //}

        //public static string ToHex(byte v, bool lowercase = false)
        //{
        //    string s = "0" + v.ToString("X");
        //    return s.Substring(s.Length - 2);
        //}

        //public static string ToHex(short v)
        //{
        //    string s = "000" + v.ToString("X");
        //    return s.Substring(s.Length - 4);
        //}

        //public static string ToHex(ushort v)
        //{
        //    string s = "000" + v.ToString("X");
        //    return s.Substring(s.Length - 4);
        //}

        //public static string ToHex(int v)
        //{
        //    string s = "0000000" + v.ToString("X");
        //    return s.Substring(s.Length - 8);
        //}

        //public static string ToHex(uint v)
        //{
        //    string s = "0000000" + v.ToString("X");
        //    return s.Substring(s.Length - 8);
        //}

        //public static string ToHex(long v)
        //{
        //    string s = "000000000000000" + v.ToString("X");
        //    return s.Substring(s.Length - 16);
        //}

        //public static string ToHex(ulong v)
        //{
        //    string s = "000000000000000" + v.ToString("X");
        //    return s.Substring(s.Length - 16);
        //}
    }

    public static partial class GlobalExtension
    {
        public static string zToHex(this byte? v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (v == null)
                return null;
            else if (lowercase)
                return ((byte)v).ToString("x2");
            else
                return ((byte)v).ToString("X2");
        }

        public static string zToHex(this byte v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (lowercase)
                return v.ToString("x2");
            else
                return v.ToString("X2");
        }

        public static string zToHex(this byte[] values, bool lowercase = false)
        {
            StringBuilder sb = new StringBuilder();
            if (lowercase)
            {
                foreach (byte value in values)
                    sb.Append(value.ToString("x2"));
            }
            else
            {
                foreach (byte value in values)
                    sb.Append(value.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string zToHex(this short v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (lowercase)
                return v.ToString("x4");
            else
                return v.ToString("X4");
        }

        public static string zToHex(this ushort v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (lowercase)
                return v.ToString("x4");
            else
                return v.ToString("X4");
        }

        public static string zToHex(this int v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (lowercase)
                return v.ToString("x8");
            else
                return v.ToString("X8");
        }

        public static string zToHex(this uint v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (lowercase)
                return v.ToString("x8");
            else
                return v.ToString("X8");
        }

        public static string zToHex(this long v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (lowercase)
                return v.ToString("x16");
            else
                return v.ToString("X16");
        }

        public static string zToHex(this ulong v, bool lowercase = false)
        {
            //return zconvert.ToHex(v);
            if (lowercase)
                return v.ToString("x16");
            else
                return v.ToString("X16");
        }

        public static string zTime(this int seconds, string format = "HH:mm")
        {
            return new DateTime().AddSeconds(seconds).ToString(format);
        }

        public static string zTime(this int? seconds, string format = "HH:mm")
        {
            if (seconds == null)
                return null;
            else
                return new DateTime().AddSeconds(seconds.Value).ToString(format);
        }
    }
}
