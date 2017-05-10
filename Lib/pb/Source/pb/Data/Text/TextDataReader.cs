using pb.IO;
using pb.Text;
using System.Collections.Generic;

namespace pb.Data.Text
{
    public class TextData
    {
        public string File;
        public string Filename;
        public int ColumnNumber;
        public int LineNumber;
        public int Length;
        public string Line;
        //public NamedValues<ZValue> Values;
        public NamedValues<RegexValue<ZValue>> Values;
    }

    public class TextDataReader
    {
        private RegexValuesList _regexList = null;
        private bool _contiguousSearch = false;
        //private IEnumerable<string> _source = null;
        //private IEnumerable<string> _files = null;

        public bool ContiguousSearch { get { return _contiguousSearch; } set { _contiguousSearch = value; } }

        // RegexValuesList regexList
        //public TextDataReader(IEnumerable<string> files)
        //{
        //    //_regexList = regexList;
        //    _files = files;
        //}

        public void SetRegexList(RegexValuesList regexList)
        {
            _regexList = regexList;
        }

        public IEnumerable<TextData> Read(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                int lineNumber = 1;
                //Trace.WriteLine($"read file \"{file}\"");
                foreach (string line in zFile.ReadLines(file))
                {
                    FindText_v2 findText = _regexList.Find(line, contiguous: _contiguousSearch);
                    // Values = findText.GetValues()
                    yield return new TextData { File = file, Filename = zPath.GetFileName(file), ColumnNumber = findText.Success ? findText.MatchIndex + 1 : 0,
                        LineNumber = lineNumber, Length = findText.MatchLength, Line = line, Values = findText.GetRegexValues() };
                    while (findText.FindNext(contiguous: _contiguousSearch))
                    {
                        // Values = findText.GetValues()
                        yield return new TextData { File = file, Filename = zPath.GetFileName(file), ColumnNumber = findText.Success ? findText.MatchIndex + 1 : 0,
                            LineNumber = lineNumber, Length = findText.MatchLength, Line = line, Values = findText.GetRegexValues() };
                    }
                    lineNumber++;
                }
            }

        }
    }
}
