using System;
using System.Collections.Generic;
using System.Text;

namespace pb.Text
{
    [Serializable]
    public class StringZone
    {
        public string ContentString = null;
        public string String = null;
        public bool IsZone = false;
        public char BeginZoneChar = (char)0;
        public char EndZoneChar = (char)0;

        public override string ToString()
        {
            //string s = "";
            //if (BeginZoneChar != (char)0) s = BeginZoneChar.ToString();
            //s += String;
            //if (EndZoneChar != (char)0) s += EndZoneChar.ToString();
            return String;
        }
    }

    [Serializable]
    public class StringZones
    {
        #region variable
        private string gsString = null;                     // string à parcourir
        private char[,] gcCharZone = null;                  // tableau des caractères de début (cCharZone[i][0]) et de fin (cCharZone[i][1]) de zone
        private SortedList<char, char> gslCharZone = null;  // liste triée des caractères de début (key) et de fin (value) de zone
        private int giNbCharZone = 0;                       // nombre de zones dans cCharZone
        private List<char> gEndZonesChar = null;            // liste des caractères de fin de toutes les zones en cours
        private int giIndexZone = -1;                       // indice de la zone en cours dans gEndZoneChar
        private char gcCurrentBeginZoneChar = (char)0;      // caractère de début de zone en cours
        private char gcCurrentEndZoneChar = (char)0;        // caractère de fin de zone en cours
        private int giIndexNextChar = 0;                     // indice du prochain caractère dans sText
        private List<StringZone> gStringZones = null;       // 
        #endregion

        public StringZones(string sString, char[,] cCharZone)
        {
            gsString = sString;
            gcCharZone = cCharZone;
            giNbCharZone = cCharZone.GetUpperBound(0) + 1;
            gslCharZone = new SortedList<char, char>();
            for (int i = 0; i < giNbCharZone; i++)
                gslCharZone.Add(cCharZone[i, 0], cCharZone[i, 1]);
            gEndZonesChar = new List<char>();
            giIndexZone = -1;
            giIndexNextChar = 0;
            gStringZones = new List<StringZone>();
        }

        #region property ...
        public string String
        {
            get { return gsString; }
        }

        public char[,] CharZone
        {
            get { return gcCharZone; }
        }

        public int NbCharZone
        {
            get { return giNbCharZone; }
        }

        public List<char> EndZonesChar
        {
            get { return gEndZonesChar; }
        }

        public int IndexZone
        {
            get { return giIndexZone; }
        }

        public int CurrentBeginZoneChar
        {
            get { return gcCurrentBeginZoneChar; }
        }

        public int CurrentEndZoneChar
        {
            get { return gcCurrentEndZoneChar; }
        }

        public int IndexNextChar
        {
            get { return giIndexNextChar; }
        }

        public StringZone[] StringZonesArray
        {
            get { return gStringZones.ToArray(); }
        }
        #endregion

        /// <summary>Lit le caractère suivant en sautant les caractères appartenant à une zone</summary>
        public int ReadChar()
        {
            int ic;
            while ((ic = ReadCharZone(true)) != -1)
            {
                if (giIndexZone == -1) break;
            }
            return ic;
        }

        public string ReadZone()
        {
            int i1 = giIndexNextChar;
            int ic = ReadCharZone(true);
            if (ic == -1) return null;

            StringZone sz = new StringZone();
            StringBuilder sb = new StringBuilder();
            sb.Append((char)ic);
            bool bZone = false;
            sz.IsZone = false;
            if (giIndexZone != -1)
            {
                sz.IsZone = true;
                sz.BeginZoneChar = gcCurrentBeginZoneChar;
                sz.EndZoneChar = gcCurrentEndZoneChar;
                bZone = true;
            }
            while ((ic = ReadCharZone(false)) != -1)
            {
                sb.Append((char)ic);
                if (bZone && giIndexZone == -1) break;
            }
            string s = sb.ToString();
            sz.String = s;
            if (bZone)
            {
                if (giIndexZone == -1)
                    sz.ContentString = s.Substring(1, s.Length - 2);
                else
                    sz.ContentString = s.Substring(1);
            }
            else
                sz.ContentString = s;
            if (bZone && giIndexZone != -1)
                sz.EndZoneChar = (char)0;
            gStringZones.Add(sz);
            return s;
        }

        public StringZone[] SplitZone()
        {
            List<string> zones = new List<string>();
            string s;
            while ((s = ReadZone()) != null)
            {
                zones.Add(s);
            }
            //return zones.ToArray();
            return gStringZones.ToArray();
        }

        public int ReadCharZone(bool bReadZone)
        {
            if (giIndexNextChar >= gsString.Length) return -1;
            char c = gsString[giIndexNextChar];
            if (giIndexZone != -1 && c == gcCurrentEndZoneChar)
            {
                gEndZonesChar.RemoveAt(giIndexZone--);
                gcCurrentBeginZoneChar = (char)0;
                if (giIndexZone != -1) gcCurrentEndZoneChar = gEndZonesChar[giIndexZone];
            }
            else
            {
                int i = gslCharZone.IndexOfKey(c);
                if (i != -1)
                {
                    if (giIndexZone == -1 && !bReadZone) return -1;
                    gcCurrentBeginZoneChar = c;
                    gcCurrentEndZoneChar = gslCharZone.Values[i];
                    gEndZonesChar.Add(gcCurrentEndZoneChar);
                    giIndexZone++;
                }
            }
            giIndexNextChar++;
            return c;
        }

        public static StringZone[] SplitZone(string sString, char[,] cCharZone)
        {
            StringZones sz = new StringZones(sString, cCharZone);
            return sz.SplitZone();
        }
    }
}
