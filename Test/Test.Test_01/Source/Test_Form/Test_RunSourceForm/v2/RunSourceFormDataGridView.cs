namespace Test.Test_Form.Test_RunSourceForm.v2
{
    partial class RunSourceForm
    {
        //private System.Windows.Forms.DataGridView _gridResult2;
        private void CreateDataGridView()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._gridResult2 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._gridResult2)).BeginInit();
            //this.tab_result2.Controls.Add(this._gridResult2);
            this._gridResult2.AllowUserToAddRows = false;
            this._gridResult2.AllowUserToDeleteRows = false;
            this._gridResult2.AllowUserToOrderColumns = true;
            this._gridResult2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dataGridViewCellStyle1.NullValue = "(null)";
            this._gridResult2.DefaultCellStyle = dataGridViewCellStyle1;
            this._gridResult2.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridResult2.Location = new System.Drawing.Point(0, 0);
            this._gridResult2.Name = "grid_result2";
            this._gridResult2.ReadOnly = true;
            this._gridResult2.Size = new System.Drawing.Size(1048, 364);
            this._gridResult2.TabIndex = 0;
            //this._gridResult2.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.grid_result2_RowPostPaint);
            ((System.ComponentModel.ISupportInitialize)(this._gridResult2)).EndInit();
        }
    }
}
