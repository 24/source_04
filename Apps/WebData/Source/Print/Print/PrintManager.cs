using System;
using System.Collections.Generic;
using System.Xml.Linq;
using pb.Data.Xml;

//namespace Print
namespace Download.Print
{
    public class PrintManager
    {
        private Dictionary<string, Print> _prints = null;

        public PrintManager(IEnumerable<XElement> xelements)
        {
            init(xelements);
        }

        private void init(IEnumerable<XElement> xelements)
        {
            _prints = new Dictionary<string, Print>();
            foreach (XElement xe in xelements)
            {
                Print print = null;
                switch (xe.zXPathValue("Class"))
                {
                    case "LeMonde":
                        //print = new PrintLeMonde(xe, _directory, _regexModels);
                        print = new PrintLeMonde(xe);
                        break;
                    case "LeParisien":
                        //print = new PrintLeParisien(xe, _directory, _regexModels);
                        print = new PrintLeParisien(xe);
                        break;
                    case "LExpress":
                        //print = new PrintLExpress(xe, _directory, _regexModels);
                        print = new PrintLExpress(xe);
                        break;
                    case "LeVifExpress":
                        //print = new PrintLeVifExpress(xe, _directory, _regexModels);
                        print = new PrintLeVifExpress(xe);
                        break;
                    default:
                        //print = new Print(xe, _directory, _regexModels);
                        print = new Print(xe);
                        break;
                }
                _prints.Add(print.Name, print);
            }
        }

        public Dictionary<string, Print> Prints { get { return _prints; } }
        public Print this[string name] { get { if (_prints.ContainsKey(name)) return _prints[name]; else return null; } }
    }
}
