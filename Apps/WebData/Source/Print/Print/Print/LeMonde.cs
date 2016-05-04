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
    public class PrintLeMonde : Print
    {
        //public PrintLeMonde(string name, string title, PrintFrequency frequency, string directory, Date refPrintDate, int refPrintNumber, DayOfWeek? weekday, SpecialDay noPrintDays)
        //    : base(name, title, frequency, directory, refPrintDate, refPrintNumber, weekday, noPrintDays)
        //{
        //}

        public PrintLeMonde(XElement xe, string baseDirectory = null, Dictionary<string, RegexValuesModel> regexModels = null)
            : base(xe, baseDirectory, regexModels)
        {
        }

        public override PrintIssue NewPrintIssue()
        {
            return new PrintIssueLeMonde(this);
        }

        public override PrintIssue NewPrintIssue(Date date)
        {
            return new PrintIssueLeMonde(this, date);
        }

        public override PrintIssue NewPrintIssue(int printNumber)
        {
            return new PrintIssueLeMonde(this, printNumber);
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

        public override int GetPrintNumber(Date date, bool throwException = true)
        {
            //int refPrintNumber = _refPrintNumber;
            //Date refPrintDate = _refPrintDate;
            PrintDateNumberReference dateNumberReference = _dateNumberReferences.GetReference(date);
            int refPrintNumber = dateNumberReference.number;
            Date refPrintDate = dateNumberReference.date;
            if (_lastDate != null)
            {
                refPrintNumber = _lastPrintNumber;
                refPrintDate = (Date)_lastDate;
            }
            int printNumber = zprint.GetDailyPrintNumber(date, refPrintNumber, refPrintDate, new PrintExistsDelegate(PrintExists));
            _lastDate = date;
            _lastPrintNumber = printNumber;
            return printNumber;
        }

    }

    public class PrintIssueLeMonde : PrintIssue
    {
        private LeMondeType _type = LeMondeType.Unknow;
        private string _typeCode = null;
        private string _dossier = null;

        public PrintIssueLeMonde(Print print)
            : base(print)
        {
        }

        public PrintIssueLeMonde(Print print, Date date)
            : base(print, date)
        {
        }

        public PrintIssueLeMonde(Print print, int printNumber)
            : base(print, printNumber)
        {
        }

        // type_dossier  : name
        public override bool TrySetValues(NamedValues<ZValue> values)
        {
            if (!base.TrySetValues(values))
                return false;

            LeMondeType type;
            if (!TryGetType(values, out type))
                return false;
            if (type != LeMondeType.Unknow)
                _type = type;

            if (values.ContainsKey("type_dossier"))
            {
                ZValue v = values["type_dossier"];
                if (v != null && v is ZString)
                {
                    _dossier = (string)v;
                }
            }

            _typeCode = null;
            return true;
        }

        public string GetTypeCode()
        {
            if (_typeCode == null)
            {
                //if ((_type & LeMondeType.Quotidien) == LeMondeType.Quotidien)
                //    _typeCode = "_quo" + GetTypeCodeSup('+');
                if ((_type & LeMondeType.Quotidien) == LeMondeType.Quotidien)
                    _typeCode = "";
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
            //string file = string.Format("Le monde - {0:yyyy-MM-dd} - no {1} {2}", Date, GetPrintNumber(), GetTypeCode());
            string file = string.Format("Le monde - {0:yyyy-MM-dd} - no {1}", Date, GetPrintNumber());
            string typeCode = GetTypeCode();
            if (typeCode != "")
                file += " " + typeCode;
            if (_dossier != null)
                file += " -dossier " + _dossier;
            if (index != 0)
                file += "_" + index.ToString();
            return file + ".pdf";
        }

        // type_code     : quo, arg, arh, dos, mde, edu, aut, peh, liv, mag, mdv, sch, scq, sph, sty, nyt, tel
        // type_quo      :
        // fix_type_quo  :
        // types_quo_sup : argent, culture, dossier, éco, eco, édu, edu, festival, géo, geo, livres, mag, mode, science, sport, style, nyt, tv, document, élection, election, hebdo, sup
        // type_sup      : argent, culture, dossier, éco, eco, édu, edu, festival, géo, geo, livres, mag, mode, science, sport, style, nyt, tv, document, élection, election, hebdo, sup
        // type_sups     : argent, culture, dossier, éco, eco, édu, edu, festival, géo, geo, livres, mag, mode, science, sport, style, nyt, tv, document, élection, election, hebdo, sup
        // fix_type_eco  :
        // fix_type_tv   :
        public static bool TryGetType(NamedValues<ZValue> values, out LeMondeType type)
        {
            type = LeMondeType.Unknow;
            if (values.ContainsKey("type_code"))
            {
                type = GetTypeCode((string)values["type_code"]);
                if (type == LeMondeType.Unknow)
                {
                    values.SetError("error unknow le monde type sup : \"{0}\"", values["type_code"]);
                    return false;
                }
            }
            else if ((values.ContainsKey("type_quo") && values["type_quo"] != null) || values.ContainsKey("fix_type_quo"))
            {
                type = LeMondeType.Quotidien;
                if (values.ContainsKey("types_quo_sup"))
                {
                    ZValue v = values["types_quo_sup"];
                    if (v != null && !(v is ZStringArray))
                    {
                        values.SetError("error creating LeMonde value types_quo_sup should be a string array : {0}", v);
                        return false;
                    }
                    foreach (string s in (string[])v)
                    {
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
                ZValue v = values["type_sup"];
                if (!(v is ZString))
                {
                    values.SetError("error creating LeMonde value type_sup should be a string : {0}", v);
                    return false;
                }
                type = GetTypeSup((string)v);
                if (type == LeMondeType.Unknow)
                {
                    values.SetError("error unknow le monde type sup : \"{0}\"", v);
                    return false;
                }
            }
            else if (values.ContainsKey("type_sups"))
            {
                ZValue v = values["type_sups"];
                if (v != null)
                {
                    if (v is ZString)
                    {
                        LeMondeType type2 = GetTypeSup((string)v);
                        if (type2 == LeMondeType.Unknow)
                        {
                            values.SetError("error unknow le monde type sup : \"{0}\"", (string)v);
                            return false;
                        }
                        type = type2;
                    }
                    else if (v is ZStringArray)
                    {
                        type = LeMondeType.Unknow;
                        foreach (string s in (string[])v)
                        {
                            LeMondeType type2 = GetTypeSup(s);
                            if (type2 == LeMondeType.Unknow)
                            {
                                values.SetError("error unknow le monde type sup : \"{0}\"", s);
                                //return false;
                            }
                            else
                                type |= type2;
                        }
                    }
                    else
                    {
                        values.SetError("error creating LeMonde value type_sups should be a string or string array : {0}", v);
                        return false;
                    }
                }
            }
            else if (values.ContainsKey("fix_type_eco"))
                type = LeMondeType.Economie;
            else if (values.ContainsKey("fix_type_tv"))
                type = LeMondeType.TV;
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
