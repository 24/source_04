using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Text;

namespace Download.Print
{
    class LeVifExpress : Print1
    {
        private bool _weekEnd = false;
        private bool _extra = false;
        private bool _focus = false;

        public LeVifExpress(string name, string title, PrintFrequency frequency, DayOfWeek? weekday, string directory, Date refPrintDate, int refPrintNumber, SpecialDay noPrintDays)
            : base(name, title, frequency, weekday, directory, refPrintDate, refPrintNumber, noPrintDays)
        {
        }

        public LeVifExpress(XElement xe, string baseDirectory, Dictionary<string, RegexValuesModel> regexModels = null)
            : base(xe, baseDirectory, regexModels)
        {
        }

        //NamedValues1
        public override bool TrySetValues(NamedValues<ZValue> values)
        {
            if (!base.TrySetValues(values))
                return false;
            _weekEnd = false;
            _extra = false;
            _focus = false;
            if (values.ContainsKey("we"))
                _weekEnd = true;
            if (values.ContainsKey("extra"))
                _extra = true;
            if (values.ContainsKey("focus"))
                _focus = true;
            if (values.ContainsKey("type_code"))
                SetType((string)values["type_code"]);
            return true;
        }

        private void SetType(string type_code)
        {
            if (type_code == null)
                return;
            if (_trace)
                Trace.CurrentTrace.WriteLine("type_code \"{0}\"", type_code);
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

        private string GetTypeCode()
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

        public override string GetFilename(int index = 0)
        {
            string file = string.Format("{0} - {1:yyyy-MM-dd} - no {2}{3}", _title, GetPrintDate(), GetPrintNumber(), GetTypeCode());
            if (index != 0) file += "_" + index.ToString();
            return file + ".pdf";
        }
    }
}
