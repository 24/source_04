using System;
using System.IO;
using System.Text;
using pb.IO;
using pb.Text;

namespace pb.Web.Html
{
    public class HtmlReader : HtmlReaderBase
    {
        public class HTMLReaderException : Exception
        {
            public HTMLReaderException(string sMessage) : base(sMessage) { }
            public HTMLReaderException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)) { }
            public HTMLReaderException(Exception InnerException, string sMessage) : base(sMessage, InnerException) { }
            public HTMLReaderException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
        }

        // source Html : soit une url http:... soit un fichier
        //private string _urlPath = null;

        // stream du fichier Html
        //private FileStream _fs = null;
        private int _bufferSize = 32000;  // taille buffer de FileStream
        //private Encoding _encoding = Encoding.UTF8; // code caractère : Default = ANSI avec le code page du système
        // no bom with new UTF8Encoding()
        private Encoding _encoding = new UTF8Encoding(); // code caractère : Default = ANSI avec le code page du système

        // stream de lecture de la source Html fichier Html
        private TextReader _textReader = null;
        private bool _closeTextReader = false;

        private static string __traceHtmlReaderFile = null;
        private static bool __tracePeekChar = false;
        //private string _traceHtmlReaderFile = null;
        private StreamWriter _traceHtmlReaderStreamWriter = null;
        private int _traceIndex = 1;

        // export du flux Html dans path
        private string _exportHtmlFile = null;
        // flux d'export Html
        private StreamWriter _exportHtmlStreamWriter = null;

        private bool _readCommentInText = false;

        // lecture du flux Html pour pouvoir faire des Peek()
        private StringBuilder _stringStream = new StringBuilder(100, 1000000);
        // position du prochain caractère à lire dans _stringStream
        private int _posStringStream = 0;
        private int _posSourceStream = 0;

        // control PeekChar(0) pour éviter une boucle sans fin
        private static int __maxPeekCharCount = 200;
        private int _lastPosPeekChar = -1;
        private int _peekCharCount = 0;

        // Compte le nombre de caractère Unread pour ne pas exporter 2 fois ces caractères dans l'export du flux Html
        private int _nbUnreadChar = 0;

        // construction des string : Tag, Comment, Property ...
        private StringBuilder _stringBuilder = new StringBuilder(10000, 1000000);

        private int _line = 1;
        private int _column = 0;

        private bool _readInProgress = false;

        private bool _isMarkBegin = false;
        private bool _isMarkEnd = false;
        private bool _isMarkBeginEnd = false;
        private bool _markInProgress = false;

        private bool _scriptMarkInProgress = false; // true si le segment Script est en cours

        private bool _isProperty = false;
        private bool _isText = false;
        private bool _isTextSeparator = false;
        private bool _isComment = false;
        private bool _isDocType = false;
        private string _markName = null;
        private string _propertyName = null;
        private string _propertyValue = null;
        private string _propertyQuote;
        private string _text = null;
        private string _comment = null;
        private string _docType = null;
        private string _separator = null;

        private char _char = '\0';
        private int _charInt = 0;
        private char _lastChar = '\0';

        public HtmlReader(TextReader textReader, bool closeTextReader = true)
        {
            _textReader = textReader;
            _closeTextReader = closeTextReader;
        }

        ///// <summary></summary>
        ///// <param name="Url_Path_Html">may be url, path or html</param>
        //public HtmlReader(string Url_Path_Html)
        //{
        //    _urlPath = Url_Path_Html;
        //}

        //public HtmlReader(Http http)
        //{
        //    _textReader = http.WebStream;
        //}

        //private void Init()
        //{
        //    InitExportHtml();
        //    //Open();
        //}

        public static string GetTraceFile(string file)
        {
            return zpath.PathSetFileName(file, zPath.GetFileName(file) + ".trace.txt");
        }

        private void OpenTraceHtmlReader()
        {
            CloseTraceHtmlReader();
            if (__traceHtmlReaderFile != null)
            {
                //FileStream fs = new FileStream(__traceHtmlReaderFile, FileMode.Create, FileAccess.Write, FileShare.Read, _bufferSize);
                FileStream fs = zFile.Open(__traceHtmlReaderFile, FileMode.Create, FileAccess.Write, FileShare.Read, _bufferSize);
                _traceHtmlReaderStreamWriter = new StreamWriter(fs, _encoding);
                _traceIndex = 1;
            }
        }

        private void CloseTraceHtmlReader()
        {
            if (_traceHtmlReaderStreamWriter != null)
            {
                _traceHtmlReaderStreamWriter.Close();
                _traceHtmlReaderStreamWriter = null;
            }
        }

        private void TraceHtmlReader()
        {
            if (_traceHtmlReaderStreamWriter == null)
                return;
            _traceHtmlReaderStreamWriter.Write("{0,5}", _traceIndex);
            if (_isDocType)
            {
                _traceHtmlReaderStreamWriter.Write(" doc type : \"{0}\"", _docType);
            }
            if (_isComment)
            {
                _traceHtmlReaderStreamWriter.Write(" comment : \"{0}\"", _comment.zReplaceControl());
            }
            if (_isText)
            {
                if (_scriptMarkInProgress)
                    _traceHtmlReaderStreamWriter.Write("   script");
                else
                    _traceHtmlReaderStreamWriter.Write(" text");
                _traceHtmlReaderStreamWriter.Write(" : \"{0}\"", _text.zReplaceControl());
            }
            if (_isMarkBegin)
            {
                _traceHtmlReaderStreamWriter.Write(" mark begin : \"{0}\"", _markName);
            }
            if (_isMarkEnd)
            {
                _traceHtmlReaderStreamWriter.Write("   mark end : \"{0}\"", _markName);
            }
            //if (htmlReader.IsMarkInProgress)
            //{
            //    sw.WriteLine("{0,5} mark in progress : \"{1}\"", i, htmlReader.MarkName);
            //}
            if (_isProperty)
            {
                _traceHtmlReaderStreamWriter.Write("   property : \"{0}\" = \"{1}\" quote {2}", _propertyName, _propertyValue, _propertyQuote);
            }
            if (_isTextSeparator)
            {
                _traceHtmlReaderStreamWriter.Write(" text separator");
            }
            if (_isMarkBeginEnd)
            {
                _traceHtmlReaderStreamWriter.Write(" mark begin end : \"{0}\"", _markName);
            }
            _traceHtmlReaderStreamWriter.WriteLine();
            _traceIndex++;
        }

        private void OpenExportHtml()
        {
            CloseExportHtml();
            if (_exportHtmlFile != null)
            {
                //FileStream fs = new FileStream(_exportHtmlFile, FileMode.Create, FileAccess.Write, FileShare.Read, _bufferSize);
                FileStream fs = zFile.Open(_exportHtmlFile, FileMode.Create, FileAccess.Write, FileShare.Read, _bufferSize);
                _exportHtmlStreamWriter = new StreamWriter(fs, _encoding);
            }
        }

        private void CloseExportHtml()
        {
            if (_exportHtmlStreamWriter != null)
            {
                _exportHtmlStreamWriter.Close();
                _exportHtmlStreamWriter = null;
            }
        }

        public override void Dispose()
        {
            Close();
        }

        //public string TraceHtmlReaderFile { get { return _traceHtmlReaderFile; } set { _traceHtmlReaderFile = value; } }
        public static string TraceHtmlReaderFile { get { return __traceHtmlReaderFile; } set { __traceHtmlReaderFile = value; } }
        public static bool TracePeekChar { get { return __tracePeekChar; } set { __tracePeekChar = value; } }
        public string ExportHtmlFile { get { return _exportHtmlFile; } set { _exportHtmlFile = value; } }
        public override bool ReadCommentInText { get { return _readCommentInText; } set { _readCommentInText = value; } }
        public Encoding encoding { get { return _encoding; } set { _encoding = value; } }
        public override int Line { get { return _line; } }
        public override int Column { get { return _column; } }
        public override bool IsMarkBegin { get { return _isMarkBegin; } }
        public override bool IsMarkEnd { get { return _isMarkEnd; } }
        public override bool IsMarkBeginEnd { get { return _isMarkBeginEnd; } }
        public override bool IsMarkInProgress { get { return _markInProgress; } }
        public override bool IsProperty { get { return _isProperty; } }
        public override bool IsText { get { return _isText; } }
        public override bool IsTextSeparator { get { return _isTextSeparator; } }
        public override bool IsComment { get { return _isComment; } }
        public override bool IsDocType { get { return _isDocType; } }
        public override bool IsScript { get { return _scriptMarkInProgress; } }
        public override string MarkName { get { return _markName; } }
        public override string PropertyName { get { return _propertyName; } }
        public override string PropertyValue { get { return _propertyValue; } }
        public override string PropertyQuote { get { return _propertyQuote; } }
        public override string Text { get { return _text; } }
        public override string Comment { get { return _comment; } }
        public override string DocType { get { return _docType; } }
        public override string Separator { get { return _separator; } }
        public override bool HasValue { get { return _isProperty || _isText || _isComment || _isDocType; } }
        public override string Value
        {
            get
            {
                if (_isProperty) return _propertyValue;
                if (_isText) return _text;
                if (_isComment) return _comment;
                if (_isDocType) return _docType;
                return null;
            }
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
        public override bool Read()
        {
            if (!_readInProgress)
            {
                OpenTraceHtmlReader();
                OpenExportHtml();
                _readInProgress = true;
            }
            bool ret = false;
            _isMarkBegin = false;
            _isMarkEnd = false;
            _isMarkBeginEnd = false;
            _isProperty = false;
            _isText = false;
            _isTextSeparator = false;
            _isComment = false;
            _isDocType = false;
            _propertyName = null;
            _propertyValue = null;
            _text = null;
            _comment = null;
            _docType = null;
            _separator = null;

            int charInt = PeekChar();
            if (charInt == -1)
                return false;  // EOF

            char car = (char)charInt;
            while (true)
            {
                if (_markInProgress)
                {
                    if (ReadProperty())
                    {
                        if (_char == '>') _markInProgress = false;
                        ret = true;
                        break;
                    }
                    else
                        _markInProgress = false;
                }
                // $$pb modif le 11/01/2015 pour gérer <!DOCTYPE ...
                //if (car == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2)))))
                if (car == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2))) || (PeekChar(1) == '!' && PeekChar(2) == 'D')))
                {
                    GetChar();
                    ReadMarkName();
                    if (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n')
                        _markInProgress = true;
                    else if (_char != '>')
                        UnreadChar();
                    ret = true;
                    break;
                }
                else if (car == '<' && PeekChar(1) == '!' && PeekChar(2) == '-' && PeekChar(3) == '-')
                {
                    ReadComment();
                    ret = true;
                    break;
                }
                else
                {
                    _isText = true;
                    if (_scriptMarkInProgress)
                        ReadScript();
                    else
                        ReadText();
                    ret = true;
                    break;
                }
            }
            if (ret)
                TraceHtmlReader();
            return ret;
        }

        public void ReadAll()
        {
            try
            {
                while (Read()) ;
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// lit le flux Html et génère un fichier Html en sortie, permet de vérifier si la lecture est ok
        /// </summary>
        public void ReadWrite(StreamWriter writer)
        {
            bool markInProgress = false;
            while (Read())
            {
                if (markInProgress)
                {
                    if (IsProperty)
                    {
                        writer.Write("{0}", PropertyName);
                        if (PropertyValue != null)
                            writer.Write("={0}{1}{2}", PropertyQuote, PropertyValue, PropertyQuote);
                        writer.Write(Separator);
                    }
                    if (!IsMarkInProgress)
                    {
                        if (IsMarkBeginEnd) writer.Write("/");
                        writer.Write(">");
                        markInProgress = false;
                    }
                }
                else if (IsMarkBegin && !IsMarkInProgress)
                    writer.Write("<{0}{1}>", MarkName, Separator);
                else if (IsMarkBegin && IsMarkInProgress)
                {
                    markInProgress = true;
                    writer.Write("<{0}{1}", MarkName, Separator);
                }
                else if (IsMarkEnd)
                    writer.Write("</{0}{1}>", MarkName, Separator);
                else if (IsMarkBeginEnd)
                    writer.Write("<{0}{1}/>", MarkName, Separator);
                else if (IsText)
                    writer.Write(Text);
                else if (IsComment)
                    writer.Write(Comment);
            }
        }

        private void ReadMarkName()
        {
            if (PeekChar() == '/')
            {
                _isMarkEnd = true;
                GetChar();
            }
            else
                _isMarkBegin = true;
            _stringBuilder.Remove(0, _stringBuilder.Length);
            GetChar();
            while (_charInt != -1 && _char != '<' && _char != '/' && _char != '>' && _char != ' ' && _char != '\t' && _char != '\r' && _char != '\n')
            {
                _stringBuilder.Append(_char);
                GetChar();
            }
            _markName = _stringBuilder.ToString();
            _scriptMarkInProgress = false;
            if (string.Compare(_markName, "!doctype", true) == 0 && _isMarkBegin)
            {
                ReadDocType();
                return;
            }
            if (string.Compare(_markName, "script", true) == 0 && _isMarkBegin) _scriptMarkInProgress = true;
            if (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n')
                ReadSeparator();
            if (_char == '/')
            {
                _isMarkBegin = false;
                _isMarkEnd = false;
                _isMarkBeginEnd = true;
                GetChar();
            }
        }

        private void ReadDocType()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            int i = PeekChar();
            while (i != -1 && i != '>')
            {
                GetChar();
                _stringBuilder.Append(_char);
                i = PeekChar();
            }
            _isDocType = true;
            _docType = _stringBuilder.ToString();
            // $$pb modif le 11/01/2015 lit '>'
            if (i == '>')
                GetChar();
        }

        private bool ReadProperty()
        {
            ReadSpaceChar();
            if (PeekChar() == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2)))))
                return false;
            ReadPropertyName();
            ReadSeparator();
            if (PeekChar() == '=')
            {
                GetChar();
                ReadSpaceChar();
                ReadStringValue();
                ReadSeparator();
            }
            if (_char == '/')
            {
                _isMarkBeginEnd = true;
                GetChar();
            }
            _isProperty = true;
            return true;
        }

        private void ReadComment()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                GetChar();
                if (_charInt == -1) break;
                _stringBuilder.Append(_char);
                int l = _stringBuilder.Length;
                if (l >= 3 && _stringBuilder[l - 3] == '-' && _stringBuilder[l - 2] == '-' && _stringBuilder[l - 1] == '>') break;
            }
            _isComment = true;

            string s = _stringBuilder.ToString();
            // pour supprimer <!-- et -->
            if (s.Length >= 7) _comment = s.Substring(4, s.Length - 7);
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

        private void ReadSeparator()
        {
            if (_char == '>' || _char == '/') return;
            _stringBuilder.Remove(0, _stringBuilder.Length);
            if (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n') _stringBuilder.Append(_char);
            while (true)
            {
                int iChar = PeekChar();
                if (iChar == -1) break;
                char cChar = (char)iChar;
                if (cChar != '>' && cChar != '/' && cChar != ' ' && cChar != '\t' && cChar != '\r' && cChar != '\n') break;
                GetChar();
                if (_char != '>' && _char != '/') _stringBuilder.Append(_char); else break;
            }
            _separator = _stringBuilder.ToString();
        }

        private void ReadPropertyName()
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
            _propertyName = _stringBuilder.ToString();
        }

        private void ReadStringValue()
        {
            bool quote = false;
            _propertyQuote = "";
            char quoteChar = (char)PeekChar();
            if (quoteChar == '"' || quoteChar == '\'')
            {
                quote = true;
                _propertyQuote = quoteChar.ToString();
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
            _propertyValue = _stringBuilder.ToString();
            _propertyValue = HtmlCharCodes.TranslateCode(_propertyValue);
        }

        private void ReadStringValue2()
        {
            bool quote = false;
            _propertyQuote = "";
            char quoteChar = (char)PeekChar();
            if (quoteChar == '"' || quoteChar == '\'')
            {
                quote = true;
                _propertyQuote = quoteChar.ToString();
                GetChar();
            }

            _stringBuilder.Remove(0, _stringBuilder.Length);

            bool quote2 = quote;
            int charInt;
            int i = 0, endQuotePos = -1;
            while ((charInt = PeekChar(i)) != -1)
            {
                char c = (char)charInt;
                if (quote2)
                {
                    if (c == quoteChar)
                    {
                        endQuotePos = i;
                        quote2 = false;
                    }
                }
                else
                {
                    if (c == '=')
                    {
                        for (; i >= 0; i--)
                        {
                            c = (char)PeekChar(i);
                            if (c != ' ' && c != '\t' && c != '\r' && c != '\n') break;
                        }
                        for (; i >= 0 && (endQuotePos == -1 || i > endQuotePos); i--)
                        {
                            char cChar = (char)PeekChar(i);
                            if (cChar == ' ' || cChar == '\t' || cChar == '\r' || cChar == '\n') break;
                        }
                        break;
                    }
                    else if (c == '>')
                    {
                        if (i > 0 && (char)PeekChar(i - 1) == '/') i--;
                        i--;
                        break;
                    }
                    else if (c == '<')
                    {
                        i--;
                        break;
                    }
                }
                i++;
            }

            for (; i >= 0; i--)
            {
                char c = (char)PeekChar(i);
                if (c != ' ' && c != '\t' && c != '\r' && c != '\n') break;
            }

            if (quote && i >= 0 && (char)PeekChar(i) == quoteChar) i--;

            for (; i >= 0; i--)
            {
                GetChar();
                _stringBuilder.Append(_char);
            }

            _propertyValue = _stringBuilder.ToString();
            _propertyValue = HtmlCharCodes.TranslateCode(_propertyValue);
        }

        private void ReadText()
        {
            int l;

            bool isTextSeparator = true;
            bool comment = false;
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int charInt = PeekChar();
                if (!comment && (char)charInt == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2)))))
                {
                    break;
                }
                if (!comment && (char)charInt == '<' && PeekChar(1) == '!' && PeekChar(2) == '-' && PeekChar(3) == '-')
                {
                    if (!_readCommentInText) break;
                    comment = true;
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    charInt = PeekChar();
                    isTextSeparator = false;
                }

                if (charInt == -1) break;
                GetChar();
                _stringBuilder.Append(_char);
                if (_char != ' ' && _char != '\t' && _char != '\r' && _char != '\n')
                    isTextSeparator = false;
                l = _stringBuilder.Length;
                if (comment && l >= 3 && _stringBuilder[l - 3] == '-' && _stringBuilder[l - 2] == '-' && _stringBuilder[l - 1] == '>')
                    comment = false;
            }
            _text = _stringBuilder.ToString();
            _text = HtmlCharCodes.TranslateCode(_text);
            _isTextSeparator = isTextSeparator;
        }

        private void ReadScript()
        {
            _stringBuilder.Remove(0, _stringBuilder.Length);
            while (true)
            {
                int iChar = PeekChar();
                if (iChar == -1) break;
                if ((char)iChar == '<' && PeekChar(1) == '/' && char.ToLower((char)PeekChar(2)) == 's' && char.ToLower((char)PeekChar(3)) == 'c' &&
                    char.ToLower((char)PeekChar(4)) == 'r' && char.ToLower((char)PeekChar(5)) == 'i' && char.ToLower((char)PeekChar(6)) == 'p' &&
                    char.ToLower((char)PeekChar(7)) == 't' && char.ToLower((char)PeekChar(8)) == '>')
                    break;
                GetChar();
                _stringBuilder.Append(_char);
            }
            _text = _stringBuilder.ToString();
            _text = HtmlCharCodes.TranslateCode(_text);
        }

        private bool NextIsMark()
        {
            char car;

            if (PeekChar(0) != '<') return false;
            int i = 1;
            if ((char)PeekChar(i) == '/')
            {
                i++;
                if (!PeekName(ref i)) return false;
                car = (char)PeekChar(i);
                if (car != '>') return false;
                return true;
            }
            else
            {
                if (!PeekName(ref i)) return false;
                car = (char)PeekChar(i);
                if (car == '>') return true;
                if (!char.IsSeparator(car)) return false;
                i++;
                PeekSeparator(ref i);
                if (!PeekName(ref i)) return false;
                PeekSeparator(ref i);
                car = (char)PeekChar(i);
                if (car != '=') return false;
                return true;
            }
        }

        private bool PeekName(ref int i)
        {
            while (true)
            {
                int charInt = PeekChar(i);
                if (charInt == -1) return false;
                char car = (char)charInt;
                if (car == '/' || car == '>' || car == '=') return true;
                if (char.IsControl(car)) return false;
                if (char.IsSeparator(car)) return true;
                if (char.IsPunctuation(car)) return false;
                if (char.IsSurrogate(car)) return false;
                if (char.IsSymbol(car)) return false;
                i++;
            }
        }

        private void PeekSeparator(ref int i)
        {
            while (true)
            {
                int charInt = PeekChar(i);
                if (charInt == -1) return;
                char car = (char)charInt;
                if (!char.IsSeparator(car)) return;
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
                    _column = 0;
                    _line++;
                }
                else if (_char == '\n')
                {
                    _column = 0;
                    if (_lastChar != '\r') _line++;
                }
                else
                    _column++;
            }
        }

        private void UnreadChar()
        {
            if (_posStringStream == 0)
                throw new HTMLReaderException("Error UnreadChar line {0} column {1}", _line, _column);
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
                throw new HTMLReaderException("Error PeekChar({0}) max = {1}", i, _stringStream.Capacity);
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
                    //int iRetry = 0;
                    //int iChar;
                    //while (true)
                    //{
                    //    try
                    //    {
                    //        iChar = gtr.Read();
                    //        break;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Exception exInner = cError.GetFirstException(ex);
                    //        AccessViolationException exAccess = exInner as AccessViolationException;
                    //        if (exAccess == null)
                    //            throw new HTMLReaderException("Error reading html stream line {0} column {1}", ex, giLine, giColumn);
                    //        if (iRetry++ == 9)
                    //            throw new HTMLReaderException("Error reading html stream line {0} column {1}", ex, giLine, giColumn);
                    //        cTrace.Trace("Error reading html stream line {0} column {1}", giLine, giColumn);
                    //        cTrace.Trace(cError.GetErrorMessage(ex, false, true));
                    //    }
                    //}
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
            //return _stringStream[_posStringStream + i];
            charInt = _stringStream[_posStringStream + i];
            if (__tracePeekChar && _traceHtmlReaderStreamWriter != null)
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
                    if (++_peekCharCount >= __maxPeekCharCount)
                    {
                        charText = (char)charInt;
                        throw new HTMLReaderException("pb.Web.HtmlReader.PeekChar : to much read of same character {0} {1} line {2} column {3} position {4}", charInt.zToHex(), !char.IsControl(charText) ? "\"" + charText + "\"" : "---", _line, _column, _lastPosPeekChar);
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


        //private void Open()
        //{
        //    _readInProgress = false;
        //    if (_urlPath != null)
        //    {
        //        if (_urlPath.StartsWith("http:") || _urlPath.StartsWith("file:"))
        //        {
        //            Http http = new Http(_urlPath);
        //            _textReader = http.WebStream;
        //        }
        //        else
        //        {
        //            _fs = new FileStream(_urlPath, FileMode.Open, FileAccess.Read, FileShare.Read, _bufferSize);
        //            _textReader = new StreamReader(_fs, _encoding);
        //        }
        //    }
        //}

        public override void Close()
        {
            if (_textReader != null && _closeTextReader)
            {
                _textReader.Close();
                _textReader = null;
            }
            _stringStream.Remove(0, _stringStream.Length);
            _posStringStream = 0;
            _stringBuilder.Remove(0, _stringBuilder.Length);
            CloseTraceHtmlReader();
            CloseExportHtml();
            _readInProgress = false;
        }
    }
}
