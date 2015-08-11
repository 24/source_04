using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace pb.Text
{
    public static partial class GlobalExtension
    {
        public static string zReplace(this Match match, string input, string replace)
        {
            if (match == null || !match.Success) return input;
            //int i = match.Index + match.Length;
            //return input.Substring(0, match.Index) + replace + input.Substring(i, input.Length - i);
            return input.Substring(0, match.Index) + replace + input.Substring(match.Index + match.Length);
        }
    }
}
