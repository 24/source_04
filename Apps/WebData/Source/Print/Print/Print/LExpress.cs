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
    public class PrintLExpress : Print
    {
        //public PrintLExpress(string name, string title, PrintFrequency frequency, string directory, Date refPrintDate, int refPrintNumber, DayOfWeek? weekday, SpecialDay noPrintDays)
        //    : base(name, title, frequency, directory, refPrintDate, refPrintNumber, weekday, noPrintDays)
        //{
        //}

        public PrintLExpress(XElement xe, string baseDirectory = null, Dictionary<string, RegexValuesModel> regexModels = null)
            : base(xe, baseDirectory, regexModels)
        {
        }

        public override PrintIssue NewPrintIssue()
        {
            return new PrintIssueLExpress(this);
        }

        public override PrintIssue NewPrintIssue(Date date)
        {
            return new PrintIssueLExpress(this, date);
        }

        public override PrintIssue NewPrintIssue(int printNumber)
        {
            return new PrintIssueLExpress(this, printNumber);
        }
    }

    public class PrintIssueLExpress : PrintIssue
    {
        private bool _es = false;  // édition spéciale
        private bool _styles = false;

        public PrintIssueLExpress(Print print)
            : base(print)
        {
        }

        public PrintIssueLExpress(Print print, Date date)
            : base(print, date)
        {
        }

        public PrintIssueLExpress(Print print, int printNumber)
            : base(print, printNumber)
        {
        }

        //NamedValues1
        public override bool TrySetValues(NamedValues<ZValue> values)
        {
            if (!base.TrySetValues(values))
                return false;
            if (values.ContainsKey("es"))
                _es = true;
            if (values.ContainsKey("styles"))
                _styles = true;
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
                case "es":
                    _es = true;
                    break;
                case "styles":
                    _styles = true;
                    break;
            }
        }

        protected override string GetFilenameOption()
        {
            string code = " ";
            if (_es)
                code += "-es";
            if (_styles)
                code += "-styles";
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
