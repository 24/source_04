using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using pb.IO;
using pb.IO.zlib;

namespace pb.Pdf
{
    public class PdfXrefHeader
    {
        public long filePosition;
        public int objectId;
        public int objectNb;
    }

    public class PdfXref
    {
        public int objectId;
        public long filePosition;
        public int generationNumber;
    }

    public interface IPdfObject
    {
        int id { get; }
        int generationNumber { get; }
        IPdfValue value { get; }
        string source { get; }
        byte[] stream { get; set; }
        byte[] deflatedStream { get; }
    }

    // 'PB_Pdf.PdfObjectDictionary' does not implement interface member 'PB_Pdf.IPdfObject.deflatedStream.set'
    public class PdfObjectDictionary : IPdfObject
    {
        private int gid;
        private int gGenerationNumber;
        private IPdfValue gValue = null;
        private string gSource = null;
        private byte[] gStream = null;
        private byte[] gDeflatedStream = null;

        public PdfObjectDictionary(int id, int generationNumber)
        {
            gid = id;
            gGenerationNumber = generationNumber;
        }

        public int id { get { return gid; } }
        public int generationNumber { get { return gGenerationNumber; } }
        public IPdfValue value { get { return gValue; } set { gValue = value; } }
        public string source { get { return gSource; } set { gSource = value; } }
        public byte[] stream { get { return gStream; } set { gStream = value; } }
        public byte[] deflatedStream { get { return DeflateStream(); } }
        private byte[] DeflateStream()
        {
            if (gDeflatedStream == null && gStream != null)
            {
                if (gValue.isObject() && gValue["Filter"] != null && gValue["Filter"].value.valueName == "FlateDecode")
                    gDeflatedStream = PdfReader.FlateDecode(gStream);
                else
                    gDeflatedStream = gStream;
            }
            return gDeflatedStream;
        }
    }

    public class PdfNValue
    {
        public string name;
        public IPdfValue value;
        public override string ToString()
        {
            return string.Format("{0} = {1}", name, value);
        }
    }

    public interface IPdfValue
    {
        bool valueBool { get; set; }
        int valueInt { get; set; }
        double valueDouble { get; set; }
        string valueName { get; }
        string valueString { get; set; }
        DateTime valueDateTime { get; set; }
        int valueObjectId { get; set; }
        int valueObjectGenerationNumber { get; set; }
        IPdfValue this[int index] { get; set; }
        PdfNValue this[string key] { get; set; }
        IPdfValue[] arrayValues { get; set; }
        Dictionary<string, PdfNValue> objectValues { get; set; }
        bool isNull();
        bool isBool();
        bool isInt();
        bool isDouble();
        bool isName();
        bool isString();
        bool isDateTime();
        bool isObjectRef();
        bool isObject();
        bool isArray();
    }

