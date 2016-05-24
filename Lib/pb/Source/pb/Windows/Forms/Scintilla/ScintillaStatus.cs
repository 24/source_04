using ScintillaNET;
using System;

namespace pb.Windows.Forms
{
    public class ScintillaStatus
    {
        private Scintilla _scintillaControl = null;
        private int _lastCaretPosition = -1;
        private bool _lastOvertype = false;

        public Action<int> PositionChange = null;
        public Action<bool> OvertypeChange = null;

        public ScintillaStatus(Scintilla scintillaControl)
        {
            _scintillaControl = scintillaControl;
            _scintillaControl.UpdateUI += scintillaControl_UpdateUI;
            _lastOvertype = !_scintillaControl.Overtype;
        }

        private void scintillaControl_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            int caretPosition = _scintillaControl.CurrentPosition;
            if (_lastCaretPosition != caretPosition)
            {
                _lastCaretPosition = caretPosition;
                if (PositionChange != null)
                    PositionChange(caretPosition);
            }
            bool overtype = _scintillaControl.Overtype;
            if (_lastOvertype != overtype)
            {
                _lastOvertype = overtype;
                if (OvertypeChange != null)
                    OvertypeChange(overtype);
            }
        }
    }
}
