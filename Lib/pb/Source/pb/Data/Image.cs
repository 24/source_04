using System;
using System.Drawing;
using System.IO;
using System.Net;
using pb.IO;
using System.Drawing.Imaging;

namespace pb.Data
{
    // Image :
    //   PixelFormat PixelFormat : Format24bppRgb
    //   ImageFormat RawFormat   : [ImageFormat: b96b3cae-0728-11d3-9d7b-0000f81ef32e]
    //
    // Loading an image from a stream without keeping the stream open http://stackoverflow.com/questions/3845456/loading-an-image-from-a-stream-without-keeping-the-stream-open
    // Dépendances de constructeur bitmap et images https://support.microsoft.com/fr-fr/help/814675/bitmap-and-image-constructor-dependencies

    public static class zimg
    {
        // bad bad bad bad bad bad bad bad
        public static Bitmap LoadBitmapFromFile(string file)
        {
            if (!zFile.Exists(file))
                throw new PBException("error reading image file \"{0}\" does'nt exists", file);
            //Bitmap bitmap = new Bitmap(sourceFile);
            Image image;
            try
            {
                image = Image.FromFile(file);
                //image.RawFormat.Guid
            }
            catch (Exception exception)
            {
                if (exception is OutOfMemoryException)
                    throw new PBException("error unknow image file format \"{0}\"", file);
                else
                    throw;
            }
            //Bitmap bitmap = new Bitmap(image.Width, image.Height);
            //Graphics graphics = Graphics.FromImage(bitmap);
            //graphics.DrawImage(image, 0, 0);
            //image.Dispose();
            //return bitmap;
            Bitmap bitmap = Clone(image);
            image.Dispose();
            return bitmap;
        }

        // bad bad bad bad bad bad bad bad
        public static Bitmap Clone(Image image)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            using(Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(image, 0, 0);
            }
            return bitmap;
        }

        public static Image LoadImageFromFile(string file)
        {
            //using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream fs = zFile.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Image.FromStream(fs);
            }
        }

        public static Image LoadImageFromUrl(string url)
        {
            WebRequest wr = WebRequest.Create(url);
            WebResponse r = null;
            Stream s = null;
            try
            {
                r = wr.GetResponse();
                s = r.GetResponseStream();
                Image image = Image.FromStream(s);
                return image;
            }
            finally
            {
                if (s != null)
                    s.Close();
                if (r != null)
                    r.Close();
            }
        }

        public static void GetImageWidthHeight(string file, out int width, out int height)
        {
            using (Image image = Image.FromFile(file))
            {
                width = image.Width;
                height = image.Height;
            }
        }

        public static Image ResizeImage(Image image, int width = 0, int height = 0, bool preserveAspectRatio = true)
        {
            if (width == 0 && height == 0)
                return image;
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = 0;
                float percentHeight = 0;
                float percent = 0;
                if (width != 0)
                    percent = percentWidth = (float)width / (float)originalWidth;
                if (height != 0)
                    percent = percentHeight = (float)height / (float)originalHeight;
                if (percentHeight != 0 && percentWidth != 0)
                    percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public static string GetMimeType(Image image)
        {
            var guid = image.RawFormat.Guid;
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == guid)
                    return codec.MimeType;
            }
            return "image/unknown";
        }

        public static void CalculateImageWidthHeight(string file, int? width, int? height, out int calculatedWidth, out int calculatedHeight)
        {
            if (width == null || height == null)
            {
                Image image = LoadImageFromFile(file);
                if (width != null)
                {
                    calculatedWidth = (int)width;
                    calculatedHeight = (int)(image.Height * ((double)width / image.Width) + 0.5);
                }
                else if (height != null)
                {
                    calculatedWidth = (int)(image.Width * ((double)height / image.Height) + 0.5);
                    calculatedHeight = (int)height;
                }
                else
                {
                    calculatedWidth = image.Width;
                    calculatedHeight = image.Height;
                }
            }
            else
            {
                calculatedWidth = (int)width;
                calculatedHeight = (int)height;
            }
        }
    }

    public static partial class GlobalExtension
    {
        public static Image zResize(this Image image, int width = 0, int height = 0, bool preserveAspectRatio = true)
        {
            return zimg.ResizeImage(image, width, height, preserveAspectRatio);
        }
    }
}
