using Novacode;
using pb;
using pb.IO;

// DocX https://docx.codeplex.com/
// add Image http://cathalscorner.blogspot.fr/2009/04/docx-version-1002-released.html

namespace Test.Test_OpenXml
{
    public static class Test_OpenXml_02
    {
        private const string _directoryBase = @"c:\pib\dev_data\exe\runsource\test\Test_OpenXml";
        private static string _directory = null;

        public static void Test_01()
        {
            Trace.WriteLine("Test_OpenXml_02");
        }

        public static void SetDirectory()
        {
            //_directory = zPath.Combine(RunSource.CurrentRunSource.ProjectDirectory, "docs\\Test_OpenXml_02");
            _directory = zPath.Combine(_directoryBase, "docs\\Test_OpenXml_02");
            if (!zDirectory.Exists(_directory))
                zDirectory.CreateDirectory(_directory);
        }

        public static void Test_Images()
        {
            SetDirectory();

            // Create a .docx file
            using (DocX document = DocX.Create(zPath.Combine(_directory, "Example.docx")))
            {
                // Add an Image to the docx file
                Image img = document.AddImage(zPath.Combine(_directory, "images\\donkey.png"));

                // Insert an emptyParagraph into this document.
                Paragraph p = document.InsertParagraph("", false);

                #region pic1
                //Picture pic1 = p.InsertPicture(img.Id, "Donkey", "Taken on Omey island");
                Picture pic1 = img.CreatePicture();
                Paragraph paragraph1 = p.InsertPicture(pic1);

                // Set the Picture pic1’s shape
                pic1.SetPictureShape(BasicShapes.cube);

                // Rotate the Picture pic1 clockwise by 30 degrees
                pic1.Rotation = 30;
                #endregion

                #region pic2
                // Create a Picture. A Picture is a customized view of an Image
                //Picture pic2 = p.InsertPicture(img.Id, "Donkey", "Taken on Omey island");
                Picture pic2 = img.CreatePicture();
                Paragraph paragraph2 = p.InsertPicture(pic2);

                // Set the Picture pic2’s shape
                pic2.SetPictureShape(CalloutShapes.cloudCallout);

                // Flip the Picture pic2 horizontally
                pic2.FlipHorizontal = true;
                #endregion

                // Save the docx file
                document.Save();
            }// Release this document from memory.
        }

        public static void HelloWorldAddPictureToWord()
        {
            //Console.WriteLine("\tHelloWorldAddPictureToWord()");

            // Create a document.
            using (DocX document = DocX.Create(zPath.Combine(_directory, "HelloWorldAddPictureToWord.docx")))
            {
                // Add an image into the document.    
                //RelativeDirectory rd = new RelativeDirectory(); // prepares the files for testing
                //rd.Up(2);
                //Image image = document.AddImage(rd.Path + @"\images\logo_template.png");
                Image image = document.AddImage(@"images\logo_template.png");


                // Create a picture (A custom view of an Image).
                Picture picture = image.CreatePicture();
                picture.Rotation = 10;
                picture.SetPictureShape(BasicShapes.cube);

                // Insert a new Paragraph into the document.
                // new Font("Comic Sans MS")  (Novacode.Font dont exist)
                // new WindowsFontFamily("Comic Sans MS")
                Paragraph title = document.InsertParagraph().Append("This is a test for a picture").FontSize(20).Font(new Font("Comic Sans MS"));
                title.Alignment = Alignment.center;

                // Insert a new Paragraph into the document.
                Paragraph p1 = document.InsertParagraph();

                // Append content to the Paragraph
                p1.AppendLine("Just below there should be a picture ").Append("picture").Bold().Append(" inserted in a non-conventional way.");
                p1.AppendLine();
                p1.AppendLine("Check out this picture ").AppendPicture(picture).Append(" its funky don't you think?");
                p1.AppendLine();

                // Insert a new Paragraph into the document.
                Paragraph p2 = document.InsertParagraph();
                // Append content to the Paragraph.

                p2.AppendLine("Is it correct?");
                p2.AppendLine();

                // Lets add another picture (without the fancy stuff)
                Picture pictureNormal = image.CreatePicture();

                Paragraph p3 = document.InsertParagraph();
                p3.AppendLine("Lets add another picture (without the fancy  rotation stuff)");
                p3.AppendLine();
                p3.AppendPicture(pictureNormal);

                // Save this document.
                document.Save();
                //Console.WriteLine("\tCreated: docs\\HelloWorldAddPictureToWord.docx\n");
            }
        }
    }
}
