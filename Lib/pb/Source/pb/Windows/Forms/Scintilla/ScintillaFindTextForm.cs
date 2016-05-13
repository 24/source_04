using System.Windows.Forms;
using ScintillaNET;

namespace pb.Windows.Forms
{
    public partial class ScintillaFindText
    {
        private Form _parentForm = null;
        private ScintillaFindForm _findForm = null;

        private void InitForm()
        {
            InitFindForm();
            //InitParentForm();
            InitScintillaControlEvent();
            ControlFindForm.Find(_scintillaControl, InitParentForm);
        }

        private void InitFindForm()
        {
            _findForm = new ScintillaFindForm();
            _findForm.SetFindParam = SetFindParam;
            _findForm.FindNext = FindNext;
        }

        private void InitParentForm(Form form)
        {
            form.KeyDown += ParentForm_KeyDown;
            //form.KeyPreview = true;
            _parentForm = form;
        }

        private void InitScintillaControlEvent()
        {
            //_scintillaControl.BeforeDelete += new EventHandler<BeforeModificationEventArgs>(scintillaControl_BeforeDelete);
            _scintillaControl.BeforeDelete += scintillaControl_BeforeDelete;
            //_scintillaControl.BeforeInsert += new EventHandler<BeforeModificationEventArgs>(scintillaControl_BeforeInsert);
            _scintillaControl.BeforeInsert += scintillaControl_BeforeInsert;
        }

        public void OpenFindForm()
        {
            string word = _scintillaControl.zGetCurrentWord();
            if (word != null)
                _findForm.SetText(word);
            _findForm.Show(_parentForm);
            //_findForm.IsDisposed;
            //_findForm.ShowDialog();
        }

        private void HideFindForm()
        {
            _findForm.Hide();
        }

        //private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    //_findForm.ForceClose();
        //}

        private void ParentForm_KeyDown(object sender, KeyEventArgs e)
        {
            // mabage ctrl+F, F3, shift+F3, Esc
            if (!e.Alt && e.Control && !e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.F:
                        OpenFindForm();
                        break;
                }
            }
            else if (!e.Alt && !e.Control && !e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        HideFindForm();
                        break;
                    case Keys.F3:
                        FindNext();
                        break;
                }
            }
            else if (!e.Alt && !e.Control && e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.F3:
                        FindPrevious();
                        break;
                }
            }
        }

        private void scintillaControl_BeforeDelete(object sender, BeforeModificationEventArgs e)
        {
            ScintillaTextModified();
        }

        private void scintillaControl_BeforeInsert(object sender, BeforeModificationEventArgs e)
        {
            ScintillaTextModified();
        }
    }
}
