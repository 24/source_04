using pb.Windows.Forms;
using System;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    partial class RunSourceForm
    {
        protected ScintillaControl _source;
        protected ToolStripMenuItem _menuViewSourceLineNumber;

        private void CreateScintillaControl()
        {
            _source = ScintillaControl.Create(dockStyle: DockStyle.Fill);
            _source.SetFont("Consolas", 10);
            _source.ConfigureLexerCpp();
            _source.SetTabIndent(2, 0, false);
            _source.ScintillaStatus.PositionChange += PositionChange;
            _source.ScintillaStatus.OvertypeChange += OvertypeChange;
            //_source.TextChanged += source_TextChanged;
            _editPanel.Controls.Add(_source);

            _menuViewSourceLineNumber = zForm.CreateMenuItem("View source line number", checkOnClick: true, @checked: true, onClick: menuViewSourceLineNumber_Click);
            //_menuOptions.DropDownItems.Add(_menuViewSourceLineNumber);
        }

        private void InitMenuScintilla()
        {
            ScintillaViewLineNumber(_menuViewSourceLineNumber.Checked);
        }

        private void ScintillaViewLineNumber(bool view)
        {
            int nbChar = 0;
            if (view)
                nbChar = 4;
            _source.DisplayLineNumber(nbChar);
        }

        private void PositionChange(int position)
        {
            UpdateStatusPosition(_source.GetColumn(position) + 1, _source.LineFromPosition(position) + 1);
        }

        private void OvertypeChange(bool overtype)
        {
            UpdateStatusInsertMode(!overtype);
        }

        private void menuViewSourceLineNumber_Click(object sender, EventArgs e)
        {
            ScintillaViewLineNumber(_menuViewSourceLineNumber.Checked);
        }
    }
}
