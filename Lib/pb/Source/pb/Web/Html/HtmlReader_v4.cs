using System.Collections.Generic;
using System.IO;
using System.Text;
using pb.IO;
using pb.Text;
using System;

// todo :
//   - manage cdata : <![CDATA[   ]]>
//   - create HtmlToXml_v2 using HtmlReader_v4
//
// doc :
//   _useReadAttributeValue_v2 : use to get same result as HtmlReader_v2
//     false : bad read of attribute
//     true  : good read of attribute
//     example : line 1063 of free-telechargement.org_1_categorie-Magazines_01_01.html (c:\pib\drive\google\dev_data\exe\runsource\test_unit\Web\HtmlToXml\sites\free-telechargement.org\header)
//       onmouseover="Tip('<b>Détours en France  No.133 </b><br /><br />Ajoute par <i>devilman000</i> le <i>29/10/2014</i><br /><br /><b>Catégorie :</b> Magazines<br /><b>Date de sortie :</b> 2014<br /><b>Genre :</b> Actualité<br /><b>Type de Fichier :</b> PDF FRENCH<br /><b>Taille :</b> 107 Mo<br /><br /><b>Description :</b> <i>French | HQ PDF | 94 Pages | 107 Mb   Montagnes du Jura  Au sommaire : Jura.....</i>')"

namespace pb.Web
{
    public enum HtmlReaderNodeType
    {
        Eof = 0,
        DocumentType = 1,
        OpenTag,
        EndTag,
        Comment,
        Text
    }

    public class HtmlReader_v4
    {
        private CharStreamReader _charStreamReader = null;
        private bool _generateCloseTag = false;
        private bool _disableLineColumn = false;
        //private bool _readCommentInText = false;
        private bool _disableScriptTreatment = false;
        private bool _useReadAttributeValue_v2 = true;

        private int _htmlNodeIndex = 1;
        // construction des string : Tag, Comment, Property ...
        private StringBuilder _stringBuilder = new StringBuilder(10000, 1000000);

        public HtmlReader_v4(TextReader textReader, bool useTranslateChar = true)
        {
            _charStreamReader = new CharStreamReader(textReader);
            if (useTranslateChar)
                _charStreamReader.TranslateChar = TranslateChar;
        }

        public bool GenerateCloseTag { get { return _generateCloseTag; } set { _generateCloseTag = value; } }
        public bool DisableLineColumn { get { return _disableLineColumn; } set { _disableLineColumn = value; } }
        //public bool ReadCommentInText { get { return _readCommentInText; } set { _readCommentInText = value; } }
        public bool DisableScriptTreatment { get { return _disableScriptTreatment; } set { _disableScriptTreatment = value; } }
        public bool UseReadAttributeValue_v2 { get { return _useReadAttributeValue_v2; } set { _useReadAttributeValue_v2 = value; } }

        private string TranslateChar(char car)
        {
            int code = car;
            if (car == '—')
            {
                //car = '-';
                //_stringStream.Append(car);
                return "--";
            }
            // '\x07' dans http://www.telecharger-magazine.com/livres/502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html   c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\detail\0\livres_502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html
            // '\x1F' dans http://www.telecharger-magazine.com/actualit/117-grand-guide-2014-du-seo.html  c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\detail\0\actualit_117-grand-guide-2014-du-seo.html
            // remplace '\x07' '\x1F' par un blanc
            else if (code == 7 || code == 0x1F)
            {
                //car = ' ';
                return " ";
            }
            return null;
        }

