using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using pb.IO;
using System.Collections.Generic;

namespace pb.Data.OpenXml.Test
{
    public static class Test_OpenXml_Image
    {
        private const string _directoryBase = @"c:\pib\dev_data\exe\runsource\test\Test_OpenXml";
        private static string _directory = null;
        private static string _pictureDir = @"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images\";

        public static void SetDirectory()
        {
            //_directory = zPath.Combine(RunSource.CurrentRunSource.ProjectDirectory, "docs\\Test_OpenXml_Image");
            _directory = zPath.Combine(_directoryBase, "docs\\Test_OpenXml_Image");
            if (!zDirectory.Exists(_directory))
                zDirectory.CreateDirectory(_directory);
        }

        public static void Test_01()
        {
            SetDirectory();
            string file = "test_01.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);
            OXmlDoc.Create(zPath.Combine(_directory, file), GetDocXElements_01());
        }

        public static void Test_02()
        {
            SetDirectory();
            string file = "test_02.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);
            OXmlDoc.Create(zPath.Combine(_directory, file), GetDocXElements_02());
        }

        public static void Test_03()
        {
            SetDirectory();
            string file = "test_03.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);
            OXmlDoc.Create(zPath.Combine(_directory, file), GetDocXElements_03());
        }

        public static void Test_04()
        {
            SetDirectory();
            string file = "test_04.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);
            OXmlDoc.Create(zPath.Combine(_directory, file), GetDocXElements_04());
        }

        public static void Test_05()
        {
            SetDirectory();
            string file = "test_05.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);
            OXmlDoc.Create(zPath.Combine(_directory, file), GetDocXElements_05());
        }

        public static void Test_06()
        {
            SetDirectory();
            string file = "test_06.docx";
            Trace.WriteLine("create docx \"{0}\" using OXmlDoc", file);
            OXmlDoc.Create(zPath.Combine(_directory, file), GetDocXElements_06());
        }

        public static IEnumerable<OXmlElement> GetDocXElements_06()
        {
            //yield return new OXmlDocDefaultsParagraphPropertiesElement();
            yield return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "La nuit fut agitée. Pas de bataille de chiens errants mais passage du camion poubelle à 4h... et nous étions juste à côté ! Charlotte s est levée pour vérifier qu ils n embarquaient pas nos vélos avec. La musique a duré un certain temps... car la vidange des poubelles se fait manuellement." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Puis en fait notre hôtel perdu... est situé à côté de une grande nationale manifestement... bruit ou chaleur... au choix... sans parler de la multitude de mouches qui viennent nous caresser. .." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Petit déjeuner puis regard douteux, un peu apeuré de Tom... tenant dans sa main une dent... Première dent tombée en Bulgarie, y a t il des petites souris en Bulgarie?" };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Nous comprenons pourquoi Tom disait à chaque fois que nous mangions des épis de maïs qu il avait mal aux dents..." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Départ tôt pour arriver en fin de matinée dans un garage du nord de Plovdiv pour voir si on soigne Baloo." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            // SquareSize = 21800
            //yield return new OXmlPictureElement { File = _pictureDir + "112509336.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509336.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Chargement des vélos (5) et du siège enfant dans l habitacle... Oui, nous commencions à nous sentir trop au large dans Baloo... alors nous avons souhaité partager notre espace avec la famille vélo !" };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "On estime les travaux. OK. Pause déjeuner pour le garage. Alors nous aussi. Si Baloo se fait soigner, Damien restera à ses côtés et Charlotte ira se promener avec les enfants." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Mais Baloo ne se fera pas soigner... après être monté au dessus de la fosse, le garagiste n a pas envie de se lancer et trouver une remorque est mission compliquée en Bulgarie." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Alors, visite de Plovdiv en famille." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Plovdiv est la 2eme ville en taille et etait avant la capitale. Cette ville est peu connue en dehors de la Bulgarie pourtant elle sera capitale europeenne en 2019 et est une des plus anciennes villes d Europe." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Très belle ville colorée, pleine d histoires." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Minarets, clochers, ruines, rue commerçantes et pietonnes avec magasins de gelato. .. il y en a pour tous les goûts !" };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            // SquareSize = 21800
            //yield return new OXmlPictureElement { File = _pictureDir + "112509351.jpg", Width = 300, PictureDrawing = new OXmlPictureAnchorDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509354.jpg", Width = 300, PictureDrawing = new OXmlPictureAnchorDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(50000, 21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509351.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapSquare(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509354.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapSquare(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Nous passons du temps dans différents lieux romains datant de l antiquité (les restes d un stade qui pouvait contenir 30 000 spectateurs mais dont il ne reste pas grand chose, avec film en 3D aidant les enfants à imaginer, situe dans la zone commerçante et un théâtre romain dans la vieille ville). Balade dans la zone pietonne de Plovdiv et visite de la mosquée Djoumaya qui est l’une des plus grandes de Bulgarie. Elle date du XIVe siècle, date à laquelle Plovdiv a été conquise par les Ottomans. Les non-musulmans peuvent entrer pour la visiter et les femmes n’ont pas besoin d’être voilées donc l occasion fut prise pour nous qui n avaient jamais eu l occasion de rentrer dedans sauf Damien qui avait déjà eu l occasion au Maroc." };

