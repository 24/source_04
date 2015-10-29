﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace pb.Text
{
    /// <summary>
    /// Permet la lecture caractère par caractère en sautant les caractères appartenant à une zone
    /// </summary>
    public class StringZone00
    {
        #region variable
        private string sText;      // string à parcourir
        private char[,] cCharZone; // tableau des caractères de début (cCharZone[i][0]) et de fin (cCharZone[i][1]) de zone
        private int iNbCharZone;   // nombre de zones dans cCharZone
        private ArrayList alZone;  // liste des caractères de fin de toutes les zones en cours
        private int iZone;         // indice de la zone en cours dans alZone
        private char cZoneFin;     // caractère de fin de zone en cours
        private int iText;         // indice du prochain caractère dans sText
        #endregion

        public StringZone00(string sText, char[,] cCharZone)
        {
            this.sText = sText;
            this.cCharZone = cCharZone;
            iNbCharZone = cCharZone.GetUpperBound(0) + 1;
            alZone = new ArrayList();
            iZone = -1;
            iText = 0;
        }

        public int IndiceNextChar
        {
            get { return iText; }
        }

        public int NextChar()
        {
            int ic;

            while ((ic = NextCharZone()) != -1)
            {
                if (iZone == -1) break;
            }
            return ic;
        }

        public int NextCharZone()
        {
            if (iText >= sText.Length) return -1;
            char c = sText[iText++];
            if (iZone != -1 && c == cZoneFin)
            {
                alZone.RemoveAt(iZone--);
                if (iZone != -1) cZoneFin = (char)alZone[iZone];
            }
            else
            {
                for (int i = 0; i < iNbCharZone; i++)
                {
                    if (c == cCharZone[i, 0])
                    {
                        cZoneFin = cCharZone[i, 1];
                        alZone.Add(cZoneFin);
                        iZone++;
                        break;
                    }
                }
            }
            return c;
        }
    }

    public class NumbersInString
    {
        public string String;
        public string CorrectedString;
        public string[] StringNumbers;
        public int[] IndexNumbers;
    }

    public static class zstr
    {
        private static SortedList<char, char> _accent = null;

        static zstr()
        {
            InitAccents();
        }

        public static string DoubleCarQuote(string s)
        {
            return s.Replace("'", "''");
        }

        public static string left(string s, int l)
        {
            if (l > s.Length) l = s.Length;
            return s.Substring(0, l);
        }

        public static string mid(string s, int pos)
        {
            if (pos < s.Length)
                return s.Substring(pos, s.Length - pos);
            return "";
        }

        public static string mid(string s, int pos, int len)
        {
            if (pos < s.Length)
            {
                if (pos + len > s.Length) len = s.Length - pos;
                return s.Substring(pos, len);
            }
            return "";
        }

        public static string right(string s, int l)
        {
            if (l > s.Length) l = s.Length;
            return s.Substring(s.Length - l, l);
        }

        public static int StringGetNbLines(string s)
        {
            char[] cEndOfLine = { '\r', '\n' };
            int i, iNbLine;

            i = 0;
            iNbLine = 0;
            while (i < s.Length)
            {
                i = s.IndexOfAny(cEndOfLine, i);
                if (i == -1) break;
                iNbLine++;
                if (++i < s.Length && (s[i] == '\r' || s[i] == '\n')) i++;
            }
            return iNbLine;
        }

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

        /// <summary>
        /// Compute Levenshtein distance
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance between the two strings.
        /// The larger the number, the bigger the difference.
        /// </returns>
        public static int StringDistance(string s, string t)
        {
            // source from http://www.merriampark.com/ldcsharp.htm
            // doc http://www.merriampark.com/ld.htm#FLAVORS

            int n = s.Length; //length of s
            int m = t.Length; //length of t
            int[,] d = new int[n + 1, m + 1]; // matrix
            int cost; // cost

            // Step 1
            if (n == 0) return m;
            if (m == 0) return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {

                    // Step 5
                    cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1);

                    // Step 6
                    d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }

        public static char GetCharWithoutAccent0(char c)
        {
            switch (c)
            {
                case 'à':
                case 'á':
                case 'â':
                case 'ä':
                case 'ã':
                case 'å':
                case 'æ':
                    return 'a';
                case 'é':
                case 'è':
                case 'ê':
                case 'ë':
                    return 'e';
                case 'ì':
                case 'í':
                case 'î':
                case 'ï':
                    return 'i';
                case 'ò':
                case 'ó':
                case 'ô':
                case 'ö':
                case 'õ':
                case 'ø':
                    return 'o';
                case 'ù':
                case 'ú':
                case 'û':
                case 'ü':
                    return 'u';
                case 'ý':
                case 'ÿ':
                    return 'y';
                case 'ç':
                    return 'c';
                case 'ð':
                    return 'd';
                case 'ñ':
                    return 'n';
                case 'À':
                case 'Á':
                case 'Â':
                case 'Ä':
                case 'Ã':
                case 'Å':
                case 'Æ':
                    return 'A';
                case 'É':
                case 'È':
                case 'Ê':
                case 'Ë':
                    return 'E';
                case 'Ì':
                case 'Í':
                case 'Î':
                case 'Ï':
                    return 'I';
                case 'Ò':
                case 'Ó':
                case 'Ô':
                case 'Ö':
                case 'Õ':
                case 'Ø':
                    return 'O';
                case 'Ù':
                case 'Ú':
                case 'Û':
                case 'Ü':
                    return 'U';
                case 'Ý':
                case 'Ÿ':
                    return 'Y';
                case 'ß':
                    return 'B';
                case 'Ç':
                    return 'c';
                case 'Ð':
                    return 'D';
                case 'Ñ':
                    return 'N';
            }
            return c;
        }

        private static Regex grxNumber = new Regex(@"[0-9\.,]+", RegexOptions.Compiled);
        private static Regex grxRomanNumber1 = new Regex("(?<n>[IVXLCDM]{2,})(?<c>[\\s'\"\\-,;\\.:\\(\\)\\[\\]\\{\\}])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex grxRomanNumber2 = new Regex("(?<c>[\\s'\"\\-,;\\.:\\(\\)\\[\\]\\{\\}])(?<n>[IVXLCDM]{2,})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex grxRomanNumber3 = new Regex("^(?<n>[IVXLCDM]{2,})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static Regex grxTrimNumber1 = new Regex(@"[\.,]0+$", RegexOptions.Compiled);
        private static Regex grxTrimNumber2 = new Regex(@"([\.,][0-9]*?)0+$", RegexOptions.Compiled);
        private static Regex grxTrimNumber3 = new Regex(@"^0+", RegexOptions.Compiled);
        public static NumbersInString GetNumbersInString(string s)
        {
            MatchCollection matchs = grxNumber.Matches(s);
            Match match = null;
            int i = 0;
            if (i < matchs.Count) match = matchs[i++];

            MatchCollection romanMatchs1 = grxRomanNumber1.Matches(s);
            MatchCollection romanMatchs2 = grxRomanNumber2.Matches(s);
            MatchCollection romanMatchs3 = grxRomanNumber3.Matches(s);
            Match[] romanMatchs = new Match[romanMatchs1.Count + romanMatchs2.Count + romanMatchs3.Count];
            romanMatchs1.CopyTo(romanMatchs, 0);
            romanMatchs2.CopyTo(romanMatchs, romanMatchs1.Count);
            romanMatchs3.CopyTo(romanMatchs, romanMatchs1.Count + romanMatchs2.Count);

            Match romanMatch = null;
            int i2 = 0;
            if (i2 < romanMatchs.Length) romanMatch = romanMatchs[i2++];

            List<string> numbers = new List<string>();
            while (match != null || romanMatch != null)
            {
                string sNumber = null;
                if (match != null && (romanMatch == null || match.Index < romanMatch.Index))
                {
                    sNumber = match.Value;
                    sNumber = grxTrimNumber1.Replace(sNumber, "");
                    sNumber = grxTrimNumber2.Replace(sNumber, "$1");
                    sNumber = grxTrimNumber3.Replace(sNumber, "");
                    if (i < matchs.Count) match = matchs[i++]; else match = null;
                }
                else
                {
                    sNumber = romanMatch.Groups["n"].Value;
                    sNumber = GetRomanNumberValue(sNumber).ToString();
                    if (i2 < romanMatchs.Length) romanMatch = romanMatchs[i2++]; else romanMatch = null;
                }
                numbers.Add(sNumber);
            }

            string sCorrectedString = s;
            sCorrectedString = grxNumber.Replace(sCorrectedString, "");
            sCorrectedString = grxRomanNumber1.Replace(sCorrectedString, "${c}");
            sCorrectedString = grxRomanNumber2.Replace(sCorrectedString, "");
            sCorrectedString = grxRomanNumber3.Replace(sCorrectedString, "");

            string[] StringNumbers = numbers.ToArray();
            int[] IndexNumbers = new int[StringNumbers.Length]; for (int j = 0; j < StringNumbers.Length; j++) IndexNumbers[j] = j;
            Array.Sort<string, int>(StringNumbers, IndexNumbers);

            return new NumbersInString() { String = s, CorrectedString = sCorrectedString, StringNumbers = StringNumbers, IndexNumbers = IndexNumbers };
        }

        public static string GetWordCode(string s)
        {
            if (s.Length <= 3)
            {
                switch (s)
                {
                    case "and":
                    case "et":
                    case "&":
                        return "#and";
                }
            }
            return null;
        }

        public static string ReplaceControl(string text)
        {
            if (text == null)
                return null;
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\r':
                        sb.Append(@"\r");
                        break;
                    case '\n':
                        sb.Append(@"\n");
                        break;
                    case '\t':
                        sb.Append(@"\t");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        //public static string ReplaceControl(string text)
        //{
        //    text = text.Replace("\r", @"\r");
        //    text = text.Replace("\n", @"\n");
        //    text = text.Replace("\t", @"\t");
        //    return text;
        //}

        public static int GetRomanNumberValue(string sRomanNumber)
        {
            int iNumber = 0;
            int iLastDigit = 0;
            for (int i = sRomanNumber.Length - 1; i >= 0; i--)
            {
                int iDigit = GetRomanDigitValue(sRomanNumber[i]);
                if (iLastDigit != 0 && iDigit < iLastDigit)
                    iNumber -= iDigit;
                else
                    iNumber += iDigit;
                iLastDigit = iDigit;
            }
            return iNumber;
        }

        public static int GetRomanDigitValue(char cRomanDigit)
        {
            switch (char.ToLower(cRomanDigit))
            {
                case 'i':
                    return 1;
                case 'v':
                    return 5;
                case 'x':
                    return 10;
                case 'l':
                    return 50;
                case 'c':
                    return 100;
                case 'd':
                    return 500;
                case 'm':
                    return 1000;
            }
            throw new PBException("'{0}' is not a roman digit", cRomanDigit);
        }

        public static void InitAccents()
        {
            char[,] cAccents = {
                { 'á', 'a' }, { 'à', 'a' }, { 'â', 'a' }, { 'ä', 'a' }, { 'ã', 'a' }, { 'å', 'a' }, { 'æ', 'a' },
                { 'é', 'e' }, { 'è', 'e' }, { 'ê', 'e' }, { 'ë', 'e' },
                { 'í', 'i' }, { 'ì', 'i' }, { 'î', 'i' }, { 'ï', 'i' },
                { 'ó', 'o' }, { 'ò', 'o' }, { 'ô', 'o' }, { 'ö', 'o' }, { 'õ', 'o' }, { 'ø', 'o' },
                { 'ú', 'u' }, { 'ù', 'u' }, { 'û', 'u' }, { 'ü', 'u' },
                { 'ý', 'y' }, { 'ÿ', 'y' },
                { 'ç', 'c' },
                { 'ð', 'd' },
                { 'ñ', 'n' },

                //{ 'À', 'A' }, { 'Â', 'A' }, { 'Ä', 'A' },
                //{ 'É', 'E' }, { 'È', 'E' }, { 'Ê', 'E' }, { 'Ë', 'E' },
                //{ 'Î', 'I' }, { 'Ï', 'I' },
                //{ 'Ô', 'O' }, { 'Ö', 'O' },
                //{ 'Ù', 'U' }, { 'Û', 'U' }, { 'Ü', 'U' } };

                { 'Á', 'A' }, { 'À', 'A' }, { 'Â', 'A' }, { 'Ä', 'A' }, { 'Ã', 'A' }, { 'Å', 'A' }, { 'Æ', 'A' },
                { 'É', 'E' }, { 'È', 'E' }, { 'Ê', 'E' }, { 'Ë', 'E' },
                { 'Í', 'I' }, { 'Ì', 'I' }, { 'Î', 'I' }, { 'Ï', 'I' },
                { 'Ó', 'O' }, { 'Ò', 'O' }, { 'Ô', 'O' }, { 'Ö', 'O' }, { 'Õ', 'O' }, { 'Ø', 'O' },
                { 'Ú', 'U' }, { 'Ù', 'U' }, { 'Û', 'U' }, { 'Ü', 'U' },
                { 'Ý', 'Y' }, { 'Ÿ', 'Y' },
                { 'ß', 'B' },
                { 'Ç', 'C' },
                { 'Ð', 'D' },
                { 'Ñ', 'N' }

                };

            _accent = new SortedList<char, char>();
            for (int i = 0; i < cAccents.GetLength(0); i++) _accent.Add(cAccents[i, 0], cAccents[i, 1]);
        }

        public static char GetCharWithoutAccent(char c)
        {
            if (_accent.ContainsKey(c))
                return _accent[c];
            else
                return c;
        }

        public static string GetStringWithoutAccent(string text)
        {
            char[] chars = text.ToCharArray();
            for (int i = 0; i < chars.Length; i++) chars[i] = GetCharWithoutAccent(chars[i]);
            return new string(chars);
        }

        // suppression caractères spéciaux : ' " - , ; . : ! ? # | @ $ µ % £ ( ) / \ [ ] { } = < > + « »
        private static Regex grxNormalize_RemoveSpecialChars = new Regex("\\s*[\\s'\"\\-,;\\.\\:!?#\\|@$µ%£\\(\\)/\\\\\\[\\]\\{\\}=<>+«»]\\s*", RegexOptions.Compiled);
        // suppression blanc début
        private static Regex grxNormalize_RemoveBeginingSpace = new Regex(@"^\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // suppression blanc fin
        private static Regex grxNormalize_RemoveEndingSpace = new Regex(@"\s+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // Remplacement des répétitions de blancs par un blanc
        public static Regex MultipleSpace_Regex = new Regex(@"\s{2,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // suppression lettre seule sauf a i v l c d m, au milieu (a c d i l m v)
        private static Regex grxNormalize_SingleLetterMiddle = new Regex(@"\s+[be-hj-kn-uw-z]\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // suppression lettre seule sauf a i v l c d m, au début
        private static Regex grxNormalize_SingleLetterBegin = new Regex(@"^[be-hj-kn-uw-z]\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        // suppression lettre seule sauf a i v l c d m, à la fin
        private static Regex grxNormalize_SingleLetterEnd = new Regex(@"\s+[be-hj-kn-uw-z]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static string NormalizeString(string s)
        {
            // suppression des accents
            s = GetStringWithoutAccent(s);
            // suppression caractères spéciaux : ' " - , ; . : ! ? & # | @ $ µ % £ ( ) / \ [ ] { } = < > +
            s = grxNormalize_RemoveSpecialChars.Replace(s, " ");
            // suppression blanc début
            s = grxNormalize_RemoveBeginingSpace.Replace(s, "");
            // suppression blanc fin
            s = grxNormalize_RemoveEndingSpace.Replace(s, "");
            // Remplacement des répétitions de blancs par un blanc
            s = MultipleSpace_Regex.Replace(s, " ");
            // suppression lettre seule sauf a, au milieu
            s = grxNormalize_SingleLetterMiddle.Replace(s, " ");
            // suppression lettre seule sauf a, au début
            s = grxNormalize_SingleLetterBegin.Replace(s, "");
            // suppression lettre seule sauf a, à la fin
            s = grxNormalize_SingleLetterEnd.Replace(s, "");
            return s;
        }

        public static string RemoveMultipleSpace(string s)
        {
            // suppression blanc début
            s = grxNormalize_RemoveBeginingSpace.Replace(s, "");
            // suppression blanc fin
            s = grxNormalize_RemoveEndingSpace.Replace(s, "");
            // Remplacement des répétitions de blancs par un blanc
            s = MultipleSpace_Regex.Replace(s, " ");
            return s;
        }

        private static Regex _utf8Regex = new Regex("_([0-9a-f]{2})_([0-9a-f]{2})", RegexOptions.IgnoreCase);
        public static string ReplaceUTF8Code(string s)
        {
            Match match = _utf8Regex.Match(s);
            while (match.Success)
            {
                byte[] bytes = new byte[2];
                bytes[0] = byte.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                bytes[1] = byte.Parse(match.Groups[2].Value, System.Globalization.NumberStyles.AllowHexSpecifier);
                s = match.zReplace(s, Encoding.UTF8.GetString(bytes));
                match = _utf8Regex.Match(s);
            }
            return s;
        }

        public static string ToFirstCharUpper(string text)
        {
            //CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            //TextInfo textInfo = cultureInfo.TextInfo;
            //textInfo.ToTitleCase();
            if (string.IsNullOrEmpty(text))
                return text;
            else
                return char.ToUpperInvariant(text[0]).ToString() + text.Substring(1).ToLowerInvariant();
        }

        public static string ConcatStrings(IEnumerable<string> texts, string separator = null)
        {
            if (texts == null)
                return null;
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string text in texts)
            {
                if (!first && separator != null)
                    sb.Append(separator);
                first = false;
                sb.Append(text);
            }
            if (!first)
                return sb.ToString();
            else
                return null;
        }
    }

    public static partial class GlobalExtension
    {
        //public static string zAddValue<T>(this string values, T value)
        //{
        //    return values.zAddValue(value, ", ");
        //}

        public static string zAddValue<T>(this string values, T value, string separator = ", ")
        {
            if (value == null)
                return values;
            string s = value.ToString();
            if (s == null)
                return values;
            if (values == null)
                values = s;
            else
            {
                if (values != "")
                    values += separator;
                values += s;
            }
            return values;
        }

        //public static void zAddValue<T>(this StringBuilder values, T value)
        //{
        //    values.zAddValue(value, ", ");
        //}

        public static void zAddValue<T>(this StringBuilder values, T value, string separator = ", ")
        {
            if (values == null || value == null)
                return;
            string s = value.ToString();
            if (s == null)
                return;
            if (values.Length != 0 && separator != null)
                values.Append(separator);
            values.Append(s);
        }

        public static string zNullIfEmpty(this string text)
        {
            if (text == "")
                return null;
            else
                return text;
        }

        public static string zToFirstCharUpper(this string text)
        {
            return zstr.ToFirstCharUpper(text);
        }

        public static string zToStringWithoutAccent(this string text)
        {
            return zstr.GetStringWithoutAccent(text);
        }

        public static string zReplaceControl(this string text)
        {
            if (text == null)
                return null;
            return zstr.ReplaceControl(text);
        }

        public static string zConcatStrings(this IEnumerable<string> texts, string separator = null)
        {
            return zstr.ConcatStrings(texts, separator);
        }
    }
}
