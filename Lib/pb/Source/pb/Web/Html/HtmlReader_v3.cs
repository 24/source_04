using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using pb.IO;
using pb.Text;

namespace pb.Web.Html
{

    public class HtmlReaderTag
    {
        public int Line;
        public int Column;
        public string Name;
        public bool BeginTag = false;
        public bool EndTag = false;
        public bool CloseTag = false;
        public bool ScriptTag = false;
        public bool DocTypeTag = false;
        public string DocType;
    }

    public class HtmlReaderProperty
    {
        public int Line;
        public int Column;
        public string Name;
        public HtmlReaderStringValue Value;
        public bool EndTag = false;
        public bool CloseTag = false;
    }

    public class HtmlReaderComment
    {
        public int Line;
        public int Column;
        public string Comment;
    }

    public class HtmlReaderScript
    {
        public int Line;
        public int Column;
        public string Script;
    }

    public class HtmlReaderText
    {
        public int Line;
        public int Column;
        public string Text;
        public bool IsTextSeparator;
    }

    public class HtmlReader_v3 : IDisposable
    {
        //private static bool __generateCloseTag = false;
        //private static bool __disableLineColumn = false;
        private bool _generateCloseTag = false;
        private bool _disableLineColumn = false;

        // stream de lecture de la source Html fichier Html
        private TextReader _textReader = null;
        private bool _closeTextReader = false;

        private bool _tracePeekChar = false;
        private StreamWriter _traceHtmlReaderStreamWriter = null;
        //private int _traceIndex = 0;
        private int _htmlNodeIndex = 1;

        // export du flux Html dans path
        //private string _exportHtmlFile = null;
        // flux d'export Html
        private StreamWriter _exportHtmlStreamWriter = null;

        private bool _readCommentInText = false;

        // lecture du flux Html pour pouvoir faire des Peek()
        private StringBuilder _stringStream = new StringBuilder(100, 1000000);
        // position du prochain caractère à lire dans _stringStream
        private int _posStringStream = 0;
        private int _posSourceStream = 0;

        // control PeekChar(0) pour éviter une boucle sans fin
        private int _maxPeekCharCount = 200;
        private int _lastPosPeekChar = -1;
        private int _peekCharCount = 0;

        // Compte le nombre de caractère Unread pour ne pas exporter 2 fois ces caractères dans l'export du flux Html
        private int _nbUnreadChar = 0;

        //private bool _markInProgress = false;

        // construction des string : Tag, Comment, Property ...
        private StringBuilder _stringBuilder = new StringBuilder(10000, 1000000);

        private int _line = 1;
        private int _column = 1;

        private char _char = '\0';
        private int _charInt = 0;
        private char _lastChar = '\0';

        public HtmlReader_v3(TextReader textReader, bool closeTextReader = false)
        {
            _textReader = textReader;
            _closeTextReader = closeTextReader;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (_textReader != null && _closeTextReader)
            {
                _textReader.Close();
                _textReader = null;
            }
            _stringStream.Remove(0, _stringStream.Length);
            _posStringStream = 0;
            _stringBuilder.Remove(0, _stringBuilder.Length);
            //CloseTraceHtmlReader();
            //CloseExportHtml();
            //_readInProgress = false;
        }

        public bool GenerateCloseTag { get { return _generateCloseTag; } set { _generateCloseTag = value; } }
        public bool DisableLineColumn { get { return _disableLineColumn; } set { _disableLineColumn = value; } }
        public bool ReadCommentInText { get { return _readCommentInText; } set { _readCommentInText = value; } }

        public IEnumerable<HtmlNode> Read()
        {
            return Read_v1();
            //return Read_v2();
        }

