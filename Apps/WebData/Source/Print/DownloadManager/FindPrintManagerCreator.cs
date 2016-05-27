using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Download.Print
{
    // create a FindPrintManager
    //   create PrintTitleManager, FindDayManager, PrintManager, FindPrintManager
    // param :
    //   from NamedValues<ZValue> parameters or XElement xeConfig
    //     int Version, bool DailyPrintManager, int GapDayBefore, int GapDayAfter
    //
    // FindPrintManager
    //   from print_list1.xml                                (PrintList1Config in current XmlConfig)
    //     FindPrints/DefaultPrintDirectory                  ".05_new_print"
    //     FindPrints/UnknowPrintDirectory                   ".06_unknow_print"
    //     Post type directories Dictionary<PrintType, string>
    //     FindPrints/PostTypeDirectories/Print              PrintType.Print            "print"
    //     FindPrints/PostTypeDirectories/Book               PrintType.Book             "book"
    //     FindPrints/PostTypeDirectories/Comics             PrintType.Comics           "comics"
    //     FindPrints/PostTypeDirectories/UnknowEBook        PrintType.UnknowEBook      "unknow_ebook"
    //     FindPrints/PostTypeDirectories/UnknowPostType     PrintType.Unknow           "unknow_post_type"
    //   from print_list2.xml                                (PrintList2Config in current XmlConfig)
    //     FindPrints/Prints/Print                           paramétrage simple pour chaque journal
    //
    // FindDayManager
    //   from XmlConfig PrintList1Config                     (print_list1.xml)   (defined in current XmlConfig)
    //     FindPrints/Dates/ShortDay                         jour du mois dans les packs de journaux



    //
    // PrintManager
    //   from XmlConfig PrintConfig              (print_config.xml)  (defined in current XmlConfig)
    //     Print/Prints/Print                    paramétrage détaillé pour chaque journal
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
    public class FindPrintManagerCreator
    {
        // parameters
        private int _version = 3;
        private bool _dailyPrintManager = false;
        private int _gapDayBefore = 0;
        private int _gapDayAfter = 0;
        private bool _trySplitTitle = false;

        private XElement _xeConfig = null;
        private NamedValues<ZValue> _parameters = null;

        private XmlConfig _printConfig = null;
        private XmlConfig _printList1Config = null;
        private XmlConfig _printList2Config = null;

        private PrintTitleManager _printTitleManager = null;
        private FindDayManager _findDayManager = null;
        private PrintManager _printManager = null;
        private Dictionary<PrintType, string> _postTypeDirectories = null;
        private FindPrintManager _findPrintManager = null;

        public int Version { get { return _version; } set { _version = value; } }
        public bool DailyPrintManager { get { return _dailyPrintManager; } set { _dailyPrintManager = value; } }
        public int GapDayBefore { get { return _gapDayBefore; } set { _gapDayBefore = value; } }
        public int GapDayAfter { get { return _gapDayAfter; } set { _gapDayAfter = value; } }
        public bool TrySplitTitle { get { return _trySplitTitle; } set { _trySplitTitle = value; } }

        //public static FindPrintManager Create(int version = 3, bool dailyPrintManager = false, int gapDayBefore = 0, int gapDayAfter = 0)
        //{
        //    CreateFindPrintManager create = new CreateFindPrintManager();
        //    create._version = version;
        //    create._dailyPrintManager = dailyPrintManager;
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
                _dailyPrintManager = xe.zXPathValue("DailyPrintManager").zTryParseAs(false);
                _gapDayBefore = xe.zXPathValue("GapDayBefore").zTryParseAs(0);
                _gapDayAfter = xe.zXPathValue("GapDayAfter").zTryParseAs(0);
                _trySplitTitle = xe.zXPathValue("TrySplitTitle").zTryParseAs(false);
            }
        }

        public void SetParameters(NamedValues<ZValue> parameters)
        {
            _parameters = parameters;
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
                case "dailyprintmanager":
                    _dailyPrintManager = (bool)parameter.Value;
                    break;
                case "gapdaybefore":
                    _gapDayBefore = (int)parameter.Value;
                    break;
                case "gapdayafter":
                    _gapDayAfter = (int)parameter.Value;
                    break;
                case "trysplittitle":
                    _trySplitTitle = (bool)parameter.Value;
                    break;
            }
        }

        public FindPrintManager Create()
        {
            if (_version < 3 || _version > 6)
                throw new PBException("bad version {0}", _version);

            XmlConfig config = XmlConfig.CurrentConfig;
            _printConfig = config.GetConfig("PrintConfig");
            _printList1Config = config.GetConfig("PrintList1Config");
            _printList2Config = config.GetConfig("PrintList2Config");

            //_printTitleManager = CreatePrintTitleManager.Create(_version, _gapDayBefore, _gapDayAfter);
            PrintTitleManagerCreator createPrintTitleManager = new PrintTitleManagerCreator();
            createPrintTitleManager.Init(_xeConfig);
            createPrintTitleManager.SetParameters(_parameters);
            createPrintTitleManager.Version = _version;
            createPrintTitleManager.GapDayBefore = _gapDayBefore;
            createPrintTitleManager.GapDayAfter = _gapDayAfter;
            _printTitleManager = createPrintTitleManager.Create();

            _findDayManager = CreateFindDayManager();
            _printManager = CreatePrintManager();
            _postTypeDirectories = CreatePostTypeDirectories();
            _findPrintManager = _CreateFindPrintManager();

            return _findPrintManager;
        }

        private FindPrintManager _CreateFindPrintManager()
        {
            FindPrintManager findPrintManager = new FindPrintManager();
            findPrintManager.TrySplitTitle = _trySplitTitle;
            findPrintManager.PrintTitleManager = _printTitleManager;
            findPrintManager.FindPrintList = new RegexValuesList(_printList2Config.GetElements("FindPrints/Prints/Print"), compileRegex: true);
            if (_dailyPrintManager)
            {
                findPrintManager.FindPrintList.Add(_printList2Config.GetElements("FindPrints/Prints/ShortPrint"), compileRegex: true);
                findPrintManager.FindDayManager = _findDayManager;
                findPrintManager.UseFindDay = true;
                findPrintManager.SetGapDayBefore(_gapDayBefore);
                findPrintManager.SetGapDayAfter(_gapDayAfter);
            }
            findPrintManager.PrintManager = _printManager;
            findPrintManager.PostTypeDirectories = _postTypeDirectories;
            findPrintManager.DefaultPrintDirectory = _printList1Config.Get("FindPrints/DefaultPrintDirectory");
            findPrintManager.UnknowPrintDirectory = _printList1Config.Get("FindPrints/UnknowPrintDirectory");

            //if (_version >= 6)
            //    findPrintManager.Version = 2;

            return findPrintManager;
        }

        public FindDayManager CreateFindDayManager()
        {
            return new FindDayManager(_printList1Config.GetElements("FindPrints/Dates/ShortDay"), compileRegex: true);
        }

        public PrintManager CreatePrintManager()
        {
            return new PrintManager(_printConfig.GetElements("Print/Prints/Print"));
        }

        private Dictionary<PrintType, string> CreatePostTypeDirectories()
        {
            Dictionary<PrintType, string> postTypeDirectories = new Dictionary<PrintType, string>();
            postTypeDirectories.Add(PrintType.Print, _printList1Config.GetExplicit("FindPrints/PostTypeDirectories/Print"));
            postTypeDirectories.Add(PrintType.Book, _printList1Config.GetExplicit("FindPrints/PostTypeDirectories/Book"));
            postTypeDirectories.Add(PrintType.Comics, _printList1Config.GetExplicit("FindPrints/PostTypeDirectories/Comics"));
            postTypeDirectories.Add(PrintType.UnknowEBook, _printList1Config.GetExplicit("FindPrints/PostTypeDirectories/UnknowEBook"));
            postTypeDirectories.Add(PrintType.Unknow, _printList1Config.GetExplicit("FindPrints/PostTypeDirectories/UnknowPostType"));
            return postTypeDirectories;
        }

        public static FindPrintManager Create(XElement xe = null, NamedValues<ZValue> parameters = null, int version = 0)
        {
            FindPrintManagerCreator createFindPrintManager = new FindPrintManagerCreator();
            createFindPrintManager.Init(xe);
            if (version != 0)
                createFindPrintManager.Version = version;
            createFindPrintManager.SetParameters(parameters);
            return createFindPrintManager.Create();
        }
    }
}
