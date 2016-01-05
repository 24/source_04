using pb;
using pb.Data;
using pb.Data.Xml;
using pb.Text;
using Print;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Download.Print
{
    public class CreateFindPrintManager
    {
        // parameters
        private int _version = 3;
        private bool _dailyPrintManager = false;
        private int _gapDayBefore = 0;
        private int _gapDayAfter = 0;

        private XElement _xeConfig = null;
        private NamedValues<ZValue> _parameters = null;

        private XmlConfig _config = null;
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
            _version = xe.zXPathValue("Version").zTryParseAs(_version);
            _dailyPrintManager = xe.zXPathValue("DailyPrintManager").zTryParseAs(false);
            _gapDayBefore = xe.zXPathValue("GapDayBefore").zTryParseAs(0);
            _gapDayAfter = xe.zXPathValue("GapDayAfter").zTryParseAs(0);
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
            switch (parameter.Key.ToLower())
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
            }
        }

        public FindPrintManager Create()
        {
            if (_version < 3 || _version > 6)
                throw new PBException("bad version {0}", _version);

            _config = XmlConfig.CurrentConfig;
            _printConfig = _config.GetConfig("PrintConfig");
            _printList1Config = _config.GetConfig("PrintList1Config");
            _printList2Config = _config.GetConfig("PrintList2Config");

            //_printTitleManager = CreatePrintTitleManager.Create(_version, _gapDayBefore, _gapDayAfter);
            CreatePrintTitleManager createPrintTitleManager = new CreatePrintTitleManager();
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

            if (_version >= 6)
                findPrintManager.Version = 2;

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
    }
}
