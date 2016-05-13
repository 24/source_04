using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace pb.Windows.Forms
{
    public static class zDataGridView
    {
        public static DataGridView Create(string name = null, DockStyle dockStyle = DockStyle.None, int? x = null, int? y = null, int? width = null, int? height = null)
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

            DataGridView grid = new DataGridView();
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
            ((ISupportInitialize)grid).EndInit();
            return grid;
        }
    }
}
