using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Data;
using System.Globalization;

namespace pb.Windows.Forms
{
    public class XtraGridControl : GridControl
    {
        public XtraGridControl()
        {
            cGrid.Culture = CultureInfo.CurrentUICulture;
        }

        public void SetDataSource(DataTable dt, string xmlParam)
        {
            cGrid.GridSetDataSource(this, dt, xmlParam);
        }

        public void ClearDataSource()
        {
            cGrid.GridClearDataSource(this);
        }

        public static XtraGridControl Create(string name = null, DockStyle dockStyle = DockStyle.None, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            XtraGridControl grid = new XtraGridControl();
            GridView gridView = new GridView();

            ((ISupportInitialize)grid).BeginInit();
            ((ISupportInitialize)gridView).BeginInit();

            grid.MainView = gridView;
            grid.Name = name;
            grid.Dock = dockStyle;
            grid.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Point? point = zForm.GetPoint(x, y);
            if (point != null)
                grid.Location = (Point)point;
            Size? size = zForm.GetSize(width, height);
            if (size != null)
                grid.Size = (Size)size;
            //grid.TabIndex = 0;

            gridView.GridControl = grid;

            ((ISupportInitialize)grid).EndInit();
            ((ISupportInitialize)gridView).EndInit();

            return grid;
        }
    }
}
