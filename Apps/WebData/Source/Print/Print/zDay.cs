using System.Text.RegularExpressions;
using pb.Text;
using pb.Data;
using pb;

namespace Print
{
    public static class zDay
    {
        public static bool TryGetDay(NamedValues<ZValue> values, out int day)
        {
            day = 0;
            if (values.ContainsKey("day"))
            {
                ZValue v = values["day"];
                if (v is ZString)
                {
                    day = GetDayNumber((string)v);
                    if (day != 0)
                        return true;
                }
                else
                    Trace.WriteLine("error day should be a string : {0}", v);
            }
            return false;
        }

        private static Regex __dayNumberDigitRoman = new Regex("^[012][ivx]+|3i$", RegexOptions.Compiled);
        private static Regex __dayNumberRomanDigit = new Regex("^i[0-9]$", RegexOptions.Compiled);
        private static Regex __dayNumberRoman = new Regex("^[ivx]+$", RegexOptions.Compiled);
        private static Regex __dayNumberDigit = new Regex("^[0-9]+$", RegexOptions.Compiled);
        public static int GetDayNumber(string day)
        {
            // ii = 11, 
            // 0i = 1
            // 1I = 11, 1II = 12, 1III = 13, 1IV = 14, 1V = 15, 1VI = 16, 1VII = 17, 1VIII = 18, 1IX = 19
            // 2I = 21, 2II = 22, 2III = 23, 2IV = 24, 2V = 25, 2VI = 26, 2VII = 27, 2VIII = 28, 2IX = 29
            // 3I = 31
            // i0 = 10, i1 = 11, i2 = 12, i3 = 13, i4 = 14, i5 = 15, i6 = 16, i7 = 17, i8 = 18, i9 = 19
            // v = 5, iv = 4
            day = day.ToLowerInvariant();
            if (day == "ii")
                return 11;
            day = day.Replace('o', '0');
            if (__dayNumberDigitRoman.IsMatch(day))
            {
                int value = zstr.GetRomanNumberValue(day.Substring(1));
                if (value == 0)
                    return 0;
                return ((int)day[0] - 48) * 10 + value;
            }
            if (__dayNumberRomanDigit.IsMatch(day))
                return (int)day[1] - 38;
            if (__dayNumberDigit.IsMatch(day))
                return int.Parse(day);
            if (__dayNumberRoman.IsMatch(day))
                return zstr.GetRomanNumberValue(day);
            return 0;
        }
    }
}
