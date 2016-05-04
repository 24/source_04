using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pb;
using pb.Data;

//namespace Print
namespace Download.Print
{
    class LeMonde0 // : IPrint
    {
        private Date _date;
        private LeMondeType _type;
        private int _printNumber = 0;
        private string _typeCode = null;
        private static Date _refPrintDate = new Date(2012, 12, 2);
        private static int _refPrintNumber = 21110;

        public LeMonde0(Date date, LeMondeType type)
        {
            _date = date;
            _type = type;
        }

        //NamedValues1
        public LeMonde0(NamedValues<ZValue> values)
        {
            // year          : mandatory, int or numeric string
            // day           : mandatory, int or numeric string
            // month         : mandatory, int or numeric string
            // type_quo      : optional, string null, "_quo"
            // types_quo_sup : optional, string[], "tv", "argent"
            // type_sup      : optional, string null, "livres"
            // number        : not used, int or numeric string
            // type          : not used, string "_quo", "-livres", "_quo+tv", "_quo+tv+argent"
            if (!values.ContainsKey("year"))
                throw new PBException("error creating LeMonde unknow year");
            if (!values.ContainsKey("month"))
                throw new PBException("error creating LeMonde unknow month");
            if (!values.ContainsKey("day"))
                throw new PBException("error creating LeMonde unknow day");
            _date = zdate.CreateDate(values);
            _type = GetType(values);
        }

        public string GetName() { return "le_monde"; }

        //NamedValues1
        public static LeMondeType GetType(NamedValues<ZValue> values)
        {
            LeMondeType type = LeMondeType.Unknow;
            if (values.ContainsKey("type_code"))
            {
                type = GetTypeCode((string)values["type_code"]);
            }
            else if (values.ContainsKey("type_quo") && values["type_quo"] != null)
            {
                type = LeMondeType.Quotidien;
                if (values.ContainsKey("types_quo_sup"))
                {
                    object o = values["types_quo_sup"];
                    if (o != null && !(o is string[]))
                        throw new PBException("error creating LeMonde value types_quo_sup should be a string array : {0}", o);
                    foreach (string s in (string[])o)
                        type |= GetTypeSup(s);
                }
            }
            else
            {
                if (!values.ContainsKey("type_sup"))
                    throw new PBException("error creating LeMonde unknow type_sup");
                object o = values["type_sup"];
                if (!(o is string))
                    throw new PBException("error creating LeMonde value type_sup should be a string : {0}", o);
                type = GetTypeSup((string)o);
            }
            return type;
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
            throw new PBException("error unknow le monde type sup : \"{0}\"", type);
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
            }
            throw new PBException("error unknow le monde type code : \"{0}\"", typeCode);
        }

        public int GetPrintNumber()
        {
            if (_printNumber == 0)
                _printNumber = GetPrintNumber(_date);
            return _printNumber;
        }

        public static bool PrintExists(Date date)
        {
            // Le monde - 2012-12-02 - no 21110.pdf
            // Le monde - 2012-10-12 - no 21066.pdf
            // Le monde - 2012-07-19 - no 20993.pdf
            // pas de journal le 1er mai sauf si c'est un dimanche, journal le dimanche 1er mai 2011
            // Test_GetLeMondePrintNumber("2012-04-29"); // ok  20925
            // Test_GetLeMondePrintNumber("2012-05-02"); // ok  20926
            if (date.DayOfWeek != DayOfWeek.Sunday && (date.Month != 5 || date.Day != 1 || date.DayOfWeek == DayOfWeek.Sunday))
                return true;
            else
                return false;
        }

        public static int GetPrintNumber(Date date)
        {
            //return pu.GetPrintNumber(PrintFrequency.Daily, date, _refPrintNumber, _refPrintDate, new PrintExistsDelegate(PrintExists));
            return zprint.GetDailyPrintNumber(date, _refPrintNumber, _refPrintDate, new PrintExistsDelegate(PrintExists));
        }

        //public static int GetPrintNumber(Date date)
        //{
        //    int no = _refPrintNumber;
        //    Date date2 = new Date(_refPrintDate.DateTime.Ticks);
        //    while (date > date2)
        //    {
        //        if (date2.DayOfWeek != DayOfWeek.Sunday && (date2.Month != 5 || date2.Day != 1 || date2.DayOfWeek == DayOfWeek.Sunday))
        //            no++;
        //        date2 = date2.AddDays(1);
        //    }
        //    while (date < date2)
        //    {
        //        if (date2.DayOfWeek != DayOfWeek.Monday && (date2.Month != 5 || date2.Day != 1 || date2.DayOfWeek == DayOfWeek.Sunday))
        //            no--;
        //        date2 = date2.AddDays(-1);
        //    }
        //    if (date != date2) throw new PBException("error date not found {0}", date.ToString());
        //    return no;
        //}

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

        public string GetFilename(int index = 0)
        {
            string file = string.Format("Le monde - {0:yyyy-MM-dd} - no {1} {2}", _date, GetPrintNumber(), GetTypeCode());
            if (index != 0) file += "_" + index.ToString();
            return file + ".pdf";
        }
    }
}
