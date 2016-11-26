using System;
using System.Globalization;
using pb.Reflection;
using System.Text.RegularExpressions;

namespace pb
{
    public static class zparse
    {
        public static TimeSpan CreateTimeSpan(int days, int hours, int minutes, int seconds, int ticks)
        {
            return TimeSpan.FromTicks( (((((((days * 24L) + hours) * 60L) + minutes) * 60L) + seconds) * 10000000L) + ticks );
        }

        // 1.10:30:40.12345
        // Groups[1] days, Groups[2] hours, Groups[3] minutes, Groups[4] seconds, Groups[5] seconds decimales
        private static Regex __timespan = new Regex(@"^(?:(?:(?:([0-9]+)\.)?(?:([0-9]{1,2}):))?(?:([0-9]{1,2}):))?([0-9]{1,2})(?:\.([0-9]{1,7}))?$", RegexOptions.Compiled);
        public static bool TryParseTimeSpan(string text, out TimeSpan timespan)
        {
            Match match = __timespan.Match(text);
            if (match.Success)
            {
                //long ticks = 0;

                //// days
                //string group = match.Groups[1].Value;
                //if (group != "")
                //    ticks += int.Parse(group);

                //// hours
                //ticks *= 24;
                //group = match.Groups[2].Value;
                //if (group != "")
                //    ticks += int.Parse(group);

                //// minutes
                //ticks *= 60;
                //group = match.Groups[3].Value;
                //if (group != "")
                //    ticks += int.Parse(group);

                //// seconds
                //ticks *= 60;
                //group = match.Groups[4].Value;
                //if (group != "")
                //    ticks += int.Parse(group);

                //// seconds decimales 1 234 567
                //ticks *= 10000000;
                //group = match.Groups[5].Value;
                //if (group != "")
                //    ticks += int.Parse(group.PadRight(7, '0'));

                //timespan = TimeSpan.FromTicks(ticks);

                int days = 0;
                string group = match.Groups[1].Value;
                if (group != "")
                    days = int.Parse(group);

                int hours = 0;
                group = match.Groups[2].Value;
                if (group != "")
                    hours = int.Parse(group);

                int minutes = 0;
                group = match.Groups[3].Value;
                if (group != "")
                    minutes = int.Parse(group);

                int seconds = 0;
                group = match.Groups[4].Value;
                if (group != "")
                    seconds = int.Parse(group);

                // ticks 1 234 567
                int ticks = 0;
                group = match.Groups[5].Value;
                if (group != "")
                    ticks = int.Parse(group.PadRight(7, '0'));

                timespan = CreateTimeSpan(days, hours, minutes, seconds, ticks);
                return true;
            }
            else
            {
                timespan = TimeSpan.Zero;
                return false;
            }
        }

