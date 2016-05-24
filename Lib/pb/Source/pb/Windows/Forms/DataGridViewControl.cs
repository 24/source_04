using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public class DataGridViewControl : DataGridView
    {
        private bool _setGridMaxColumnsWidthAndRowsHeight = true;
        private int _maxWidth = 0;
        private int _maxHeight = 0;

        public bool GridMaxColumnsWidthAndRowsHeight { get { return _setGridMaxColumnsWidthAndRowsHeight; } set { _setGridMaxColumnsWidthAndRowsHeight = value; } }
        public int MaxWidth { get { return _maxWidth; } set { _maxWidth = value; } }
        public int MaxHeight { get { return _maxHeight; } set { _maxHeight = value; } }

        public void SetDataSource(DataTable dt)
        {
            this.SuspendLayout();
            this.DataSource = null;
            if (this.DataSource != dt.DefaultView)
            {
                this.DataSource = dt.DefaultView;
                this.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                this.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
                SetGridMaxColumnsWidthAndRowsHeight();
                this.AllowUserToOrderColumns = true;
                this.ReadOnly = false;
            }
            this.ResumeLayout();
        }

        private void SetGridMaxColumnsWidthAndRowsHeight()
        {
            if (!_setGridMaxColumnsWidthAndRowsHeight)
                return;
            if (_maxWidth != 0)
            {
                foreach (DataGridViewColumn col in this.Columns)
                {
                    if (col.Width > _maxWidth)
                        col.Width = _maxWidth;
                }
            }
            if (_maxHeight != 0)
            {
                foreach (DataGridViewRow row in this.Rows)
                {
                    if (row.Height > _maxHeight)
                        row.Height = _maxHeight;
                }
            }
        }

        private void DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Paint the row number on the row header.
            // The using statement automatically disposes the brush.
            using (SolidBrush brush = new SolidBrush(this.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, brush, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }

        public static DataGridViewControl Create(string name = null, DockStyle dockStyle = DockStyle.None, int? x = null, int? y = null, int? width = null, int? height = null, bool showRowNumber = false)
        {
            DataGridViewCellStyle viewCellStyle = new DataGridViewCellStyle();
            viewCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            viewCellStyle.BackColor = SystemColors.Window;
            viewCellStyle.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            viewCellStyle.ForeColor = SystemColors.ControlText;
            viewCellStyle.SelectionBackColor = SystemColors.Highlight;
            viewCellStyle.SelectionForeColor = SystemColors.HighlightText;
            viewCellStyle.WrapMode = DataGridViewTriState.False;
            viewCellStyle.NullValue = "(null)";

            DataGridViewControl grid = new DataGridViewControl();
            ((ISupportInitialize)grid).BeginInit();
            grid.Name = name;
            grid.Dock = dockStyle;
            Point? point = zForm.GetPoint(x, y);
            if (point != null)
                grid.Location = (Point)point;
            Size? size = zForm.GetSize(width, height);
            if (size != null)
                grid.Size = (Size)size;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToOrderColumns = true;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.DefaultCellStyle = viewCellStyle;
            grid.ReadOnly = true;
            //grid.TabIndex = 0;
            if (showRowNumber)
                grid.RowPostPaint += grid.DataGridView_RowPostPaint;
            ((ISupportInitialize)grid).EndInit();
            return grid;
        }
    }
}
