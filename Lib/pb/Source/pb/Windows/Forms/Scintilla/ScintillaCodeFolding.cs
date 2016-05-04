using ScintillaNET;
using System.Drawing;

// Automatic Code Folding https://github.com/jacobslusser/ScintillaNET/wiki/Automatic-Code-Folding
// Using folding in scintilla http://sphere.sourceforge.net/flik/docs/scintilla-folding.html

namespace pb.Windows.Forms
{
    public class ScintillaCodeFolding
    {
        private Scintilla _scintillaControl = null;
        //private int _codeFoldingMargin = 2;

        public ScintillaCodeFolding(Scintilla scintillaControl)
        {
            _scintillaControl = scintillaControl;
            InitScintillaControl();
        }

        private void InitScintillaControl()
        {
            // Instruct the lexer to calculate folding
            _scintillaControl.SetProperty("fold", "1");
            // folds blank lines
            //_scintillaControl.SetProperty("fold.compact", "1");
            _scintillaControl.SetProperty("fold.comment", "1");
            _scintillaControl.SetProperty("fold.preprocessor", "1");

            // fold.by.indentation
            // from Scintillua http://foicica.com/scintillua/manual.html
            //   Scintillua adds dynamic Lua LPeg lexers to Scintilla

            // Configure a margin to display folding symbols
            Margin margin = _scintillaControl.Margins[ScintillaMargin.CodeFolding];
            margin.Type = MarginType.Symbol;
            margin.Mask = Marker.MaskFolders;
            //margin.Mask = Marker.MaskAll;
            margin.Sensitive = true;
            margin.Width = 20;


            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                _scintillaControl.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                _scintillaControl.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            _scintillaControl.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            _scintillaControl.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            _scintillaControl.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            _scintillaControl.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            _scintillaControl.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            _scintillaControl.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            _scintillaControl.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            _scintillaControl.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }
    }
}
