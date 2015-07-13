using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;

namespace pb.old
{
    public class HtmlReader_v1 : IDisposable
	{
		public class HTMLReaderException : Exception
		{
			public HTMLReaderException(string sMessage) : base(sMessage){}
			public HTMLReaderException(string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm)){}
			public HTMLReaderException(Exception InnerException, string sMessage) : base(sMessage,InnerException){}
            public HTMLReaderException(Exception InnerException, string sMessage, params object[] oPrm) : base(string.Format(sMessage, oPrm), InnerException) { }
		}

        // source Html : soit une url http:... soit un fichier
		private string _urlPath = null;

        // stream du fichier Html
		private FileStream _fs = null;
		private int _bufferSize = 32000;  // taille buffer de FileStream
		private Encoding _encoding = Encoding.Default; // code caractère : Default = ANSI avec le code page du système

        // stream de lecture de la source Html fichier Html
        private TextReader _tr = null;

        // export du flux Html dans path
        private string _exportHtmlPath = null;
        // flux d'export Html
        private StreamWriter _exportHtmlStreamWriter = null;

        private bool _readCommentInText = false;

        // lecture du flux Html pour pouvoir faire des Peek()
		private StringBuilder _stringStream = new StringBuilder(100, 1000000);
        // position du prochain caractère à lire dans gsbStream
		private int _posStream = 0;

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

        public HtmlReader_v1(TextReader tr)
        {
			_tr = tr;
		}

        ///// <summary></summary>
        ///// <param name="Url_Path_Html">may be url, path or html</param>
        //public HtmlReader(string Url_Path_Html)
        //{
        //    _urlPath = Url_Path_Html;
        //}

        //public HtmlReader(Http http)
        //{
        //    //gStream = http.WebStream;
        //    _tr = http.WebStream;
        //}

        private void Init()
        {
            InitExportHtml();
            Open();
        }

        private void InitExportHtml()
        {
            if (_exportHtmlStreamWriter != null)
            {
                _exportHtmlStreamWriter.Close();
                _exportHtmlStreamWriter = null;
            }
            if (_exportHtmlPath != null)
            {
                FileStream fs = new FileStream(_exportHtmlPath, FileMode.Create, FileAccess.Write, FileShare.Read, _bufferSize);
                _exportHtmlStreamWriter = new StreamWriter(fs, _encoding);
            }
        }

        public void Dispose()
		{
			Close();
		}

