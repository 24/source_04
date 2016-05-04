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

//namespace Print
namespace Download.Print
{
    public class PrintLeParisien : Print
    {
        //public PrintLeParisien(string name, string title, PrintFrequency frequency, string directory, Date refPrintDate, int refPrintNumber, DayOfWeek? weekday, SpecialDay noPrintDays)
        //    : base(name, title, frequency, directory, refPrintDate, refPrintNumber, weekday, noPrintDays)
        //{
        //}

        public PrintLeParisien(XElement xe, string baseDirectory = null, Dictionary<string, RegexValuesModel> regexModels = null)
            : base(xe, baseDirectory, regexModels)
        {
        }

        public override PrintIssue NewPrintIssue()
        {
            return new PrintIssueLeParisien(this);
        }

        public override PrintIssue NewPrintIssue(Date date)
        {
            return new PrintIssueLeParisien(this, date);
        }

        public override PrintIssue NewPrintIssue(int printNumber)
        {
            return new PrintIssueLeParisien(this, printNumber);
        }
    }

    public class PrintIssueLeParisien : PrintIssue
    {
        private bool _eco = false;
        private bool _mag = false;

        public PrintIssueLeParisien(Print print)
            : base(print)
        {
        }

        public PrintIssueLeParisien(Print print, Date date)
            : base(print, date)
        {
        }

        public PrintIssueLeParisien(Print print, int printNumber)
            : base(print, printNumber)
        {
        }

        //NamedValues1
        public override bool TrySetValues(NamedValues<ZValue> values)
        {
            if (!base.TrySetValues(values))
                return false;
            //_eco = false;
            //_mag = false;
            if (values.ContainsKey("eco"))
                _eco = true;
            if (values.ContainsKey("mag"))
                _mag = true;
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
                case "éco":
                    _eco = true;
                    break;
                case "mag":
                    _mag = true;
                    break;
            }
        }

        public override string GetFilename(int index = 0)
        {
            string sunday = "";
            Date? date = Date;
            if (date != null && ((Date)date).DayOfWeek == DayOfWeek.Sunday)
                sunday = " bis";
            string sup = "";
            if (_eco)
                sup = " -éco";
            if (_mag)
                sup += " -mag";
            //string file = string.Format("{0} - {1:yyyy-MM-dd} - no {2}{3}{4}", _print.Title, date, GetPrintNumber(), sunday, sup);
            string file = _print.Title;
            if (date != null)
                file += string.Format(" - {0:yyyy-MM-dd}", date);
            file += string.Format(" - no {0}{1}{2}", GetPrintNumber(), sunday, sup);
            if (index != 0)
                file += "_" + index.ToString();
            return file + ".pdf";
        }
    }
}
