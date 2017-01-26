using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using pb.IO;

namespace pb.Pdf
{
    public class PdfWriter : IDisposable
    {
        private FileStream gfs = null;
        private Writer gw = null;
        private PdfReader gpr = null;
        private bool gUpdatePdf = false;
        private int gXrefPosition = 0;
        private int gUpdatePosition = 0;
        private PdfValueObject gTrailer = new PdfValueObject();
        private Dictionary<int, PdfXref> gXref = new Dictionary<int, PdfXref>();

        public PdfWriter(string file, FileMode mode = FileMode.CreateNew, FileShare share = FileShare.Read, Encoding encoding = null, bool updatePdf = false)
        {
            gUpdatePdf = updatePdf;
            if (updatePdf)
                mode = FileMode.Open;
            //gfs = new FileStream(file, mode, FileAccess.ReadWrite, share);
            gfs = zFile.Open(file, mode, FileAccess.ReadWrite, share);
            gw = new Writer(gfs, encoding);
            if (updatePdf)
            {
                //gpr = new PdfReader(file);
                gpr = new PdfReader(gfs, encoding);
                gTrailer = gpr.Trailer;
                gXref = gpr.Xref;
                gUpdatePosition = gpr.XrefPosition;
            }
            else
                WriteHeader();
        }

        public void Dispose()
        {
            Close();
        }

        public PdfReader reader { get { return gpr; } set { gpr = value; } }

        public PdfValueObject Trailer { get { return gTrailer; } }

        public void Close()
        {
            if (gw != null)
            {
                WriteFooter();
                gfs.Close();
            }
            gfs = null;
            gw = null;
        }

        private void WriteHeader()
        {
            gw.Write("%PDF-1.3\n");
        }

        private void WriteFooter()
        {
            WriteXref();
            WriteTrailer();
            WriteXrefPosition();
            gw.Write("%%EOF\n");
        }

        private void WriteXref()
        {
            // xref
            // 0 5274
            // 0000000000 65535 f 
            // 0000006878 00000 n 
            // 0010645614 00000 n 

            gXrefPosition = (int)gw.Position;
            gw.Write("xref\n");

            int headerid = 0;
            int lastid = 0;
            List<PdfXref> xrefList = new List<PdfXref>();
            xrefList.Add(new PdfXref { objectId = 0, generationNumber = 65535, filePosition = 0 });
            foreach (PdfXref xref in from xref in gXref orderby xref.Key select xref.Value)
            {
                if (xref.objectId == lastid + 1)
                {
                    xrefList.Add(xref);
                    lastid++;
                }
                else
                {
                    gw.Write("{0} {1}\n", headerid, xrefList.Count);
                    foreach (PdfXref xref2 in xrefList)
                    {
                        char state = 'n';
                        if (xref2.objectId == 0) state = 'f';
                        gw.Write("{0:0000000000} {1:00000} {2} \n", xref2.filePosition, xref2.generationNumber, state);
                    }
                    xrefList.Clear();
                    xrefList.Add(xref);
                    lastid = headerid = xref.objectId;
                }
            }
            gw.Write("{0} {1}\n", headerid, xrefList.Count);
            foreach (PdfXref xref2 in xrefList)
            {
                char state = 'n';
                if (xref2.objectId == 0) state = 'f';
                gw.Write("{0:0000000000} {1:00000} {2} \n", xref2.filePosition, xref2.generationNumber, state);
            }
        }

        private void WriteTrailer()
        {
            // trailer
            // <<
            // /Size 5274
            // /Root 5273 0 R
            // /Info 5272 0 R
            // >>

            gw.Write("trailer\n");
            gw.Write("<<\n");
            gw.Write("/Size {0}\n", gXref.Count);
            foreach (PdfNValue value in gTrailer.objectValues.Values)
            {
                gw.Write("/{0} ", value.name);
                WriteValue(value.value);
                gw.Write("\n");
            }
            gw.Write(">>\n");
        }

        private void WriteXrefPosition()
        {
            // startxref
            // 10646302
            gw.Write("startxref\n");
            gw.Write("{0}\n", gXrefPosition);
        }