        public IEnumerable<HtmlNode> Read()
        {
            bool script = false;
            while (true)
            {
                HtmlReaderNodeType nodeType = DetectNodeType();
                if (nodeType == HtmlReaderNodeType.Eof)
                    break;
                switch (nodeType)
                {
                    case HtmlReaderNodeType.DocumentType:
                        yield return ReadDocumentType();
                        break;
                    case HtmlReaderNodeType.OpenTag:
                        bool first = true;
                        script = false;
                        foreach (HtmlNode node in ReadOpenTag())
                        {
                            if (first)
                            {
                                script = ((HtmlNodeOpenTag)node).IsScript;
                                first = false;
                            }
                            yield return node;
                        }
                        //if (isScript)
                        //    yield return ReadScript();
                        break;
                    case HtmlReaderNodeType.EndTag:
                        script = false;
                        yield return ReadEndTag();
                        break;
                    case HtmlReaderNodeType.Comment:
                        yield return ReadComment();
                        break;
                    case HtmlReaderNodeType.Text:
                        if (script)
                            yield return ReadScript();
                        else
                            yield return ReadText();
                        break;
                }
            }
        }

        private HtmlReaderNodeType DetectNodeType()
        {
            int code = _charStreamReader.PeekChar();
            if (code == -1)
                return HtmlReaderNodeType.Eof;
            if ((char)code == '<')
            {
                char car2 = (char)_charStreamReader.PeekChar(1);
                if (car2 == '/')
                {
                    char car3 = (char)_charStreamReader.PeekChar(2);
                    if (char.IsLetter(car3))
                    {
                        return HtmlReaderNodeType.EndTag;
                    }
                }
                else if (car2 == '!')
                {
                    char car3 = (char)_charStreamReader.PeekChar(2);
                    if (car3 == 'D')
                    {
                        // <!DOCTYPE
                        if ((char)_charStreamReader.PeekChar(3) == 'O' && (char)_charStreamReader.PeekChar(4) == 'C' && (char)_charStreamReader.PeekChar(5) == 'T'
                            && (char)_charStreamReader.PeekChar(6) == 'Y' && (char)_charStreamReader.PeekChar(7) == 'P' && (char)_charStreamReader.PeekChar(8) == 'E')
                            return HtmlReaderNodeType.DocumentType;
                    }
                    else if (car3 == '-' && (char)_charStreamReader.PeekChar(3) == '-')
                        return HtmlReaderNodeType.Comment;
                }
                else if (char.IsLetter(car2))
                {
                    return HtmlReaderNodeType.OpenTag;
                }
            }
            return HtmlReaderNodeType.Text;
        }

        private HtmlNodeDocType ReadDocumentType()
        {
            // <!DOCTYPE
            // read <
            _charStreamReader.ReadChar();
            HtmlNodeDocType node = new HtmlNodeDocType { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : _charStreamReader.Line,
                Column = _disableLineColumn ? 0 : _charStreamReader.Column };

            // read !DOCTYPE
            _charStreamReader.ReadChar(8);

            ReadSeparator();
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int code = _charStreamReader.PeekChar();
                if (code == -1)
                    break;
                char car = (char)code;
                if (car == '>')
                {
                    _charStreamReader.ReadChar();
                    break;
                }
                _charStreamReader.ReadChar();
                _stringBuilder.Append((char)code);
                code = _charStreamReader.PeekChar();
            }
            //_isDocType = true;
            //_docType = _stringBuilder.ToString();

            // $$pb modif le 11/01/2015 lit '>'
            //if (code == '>')
            //    _charStreamReader.ReadChar();

            //return new HtmlReaderTag { DocTypeTag = true, DocType = _stringBuilder.ToString(), CloseTag = true };
            node.DocType = _stringBuilder.ToString();
            return node;
        }

