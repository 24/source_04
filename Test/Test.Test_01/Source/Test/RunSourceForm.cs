using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Test
{
    public partial class RunSourceForm : Form
    {
        public RunSourceForm()
        {
            InitializeComponent();
            initGrid();
            //tc_result.SelectedTab = tab_message;
            //ActiveControl = me_source;
        }

        private System.Windows.Forms.DataGridView _gridResult2;
        private void initGrid()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this._gridResult2 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this._gridResult2)).BeginInit();
            this.tab_result2.Controls.Add(this._gridResult2);
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

        private void RunSourceForm_Load(object sender, EventArgs e)
        {
        }

        private void RunSourceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void RunSourceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void fWRun_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void me_source_TextChanged(object sender, EventArgs e)
        {
        }

        private void m_new_Click(object sender, EventArgs e)
        {
        }

        private void m_open_Click(object sender, EventArgs e)
        {
        }

        private void m_save_Click(object sender, EventArgs e)
        {
        }

        private void m_save_as_Click(object sender, EventArgs e)
        {
        }

        private void m_execute_Click(object sender, EventArgs e)
        {
        }

        private void m_compile_Click(object sender, EventArgs e)
        {
        }

        private void m_update_Click(object sender, EventArgs e)
        {
        }

        private void m_quit_Click(object sender, EventArgs e)
        {
        }

        private void bt_execute_Click(object sender, EventArgs e)
        {
        }

        private void bt_pause_Click(object sender, EventArgs e)
        {
        }
    }
}
