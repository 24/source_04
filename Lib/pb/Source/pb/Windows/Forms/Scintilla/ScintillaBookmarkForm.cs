using ScintillaNET;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public partial class ScintillaBookmark
    {
        //private Form _parentForm = null;
        private KeyEventArgs _previousKey = null;

        private void InitParentForm()
        {
            ControlFindForm.Find(_scintillaControl, InitForm);
            _scintillaControl.MarginClick += scintillaControl_MarginClick;
        }

        private void InitForm(Form form)
        {
            form.KeyDown += ParentForm_KeyDown;
            //form.KeyPreview = true;
        }

        private void scintillaControl_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == ScintillaMargin.Bookmark)
            {
                SetBookmark(_scintillaControl.zGetLineFromPosition(e.Position));
            }
        }

        private void ParentForm_KeyDown(object sender, KeyEventArgs e)
        {
            bool ctrlK = false;
            if (_previousKey != null && !_previousKey.Alt && _previousKey.Control && !_previousKey.Shift && _previousKey.KeyCode == Keys.K)
                ctrlK = true;
            _previousKey = null;
            if (ctrlK)
            {
                if (!e.Alt && e.Control && !e.Shift)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.K:
                            SetBookmark();
                            break;
                        case Keys.P:
                            GotoPreviousBookmark();
                            break;
                        case Keys.N:
                            GotoNextBookmark();
                            break;
                    }
                }
            }
            else
                _previousKey = e;
        }
    }
}
