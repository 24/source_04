using pb.Windows.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    partial class RunSourceForm
    {
        private static bool __test = false;
        private ToolStripLabel _lineNumber;
        private ToolStripLabel _columnNumber;
        private ToolStripLabel _insertMode;
        private ToolStripLabel _runType;
        private ToolStripLabel _runInit;
        private ToolStripLabel _running;

        public static bool Test { get { return __test; } set { __test = value; } }

        private void CreateStatusBar()
        {
            // run, run on main thread, run multiple, [without project], run init, running
            // ToolStrip : ToolStripButton, ToolStripLabel, ToolStripSplitButton, ToolStripDropDownButton, ToolStripSeparator, ToolStripTextBox, ToolStripProgressBar
            Color? backColor = null;
            if (__test)
                backColor = SystemColors.ControlDark;
            _lineNumber = zForm.CreateToolStripLabel(width: 30, backColor: backColor);
            //Trace.WriteLine("lineNumber label : Width {0} Height {1} AutoSize {2} Text {3}", _lineNumber.Size.Width, _lineNumber.Size.Height, _lineNumber.AutoSize, _lineNumber.Text);
            //Trace.WriteLine("lineNumber label : Margin.Bottom {0} Margin.Top {1}", _lineNumber.Margin.Bottom, _lineNumber.Margin.Top);
            //Trace.WriteLine("lineNumber label : Padding.Bottom {0} Padding.Top {1}", _lineNumber.Padding.Bottom, _lineNumber.Padding.Top);
            _columnNumber = zForm.CreateToolStripLabel(width: 30, textAlign: ContentAlignment.MiddleLeft, backColor: backColor);
            _insertMode = zForm.CreateToolStripLabel(width: 30, textAlign: ContentAlignment.MiddleLeft, backColor: backColor);
            _runType = zForm.CreateToolStripLabel(width: 200, textAlign: ContentAlignment.MiddleLeft, backColor: backColor);
            _runInit = zForm.CreateToolStripLabel(width: 10, textAlign: ContentAlignment.MiddleLeft, backColor: backColor);
            _running = zForm.CreateToolStripLabel(width: 18, textAlign: ContentAlignment.MiddleLeft, backColor: backColor);
            _bottomToolStrip.Items.AddRange(new ToolStripItem[] {
                zForm.CreateToolStripLabelSeparator(100),
                zForm.CreateToolStripLabel("Line"),
                _lineNumber,
                zForm.CreateToolStripLabel("Col"),
                _columnNumber,
                zForm.CreateToolStripLabelSeparator(30),
                _insertMode,
                zForm.CreateToolStripLabelSeparator(25),
                new ToolStripSeparator(),
                zForm.CreateToolStripLabelSeparator(25),
                _runType,
                zForm.CreateToolStripLabelSeparator(30),
                zForm.CreateToolStripLabel("run init"),
                _runInit,
                zForm.CreateToolStripLabelSeparator(30),
                zForm.CreateToolStripLabel("running"),
                _running
            });
            if (__test)
            {
                _bottomToolStrip.RenderMode = ToolStripRenderMode.System;
                UpdateStatusRunType("run on main thread withou project");
                UpdateStatusRunning(99);
                _runInit.Text = "x";
            }
        }

        protected void UpdateStatusPosition(int column, int line)
        {
            _lineNumber.Text = line.ToString();
            _columnNumber.Text = column.ToString();
        }

        protected void UpdateStatusInsertMode(bool insert)
        {
            _insertMode.Text = insert ? "INS" : "OVR";
        }

        protected void UpdateStatusRunType(string runType)
        {
            _runType.Text = runType;
        }

        protected void UpdateStatusRunInit(string runInit)
        {
            _runInit.Text = runInit;
        }

        protected void UpdateStatusRunning(int nb)
        {
            _running.Text = nb.ToString();
        }
    }
}
