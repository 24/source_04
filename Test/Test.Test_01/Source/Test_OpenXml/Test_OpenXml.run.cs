Trace.WriteLine("toto");
Test_OpenXml_01.Test_OpenXml_01();

Test_OpenXml_01.Test_CreateWordDoc_02("test_02.docx", "toto tata");
Test_OpenXml_01.Test_CreateWordDoc_01("test_01.docx", "toto tata");
Test_OpenXml_01.InsertAPicture("test_04.docx", @"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images\112509336.jpg", 1, "Picture1", cx: 2930400, cy: 2930400);

Test_OpenXml_02.Test_Images();
Test_OpenXml_03.Test();
Test_OpenXml_04.Test_01();
Test_OpenXml_04.Test_02();
Test_OpenXml_05.Test_01();
Test_OpenXml_06.Test_01();

The type 'Package' is defined in an assembly that is not referenced. You must add a reference to assembly 'System.IO.Packaging, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'.




System.Drawing.Image image = zimg.LoadImageFromFile(@"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images\112509336.jpg");
Trace.WriteLine("image width {0} height {1} type {2} PixelFormat {3} RawFormat {4}", image.Width, image.Height, image.GetType().zGetTypeName(), image.PixelFormat, image.RawFormat);
Trace.WriteLine("{0}", zimg.GetMimeType(image));
Trace.WriteLine("{0}", System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders().Length);     // 8
Trace.WriteLine("{0}", System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders().Length);     // 5

Trace.WriteLine(System.Drawing.Imaging.ImageFormat.Jpeg.ToString());
Trace.WriteLine("{0}", System.Drawing.Imaging.ImageFormat.Jpeg);
Trace.WriteLine("{0}", System.Drawing.Imaging.ImageFormat.Jpeg.Guid);

