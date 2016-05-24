using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Download.Print
{
    // create a PrintTitleManager
    // param :
    //   from NamedValues<ZValue> parameters or XElement xeConfig
    //     int Version, int GapDayBefore, int GapDayAfter
    //
    //   from XmlConfig PrintList1Config         (print_list1.xml)   (defined in current XmlConfig)
    //     FindPrints/Numbers/Number             pour récupérer le numéro d'un journal dans le titre
    //     FindPrints/Specials/Special           hors-série dans le titre
    //     FindPrints/UnknowPrintDirectory       ".06_unknow_print"
    //     Version < 5
    //       FindPrints/Dates/Date               date dans le titre
    //     Version >= 5
    //       FindPrints/Dates/Date2              date dans le titre
    //       FindPrints/Dates/DigitDate          date yyyyMMdd ddMMyyyy ou dd-MM-yy dans le titre

    public class PrintTitleManagerCreator
    {
        // parameters
        private int _version = 3;
        private int _gapDayBefore = 0;
        private int _gapDayAfter = 0;

        private XElement _xeConfig = null;
        private XmlConfig _printList1Config = null;

        public int Version { get { return _version; } set { _version = value; } }
        public int GapDayBefore { get { return _gapDayBefore; } set { _gapDayBefore = value; } }
        public int GapDayAfter { get { return _gapDayAfter; } set { _gapDayAfter = value; } }

        //public static PrintTitleManager Create(int version = 3, int gapDayBefore = 0, int gapDayAfter = 0)
        //{
        //    CreatePrintTitleManager create = new CreatePrintTitleManager();
        //    create._version = version;
        //    create._gapDayBefore = gapDayBefore;
        //    create._gapDayAfter = gapDayAfter;
        //    return create._Create();
        //}

        public void Init(XElement xe)
        {
            _xeConfig = xe;
            if (xe != null)
            {
                _version = xe.zXPathValue("Version").zTryParseAs(_version);
                _gapDayBefore = xe.zXPathValue("GapDayBefore").zTryParseAs(0);
                _gapDayAfter = xe.zXPathValue("GapDayAfter").zTryParseAs(0);
            }
            _printList1Config = XmlConfig.CurrentConfig.GetConfig("PrintList1Config");
        }

        public void SetParameters(NamedValues<ZValue> parameters)
        {
            if (parameters == null)
                return;
            foreach (KeyValuePair<string, ZValue> parameter in parameters)
                SetParameter(parameter);
        }

        public void SetParameter(KeyValuePair<string, ZValue> parameter)
        {
            //switch (parameter.Key.ToLower())
            switch (parameter.Key)
            {
                case "version":
                    _version = (int)parameter.Value;
                    break;
                case "gapdaybefore":
                    _gapDayBefore = (int)parameter.Value;
                    break;
                case "gapdayafter":
                    _gapDayAfter = (int)parameter.Value;
                    break;
            }
        }

        //public static PrintTitleManager CreatePrintTitleManager(int version = 3, int gapDayBefore = 0, int gapDayAfter = 0)
        public PrintTitleManager Create()
        {
            // version 2 : utilise le nouveau PrintTitleManager, l'ancien pattern de date FindPrints/Dates/Date avec l'ancien FindDateManager
            // version 3 : version 2 + le nouveau pattern de date FindPrints/Dates/DateNew avec le nouveau FindDateManager_new
            // version 4 (not used) : version 3 + découpe le titre avec "du" ou "-" (PrintTitleManager)
            // version 5 : version 3 +  new find date
            // version 6 : version 5 +  printTitleManager version 2

            if (_version < 3 || _version > 6)
                throw new PBException("bad version {0}", _version);

            //_printList1Config = XmlConfig.CurrentConfig.GetConfig("PrintList1Config");

            FindDateManager findDateManager = CreateFindDateManager();

            PrintTitleManager printTitleManager = new PrintTitleManager();
            printTitleManager.FindDateManager = findDateManager;
            printTitleManager.FindNumberManager = new FindNumberManager(_printList1Config.GetElements("FindPrints/Numbers/Number"), compileRegex: true);
            printTitleManager.FindSpecial = new RegexValuesList(_printList1Config.GetElements("FindPrints/Specials/Special"), compileRegex: true);
            printTitleManager.PrintDirectory = _printList1Config.GetExplicit("FindPrints/UnknowPrintDirectory");
            if (_version == 4)
                printTitleManager.SplitTitle = true;
            else
                printTitleManager.SplitTitle = false;
            if (_version == 6)
                printTitleManager.Version = 2;
            return printTitleManager;
        }

        //public static FindDateManager CreateFindDateManager(int version = 3, int gapDayBefore = 0, int gapDayAfter = 0)
        public FindDateManager CreateFindDateManager()
        {
            // version 5 : version 3 +  new find date
            //if (version < 3 || version > 6)
            //    throw new PBException("bad version {0}", version);
            FindDateManager findDateManager;
            if (_version < 5)
                findDateManager = new FindDateManager(_printList1Config.GetElements("FindPrints/Dates/Date"), compileRegex: true);
            else
            {
                findDateManager = new FindDateManager(_printList1Config.GetElements("FindPrints/Dates/Date2"), _printList1Config.GetElements("FindPrints/Dates/DigitDate"), compileRegex: true);
                findDateManager.MultipleSearch = true;
                findDateManager.GapDayBefore = _gapDayBefore;
                findDateManager.GapDayAfter = _gapDayAfter;
            }
            return findDateManager;
        }
    }
}
