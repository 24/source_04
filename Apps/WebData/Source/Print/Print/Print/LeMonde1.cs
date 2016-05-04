using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using pb;
using pb.Data;
using pb.Text;

//namespace Print
namespace Download.Print
{
    class LeMonde : Print1
    {
        private LeMondeType _type;
        private string _typeCode = null;

        public LeMonde(string name, string title, PrintFrequency frequency, DayOfWeek? weekday, string directory, Date refPrintDate, int refPrintNumber, SpecialDay noPrintDays)
            : base(name, title, frequency, weekday, directory, refPrintDate, refPrintNumber, noPrintDays)
        {
        }

        public LeMonde(XElement xe, string baseDirectory, Dictionary<string, RegexValuesModel> regexModels = null)
            : base(xe, baseDirectory, regexModels)
        {
        }

        //NamedValues1
        public override bool TrySetValues(NamedValues<ZValue> values)
        {
            if (!base.TrySetValues(values))
                return false;
            //_type = GetType(values);
            if (!TryGetType(values, out _type))
                return false;
            _typeCode = null;
            return true;
        }

        public static bool PrintExists(Date date)
        {
            // pas de journal le 1er mai sauf si c'est un dimanche, journal le dimanche 1er mai 2011
            //if (date.DayOfWeek != DayOfWeek.Sunday && (date.Month != 5 || date.Day != 1 || date.DayOfWeek == DayOfWeek.Sunday))
            if (date.DayOfWeek != DayOfWeek.Monday && (date.Month != 5 || date.Day != 1 || date.DayOfWeek == DayOfWeek.Sunday))
                return true;
            else
                return false;
        }

        public override int GetPrintNumber(Date date)
        {
            //return pu.GetPrintNumber(date, _refPrintNumber, _refPrintDate, new PrintExistsDelegate(PrintExists));
            //int printNumber;
            //if (_lastDate != null)
            //    printNumber = pu.GetPrintNumber(date, _lastPrintNumber, (Date)_lastDate, new PrintExistsDelegate(PrintExists));
            //else
            //    printNumber = pu.GetPrintNumber(date, _refPrintNumber, _refPrintDate, new PrintExistsDelegate(PrintExists));
            int refPrintNumber = _refPrintNumber;
            Date refPrintDate = _refPrintDate;
            if (_lastDate != null)
            {
                refPrintNumber = _lastPrintNumber;
                refPrintDate = (Date)_lastDate;
            }
            //int printNumber = pu.GetPrintNumber(_frequency, date, refPrintNumber, refPrintDate, new PrintExistsDelegate(PrintExists));
            int printNumber = zprint.GetDailyPrintNumber(date, refPrintNumber, refPrintDate, new PrintExistsDelegate(PrintExists));
            _lastDate = date;
            _lastPrintNumber = printNumber;
            return printNumber;
        }

        public string GetTypeCode()
        {
            if (_typeCode == null)
            {
                if ((_type & LeMondeType.Quotidien) == LeMondeType.Quotidien)
                    _typeCode = "_quo" + GetTypeCodeSup('+');
                else
                    _typeCode = GetTypeCodeSup('-');
            }
            return _typeCode;
        }

