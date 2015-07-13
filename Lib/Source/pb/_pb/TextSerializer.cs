using System;
using System.Globalization;

namespace pb
{
    public class TextSerializer
    {
        private static TextSerializer __currentTextSerializer = new TextSerializer();
        private static CultureInfo __defaultCultureInfo = CultureInfo.InvariantCulture;
        private static string __defaultByteFormat = null;
        private static string __defaultDateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        private static string __defaultDecimalFormat = null;
        private static string __defaultDoubleFormat = null;
        private static string __defaultShortFormat = null;
        private static string __defaultIntFormat = null;
        private static string __defaultLongFormat = null;
        private static string __defaultSbyteFormat = null;
        private static string __defaultFloatFormat = null;
        private static string __defaultUshortFormat = null;
        private static string __defaultUintFormat = null;
        private static string __defaultUlongFormat = null;
        private static string __defaultDateFormat = "dd-MM-yyyy";
        private static string __defaultTimeSpanFormat = "c";

        private CultureInfo _cultureInfo = __defaultCultureInfo;
        private string _byteFormat = __defaultByteFormat;
        private string _dateTimeFormat = __defaultDateTimeFormat;
        private string _decimalFormat = __defaultDecimalFormat;
        private string _doubleFormat = __defaultDoubleFormat;
        private string _shortFormat = __defaultShortFormat;
        private string _intFormat = __defaultIntFormat;
        private string _longFormat = __defaultLongFormat;
        private string _sbyteFormat = __defaultSbyteFormat;
        private string _floatFormat = __defaultFloatFormat;
        private string _ushortFormat = __defaultUshortFormat;
        private string _uintFormat = __defaultUintFormat;
        private string _ulongFormat = __defaultUlongFormat;
        private string _dateFormat = __defaultDateFormat;
        private string _timeSpanFormat = __defaultTimeSpanFormat;

        public static TextSerializer CurrentTextSerializer { get { return __currentTextSerializer; } }

        public string Serialize<T>(T value)
        {
            // value as DateTime : Error 104 - The as operator must be used with a reference type or nullable type ('System.DateTime' is a non-nullable value type)
            // (DateTime)value   : Error 102 - Cannot convert type 'T' to 'System.DateTime'

            Type type = typeof(T);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return __refvalue(__makeref(value), bool).ToString(_cultureInfo);
                case TypeCode.Byte:
                    return __refvalue(__makeref(value), byte).ToString(_byteFormat, _cultureInfo);
                case TypeCode.Char:
                    return __refvalue(__makeref(value), char).ToString(_cultureInfo);
                case TypeCode.DateTime:
                    return __refvalue(__makeref(value), DateTime).ToString(_dateTimeFormat, _cultureInfo);
                case TypeCode.Decimal:
                    return __refvalue(__makeref(value), decimal).ToString(_decimalFormat, _cultureInfo);
                case TypeCode.Double:
                    return __refvalue(__makeref(value), double).ToString(_doubleFormat, _cultureInfo);
                case TypeCode.Int16:
                    return __refvalue(__makeref(value), short).ToString(_shortFormat, _cultureInfo);
                case TypeCode.Int32:
                    if (type.IsEnum)
                        return value.ToString();
                    else
                        return __refvalue(__makeref(value), int).ToString(_intFormat, _cultureInfo);
                case TypeCode.Int64:
                    return __refvalue(__makeref(value), long).ToString(_longFormat, _cultureInfo);
                case TypeCode.SByte:
                    return __refvalue(__makeref(value), sbyte).ToString(_sbyteFormat, _cultureInfo);
                case TypeCode.Single:
                    return __refvalue(__makeref(value), float).ToString(_floatFormat, _cultureInfo);
                case TypeCode.String:
                    return __refvalue(__makeref(value), string);
                case TypeCode.UInt16:
                    return __refvalue(__makeref(value), ushort).ToString(_ushortFormat, _cultureInfo);
                case TypeCode.UInt32:
                    return __refvalue(__makeref(value), uint).ToString(_uintFormat, _cultureInfo);
                case TypeCode.UInt64:
                    return __refvalue(__makeref(value), ulong).ToString(_ulongFormat, _cultureInfo);
                default:
                    if (value is Date)
                        return __refvalue(__makeref(value), Date).ToString(_dateFormat, _cultureInfo);
                    else if (value is TimeSpan)
                        return __refvalue(__makeref(value), TimeSpan).ToString(_timeSpanFormat, _cultureInfo);
                    else
                        throw new PBException("TextSerializer unknow type to serialize \"{0}\"", type.zGetTypeName());
            }
        }