        public static object ParseAs(string text, Type asType, object defaultValue = null, bool tryParse = false, string format = null)
        {
            if (text == null)
            {
                if (tryParse)
                    return defaultValue;
                else
                    throw new PBException("can't parse null string to {0}", asType.zGetTypeName());
            }
            switch (Type.GetTypeCode(asType))
            {
                case TypeCode.Boolean:
                    bool boolValue;
                    if (!tryParse)
                        return bool.Parse(text);
                    else if (bool.TryParse(text, out boolValue))
                        return boolValue;
                    else
                        return defaultValue;
                //case TypeCode.SByte:
                //case TypeCode.Byte:
                //case TypeCode.Int16:
                //case TypeCode.UInt16:
                case TypeCode.Int32:
                    int intValue;
                    if (!tryParse)
                        return int.Parse(text);
                    else if (int.TryParse(text, out intValue))
                        return intValue;
                    else
                        return defaultValue;
                //case TypeCode.UInt32:
                //case TypeCode.Int64:
                //case TypeCode.UInt64:
                //case TypeCode.Single:
                //case TypeCode.Double:
                //case TypeCode.Decimal:
                case TypeCode.DateTime:
                    DateTime dateTimeValue;
                    if (!tryParse)
                    {
                        if (format == null)
                            return DateTime.Parse(text);
                        else
                            return DateTime.ParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                    }
                    else
                    {
                        if (format == null)
                        {
                            if (DateTime.TryParse(text, out dateTimeValue))
                                return dateTimeValue;
                            else
                                return defaultValue;
                        }
                        else
                        {
                            if (DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTimeValue))
                                return dateTimeValue;
                            else
                                return defaultValue;
                        }
                    }
                default:
                    if (asType == typeof(TimeSpan))
                    {
                        TimeSpan timeSpanValue;
                        if (!tryParse)
                        {
                            if (format == null)
                            {
                                //return TimeSpan.Parse(text);
                                if (TryParseTimeSpan(text, out timeSpanValue))
                                    return timeSpanValue;
                                else
                                    throw new PBException("wrong TimeSpan value \"{0}\"", text);

                            }
                            else
                                return TimeSpan.ParseExact(text, format, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            if (format == null)
                            {
                                //if (TimeSpan.TryParse(text, out timeSpanValue))
                                if (TryParseTimeSpan(text, out timeSpanValue))
                                    return timeSpanValue;
                                else
                                    return defaultValue;
                            }
                            else
                            {
                                if (TimeSpan.TryParseExact(text, format, CultureInfo.InvariantCulture, out timeSpanValue))
                                    return timeSpanValue;
                                else
                                    return defaultValue;
                            }
                        }
                    }
                    else
                        throw new PBException("can't parse string to {0}", asType.zGetTypeName());
            }
        }

