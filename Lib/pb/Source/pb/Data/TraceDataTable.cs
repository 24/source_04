using pb.IO;
using System.Data;
using System.IO;
using System.Text;

namespace pb.Data
{
    public enum FrameChar
    {
        TopLeft = 0,
        Top,
        TopMiddle,
        TopRight,
        Left,
        Blank,
        Middle,
        Right,
        SeparatorLeft,
        Separator,
        SeparatorMiddle,
        SeparatorRight,
        BottomLeft,
        Bottom,
        BottomMiddle,
        BottomRight
    }

    public class TraceDataTable
    {
        private DataTable _dt;
        private int[] _columnsWidth;
        private char[] _frameChars = {
            '╔', '═','╤', '╗',
            '║', ' ','│', '║',
            '╟', '─','┼', '╢',
            '╚', '═','╧', '╝'
        };

        /*
          ╔═══════════╤═══════════╤═══════════╤═══════════╗
          ║           │           │           │           ║
          ║           │           │           │           ║
          ║           │           │           │           ║
          ║           │           │           │           ║
          ╟───────────┼───────────┼───────────┼───────────╢
          ║           │           │           │           ║
          ║           │           │           │           ║
          ║           │           │           │           ║
          ║           │           │           │           ║
          ╚═══════════╧═══════════╧═══════════╧═══════════╝
        */

        public TraceDataTable(DataTable dt)
        {
            _dt = dt;
        }

        private void _Trace(StreamWriter sw)
        {
            GetColumnsWidth();

            char separatorLeft = _frameChars[(int)FrameChar.SeparatorLeft];
            char separator = _frameChars[(int)FrameChar.Separator];
            char separatorMiddle = _frameChars[(int)FrameChar.SeparatorMiddle];
            char separatorRight = _frameChars[(int)FrameChar.SeparatorRight];

            char left = _frameChars[(int)FrameChar.Left];
            char blank = _frameChars[(int)FrameChar.Blank];
            char middle = _frameChars[(int)FrameChar.Middle];
            char right = _frameChars[(int)FrameChar.Right];

            sw.WriteLine(DrawLine(_frameChars[(int)FrameChar.TopLeft], _frameChars[(int)FrameChar.Top], _frameChars[(int)FrameChar.TopMiddle], _frameChars[(int)FrameChar.TopRight]));
            sw.WriteLine(DrawHeaderLine(left, blank, middle, right));

            //bool firstRow = true;
            int rowNumber = 1;
            foreach (DataRow row in _dt.Rows)
            {
                //if (firstRow)
                //{
                //    sw.WriteLine(DrawLine(_frameChars[(int)FrameChar.TopLeft], _frameChars[(int)FrameChar.Top], _frameChars[(int)FrameChar.TopMiddle], _frameChars[(int)FrameChar.TopRight]));
                //    firstRow = false;
                //}
                //else
                //    sw.WriteLine(DrawLine(separatorLeft, separator, separatorMiddle, separatorRight));
                sw.WriteLine(DrawLine(separatorLeft, separator, separatorMiddle, separatorRight));
                sw.WriteLine(DrawDataLine(row, rowNumber++, left, blank, middle, right));
            }
            sw.WriteLine(DrawLine(_frameChars[(int)FrameChar.BottomLeft], _frameChars[(int)FrameChar.Bottom], _frameChars[(int)FrameChar.BottomMiddle], _frameChars[(int)FrameChar.BottomRight]));
        }

        private string DrawLine(char left, char blank, char middle, char right)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (int width in _columnsWidth)
            {
                if (first)
                {
                    sb.Append(left);
                    first = false;
                }
                else
                    sb.Append(middle);
                sb.Append(new string(blank, width + 2));
            }
            sb.Append(right);
            return sb.ToString();
        }

        private string DrawHeaderLine(char left, char blank, char middle, char right)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(left);
            sb.Append(new string(' ', _columnsWidth[0] + 2));

            //bool first = true;
            int columnIndex = 1;
            foreach (DataColumn column in _dt.Columns)
            {
                //if (first)
                //{
                //    sb.Append(left);
                //    first = false;
                //}
                //else
                //    sb.Append(middle);
                sb.Append(middle);
                sb.Append(' ');
                string s = column.Caption;
                sb.Append(s);
                sb.Append(new string(' ', _columnsWidth[columnIndex++] - s.Length + 1));
            }
            sb.Append(right);
            return sb.ToString();
        }

        private string DrawDataLine(DataRow row, int rowNumber, char left, char blank, char middle, char right)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(left);
            sb.Append(' ');
            string s = rowNumber.ToString();
            sb.Append(s);
            sb.Append(new string(' ', _columnsWidth[0] - s.Length + 1));

            //bool first = true;
            int columnIndex = 1;
            foreach (DataColumn column in _dt.Columns)
            {
                //if (first)
                //{
                //    sb.Append(left);
                //    first = false;
                //}
                //else
                //    sb.Append(middle);
                sb.Append(middle);
                sb.Append(' ');
                s = row[column].ToString();
                sb.Append(s);
                sb.Append(new string(' ', _columnsWidth[columnIndex++] - s.Length + 1));
            }
            sb.Append(right);
            return sb.ToString();
        }

        private void GetColumnsWidth()
        {
            _columnsWidth = new int[_dt.Columns.Count + 1];
            _columnsWidth[0] = _dt.Rows.Count.ToString().Length;
            int iColumn = 1;
            foreach (DataColumn column in _dt.Columns)
            {
                int width = column.Caption.Length;
                foreach (DataRow row in _dt.Rows)
                {
                    int l = row[column].ToString().Length;
                    if (l > width)
                        width = l;
                }
                _columnsWidth[iColumn++] = width;
            }
        }

        public static void Trace(DataTable dt, string file)
        {
            using (FileStream fs = zFile.OpenWrite(file))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                    new TraceDataTable(dt)._Trace(sw);
            }
        }

        public static void Trace(DataTable dt, StreamWriter sw)
        {
            new TraceDataTable(dt)._Trace(sw);
        }
    }
}
