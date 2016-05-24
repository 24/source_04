using pb.Windows.Forms;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    partial class RunSourceForm
    {
        protected XtraGridControl _gridResult1;
        protected int _grid1ResultTabIndex = -1;
        protected DataGridViewControl _gridResult2;
        protected int _grid2ResultTabIndex = -1;
        protected DataGrid _gridResult3;
        protected int _grid3ResultTabIndex = -1;
        protected int _gridResultTabIndex = -1;
        protected TreeView _treeResult;
        protected int _treeResultTabIndex = -1;
        protected LogTextBox _logTextBox;
        protected int _messageResultTabIndex = -1;

        protected void CreateResultControls()
        {
            int resultTabIndex = 0;

            Panel panel = AddResultPanel("result 1");
            _gridResult1 = XtraGridControl.Create(dockStyle: DockStyle.Fill);
            panel.Controls.Add(_gridResult1);
            _grid1ResultTabIndex = resultTabIndex++;

            panel = AddResultPanel("result 2");
            _gridResult2 = DataGridViewControl.Create(dockStyle: DockStyle.Fill, showRowNumber: true);
            panel.Controls.Add(_gridResult2);
            _grid2ResultTabIndex = resultTabIndex++;

            panel = AddResultPanel("result 3");
            _gridResult3 = zForm.CreateDataGrid(dockStyle: DockStyle.Fill);
            panel.Controls.Add(_gridResult3);
            _grid3ResultTabIndex = resultTabIndex++;

            _gridResultTabIndex = _grid2ResultTabIndex;

            panel = AddResultPanel("result 4");
            _treeResult = new TreeView();
            _treeResult.Dock = DockStyle.Fill;
            panel.Controls.Add(_treeResult);
            _treeResultTabIndex = resultTabIndex++;

            panel = AddResultPanel("message");
            _logTextBox = LogTextBox.Create(dockStyle: DockStyle.Fill);
            panel.Controls.Add(_logTextBox);
            _messageResultTabIndex = resultTabIndex++;
        }

        protected void SelectGrid1ResultTab()
        {
            Control activeControl = this.ActiveControl;
            _resultTab.SelectedIndex = _grid1ResultTabIndex;
            this.ActiveControl = activeControl;
        }

        protected void SelectGridResultTab()
        {
            Control activeControl = this.ActiveControl;
            _resultTab.SelectedIndex = _gridResultTabIndex;
            this.ActiveControl = activeControl;
        }

        protected void SelectTreeResultTab()
        {
            Control activeControl = this.ActiveControl;
            _resultTab.SelectedIndex = _treeResultTabIndex;
            this.ActiveControl = activeControl;
        }

        protected void SelectMessageResultTab()
        {
            Control activeControl = this.ActiveControl;
            _resultTab.SelectedIndex = _messageResultTabIndex;
            this.ActiveControl = activeControl;
        }
    }
}
