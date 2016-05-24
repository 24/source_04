using pb.Windows.Forms;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceForm_v3
    {
        private ToolStripLabel _lineNumber;
        private ToolStripLabel _columnNumber;
        private ToolStripLabel _insertMode;

        private void CreateStatusBar()
        {
            //_bottomToolStrip
            //System.Windows.Forms.ToolStripButton
            //System.Windows.Forms.ToolStripControlHost
            //System.Windows.Forms.ToolStripDropDownItem
            //System.Windows.Forms.ToolStripLabel
            //System.Windows.Forms.ToolStripSeparator
            // Button, Label, SplitButton, DropDownButton, Separator, ComboBox, TextBox, ProgressBar
            // ToolStrip : ToolStripButton, ToolStripLabel, ToolStripSplitButton, ToolStripDropDownButton, ToolStripSeparator, ToolStripTextBox, ToolStripProgressBar
            _lineNumber = zForm.CreateToolStripLabel(width: 30);
            _columnNumber = zForm.CreateToolStripLabel(width: 30);
            _insertMode = zForm.CreateToolStripLabel(width: 30);
            _bottomToolStrip.Items.AddRange(new ToolStripItem[] {
                zForm.CreateToolStripLabelSeparator(100),
                zForm.CreateToolStripLabel("Line"),
                _lineNumber,
                zForm.CreateToolStripLabel("Col"),
                _columnNumber,
                zForm.CreateToolStripLabelSeparator(30),
                _insertMode
            });
        }
    }
}
