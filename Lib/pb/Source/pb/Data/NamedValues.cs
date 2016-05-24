using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace pb.Data
{
    // NamedValues1 : 
    //   download\test_f.cs
    //   frboard.cs frboard1.cs
    //   PrintManager.cs PrintManager1.cs Print.cs Print1.cs
    //   LaCroix.cs LeFigaro.cs LeMonde.cs LeMonde0.cs LeMonde1.cs LeParisien.cs LeVifExpress.cs LExpress.cs
    //   Date.cs RegexValues.cs

    //ArrayValueType arrayValueType = ArrayValueType.FirstValue
    public static class NamedValues
    {
        static NamedValues()
        {
            InitParseValues();
        }

        // Groups[1] name, Groups[2] datetime value jj/mm/yyyy hh:mm:ss, Groups[3] datetime value yyyy-mm-jj hh:mm:ss, Groups[4] date value jj/mm/yyyy, Groups[5] date value yyyy-mm-jj,
        // Groups[6] time value d.hh:mm:ss.fffffff, Groups[7] double value, Groups[8] int value, Groups[9] text value, Groups[10] text value
        // datetime jj/mm/yyyy hh:mm:ss(.fff) : Groups[2] day, Groups[3] month, Groups[4] year, Groups[5] hour, Groups[6] minute, Groups[7] second, Groups[8] millisecond
        private static string __parseDatetime1 = @"([0-9]{2})/([0-9]{2})/([0-9]{4})\s+([0-9]{2}):([0-9]{2}):([0-9]{2})(?:\.([0-9]{1,3}))?";
        // datetime yyyy-mm-jj hh:mm:ss(.fff) : Groups[9] year, Groups[10] month, Groups[11] day, Groups[12] hour, Groups[13] minute, Groups[14] second, Groups[15] millisecond
        private static string __parseDatetime2 = @"([0-9]{4})-([0-9]{2})-([0-9]{2})\s+([0-9]{2}):([0-9]{2}):([0-9]{2})(?:\.([0-9]{1,3}))?";
        // date jj/mm/yyyy : Groups[16] day, Groups[17] month, Groups[18] year
        private static string __parseDate1 = "([0-9]{2})/([0-9]{2})/([0-9]{4})";
        // date yyyy-mm-jj : Groups[19] year, Groups[20] month, Groups[21] day
        private static string __parseDate2 = "([0-9]{4})-([0-9]{2})-([0-9]{2})";
        // time ((d.)hh:)mm:ss(.fffffff) : Groups[22] days, Groups[23] hours, Groups[24] minutes, Groups[25] seconds, Groups[26] seconds decimales
        //private static string __parseTimespan = "((?:[0-9]{2}:)?[0-9]{2}:[0-9]{2})";
        //private static string __parseTimespan = @"(?:(?:(?:(?:([0-9]+)\.)?(?:([0-9]{1,2}):))?(?:([0-9]{1,2}):))?([0-9]{1,2})(?:\.([0-9]{1,7})))";
        private static string __parseTimespan = @"(?:(?:(?:([0-9]+)\.)?(?:([0-9]{1,2}):))?([0-9]{1,2}):([0-9]{1,2})(?:\.([0-9]{1,7})))";
        // double 1.(234) : Groups[27] double
        private static string __parseDouble = @"([0-9]+\.[0-9]*)";
        // int 123 : Groups[28] int
        private static string __parseInt = "([0-9]+)";
        // bool true false : Groups[29] bool
        private static string __parseBool = "(true|false)";
        // text 'text' : Groups[30] text
        private static string __parseText1 = "(?:'([^']*)')";
        // text "text" : Groups[31] text
        private static string __parseText2 = "(?:\"([^\"]*)\")";
        //private static Regex __parseValues = new Regex("\\s*([a-z_]+)\\s*=\\s*(?:||||||||)\\s*,?\\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static Regex __parseValues = null;
        public static string __parseValuesFormat = null;
        private static void InitParseValues()
        {
            string parseValues = @"\s*([a-z_][a-z_0-9]+)\s*=\s*";
            parseValues += "(?:" + __parseDatetime1;
            parseValues += "|" + __parseDatetime2;
            parseValues += "|" + __parseDate1;
            parseValues += "|" + __parseDate2;
            parseValues += "|" + __parseTimespan;
            parseValues += "|" + __parseDouble;
            parseValues += "|" + __parseInt;
            parseValues += "|" + __parseBool;
            parseValues += "|" + __parseText1;
            parseValues += "|" + __parseText2 + ")";
            parseValues += @"\s*,?\s*";
            __parseValuesFormat = parseValues;
            __parseValues = new Regex(parseValues, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static NamedValues<ZValue> ParseValues(string values, bool useLowercaseKey = false)
        {
            // datetime yyyy-mm-jj hh:mm:ss(.fff), datetime yyyy-mm-jj hh:mm:ss(.fff), date jj/mm/yyyy, date yyyy-mm-jj, time ((d.)hh:)mm:ss(.fff), double 1.(234), int 123, text 'text', text "text"
            // "datetime1 = 01/01/2015 01:35:52.123, datetime2 = 2015-01-01 01:35:52.123, date1 = 01/01/2015, date2 = 2015-01-01, time = 1.01:35:52.1234567, double = 1.123, int = 3, bool1 = true, bool2 =  false, text1 = 'toto', text2 = \"toto\""
            // "version = 3, date = 01/01/2015, text1 = 'toto', text2 = \"toto\", time = 01:35:52"
            NamedValues<ZValue> namedValues = new NamedValues<ZValue>(useLowercaseKey);
            if (values == null)
                return namedValues;
            Match match = __parseValues.Match(values);
            while (match.Success)
            {
                string name = match.Groups[1].Value;
                ZValue value = null;
                if (match.Groups[2].Value != "")
                {
                    // datetime jj/mm/yyyy hh:mm:ss(.fff) : Groups[2] day, Groups[3] month, Groups[4] year, Groups[5] hour, Groups[6] minute, Groups[7] second, Groups[8] millisecond
                    value = new DateTime(int.Parse(match.Groups[4].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[5].Value),
                        int.Parse(match.Groups[6].Value), int.Parse(match.Groups[7].Value), int.Parse(match.Groups[8].Value.PadRight(3, '0')));
                }
                else if (match.Groups[9].Value != "")
                {
                    // datetime yyyy-mm-jj hh:mm:ss(.fff) : Groups[9] year, Groups[10] month, Groups[11] day, Groups[12] hour, Groups[13] minute, Groups[14] second, Groups[15] millisecond
                    value = new DateTime(int.Parse(match.Groups[9].Value), int.Parse(match.Groups[10].Value), int.Parse(match.Groups[11].Value), int.Parse(match.Groups[12].Value),
                        int.Parse(match.Groups[13].Value), int.Parse(match.Groups[14].Value), int.Parse(match.Groups[15].Value.PadRight(3, '0')));
                }
                else if (match.Groups[16].Value != "")
                {
                    // date jj/mm/yyyy : Groups[16] day, Groups[17] month, Groups[18] year
                    value = new DateTime(int.Parse(match.Groups[18].Value), int.Parse(match.Groups[17].Value), int.Parse(match.Groups[16].Value));
                }
                else if (match.Groups[19].Value != "")
                {
                    // date yyyy-mm-jj : Groups[19] year, Groups[20] month, Groups[21] day
                    value = new DateTime(int.Parse(match.Groups[19].Value), int.Parse(match.Groups[20].Value), int.Parse(match.Groups[21].Value));
                }
                else if (match.Groups[22].Value != "")
                {
                    // time ((d.)hh:)mm:ss(.fffffff) : Groups[22] days, Groups[23] hours, Groups[24] minutes, Groups[25] seconds, Groups[26] seconds decimales
                    int days = 0;
                    string group = match.Groups[22].Value;
                    if (group != "")
                        days = int.Parse(group);

                    int hours = 0;
                    group = match.Groups[23].Value;
                    if (group != "")
                        hours = int.Parse(group);

                    int minutes = 0;
                    group = match.Groups[24].Value;
                    if (group != "")
                        minutes = int.Parse(group);

                    int seconds = 0;
                    group = match.Groups[25].Value;
                    if (group != "")
                        seconds = int.Parse(group);

                    // ticks 1 234 567
                    int ticks = 0;
                    group = match.Groups[26].Value;
                    if (group != "")
                        ticks = int.Parse(group.PadRight(7, '0'));

                    value = zparse.CreateTimeSpan(days, hours, minutes, seconds, ticks);
                }
                else if (match.Groups[27].Value != "")
                {
                    // double 1.(234) : Groups[27] double
                    value = double.Parse(match.Groups[27].Value);
                }
                else if (match.Groups[28].Value != "")
                {
                    // int 123 : Groups[28] int
                    value = int.Parse(match.Groups[28].Value);
                }
                else if (match.Groups[29].Value != "")
                {
                    // bool true false : Groups[29] bool
                    value = bool.Parse(match.Groups[29].Value);
                }
                else if (match.Groups[30].Value != "")
                {
                    // text 'text' : Groups[30] text
                    value = match.Groups[30].Value;
                }
                else if (match.Groups[31].Value != "")
                {
                    // text "text" : Groups[31] text
                    value = match.Groups[31].Value;
                }
                namedValues.Add(name, value);
                match = match.NextMatch();
            }
            return namedValues;
        }
    }

    public class NamedValues<T> : Dictionary<string, T>
    {
        protected string _error = null;
        protected bool _useLowercaseKey = false;

        public NamedValues()
        {
        }

        public NamedValues(bool useLowercaseKey)
        {
            _useLowercaseKey = useLowercaseKey;
        }

        public NamedValues(Dictionary<string, T> dictionary, bool useLowercaseKey = false)
            : base(dictionary)
        {
            _useLowercaseKey = useLowercaseKey;
        }

        public string Error { get { return _error; } }

        public void SetError(string error, params object[] prm)
        {
            if (prm.Length > 0)
                error = string.Format(error, prm);
            _error = error;
        }

        public void SetValues(NamedValues<T> values, params string[] names)
        {
            if (names.Length > 0)
            {
                foreach (string name in names)
                {
                    if (values.ContainsKey(name))
                    {
                        string name2 = name;
                        if (_useLowercaseKey)
                            name2 = name2.ToLowerInvariant();
                        if (this.ContainsKey(name2))
                            this[name2] = values[name];
                        else
                            this.Add(name2, values[name]);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, T> value in values)
                {
                    string key = value.Key;
                    if (_useLowercaseKey)
                        key = key.ToLowerInvariant();
                    if (this.ContainsKey(key))
                        this[key] = value.Value;
                    else
                        this.Add(key, value.Value);
                }
            }
        }

        public void SetValue(string key, T value)
        {
            if (_useLowercaseKey)
                key = key.ToLowerInvariant();
            if (this.ContainsKey(key))
            {
                //Trace.WriteLine("replace \"{0}\" value \"{1}\" by \"{2}\"", key, this[key], value);
                this[key] = value;
            }
            else
                this.Add(key, value);
        }
    }

    //public class NamedValues1 : Dictionary<string, object>
    //{
    //    protected string _error = null;

    //    public NamedValues1()
    //    {
    //    }

    //    public NamedValues1(Dictionary<string, object> dictionary)
    //        : base(dictionary)
    //    {
    //    }

    //    public string Error { get { return _error; } }

    //    public void SetError(string error, params object[] prm)
    //    {
    //        if (prm.Length > 0)
    //            error = string.Format(error, prm);
    //        _error = error;
    //    }

    //    public void SetValues(NamedValues1 values, params string[] names)
    //    {
    //        if (names.Length > 0)
    //        {
    //            foreach (string name in names)
    //            {
    //                if (values.ContainsKey(name))
    //                {
    //                    if (this.ContainsKey(name))
    //                        this[name] = values[name];
    //                    else
    //                        this.Add(name, values[name]);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            foreach (KeyValuePair<string, object> value in values)
    //            {
    //                if (this.ContainsKey(value.Key))
    //                    this[value.Key] = value.Value;
    //                else
    //                    this.Add(value.Key, value.Value);
    //            }
    //        }
    //    }

    //    public void SetValues(string key, object value)
    //    {
    //        if (this.ContainsKey(key))
    //            this[key] = value;
    //        else
    //            this.Add(key, value);
    //    }
    //}
}
