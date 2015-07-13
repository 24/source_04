using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace pb.old
{
    public enum ImageCompareResult
    {
        CompareOk,
        PixelMismatch,
        SizeMismatch
    }

    public class ImageComparePixelResult
    {
        public int nbPixel;
        public int nbDifferentPixel;
        public double differentPixelPercent;
        public double maxPixelDistance;
        public double averagePixelDistance;
    }

    public class img
    {
        private string gDataDirectory;

        public img(string dataDirectory)
        {
            gDataDirectory = dataDirectory;
        }

        // replace by zimg.LoadFromFile()
        //public static Bitmap ReadBitmap(string sourceFile)
        //{
        //    if (!File.Exists(sourceFile))
        //        throw new PBException("error reading bitmap file \"{0}\" does'nt exists", sourceFile);
        //    Bitmap bitmap = new Bitmap(sourceFile);
        //    Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height);
        //    Graphics graphics = Graphics.FromImage(bitmap2);
        //    graphics.DrawImage(bitmap, 0, 0);
        //    bitmap.Dispose();
        //    return bitmap2;
        //}

        public void ExtractBitmapCenter(string sourceFile, string destFile, int xCenter, int yCenter, int dxFromCenter, int dyFromCenter)
        {
            int x, y, dx, dy;
            img.CenterToTopLeft(xCenter, yCenter, dxFromCenter, dyFromCenter, out x, out y, out dx, out dy);
            ExtractBitmap(sourceFile, destFile, x, y, dx, dy);
        }

        public void ExtractBitmap(string sourceFile, string destFile, int x, int y, int dx, int dy)
        {
            string sourcePath = Path.Combine(this.gDataDirectory, sourceFile);
            string destPath = Path.Combine(this.gDataDirectory, destFile);
            //wr.Print("Extract bitmap from \"{0}\"", sourcePath);
            //wr.Print("  x = {0}, y = {1}, dx = {2}, dy = {3}", x, y, dx, dy);
            //wr.Print("  save bitmap to \"{0}\"", destPath);
            Bitmap bitmap2 = GetBitmap(img.ReadBitmap(sourcePath), x, y, dx, dy);
            cu2.CreateFileDirectory(destPath);
            bitmap2.Save(destPath);
        }

        public static Bitmap GetBitmap(Bitmap bitmap, int x, int y, int dx, int dy)
        {
            Rectangle srcRect = new Rectangle(x, y, dx, dy);
            Rectangle dstRect = new Rectangle(0, 0, dx, dy);
            Bitmap bitmap2 = new Bitmap(srcRect.Width, srcRect.Height);
            Graphics graphics = Graphics.FromImage(bitmap2);
            graphics.DrawImage(bitmap, dstRect, srcRect, GraphicsUnit.Pixel);
            return bitmap2;
        }

        public static byte[] BitmapToBytes(Image img)
        {
            // ImageConverter convertion du type Image en byte[] génère un .bmp
            ImageConverter ic = new ImageConverter();
            return (byte[])ic.ConvertTo(img, typeof(byte[]));
        }

        public static bool Compare0(Image bmp1, Bitmap bmp2)
        {
            if (bmp1.Size != bmp2.Size) return false;
            //ImageConverter ic = new ImageConverter();
            //byte[] byImage1 = (byte[])ic.ConvertTo(bmp1, typeof(byte[]));
            //byte[] byImage2 = (byte[])ic.ConvertTo(bmp2, typeof(byte[]));
            byte[] byImage1 = img.BitmapToBytes(bmp1);
            byte[] byImage2 = img.BitmapToBytes(bmp2);
            if (byImage1.Length != byImage2.Length) return false;
            for (int i = 0; i < byImage1.Length; i++)
                if (byImage1[i] != byImage2[i]) return false;
            return true;
        }

        public static ImageCompareResult Compare(Bitmap bmp1, Bitmap bmp2)
        {
            // source from http://www.codeproject.com/Articles/9299/Comparing-Images-using-GDI
            ImageCompareResult cr = ImageCompareResult.CompareOk;

            //Test to see if we have the same size of image

            if (bmp1.Size != bmp2.Size)
            {
                cr = ImageCompareResult.SizeMismatch;
            }
            else
            {
                //Convert each image to a byte array

                System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();
                byte[] btImage1 = new byte[1];
                btImage1 = (byte[])ic.ConvertTo(bmp1, btImage1.GetType());
                byte[] btImage2 = new byte[1];
                btImage2 = (byte[])ic.ConvertTo(bmp2, btImage2.GetType());

                //Compute a hash for each image

                SHA256Managed shaM = new SHA256Managed();
                byte[] hash1 = shaM.ComputeHash(btImage1);
                byte[] hash2 = shaM.ComputeHash(btImage2);

                //Compare the hash values

                for (int i = 0; i < hash1.Length && i < hash2.Length && cr == ImageCompareResult.CompareOk; i++)
                {
                    if (hash1[i] != hash2[i])
                        cr = ImageCompareResult.PixelMismatch;
                }
            }
            return cr;
        }

        public static ImageComparePixelResult ComparePixel(Bitmap bmp1, Bitmap bmp2)
        {
            int nbPixel = Math.Min(bmp1.Width, bmp2.Width) * Math.Min(bmp1.Height, bmp2.Height);
            int nbDifferentPixel = 0;
            double maxPixelDistance = 0;
            double totalPixelDistance = 0;
            for (int y = 0; y < bmp1.Height && y < bmp2.Height; y++)
            {
                for (int x = 0; x < bmp1.Width && x < bmp2.Width; x++)
                {
                    Color co1 = bmp1.GetPixel(x, y);
                    Color co2 = bmp2.GetPixel(x, y);
                    if (co1 != co2)
                    {
                        nbDifferentPixel++;
                        double pixelDistance = ColorDistance(co1, co2);
                        totalPixelDistance += pixelDistance;
                        if (pixelDistance > maxPixelDistance) maxPixelDistance = pixelDistance;
                    }
                }
            }
            return new ImageComparePixelResult { nbPixel = nbPixel, nbDifferentPixel = nbDifferentPixel, differentPixelPercent = (double)nbDifferentPixel / (double)nbPixel * 100.0,
                                                 maxPixelDistance = maxPixelDistance, averagePixelDistance = totalPixelDistance / nbDifferentPixel };
        }

        public static void CenterToTopLeft(int xCenter, int yCenter, int dxFromCenter, int dyFromCenter, out int x, out int y, out int dx, out int dy)
        {
            x = xCenter - dxFromCenter + 1;
            y = yCenter - dyFromCenter + 1;
            dx = (2 * dxFromCenter) - 1;
            dy = (2 * dyFromCenter) - 1;
        }

        public static double ColorDistance(Color co1, Color co2)
        {
            // ColourDistance http://www.compuphase.com/cmetric.htm
            int rmean = ((int)co1.R + (int)co2.R) / 2;
            int r = (int)co1.R - (int)co2.R;
            int g = (int)co1.G - (int)co2.G;
            int b = (int)co1.B - (int)co2.B;
            return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }
    } // class img
}
