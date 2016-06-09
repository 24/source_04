using System.Collections.Generic;
using System.Xml.Linq;
using pb;
using pb.Data.Xml;

namespace Download.Print
{
    public class PrintDateNumberReference
    {
        public Date date;
        public int number;
    }

    public class PrintDateNumberReferences
    {
        private SortedList<Date, PrintDateNumberReference> _refByDate = null;
        private SortedList<int, PrintDateNumberReference> _refByNumber = null;

        public PrintDateNumberReferences(IEnumerable<XElement> xelements)
        {
            _refByDate = new SortedList<Date, PrintDateNumberReference>();
            _refByNumber = new SortedList<int, PrintDateNumberReference>();
            foreach (XElement xelement in xelements)
            {
                string dateRef = xelement.zAttribValue("date");
                string numberRef = xelement.zAttribValue("number");
                if (dateRef == null || numberRef == null)
                    throw new PBException("error missing date or number in DateNumberReference");
                PrintDateNumberReference dateNumberReference = new PrintDateNumberReference { date = Date.Parse(dateRef), number = int.Parse(numberRef) };
                _refByDate.Add(dateNumberReference.date, dateNumberReference);
                _refByNumber.Add(dateNumberReference.number, dateNumberReference);
            }

        }

        public int Count { get { return _refByDate.Count; } }

        public PrintDateNumberReference GetReference(Date date)
        {
            for (int i = _refByDate.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                    return _refByDate.Values[0];
                if (_refByDate.Keys[i] <= date)
                    return _refByDate.Values[i];
            }
            return null;
        }

        public PrintDateNumberReference GetReference(int number)
        {
            for (int i = _refByNumber.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                    return _refByNumber.Values[0];
                if (_refByNumber.Keys[i] <= number)
                    return _refByNumber.Values[i];
            }
            return null;
        }
    }
}
