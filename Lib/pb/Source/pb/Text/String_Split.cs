using System;

namespace pb.Text
{
    public static partial class zstr
    {
        public static string StringGetFirstLines(string s, int iNbLine)
        {
            return StringGetFirstLines(s, iNbLine, false);
        }

        public static string StringGetFirstLines(string s, int iNbLine, bool bWithoutEmptyLine)
        {
            bool bSupDebFin, bSupElementVide;
            int nb;
            char[] cEndOfLine = { '\r', '\n' };
            string[] sLines;

            bSupDebFin = false;
            bSupElementVide = false;
            if (bWithoutEmptyLine)
            {
                bSupDebFin = true;
                bSupElementVide = true;
            }
            sLines = zsplit.Split(s, cEndOfLine, bSupDebFin, false, bSupElementVide);
            nb = sLines.Length;
            if (nb > iNbLine) nb = iNbLine;
            return string.Join("\r\n", sLines, 0, nb);
        }

        public static string StringGetLastLines(string s, int iNbLine)
        {
            return StringGetLastLines(s, iNbLine, false);
        }

        public static string StringGetLastLines(string s, int iNbLine, bool bWithoutEmptyLine)
        {
            bool bSupDebFin, bSupElementVide;
            int i, nb;
            char[] cEndOfLine = { '\r', '\n' };
            string[] sLines;

            bSupDebFin = false;
            bSupElementVide = false;
            if (bWithoutEmptyLine)
            {
                bSupDebFin = true;
                bSupElementVide = true;
            }
            sLines = zsplit.Split(s, cEndOfLine, bSupDebFin, false, bSupElementVide);
            i = 0; nb = sLines.Length;
            if (nb > iNbLine)
            {
                i = nb - iNbLine;
                nb = iNbLine;
            }
            return string.Join("\r\n", sLines, i, nb);
        }

        public static bool StringCompare(string sString1, string sString2, char[] cSeparators, bool bIgnoreCase, bool bIgnoreAccent, bool bSupDebFin,
            bool bSupDoubleQuot, bool bSupElementVide)
        {
            int i;

            if (bIgnoreAccent)
            {
                //sString1 = StringReplaceAccent(sString1);
                //sString2 = StringReplaceAccent(sString2);
                sString1 = GetStringWithoutAccent(sString1);
                sString2 = GetStringWithoutAccent(sString2);
            }
            string[] s1 = zsplit.Split(sString1, cSeparators, bSupDebFin, bSupDoubleQuot, bSupElementVide);
            string[] s2 = zsplit.Split(sString2, cSeparators, bSupDebFin, bSupDoubleQuot, bSupElementVide);
            for (i = 0; i < s1.Length && i < s2.Length; i++)
            {
                if (string.Compare(s1[i], s2[i], bIgnoreCase) != 0) break;
            }
            int n = (Math.Max(s1.Length, s2.Length) + 1) / 2;
            if (i >= n) return true;
            return false;
        }
    }
}
