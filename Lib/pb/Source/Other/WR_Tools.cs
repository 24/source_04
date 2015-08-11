#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PB_Ado;
#endregion

namespace PB_WRTools
{
    #region class wrt
    public static class wrt
    {
        public static WebRun wr = null;

        #region Test_Regex(string regexString, params string[] text)
        public static void Test_Regex(string regexString, params string[] text)
        {
            Test_Regex(new Regex(regexString), text);
        }
        #endregion

        #region Test_Regex(string regexString, RegexOptions regexOptions, params string[] text)
        public static void Test_Regex(string regexString, RegexOptions regexOptions, params string[] text)
        {
            Test_Regex(new Regex(regexString, regexOptions), text);
        }
        #endregion

        #region Test_Regex(Regex regex, params string[] text)
        public static void Test_Regex(Regex regex, params string[] text)
        {
            var matchs = from t in text select new { text = t, match = regex.Match(t) };
            var r = from match in matchs select new { SourceString = match.text, Success = match.match.Success, CapturedString = match.match.Value, Groups = from g in match.match.Groups.Cast<Group>() select g.Value };
            wr.View(r);
        }
        #endregion


    }
    #endregion
}
