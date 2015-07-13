using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Test_iTextSharp
{
    class PdfTools
    {
        public static void ProcessContentPage(PdfReader reader, int page, Test_iTextSharp.ITextExtractionStrategy strategy)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);

            PdfDictionary pageDic = reader.GetPageN(page);
            PdfDictionary resourcesDic = pageDic.GetAsDict(PdfName.RESOURCES);

            Test_iTextSharp.PdfContentStreamProcessor processor = new Test_iTextSharp.PdfContentStreamProcessor(strategy);
            byte[] bytes = ContentByteUtils.GetContentBytesForPage(reader, page);
            processor.ProcessContent(bytes, resourcesDic);
        }
    }
}