        public void WriteObject(IPdfObject obj, string trailerName = null)
        {
            if (gXref.ContainsKey(obj.id)) throw new PBException("error object already in file id {0}", obj.id);
            gXref.Add(obj.id, new PdfXref { objectId = obj.id, generationNumber = obj.generationNumber, filePosition = gw.Position });
            if (trailerName != null)
                gTrailer[trailerName] = new PdfNValue { name = trailerName, value = new PdfValueObjectRef { valueObjectId = obj.id, valueObjectGenerationNumber = obj.generationNumber } };
            _WriteObject(obj);
            //gw.Write("{0} {1} obj\n", obj.id, obj.generationNumber);
            //WriteValue(obj.value);
            //gw.Write("\n");
            //if (obj.stream != null)
            //{
            //    gw.Write("stream\n");
            //    gw.Write(obj.stream);
            //    gw.Write("\n");
            //    gw.Write("endstream\n");
            //}
            //gw.Write("endobj\n");
        }

        public void UpdateObject(IPdfObject obj, string trailerName = null)
        {
            if (!gUpdatePdf) throw new PBException("error object update is not activate \"{0}\"", gw.File);
            if (!gXref.ContainsKey(obj.id)) throw new PBException("error update object {0}, object dont exist", obj.id);
            gw.Seek(gUpdatePosition);
            gXref[obj.id] = new PdfXref { objectId = obj.id, generationNumber = obj.generationNumber, filePosition = gUpdatePosition };
            if (trailerName != null)
                gTrailer[trailerName] = new PdfNValue { name = trailerName, value = new PdfValueObjectRef { valueObjectId = obj.id, valueObjectGenerationNumber = obj.generationNumber } };
            _WriteObject(obj);
            gUpdatePosition = (int)gw.Position;
        }

        private void _WriteObject(IPdfObject obj)
        {
            gw.Write("{0} {1} obj\n", obj.id, obj.generationNumber);
            WriteValue(obj.value);
            gw.Write("\n");
            if (obj.stream != null)
            {
                gw.Write("stream\n");
                gw.Write(obj.stream);
                gw.Write("\n");
                gw.Write("endstream\n");
            }
            gw.Write("endobj\n");
        }

        public void WriteObjectWithChilds(IPdfObject obj, string trailerName = null)
        {
            WriteObject(obj, trailerName);
            WriteChildsObject(obj.value);
        }

        public void WriteChildsObject(IPdfValue value)
        {
            if (value.isObjectRef())
            {
                WriteObjectWithChilds(gpr.ReadObject(value.valueObjectId));
            }
            else if (value.isObject())
            {
                foreach (PdfNValue nvalue in value.objectValues.Values)
                {
                    WriteChildsObject(nvalue.value);
                }
            }
            else if (value.isArray())
            {
                foreach (IPdfValue value2 in value.arrayValues)
                {
                    WriteChildsObject(value2);
                }
            }
        }

        private void WriteValue(IPdfValue value)
        {
            if (value.isNull())
            {
                // /OpenAction [3 0 R /FitH null]
                gw.Write("null");
            }
            else if (value.isBool())
            {
                // /OP true /Type /ExtGState
                gw.Write(value.valueBool ? "true" : "false");
            }
            else if (value.isInt())
            {
                // /MediaBox [0 0 907.09 1292.59]
                gw.Write(value.valueInt.ToString());
            }
            else if (value.isDouble())
            {
                // /MediaBox [0 0 907.09 1292.59]
                gw.Write(value.valueDouble.ToString("0.00"));
            }
            else if (value.isName())
            {
                // /PageLayout /OneColumn
                gw.Write("/{0}", value.valueName);
            }
            else if (value.isString())
            {
                // /Producer (FPDF 1.6)
                gw.Write("({0})", value.valueString);
            }
            else if (value.isDateTime())
            {
                // /CreationDate (D:20121205105056)
                gw.Write("(D:{0:yyyyMMddHHmmss})", value.valueDateTime);
            }
            else if (value.isObjectRef())
            {
                // /Pages 1 0 R
                gw.Write("{0} {1} R", value.valueObjectId, value.valueObjectGenerationNumber);
            }
            else if (value.isObject())
            {
                // <<
                // /Type /Page
                // /Parent 1 0 R
                // /MediaBox [0 0 907.09 1292.59]
                // /Resources 2 0 R
                // /Contents 4 0 R
                // >>
                gw.Write("<<\n");
                foreach (PdfNValue nvalue in value.objectValues.Values)
                {
                    gw.Write("/{0} ", nvalue.name);
                    WriteValue(nvalue.value);
                    gw.Write("\n");
                }
                gw.Write(">>");
            }
            else if (value.isArray())
            {
                // /OpenAction [3 0 R /FitH null]
                gw.Write("[");
                bool first = true;
                foreach (IPdfValue value2 in value.arrayValues)
                {
                    if (!first)
                        gw.Write(" ");
                    WriteValue(value2);
                    first = false;
                }
                gw.Write("]");
            }
        }
    }
}
