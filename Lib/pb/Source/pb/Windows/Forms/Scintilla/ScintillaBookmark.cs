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

            //InitParentForm();
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

            _scintillaControl.MarginClick += scintillaControl_MarginClick;
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
            int prevLine = -1;
            if (line > 0)
                prevLine = _scintillaControl.Lines[line - 1].MarkerPrevious((uint)(1 << _bookmarkMarker));
            if (prevLine == -1)
                prevLine = _scintillaControl.Lines[_scintillaControl.Lines.Count - 1].MarkerPrevious((uint)(1 << _bookmarkMarker));
            if (prevLine != -1 && prevLine != line)
                _scintillaControl.Lines[prevLine].Goto();
        }

        public void GotoNextBookmark()
        {
            int line = _scintillaControl.zGetCurrentLineNumber();
            //Trace.WriteLine("bookmark : GotoNextBookmark()");
            int nextLine = -1;
            if (line + 1 < _scintillaControl.Lines.Count)
            {
                //Trace.WriteLine("bookmark : next bookmark from line {0}", line);
                nextLine = _scintillaControl.Lines[line + 1].MarkerNext((uint)(1 << _bookmarkMarker));
                //Trace.WriteLine("bookmark : nextLine {0}", nextLine);
            }
            if (nextLine == -1)
            {
                //Trace.WriteLine("bookmark : next bookmark from line 0");
                nextLine = _scintillaControl.Lines[0].MarkerNext((uint)(1 << _bookmarkMarker));
                //Trace.WriteLine("bookmark : nextLine {0}", nextLine);
            }
            if (nextLine != -1 && nextLine != line)
            {
                //Trace.WriteLine("bookmark : goto line {0}", nextLine);
                _scintillaControl.Lines[nextLine].Goto();
            }
            //Trace.WriteLine();
        }

        private void scintillaControl_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == ScintillaMargin.Bookmark)
            {
                SetBookmark(_scintillaControl.zGetLineFromPosition(e.Position));
            }
        }

    }
}
