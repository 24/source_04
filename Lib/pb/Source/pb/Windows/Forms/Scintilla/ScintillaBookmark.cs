using System.Drawing;
using ScintillaNET;

// Bookmark Lines https://github.com/jacobslusser/ScintillaNET/wiki/Bookmark-Lines

namespace pb.Windows.Forms
{
    public partial class ScintillaBookmark
    {
        private Scintilla _scintillaControl = null;
        //private int _bookmarkMargin = 1; // Conventionally the symbol margin
        private int _bookmarkMarker = 3; // Arbitrary. Any valid index would work.
        private const uint _bookmarkMarkerMask = 8;

        public ScintillaBookmark(Scintilla scintillaControl)
        {
            _scintillaControl = scintillaControl;
            InitScintillaControl();

            InitParentForm();
        }

        //public int BookmarkMargin { get { return _bookmarkMargin; } }

        private void InitScintillaControl()
        {
            // Bookmark Lines https://github.com/jacobslusser/ScintillaNET/wiki/Bookmark-Lines
            Margin margin = _scintillaControl.Margins[ScintillaMargin.Bookmark];
            margin.Width = 16;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            //margin.Mask = Marker.MaskAll;
            margin.Mask = _bookmarkMarkerMask;
            margin.Cursor = MarginCursor.Arrow;

            var marker = _scintillaControl.Markers[_bookmarkMarker];
            marker.Symbol = MarkerSymbol.Bookmark;
            marker.SetBackColor(Color.DeepSkyBlue);
            marker.SetForeColor(Color.Black);
        }

        public void SetBookmark()
        {
            SetBookmark(_scintillaControl.zGetCurrentLine());
        }

        public void SetBookmark(Line line)
        {
            uint mask = (uint)(1 << _bookmarkMarker);
            if ((line.MarkerGet() & mask) > 0)
            {
                // Remove existing bookmark
                line.MarkerDelete(_bookmarkMarker);
                //WriteMessage("remove bookmark at {0}", line.Index);
            }
            else
            {
                // Add bookmark
                line.MarkerAdd(_bookmarkMarker);
                //WriteMessage("add bookmark at {0}", line.Index);
            }
        }

        public void GotoPreviousBookmark()
        {
            int line = _scintillaControl.zGetCurrentLineNumber();
            var prevLine = _scintillaControl.Lines[line - 1].MarkerPrevious((uint)(1 << _bookmarkMarker));
            if (prevLine == -1)
                prevLine = _scintillaControl.Lines[_scintillaControl.Lines.Count - 1].MarkerPrevious((uint)(1 << _bookmarkMarker));
            if (prevLine != -1 && prevLine != line)
                _scintillaControl.Lines[prevLine].Goto();
        }

        public void GotoNextBookmark()
        {
            //var line = scintilla1.LineFromPosition(scintilla1.CurrentPosition);
            //var nextLine = scintilla1.Lines[++line].MarkerNext(1 << _bookmarkMarker);
            int line = _scintillaControl.zGetCurrentLineNumber();
            //WriteMessage("next bookmark from {0}", line);
            var nextLine = _scintillaControl.Lines[line + 1].MarkerNext((uint)(1 << _bookmarkMarker));
            if (nextLine == -1)
                nextLine = _scintillaControl.Lines[0].MarkerNext((uint)(1 << _bookmarkMarker));
            if (nextLine != -1 && nextLine != line)
                _scintillaControl.Lines[nextLine].Goto();
        }
    }
}