    public class PdfValue
    {
        public virtual bool valueBool { get { throw new Exception("error this value is not a PdfValueBool"); } set { throw new Exception("error this value is not a PdfValueBool"); } }
        public virtual int valueInt { get { throw new Exception("error this value is not a PdfValueInt"); } set { throw new Exception("error this value is not a PdfValueInt"); } }
        public virtual double valueDouble { get { throw new Exception("error this value is not a PdfValueDouble"); } set { throw new Exception("error this value is not a PdfValueDouble"); } }
        public virtual string valueName { get { throw new Exception("error this value is not a PdfValueName"); } set { throw new Exception("error this value is not a PdfValueName"); } }
        public virtual string valueString { get { throw new Exception("error this value is not a PdfValueString"); } set { throw new Exception("error this value is not a PdfValueString"); } }
        public virtual DateTime valueDateTime { get { throw new Exception("error this value is not a PdfValueDateTime"); } set { throw new Exception("error this value is not a PdfValueDateTime"); } }
        public virtual int valueObjectId { get { throw new Exception("error this value is not a PdfValueObjectRef"); } set { throw new Exception("error this value is not a PdfValueObjectRef"); } }
        public virtual int valueObjectGenerationNumber { get { throw new Exception("error this value is not a PdfValueObjectRef"); } set { throw new Exception("error this value is not a PdfValueObjectRef"); } }
        public virtual IPdfValue this[int index] { get { throw new Exception("error this value is not a PdfValueArray"); } set { throw new Exception("error this value is not a PdfValueArray"); } }
        public virtual PdfNValue this[string key] { get { throw new Exception("error this value is not a PdfValueObject"); } set { throw new Exception("error this value is not a PdfValueObject"); } }
        public virtual IPdfValue[] arrayValues { get { throw new Exception("error this value is not a PdfValueArray"); } set { throw new Exception("error this value is not a PdfValueArray"); } }
        public virtual Dictionary<string, PdfNValue> objectValues { get { throw new Exception("error this value is not a PdfValueObject"); } set { throw new Exception("error this value is not a PdfValueObject"); } }
        public virtual bool isNull() { return false; }
        public virtual bool isBool() { return false; }
        public virtual bool isInt() { return false; }
        public virtual bool isDouble() { return false; }
        public virtual bool isName() { return false; }
        public virtual bool isString() { return false; }
        public virtual bool isDateTime() { return false; }
        public virtual bool isObjectRef() { return false; }
        public virtual bool isObject() { return false; }
        public virtual bool isArray() { return false; }
    }

    // /OpenAction [3 0 R /FitH null]
    public class PdfValueNull : PdfValue, IPdfValue
    {
        public override bool isNull() { return true; }
        public override string ToString()
        {
            return "null";
        }
    }

    // /OP true /Type /ExtGState
    public class PdfValueBool : PdfValue, IPdfValue
    {
        private bool gValue;
        public override bool valueBool { get { return gValue; } set { gValue = value; } }
        public override bool isBool() { return true; }
        public override string ToString()
        {
            return string.Format("bool({0})", gValue ? "true" : "false");
        }
    }

    // /MediaBox [0 0 907.09 1292.59]
    public class PdfValueInt : PdfValue, IPdfValue
    {
        private int gValue;
        public override int valueInt { get { return gValue; } set { gValue = value; } }
        public override bool isInt() { return true; }
        public override string ToString()
        {
            return string.Format("int({0})", gValue);
        }
    }

    // /MediaBox [0 0 907.09 1292.59]
    public class PdfValueDouble : PdfValue, IPdfValue
    {
        private double gValue;
        public override double valueDouble { get { return gValue; } set { gValue = value; } }
        public override bool isDouble() { return true; }
        public override string ToString()
        {
            return string.Format("double({0})", gValue);
        }
    }

    // /PageLayout /OneColumn
    // /OpenAction [3 0 R /FitH null]
    public class PdfValueName : PdfValue, IPdfValue
    {
        private string gValue;
        public override string valueName { get { return gValue; } set { gValue = value; } }
        public override bool isName() { return true; }
        public override string ToString()
        {
            return string.Format("name(\"{0}\")", gValue);
        }
    }

    // /Producer (FPDF 1.6)
    public class PdfValueString : PdfValue, IPdfValue
    {
        private string gValue;
        public override string valueString { get { return gValue; } set { gValue = value; } }
        public override bool isString() { return true; }
        public override string ToString()
        {
            return string.Format("string(\"{0}\")", gValue);
        }
    }

    // /CreationDate (D:20121205105056)
    public class PdfValueDateTime : PdfValue, IPdfValue
    {
        private DateTime gValue;
        public override DateTime valueDateTime { get { return gValue; } set { gValue = value; } }
        public override bool isDateTime() { return true; }
        public override string ToString()
        {
            return string.Format("datetime(\"{0}\")", gValue);
        }
    }