        /// <returns>true valeur trouvée, false plus de valeur</returns>
        // Valeurs retournées par read :
        // une marque est soit MarkBegin, soit MarkEnd, soit MarkBeginEnd
        // la dernière propriété d'une marque peut être MarkBeginEnd
        // <mark>text</mark> :
        //   - IsMarkBegin = true    MarkName = "mark"
        //   - IsText      = true    MarkName = "mark" Value = "text"
        //   - IsMarkEnd   = true    MarkName = "mark"
        // <mark property="value">text</mark> :
        //   - IsMarkBegin = true    MarkName = "mark"
        //   - IsProperty  = true    MarkName = "mark" PropertyName = "property" PropertyValue = "value"
        //   - IsText      = true    MarkName = "mark" Value = "text"
        //   - IsMarkEnd   = true    MarkName = "mark"
        // <mark property="value"/> :
        //   - IsMarkBegin = true    MarkName = "mark"
        //   - IsProperty  = true    MarkName = "mark" PropertyName = "property" PropertyValue = "value" IsMarkBeginEnd = true
        // <mark/> :
        //   - IsMarkBeginEnd = true MarkName = "mark"
        // <script>source</script>
        //   - IsMarkBegin = true    IsScript = true  MarkName = "script"
        //   - IsText      = true    IsScript = true  MarkName = "script" Value = "source"
        //   - IsMarkEnd   = true    IsScript = false MarkName = "script"
        public IEnumerable<HtmlNode> Read_v1()
        {
            while (true)
            {
                int charInt = PeekChar();
                if (charInt == -1)
                    yield break;
                char car = (char)charInt;

                HtmlReaderTag tag = ReadTag();
                if (tag != null)
                {
                    // <div>           : BeginTag = true,  EndTag = false, CloseTag = true,  DocTypeTag = false
                    // <div ... >      : BeginTag = true,  EndTag = false, CloseTag = false, DocTypeTag = false, ready to read properties
                    // <div ... />     : BeginTag = true,  EndTag = false, CloseTag = false, DocTypeTag = false, ready to read properties
                    // <div />         : BeginTag = true,  EndTag = true,  CloseTag = true,  DocTypeTag = false
                    // </div>          : BeginTag = false, EndTag = true,  CloseTag = true,  DocTypeTag = false
                    // <!DOCTYPE ... > : BeginTag = false, EndTag = false, CloseTag = true,  DocTypeTag = true

                    // <div>           : OpenTag div, CloseTag div EndTag = false
                    // <div ... >      : OpenTag div, Property ..., CloseTag div EndTag = false
                    // <div ... />     : OpenTag div, Property ..., CloseTag div EndTag = true
                    // <div />         : OpenTag div, CloseTag div EndTag = true
                    // </div>          : EndTag div
                    // <!DOCTYPE ... > : DocumentType

                    if (tag.DocTypeTag)
                    {
                        yield return new HtmlNodeDocType { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : tag.Line, Column = _disableLineColumn ? 0 : tag.Column,
                            DocType = tag.DocType };
                    }
                    else if(tag.BeginTag)
                    {
                        yield return new HtmlNodeOpenTag { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : tag.Line, Column = _disableLineColumn ? 0 : tag.Column,
                            Name = tag.Name, IsScript = tag.ScriptTag };

                        if (tag.EndTag)
                        {
                            yield return new HtmlNodeEndTag { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : tag.Line, Column = _disableLineColumn ? 0 : tag.Column,
                                Name = tag.Name };
                        }

                        // read properties
                        if (!tag.CloseTag)
                        {
                            while (true)
                            {
                                HtmlReaderProperty property = ReadProperty();
                                if (property != null)
                                {
                                    yield return new HtmlNodeProperty { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : property.Line, Column = _disableLineColumn ? 0 : property.Column,
                                        Name = property.Name, Value = property.Value.Value, Quote = property.Value.Quote };

                                    if (property.CloseTag)
                                    {
                                        if (property.EndTag)
                                            yield return new HtmlNodeEndTag { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : tag.Line, Column = _disableLineColumn ? 0 : tag.Column,
                                                Name = tag.Name };
                                        else if (_generateCloseTag)
                                            yield return new HtmlNodeCloseTag { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : tag.Line, Column = _disableLineColumn ? 0 : tag.Column,
                                                Name = tag.Name };
                                        break;
                                    }
                                }
                                else
                                {
                                    if (_generateCloseTag)
                                        yield return new HtmlNodeCloseTag { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : tag.Line, Column = _disableLineColumn ? 0 : tag.Column,
                                            Name = tag.Name };
                                    break;
                                }
                            }
                        }

                        // read script
                        if (tag.ScriptTag)
                        {
                            HtmlReaderScript script = ReadScript();
                            if (script != null)
                                yield return new HtmlNodeScript { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : script.Line, Column = _disableLineColumn ? 0 : script.Column,
                                    Script = script.Script.zReplaceControl() };
                        }
                    }
                    else if (tag.EndTag)
                    {
                        yield return new HtmlNodeEndTag { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : tag.Line, Column = _disableLineColumn ? 0 : tag.Column,
                            Name = tag.Name, };
                    }

                    continue;
                }

                HtmlReaderComment comment = ReadComment();
                if (comment != null)
                {
                    yield return new HtmlNodeComment { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : comment.Line, Column = _disableLineColumn ? 0 : comment.Column,
                        Comment = comment.Comment.zReplaceControl() };

                    continue;
                }

                HtmlReaderText text = ReadText();
                yield return new HtmlNodeText { Index = _htmlNodeIndex++, Line = _disableLineColumn ? 0 : text.Line, Column = _disableLineColumn ? 0 : text.Column,
                    Text = text.Text.zReplaceControl(), IsTextSeparator = text.IsTextSeparator };
            }
        }

