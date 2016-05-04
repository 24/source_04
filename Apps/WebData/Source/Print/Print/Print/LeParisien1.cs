using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Text;

//namespace Print
namespace Download.Print
{
    public class LeParisien : Print1
    {
        private bool _eco = false;
        private bool _mag = false;

        public LeParisien(string name, string title, PrintFrequency frequency, DayOfWeek? weekday, string directory, Date refPrintDate, int refPrintNumber, SpecialDay noPrintDays)
            : base(name, title, frequency, weekday, directory, refPrintDate, refPrintNumber, noPrintDays)
        {
        }

        public LeParisien(XElement xe, string baseDirectory, Dictionary<string, RegexValuesModel> regexModels = null)
            : base(xe, baseDirectory, regexModels)
        {
        }

        //NamedValues1
        public override bool TrySetValues(NamedValues<ZValue> values)
        {
            if (!base.TrySetValues(values))
                return false;
            _eco = false;
            _mag = false;
            if (values.ContainsKey("eco"))
                _eco = true;
            if (values.ContainsKey("mag"))
                _mag = true;
            if (values.ContainsKey("type_code"))
                SetType((string)values["type_code"]);
            return true;
        }

        private void SetType(string type_code)
        {
            if (type_code == null)
                return;
            if (_trace)
            {
                Trace.CurrentTrace.WriteLine("type_code \"{0}\"", type_code);
            }
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
            Date date = GetPrintDate();
            if (date.DayOfWeek == DayOfWeek.Sunday)
                sunday = " bis";
            string sup = "";
            if (_eco)
                sup = " -éco";
            if (_mag)
                sup += " -mag";
            string file = string.Format("{0} - {1:yyyy-MM-dd} - no {2}{3}{4}", _title, date, GetPrintNumber(), sunday, sup);
            if (index != 0) file += "_" + index.ToString();
            return file + ".pdf";
        }
    }
}
