using System;
using System.Collections.Generic;
using System.Text;

namespace pb.Text
{
    [Flags]
    public enum SplitOption
    {
        None = 0x0000,
        SplitLine = 0x0001,
        SplitSpace = 0x0002,
        SplitTab = 0x0004,
        SplitAll = 0x00FF,
        TrimStart = 0x0100,
        TrimEnd = 0x0200,
        Trim = 0x0300,
        RemoveDoubleQuot = 0x0400,
        RemoveEmptyString = 0x0800,
        All = 0xFFFF
    }

    public static class zsplit
    {
        public static string[] Split(string s, SplitOption splitOption)
        {
            List<char> cSeparator = new List<char>();
            if ((splitOption & SplitOption.SplitLine) == SplitOption.SplitLine)
            {
                cSeparator.Add('\r');
                cSeparator.Add('\n');
            }
            if ((splitOption & SplitOption.SplitSpace) == SplitOption.SplitSpace)
                cSeparator.Add(' ');
            if ((splitOption & SplitOption.SplitTab) == SplitOption.SplitTab)
                cSeparator.Add('\t');

            return _Split(s, cSeparator.ToArray(), null, splitOption);
        }

        public static string[] Split(string s, string sSeparator, bool bTrim, bool bRemoveEmpty)
        {
            return Split(s, new string[] { sSeparator }, bTrim, bRemoveEmpty);
        }

        public static string[] Split(string s, string[] sSeparators, bool bTrim, bool bRemoveEmpty)
        {
            StringSplitOptions opt = StringSplitOptions.None;
            if (bRemoveEmpty) opt = StringSplitOptions.RemoveEmptyEntries;
            string[] sSplit = s.Split(sSeparators, opt);
            if (bTrim)
            {
                for (int i = 0; i < sSplit.Length; i++) sSplit[i] = sSplit[i].Trim();
            }
            if (bRemoveEmpty)
            {
                List<string> alSplit = new List<string>();
                for (int i = 0; i < sSplit.Length; i++) if (sSplit[i] != "") alSplit.Add(sSplit[i]);
                sSplit = new string[alSplit.Count];
                alSplit.CopyTo(sSplit);
            }
            return sSplit;
        }

        public static string[] Split(string s, char cSeparator)
        {
            return _Split(s, new char[] { cSeparator }, null, SplitOption.None);
        }

        public static string[] Split(string s, char cSeparator, bool bSupDebFin)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            return _Split(s, new char[] { cSeparator }, null, splitOption);
        }