        public string ExportHtmlPath { get { return _exportHtmlPath; } set { _exportHtmlPath = value; } }
        public bool ReadCommentInText { get { return _readCommentInText; } set { _readCommentInText = value; } }
        public Encoding encoding { get { return _encoding; } set { _encoding = value; } }
		public int Line { get { return _line; } }
		public int Column { get { return _column; } }
		public bool IsMarkBegin { get { return _isMarkBegin; } }
		public bool IsMarkEnd { get { return _isMarkEnd; } }
		public bool IsMarkBeginEnd { get { return _isMarkBeginEnd; } }
		public bool IsMarkInProgress { get { return _markInProgress; } }
		public bool IsProperty { get { return _isProperty; } }
		public bool IsText { get { return _isText; } }
        public bool IsTextSeparator { get { return _isTextSeparator; } }
		public bool IsComment { get { return _isComment; } }
        public bool IsDocType { get { return _isDocType; } }
		public bool IsScript { get { return _scriptMarkInProgress; } }
		public string MarkName { get { return _markName; } }
		public string PropertyName { get { return _propertyName; } }
		public string PropertyValue { get { return _propertyValue; } }
		public string PropertyQuote { get { return _propertyQuote; } }
		public string Text { get { return _text; } }
		public string Comment { get { return _comment; } }
        public string DocType { get { return _docType; } }
		public string Separator { get { return _separator; } }
		public bool HasValue { get { return _isProperty || _isText || _isComment || _isDocType; } }
		public string Value
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
        public bool Read()
		{
            if (!_readInProgress)
            {
                Init();
                _readInProgress = true;
            }
            bool bRet = false;
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
			int iChar = PeekChar();
			if (iChar == -1) return false;  // EOF
			char cChar = (char)iChar;
			while(true)
			{
				if (_markInProgress)
				{
                    if (ReadProperty())
                    {
                        if (_char == '>') _markInProgress = false;
                        bRet = true;
                        break;
                    }
                    else
                        _markInProgress = false;
                }
                if (cChar == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2)))))
                {
                        GetChar();
                        ReadMarkName();
                        if (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n')
                            _markInProgress = true;
                        else if (_char != '>')
                            UnreadChar();
                        bRet = true;
                        break;
                }
                else if (cChar == '<' && PeekChar(1) == '!' && PeekChar(2) == '-' && PeekChar(3) == '-')
                {
                    ReadComment();
                    bRet = true;
                    break;
                }
                else
                {
                    _isText = true;
                    if (_scriptMarkInProgress)
                        ReadScript();
                    else
                        ReadText();
                    bRet = true;
                    break;
                }
                //else if (cChar == '<')
                //{
                //    if (PeekChar(1) == '!' && PeekChar(2) == '-' && PeekChar(3) == '-')
                //    {
                //        ReadComment();
                //        bRet = true;
                //        break;
                //    }
                //    else
                //    {
                //        GetChar();
                //        ReadMarkName();
                //        if (gcChar == ' ' || gcChar == '\t' || gcChar == '\r' || gcChar == '\n')
                //            gbMarkInProgress = true;
                //        else if (gcChar != '>')
                //            UnreadChar();
                //        bRet = true;
                //        break;
                //    }
                //}
                //else
                //{
                //    gbText = true;
                //    if (gbScriptMarkInProgress)
                //        ReadScript();
                //    else
                //        ReadText();
                //    bRet = true;
                //    break;
                //}
            }
            return bRet;
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
        public void ReadWrite(StreamWriter swOut)
		{
			bool bMarkInProgress = false;
			while(Read())
			{
				if (bMarkInProgress)
				{
					if (IsProperty)
					{
						swOut.Write("{0}", PropertyName);
                        if (PropertyValue != null)
						    swOut.Write("={0}{1}{2}", PropertyQuote, PropertyValue, PropertyQuote);
                        swOut.Write(Separator);
                    }
					if (!IsMarkInProgress)
					{
						if (IsMarkBeginEnd) swOut.Write("/");
						swOut.Write(">");
						bMarkInProgress = false;
					}
				}
				else if (IsMarkBegin && !IsMarkInProgress)
					swOut.Write("<{0}{1}>", MarkName, Separator);
				else if (IsMarkBegin && IsMarkInProgress)
				{
					bMarkInProgress = true;
					swOut.Write("<{0}{1}", MarkName, Separator);
				}
				else if (IsMarkEnd)
					swOut.Write("</{0}{1}>", MarkName, Separator);
				else if (IsMarkBeginEnd)
					swOut.Write("<{0}{1}/>", MarkName, Separator);
				else if (IsText)
					swOut.Write(Text);
				else if (IsComment)
					swOut.Write(Comment);
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
            //gsMarkName = gsMarkName.Replace("!", "");
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
			while(true)
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
			while(true)
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
			while(true)
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
			bool bQuote = false;
			_propertyQuote = "";
			char cQuote = (char)PeekChar();
			if (cQuote == '"' || cQuote == '\'')
			{
				bQuote = true;
				_propertyQuote = cQuote.ToString();
				GetChar();
			}

			_stringBuilder.Remove(0, _stringBuilder.Length);
			while(true)
			{
				GetChar();
                if (_charInt == -1
                    || (bQuote && (_char == cQuote || ((_char == '"' || _char == '\'') && PeekChar() == '>')))
                    || (!bQuote && (_char == ' ' || _char == '\t' || _char == '\r' || _char == '\n'))
                    || _char == '>' || (_char == '/' && PeekChar() == '>')
                    )
                    break;
				_stringBuilder.Append(_char);
			}
			_propertyValue = _stringBuilder.ToString();
            _propertyValue = Http_v1.TranslateCode(_propertyValue);
        }

        private void ReadStringValue2()
        {
            bool bQuote = false;
            _propertyQuote = "";
            char cQuote = (char)PeekChar();
            if (cQuote == '"' || cQuote == '\'')
            {
                bQuote = true;
                _propertyQuote = cQuote.ToString();
                GetChar();
            }

            _stringBuilder.Remove(0, _stringBuilder.Length);

            bool bQuote2 = bQuote;
            int iChar;
            int i = 0, iEndQuote = -1;
            while ((iChar = PeekChar(i)) != -1)
            {
                char c = (char)iChar;
                if (bQuote2)
                {
                    if (c == cQuote)
                    {
                        iEndQuote = i;
                        bQuote2 = false;
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
                        for (; i >= 0 && (iEndQuote == -1 || i > iEndQuote); i--)
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

            if (bQuote && i >= 0 && (char)PeekChar(i) == cQuote) i--;

            for (; i >= 0; i--)
            {
                GetChar();
                _stringBuilder.Append(_char);
            }

            _propertyValue = _stringBuilder.ToString();
            _propertyValue = Http_v1.TranslateCode(_propertyValue);
        }

        private void ReadText()
		{
			int l;

            bool bIsTextSeparator = true;
			bool bComment = false;
			_stringBuilder.Remove(0, _stringBuilder.Length);
			while(true)
			{
				int iChar = PeekChar();
                if (!bComment && (char)iChar == '<' && (char.IsLetter((char)PeekChar(1)) || (PeekChar(1) == '/' && char.IsLetter((char)PeekChar(2)))))
                {
                    break;
                }
                if (!bComment && (char)iChar == '<' && PeekChar(1) == '!' && PeekChar(2) == '-' && PeekChar(3) == '-')
                {
                    if (!_readCommentInText) break;
                    bComment = true;
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    GetChar(); _stringBuilder.Append(_char);
                    iChar = PeekChar();
                    bIsTextSeparator = false;
                }

                if (iChar == -1) break;
                GetChar();
				_stringBuilder.Append(_char);
                if (_char != ' ' && _char != '\t' && _char != '\r' && _char != '\n')
                    bIsTextSeparator = false;
				l = _stringBuilder.Length;
				if (bComment && l >= 3 && _stringBuilder[l - 3] == '-' && _stringBuilder[l - 2] == '-' && _stringBuilder[l - 1] == '>')
					bComment = false;
			}
			_text = _stringBuilder.ToString();
            _text = Http_v1.TranslateCode(_text);
            _isTextSeparator = bIsTextSeparator;
		}

		private void ReadScript()
		{
			_stringBuilder.Remove(0, _stringBuilder.Length);
			while(true)
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
            _text = Http_v1.TranslateCode(_text);
        }

		private bool NextIsMark()
		{
			char cChar;

			if (PeekChar(0) != '<') return false;
			int i = 1;
			if ((char)PeekChar(i) == '/')
			{
				i++;
				if (!PeekName(ref i)) return false;
				cChar = (char)PeekChar(i);
				if (cChar != '>') return false;
				return true;
			}
			else
			{
				if (!PeekName(ref i)) return false;
				cChar = (char)PeekChar(i);
				if (cChar == '>') return true;
				if (!char.IsSeparator(cChar)) return false;
				i++;
				PeekSeparator(ref i);
				if (!PeekName(ref i)) return false;
				PeekSeparator(ref i);
				cChar = (char)PeekChar(i);
				if (cChar != '=') return false;
				return true;
			}
		}

		private bool PeekName(ref int i)
		{
			while(true)
			{
				int iChar = PeekChar(i);
				if (iChar == -1) return false;
				char cChar = (char)iChar;
				if (cChar == '/' || cChar == '>' || cChar == '=') return true;
				if (char.IsControl(cChar)) return false;
				if (char.IsSeparator(cChar)) return true;
				if (char.IsPunctuation(cChar)) return false;
				if (char.IsSurrogate(cChar)) return false;
				if (char.IsSymbol(cChar)) return false;
				i++;
			}
		}

		private void PeekSeparator(ref int i)
		{
			while(true)
			{
				int iChar = PeekChar(i);
				if (iChar == -1) return ;
				char cChar = (char)iChar;
				if (!char.IsSeparator(cChar)) return;
				i++;
			}
		}

		private void GetChar()
		{
			_charInt = ReadChar();
			_lastChar = _char;
			_char = '\0';
			if (_charInt != - 1)
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
            if (_posStream == 0) throw new HTMLReaderException("Error UnreadChar line {0} column {1}", _line, _column);
            _posStream--;
            _nbUnreadChar++;
        }

        private int ReadChar()
		{
			int iChar = PeekChar(0);
			_posStream++;
            if (_exportHtmlStreamWriter != null && iChar != -1)
            {
                if (_nbUnreadChar == 0) _exportHtmlStreamWriter.Write((char)iChar);
            }
            if (_nbUnreadChar > 0) _nbUnreadChar--;
			return iChar;
		}

		private int PeekChar()
		{
			return PeekChar(0);
		}

		private int PeekChar(int i)
		{
			if (i + 10 >= _stringStream.MaxCapacity) throw new HTMLReaderException("Error PeekChar({0}) max = {1}", i, _stringStream.Capacity);
            if (i >= _stringStream.MaxCapacity - _posStream)
            {
				for(int i1 = 0, i2 = _posStream; i2 < _stringStream.Length; i1++, i2++) _stringStream[i1] = _stringStream[i2];
				_stringStream.Remove(_stringStream.Length - _posStream, _posStream);
				_posStream = 0;
			}
			if (_posStream + i >= _stringStream.Length)
			{
				for(int i1 = _stringStream.Length; i1 <= _posStream + i; i1++)
				{
                    int iChar = _tr.Read();
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
					if (iChar == -1) break;
					char cChar = (char)iChar;
					if (cChar == '—')
					{
						cChar = '-';
						_stringStream.Append(cChar);
					}
					_stringStream.Append(cChar);
				}
				if (_posStream + i >= _stringStream.Length)
				{
					return -1;
				}
			}
			return _stringStream[_posStream + i];
		}

		private void Open()
		{
            _readInProgress = false;
            if (_urlPath != null)
            {
                if (_urlPath.StartsWith("http:") || _urlPath.StartsWith("file:"))
                {
                    //WebClient client = new WebClient();
                    //client.Headers.Add(HttpRequestHeader.UserAgent, "");
                    ////cTrace.StartNestedLevel("WebClient.OpenRead");
                    //Stream st = client.OpenRead(gsUrl_Path);
                    ////cTrace.StopLevel();
                    //gtr = new StreamReader(st, gEncoding);

                    Http_v1 http = new Http_v1(_urlPath);
                    //gtr = new StreamReader(http.WebStream, gEncoding);
                    _tr = http.WebStream;
                }
                else
                {
                    _fs = new FileStream(_urlPath, FileMode.Open, FileAccess.Read, FileShare.Read, _bufferSize);
                    _tr = new StreamReader(_fs, _encoding);
                }
            }
            //else if (gStream != null)
            //{
            //    gtr = new StreamReader(gStream, gEncoding);
            //}
        }

		public void Close()
		{
            if (_tr != null)
			{
				_tr.Close();
				_tr = null;
			}
			_fs = null;
			_urlPath = null;
			//gEncoding = Encoding.Default;
			_stringStream.Remove(0, _stringStream.Length);
			_posStream = 0;
			_stringBuilder.Remove(0, _stringBuilder.Length);
            if (_exportHtmlStreamWriter != null)
            {
                _exportHtmlStreamWriter.Close();
                _exportHtmlStreamWriter = null;
            }
            _readInProgress = false;
        }
	}
}
