using pb.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace pb.Data.Test
{
    public static class Test_Image_01
    {
        public static void Test_01(string file)
        {
            using (Bitmap bitmap = new  Bitmap(file))
            //using (Image image = Image.FromFile(file))
            {
                Trace.WriteLine($"image \"{file}\"");
                Trace.WriteLine($"width {bitmap.Width} height {bitmap.Height} PixelFormat {bitmap.PixelFormat}");
                int length = bitmap.Width * bitmap.Height;
                byte[] bytes = new byte[length];
                // bitmap.PixelFormat
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
                Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
                bitmap.UnlockBits(bitmapData);
                zFile.WriteAllBytes(file + ".bin", bytes);
            }
        }

        public static void Test_02(string file)
        {
            byte[] bytes = new byte[] {
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
            };
            zFile.WriteAllBytes(file + ".bin", bytes);
            int width = 16;
            int height = 16;
            int length = width * height;
            PixelFormat format = PixelFormat.Format8bppIndexed;
            using (Bitmap bitmap = new Bitmap(width, height, format))
            {
                //BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, format);
                //Marshal.Copy(bitmapData.Scan0, bytes, 0, length);
                Marshal.Copy(bytes, 0, bitmapData.Scan0, length);
                bitmap.UnlockBits(bitmapData);
                bitmap.Save(file, ImageFormat.Bmp);
            }
        }

        public static void CopyBytes(string inputFile, int width, int x1, int y1, int x2, int y2, string outputFile)
        {
            byte[] bytes = zFile.ReadAllBytes(inputFile);
            int width2 = x2 - x1 + 1;
            int height2 = y2 - y1 + 1;
            byte[] bytes2 = new byte[width2 * height2];
            int pos = y1 * width + x1;
            int pos2 = 0;
            for (int y = y1; y <= y2; y++)
            {
                Array.Copy(bytes, pos, bytes2, pos2, width2);
                pos += width;
                pos2 += width2;
            }
            zFile.WriteAllBytes(outputFile, bytes2);
        }

        public static void CreateBitmap(string inputFile, int width, int height, string outputFile)
        {
            byte[] bytes = zFile.ReadAllBytes(inputFile);
            using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed))
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                Marshal.Copy(bytes, 0, bitmapData.Scan0, width * height);
                bitmap.UnlockBits(bitmapData);
                bitmap.Save(outputFile, ImageFormat.Bmp);
            }

        }

        public static void BitmapInfo(string inputFile)
        {
            Trace.WriteLine($"load bitmap \"{inputFile}\"");
            using (Bitmap bitmap = new Bitmap(inputFile))
            {
                Trace.WriteLine($"Width {bitmap.Width} Height {bitmap.Height} PixelFormat {bitmap.PixelFormat}");
            }
        }

        public static void SaveBitmapData(string inputFile, string outputFile)
        {
            Trace.WriteLine($"load bitmap \"{inputFile}\"");
            using (Bitmap bitmap = new Bitmap(inputFile))
            {
                int bitsPerPixel = GetBitsPerPixel(bitmap.PixelFormat);
                int byteNb = (bitmap.Width * bitmap.Height * bitsPerPixel + 7) / 8;
                Trace.WriteLine($"Width {bitmap.Width} Height {bitmap.Height} PixelFormat {bitmap.PixelFormat} bitsPerPixel {bitsPerPixel} byteNb {byteNb}");
                //bitmap.Save(outputFile, );
                byte[] bytes = new byte[byteNb];
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                Marshal.Copy(bitmapData.Scan0, bytes, 0, byteNb);
                bitmap.UnlockBits(bitmapData);
                zFile.WriteAllBytes(outputFile, bytes);
                //bytes.SetValue
                //Buffer.BlockCopy
            }
        }

        public static int GetBitsPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    return 1;
                case PixelFormat.Format4bppIndexed:
                    return 4;
                case PixelFormat.Format8bppIndexed:
                    return 8;
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppGrayScale:
                    return 16;
                case PixelFormat.Format24bppRgb:
                    return 24;
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppArgb:
                    return 32;
                case PixelFormat.Format48bppRgb:
                    return 48;
                case PixelFormat.Format64bppPArgb:
                case PixelFormat.Format64bppArgb:
                    return 64;
                default:
                    throw new PBException($"unknow bits per pixel for {pixelFormat}");
            }
        }

        public static void ConvertToGif(string inputFile, string outputFile)
        {
            using (Bitmap bitmap = new Bitmap(inputFile))
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                Image image = Image.FromStream(ms);
                image.Save(outputFile, ImageFormat.Gif);
            }
        }

        public static void ReplaceData(string inputFile, string outputFile)
        {
            byte[] bytes = zFile.ReadAllBytes(inputFile);
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != 0)
                    bytes[i] = 0xFF;
            }
            zFile.WriteAllBytes(outputFile, bytes);
        }

        public static void Test_03(string inputFile, int width, int x1, int y1, int x2, int y2, string outputFile)
        {
            byte[] bytes = zFile.ReadAllBytes(inputFile);
            int width2 = x2 - x1 + 1;
            int height2 = y2 - y1 + 1;
            using (Bitmap bitmap = new Bitmap(width2, height2, PixelFormat.Format8bppIndexed))
            {
                int pos = y1 * width + x1;
                for (int y = y1; y <= y2; y++)
                {
                    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width2, 1), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                    Marshal.Copy(bytes, pos, bitmapData.Scan0, width2);
                    bitmap.UnlockBits(bitmapData);
                    pos += width;
                }
                bitmap.Save(outputFile, ImageFormat.Bmp);
            }
        }

        public static void DrawOutCropArea(Bitmap currentBitmap, int xPosition, int yPosition, int width, int height)
        {
            //_bitmapPrevCropArea = (Bitmap)_currentBitmap;
            Bitmap bitmapPrevCropArea = (Bitmap)currentBitmap;
            Bitmap bmap = (Bitmap)bitmapPrevCropArea.Clone();
            Graphics gr = Graphics.FromImage(bmap);
            Brush cBrush = new Pen(Color.FromArgb(150, Color.White)).Brush;
            Rectangle rect1 = new Rectangle(0, 0, currentBitmap.Width, yPosition);
            Rectangle rect2 = new Rectangle(0, yPosition, xPosition, height);
            Rectangle rect3 = new Rectangle
            (0, (yPosition + height), currentBitmap.Width, currentBitmap.Height);
            Rectangle rect4 = new Rectangle((xPosition + width),
                yPosition, (currentBitmap.Width - xPosition - width), height);
            gr.FillRectangle(cBrush, rect1);
            gr.FillRectangle(cBrush, rect2);
            gr.FillRectangle(cBrush, rect3);
            gr.FillRectangle(cBrush, rect4);
            currentBitmap = (Bitmap)bmap.Clone();
        }

        // Out of memory, with Document-page-015.jpg, Type System.Drawing.Bitmap PixelFormat Format24bppRgb width 1240 height 1754 in c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\images
        public static Bitmap Crop_v1(Bitmap currentBitmap, int xPosition, int yPosition, int width, int height)
        {
            // from https://www.codeproject.com/Articles/33838/Image-Processing-using-C
            //Bitmap temp = (Bitmap)_currentBitmap;
            //Bitmap temp = currentBitmap;
            //Bitmap bmap = (Bitmap)currentBitmap.Clone();
            if (xPosition + width > currentBitmap.Width)
                width = currentBitmap.Width - xPosition;
            if (yPosition + height > currentBitmap.Height)
                height = currentBitmap.Height - yPosition;
            Trace.WriteLine($"PixelFormat {currentBitmap.PixelFormat} xPosition {xPosition} yPosition {yPosition} width {width} height {height}");
            Rectangle rect = new Rectangle(xPosition, yPosition, width, height);
            //currentBitmap = bmap.Clone(rect, bmap.PixelFormat);
            return currentBitmap.Clone(rect, currentBitmap.PixelFormat);
            //currentBitmap.Clone()
        }

        // ok
        public static Bitmap Crop_v2(Bitmap img, Rectangle cropArea)
        {
            // from http://www.codingdefined.com/2015/04/solved-bitmapclone-out-of-memory.html
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics gph = Graphics.FromImage(bmp))
            {
                gph.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public static Bitmap Crop_v3(Image image, Rectangle cropArea)
        {
            // from http://www.codingdefined.com/2015/04/solved-bitmapclone-out-of-memory.html
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height, image.PixelFormat);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmp;
        }
    }
}
