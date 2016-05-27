using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;

// Brace Matching https://github.com/jacobslusser/ScintillaNET/wiki/Brace-Matching

namespace pb.Windows.Forms
{
    public enum BraceType
    {
        Parenthesis = 1,    // ()
        Bracket,            // []
        CurlyBrace,         // {}
        LessGreater         // <>
    }

    public class BraceInfo
    {
        public int Position;
        public BraceType BraceType;
        public bool OpenBrace;
    }

    public class ScintillaBrace
    {
        private Scintilla _scintillaControl = null;
        //private Form _parentForm = null;
        private int _lastCaretPosition = -1;

        public ScintillaBrace(Scintilla scintillaControl)
        {
            _scintillaControl = scintillaControl;

            _scintillaControl.Styles[Style.BraceLight].BackColor = Color.LightGray;
            _scintillaControl.Styles[Style.BraceLight].ForeColor = Color.BlueViolet;
            _scintillaControl.Styles[Style.BraceBad].ForeColor = Color.Red;

            _scintillaControl.UpdateUI += scintillaControl_UpdateUI;
            //ControlFindForm.Find(_scintillaControl, InitForm);
        }

        //private void InitForm(Form form)
        //{
        //    form.KeyDown += ParentForm_KeyDown;
        //    //form.KeyPreview = true;
        //}

        private BraceInfo GetBraceInfo(int position)
        {
            BraceType braceType = BraceType.Parenthesis;
            bool openBrace = false;
            switch ((char)_scintillaControl.GetCharAt(position))
            {
                case '(':
                    braceType = BraceType.Parenthesis;
                    openBrace = true;
                    break;
                case ')':
                    braceType = BraceType.Parenthesis;
                    openBrace = false;
                    break;
                case '[':
                    braceType = BraceType.Bracket;
                    openBrace = true;
                    break;
                case ']':
                    braceType = BraceType.Bracket;
                    openBrace = false;
                    break;
                case '{':
                    braceType = BraceType.CurlyBrace;
                    openBrace = true;
                    break;
                case '}':
                    braceType = BraceType.CurlyBrace;
                    openBrace = false;
                    break;
                case '<':
                    braceType = BraceType.LessGreater;
                    openBrace = true;
                    break;
                case '>':
                    braceType = BraceType.LessGreater;
                    openBrace = false;
                    break;
                default:
                    return null;
            }
            return new BraceInfo { BraceType = braceType, OpenBrace = openBrace, Position = position };
        }

        private BraceInfo GetCurrentBraceInfo()
        {
            // check if current character or character at left is a brace an return it's position
            int position = _scintillaControl.CurrentPosition;
            if (position > 0)
            {
                BraceInfo braceInfo = GetBraceInfo(position - 1);
                if (braceInfo != null)
                    return braceInfo;
            }
            if (position != -1)
                return GetBraceInfo(position);
            return null;
        }

        public void GotoBraceMatch()
        {
            BraceInfo braceInfo = GetCurrentBraceInfo();
            if (braceInfo != null)
            {
                int bracePosition = _scintillaControl.BraceMatch(braceInfo.Position);
                if (bracePosition != -1)
                {
                    if (braceInfo.OpenBrace)
                        bracePosition++;
                    _scintillaControl.GotoPosition(bracePosition);
                }
            }
        }

        private void scintillaControl_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            // Has the caret changed position?
            var caretPosition = _scintillaControl.CurrentPosition;
            if (_lastCaretPosition != caretPosition)
            {
                _lastCaretPosition = caretPosition;
                BraceInfo braceInfo = GetCurrentBraceInfo();

                if (braceInfo != null)
                {
                    int bracePosition2 = _scintillaControl.BraceMatch(braceInfo.Position);
                    if (bracePosition2 == Scintilla.InvalidPosition)
                        _scintillaControl.BraceBadLight(braceInfo.Position);
                    else
                        _scintillaControl.BraceHighlight(braceInfo.Position, bracePosition2);
                }
                else
                    _scintillaControl.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
            }
        }

        //private void ParentForm_KeyDown(object sender, KeyEventArgs e)
        //{
        //    // ctrl+^
        //    if (!e.Alt && e.Control && !e.Shift && e.KeyCode == Keys.Oem6)
        //        GotoBraceMatch();
        //}
    }
}
