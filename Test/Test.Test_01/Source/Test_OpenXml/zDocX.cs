using Novacode;
using pb;
using pb.Data.OpenXml;
using System.Collections.Generic;


// lecture du html :
//   - corrections :
//     - suppression des espaces avant et après <br>
namespace Test.Test_OpenXml
{
    public class zDocX
    {
        private DocX _document = null;
        private Paragraph _paragraph = null;

        private void _Create(string file, IEnumerable<zDocXElement> elements)
        {
            using (_document = DocX.Create(file))
            {
                foreach (zDocXElement element in elements)
                {
                    switch (element.Type)
                    {
                        //case zDocXElementType.BeginParagraph:
                        //    _paragraph = _document.InsertParagraph();
                        //    break;
                        //case zDocXElementType.EndParagraph:
                        //    _paragraph = null;
                        //    break;
                        case zDocXElementType.Paragraph:
                            _paragraph = _document.InsertParagraph();
                            break;
                        case zDocXElementType.Text:
                            AddText(element);
                            break;
                        case zDocXElementType.Line:
                            AddLine();
                            break;
                        case zDocXElementType.Picture:
                            AddPicture(element);
                            break;
                    }
                }
                _document.Save();
            }
        }

        private void AddText(zDocXElement element)
        {
            if (_paragraph == null)
                throw new PBException("missing begin paragraph");
            if (!(element is zDocXTextElement))
                throw new PBException("text element must be a zDocXElementText");
            _paragraph.Append(((zDocXTextElement)element).Text);
        }

        private void AddLine()
        {
            if (_paragraph == null)
                throw new PBException("missing begin paragraph");
            _paragraph.AppendLine();
        }

        private void AddPicture(zDocXElement element)
        {
            if (_paragraph == null)
                throw new PBException("missing begin paragraph");
            if (!(element is zDocXPictureElement))
                throw new PBException("picture element must be a zDocXElementPicture");
            zDocXPictureElement pictureElement = (zDocXPictureElement)element;
            Image image = _document.AddImage(pictureElement.File);
            Picture picture = image.CreatePicture();
            Trace.WriteLine("picture width {0} height {1}", picture.Width, picture.Height);
            if (pictureElement.Name != null)
                picture.Name = pictureElement.Name;
            if (pictureElement.Description != null)
                picture.Description = pictureElement.Description;
            if (pictureElement.Width != null)
                picture.Width = (int)pictureElement.Width;
            if (pictureElement.Height != null)
                picture.Height = (int)pictureElement.Height;
            //picture.Rotation = pictureElement.Rotation;
            //picture.FlipHorizontal = pictureElement.HorizontalFlip;
            //picture.FlipVertical = pictureElement.VerticalFlip;
            _paragraph.AppendPicture(picture);
            // option image from google doc :
            //   Aligner, Envelopper, Intercaler, 3.2 mm de marge
            //   In line, Wrap text, Break text, 1/8" margin
            // option image from word :
            //   disposition / position :
            //     horizontal / position absolue -0.1 cm à droite de marge
            //     vertical / position absolue 2.01 cm au dessous de paragraphe
            //     déplacer avec le texte
            //     autoriser le chevauchement de texte
        }

        public static void Create(string file, IEnumerable<zDocXElement> elements)
        {
            new zDocX()._Create(file, elements);
        }
    }
}
