using System.Drawing;

namespace pb.Data.Test
{
    public static class Test_Image_01
    {
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