        private string GetTypeCodeSup(char prefix)
        {
            StringBuilder sb = new StringBuilder();
            if ((_type & LeMondeType.Argent) == LeMondeType.Argent)
            {
                sb.Append(prefix);
                sb.Append("argent");
            }
            if ((_type & LeMondeType.Culture) == LeMondeType.Culture)
            {
                sb.Append(prefix);
                sb.Append("culture");
            }
            if ((_type & LeMondeType.Dossier) == LeMondeType.Dossier)
            {
                sb.Append(prefix);
                sb.Append("dossier");
            }
            if ((_type & LeMondeType.Economie) == LeMondeType.Economie)
            {
                sb.Append(prefix);
                sb.Append("éco");
            }
            if ((_type & LeMondeType.Education) == LeMondeType.Education)
            {
                sb.Append(prefix);
                sb.Append("édu");
            }
            if ((_type & LeMondeType.Festival) == LeMondeType.Festival)
            {
                sb.Append(prefix);
                sb.Append("festival");
            }
            if ((_type & LeMondeType.GeoPolitique) == LeMondeType.GeoPolitique)
            {
                sb.Append(prefix);
                sb.Append("géo");
            }
            if ((_type & LeMondeType.Livres) == LeMondeType.Livres)
            {
                sb.Append(prefix);
                sb.Append("livres");
            }
            if ((_type & LeMondeType.Magazine) == LeMondeType.Magazine)
            {
                sb.Append(prefix);
                sb.Append("mag");
            }
            if ((_type & LeMondeType.Mode) == LeMondeType.Mode)
            {
                sb.Append(prefix);
                sb.Append("mode");
            }
            if ((_type & LeMondeType.Science) == LeMondeType.Science)
            {
                sb.Append(prefix);
                sb.Append("science");
            }
            if ((_type & LeMondeType.Sport) == LeMondeType.Sport)
            {
                sb.Append(prefix);
                sb.Append("sport");
            }
            if ((_type & LeMondeType.Style) == LeMondeType.Style)
            {
                sb.Append(prefix);
                sb.Append("style");
            }
            if ((_type & LeMondeType.TheNewyorkTimes) == LeMondeType.TheNewyorkTimes)
            {
                sb.Append(prefix);
                sb.Append("nyt");
            }
            if ((_type & LeMondeType.TV) == LeMondeType.TV)
            {
                sb.Append(prefix);
                sb.Append("tv");
            }
            if ((_type & LeMondeType.Document) == LeMondeType.Document)
            {
                sb.Append(prefix);
                sb.Append("document");
            }
            if ((_type & LeMondeType.Election) == LeMondeType.Election)
            {
                sb.Append(prefix);
                sb.Append("élection");
            }
            if ((_type & LeMondeType.SelectionHebdomadaire) == LeMondeType.SelectionHebdomadaire)
            {
                sb.Append(prefix);
                sb.Append("hebdo");
            }
            if ((_type & LeMondeType.Supplement) == LeMondeType.Supplement)
            {
                sb.Append(prefix);
                sb.Append("sup");
            }
            return sb.ToString();
        }

        public override string GetFilename(int index = 0)
        {
            //string file = string.Format("Le monde - {0:yyyy-MM-dd} - no {1} {2}", _date, GetPrintNumber(), GetTypeCode());
            string file = string.Format("Le monde - {0:yyyy-MM-dd} - no {1} {2}", GetPrintDate(), GetPrintNumber(), GetTypeCode());
            if (index != 0) file += "_" + index.ToString();
            return file + ".pdf";
        }

