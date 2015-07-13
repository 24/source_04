using System;
using System.Globalization;

namespace pb
{
    public static class zparse
    {
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
                                return TimeSpan.Parse(text);
                            else
                                return TimeSpan.ParseExact(text, format, CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            if (format == null)
                            {
                                if (TimeSpan.TryParse(text, out timeSpanValue))
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
            switch (Type.GetTypeCode(asType))
            {
                case TypeCode.Boolean:
                    bool boolValue;
                    if (!tryParse)
                    {
                        boolValue = bool.Parse(text);
                        return __refvalue(__makeref(boolValue), T);
                    }
                    else if (bool.TryParse(text, out boolValue))
                        return __refvalue(__makeref(boolValue), T);
                    else
                        return defaultValue;
                //case TypeCode.SByte:
                //case TypeCode.Byte:
                //case TypeCode.Int16:
                //case TypeCode.UInt16:
                case TypeCode.Int32:
                    int intValue;
                    if (!tryParse)
                    {
                        intValue = int.Parse(text);
                        return __refvalue(__makeref(intValue), T);
                    }
                    else if (int.TryParse(text, out intValue))
                        return __refvalue(__makeref(intValue), T);
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
                            dateTimeValue = DateTime.Parse(text);
                        else
                            dateTimeValue = DateTime.ParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                        return __refvalue(__makeref(dateTimeValue), T);
                    }
                    else
                    {
                        if (format == null)
                        {
                            if (DateTime.TryParse(text, out dateTimeValue))
                                return __refvalue(__makeref(dateTimeValue), T);
                            else
                                return defaultValue;
                        }
                        else
                        {
                            if (DateTime.TryParseExact(text, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dateTimeValue))
                                return __refvalue(__makeref(dateTimeValue), T);
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
                                timeSpanValue = TimeSpan.Parse(text);
                            else
                                timeSpanValue = TimeSpan.ParseExact(text, format, CultureInfo.InvariantCulture);
                            return __refvalue(__makeref(timeSpanValue), T);
                        }
                        else
                        {
                            if (format == null)
                            {
                                if (TimeSpan.TryParse(text, out timeSpanValue))
                                    return __refvalue(__makeref(timeSpanValue), T);
                                else
                                    return defaultValue;
                            }
                            else
                            {
                                if (TimeSpan.TryParseExact(text, format, CultureInfo.InvariantCulture, out timeSpanValue))
                                    return __refvalue(__makeref(timeSpanValue), T);
                                else
                                    return defaultValue;
                            }
                        }
                    }
                    else
                        throw new PBException("can't parse string to {0}", asType.zGetTypeName());
            }
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
    }
}