    // /Pages 1 0 R
    // /OpenAction [3 0 R /FitH null]
    public class PdfValueObjectRef : PdfValue, IPdfValue
    {
        private int gId;
        private int gGenerationNumber;
        public override int valueObjectId { get { return gId; } set { gId = value; } }
        public override int valueObjectGenerationNumber { get { return gGenerationNumber; } set { gGenerationNumber = value; } }
        public override bool isObjectRef() { return true; }
        public override string ToString()
        {
            return string.Format("objectRef({0})", gId);
        }
    }

    // <<
    // /Type /Page
    // /Parent 1 0 R
    // /MediaBox [0 0 907.09 1292.59]
    // /Resources 2 0 R
    // /Contents 4 0 R
    // >>
    public class PdfValueObject : PdfValue, IPdfValue
    {
        private Dictionary<string, PdfNValue> gValues = new Dictionary<string, PdfNValue>();
        public override PdfNValue this[string key] { get { if (gValues.ContainsKey(key)) return gValues[key]; else return null; } set { if (!gValues.ContainsKey(key)) gValues.Add(key, value); else gValues[key] = value; } }
        public override bool isObject() { return true; }
        //public bool ContainsKey(string key) { return gValues.ContainsKey(key); }
        //public override Dictionary<string, PdfNValue>.ValueCollection objectValues { get { return gValues.Values; } }
        public override Dictionary<string, PdfNValue> objectValues { get { return gValues; } set { gValues = value; } }
        public override string ToString()
        {
            return string.Format("object({0})", gValues.Values.zToStringValues());
        }
    }

    // /OpenAction [3 0 R /FitH null]
    // /MediaBox [0 0 907.09 1292.59]
    public class PdfValueArray : PdfValue, IPdfValue
    {
        private IPdfValue[] gValues = null;
        public override IPdfValue this[int index] { get { return gValues[index]; } }
        public override IPdfValue[] arrayValues { get { return gValues; } set { gValues = value; } }
        public override bool isArray() { return true; }
        public override string ToString()
        {
            return string.Format("array({0})", gValues.zToStringValues());
        }
    }

    public class PdfReader : IDisposable
    {
        private string gFile = null;
        private PdfStreamReader gpsr = null;
        //private bool gStreamCreated = false;
        private PdfObjectReader gObjectReader = null;
        private const int pdf_startxref_max_length = 21;
        private const int pdf_eof_marker_length = 6;
        private const int pdf_xref_line_length = 20;
        private int gXrefPosition;
        private int gTrailerPosition;
        private int gObjectsNumber;
        private List<PdfXrefHeader> gXrefHeaders = null;
        private PdfValueObject gTrailer = null;
        private Dictionary<int, PdfXref> gXref = null;
        //private IPdfObject gRoot = null;

        public PdfReader(string file, Encoding encoding = null)
        {
            gFile = file;
            gpsr = new PdfStreamReader(gFile, encoding: encoding);
            Init();
        }

        public PdfReader(Stream stream, Encoding encoding = null)
        {
            gpsr = new PdfStreamReader(stream, encoding);
            Init();
        }

        public void Dispose()
        {
            Close();
        }

        public bool KeepObjectSource { get { return gpsr.KeepObjectSource; } set { gpsr.KeepObjectSource = value; } }
        public int XrefPosition { get { return gXrefPosition; } }
        public int TrailerPosition { get { return gTrailerPosition; } }
        public int ObjectsNumber { get { return gObjectsNumber; } }
        public List<PdfXrefHeader> XrefHeaders { get { return gXrefHeaders; } }
        public PdfValueObject Trailer { get { return gTrailer; } }
        public Dictionary<int, PdfXref> Xref { get { return gXref; } }
        public TraceDelegate Trace { get { return gpsr.Trace; } set { gpsr.Trace = value; } }

        public void Close()
        {
            if (gpsr != null)
            {
                gpsr.Close();
                gpsr = null;
            }
        }

        private void Init()
        {
            ReadXrefPosition();
            ReadXrefHeaders();
            ReadXref();
            gObjectReader = new PdfObjectReader(gpsr, gXref);
            gTrailer = gObjectReader.ReadTrailer(gTrailerPosition);
        }

