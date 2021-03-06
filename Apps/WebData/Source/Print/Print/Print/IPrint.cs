﻿using System.Collections.Generic;
using pb;

namespace Download.Print
{
    //public interface IPrintCreate
    //{
    //    string GetName();
    //    IPrint CreatePrint(Dictionary<string, object> values);
    //}

    public interface IPrint
    {
        string GetName();
        void SetDate(Date date);
        void SetValues(Dictionary<string, object> values);
        int GetPrintNumber();
        string GetFilename(int index = 0);
    }
}