        //NamedValues1
        public static bool TryGetType(NamedValues<ZValue> values, out LeMondeType type)
        {
            type = LeMondeType.Unknow;
            if (values.ContainsKey("type_code"))
            {
                type = GetTypeCode((string)values["type_code"]);
                if (type == LeMondeType.Unknow)
                {
                    values.SetError("error unknow le monde type sup : \"{0}\"", (string)values["type_code"]);
                    return false;
                }
            }
            else if ((values.ContainsKey("type_quo") && values["type_quo"] != null) || values.ContainsKey("fix_type_quo"))
            //else if (values.ContainsKey("type_quo"))
            {
                type = LeMondeType.Quotidien;
                if (values.ContainsKey("types_quo_sup"))
                {
                    object o = values["types_quo_sup"];
                    if (o != null && !(o is string[]))
                    {
                        //throw new PBException("error creating LeMonde value types_quo_sup should be a string array : {0}", o);
                        values.SetError("error creating LeMonde value types_quo_sup should be a string array : {0}", o);
                        return false;
                    }
                    foreach (string s in (string[])o)
                    {
                        //type |= GetTypeSup(s);
                        LeMondeType type2 = GetTypeSup(s);
                        if (type2 == LeMondeType.Unknow)
                        {
                            values.SetError("error unknow le monde type sup : \"{0}\"", s);
                            return false;
                        }
                        type |= type2;
                    }
                }
            }
            else if (values.ContainsKey("type_sup"))
            {
                //if (!values.ContainsKey("type_sup"))
                //    throw new PBException("error creating LeMonde unknow type_sup");
                object o = values["type_sup"];
                if (!(o is string))
                {
                    //throw new PBException("error creating LeMonde value type_sup should be a string : {0}", o);
                    values.SetError("error creating LeMonde value type_sup should be a string : {0}", o);
                    return false;
                }
                type = GetTypeSup((string)o);
                if (type == LeMondeType.Unknow)
                {
                    values.SetError("error unknow le monde type sup : \"{0}\"", o);
                    return false;
                }
            }
            else if (values.ContainsKey("fix_type_eco"))
                type = LeMondeType.Economie;
            else if (values.ContainsKey("fix_type_tv"))
                type = LeMondeType.TV;
            else
            {
                //throw new PBException("error creating LeMonde unknow type");
                values.SetError("error creating LeMonde unknow type");
                return false;
            }
            //return type;
            return true;
        }

        public static LeMondeType GetTypeSup(string type)
        {
            switch (type.ToLower())
            {
                case "argent":
                    return LeMondeType.Argent;
                case "culture":
                    return LeMondeType.Culture;
                case "dossier":
                    return LeMondeType.Dossier;
                case "éco":
                case "eco":
                    return LeMondeType.Economie;
                case "édu":
                case "edu":
                    return LeMondeType.Education;
                case "festival":
                    return LeMondeType.Festival;
                case "géo":
                case "geo":
                    return LeMondeType.GeoPolitique;
                case "livres":
                    return LeMondeType.Livres;
                case "mag":
                    return LeMondeType.Magazine;
                case "mode":
                    return LeMondeType.Mode;
                case "science":
                    return LeMondeType.Science;
                case "sport":
                    return LeMondeType.Sport;
                case "style":
                    return LeMondeType.Style;
                case "nyt":
                    return LeMondeType.TheNewyorkTimes;
                case "tv":
                    return LeMondeType.TV;
                case "document":
                    return LeMondeType.Document;
                case "élection":
                case "election":
                    return LeMondeType.Election;
                case "hebdo":
                    return LeMondeType.SelectionHebdomadaire;
                case "sup":
                    return LeMondeType.Supplement;
            }
            //throw new PBException("error unknow le monde type sup : \"{0}\"", type);
            return LeMondeType.Unknow;
        }

        public static LeMondeType GetTypeCode(string typeCode)
        {
            switch (typeCode.ToLower())
            {
                case "quo":
                    return LeMondeType.Quotidien;
                case "arg":
                    return LeMondeType.Argent;
                case "arh":
                    return LeMondeType.Culture;
                case "dos":
                    return LeMondeType.Dossier;
                case "mde":
                    return LeMondeType.Economie;
                case "edu":
                    return LeMondeType.Education;
                case "aut":
                    return LeMondeType.Festival;
                case "peh":
                    return LeMondeType.GeoPolitique;
                case "liv":
                    return LeMondeType.Livres;
                case "mag":
                    return LeMondeType.Magazine;
                case "mdv":
                    return LeMondeType.Mode;
                case "sch":
                    return LeMondeType.Science;
                case "scq":
                    return LeMondeType.Economie;
                case "sph":
                    return LeMondeType.Sport;
                case "sty":
                    return LeMondeType.Style;
                case "nyt":
                    return LeMondeType.TheNewyorkTimes;
                case "tel":
                    return LeMondeType.TV;
                default:
                    return LeMondeType.Unknow;
            }
            //throw new PBException("error unknow le monde type code : \"{0}\"", typeCode);
        }
    }
}