        public static T ParseAs<T>(string text, T defaultValue = default(T), bool tryParse = false, string format = null)
        {
            Type asType = typeof(T);
            if (text == null)
            {
                if (tryParse)
                    return defaultValue;
                else
                    throw new PBException("can't parse null string to {0}", asType.zGetTypeName());
            }

            //bool isNullable = false;
            //if (asType.IsGenericType && asType.GetGenericTypeDefinition() == typeof(Nullable<>))
            //{
            //    asType = asType.GetGenericArguments()[0];
            //    isNullable = true;
            //}
            // nullable type
            Type nullableType = zReflection.GetNullableType(asType);
            bool isNullable = false;
            if (nullableType != null)
            {
                asType = nullableType;
                isNullable = true;
            }

            bool parse;
            switch (Type.GetTypeCode(asType))
            {
                case TypeCode.Boolean:
                    //bool boolValue;
                    //if (!tryParse)
                    //{
                    //    boolValue = bool.Parse(text);
                    //    if (!isNullable)
                    //        return __refvalue( __makeref(boolValue),T);
                    //    else
                    //    {
                    //        bool? nullableBoolValue = boolValue;
                    //        return __refvalue( __makeref(nullableBoolValue),T);
                    //    }
                    //}
                    //else if (bool.TryParse(text, out boolValue))
                    //    return __refvalue(__makeref(boolValue), T);
                    //else
                    //    return defaultValue;
                    bool boolValue;
                    if (bool.TryParse(text, out boolValue))
                    {
                        if (!isNullable)
                            return __refvalue( __makeref(boolValue),T);
                        else
                        {
                            bool? nullableBoolValue = boolValue;
                            return __refvalue( __makeref(nullableBoolValue),T);
                        }
                    }
                    else if (tryParse)
                        return defaultValue;
                    else
                        throw new PBException("can't parse \"{0}\" to bool", text);

                //case TypeCode.SByte:
                //case TypeCode.Byte:
                //case TypeCode.Int16:
                //case TypeCode.UInt16:
                case TypeCode.Int32:
                    //int intValue;
                    //if (!tryParse)
                    //{
                    //    intValue = int.Parse(text);
                    //    return __refvalue(__makeref(intValue), T);
                    //}
                    //else if (int.TryParse(text, out intValue))
                    //    return __refvalue(__makeref(intValue), T);
                    //else
                    //    return defaultValue;
                    int intValue;
                    if (int.TryParse(text, out intValue))
                    {
                        if (!isNullable)
                            return __refvalue( __makeref(intValue),T);
                        else
                        {
                            int? nullableIntValue = intValue;
                            return __refvalue( __makeref(nullableIntValue),T);
                        }
                    }
                    else if (tryParse)
                        return defaultValue;
                    else
                        throw new PBException("can't parse \"{0}\" to int", text);

                //case TypeCode.UInt32:
                //case TypeCode.Int64:
                //case TypeCode.UInt64:
                //case TypeCode.Single:
                //case TypeCode.Double:
                //case TypeCode.Decimal:
                case TypeCode.DateTime:
                    //DateTime dateTimeValue;
                    //if (!tryParse)
                    //{
                    //    if (format == null)
                    //        dateTimeValue = DateTime.Parse(text);
                    //    else
                    //        dateTimeValue = DateTime.ParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                    //    return __refvalue(__makeref(dateTimeValue), T);
                    //}
                    //else
                    //{
                    //    if (format == null)
                    //    {
                    //        if (DateTime.TryParse(text, out dateTimeValue))
                    //            return __refvalue(__makeref(dateTimeValue), T);
                    //        else
                    //            return defaultValue;
                    //    }
                    //    else
                    //    {
                    //        if (DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTimeValue))
                    //            return __refvalue(__makeref(dateTimeValue), T);
                    //        else
                    //            return defaultValue;
                    //    }
                    //}
                    DateTime dateTimeValue;
                    if (format != null)
                        parse = DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTimeValue);
                    else
                        parse = DateTime.TryParse(text, out dateTimeValue);
                    if (parse)
                    {
                        if (!isNullable)
                            return __refvalue( __makeref(dateTimeValue),T);
                        else
                        {
                            DateTime? nullableDateTimeValue = dateTimeValue;
                            return __refvalue( __makeref(nullableDateTimeValue),T);
                        }
                    }
                    else if (tryParse)
                        return defaultValue;
                    else
                        throw new PBException("can't parse \"{0}\" to DateTime, format \"{1}\"", text, format);

                default:
                    if (asType == typeof(TimeSpan))
                    {
                        TimeSpan timeSpanValue;
                        if (format != null)
                            parse = TimeSpan.TryParseExact(text, format, CultureInfo.InvariantCulture, out timeSpanValue);
                        else
                            //parse = TimeSpan.TryParse(text, out timeSpanValue);
                            parse = TryParseTimeSpan(text, out timeSpanValue);
                        if (parse)
                        {
                            if (!isNullable)
                                return __refvalue( __makeref(timeSpanValue),T);
                        else
                            {
                                TimeSpan? nullableTimeSpanValue = timeSpanValue;
                                return __refvalue( __makeref(nullableTimeSpanValue),T);
                            }
                        }
                        else if (tryParse)
                            return defaultValue;
                        else
                            throw new PBException("can't parse \"{0}\" to TimeSpan, format \"{1}\"", text, format);
                    }
                    else
                        throw new PBException("can't parse string to {0}", asType.zGetTypeName());
            }
        }

        public static T ParseEnum<T>(string value, bool ignoreCase = true)
        {
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static T TryParseEnum<T>(string value, T defaultValue, bool ignoreCase = true)
        {
            if (value == null)
                return defaultValue;
            else
                return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }

    public static partial class GlobalExtension
    {
        public static T zParseAs<T>(this string text, string format = null)
        {
            return zparse.ParseAs<T>(text, tryParse: false, format: format);
        }

        public static T zTryParseAs<T>(this string text, T defaultValue = default(T), string format = null)
        {
            return zparse.ParseAs(text, defaultValue, tryParse: true, format: format);
        }

        public static object zObjectParseAs(this string text, Type asType, string format = null)
        {
            return zparse.ParseAs(text, asType, tryParse: false, format: format);
        }

        public static object zObjectTryParseAs(this string text, Type asType, object defaultValue = null, string format = null)
        {
            return zparse.ParseAs(text, asType, defaultValue, tryParse: true, format: format);
        }

        public static T zParseEnum<T>(this string value, bool ignoreCase = false)
        {
            return zparse.ParseEnum<T>(value, ignoreCase);
        }

        public static T zTryParseEnum<T>(this string value, T defaultValue, bool ignoreCase = true)
        {
            return zparse.TryParseEnum<T>(value, defaultValue, ignoreCase);
        }
    }
}