        public void ReadXrefPosition()
        {
            gpsr.Seek(gpsr.Length - pdf_startxref_max_length - pdf_eof_marker_length);
            //string s = gpsr.ReadLine();
            gpsr.ReadBytesUntil("\nstartxref\n");
            //if (s != "startxref") throw new PBException("error \"startxref\" not found at position {0}", gpsr.Length - pdf_startxref_max_length - pdf_eof_marker_length);
            string s = gpsr.ReadLine();
            gXrefPosition = int.Parse(s);
        }

        public void ReadXrefHeaders()
        {
            gpsr.Seek(gXrefPosition);
            int position = gXrefPosition;
            string s = gpsr.ReadLine();
            position += s.Length + 1;
            if (s != "xref") throw new PBException("error \"xref\" not found at position {0} \"{1}\"", gXrefPosition, s);
            gXrefHeaders = new List<PdfXrefHeader>();
            Regex rg = new Regex("^([0-9]+) ([0-9]+)$", RegexOptions.Compiled);
            int objectNb = 0;
            s = gpsr.ReadLine(); // object_id number_of _objects
            while (s != "trailer")
            {
                position += s.Length + 1;
                Match match = rg.Match(s);
                if (!match.Success) throw new PBException("error reading pdf xref headers : \"{0}\"", s);
                int id = int.Parse(match.Groups[1].Value);
                int nb = int.Parse(match.Groups[2].Value);
                gXrefHeaders.Add(new PdfXrefHeader { filePosition = position, objectId = id, objectNb = nb });
                position += nb * pdf_xref_line_length;
                gpsr.Seek(position);
                s = gpsr.ReadLine(); // object_id number_of _objects
                objectNb += nb;
            }
            gObjectsNumber = objectNb;
            gTrailerPosition = position;
        }

        public void ReadXref()
        {
            Regex rg = new Regex("^([0-9]{10}) ([0-9]{5}) ([nf]) $", RegexOptions.Compiled);
            gXref = new Dictionary<int, PdfXref>();
            foreach (PdfXrefHeader header in gXrefHeaders)
            {
                gpsr.Seek(header.filePosition);
                int id = header.objectId;
                for (int i = 0; i < header.objectNb; i++, id++)
                {
                    // 0000000000 65535 f 
                    // 0000006878 00000 n 
                    string s = gpsr.ReadLine();
                    Match match = rg.Match(s);
                    if (!match.Success) throw new PBException("error reading pdf xref : \"{0}\"", s);
                    if (match.Groups[3].Value == "f") continue;
                    gXref.Add(id, new PdfXref { objectId = id, filePosition = int.Parse(match.Groups[1].Value), generationNumber = int.Parse(match.Groups[2].Value) });
                }
            }
        }

        public IPdfObject ReadObject(int id)
        {
            return gObjectReader.ReadObject(id);
        }

        public static byte[] FlateDecode(byte[] inp, bool strict = true)
        {
            MemoryStream stream = new MemoryStream(inp);
            ZInflaterInputStream zip = new ZInflaterInputStream(stream);
            MemoryStream outp = new MemoryStream();
            byte[] b = new byte[strict ? 4092 : 1];
            try
            {
                int n;
                while ((n = zip.Read(b, 0, b.Length)) > 0)
                {
                    outp.Write(b, 0, n);
                }
                zip.Close();
                outp.Close();
                return outp.ToArray();
            }
            catch
            {
                if (strict)
                    return null;
                return outp.ToArray();
            }
        }
    }

    public class PdfObjectReader
    {
        private PdfStreamReader gpsr = null;
        private Dictionary<int, PdfXref> gXref = null;

        public PdfObjectReader(PdfStreamReader psr, Dictionary<int, PdfXref> xref = null)
        {
            gpsr = psr;
            gXref = xref;
        }

