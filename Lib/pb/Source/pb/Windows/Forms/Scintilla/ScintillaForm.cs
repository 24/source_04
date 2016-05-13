using ScintillaNET;

namespace pb.Windows.Forms
{
    public class ScintillaForm
    {
        private Scintilla _scintillaControl = null;
        private ScintillaBookmark _scintillaBookmark = null;
        private ScintillaCodeFolding _scintillaCodeFolding = null;
        private ScintillaBrace _scintillaBrace = null;
        private ScintillaFindText _scintillaFindText = null;

        public ScintillaForm(Scintilla scintillaControl)
        {
            _scintillaControl = scintillaControl;
            InitScintillaControl();
        }

        public ScintillaBookmark ScintillaBookmark { get { return _scintillaBookmark; } }
        public ScintillaFindText ScintillaFindText { get { return _scintillaFindText; } }

        private void InitScintillaControl()
        {
            _scintillaControl.zClearStyle();
            _scintillaControl.zClearCmdKeys();
            //_scintillaControl.zConfigureLexerCpp();
            //_scintillaControl.zSetTabIndent(2, 0, false);
            //_scintillaControl.zDisplayLineNumber(4);

            //_scintillaIndent = new ScintillaIndent(scintilla1);
            new ScintillaIndent(_scintillaControl);
            _scintillaBookmark = new ScintillaBookmark(_scintillaControl);
            //_scintillaCodeFolding = new ScintillaCodeFolding(_scintillaControl);
            //_scintillaBrace = new ScintillaBrace(_scintillaControl);
            _scintillaFindText = new ScintillaFindText(_scintillaControl);

            // indent parameters from SciTE Documentation http://www.scintilla.org/SciTEDoc.html
            // dont work 
            // indent.automatic, indent.opening, indent.closing, indent.maintain.filepattern
            //scintilla1.SetProperty("indent.automatic", "1");
            //scintilla1.SetProperty("indent.opening", "1");
            //scintilla1.SetProperty("indent.closing", "1");
            //scintilla1.SetProperty("indent.automatic", "0");
            //scintilla1.SetProperty("indent.opening", "0");
            //scintilla1.SetProperty("indent.closing", "0");
        }

        public void SetFont(string font, int fontSize)
        {
            _scintillaControl.zSetFont(font, fontSize);
        }

        public void ConfigureLexerCpp()
        {
            _scintillaControl.zConfigureLexerCpp();
            _scintillaCodeFolding = new ScintillaCodeFolding(_scintillaControl);
            _scintillaBrace = new ScintillaBrace(_scintillaControl);
        }

        public void SetTabIndent(int tabWidth, int identWidth, bool useTabs)
        {
            _scintillaControl.zSetTabIndent(tabWidth, identWidth, useTabs);
        }

        public void DisplayLineNumber(int length)
        {
            _scintillaControl.zDisplayLineNumber(length);
        }
    }
}
