using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceForm_copy
    {
        private GridControl _gridResult1;
        private GridView _gridView1;

        private void CreateXtraGrid()
        {
            _gridResult1 = new GridControl();
            _gridView1 = new GridView();

            ((ISupportInitialize)(_gridResult1)).BeginInit();
            ((ISupportInitialize)(_gridView1)).BeginInit();

            //tab_result.Controls.Add(_gridResult1);

            // 
            // _gridResult1
            // 
            _gridResult1.Dock = DockStyle.Fill;
            _gridResult1.EmbeddedNavigator.Name = "";
            _gridResult1.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            _gridResult1.Location = new Point(3, 3);
            _gridResult1.MainView = _gridView1;
            _gridResult1.Name = "_gridResult1";
            _gridResult1.Size = new Size(1042, 358);
            _gridResult1.TabIndex = 0;
            //_gridResult1.Text = "gridControl1";

            // 
            // gridView1
            // 
            _gridView1.GridControl = _gridResult1;
            _gridView1.Name = "_gridView1";

            ((ISupportInitialize)(_gridResult1)).EndInit();
            ((ISupportInitialize)(_gridView1)).EndInit();

            tab_result1.Controls.Add(_gridResult1);
        }
    }
}