        public PdfObjectReader(PdfObjectReader or)
        {
            gpsr = new PdfStreamReader(or.gpsr);
            gXref = or.gXref;
        }

        public PdfValueObject ReadTrailer(long filePosition)
        {
            gpsr.StartReadObject("trailer");
            gpsr.Seek(filePosition);
            string s = gpsr.ReadLine();
            if (s != "trailer") throw new PBException("error reading {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
            s = gpsr.ReadLine();
            return (PdfValueObject)ReadPdfValue(ref s);
        }

        public IPdfObject ReadObject(int id)
        {
            // trailer
            // <<
            // /Size 5274
            // /Root 5273 0 R
            // /Info 5272 0 R
            // >>

            // 5273 0 obj
            // <<
            // /Type /Catalog
            // /Pages 1 0 R
            // /OpenAction [3 0 R /FitH null]
            // /PageLayout /OneColumn
            // >>
            // endobj

            // 3 0 obj
            // <</Type /Page
            // /Parent 1 0 R
            // /MediaBox [0 0 907.09 1292.59]
            // /Resources 2 0 R
            // /Contents 4 0 R>>
            // endobj

            if (!gXref.ContainsKey(id)) throw new PBException("error unknow object {0}", id);
            PdfXref xref = gXref[id];

            gpsr.StartReadObject(string.Format("object {0}", id));
            gpsr.Seek(xref.filePosition);
            // 5273 0 obj
            Regex rg_object_begin = new Regex("^([0-9]+) ([0-9]+) obj$", RegexOptions.Compiled);
            PdfObjectDictionary o = new PdfObjectDictionary(id, xref.generationNumber);
            string s = gpsr.ReadLine();
            Match match = rg_object_begin.Match(s);
            if (!match.Success || int.Parse(match.Groups[1].Value) != id || int.Parse(match.Groups[2].Value) != xref.generationNumber)
                throw new PBException("error reading {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
            s = gpsr.ReadLine();

            o.value = ReadPdfValue(ref s);

            if (s == "")
                s = gpsr.ReadLine();
            if (s == "stream")
            {
                int length = 0;
                PdfNValue lengthValue = null;
                if (o.value.isObject())
                    lengthValue = o.value["Length"];
                if (lengthValue == null) throw new PBException("error stream without /Length reading {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
                if (lengthValue.value.isInt())
                    length = lengthValue.value.valueInt;
                else if (lengthValue.value.isObjectRef())
                {
                    long position = gpsr.Position;
                    PdfObjectReader or = new PdfObjectReader(this);
                    IPdfObject lengthObject = or.ReadObject(lengthValue.value.valueObjectId);
                    gpsr.Seek(position);
                    if (lengthObject.value.isInt())
                        length = lengthObject.value.valueInt;
                }
                if (length != 0)
                    o.stream = gpsr.ReadStream(length);
                else
                    o.stream = gpsr.ReadStream();
                s = gpsr.ReadLine();
            }
            if (s != "endobj") throw new PBException("error endobj not found reading {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
            o.source = gpsr.GetObjectSource();
            return o;
        }

        public PdfNValue ReadPdfNValue(ref string s)
        {
            // /Name ...
            // /Pages 1 0 R
            // /PageLayout /OneColumn
            // /OpenAction [3 0 R /FitH null]
            // /MediaBox [0 0 907.09 1292.59]
            Regex rg1 = new Regex("^/([a-zA-Z][a-zA-Z0-9]*) +", RegexOptions.Compiled);
            Match match = rg1.Match(s);
            if (!match.Success) throw new PBException("error reading value {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
            string name = match.Groups[1].Value;
            s = s.Substring(match.Length);
            IPdfValue value = ReadPdfValue(ref s);
            return new PdfNValue { name = name, value = value };
        }

        public IPdfValue ReadPdfValue(ref string s)
        {
            // 1 0 R
            // /OneColumn
            // /Producer (FPDF 1.6)
            // /CreationDate (D:20121205105056)
            // [3 0 R /FitH null]
            // [0 0 907.09 1292.59]
            while (s.Length == 0)
                s = gpsr.ReadLine();
            Regex rg_value_name = new Regex(@"^/([a-zA-Z0-9_\-\+]+) *", RegexOptions.Compiled);
            Regex rg_value_string = new Regex(@"^\((?:(D):([0-9]{4})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2}))?([^)]*)\)", RegexOptions.Compiled);
            Regex rg_end_array = new Regex(@"^ *\] *", RegexOptions.Compiled);
            Regex rg_value_bool = new Regex(@"^ *(true|false) *", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex rg_value_object_ref = new Regex("^ *([0-9]+) +([0-9]+) +R *", RegexOptions.Compiled);
            Regex rg_value_double = new Regex(@"^ *(-?[0-9]+\.[0-9]+) *", RegexOptions.Compiled);
            Regex rg_value_int = new Regex(@"^ *(-?[0-9]+) *", RegexOptions.Compiled);
            Regex rg_value_null = new Regex(@"^ *null *", RegexOptions.Compiled);
            Match match;
            if (s[0] == '/')
            {
                match = rg_value_name.Match(s);
                if (!match.Success) throw new PBException("error reading value {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
                s = s.Substring(match.Length);
                return new PdfValueName { valueName = match.Groups[1].Value };
            }
            // /Producer (FPDF 1.6)
            if (s[0] == '(')
            {
                match = rg_value_string.Match(s);
                if (!match.Success) throw new PBException("error reading value {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
                s = s.Substring(match.Length);
                switch (match.Groups[1].Value)
                {
                    case "D":
                        return new PdfValueDateTime { valueDateTime = new DateTime(int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value),
                            int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value), int.Parse(match.Groups[7].Value)) };
                    case "":
                        return new PdfValueString { valueString = match.Groups[8].Value };
                }
            }
            if (s[0] == '[')
            {
                s = s.Substring(1);
                List<IPdfValue> values = new List<IPdfValue>();
                while (true)
                {
                    match = rg_end_array.Match(s);
                    if (match.Success)
                    {
                        s = s.Substring(match.Length);
                        break;
                    }
                    values.Add(ReadPdfValue(ref s));
                }
                return new PdfValueArray { arrayValues = values.ToArray() };
            }
            if (s.StartsWith("<<"))
            {
                PdfValueObject obj = new PdfValueObject();
                if (s.Length > 2)
                    s = s.Substring(2);
                else
                    s = gpsr.ReadLine();
                while (true)
                {
                    if (s.StartsWith(">>"))
                    {
                        s = s.Substring(2);
                        break;
                    }
                    PdfNValue value = ReadPdfNValue(ref s);
                    obj[value.name] = value;
                    if (s == "")
                        s = gpsr.ReadLine();
                }
                return obj;
            }
            match = rg_value_bool.Match(s);
            if (match.Success)
            {
                s = s.Substring(match.Length);
                return new PdfValueBool { valueBool = match.Groups[1].Value.ToLower() == "true" ? true : false };
            }
            match = rg_value_object_ref.Match(s);
            if (match.Success)
            {
                s = s.Substring(match.Length);
                return new PdfValueObjectRef { valueObjectId = int.Parse(match.Groups[1].Value), valueObjectGenerationNumber = int.Parse(match.Groups[2].Value) };
            }
            match = rg_value_double.Match(s);
            if (match.Success)
            {
                s = s.Substring(match.Length);
                return new PdfValueDouble { valueDouble = double.Parse(match.Groups[1].Value) };
            }
            match = rg_value_int.Match(s);
            if (match.Success)
            {
                s = s.Substring(match.Length);
                return new PdfValueInt { valueInt = int.Parse(match.Groups[1].Value) };
            }
            match = rg_value_null.Match(s);
            if (match.Success)
            {
                s = s.Substring(match.Length);
                return new PdfValueNull();
            }
            throw new PBException("error reading value {0} line {1} \"{2}\"", gpsr.ObjectName, gpsr.LineNumber, s);
        }
    }

    public class PdfStreamReader : IDisposable
    {
        private Reader gr = null;
        private bool gReaderCreated = false;
        private string gObjectName = null;
        private int gLineNumber = 0;
        private bool gKeepObjectSource = false;
        private StringBuilder gObjectSource = null;
        public TraceDelegate Trace = null;

        public PdfStreamReader(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read, Encoding encoding = null)
        {
            gr = new Reader(path, mode, access, share, encoding);
            gReaderCreated = true;
        }

        public PdfStreamReader(Stream s, Encoding encoding = null)
        {
            gr = new Reader(s, encoding);
            gReaderCreated = false;
        }

        public PdfStreamReader(Reader r)
        {
            gr = r;
            gReaderCreated = false;
        }

        public PdfStreamReader(PdfStreamReader psr)
        {
            gr = psr.gr;
            gReaderCreated = false;
        }

        public void Dispose()
        {
            Close();
        }

        public Reader fileReader { get { return gr; } }
        public string ObjectName { get { return gObjectName; } }
        public int LineNumber { get { return gLineNumber; } set { gLineNumber = value; } }
        public long Length { get { return gr.Length; } }
        public long Position { get { return gr.Position; } }
        public bool KeepObjectSource { get { return gKeepObjectSource; } set { gKeepObjectSource = value; } }

        public void Close()
        {
            if (gr != null && gReaderCreated)
                gr.Close();
            gr = null;
        }

        private void trace(string msg, params object[] prm)
        {
            if (Trace == null) return;
            if (prm.Length > 0)
                msg = string.Format(msg, prm);
            Trace(msg);
        }

        public void StartReadObject(string name)
        {
            gObjectName = name;
            gLineNumber = 0;
            if (gKeepObjectSource)
                gObjectSource = new StringBuilder();
        }

        public string GetObjectSource()
        {
            if (gObjectSource == null)
                return null;
            string s = gObjectSource.ToString();
            gObjectSource = null;
            return s;
        }

        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            return gr.Seek(offset, origin);
        }

        public string ReadLine()
        {
            gLineNumber++;
            string s = gr.ReadLine();
            if (gObjectSource != null)
            {
                gObjectSource.Append(s);
                gObjectSource.Append("\r\n");
            }
            return s;
        }

        public byte[] ReadStream()
        {
            trace("read stream without length for {0}", gObjectName);
            return gr.ReadBytesUntil("\nendstream\n");
            //string endStream = "\nendstream\n";
            //int endStreamIndex = 0;
            //List<byte> stream = new List<byte>();
            //List<byte> buf = new List<byte>();
            //while (true)
            //{
            //    byte b = gr.ReadByte();
            //    char c = (char)b;
            //    if (endStream[endStreamIndex] == c)
            //    {
            //        buf.Add(b);
            //        if (++endStreamIndex == endStream.Length) break;
            //    }
            //    else
            //    {
            //        if (endStreamIndex != 0)
            //        {
            //            stream.AddRange(buf);
            //            buf.Clear();
            //            endStreamIndex = 0;
            //        }
            //        stream.Add(b);
            //    }
            //}
            //return stream.ToArray();
        }

        public byte[] ReadStream(int count)
        {
            byte[] stream = gr.ReadBytes(count);
            string s = ReadLine();
            if (s != "") throw new PBException("error reading stream reading {0} line {1} \"{2}\"", gObjectName, gLineNumber, s);
            s = ReadLine();
            if (s != "endstream") throw new PBException("error endstream not found reading {0} line {1} \"{2}\"", gObjectName, gLineNumber, s);
            return stream;
        }

        public byte[] ReadBytesUntil(string mark)
        {
            return gr.ReadBytesUntil(mark);
        }
    }
}
