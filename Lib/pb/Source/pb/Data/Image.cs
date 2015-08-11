using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using pb.IO;

namespace pb.Data
{
    public static class zimg
    {
        //public static Bitmap ReadBitmap(string sourceFile)
        public static Bitmap LoadFromFile(string file)
        {
            if (!zFile.Exists(file))
                throw new PBException("error reading image file \"{0}\" does'nt exists", file);
            //Bitmap bitmap = new Bitmap(sourceFile);
            Image image;
            try
            {
                image = Image.FromFile(file);
            }
            catch (Exception exception)
            {
                if (exception is OutOfMemoryException)
                    throw new PBException("error unknow image file format \"{0}\"", file);
                else
                    throw;
            }
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(image, 0, 0);
            image.Dispose();
            return bitmap;
        }

        public static Image LoadFromUrl(string url)
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
    }

    public static partial class GlobalExtension
    {
        public static Image zResize(this Image image, int width = 0, int height = 0, bool preserveAspectRatio = true)
        {
            return zimg.ResizeImage(image, width, height, preserveAspectRatio);
        }
    }
}
