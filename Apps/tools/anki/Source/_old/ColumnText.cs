using pb;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace anki
{
    public class ColumnTextResult
    {
        public bool Success;
        public int ColumnPosition;
        //public int Line;
        public string Error;
    }

    public class ColumnTextManager
    {
        private int _minSpaceBeforeColumn = 4;
        private Regex _columnDetect = null;

        public ColumnTextManager(int minSpaceBeforeColumn = 4)
        {
            _minSpaceBeforeColumn = minSpaceBeforeColumn;
            _columnDetect = new Regex($@"\s{{{_minSpaceBeforeColumn}}}([A-Z])", RegexOptions.Compiled);
        }

        public ColumnTextResult FindColumn(IEnumerable<string> lines)
        {
            return new ColumnText(_columnDetect).FindColumn(lines);
        }

        public IEnumerable<ColumnText.ColumnInfo> Test_GetColumnInfos(IEnumerable<string> lines)
        {
            return new ColumnText(_columnDetect).Test_GetColumnInfos(lines);
        }
    }

    public class ColumnText
    {
        private int _maxHeaderLineCount = 2;
        private int _minHeaderEmptyLineCount = 2;
        private int _minColumnPosition = 50;
        private int _maxColumnPositionGap = 5;
        private Regex _columnDetect = null;

        private string _error = null;
        private int _columnPosition = -1;

        private string[] _lines = null;
        private IEnumerator<string> _linesEnumerator = null;
        private string _line = null;
        private int _lineIndex = -1;
        private bool _body = false;
        private int _headerLineCount = 0;
        private int _headerEmptyLineCount = 0;
        private ColumnInfo[] _columnInfos = null;

        public ColumnText(Regex columnDetect)
        {
            _columnDetect = columnDetect;
        }

        // détection de la colonne : commence par une majuscule et est précédé par n espaces
        public ColumnTextResult FindColumn(IEnumerable<string> lines)
        {
            _lines = lines.ToArray();
            _linesEnumerator = ((IEnumerable<string>)_lines).GetEnumerator();
            if (FindBody() && FindColumn())
            {
                CheckColumnInfos();
            }
            if (_error == null)
            {
                if (!_body)
                    _error = $"body not found line {_lineIndex + 1}";
            }
            // Line = _lineIndex
            return new ColumnTextResult { Success = _error == null, ColumnPosition = _columnPosition, Error = _error };
        }

        public IEnumerable<ColumnInfo> Test_GetColumnInfos(IEnumerable<string> lines)
        {
            _linesEnumerator = lines.GetEnumerator();
            if (FindBody())
                return GetColumnInfos();
            else
            {
                Trace.WriteLine("body not found");
                if (_error != null)
                    Trace.WriteLine(_error);
                return new ColumnInfo[0];
            }
        }

        private bool FindColumn()
        {
            //new ColumnTextManager(minSpaceBeforeColumn: 4).Test_GetColumnInfos(zFile.ReadAllLines(@"c:\pib\drive\google\valentin\UE5\UE5-01-anatomie\data\scan\test\UE5 - Fiche - Introduction à l'anatomie-page-015.txt"))
            //  .Where(columnInfo => columnInfo.ColumnPosition != -1).GroupBy(columnInfo => columnInfo.ColumnPosition).Select(group => new { ColumnPosition = group.Key, Count = group.Count() }).zTraceJson();
            //ColumnInfo[] columnInfos = GetColumnInfos().ToArray();
            //var groups = columnInfos.Where(columnInfo => columnInfo.ColumnPosition != -1).GroupBy(columnInfo => columnInfo.ColumnPosition)
            //    .Select(group => new { ColumnPosition = group.Key, Count = group.Count() });
            //int columnPosition = groups.Where(group => group.Count > 2).Min(group => group.ColumnPosition);
            _columnInfos = GetColumnInfos().ToArray();
            var groups = _columnInfos
                .Where(columnInfo => columnInfo.ColumnPosition != -1)
                .GroupBy(columnInfo => columnInfo.ColumnPosition)
                .Select(group => new { ColumnPosition = group.Key, Count = group.Count() })
                .Where(group => group.Count > 2)
                .ToArray();
            if (groups.Length == 0)
            {
                _error = "column not found";
                return false;
            }
            _columnPosition = groups.Min(group => group.ColumnPosition);
            return true;
        }

        private bool CheckColumnInfos()
        {
            bool first = true;
            int index = -1;
            foreach (ColumnInfo columnInfo in _columnInfos)
            {
                index++;
                if (first)
                {
                    if (columnInfo.ColumnPosition == _columnPosition)
                    {
                        if (index > _maxHeaderLineCount)
                        {
                            _error = $"to many header lines {index}";
                            return false;
                        }
                        first = false;
                    }
                }
                else
                {
                    if (columnInfo.ColumnPosition == -1)
                    {
                        if (columnInfo.Length > _columnPosition)
                        {
                            //_error = $"column not found line {columnInfo.Line}";
                            //return false;
                            if (!CheckLine(_lines[columnInfo.Line]))
                            {
                                Trace.WriteLine($"column not found line {columnInfo.Line + 1}");
                                Trace.WriteLine(_lines[columnInfo.Line]);
                                Trace.WriteLine(new string(' ', _columnPosition) + "^");
                            }
                        }
                    }
                    //else if (columnInfo.ColumnPosition < _columnPosition)
                    //{
                    //    throw new PBException($"logic problem column position {columnInfo.ColumnPosition} line {columnInfo.Line}");
                    //}
                    else if (columnInfo.ColumnPosition < _columnPosition || columnInfo.ColumnPosition > _columnPosition + _maxColumnPositionGap)
                    {
                        //_error = $"wrong column position {columnInfo.ColumnPosition} line {columnInfo.Line}";
                        //return false;
                        if (!CheckLine(_lines[columnInfo.Line]))
                        {
                            Trace.WriteLine($"wrong column position {columnInfo.ColumnPosition} line {columnInfo.Line + 1}");
                            Trace.WriteLine(_lines[columnInfo.Line]);
                            Trace.WriteLine(new string(' ', _columnPosition) + "^");
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckLine(string line)
        {
            char c = line[_columnPosition];
            if (c == ' ' || (c >= 'A' && c <= 'Z') || line[_columnPosition - 1] == ' ')
                return true;
            else
                return false;
        }

        public class ColumnInfo
        {
            public bool Found;
            public int ColumnPosition;
            public int Line;
            public int Length;
        }

        private IEnumerable<ColumnInfo> GetColumnInfos()
        {
            while (GetLine())
            {
                if (_line == "")
                    continue;
                Match match = _columnDetect.Match(_line);
                int index = -2;
                while (true)
                {
                    if (!match.Success)
                        break;
                    index = match.Groups[1].Index;
                    if (index >= _minColumnPosition)
                        break;
                    match = match.NextMatch();
                }
                if (match.Success)
                    yield return new ColumnInfo { Found = true, ColumnPosition = index, Line = _lineIndex, Length = _line.Length };
                else
                    yield return new ColumnInfo { Found = false, ColumnPosition = -1, Line = _lineIndex, Length = _line.Length };
            }
        }

        private bool FindBody()
        {
            while (GetLine())
            {
                if (_line != "")
                {
                    if (++_headerLineCount > _maxHeaderLineCount)
                    {
                        _error = $"to many header line {_headerLineCount} line {_lineIndex + 1}";
                        return false;
                    }
                    _headerEmptyLineCount = 0;
                }
                else
                {
                    if (++_headerEmptyLineCount == _minHeaderEmptyLineCount)
                    {
                        _body = true;
                        return true;
                    }
                }
            }
            _error = $"body not found : search {_minHeaderEmptyLineCount} empty lines";
            return false;
        }

        private bool GetLine()
        {
            if (_linesEnumerator.MoveNext())
            {
                _line = _linesEnumerator.Current;
                _lineIndex++;
                return true;
            }
            else
            {
                _line = null;
                return false;
            }
        }
    }
}