        public static string[] Split(string s, char cSeparator, bool bSupDebFin, bool bSupDoubleQuot)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            return _Split(s, new char[] { cSeparator }, null, splitOption);
        }

        public static string[] Split(string s, char cSeparator, bool bSupDebFin, bool bSupDoubleQuot, bool bSupElementVide)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            if (bSupElementVide) splitOption |= SplitOption.RemoveEmptyString;
            return _Split(s, new char[] { cSeparator }, null, splitOption);
        }

        public static string[] Split(string s, char[] cSeparator)
        {
            return _Split(s, cSeparator, null, SplitOption.None);
        }

        public static string[] Split(string s, char[] cSeparator, bool bSupDebFin)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            return _Split(s, cSeparator, null, splitOption);
        }

        public static string[] Split(string s, char[] cSeparator, bool bSupDebFin, bool bSupDoubleQuot)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            return _Split(s, cSeparator, null, splitOption);
        }

        public static string[] Split(string s, char[] cSeparator, bool bSupDebFin, bool bSupDoubleQuot, bool bSupElementVide)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            if (bSupElementVide) splitOption |= SplitOption.RemoveEmptyString;
            return _Split(s, cSeparator, null, splitOption);
        }

        public static string[] Split(string s, char cSeparator, char[,] cCharZone)
        {
            return _Split(s, new char[] { cSeparator }, cCharZone, SplitOption.None);
        }

        public static string[] Split(string s, char cSeparator, char[,] cCharZone, bool bSupDebFin)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            return _Split(s, new char[] { cSeparator }, cCharZone, splitOption);
        }

        public static string[] Split(string s, char cSeparator, char[,] cCharZone, bool bSupDebFin, bool bSupDoubleQuot)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            return _Split(s, new char[] { cSeparator }, cCharZone, splitOption);
        }

        public static string[] Split(string s, char cSeparator, char[,] cCharZone, bool bSupDebFin, bool bSupDoubleQuot, bool bSupElementVide)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            if (bSupElementVide) splitOption |= SplitOption.RemoveEmptyString;
            return _Split(s, new char[] { cSeparator }, cCharZone, splitOption);
        }

        public static string[] Split(string s, char[] cSeparator, char[,] cCharZone)
        {
            return _Split(s, cSeparator, cCharZone, SplitOption.None);
        }

        public static string[] Split(string s, char[] cSeparator, char[,] cCharZone, bool bSupDebFin)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            return _Split(s, cSeparator, cCharZone, splitOption);
        }

        public static string[] Split(string s, char[] cSeparator, char[,] cCharZone, bool bSupDebFin, bool bSupDoubleQuot)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            return _Split(s, cSeparator, cCharZone, splitOption);
        }

        public static string[] Split(string s, char[] cSeparator, char[,] cCharZone, bool bSupDebFin, bool bSupDoubleQuot, bool bSupElementVide)
        {
            SplitOption splitOption = SplitOption.None;
            if (bSupDebFin) splitOption = SplitOption.Trim;
            if (bSupDoubleQuot) splitOption |= SplitOption.RemoveDoubleQuot;
            if (bSupElementVide) splitOption |= SplitOption.RemoveEmptyString;
            return _Split(s, cSeparator, cCharZone, splitOption);
        }

        private static string[] _Split(string s, char[] cSeparator, char[,] cCharZone, SplitOption splitOption)
        {
            char c;
            int i, ic, iDebutElement;
            string[] r;
            List<string> alSplit;
            StringZones sz;

            if (s == null) return new string[0];

            if (cCharZone == null)
                r = s.Split(cSeparator);
            else
            {
                // cCharZone tableau des caractères de délimitation des zones par exemple ( et )
                // le tableau doit avoir comme dimension [n,2]
                // split en tenant compte des zones de texte délimitées par cCharZone
                alSplit = new List<string>();
                sz = new StringZones(s, cCharZone);
                iDebutElement = 0;
                while ((ic = sz.ReadChar()) != -1)
                {
                    c = (char)ic;
                    if (Array.IndexOf(cSeparator, c) != -1)
                    {
                        alSplit.Add(s.Substring(iDebutElement, sz.IndexNextChar - 1 - iDebutElement));
                        iDebutElement = sz.IndexNextChar;
                    }
                }
                //if (sz.IndiceNextChar - 1 - iDebutElement > 0)
                if (sz.IndexNextChar - iDebutElement > 0)
                    alSplit.Add(s.Substring(iDebutElement, sz.IndexNextChar - iDebutElement));
                else
                    alSplit.Add("");
                r = new string[alSplit.Count];
                alSplit.CopyTo(r);
            }
            //if (bTrim)
            //    for(i = 0; i < r.Length; i++)  r[i] = r[i].Trim();
            //if (bRemoveDoubleQuot)
            //    for(i = 0; i < r.Length; i++)
            //    {
            //        s = r[i];
            //        if (s.StartsWith("\"") && s.EndsWith("\""))
            //            r[i] = s.Substring(1, s.Length - 2);
            //    }
            if ((splitOption & SplitOption.RemoveDoubleQuot) == SplitOption.RemoveDoubleQuot ||
                (splitOption & SplitOption.TrimStart) == SplitOption.TrimStart ||
                (splitOption & SplitOption.TrimEnd) == SplitOption.TrimEnd)
            {
                for (i = 0; i < r.Length; i++)
                {
                    s = r[i];
                    bool bNew = false;
                    if ((splitOption & SplitOption.RemoveDoubleQuot) == SplitOption.RemoveDoubleQuot)
                    {
                        if (s.StartsWith("\"") && s.EndsWith("\""))
                        {
                            s = s.Substring(1, s.Length - 2);
                            bNew = true;
                        }
                    }
                    if ((splitOption & SplitOption.Trim) == SplitOption.Trim)
                    {
                        s = s.Trim();
                        bNew = true;
                    }
                    else if ((splitOption & SplitOption.TrimStart) == SplitOption.TrimStart)
                    {
                        s = s.TrimStart();
                        bNew = true;
                    }
                    else if ((splitOption & SplitOption.TrimEnd) == SplitOption.TrimEnd)
                    {
                        s = s.TrimEnd();
                        bNew = true;
                    }
                    if (bNew) r[i] = s;
                }
            }
            //if (bRemoveEmpty)
            if ((splitOption & SplitOption.RemoveEmptyString) == SplitOption.RemoveEmptyString)
            {
                alSplit = new List<string>();
                for (i = 0; i < r.Length; i++) if (r[i] != "") alSplit.Add(r[i]);
                r = new string[alSplit.Count];
                alSplit.CopyTo(r);
            }
            return r;
        }
    }
}