        public IEnumerable<HtmlNode> Read_v2()
        {
            while (true)
            {
                int charInt = PeekChar();
                if (charInt == -1)
                    yield break;
                char car = (char)charInt;

                HtmlReaderTag tag = ReadTag();
                if (tag != null)
                {
                    foreach (HtmlNode node in ReadTagNodes(tag))
                        yield return node;
                    continue;
                }

                HtmlReaderComment comment = ReadComment();
                if (comment != null)
                {
                    yield return CreateHtmlNodeComment(comment);
                    continue;
                }

                HtmlReaderText text = ReadText();
                yield return CreateHtmlNodeText(text);
            }
        }

        //private bool BeginTag()
        //{
        //    // $$pb modif le 11/01/2015 pour gérer <!DOCTYPE ...
        //    return (char)PeekChar() == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2))) || (PeekChar(1) == '!' && PeekChar(2) == 'D'));
        //}

        //private void ReadMarkName()
        private HtmlReaderTag ReadTag()
        {
            // <div>           : BeginTag = true,  EndTag = false, CloseTag = true,  DocTypeTag = false
            // <div ... >      : BeginTag = true,  EndTag = false, CloseTag = false, DocTypeTag = false, ready to read properties
            // <div ... />     : BeginTag = true,  EndTag = false, CloseTag = false, DocTypeTag = false, ready to read properties
            // <div />         : BeginTag = true,  EndTag = true,  CloseTag = true,  DocTypeTag = false
            // </div>          : BeginTag = false, EndTag = true,  CloseTag = true,  DocTypeTag = false
            // <!DOCTYPE ... > : BeginTag = false, EndTag = false, CloseTag = true,  DocTypeTag = true
            HtmlReaderTag tag = null;
            if ((char)PeekChar() == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2))) || (PeekChar(1) == '!' && PeekChar(2) == 'D')))
            {
                tag = new HtmlReaderTag();
                tag.Line = _line;
                tag.Column = _column;

                // read '<'
                GetChar();

                if (PeekChar() == '/')
                {
                    //_isMarkEnd = true;
                    tag.EndTag = true;
                    GetChar();
                }
                else
                    //_isMarkBegin = true;
                    tag.BeginTag = true;

                // read tag name
                _stringBuilder.Remove(0, _stringBuilder.Length);
                GetChar();
                while (_charInt != -1 && _char != '<' && _char != '/' && _char != '>' && _char != ' ' && _char != '\t' && _char != '\r' && _char != '\n')
                {
                    _stringBuilder.Append(_char);
                    GetChar();
                }
                //_markName = _stringBuilder.ToString();
                tag.Name = _stringBuilder.ToString();

                //_scriptMarkInProgress = false;
                //if (string.Compare(_markName, "!doctype", true) == 0 && _isMarkBegin)
                if (string.Compare(tag.Name, "!doctype", true) == 0 && tag.BeginTag)
                {
                    HtmlReaderTag tag2 = ReadDocType();
                    tag2.Line = tag.Line;
                    tag2.Column = tag.Column;
                    tag = tag2;
                    //return;
                }
                else
                {
                    //if (string.Compare(_markName, "script", true) == 0 && _isMarkBegin)
                    if (string.Compare(tag.Name, "script", true) == 0 && tag.BeginTag)
                        //_scriptMarkInProgress = true;
                        tag.ScriptTag = true;
                    if (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n')
                        ReadSeparator();
                    if (_char == '/')
                    {
                        //_isMarkBegin = false;
                        //_isMarkEnd = false;
                        //_isMarkBeginEnd = true;
                        tag.BeginTag = true;
                        tag.EndTag = true;
                        //tag.Tag = true;
                        GetChar();
                    }

                    if (_char == '>')
                        tag.CloseTag = true;
                    else
                        UnreadChar();

                }
            }
            return tag;
        }

        private IEnumerable<HtmlNode> ReadTagNodes(HtmlReaderTag tag)
        {
            // <div>           : BeginTag = true,  EndTag = false, CloseTag = true,  DocTypeTag = false
            // <div ... >      : BeginTag = true,  EndTag = false, CloseTag = false, DocTypeTag = false, ready to read properties
            // <div ... />     : BeginTag = true,  EndTag = false, CloseTag = false, DocTypeTag = false, ready to read properties
            // <div />         : BeginTag = true,  EndTag = true,  CloseTag = true,  DocTypeTag = false
            // </div>          : BeginTag = false, EndTag = true,  CloseTag = true,  DocTypeTag = false
            // <!DOCTYPE ... > : BeginTag = false, EndTag = false, CloseTag = true,  DocTypeTag = true

            // <div>           : OpenTag div, CloseTag div EndTag = false
            // <div ... >      : OpenTag div, Property ..., CloseTag div EndTag = false
            // <div ... />     : OpenTag div, Property ..., CloseTag div EndTag = true
            // <div />         : OpenTag div, CloseTag div EndTag = true
            // </div>          : EndTag div
            // <!DOCTYPE ... > : DocumentType

            if (tag.DocTypeTag)
            {
                yield return CreateHtmlNodeDocType(tag);
            }
            else if (tag.BeginTag)
            {
                yield return CreateHtmlNodeOpenTag(tag);

                if (tag.EndTag)
                {
                    yield return CreateHtmlNodeEndTag(tag);
                }

                // read properties
                if (!tag.CloseTag)
                {
                    foreach (HtmlNode node in ReadPropertiesNodes(tag))
                        yield return node;
                }

                // read script
                if (tag.ScriptTag)
                {
                    HtmlReaderScript script = ReadScript();
                    if (script != null)
                        yield return CreateHtmlNodeScript(script);
                }
            }
            else if (tag.EndTag)
            {
                yield return CreateHtmlNodeEndTag(tag);
            }
        }

        private IEnumerable<HtmlNode> ReadPropertiesNodes(HtmlReaderTag tag)
        {
            while (true)
            {
                HtmlReaderProperty property = ReadProperty();
                if (property != null)
                {
                    yield return CreateHtmlNodeProperty(property);

                    if (property.CloseTag)
                    {
                        if (property.EndTag)
                            yield return CreateHtmlNodeEndTag(tag);
                        else if (_generateCloseTag)
                            yield return CreateHtmlNodeCloseTag(tag);
                        break;
                    }
                }
                else
                {
                    if (_generateCloseTag)
                        yield return CreateHtmlNodeCloseTag(tag);
                    break;
                }
            }
        }

        private HtmlReaderTag ReadDocType()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            int i = PeekChar();
            while (i != -1 && i != '>')
            {
                GetChar();
                _stringBuilder.Append(_char);
                i = PeekChar();
            }
            //_isDocType = true;
            //_docType = _stringBuilder.ToString();

            // $$pb modif le 11/01/2015 lit '>'
            if (i == '>')
                GetChar();

            return new HtmlReaderTag { DocTypeTag = true, DocType = _stringBuilder.ToString(), CloseTag = true };
        }

        //private bool ReadProperty()
        private HtmlReaderProperty ReadProperty()
        {
            ReadSpaceChar();
            // detect a new tag inside the current tag
            if (PeekChar() == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2)))))
                //return false;
                return null;

            HtmlReaderProperty property = new HtmlReaderProperty();
            property.Line = _line;
            property.Column = _column;

            //string propertyName = ReadPropertyName();
            property.Name = ReadPropertyName();
            string separator = ReadSeparator();
            //HtmlStringValue value = null;
            if (PeekChar() == '=')
            {
                GetChar();
                ReadSpaceChar();
                property.Value = ReadStringValue();
                separator = ReadSeparator();
            }
            else
                property.Value = new HtmlReaderStringValue();
            if (_char == '/')
            {
                //_isMarkBeginEnd = true;
                property.EndTag = true;
                GetChar();
            }
            if (_char == '>')
                property.CloseTag = true;
            //_isProperty = true;
            //return true;
            return property;
        }

        private string ReadPropertyName()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int iChar = PeekChar();
                char cChar = (char)iChar;
                if (iChar == -1 || cChar == ' ' || cChar == '\t' || cChar == '\r' || cChar == '\n' || cChar == '=' || cChar == '>' || cChar == '<')
                    break;
                GetChar();
                _stringBuilder.Append(_char);
            }
            //_propertyName = _stringBuilder.ToString();
            return _stringBuilder.ToString();
        }

        private HtmlReaderStringValue ReadStringValue()
        {
            bool quote = false;
            //_propertyQuote = "";
            HtmlReaderStringValue value = new HtmlReaderStringValue();
            //value.Quote = "";
            char quoteChar = (char)PeekChar();
            if (quoteChar == '"' || quoteChar == '\'')
            {
                quote = true;
                //_propertyQuote = quoteChar.ToString();
                value.Quote = quoteChar;
                GetChar();
            }

            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                GetChar();
                if (_charInt == -1
                    || (quote && (_char == quoteChar || ((_char == '"' || _char == '\'') && PeekChar() == '>')))
                    || (!quote && (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n'))
                    || _char == '>' || (_char == '/' && PeekChar() == '>')
                    )
                    break;
                _stringBuilder.Append(_char);
            }
            //_propertyValue = _stringBuilder.ToString();
            //_propertyValue = HtmlCharCodes.TranslateCode(_propertyValue);
            value.Value = HtmlCharCodes.TranslateCode(_stringBuilder.ToString());
            return value;
        }

        //private void ReadScript()
        private HtmlReaderScript ReadScript()
        {
            int line = _line;
            int column = _column;

            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int intChar = PeekChar();
                if (intChar == -1)
                    break;
                if ((char)intChar == '<' && PeekChar(1) == '/' && char.ToLower((char)PeekChar(2)) == 's' && char.ToLower((char)PeekChar(3)) == 'c' &&
                    char.ToLower((char)PeekChar(4)) == 'r' && char.ToLower((char)PeekChar(5)) == 'i' && char.ToLower((char)PeekChar(6)) == 'p' &&
                    char.ToLower((char)PeekChar(7)) == 't' && char.ToLower((char)PeekChar(8)) == '>')
                    break;
                GetChar();
                _stringBuilder.Append(_char);
            }
            //_text = _stringBuilder.ToString();
            //_text = HtmlCharCodes.TranslateCode(_text);
            if (_stringBuilder.Length > 0)
                return new HtmlReaderScript { Line = line, Column = column, Script = HtmlCharCodes.TranslateCode(_stringBuilder.ToString()) };
            else
                return null;
        }

        //private void ReadComment()
        private HtmlReaderComment ReadComment()
        {
            HtmlReaderComment comment = null;
            if (PeekChar() == '<' && PeekChar(1) == '!' && PeekChar(2) == '-' && PeekChar(3) == '-')
            {
                int line = _line;
                int column = _column;

                _stringBuilder.Remove(0, _stringBuilder.Length);
                while (true)
                {
                    GetChar();
                    if (_charInt == -1)
                        break;
                    _stringBuilder.Append(_char);
                    int l = _stringBuilder.Length;
                    if (l >= 3 && _stringBuilder[l - 3] == '-' && _stringBuilder[l - 2] == '-' && _stringBuilder[l - 1] == '>')
                        break;
                }
                //_isComment = true;

                string s = _stringBuilder.ToString();
                // pour supprimer <!-- et -->
                if (s.Length >= 7)
                    //_comment = s.Substring(4, s.Length - 7);
                    s = s.Substring(4, s.Length - 7);
                comment = new HtmlReaderComment { Line = line, Column = column, Comment = s };
            }
            return comment;
        }

        //private void ReadText()
        private HtmlReaderText ReadText()
        {
            int line = _line;
            int column = _column;

            bool isTextSeparator = true;
            bool comment = false;
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int charInt = PeekChar();
                if (!comment && (char)charInt == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2)))))
                    break;
                if (!comment && (char)charInt == '<' && PeekChar(1) == '!' && PeekChar(2) == '-' && PeekChar(3) == '-')
                {
                    if (!_readCommentInText)
                        break;
                    comment = true;
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    charInt = PeekChar();
                    isTextSeparator = false;
                }

                if (charInt == -1)
                    break;
                GetChar();
                // modif le 24/07/2016 pour éliminer les 0x00
                if (_char != '\x00')
                    _stringBuilder.Append(_char);
                if (_char != ' ' && _char != '\t' && _char != '\r' && _char != '\n')
                    isTextSeparator = false;
                int l = _stringBuilder.Length;
                if (comment && l >= 3 && _stringBuilder[l - 3] == '-' && _stringBuilder[l - 2] == '-' && _stringBuilder[l - 1] == '>')
                    comment = false;
            }
            //_text = _stringBuilder.ToString();
            //_text = HtmlCharCodes.TranslateCode(_text);
            //_isTextSeparator = isTextSeparator;
            return new HtmlReaderText { Line = line, Column = column, Text = HtmlCharCodes.TranslateCode(_stringBuilder.ToString()), IsTextSeparator = isTextSeparator };
        }

        private void ReadSpaceChar()
        {
            while (true)
            {
                char cChar = (char)PeekChar();
                if (cChar != ' ' && cChar != '\t' && cChar != '\r' && cChar != '\n') break;
                GetChar();
            }
        }

        private string ReadSeparator()
        {
            if (_char == '>' || _char == '/')
                return "";
            _stringBuilder.Remove(0, _stringBuilder.Length);
            if (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n')
                _stringBuilder.Append(_char);
            while (true)
            {
                int intChar = PeekChar();
                if (intChar == -1)
                    break;
                char c = (char)intChar;
                if (c != '>' && c != '/' && c != ' ' && c != '\t' && c != '\r' && c != '\n')
                    break;
                GetChar();
                if (_char != '>' && _char != '/')
                    _stringBuilder.Append(_char);
                else
                    break;
            }
            //_separator = _stringBuilder.ToString();
            return _stringBuilder.ToString();
        }

        private HtmlNodeDocType CreateHtmlNodeDocType(HtmlReaderTag tag)
        {
            return new HtmlNodeDocType
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : tag.Line,
                Column = _disableLineColumn ? 0 : tag.Column,
                DocType = tag.DocType
            };
        }

        private HtmlNodeOpenTag CreateHtmlNodeOpenTag(HtmlReaderTag tag)
        {
            return new HtmlNodeOpenTag
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : tag.Line,
                Column = _disableLineColumn ? 0 : tag.Column,
                Name = tag.Name,
                IsScript = tag.ScriptTag
            };
        }

        private HtmlNodeCloseTag CreateHtmlNodeCloseTag(HtmlReaderTag tag)
        {
            return new HtmlNodeCloseTag
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : tag.Line,
                Column = _disableLineColumn ? 0 : tag.Column,
                Name = tag.Name
            };
        }

        private HtmlNodeEndTag CreateHtmlNodeEndTag(HtmlReaderTag tag)
        {
            return new HtmlNodeEndTag
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : tag.Line,
                Column = _disableLineColumn ? 0 : tag.Column,
                Name = tag.Name
            };
        }

        private HtmlNodeProperty CreateHtmlNodeProperty(HtmlReaderProperty property)
        {
            return new HtmlNodeProperty
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : property.Line,
                Column = _disableLineColumn ? 0 : property.Column,
                Name = property.Name,
                Value = property.Value.Value,
                Quote = property.Value.Quote
            };
        }

        private HtmlNodeScript CreateHtmlNodeScript(HtmlReaderScript script)
        {
            return new HtmlNodeScript
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : script.Line,
                Column = _disableLineColumn ? 0 : script.Column,
                Script = script.Script.zReplaceControl()
            };
        }

        private HtmlNodeComment CreateHtmlNodeComment(HtmlReaderComment comment)
        {
            return new HtmlNodeComment
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : comment.Line,
                Column = _disableLineColumn ? 0 : comment.Column,
                Comment = comment.Comment.zReplaceControl()
            };
        }

        private HtmlNodeText CreateHtmlNodeText(HtmlReaderText text)
        {
            return new HtmlNodeText
            {
                Index = _htmlNodeIndex++,
                Line = _disableLineColumn ? 0 : text.Line,
                Column = _disableLineColumn ? 0 : text.Column,
                Text = text.Text.zReplaceControl(),
                IsTextSeparator = text.IsTextSeparator
            };
        }

        private bool PeekName(ref int i)
        {
            while (true)
            {
                int charInt = PeekChar(i);
                if (charInt == -1)
                    return false;
                char car = (char)charInt;
                if (car == '/' || car == '>' || car == '=')
                    return true;
                if (char.IsControl(car))
                    return false;
                if (char.IsSeparator(car))
                    return true;
                if (char.IsPunctuation(car))
                    return false;
                if (char.IsSurrogate(car))
                    return false;
                if (char.IsSymbol(car))
                    return false;
                i++;
            }
        }

        private void PeekSeparator(ref int i)
        {
            while (true)
            {
                int charInt = PeekChar(i);
                if (charInt == -1)
                    return;
                char car = (char)charInt;
                if (!char.IsSeparator(car))
                    return;
                i++;
            }
        }

        private void GetChar()
        {
            _charInt = ReadChar();
            _lastChar = _char;
            _char = '\0';
            if (_charInt != -1)
            {
                _char = (char)_charInt;
                if (_char == '\r')
                {
                    _column = 1;
                    _line++;
                }
                else if (_char == '\n')
                {
                    _column = 1;
                    if (_lastChar != '\r') _line++;
                }
                else
                    _column++;
            }
        }

        private void UnreadChar()
        {
            if (_posStringStream == 0)
                throw new PBException("unable to unread char line {0} column {1}", _line, _column);
            _posStringStream--;
            _nbUnreadChar++;
        }

        private int ReadChar()
        {
            int charInt = PeekChar(0);
            _posStringStream++;
            if (_exportHtmlStreamWriter != null && charInt != -1)
            {
                if (_nbUnreadChar == 0)
                    _exportHtmlStreamWriter.Write((char)charInt);
            }
            if (_nbUnreadChar > 0)
                _nbUnreadChar--;
            return charInt;
        }

        private int PeekChar()
        {
            return PeekChar(0);
        }

        private int PeekChar(int i)
        {
            if (i + 10 >= _stringStream.MaxCapacity)
                throw new PBException("unable to peek char overflow of string stream max capacity PeekChar({0}) max capacity = {1}", i, _stringStream.Capacity);
            if (i >= _stringStream.MaxCapacity - _posStringStream)
            {
                for (int i1 = 0, i2 = _posStringStream; i2 < _stringStream.Length; i1++, i2++)
                    _stringStream[i1] = _stringStream[i2];
                _stringStream.Remove(_stringStream.Length - _posStringStream, _posStringStream);
                _posSourceStream += _posStringStream;
                _posStringStream = 0;
            }
            int charInt;
            char charText;
            if (_posStringStream + i >= _stringStream.Length)
            {
                for (int i1 = _stringStream.Length; i1 <= _posStringStream + i; i1++)
                {
                    charInt = _textReader.Read();
                    if (charInt == -1)
                        break;
                    charText = (char)charInt;
                    if (charText == '—')
                    {
                        charText = '-';
                        _stringStream.Append(charText);
                    }
                    // '\x07' dans http://www.telecharger-magazine.com/livres/502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html   c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\detail\0\livres_502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html
                    // '\x1F' dans http://www.telecharger-magazine.com/actualit/117-grand-guide-2014-du-seo.html  c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\detail\0\actualit_117-grand-guide-2014-du-seo.html
                    // remplace '\x07' '\x1F' par un blanc
                    else if (charInt == 7 || charInt == 0x1F)
                    {
                        charText = ' ';
                    }
                    _stringStream.Append(charText);
                }
                if (_posStringStream + i >= _stringStream.Length)
                {
                    return -1;
                }
            }
            charInt = _stringStream[_posStringStream + i];
            if (_tracePeekChar && _traceHtmlReaderStreamWriter != null)
            {
                charText = (char)charInt;
                _traceHtmlReaderStreamWriter.WriteLine("      peek char i {0} line {1} column {2} pos {3,7} code {4} char {5}", i, _line, _column, _posSourceStream + _posStringStream + i, charInt.zToHex(), !char.IsControl(charText) ? "\"" + charText + "\"" : "---");
            }
            // control PeekChar(0) pour éviter une boucle sans fin
            if (i == 0)
            {
                int pos = _posSourceStream + _posStringStream + i;
                if (pos == _lastPosPeekChar)
                {
                    if (++_peekCharCount >= _maxPeekCharCount)
                    {
                        charText = (char)charInt;
                        throw new PBException("PeekChar : to much read of same character {0} {1} line {2} column {3} position {4}", charInt.zToHex(), !char.IsControl(charText) ? "\"" + charText + "\"" : "---", _line, _column, _lastPosPeekChar);
                    }
                }
                else
                {
                    _lastPosPeekChar = pos;
                    _peekCharCount = 1;
                }
            }
            return charInt;
        }

        public static IEnumerable<HtmlNode> ReadFile(string file, Encoding encoding = null, bool generateCloseTag = false, bool disableLineColumn = false)
        {
            using (StreamReader sr = zfile.OpenText(file, encoding))
            {
                HtmlReader_v3 htmlReader = new HtmlReader_v3(sr);
                htmlReader.GenerateCloseTag = generateCloseTag;
                htmlReader.DisableLineColumn = disableLineColumn;
                foreach (HtmlNode node in htmlReader.Read())
                {
                    yield return node;
                }
            }
        }

        public static IEnumerable<HtmlNode> ReadFile_v2(string file, Encoding encoding = null, bool generateCloseTag = false, bool disableLineColumn = false)
        {
            using (StreamReader sr = zfile.OpenText(file, encoding))
            {
                HtmlReader_v3 htmlReader = new HtmlReader_v3(sr);
                htmlReader.GenerateCloseTag = generateCloseTag;
                htmlReader.DisableLineColumn = disableLineColumn;
                foreach (HtmlNode node in htmlReader.Read_v2())
                {
                    yield return node;
                }
            }
        }
    }
}
