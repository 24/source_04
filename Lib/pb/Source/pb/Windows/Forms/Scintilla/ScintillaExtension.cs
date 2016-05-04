using System;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;

// todo :
// ok - open find form with owner, dont solve bug
// ok - put current word in find form
// ok - generate status messages
//    - add options in find form
// ok - key enter in find form
// ok - manage indentation of new line
//    - pb d'indentation avec copier coller d'une ligne

// SciTE Documentation http://www.scintilla.org/SciTEDoc.html
// Automatic Syntax Highlighting https://github.com/jacobslusser/ScintillaNET/wiki/Automatic-Syntax-Highlighting

namespace pb.Windows.Forms
{
    public static class ScintillaMargin
    {
        public const int LineNumber = 0;
        public const int Bookmark = 1;
        public const int CodeFolding = 2;
    }

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

    public static class ScintillaExtension
    {
        private const int SCI_SETLINEINDENTATION = 2126;
        private const int SCI_GETLINEINDENTATION = 2127;

        public static void zClearStyle(this Scintilla scintilla)
        {
            scintilla.StyleResetDefault();
            scintilla.StyleClearAll();
        }

        public static void zSetFont(this Scintilla scintilla, string font, int fontSize)
        {
            scintilla.Styles[Style.Default].Font = font;
            scintilla.Styles[Style.Default].Size = fontSize;
            scintilla.StyleClearAll();
        }

        public static void zSetTabIndent(this Scintilla scintilla, int tabWidth, int identWidth, bool useTabs)
        {
            // The tab size in characters.
            scintilla.TabWidth = tabWidth;
            // The indentation size in characters or 0 to make it the same as the tab width.
            scintilla.IndentWidth = identWidth;
            // Determines whether indentation allows tab characters or purely space characters.
            scintilla.UseTabs = useTabs;
        }

        public static void zClearCmdKeys(this Scintilla scintilla)
        {
            // Key Bindings https://github.com/jacobslusser/ScintillaNET/wiki/Key-Bindings
            // keep ctrl-A : select all, ctrl-Y : redo, ctrl-Z : undo
            // keep ctrl-C, ctrl-U, ctrl-V, ctrl-X, 
            scintilla.ClearCmdKey(Keys.Control | Keys.B);
            scintilla.ClearCmdKey(Keys.Control | Keys.D);
            scintilla.ClearCmdKey(Keys.Control | Keys.E);
            scintilla.ClearCmdKey(Keys.Control | Keys.F);
            scintilla.ClearCmdKey(Keys.Control | Keys.G);
            scintilla.ClearCmdKey(Keys.Control | Keys.H);
            scintilla.ClearCmdKey(Keys.Control | Keys.I);
            scintilla.ClearCmdKey(Keys.Control | Keys.J);
            scintilla.ClearCmdKey(Keys.Control | Keys.K);
            scintilla.ClearCmdKey(Keys.Control | Keys.L);
            scintilla.ClearCmdKey(Keys.Control | Keys.M);
            scintilla.ClearCmdKey(Keys.Control | Keys.N);
            scintilla.ClearCmdKey(Keys.Control | Keys.O);
            scintilla.ClearCmdKey(Keys.Control | Keys.P);
            scintilla.ClearCmdKey(Keys.Control | Keys.Q);
            scintilla.ClearCmdKey(Keys.Control | Keys.R);
            scintilla.ClearCmdKey(Keys.Control | Keys.S);
            scintilla.ClearCmdKey(Keys.Control | Keys.T);
            scintilla.ClearCmdKey(Keys.Control | Keys.W);
            scintilla.ClearCmdKey(Keys.Control | Keys.Oem6);     // ctrl+^

            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.A);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.B);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.C);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.D);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.E);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.F);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.G);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.H);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.I);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.J);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.K);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.L);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.M);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.N);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.O);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.P);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.Q);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.R);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.S);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.T);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.U);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.V);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.W);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.X);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.Y);
            scintilla.ClearCmdKey(Keys.Control | Keys.Shift | Keys.Z);
        }

        // length : number of character to display line number. ex : 4 => 9999 lines, 0 to disable view
        public static void zDisplayLineNumber(this Scintilla scintilla, int length)
        {
            // Displaying Line Numbers https://github.com/jacobslusser/ScintillaNET/wiki/Displaying-Line-Numbers
            //scintilla1.Margins[0].Width = 16;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            int width = 0;
            if (length > 0)
                width = scintilla.TextWidth(Style.LineNumber, new string('9', length + 1)) + padding;
            scintilla.Margins[ScintillaMargin.LineNumber].Width = width;
        }

        public static void zConfigureLexerCpp(this Scintilla scintilla)
        {
            // Configure the CPP (C#) lexer styles
            scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;

            // Selecting a Lexer
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while");
            scintilla.SetKeywords(1, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void");
        }

        public static string zGetCurrentWord(this Scintilla scintilla)
        {
            int position = scintilla.CurrentPosition;
            // position where a word starts, searching backward
            int wordStart = scintilla.WordStartPosition(position, true);
            // WordEndPosition return position of first blank character
            int wordEnd = scintilla.WordEndPosition(position, true);
            return wordStart != wordEnd ? scintilla.GetTextRange(wordStart, wordEnd - wordStart): null;
        }

        public static int zGetCurrentLineNumber(this Scintilla scintilla)
        {
            return scintilla.LineFromPosition(scintilla.CurrentPosition);
        }

        public static Line zGetCurrentLine(this Scintilla scintilla)
        {
            return scintilla.Lines[scintilla.zGetCurrentLineNumber()];
        }

        public static Line zGetLineFromPosition(this Scintilla scintilla, int position)
        {
            return scintilla.Lines[scintilla.LineFromPosition(position)];
        }

        public static int zGetLineIndent(this Scintilla scintilla, int line)
        {
            return (scintilla.DirectMessage(SCI_GETLINEINDENTATION, new IntPtr(line), IntPtr.Zero).ToInt32());
        }

        public static void zSetLineIndent(this Scintilla scintilla, int line, int indent)
        {
            scintilla.DirectMessage(SCI_SETLINEINDENTATION, new IntPtr(line), new IntPtr(indent));
        }
    }
}
