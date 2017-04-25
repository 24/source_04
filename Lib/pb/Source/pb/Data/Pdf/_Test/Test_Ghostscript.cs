using System.Drawing.Imaging;
using Ghostscript.NET.Rasterizer;
using pb.IO;
using System.IO;

namespace pb.Data.Pdf.Test
{
    public static class Test_Ghostscript
    {
        // from https://ghostscriptnet.codeplex.com/SourceControl/latest#Ghostscript.NET/Ghostscript.NET.Samples/Samples/RasterizerSample1.cs
        public static void Test_PdfToImages(string file, string outputDirectory, int xDpi = 96, int yDpi = 96)
        {
            // GhostscriptRasterizer allows you to rasterize pdf and postscript files into the 
            // memory. If you want Ghostscript to store files on the disk use GhostscriptProcessor
            // or one of the GhostscriptDevices (GhostscriptPngDevice, GhostscriptJpgDevice).

            //string inputPdfPath = @"E:\gss_test\test.pdf";
            //string outputPath = @"E:\gss_test\output\";

            zdir.CreateDirectory(outputDirectory);
            Trace.WriteLine($"convert pdf to images \"{file}\"");
            using (var rasterizer = new GhostscriptRasterizer())
            {
                using (FileStream fs = zFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    //rasterizer.Open(file);
                    rasterizer.Open(fs);

                    for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                    {
                        var imageFile = zPath.Combine(outputDirectory, $"page-{pageNumber:000}.jpeg");

                        var img = rasterizer.GetPage(xDpi, yDpi, pageNumber);
                        //img.Save(imageFile, ImageFormat.Png);
                        img.Save(imageFile, ImageFormat.Jpeg);

                        Trace.WriteLine(imageFile);
                    }
                }
            }
        }

        //public static void Sample2()
        //{
        //    int desired_x_dpi = 96;
        //    int desired_y_dpi = 96;

        //    string inputPdfPath = @"E:\gss_test\test.pdf";
        //    string outputPath = @"E:\gss_test\output\";

        //    var output = new DelegateStdIOHandler(
        //        stdOut: Console.WriteLine,
        //        stdErr: Console.WriteLine
        //        );

        //    using (var rasterizer = new GhostscriptRasterizer(output))
        //    {
        //        rasterizer.Open(inputPdfPath);

        //        for (var pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
        //        {
        //            var pageFilePath = Path.Combine(outputPath, string.Format("Page-{0}.png", pageNumber));

        //            var img = rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber);
        //            img.Save(pageFilePath, ImageFormat.Png);

        //            Console.WriteLine(pageFilePath);
        //        }
        //    }
        //}
    }
}

