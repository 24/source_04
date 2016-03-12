using System;
using System.IO;
using System.Text;

namespace pb.IO
{
    //public class CharInfo
    //{
    //    public char Char = '\0';
    //    public int Line = 1;
    //    public int Column = 1;
    //}

    public class CharStreamReader
    {
        // source stream
        private TextReader _textReader = null;
        //private bool _closeTextReader = false;

        private Func<char, string> _translateChar = null;

        // export stream
        private StreamWriter _exportStream = null;

        // buffer used to peek character
        private StringBuilder _stringStream = new StringBuilder(100, 1000000);
        // position of next character to read in _stringStream
        private int _posStringStream = 0;
        private int _posSourceStream = 0;

        // control PeekChar(0) pour éviter une boucle sans fin
        private int _maxPeekCharCount = 200;
        private int _lastPosPeekChar = -1;
        private int _peekCharCount = 0;

        // Compte le nombre de caractère Unread pour ne pas exporter 2 fois ces caractères dans l'export du flux Html
        private int _nbUnreadChar = 0;

        // line number of current character
        private int _line = 1;
        // column number of current character
        private int _column = 0;

        //private char _char = '\0';
        //private int _charInt = 0;
        private char _lastChar1 = '\0';
        private char _lastChar2 = '\0';

        public CharStreamReader(TextReader textReader)
        {
            _textReader = textReader;
            //_closeTextReader = closeTextReader;
        }

        //public void Dispose()
        //{
        //    Close();
        //}

        //public void Close()
        //{
        //    //if (_textReader != null && _closeTextReader)
        //    //{
        //    //    _textReader.Close();
        //    //    _textReader = null;
        //    //}
        //    _stringStream.Remove(0, _stringStream.Length);
        //    _posStringStream = 0;
        //}

        public Func<char, string> TranslateChar { get { return _translateChar; } set { _translateChar = value; } }
        public int Line { get { return _line; } }
        public int Column { get { return _column; } }

        // read next char
        public int ReadChar(int nb = 1)
        {
            //_charInt = ReadChar();
            //_lastChar = _char;
            //_char = '\0';
            //if (_charInt != -1)
            //{
            //    _char = (char)_charInt;
            //    if (_char == '\r')
            //    {
            //        _column = 1;
            //        _line++;
            //    }
            //    else if (_char == '\n')
            //    {
            //        _column = 1;
            //        if (_lastChar != '\r') _line++;
            //    }
            //    else
            //        _column++;
            //}

            if (nb <= 0)
                throw new PBException("read char nb must be greater or equal to 1");

            int code = 0;
            for (int i = 0; i < nb; i++)
            {
                if (_nbUnreadChar == 0)
                {
                    if (_lastChar1 == '\r')
                    {
                        _column = 1;
                        _line++;
                    }
                    else if (_lastChar1 == '\n')
                    {
                        _column = 1;
                        if (_lastChar2 != '\r')
                            _line++;
                    }
                    else
                        _column++;
                }

                code = _ReadChar();

                if (code != -1 && _nbUnreadChar == 0)
                {
                    _lastChar2 = _lastChar1;
                    _lastChar1 = (char)code;
                }
            }

            return code;
        }

        //private void UnreadChar()
        //{
        //    // problem unread char dont update _line and _column
        //    if (_posStringStream == 0)
        //        throw new PBException("unable to unread char line {0} column {1}", _line, _column);
        //    _posStringStream--;
        //    _nbUnreadChar++;
        //}

        private int _ReadChar()
        {
            int code = PeekChar();

            //if (code == -1)
            //    return -1;
            if (code != -1)
            {
                _posStringStream++;

                if (_exportStream != null)
                {
                    if (_nbUnreadChar == 0)
                        _exportStream.Write((char)code);
                }

                if (_nbUnreadChar > 0)
                    _nbUnreadChar--;
            }

            return code;
        }

        //public int PeekChar()
        //{
        //    return PeekChar(0);
        //}

        public int PeekChar(int index = 0)
        {
            if (index + 10 >= _stringStream.MaxCapacity)
                throw new PBException("unable to peek char overflow of string stream max capacity PeekChar({0}) max capacity = {1}", index, _stringStream.Capacity);
            if (index >= _stringStream.MaxCapacity - _posStringStream)
            {
                for (int i1 = 0, i2 = _posStringStream; i2 < _stringStream.Length; i1++, i2++)
                    _stringStream[i1] = _stringStream[i2];
                _stringStream.Remove(_stringStream.Length - _posStringStream, _posStringStream);
                _posSourceStream += _posStringStream;
                _posStringStream = 0;
            }
            int code;
            char car;
            if (_posStringStream + index >= _stringStream.Length)
            {
                for (int i1 = _stringStream.Length; i1 <= _posStringStream + index; i1++)
                {
                    code = _textReader.Read();
                    if (code == -1)
                        break;
                    car = (char)code;
                    //if (car == '—')
                    //{
                    //    car = '-';
                    //    _stringStream.Append(car);
                    //}
                    //// '\x07' dans http://www.telecharger-magazine.com/livres/502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html   c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\detail\0\livres_502-tout-sur-les-lgumes-lencyclopdie-des-aliments.html
                    //// '\x1F' dans http://www.telecharger-magazine.com/actualit/117-grand-guide-2014-du-seo.html  c:\pib\dev_data\exe\runsource\download\sites\telecharger-magazine.com\cache\detail\0\actualit_117-grand-guide-2014-du-seo.html
                    //// remplace '\x07' '\x1F' par un blanc
                    //else if (code == 7 || code == 0x1F)
                    //{
                    //    car = ' ';
                    //}

                    bool translate = false;
                    if (_translateChar != null)
                    {
                        string translated = _translateChar(car);
                        if (translated != null)
                        {
                            translate = true;
                            foreach (char tcar in translated)
                                _stringStream.Append(tcar);
                        }
                    }

                    if (!translate)
                        _stringStream.Append(car);
                }
                if (_posStringStream + index >= _stringStream.Length)
                {
                    return -1;
                }
            }
            code = _stringStream[_posStringStream + index];

            //if (_tracePeekChar && _traceHtmlReaderStreamWriter != null)
            //{
            //    car = (char)code;
            //    _traceHtmlReaderStreamWriter.WriteLine("      peek char i {0} line {1} column {2} pos {3,7} code {4} char {5}", i, _line, _column, _posSourceStream + _posStringStream + i, code.zToHex(), !char.IsControl(car) ? "\"" + car + "\"" : "---");
            //}

            // control PeekChar(0) pour éviter une boucle sans fin
            if (index == 0)
            {
                int pos = _posSourceStream + _posStringStream + index;
                if (pos == _lastPosPeekChar)
                {
                    if (++_peekCharCount >= _maxPeekCharCount)
                    {
                        car = (char)code;
                        throw new PBException("PeekChar : to much read of same character {0} {1} line {2} column {3} position {4}", code.zToHex(), !char.IsControl(car) ? "\"" + car + "\"" : "---", _line, _column, _lastPosPeekChar);
                    }
                }
                else
                {
                    _lastPosPeekChar = pos;
                    _peekCharCount = 1;
                }
            }
            return code;
        }
    }
}
