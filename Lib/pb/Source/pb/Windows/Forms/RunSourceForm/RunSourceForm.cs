using System.Drawing;
using System.Windows.Forms;
using System;

namespace pb.Windows.Forms
{
    public partial class RunSourceForm : RunSourceFormBase
    {
        protected ToolStripMenuItem _menuFile;
        protected ToolStripMenuItem _menuOptions;
        protected FormKeyboard _formKeyboard;

        public RunSourceForm()
        {
            CreateMenu();
            CreateScintillaControl();
            CreateResultControls();
            CreateStatusBar();
            InitMenuScintilla();
            this.ClientSize = new Size(1060, 650);
            //this.InitializeForm();
            //this.KeyPreview = true;
            //SetKeyboardShortcuts();
            Try(SetKeyboardShortcuts);
        }

        private void CreateMenu()
        {
            _menuFile = zForm.CreateMenuItem("&File");
            _menuOptions = zForm.CreateMenuItem("&Options");
            this.MainMenuStrip.Items.AddRange(new ToolStripItem[] { _menuFile, _menuOptions });
        }

        private void SetKeyboardShortcuts()
        {
            _formKeyboard = new FormKeyboard(this);

            // ctrl-K + ctrl-K : SetBookmark
            _formKeyboard.SetMultipleKey(Keys.Control | Keys.K, Keys.Control | Keys.K, () => _source.ScintillaBookmark.SetBookmark());
            // ctrl-K + ctrl-P : GotoPreviousBookmark
            _formKeyboard.SetMultipleKey(Keys.Control | Keys.K, Keys.Control | Keys.P, () => _source.ScintillaBookmark.GotoPreviousBookmark());
            // ctrl-K + ctrl-N : GotoNextBookmark
            _formKeyboard.SetMultipleKey(Keys.Control | Keys.K, Keys.Control | Keys.N, () => _source.ScintillaBookmark.GotoNextBookmark());

            // ctrl-^ : GotoBraceMatch
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.Oem6, () => _source.ScintillaBrace.GotoBraceMatch());

            // ctrl-F : OpenFindForm
            _formKeyboard.SetSimpleKey(Keys.Control | Keys.F, () => _source.ScintillaFindText.OpenFindForm());
            // esc : HideFindForm
            _formKeyboard.SetSimpleKey(Keys.Escape, () => _source.ScintillaFindText.HideFindForm());
            // F3 : FindNext
            _formKeyboard.SetSimpleKey(Keys.F3, () => _source.ScintillaFindText.FindNext());
            // shift-F3
            _formKeyboard.SetSimpleKey(Keys.Shift | Keys.F3, () => _source.ScintillaFindText.FindPrevious());
        }

        private void Try(Action action, bool errorMessageBox = false)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.CurrentTrace.WriteError(ex);
                if (errorMessageBox)
                    zerrf.ErrorMessageBox(ex);
            }
        }
    }
}
