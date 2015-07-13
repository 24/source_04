using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Text;

// A faire :
//   supprimer baseDirectory dans le constructeur

namespace Print
{
    public class PrintLeVifExpress : Print
    {
        //public PrintLeVifExpress(string name, string title, PrintFrequency frequency, string directory, Date refPrintDate, int refPrintNumber, DayOfWeek? weekday, SpecialDay noPrintDays)
        //    : base(name, title, frequency, directory, refPrintDate, refPrintNumber, weekday, noPrintDays)
        //{
        //}

        public PrintLeVifExpress(XElement xe, string baseDirectory = null, Dictionary<string, RegexValuesModel> regexModels = null)
            : base(xe, baseDirectory, regexModels)
        {
        }

        public override PrintIssue NewPrintIssue()
        {
            return new PrintIssueLeVifExpress(this);
        }

        public override PrintIssue NewPrintIssue(Date date)
        {
            return new PrintIssueLeVifExpress(this, date);
        }

        public override PrintIssue NewPrintIssue(int printNumber)
        {
            return new PrintIssueLeVifExpress(this, printNumber);
        }
    }

    public class PrintIssueLeVifExpress : PrintIssue
    {
        private bool _weekEnd = false;
        private bool _extra = false;
        private bool _focus = false;

        public PrintIssueLeVifExpress(Print print)
            : base(print)
        {
        }

        public PrintIssueLeVifExpress(Print print, Date date)
            : base(print, date)
        {
        }

        public PrintIssueLeVifExpress(Print print, int printNumber)
            : base(print, printNumber)
        {
        }

        //NamedValues1
        public override bool TrySetValues(NamedValues<ZValue> values)
        {
            if (!base.TrySetValues(values))
                return false;
            //_weekEnd = false;
            //_extra = false;
            //_focus = false;
            if (values.ContainsKey("we"))
                _weekEnd = true;
            if (values.ContainsKey("extra"))
                _extra = true;
            if (values.ContainsKey("focus"))
                _focus = true;
            if (values.ContainsKey("type_code"))
                //string
                SetType((string)values["type_code"]);
            return true;
        }

        private void SetType(string type_code)
        {
            if (type_code == null)
                return;
            WriteLine("type_code \"{0}\"", type_code);
            switch (type_code.ToLower())
            {
                case "we":
                    _weekEnd = true;
                    break;
                case "extra":
                    _extra = true;
                    break;
                case "focus":
                    _focus = true;
                    break;
            }
        }

        protected override string GetFilenameOption()
        {
            string code = " ";
            if (_weekEnd)
                code += "-we";
            if (_extra)
                code += "-extra";
            if (_focus)
                code += "-focus";
            if (code == " ")
                code = null;
            return code;
        }

        //public override string GetFilename(int index = 0)
        //{
        //    string file = string.Format("{0} - {1:yyyy-MM-dd} - no {2}{3}", _print.Title, GetPrintDate(), GetPrintNumber(), GetFilenameOption());
        //    if (index != 0) file += "_" + index.ToString();
        //    return file + ".pdf";
        //}
    }
}