        public T Deserialize<T>(string text, T defaultValue = default(T), bool tryParse = false)
        {
            Type type = typeof(T);
            if (text == null || text == "")
            {
                if (tryParse)
                    return defaultValue;
                else
                    throw new PBException("can't deserialize null or empty string to {0}", type.zGetTypeName());
            }
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    bool boolValue = bool.Parse(text);
                    return __refvalue(__makeref(boolValue), T);
                case TypeCode.Byte:
                    byte byteValue = byte.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(byteValue), T);
                case TypeCode.Char:
                    char charValue = text[0];
                    return __refvalue(__makeref(charValue), T);
                case TypeCode.DateTime:
                    DateTime dateTimeValue = DateTime.ParseExact(text, _dateTimeFormat, _cultureInfo);
                    return __refvalue(__makeref(dateTimeValue), T);
                case TypeCode.Decimal:
                    decimal decimalValue = decimal.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(decimalValue), T);
                case TypeCode.Double:
                    double doubleValue = double.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(doubleValue), T);
                case TypeCode.Int16:
                    short shortValue = short.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(shortValue), T);
                case TypeCode.Int32:
                    if (type.IsEnum)
                    {
                        return (T)Enum.Parse(type, text);
                        // impossible to convert object to enum type using __makeref() and __refvalue()
                        // Specified cast is not valid. (System.InvalidCastException)
                        // object enumValue = Enum.Parse(type, text);
                        // return __refvalue( __makeref(enumValue),T);
                    }
                    else
                    {
                        int intValue = int.Parse(text, _cultureInfo);
                        return __refvalue(__makeref(intValue), T);
                    }
                case TypeCode.Int64:
                    long longValue = long.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(longValue), T);
                case TypeCode.SByte:
                    sbyte sbyteValue = sbyte.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(sbyteValue), T);
                case TypeCode.Single:
                    float floatValue = float.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(floatValue), T);
                case TypeCode.String:
                    return __refvalue(__makeref(text), T);
                case TypeCode.UInt16:
                    ushort ushortValue = ushort.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(ushortValue), T);
                case TypeCode.UInt32:
                    uint uintValue = uint.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(uintValue), T);
                case TypeCode.UInt64:
                    ulong ulongValue = ulong.Parse(text, _cultureInfo);
                    return __refvalue(__makeref(ulongValue), T);
                default:
                    if (type == typeof(Date))
                    {
                        Date dateValue = Date.ParseExact(text, _dateFormat, _cultureInfo);
                        return __refvalue(__makeref(dateValue), T);
                    }
                    else if (type == typeof(TimeSpan))
                    {
                        TimeSpan timeSpanValue = TimeSpan.ParseExact(text, _timeSpanFormat, _cultureInfo);
                        return __refvalue(__makeref(timeSpanValue), T);
                    }
                    else
                        throw new PBException("TextSerializer unknow type to deserialize \"{0}\"", type.zGetTypeName());
            }
        }
    }

    public static class TextSerializerExtension
    {
        public static string zTextSerialize<T>(this T value)
        {
            return TextSerializer.CurrentTextSerializer.Serialize(value);
        }

        public static T zTextDeserialize<T>(this string text, T defaultValue = default(T), bool tryParse = true)
        {
            return TextSerializer.CurrentTextSerializer.Deserialize(text, defaultValue, tryParse);
        }
    }
}