        private IEnumerable<HtmlNode> ReadOpenTag()
        {
            // read <
            _charStreamReader.ReadChar();
            HtmlNodeOpenTag openTag = new HtmlNodeOpenTag { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : _charStreamReader.Line,
                Column = _disableLineColumn ? 0 : _charStreamReader.Column };

            // read tag name
            openTag.Name = ReadTagName();

            //if (string.Compare(openTag.Name, "script", true) == 0)
            if (!_disableScriptTreatment && string.Compare(openTag.Name, "script", true) == 0)
                openTag.IsScript = true;

            yield return openTag;

            int code = _charStreamReader.PeekChar();
            if (code == -1)
                yield break;

            char car = (char)code;
            ReadSeparator();

            int line = 0;
            int column = 0;

            code = _charStreamReader.PeekChar();
            car = (char)code;
            // read properties
            if (car != '/' && car != '>')
            {
                while (true)
                {
                    // read property name
                    _stringBuilder.Remove(0, _stringBuilder.Length);
                    line = 0;
                    column = 0;
                    while (true)
                    {
                        code = _charStreamReader.PeekChar();
                        if (code == -1)
                            break;
                        car = (char)code;
                        if (car == ' ' || car == '\t' || car == '\r' || car == '\n' || car == '=' || car == '>' || car == '<' || car == '/')
                            break;
                        _stringBuilder.Append(car);
                        _charStreamReader.ReadChar();
                        if (line == 0)
                        {
                            line = _charStreamReader.Line;
                            column = _charStreamReader.Column;
                        }
                    }
                    if (_stringBuilder.Length == 0)
                        break;

                    HtmlNodeProperty property = new HtmlNodeProperty
                    {
                        Index = _htmlNodeIndex++,
                        Line = _disableLineColumn ? 0 : line,
                        Column = _disableLineColumn ? 0 : column
                    };

                    property.Name = _stringBuilder.ToString();
                    ReadSeparator();

                    if ((char)_charStreamReader.PeekChar() == '=')
                    {
                        _charStreamReader.ReadChar();
                        ReadSeparator();
                        HtmlReaderStringValue value = ReadStringValue();
                        property.Quote = value.Quote;
                        property.Value = value.Value;
                        ReadSeparator();
                    }
                    yield return property;
                }
            }

            code = _charStreamReader.PeekChar();
            car = (char)code;

            bool endTag = false;
            line = 0;
            column = 0;
            if (car == '/')
            {
                endTag = true;
                _charStreamReader.ReadChar();
                line = _charStreamReader.Line;
                column = _charStreamReader.Column;
                code = _charStreamReader.PeekChar();
                car = (char)code;
            }

            if (car == '>')
            {
                _charStreamReader.ReadChar();
                if (endTag)
                    yield return CreateHtmlNodeEndTag(openTag.Name, line, column);
                else if (_generateCloseTag)
                    yield return CreateHtmlNodeCloseTag(openTag.Name, _charStreamReader.Line, _charStreamReader.Column);
            }
        }

        private HtmlNodeEndTag ReadEndTag()
        {
            // read <
            _charStreamReader.ReadChar();
            HtmlNodeEndTag endTag = new HtmlNodeEndTag
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : _charStreamReader.Line,
                Column = _disableLineColumn ? 0 : _charStreamReader.Column
            };
            // read /
            _charStreamReader.ReadChar();
            endTag.Name = ReadTagName();

