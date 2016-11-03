using pb;
using pb.Data.OpenXml;
using pb.IO;
using System.Collections.Generic;

namespace Test.Test_OpenXml
{
    public static class Test_OpenXml_05
    {
        private const string _directoryBase = @"c:\pib\dev_data\exe\runsource\test\Test_OpenXml";
        private static string _directory = null;

        public static void SetDirectory()
        {
            //_directory = zPath.Combine(RunSource.CurrentRunSource.ProjectDirectory, "docs\\Test_OpenXml_05");
            _directory = zPath.Combine(_directoryBase, "docs\\Test_OpenXml_05");
            if (!zDirectory.Exists(_directory))
                zDirectory.CreateDirectory(_directory);
        }

        public static void Test_01()
        {
            SetDirectory();
            string file = "test_01.docx";
            Trace.WriteLine("create docx \"{0}\" using zDocX", file);
            zDocX.Create(zPath.Combine(_directory, file), GetDocXElements_01());
        }

        public static IEnumerable<zDocXElement> GetDocXElements_01()
        {
            string pictureDir = @"c:\pib\_dl\test\BlogDemoor\from-chrome\files\images\";

            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            yield return new zDocXTextElement { Text = "La nuit fut agitée. Pas de bataille de chiens errants mais passage du camion poubelle à 4h... et nous étions juste à côté ! Charlotte s est levée pour vérifier qu ils n embarquaient pas nos vélos avec. La musique a duré un certain temps... car la vidange des poubelles se fait manuellement." };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXTextElement { Text = "Puis en fait notre hôtel perdu... est situé à côté de une grande nationale manifestement... bruit ou chaleur... au choix... sans parler de la multitude de mouches qui viennent nous caresser. .." };
            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            yield return new zDocXTextElement { Text = "Petit déjeuner puis regard douteux, un peu apeuré de Tom... tenant dans sa main une dent... Première dent tombée en Bulgarie, y a t il des petites souris en Bulgarie?" };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXTextElement { Text = "Nous comprenons pourquoi Tom disait à chaque fois que nous mangions des épis de maïs qu il avait mal aux dents..." };
            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            yield return new zDocXTextElement { Text = "Départ tôt pour arriver en fin de matinée dans un garage du nord de Plovdiv pour voir si on soigne Baloo." };
            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            // <img src="./files/images/112509336.jpg" border="0" alt="Baloo va t il etre soigne" class="width300 leftalign">
            //yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", DrawingMode = zDocXPictureDrawingMode.Inline, Width = 300, Height = 300 };
            yield return new zDocXPictureElement { File = pictureDir + "112509336.jpg", Description = "Baloo va t il etre soigne", Width = 300, PictureDrawing = new zDocXPictureInlineDrawing() };
            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            yield return new zDocXTextElement { Text = "Chargement des vélos (5) et du siège enfant dans l habitacle... Oui, nous commencions à nous sentir trop au large dans Baloo... alors nous avons souhaité partager notre espace avec la famille vélo !" };
            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            yield return new zDocXTextElement { Text = "On estime les travaux. OK. Pause déjeuner pour le garage. Alors nous aussi. Si Baloo se fait soigner, Damien restera à ses côtés et Charlotte ira se promener avec les enfants." };
            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            yield return new zDocXTextElement { Text = "Mais Baloo ne se fera pas soigner... après être monté au dessus de la fosse, le garagiste n a pas envie de se lancer et trouver une remorque est mission compliquée en Bulgarie." };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXElement { Type = zDocXElementType.Paragraph };
            yield return new zDocXTextElement { Text = "Alors, visite de Plovdiv en famille." };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXTextElement { Text = "Plovdiv est la 2eme ville en taille et etait avant la capitale. Cette ville est peu connue en dehors de la Bulgarie pourtant elle sera capitale europeenne en 2019 et est une des plus anciennes villes d Europe." };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXTextElement { Text = "Très belle ville colorée, pleine d histoires." };
            yield return new zDocXElement { Type = zDocXElementType.Line };
            yield return new zDocXTextElement { Text = "Minarets, clochers, ruines, rue commerçantes et pietonnes avec magasins de gelato. .. il y en a pour tous les goûts !" };
            //yield return new zDocXTextElement { Text = "" };
        }
    }
}
