using Ghostscript.NET.Rasterizer;
using pb.IO;
using pb.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace pb.Data.Pdf
{
    public static class PdfScript
    {
        public static IEnumerable<Image> PdfToImages(string file, int xDpi = 96, int yDpi = 96, string range = null)
        {
            using (var rasterizer = new GhostscriptRasterizer())
            {
                using (FileStream fs = zFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    rasterizer.Open(fs);

                    if (range != null)
                    {
                        foreach (int page in zstr.EnumRange(range))
                            yield return rasterizer.GetPage(xDpi, yDpi, page);
                    }
                    else
                    {
                        for (int page = 1; page <= rasterizer.PageCount; page++)
                            yield return rasterizer.GetPage(xDpi, yDpi, page);
                    }
                }
            }
        }
    }
}