            //yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509392.jpg", Width = 300, PictureDrawing = new OXmlPictureAnchorDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509399.jpg", Width = 300, PictureDrawing = new OXmlPictureAnchorDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(50000, 21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509392.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapSquare(),
            HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509399.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapSquare(),
            HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509404.jpg", Width = 300, PictureDrawing = new OXmlPictureAnchorDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(50000, 21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509404.jpg", Width = 300, PictureDrawing = new OXmlInlinePictureDrawing() };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Balade dans la vieille ville aux rues pavées en haut d une colline. Les maisons sont très élégantes, souvent colorées, à colombage. Il s agit de maisons datant de la renaissance bulgare XVIII-XIX e siècle où la Bulgarie s affirmait par rapport à l empire ottoman. Notre ballade nous a ainsi conduit devant lamaison Kouyoumdjioglu, occupée par le musée d’ethnographie (fermée le lundi), la maison de Dimiter Gueorguiadi, où se trouve le musée d’histoire et de la Renaissance nationale bulgare, la maison de Lamartine, où le poète français a fait escale en 1833 lors de l’écriture de son Voyage en Orient." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "C est egalement lors de cette balade que nous croisons des touristes français, rien que des touristes, ce n est pas fréquent. La dame rassure Tom, elle a déjà vu des petites souris en Bulgarie ;-)" };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509359.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(50000, 21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509359.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 50000, VerticalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "D autres vestiges, des ruines d un forum, s etendent vers la ville moderne mais très vaste et très en ruine, un passage sans explication et pas encore mis en valeur ne permet pas de comprendre toute cette richesse à nos pieds." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Fin de la balade avec... une gelato !" };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Jamais les enfants n en auront autant mangé !" };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Direction Kazanlak. Le paysage est splendide avec de superbes montagnes." };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(50000, 21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 50000, VerticalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509447.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(50000, 21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509447.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 50000, VerticalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Dodo sous la tombe thrace après que Charlotte ait joué au super héros en haut du toi pour y accrocher 3 vélos... sans bloquer les fenêtres de toit... mais en coinçant le coffre de toit... récupérer les planches sera difficile..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "1h18..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "leka nohst" };
            //yield return new OXmlTextElement { Text = "" };
            //yield return new OXmlTextElement { Text = "" };
            //yield return new OXmlTextElement { Text = "" };
            //yield return new OXmlTextElement { Text = "" };
            //yield return new OXmlTextElement { Text = "" };
            //yield return new OXmlTextElement { Text = "" };
            //yield return new OXmlTextElement { Text = "" };
        }

        public static IEnumerable<OXmlElement> GetDocXElements_05()
        {
            //yield return new OXmlDocDefaultsParagraphPropertiesElement();
            yield return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Alors, visite de Plovdiv en famille." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Plovdiv est la 2eme ville en taille et etait avant la capitale. Cette ville est peu connue en dehors de la Bulgarie pourtant elle sera capitale europeenne en 2019 et est une des plus anciennes villes d Europe." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Très belle ville colorée, pleine d histoires." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Minarets, clochers, ruines, rue commerçantes et pietonnes avec magasins de gelato. .. il y en a pour tous les goûts !" };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            // SquareSize = 21800
            //yield return new OXmlPictureElement { File = _pictureDir + "112509351.jpg", Width = 450, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509351.jpg", Width = 450, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509354.jpg", Width = 450, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 455 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509354.jpg", Width = 450, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 455 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Nous passons du temps dans différents lieux romains datant de l antiquité (les restes d un stade qui pouvait contenir 30 000 spectateurs mais dont il ne reste pas grand chose, avec film en 3D aidant les enfants à imaginer, situe dans la zone commerçante et un théâtre romain dans la vieille ville). Balade dans la zone pietonne de Plovdiv et visite de la mosquée Djoumaya qui est l’une des plus grandes de Bulgarie. Elle date du XIVe siècle, date à laquelle Plovdiv a été conquise par les Ottomans. Les non-musulmans peuvent entrer pour la visiter et les femmes n’ont pas besoin d’être voilées donc l occasion fut prise pour nous qui n avaient jamais eu l occasion de rentrer dedans sauf Damien qui avait déjà eu l occasion au Maroc." };
            //yield return new OXmlTextElement { Text = "" };
        }

        public static IEnumerable<OXmlElement> GetDocXElements_04()
        {
            // 2 images using WrapTight (or WrapTopAndBottom or WrapSquare)
            // 1 image with text beside using WrapTight (or WrapSquare)
            //   image 1   -   image 2
            //   image 3   -   text

            //yield return new OXmlDocDefaultsParagraphPropertiesElement();
            yield return new OXmlElement { Type = OXmlElementType.DocDefaultsParagraphProperties };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Direction Kazanlak. Le paysage est splendide avec de superbes montagnes." };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            // picture width 300 height 169
            //yield return new OXmlPictureElement { File = _pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new OXmlPictureAnchorDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new OXmlPictureAnchorDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            // SquareSize = 21800
            //yield return new OXmlPictureElement { File = _pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new OXmlPictureElement { File = _pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };

            //yield return new OXmlPictureElement { File = _pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing { Wrap = new OXmlAnchorWrapTight { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) },
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing {
                Wrap = new OXmlAnchorWrapTight { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } },
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };

            //yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Dodo sous la tombe thrace après que Charlotte ait joué au super héros en haut du toi pour y accrocher 3 vélos... sans bloquer les fenêtres de toit... mais en coinçant le coffre de toit... récupérer les planches sera difficile..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "1h18..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "leka nohst" };
        }

        public static IEnumerable<OXmlElement> GetDocXElements_03()
        {
            // 4 images using WrapTopAndBottom
            //   image 1   -   image 2
            //   Paragraph
            //   image 3   -   image 4

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Direction Kazanlak. Le paysage est splendide avec de superbes montagnes." };

            //yield return new zDocXPictureElement { File = pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new zDocXPictureElement { File = pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new zDocXPictureElement { File = pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };
            //yield return new zDocXPictureElement { File = pictureDir + "112509447.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            // picture width 300 height 169
            yield return new OXmlPictureElement { File = _pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlPictureElement { File = _pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509447.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Dodo sous la tombe thrace après que Charlotte ait joué au super héros en haut du toi pour y accrocher 3 vélos... sans bloquer les fenêtres de toit... mais en coinçant le coffre de toit... récupérer les planches sera difficile..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "1h18..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "leka nohst" };
        }

        public static IEnumerable<OXmlElement> GetDocXElements_02()
        {
            // 4 images using WrapTopAndBottom
            //   image 1   -   image 2
            //   image 3   -   image 4

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Direction Kazanlak. Le paysage est splendide avec de superbes montagnes." };

            //yield return new zDocXPictureElement { File = pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new zDocXPictureElement { File = pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            //yield return new zDocXPictureElement { File = pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };
            //yield return new zDocXPictureElement { File = pictureDir + "112509447.jpg", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing()
            //{ HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            // picture width 300 height 169
            yield return new OXmlPictureElement { File = _pictureDir + "112509426.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509432.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 0 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509438.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 0, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509447.jpg", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTopAndBottom(),
                HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin, HorizontalPositionOffset = 305, VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph, VerticalPositionOffset = 174 } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Dodo sous la tombe thrace après que Charlotte ait joué au super héros en haut du toi pour y accrocher 3 vélos... sans bloquer les fenêtres de toit... mais en coinçant le coffre de toit... récupérer les planches sera difficile..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "1h18..." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "leka nohst" };
        }

        public static IEnumerable<OXmlElement> GetDocXElements_01()
        {
            // 1 image with text beside using WrapSquare or WrapTight

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "La nuit fut agitée. Pas de bataille de chiens errants mais passage du camion poubelle à 4h... et nous étions juste à côté ! Charlotte s est levée pour vérifier qu ils n embarquaient pas nos vélos avec. La musique a duré un certain temps... car la vidange des poubelles se fait manuellement." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Puis en fait notre hôtel perdu... est situé à côté de une grande nationale manifestement... bruit ou chaleur... au choix... sans parler de la multitude de mouches qui viennent nous caresser. .." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Petit déjeuner puis regard douteux, un peu apeuré de Tom... tenant dans sa main une dent... Première dent tombée en Bulgarie, y a t il des petites souris en Bulgarie?" };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Nous comprenons pourquoi Tom disait à chaque fois que nous mangions des épis de maïs qu il avait mal aux dents..." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Départ tôt pour arriver en fin de matinée dans un garage du nord de Plovdiv pour voir si on soigne Baloo." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };

            // <img src="./files/images/112509336.jpg" border="0" alt="Baloo va t il etre soigne" class="width300 leftalign">
            // 2857500 = 300 pt       Width = 300, Height = 300          300 * 9525 = 2 857 500
            // cx = img.Width * 9526; cy = img.Height * 9526; from Novacode.Paragraph.CreatePicture()
            // DrawingMode : Inline, AnchorWrapNone, AnchorWrapSquare, AnchorWrapTight, AnchorWrapThrough, AnchorWrapTopBottom
            // DrawingMode = zDocXPictureDrawingMode.AnchorWrapSquare
            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new zDocXPictureWrapSquareAnchorDrawing() };
            //zDocXPictureWrapSquareAnchorDrawing pictureDrawing = new zDocXPictureWrapSquareAnchorDrawing()
            //{
            //    HorizontalRelativeFrom = DW.HorizontalRelativePositionValues.Margin,
            //    HorizontalPositionOffset = 0,
            //    VerticalRelativeFrom = DW.VerticalRelativePositionValues.Paragraph,
            //    VerticalPositionOffset = 0,
            //    WrapText = DW.WrapTextValues.BothSides
            //};
            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = pictureDrawing };
            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 2 } };
            // SquareSize = 21451     SquareSize = 25451
            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new zDocXPictureWrapTightAnchorDrawing() { SquareSize = 21800 } };
            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new zDocXPictureWrapTopAndBottomAnchorDrawing() };

            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new zDocXPictureInlineDrawing() };
            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new zDocXPictureAnchorDrawing() { Wrap = new zDocXAnchorWrapSquare() } };
            // SquareSize = 21800
            //yield return new OXmlPictureElement { File = _pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() { Wrap = new OXmlAnchorWrapTight() { WrapPolygon = OXmlDoc.CreateWrapPolygon(21800) } } };
            yield return new OXmlPictureElement { File = _pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new OXmlAnchorPictureDrawing() {
                Wrap = new OXmlAnchorWrapTight() { WrapPolygon = new OXmlSquare { HorizontalSize = 21800 } } } };

            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Chargement des vélos (5) et du siège enfant dans l habitacle... Oui, nous commencions à nous sentir trop au large dans Baloo... alors nous avons souhaité partager notre espace avec la famille vélo !" };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "On estime les travaux. OK. Pause déjeuner pour le garage. Alors nous aussi. Si Baloo se fait soigner, Damien restera à ses côtés et Charlotte ira se promener avec les enfants." };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Mais Baloo ne se fera pas soigner... après être monté au dessus de la fosse, le garagiste n a pas envie de se lancer et trouver une remorque est mission compliquée en Bulgarie." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlElement { Type = OXmlElementType.Paragraph };
            yield return new OXmlTextElement { Text = "Alors, visite de Plovdiv en famille." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Plovdiv est la 2eme ville en taille et etait avant la capitale. Cette ville est peu connue en dehors de la Bulgarie pourtant elle sera capitale europeenne en 2019 et est une des plus anciennes villes d Europe." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Très belle ville colorée, pleine d histoires." };
            yield return new OXmlElement { Type = OXmlElementType.Line };
            yield return new OXmlTextElement { Text = "Minarets, clochers, ruines, rue commerçantes et pietonnes avec magasins de gelato. .. il y en a pour tous les goûts !" };
            //yield return new zDocXTextElement { Text = "" };
        }
    }
}
