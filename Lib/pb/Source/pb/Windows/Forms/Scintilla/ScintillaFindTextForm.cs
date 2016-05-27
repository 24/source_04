using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public partial class ScintillaFindText
    {
        //private Form _parentForm = null;

        //private void InitForm()
        //{
        //    //InitFindForm();
        //    //InitScintillaControlEvent();
        //    ControlFindForm.Find(_scintillaControl, InitParentForm);
        //}

        //private void InitParentForm(Form form)
        //{
        //    form.KeyDown += ParentForm_KeyDown;
        //    //form.KeyPreview = true;
        //    _parentForm = form;
        //}

        private void ParentForm_KeyDown(object sender, KeyEventArgs e)
        {
            // manage ctrl+F, F3, shift+F3, Esc
            if (!e.Alt && e.Control && !e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.F:
                        // ctrl-F
                        OpenFindForm();
                        break;
                }
            }
            else if (!e.Alt && !e.Control && !e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        // esc
                        HideFindForm();
                        break;
                    case Keys.F3:
                        // F3
                        FindNext();
                        break;
                }
            }
            else if (!e.Alt && !e.Control && e.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.F3:
                        // shift-F3
                        FindPrevious();
                        break;
                }
            }
        }

    }
}
