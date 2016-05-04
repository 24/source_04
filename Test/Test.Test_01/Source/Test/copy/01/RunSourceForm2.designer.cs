namespace Test
{
    partial class RunSourceForm2
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunSourceForm));
            this.pan_top = new System.Windows.Forms.Panel();
            //this.me_source = new DevExpress.XtraEditors.MemoEdit();
            this.bt_execute = new System.Windows.Forms.Button();
            this.pan_button = new System.Windows.Forms.Panel();
            this.bt_pause = new System.Windows.Forms.Button();
            this.lb_progress_label = new System.Windows.Forms.Label();
            this.pb_progress_bar = new System.Windows.Forms.ProgressBar();
            this.split_top = new System.Windows.Forms.Splitter();
            this.tc_result = new System.Windows.Forms.TabControl();
            //this.tab_result = new System.Windows.Forms.TabPage();
            //this.grid_result = new DevExpress.XtraGrid.GridControl();
            //this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            //this.tab_result2 = new System.Windows.Forms.TabPage();
            //this.tab_result3 = new System.Windows.Forms.TabPage();
            //this.grid_result3 = new System.Windows.Forms.DataGrid();
            //this.tab_result4 = new System.Windows.Forms.TabPage();
            //this.tree_result = new System.Windows.Forms.TreeView();
            this.tab_message = new System.Windows.Forms.TabPage();
            //this.tb_message = new System.Windows.Forms.TextBox();
            this.pan_top.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.me_source.Properties)).BeginInit();
            this.pan_button.SuspendLayout();
            this.tc_result.SuspendLayout();
            //this.tab_result.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.grid_result)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            //this.tab_result2.SuspendLayout();
            //this.tab_result3.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.grid_result3)).BeginInit();
            //this.tab_result4.SuspendLayout();
            this.tab_message.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan_top
            // 
            //this.pan_top.Controls.Add(this.me_source);
            this.pan_top.Controls.Add(this.bt_execute);
            this.pan_top.Controls.Add(this.pan_button);
            this.pan_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_top.Location = new System.Drawing.Point(0, 24);
            this.pan_top.Name = "pan_top";
            this.pan_top.Size = new System.Drawing.Size(1056, 197);
            this.pan_top.TabIndex = 0;
            // 
            // me_source
            // 
            //this.me_source.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.me_source.EditValue = "";
            //this.me_source.Location = new System.Drawing.Point(0, 35);
            //this.me_source.Name = "me_source";
            // 
            // 
            // 
            //this.me_source.Properties.Style = new DevExpress.Utils.ViewStyle("ControlStyle", null, new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))), "", DevExpress.Utils.StyleOptions.StyleEnabled, true, false, false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Utils.VertAlignment.Center, null, System.Drawing.SystemColors.Window, System.Drawing.SystemColors.WindowText);
            //this.me_source.Properties.WordWrap = false;
            //this.me_source.Size = new System.Drawing.Size(1056, 162);
            //this.me_source.TabIndex = 4;
            // 
            // bt_execute
            // 
            this.bt_execute.Location = new System.Drawing.Point(3, 3);
            this.bt_execute.Name = "bt_execute";
            this.bt_execute.Size = new System.Drawing.Size(75, 23);
            this.bt_execute.TabIndex = 1;
            this.bt_execute.Text = "&Run";
            this.bt_execute.UseVisualStyleBackColor = true;
            // 
            // pan_button
            // 
            this.pan_button.Controls.Add(this.bt_pause);
            this.pan_button.Controls.Add(this.lb_progress_label);
            this.pan_button.Controls.Add(this.pb_progress_bar);
            this.pan_button.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_button.Location = new System.Drawing.Point(0, 0);
            this.pan_button.Name = "pan_button";
            this.pan_button.Size = new System.Drawing.Size(1056, 35);
            this.pan_button.TabIndex = 2;
            // 
            // bt_pause
            // 
            this.bt_pause.Location = new System.Drawing.Point(84, 3);
            this.bt_pause.Name = "bt_pause";
            this.bt_pause.Size = new System.Drawing.Size(75, 23);
            this.bt_pause.TabIndex = 5;
            this.bt_pause.Text = "&Pause";
            this.bt_pause.UseVisualStyleBackColor = true;
            // 
            // lb_progress_label
            // 
            this.lb_progress_label.AutoSize = true;
            this.lb_progress_label.Location = new System.Drawing.Point(624, 1);
            this.lb_progress_label.Name = "lb_progress_label";
            this.lb_progress_label.Size = new System.Drawing.Size(47, 13);
            this.lb_progress_label.TabIndex = 1;
            this.lb_progress_label.Text = "progress";
            this.lb_progress_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_progress_bar
            // 
            this.pb_progress_bar.Location = new System.Drawing.Point(622, 16);
            this.pb_progress_bar.Name = "pb_progress_bar";
            this.pb_progress_bar.Size = new System.Drawing.Size(422, 16);
            this.pb_progress_bar.TabIndex = 0;
            // 
            // split_top
            // 
            this.split_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.split_top.Location = new System.Drawing.Point(0, 221);
            this.split_top.Name = "split_top";
            this.split_top.Size = new System.Drawing.Size(1056, 3);
            this.split_top.TabIndex = 1;
            this.split_top.TabStop = false;
            // 
            // tc_result
            // 
            this.tc_result.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            //this.tc_result.Controls.Add(this.tab_result);
            //this.tc_result.Controls.Add(this.tab_result2);
            //this.tc_result.Controls.Add(this.tab_result3);
            //this.tc_result.Controls.Add(this.tab_result4);
            this.tc_result.Controls.Add(this.tab_message);
            this.tc_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc_result.Location = new System.Drawing.Point(0, 224);
            this.tc_result.Name = "tc_result";
            this.tc_result.SelectedIndex = 0;
            this.tc_result.Size = new System.Drawing.Size(1056, 390);
            this.tc_result.TabIndex = 2;
            // 
            // tab_result
            // 
            //this.tab_result.Controls.Add(this.grid_result);
            //this.tab_result.Location = new System.Drawing.Point(4, 4);
            //this.tab_result.Name = "tab_result";
            //this.tab_result.Padding = new System.Windows.Forms.Padding(3);
            //this.tab_result.Size = new System.Drawing.Size(1048, 364);
            //this.tab_result.TabIndex = 0;
            //this.tab_result.Text = "Results";
            //this.tab_result.UseVisualStyleBackColor = true;
            // 
            // grid_result
            // 
            //this.grid_result.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // 
            // 
            //this.grid_result.EmbeddedNavigator.Name = "";
            //this.grid_result.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.grid_result.Location = new System.Drawing.Point(3, 3);
            //this.grid_result.MainView = this.gridView1;
            //this.grid_result.Name = "grid_result";
            //this.grid_result.Size = new System.Drawing.Size(1042, 358);
            //this.grid_result.TabIndex = 0;
            //this.grid_result.Text = "gridControl1";
            // 
            // gridView1
            // 
            //this.gridView1.GridControl = this.grid_result;
            //this.gridView1.Name = "gridView1";
            // 
            // tab_result2
            // 
            //this.tab_result2.Controls.Add(this.grid_result2);
            //this.tab_result2.Location = new System.Drawing.Point(4, 4);
            //this.tab_result2.Name = "tab_result2";
            //this.tab_result2.Size = new System.Drawing.Size(1048, 364);
            //this.tab_result2.TabIndex = 2;
            //this.tab_result2.Text = "Results";
            //this.tab_result2.UseVisualStyleBackColor = true;
            //// 
            //// grid_result2
            //// 
            //this.grid_result2.AllowUserToAddRows = false;
            //this.grid_result2.AllowUserToDeleteRows = false;
            //this.grid_result2.AllowUserToOrderColumns = true;
            //this.grid_result2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            //dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            //dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            //dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            //dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            //dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            //this.grid_result2.DefaultCellStyle = dataGridViewCellStyle1;
            //this.grid_result2.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.grid_result2.Location = new System.Drawing.Point(0, 0);
            //this.grid_result2.Name = "grid_result2";
            //this.grid_result2.ReadOnly = true;
            //this.grid_result2.Size = new System.Drawing.Size(1048, 364);
            //this.grid_result2.TabIndex = 0;
            //this.grid_result2.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.grid_result2_RowPostPaint);
            // 
            // tab_result3
            // 
            //this.tab_result3.Controls.Add(this.grid_result3);
            //this.tab_result3.Location = new System.Drawing.Point(4, 4);
            //this.tab_result3.Name = "tab_result3";
            //this.tab_result3.Size = new System.Drawing.Size(1048, 364);
            //this.tab_result3.TabIndex = 3;
            //this.tab_result3.Text = "Results";
            //this.tab_result3.UseVisualStyleBackColor = true;
            // 
            // grid_result3
            // 
            //this.grid_result3.DataMember = "";
            //this.grid_result3.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.grid_result3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.grid_result3.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            //this.grid_result3.Location = new System.Drawing.Point(0, 0);
            //this.grid_result3.Name = "grid_result3";
            //this.grid_result3.Size = new System.Drawing.Size(1048, 364);
            //this.grid_result3.TabIndex = 0;
            // 
            // tab_result4
            // 
            //this.tab_result4.Controls.Add(this.tree_result);
            //this.tab_result4.Location = new System.Drawing.Point(4, 4);
            //this.tab_result4.Name = "tab_result4";
            //this.tab_result4.Size = new System.Drawing.Size(1048, 364);
            //this.tab_result4.TabIndex = 4;
            //this.tab_result4.Text = "Results";
            //this.tab_result4.UseVisualStyleBackColor = true;
            // 
            // tree_result
            // 
            //this.tree_result.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.tree_result.Location = new System.Drawing.Point(0, 0);
            //this.tree_result.Name = "tree_result";
            //this.tree_result.Size = new System.Drawing.Size(1048, 364);
            //this.tree_result.TabIndex = 0;
            // 
            // tab_message
            // 
            //this.tab_message.Controls.Add(this.tb_message);
            this.tab_message.Location = new System.Drawing.Point(4, 4);
            this.tab_message.Name = "tab_message";
            this.tab_message.Padding = new System.Windows.Forms.Padding(3);
            this.tab_message.Size = new System.Drawing.Size(1048, 364);
            this.tab_message.TabIndex = 1;
            this.tab_message.Text = "Messages";
            this.tab_message.UseVisualStyleBackColor = true;
            // 
            // tb_message
            // 
            //this.tb_message.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.tb_message.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.tb_message.Location = new System.Drawing.Point(3, 3);
            //this.tb_message.Multiline = true;
            //this.tb_message.Name = "tb_message";
            //this.tb_message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            //this.tb_message.Size = new System.Drawing.Size(1042, 358);
            //this.tb_message.TabIndex = 0;
            //this.tb_message.WordWrap = false;
            // 
            // fWRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 614);
            this.Controls.Add(this.tc_result);
            this.Controls.Add(this.split_top);
            this.Controls.Add(this.pan_top);
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "fWRun";
            this.Text = "Run source 2";
            this.pan_top.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.me_source.Properties)).EndInit();
            this.pan_button.ResumeLayout(false);
            this.pan_button.PerformLayout();
            this.tc_result.ResumeLayout(false);
            //this.tab_result.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.grid_result)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            //this.tab_result2.ResumeLayout(false);
            //this.tab_result3.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.grid_result3)).EndInit();
            //this.tab_result4.ResumeLayout(false);
            this.tab_message.ResumeLayout(false);
            this.tab_message.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.Button bt_execute;
        private System.Windows.Forms.Panel pan_button;
        private System.Windows.Forms.Splitter split_top;
        private System.Windows.Forms.TabControl tc_result;
        //private System.Windows.Forms.TabPage tab_result;
        private System.Windows.Forms.TabPage tab_message;
        //private System.Windows.Forms.TextBox tb_message;
        //private DevExpress.XtraGrid.GridControl grid_result;
        //private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        //private DevExpress.XtraEditors.MemoEdit me_source;
        //private System.Windows.Forms.TabPage tab_result2;
        private System.Windows.Forms.ProgressBar pb_progress_bar;
        private System.Windows.Forms.Label lb_progress_label;
        //private System.Windows.Forms.TabPage tab_result3;
        //private System.Windows.Forms.DataGrid grid_result3;
        private System.Windows.Forms.Button bt_pause;
        //private System.Windows.Forms.TabPage tab_result4;
        //private System.Windows.Forms.TreeView tree_result;
    }
}

