using pb.Windows.Forms;
using System;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceForm_v3
    {
        private ScintillaControl _source;

        //public ScintillaControl Source { get { return _source; } }

        private void CreateScintillaControl()
        {
            _source = ScintillaControl.Create(dockStyle: DockStyle.Fill);
            //EditPanel.Controls.Add(_source);
            _source.SetFont("Consolas", 10);
            _source.ConfigureLexerCpp();
            _source.SetTabIndent(2, 0, false);
            //_source.DisplayLineNumber(4);
            _source.TextChanged += source_TextChanged;
        }

        private void source_TextChanged(object sender, EventArgs e)
        {
            SetFileNotSaved();
        }

        private string GetCode()
        {
            return _source.SelectedText;
        }
    }
}
