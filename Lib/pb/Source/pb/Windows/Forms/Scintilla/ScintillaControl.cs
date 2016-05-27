using ScintillaNET;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class ScintillaControl : Scintilla
    {
        private ScintillaStatus _scintillaStatus = null;
        private ScintillaBookmark _scintillaBookmark = null;
        private ScintillaCodeFolding _scintillaCodeFolding = null;
        private ScintillaBrace _scintillaBrace = null;
        private ScintillaFindText _scintillaFindText = null;

        public ScintillaControl()
        {
            InitScintillaControl();
        }

        public ScintillaStatus ScintillaStatus { get { return _scintillaStatus; } }
        public ScintillaBookmark ScintillaBookmark { get { return _scintillaBookmark; } }
        public ScintillaCodeFolding ScintillaCodeFolding { get { return _scintillaCodeFolding; } }
        public ScintillaBrace ScintillaBrace { get { return _scintillaBrace; } }
        public ScintillaFindText ScintillaFindText { get { return _scintillaFindText; } }

        private void InitScintillaControl()
        {
            this.zClearStyle();
            this.zClearCmdKeys();
            new ScintillaIndent(this);
            _scintillaStatus = new ScintillaStatus(this);
            _scintillaBookmark = new ScintillaBookmark(this);
            _scintillaFindText = new ScintillaFindText(this);
        }

        public void SetFont(string font, int fontSize)
        {
            this.zSetFont(font, fontSize);
        }

        public void ConfigureLexerCpp()
        {
            this.zConfigureLexerCpp();
            _scintillaCodeFolding = new ScintillaCodeFolding(this);
            _scintillaBrace = new ScintillaBrace(this);
        }

        public void SetTabIndent(int tabWidth, int identWidth, bool useTabs)
        {
            this.zSetTabIndent(tabWidth, identWidth, useTabs);
        }

        public void DisplayLineNumber(int length)
        {
            this.zDisplayLineNumber(length);
        }

        public static ScintillaControl Create(string name = null, DockStyle dockStyle = DockStyle.None, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            ScintillaControl scintilla = new ScintillaControl();
            scintilla.Name = name;
            scintilla.Dock = dockStyle;
            //scintilla.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            //scintilla.ScrollBars = ScrollBars.Both;
            Point? point = zForm.GetPoint(x, y);
            if (point != null)
                scintilla.Location = (Point)point;
            Size? size = zForm.GetSize(width, height);
            if (size != null)
                scintilla.Size = (Size)size;
            return scintilla;
        }
    }
}