            if ((char)_charStreamReader.PeekChar() == '>')
                _charStreamReader.ReadChar();
            return endTag;
        }

        private HtmlNodeScript ReadScript()
        {
            HtmlNodeScript script = new HtmlNodeScript
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : _charStreamReader.Line,
                Column = _disableLineColumn ? 0 : _charStreamReader.Column
            };
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int code = _charStreamReader.PeekChar();
                if (code == -1)
                    break;
                char car = (char)code;
                if (car == '<' && _charStreamReader.PeekChar(1) == '/' && char.ToLower((char)_charStreamReader.PeekChar(2)) == 's'
                    && char.ToLower((char)_charStreamReader.PeekChar(3)) == 'c' && char.ToLower((char)_charStreamReader.PeekChar(4)) == 'r'
                    && char.ToLower((char)_charStreamReader.PeekChar(5)) == 'i' && char.ToLower((char)_charStreamReader.PeekChar(6)) == 'p'
                    && char.ToLower((char)_charStreamReader.PeekChar(7)) == 't' && char.ToLower((char)_charStreamReader.PeekChar(8)) == '>')
                    break;
                _stringBuilder.Append(car);
                _charStreamReader.ReadChar();
            }
            //_text = _stringBuilder.ToString();
            //_text = HtmlCharCodes.TranslateCode(_text);
            script.Script = HtmlCharCodes.TranslateCode(_stringBuilder.ToString()).zReplaceControl();
            return script;
        }

        private HtmlNodeComment ReadComment()
        {
            // read <
            _charStreamReader.ReadChar();
            HtmlNodeComment comment = new HtmlNodeComment
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : _charStreamReader.Line,
                Column = _disableLineColumn ? 0 : _charStreamReader.Column
            };
            // read !--
            _charStreamReader.ReadChar(3);

            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int code = _charStreamReader.PeekChar();
                if (code == -1)
                    break;
                char car = (char)code;
                if (car == '-' && _charStreamReader.PeekChar(1) == '-' && _charStreamReader.PeekChar(2) == '>')
                {
                    _charStreamReader.ReadChar(3);
                    break;
                }
                _stringBuilder.Append(car);
                _charStreamReader.ReadChar();
            }
            comment.Comment = _stringBuilder.ToString().zReplaceControl();
            return comment;
        }

        private HtmlNodeText ReadText()
        {
            bool isTextSeparator = true;
            _stringBuilder.Remove(0, _stringBuilder.Length);
            //_stringBuilder.Append((char)_charStreamReader.ReadChar());
            int line = 0;
            int column = 0;
            while (true)
            {
                if (IsEndText())
                    break;
                char car = (char)_charStreamReader.ReadChar();
                if (line == 0)
                {
                    line = _charStreamReader.Line;
                    column = _charStreamReader.Column;
                }
                if (car != ' ' && car != '\t' && car != '\r' && car != '\n')
                    isTextSeparator = false;
                _stringBuilder.Append(car);
            }
            return new HtmlNodeText
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : line,
                Column = _disableLineColumn ? 0 : column,
                Text = HtmlCharCodes.TranslateCode(_stringBuilder.ToString()).zReplaceControl(),
                IsTextSeparator = isTextSeparator
            };
        }

        private bool IsEndText()
        {
            // end text if "<[a-z]" "</[a-z]" ou "<!--"
            int code = _charStreamReader.PeekChar();
            if (code == -1)
                return true;
            if ((char)code == '<')
            {
                code = _charStreamReader.PeekChar(1);
                char car = (char)code;
                if (char.IsLetter(car))
                    return true;
                else if (car == '/')
                {
                    if (char.IsLetter((char)_charStreamReader.PeekChar(2)))
                        return true;
                }
                else if (car == '!')
                {
                    if ((char)_charStreamReader.PeekChar(2) == '-' && (char)_charStreamReader.PeekChar(3) == '-')
                        return true;
                }
            }
            return false;
        }

        private string ReadTagName()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            int code = 0;
            char car = '\0';
            while (true)
            {
                code = _charStreamReader.PeekChar();
                if (code == -1)
                    break;
                car = (char)code;
                if (car == '<' || car == '/' || car == '>' || car == ' ' || car == '\t' || car == '\r' || car == '\n')
                    break;
                _stringBuilder.Append(car);
                _charStreamReader.ReadChar();
            }
            return _stringBuilder.ToString();
        }

        private HtmlNodeCloseTag CreateHtmlNodeCloseTag(string name, int line, int column)
        {
            return new HtmlNodeCloseTag
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : line,
                Column = _disableLineColumn ? 0 : column,
                Name = name
            };
        }

        private HtmlNodeEndTag CreateHtmlNodeEndTag(string name, int line, int column)
        {
            return new HtmlNodeEndTag
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : line,
                Column = _disableLineColumn ? 0 : column,
                Name = name
            };
        }

        private HtmlReaderStringValue ReadStringValue()
        {
            HtmlReaderStringValue value = new HtmlReaderStringValue();

            bool quote = false;
            char quoteChar = (char)_charStreamReader.PeekChar();
            if (quoteChar == '"' || quoteChar == '\'')
            {
                quote = true;
                value.Quote = quoteChar;
                _charStreamReader.ReadChar();
            }

            char car = '\0';

            Func<bool> isString_v1 = () =>
            {
                if (quote)
                {
                    if (car == quoteChar)
                    {
                        _charStreamReader.ReadChar();
                        return false;
                    }
                    if ((car == '"' || car == '\'') && _charStreamReader.PeekChar(1) == '>')
                    {
                        _charStreamReader.ReadChar();
                        return false;
                    }
                }
                else
                {
                    if (car == ' ' || car == '\t' || car == '\r' || car == '\n')
                        return false;
                    if (car == '>' || (car == '/' && _charStreamReader.PeekChar(1) == '>'))
                        return false;
                }
                // bad test see _useReadAttributeValue_v2
                if (car == '>' || (car == '/' && _charStreamReader.PeekChar(1) == '>'))
                    return false;
                return true;
            };

            Func<bool> isString_v2 = () =>
            {
                if (quote)
                {
                    if (car == quoteChar)
                    {
                        _charStreamReader.ReadChar();
                        return false;
                    }
                    if ((car == '"' || car == '\'') && _charStreamReader.PeekChar(1) == '>')
                    {
                        _charStreamReader.ReadChar();
                        return false;
                    }
                }
                else
                {
                    if (car == ' ' || car == '\t' || car == '\r' || car == '\n')
                        return false;
                    if (car == '>' || (car == '/' && _charStreamReader.PeekChar(1) == '>'))
                        return false;
                }
                return true;
            };

            Func<bool> isString;
            if (!_useReadAttributeValue_v2)
                isString = isString_v1;
            else
                isString = isString_v2;

            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int code = _charStreamReader.PeekChar();
                if (code == -1)
                    break;
                car = (char)code;
                //if ((quote && (car == quoteChar || ((car == '"' || car == '\'') && _charStreamReader.PeekChar(1) == '>')))
                //    || (!quote && (car == ' ' || car == '\t' || car == '\r' || car == '\n'))
                //    || car == '>' || (car == '/' && _charStreamReader.PeekChar(1) == '>'))
                //    break;
                //if (quote)
                //{
                //    if (car == quoteChar)
                //    {
                //        _charStreamReader.ReadChar();
                //        break;
                //    }
                //    if ((car == '"' || car == '\'') && _charStreamReader.PeekChar(1) == '>')
                //    {
                //        _charStreamReader.ReadChar();
                //        break;
                //    }
                //}
                //else
                //{
                //    if (car == ' ' || car == '\t' || car == '\r' || car == '\n')
                //        break;
                //    if (car == '>' || (car == '/' && _charStreamReader.PeekChar(1) == '>'))
                //        break;
                //}
                if (!isString())
                    break;
                _stringBuilder.Append(car);
                _charStreamReader.ReadChar();
            }
            value.Value = HtmlCharCodes.TranslateCode(_stringBuilder.ToString());
            return value;
        }

        private string ReadSeparator()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int code = _charStreamReader.PeekChar();
                if (code == -1)
                    break;
                char car = (char)code;
                if (car != ' ' && car != '\t' && car != '\r' && car != '\n')
                    break;
                _charStreamReader.ReadChar();
                _stringBuilder.Append(car);
            }
            return _stringBuilder.ToString();
        }

        public static IEnumerable<HtmlNode> ReadFile(string file, Encoding encoding = null, bool generateCloseTag = false, bool disableLineColumn = false,
            bool disableScriptTreatment = false, bool useReadAttributeValue_v2 = true, bool useTranslateChar = true)
        {
            using (StreamReader sr = zfile.OpenText(file, encoding))
            {
                HtmlReader_v4 htmlReader = new HtmlReader_v4(sr, useTranslateChar);
                htmlReader.GenerateCloseTag = generateCloseTag;
                htmlReader.DisableLineColumn = disableLineColumn;
                htmlReader.DisableScriptTreatment = disableScriptTreatment;
                htmlReader.UseReadAttributeValue_v2 = useReadAttributeValue_v2;
                foreach (HtmlNode node in htmlReader.Read())
                {
                    yield return node;
                }
            }
        }
    }
}
