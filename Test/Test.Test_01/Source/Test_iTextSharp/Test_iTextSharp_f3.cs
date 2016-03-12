using iTextSharp.text.pdf;
using iTextSharp.text;
using pb;

namespace Test.Test_iTextSharp
{
    public static class Test_iTextSharp_f3
    {
        public static void Test_iTextSharp_01()
        {
            Trace.WriteLine("Test_iTextSharp_01");
        }

        public static void Test_ReadPdf_01(string file)
        {
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(file);
                Trace.WriteLine("read pdf                          : \"{0}\"", file);
                Trace.WriteLine("number of pages                   : {0}", reader.NumberOfPages);
                Rectangle mediabox = reader.GetPageSize(1);
                Trace.WriteLine("size of page 1                    : [ {0}, {1}, {2}, {3} ]", mediabox.Left, mediabox.Bottom, mediabox.Right, mediabox.Top);
                Trace.WriteLine("rotation of page 1                : {0}", reader.GetPageRotation(1));
                Trace.WriteLine("page size with rotation of page 1 : {0}", reader.GetPageSizeWithRotation(1));
                Trace.WriteLine("file length                       : {0}", reader.FileLength);
                Trace.WriteLine("is rebuilt ?                      : {0}", reader.IsRebuilt());
                Trace.WriteLine("is encrypted ?                    : {0}", reader.IsEncrypted());
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
