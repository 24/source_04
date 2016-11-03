using Novacode;
using pb.IO;
using WindowsColor = System.Drawing.Color;

namespace Test.Test_OpenXml
{
    public static class Test_OpenXml_04
    {
        private const string _directoryBase = @"c:\pib\dev_data\exe\runsource\test\Test_OpenXml";
        private static string _directory = null;

        public static void SetDirectory()
        {
            //_directory = zPath.Combine(RunSource.CurrentRunSource.ProjectDirectory, "docs\\Test_OpenXml_04");
            _directory = zPath.Combine(_directoryBase, "docs\\Test_OpenXml_04");
            if (!zDirectory.Exists(_directory))
                zDirectory.CreateDirectory(_directory);
        }

        public static void Test_01()
        {
            SetDirectory();

            using (DocX document = DocX.Create(zPath.Combine(_directory, "test_01.docx")))
            {
                // Insert a Paragraph into this document.
                Paragraph p = document.InsertParagraph();

                // Append some text and add formatting.
                p.Append("Hello World!^011Hello World! àâä éèêë")
                .Font(new Font("Times New Roman"))
                .FontSize(32)
                .Color(WindowsColor.Blue)
                .Bold();

                // Save this document to disk.
                document.Save();
            }
        }

        public static void Test_02()
        {
            SetDirectory();

            // Create a document.
            using (DocX document = DocX.Create(zPath.Combine(_directory, "test_02.docx")))
            {
                Paragraph p = document.InsertParagraph();

                // Add an image into the document.    
                //RelativeDirectory rd = new RelativeDirectory(); // prepares the files for testing
                //rd.Up(2);
                //Image image = document.AddImage(rd.Path + @"\images\logo_template.png");
                Image image = document.AddImage(zPath.Combine(_directory, @"images\logo_template.png"));

                // Create a picture (A custom view of an Image).
                Picture picture = image.CreatePicture();
                picture.Rotation = 10;
                picture.SetPictureShape(BasicShapes.cube);

                // Insert a new Paragraph into the document.
                Paragraph title = document.InsertParagraph().Append("This is a test for a picture").FontSize(20).Font(new Font("Comic Sans MS"));
                title.Alignment = Alignment.center;

                // Insert a new Paragraph into the document.

                // Append content to the Paragraph
                p.AppendLine("Just below there should be a picture ").Append("picture").Bold().Append(" inserted in a non-conventional way.");
                p.AppendLine();
                p.AppendLine("Check out this picture ").AppendPicture(picture).Append(" its funky don't you think?");
                p.AppendLine();

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
                //DocXElement
            }
        }
    }
}
