using ScintillaNET;

// ScintillaNET - Auto Code Indenting   https://gist.github.com/JohnyMac/f2910192987a73a52ab4
// How to add automatic indention. #35  https://github.com/jacobslusser/ScintillaNET/issues/35

namespace pb.Windows.Forms
{
    public class ScintillaIndent
    {
        private Scintilla _scintillaControl = null;
        private int _tabWidth = 0;

        public ScintillaIndent(Scintilla scintillaControl)
        {
            _scintillaControl = scintillaControl;
            InitScintillaControl();
            _tabWidth = _scintillaControl.TabWidth;
        }

        private void InitScintillaControl()
        {
            _scintillaControl.CharAdded += scintillaControl_CharAdded;
            _scintillaControl.InsertCheck += scintillaControl_InsertCheck;
        }

        private void scintillaControl_InsertCheck(object sender, InsertCheckEventArgs e)
        {
            if ((e.Text.EndsWith("\r") || e.Text.EndsWith("\n")))
            {
                //var curLine = scintilla1.LineFromPosition(e.Position);
                //var curLineText = scintilla1.Lines[curLine].Text;

                //var indent = Regex.Match(curLineText, @"^\s*");
                //e.Text += indent.Value; // Add indent following "\r\n"

                // Current line end with bracket?
                //if (Regex.IsMatch(curLineText, @"{\s*$"))
                //    e.Text += '\t'; // Add tab

                int indent = _scintillaControl.zGetLineIndent(_scintillaControl.zGetCurrentLineNumber());
                int position = _scintillaControl.CurrentPosition - 1;
                if (position >= 0)
                {
                    char c = (char)_scintillaControl.GetCharAt(position);
                    if (c == '{')
                        indent += _tabWidth;
                }
                e.Text += new string(' ', indent); // Add indent following "\r\n"
            }
        }

        private void scintillaControl_CharAdded(object sender, CharAddedEventArgs e)
        {
            if (e.Char == '}')
            {
                int line = _scintillaControl.zGetCurrentLineNumber();
                int indent = _scintillaControl.zGetLineIndent(line);
                indent -= _tabWidth;
                if (indent < 0)
                    indent = 0;
                _scintillaControl.zSetLineIndent(line, indent);
            }
        }
    }
}
