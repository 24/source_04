using System.Globalization;
using System.Threading;

namespace pb
{
    //[Flags]
    //public enum StringFormatOption : uint
    //{
    //    None = 0x00000000,

    //    TrimSpace = 0x00000101,
    //    TrimTab = 0x00000202,
    //    TrimSpaceAndTab = 0x00000303,
    //    TrimNewLine = 0x00000404,
    //    TrimAll = 0x0000FFFF,

    //    TrimStartSpace = 0x00000001,
    //    TrimStartTab = 0x00000002,
    //    TrimStartSpaceAndTab = 0x00000003,
    //    TrimStartNewLine = 0x00000004,
    //    TrimStartAll = 0x000000FF,

    //    TrimEndSpace = 0x00000100,
    //    TrimEndTab = 0x00000200,
    //    TrimEndSpaceAndTab = 0x00000300,
    //    TrimEndNewLine = 0x00000400,
    //    TrimEndAll = 0x0000FF00,

    //    RemoveMultipleSpace = 0x00010000,
    //    RemoveNewLine = 0x00020000,
    //    ReplaceNewLineWithSpace = 0x00040000
    //}

    public class FormatInfo
    {
        #region variable
        public static FormatInfo CurrentFormat = new FormatInfo();

        public string ShortDatePattern = "yyyy-MM-dd";
        public string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public string TimeFormat = "HH:mm:ss";

        private CultureInfo _currentCulture = null;
        #endregion

        public FormatInfo()
        {
            _currentCulture = new CultureInfo(CultureInfo.CurrentCulture.LCID);
        }

        public FormatInfo(int LCID)
        {
            _currentCulture = new CultureInfo(LCID);
        }

        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
            set
            {
                Thread.CurrentThread.CurrentCulture = value;
                _currentCulture = value;
            }
        }

        public static void SetInvariantCulture()
        {
            CurrentFormat = new FormatInfo(CultureInfo.InvariantCulture.LCID);
            CurrentFormat.CurrentCulture.DateTimeFormat.ShortDatePattern = CurrentFormat.ShortDatePattern;
        }
    }

    //public static partial class GlobalExtension
    //{
        //NewLine  \p{Zl} -> Line separator
        //public static Regex _newLine_Regex = new Regex(@"[\r\n]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //public static string zFormat(this string text, StringFormatOption trimOption)
        //{
        //    if (text == null) return null;
        //    if (trimOption == StringFormatOption.None) return text;
        //    List<char> trimChars = new List<char>();
        //    if ((trimOption & StringFormatOption.TrimStartSpace) == StringFormatOption.TrimStartSpace)
        //        trimChars.Add(' ');
        //    if ((trimOption & StringFormatOption.TrimStartTab) == StringFormatOption.TrimStartTab)
        //        trimChars.Add('\t');
        //    if ((trimOption & StringFormatOption.TrimStartNewLine) == StringFormatOption.TrimStartNewLine)
        //    {
        //        trimChars.Add('\r');
        //        trimChars.Add('\n');
        //    }
        //    text = text.TrimStart(trimChars.ToArray());

        //    trimChars.Clear();
        //    if ((trimOption & StringFormatOption.TrimEndSpace) == StringFormatOption.TrimEndSpace)
        //        trimChars.Add(' ');
        //    if ((trimOption & StringFormatOption.TrimEndTab) == StringFormatOption.TrimEndTab)
        //        trimChars.Add('\t');
        //    if ((trimOption & StringFormatOption.TrimEndNewLine) == StringFormatOption.TrimEndNewLine)
        //    {
        //        trimChars.Add('\r');
        //        trimChars.Add('\n');
        //    }
        //    text = text.TrimEnd(trimChars.ToArray());

        //    if ((trimOption & StringFormatOption.RemoveMultipleSpace) == StringFormatOption.RemoveMultipleSpace)
        //        text = zstr.MultipleSpace_Regex.Replace(text, " ");
        //    if ((trimOption & StringFormatOption.ReplaceNewLineWithSpace) == StringFormatOption.ReplaceNewLineWithSpace)
        //        text = _newLine_Regex.Replace(text, " ");
        //    else if ((trimOption & StringFormatOption.RemoveNewLine) == StringFormatOption.RemoveNewLine)
        //        text = _newLine_Regex.Replace(text, "");

        //    return text;
        //}

        //public static bool zStringTo<T>(this string stringValue, out T value)
        //{
        //    return stringValue.zStringTo<T>(out value, null);
        //}

        //public static bool zStringTo<T>(this string stringValue, out T value, string format)
        //{
        //    //http://stackoverflow.com/questions/393731/generic-conversion-function-doesnt-seem-to-work-with-guids
        //    //T t = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(text);
        //    //value = (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(s);
        //    //Converter<string, int> f;

        //    value = default(T);
        //    bool ret = false;

        //    Type type = typeof(T);
        //    switch (Type.GetTypeCode(type))
        //    {
        //        case TypeCode.Boolean:
        //            bool b;
        //            ret = zconvert.TryParse(stringValue, out b, format);
        //            value = (T)(object)b;
        //            break;

        //        case TypeCode.Int32:
        //            int i;
        //            ret = int.TryParse(stringValue, out i);
        //            value = (T)(object)i;
        //            break;

        //        default:
        //            if (type == typeof(TimeSpan))
        //            {
        //                TimeSpan timeSpanValue;
        //                ret = zconvert.TryParse(stringValue, out timeSpanValue, format);
        //                value = (T)(object)timeSpanValue;
        //            }
        //            else
        //                throw new PBException("error zStringTo<T> does'nt manage type {0}", typeof(T).Name);
        //            break;
        //    }
        //    return ret;
        //}
    //}
}
