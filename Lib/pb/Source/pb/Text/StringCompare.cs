namespace pb.Text
{
    // from http://stackoverflow.com/questions/4718965/c-sharp-string-comparison-ignoring-spaces-carriage-return-or-line-breaks
    // IEqualityComparer<string>
    public static class StringCompare
    {
        public static bool EqualsIgnoreWhiteSpace(string strx, string stry, bool ignoreCase = false)
        {
            if (strx == null) //stry may contain only whitespace
                return string.IsNullOrWhiteSpace(stry);

            else if (stry == null) //strx may contain only whitespace
                return string.IsNullOrWhiteSpace(strx);

            int ix = 0, iy = 0;
            for (; ix < strx.Length && iy < stry.Length; ix++, iy++)
            {
                char chx = strx[ix];
                char chy = stry[iy];

                //ignore whitespace in strx
                while (char.IsWhiteSpace(chx) && ++ix < strx.Length)
                {
                    //ix++;
                    chx = strx[ix];
                }

                //ignore whitespace in stry
                while (char.IsWhiteSpace(chy) && ++iy < stry.Length)
                {
                    //iy++;
                    chy = stry[iy];
                }

                if (ix == strx.Length && iy != stry.Length)
                { //end of strx, so check if the rest of stry is whitespace
                    for (int iiy = iy + 1; iiy < stry.Length; iiy++)
                    {
                        if (!char.IsWhiteSpace(stry[iiy]))
                            return false;
                    }
                    return true;
                }

                if (ix != strx.Length && iy == stry.Length)
                { //end of stry, so check if the rest of strx is whitespace
                    for (int iix = ix + 1; iix < strx.Length; iix++)
                    {
                        if (!char.IsWhiteSpace(strx[iix]))
                            return false;
                    }
                    return true;
                }

                //The current chars are not whitespace, so check that they're equal (case-insensitive)
                if (ignoreCase)
                {
                    chx = char.ToLowerInvariant(chx);
                    chy = char.ToLowerInvariant(chy);
                }

                if (chx != chy)
                    return false;
            }

            //If strx has more chars than stry
            for (; ix < strx.Length; ix++)
            {
                if (!char.IsWhiteSpace(strx[ix]))
                    return false;
            }

            //If stry has more chars than strx
            for (; iy < stry.Length; iy++)
            {
                if (!char.IsWhiteSpace(stry[iy]))
                    return false;
            }

            return true;
        }

        //public int GetHashCode(string obj)
        //{
        //    if (obj == null)
        //        return 0;

        //    int hash = 17;
        //    unchecked // Overflow is fine, just wrap
        //    {
        //        for (int i = 0; i < obj.Length; i++)
        //        {
        //            char ch = obj[i];
        //            if (!char.IsWhiteSpace(ch))
        //                //use this line for case-insensitivity
        //                hash = hash * 23 + char.ToLowerInvariant(ch).GetHashCode();

        //            //use this line for case-sensitivity
        //            //hash = hash * 23 + ch.GetHashCode();
        //        }
        //    }
        //    return hash;
        //}
    }
}
