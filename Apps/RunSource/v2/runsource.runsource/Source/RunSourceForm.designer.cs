namespace runsourced
{
    partial class RunSourceForm
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
            //System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunSourceForm));
            this.pan_top = new System.Windows.Forms.Panel();
            //this.me_source = new DevExpress.XtraEditors.MemoEdit();
            this.tb_source = new ScintillaNET.Scintilla();
            this.bt_execute = new System.Windows.Forms.Button();
            this.pan_button = new System.Windows.Forms.Panel();
            this.bt_pause = new System.Windows.Forms.Button();
            this.lb_progress_label = new System.Windows.Forms.Label();
            this.pb_progress_bar = new System.Windows.Forms.ProgressBar();
            this.split_top = new System.Windows.Forms.Splitter();
            this.tc_result = new System.Windows.Forms.TabControl();
            this.tab_result = new System.Windows.Forms.TabPage();
            this.grid_result = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.tab_result2 = new System.Windows.Forms.TabPage();
            //this.grid_result2 = new System.Windows.Forms.DataGridView();
            this.tab_result3 = new System.Windows.Forms.TabPage();
            this.grid_result3 = new System.Windows.Forms.DataGrid();
            this.tab_result4 = new System.Windows.Forms.TabPage();
            this.tree_result = new System.Windows.Forms.TreeView();
            this.tab_message = new System.Windows.Forms.TabPage();
            this.tb_message = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.m_file = new System.Windows.Forms.ToolStripMenuItem();
            this.m_new = new System.Windows.Forms.ToolStripMenuItem();
            this.m_open = new System.Windows.Forms.ToolStripMenuItem();
            this.m_save = new System.Windows.Forms.ToolStripMenuItem();
            this.m_save_as = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_execute = new System.Windows.Forms.ToolStripMenuItem();
            this.m_execute_on_main_thread = new System.Windows.Forms.ToolStripMenuItem();
            this.m_execute_without_project = new System.Windows.Forms.ToolStripMenuItem();
            this.m_compile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_update_runsource = new System.Windows.Forms.ToolStripMenuItem();
            this.m_compile_runsource = new System.Windows.Forms.ToolStripMenuItem();
            this.m_restart_runsource = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.m_quit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_grid = new System.Windows.Forms.ToolStripMenuItem();
            this.m_grid_set_max_width_height = new System.Windows.Forms.ToolStripMenuItem();
            this.m_resize_datatable_images = new System.Windows.Forms.ToolStripMenuItem();
            this.pan_top.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.me_source.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_source)).BeginInit();
            this.pan_button.SuspendLayout();
            this.tc_result.SuspendLayout();
            this.tab_result.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_result)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.tab_result2.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.grid_result2)).BeginInit();
            this.tab_result3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid_result3)).BeginInit();
            this.tab_result4.SuspendLayout();
            this.tab_message.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan_top
            // 
            //this.pan_top.Controls.Add(this.me_source);
            this.pan_top.Controls.Add(this.tb_source);
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
            //this.me_source.TextChanged += new System.EventHandler(this.me_source_TextChanged);
            // 
            // tb_source
            // 
            this.tb_source.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.tb_source.EditValue = "";
            this.tb_source.Location = new System.Drawing.Point(0, 35);
            this.tb_source.Name = "me_source";
            this.tb_source.Size = new System.Drawing.Size(1056, 162);
            this.tb_source.Styles.BraceBad.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.BraceLight.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.CallTip.FontName = "Tahoma\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.ControlChar.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.Default.BackColor = System.Drawing.SystemColors.Window;
            this.tb_source.Styles.Default.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.IndentGuide.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.LastPredefined.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.LineNumber.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.Styles.Max.FontName = "Verdana\0\0\0\0\0\0\0\0\0\0\0\0\0";
            this.tb_source.TabIndex = 4;
            this.tb_source.TextChanged += new System.EventHandler(this.me_source_TextChanged);
            // 
            // bt_execute
            // 
            this.bt_execute.Location = new System.Drawing.Point(3, 3);
            this.bt_execute.Name = "bt_execute";
            this.bt_execute.Size = new System.Drawing.Size(75, 23);
            this.bt_execute.TabIndex = 1;
            this.bt_execute.Text = "&Run";
            this.bt_execute.UseVisualStyleBackColor = true;
            this.bt_execute.Click += new System.EventHandler(this.bt_execute_Click);
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
            this.bt_pause.Click += new System.EventHandler(this.bt_pause_Click);
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
            this.tc_result.Controls.Add(this.tab_result);
            this.tc_result.Controls.Add(this.tab_result2);
            this.tc_result.Controls.Add(this.tab_result3);
            this.tc_result.Controls.Add(this.tab_result4);
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
            this.tab_result.Controls.Add(this.grid_result);
            this.tab_result.Location = new System.Drawing.Point(4, 4);
            this.tab_result.Name = "tab_result";
            this.tab_result.Padding = new System.Windows.Forms.Padding(3);
            this.tab_result.Size = new System.Drawing.Size(1048, 364);
            this.tab_result.TabIndex = 0;
            this.tab_result.Text = "Results 1";
            this.tab_result.UseVisualStyleBackColor = true;
            // 
            // grid_result
            // 
            this.grid_result.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // 
            // 
            this.grid_result.EmbeddedNavigator.Name = "";
            this.grid_result.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grid_result.Location = new System.Drawing.Point(3, 3);
            this.grid_result.MainView = this.gridView1;
            this.grid_result.Name = "grid_result";
            this.grid_result.Size = new System.Drawing.Size(1042, 358);
            this.grid_result.TabIndex = 0;
            this.grid_result.Text = "gridControl1";
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.grid_result;
            this.gridView1.Name = "gridView1";
            // 
            // tab_result2
            // 
            //this.tab_result2.Controls.Add(this.grid_result2);
            this.tab_result2.Location = new System.Drawing.Point(4, 4);
            this.tab_result2.Name = "tab_result2";
            this.tab_result2.Size = new System.Drawing.Size(1048, 364);
            this.tab_result2.TabIndex = 2;
            this.tab_result2.Text = "Results 2";
            this.tab_result2.UseVisualStyleBackColor = true;
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
            this.tab_result3.Controls.Add(this.grid_result3);
            this.tab_result3.Location = new System.Drawing.Point(4, 4);
            this.tab_result3.Name = "tab_result3";
            this.tab_result3.Size = new System.Drawing.Size(1048, 364);
            this.tab_result3.TabIndex = 3;
            this.tab_result3.Text = "Results 3";
            this.tab_result3.UseVisualStyleBackColor = true;
            // 
            // grid_result3
            // 
            this.grid_result3.DataMember = "";
            this.grid_result3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_result3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grid_result3.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.grid_result3.Location = new System.Drawing.Point(0, 0);
            this.grid_result3.Name = "grid_result3";
            this.grid_result3.Size = new System.Drawing.Size(1048, 364);
            this.grid_result3.TabIndex = 0;
            // 
            // tab_result4
            // 
            this.tab_result4.Controls.Add(this.tree_result);
            this.tab_result4.Location = new System.Drawing.Point(4, 4);
            this.tab_result4.Name = "tab_result4";
            this.tab_result4.Size = new System.Drawing.Size(1048, 364);
            this.tab_result4.TabIndex = 4;
            this.tab_result4.Text = "Results 4";
            this.tab_result4.UseVisualStyleBackColor = true;
            // 
            // tree_result
            // 
            this.tree_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tree_result.Location = new System.Drawing.Point(0, 0);
            this.tree_result.Name = "tree_result";
            this.tree_result.Size = new System.Drawing.Size(1048, 364);
            this.tree_result.TabIndex = 0;
            // 
            // tab_message
            // 
            this.tab_message.Controls.Add(this.tb_message);
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
            this.tb_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_message.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_message.Location = new System.Drawing.Point(3, 3);
            this.tb_message.Multiline = true;
            this.tb_message.Name = "tb_message";
            this.tb_message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_message.Size = new System.Drawing.Size(1042, 358);
            this.tb_message.TabIndex = 0;
            this.tb_message.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_file, this.m_grid});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1056, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // m_file
            // 
            this.m_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_new,
            this.m_open,
            this.m_save,
            this.m_save_as,
            this.toolStripSeparator1,
            this.m_execute,
            this.m_execute_on_main_thread,
            this.m_execute_without_project,
            this.m_compile,
            this.toolStripSeparator2,
            this.m_update_runsource,
            this.m_compile_runsource,
            this.m_restart_runsource,
            this.toolStripSeparator3,
            this.m_quit});
            this.m_file.Name = "m_file";
            this.m_file.Size = new System.Drawing.Size(35, 20);
            this.m_file.Text = "&File";
            // 
            // m_new
            // 
            this.m_new.Name = "m_new";
            this.m_new.Size = new System.Drawing.Size(186, 22);
            this.m_new.Text = "&New (Ctrl-N)";
            this.m_new.Click += new System.EventHandler(this.m_new_Click);
            // 
            // m_open
            // 
            this.m_open.Name = "m_open";
            this.m_open.Size = new System.Drawing.Size(186, 22);
            this.m_open.Text = "&Open (Ctrl-O)";
            this.m_open.Click += new System.EventHandler(this.m_open_Click);
            // 
            // m_save
            // 
            this.m_save.Name = "m_save";
            this.m_save.Size = new System.Drawing.Size(186, 22);
            this.m_save.Text = "&Save (Ctrl-S)";
            this.m_save.Click += new System.EventHandler(this.m_save_Click);
            // 
            // m_save_as
            // 
            this.m_save_as.Name = "m_save_as";
            this.m_save_as.Size = new System.Drawing.Size(186, 22);
            this.m_save_as.Text = "Save &as (Ctrl-A)";
            this.m_save_as.Click += new System.EventHandler(this.m_save_as_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // m_execute
            // 
            this.m_execute.Name = "m_execute";
            this.m_execute.Size = new System.Drawing.Size(186, 22);
            this.m_execute.Text = "&Execute (F5)";
            this.m_execute.Click += new System.EventHandler(this.m_execute_Click);
            // 
            // m_execute_on_main_thread
            // 
            this.m_execute_on_main_thread.Name = "m_execute_on_main_thread";
            this.m_execute_on_main_thread.Size = new System.Drawing.Size(186, 22);
            this.m_execute_on_main_thread.Text = "Execute on &main thread (shift + F5)";
            this.m_execute_on_main_thread.Click += new System.EventHandler(this.m_execute_on_main_thread_Click);
            // 
            // m_execute_without_project
            // 
            this.m_execute_without_project.Name = "m_execute_without_project";
            this.m_execute_without_project.Size = new System.Drawing.Size(186, 22);
            this.m_execute_without_project.Text = "Execute &without project (ctrl + F5)";
            this.m_execute_without_project.Click += new System.EventHandler(this.m_execute_without_project_Click);
            // 
            // m_compile
            // 
            this.m_compile.Name = "m_compile";
            this.m_compile.Size = new System.Drawing.Size(186, 22);
            this.m_compile.Text = "&Compile (Shift-Ctrl-B)";
            this.m_compile.Click += new System.EventHandler(this.m_compile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
            // 
            // m_update_runsource
            // 
            this.m_update_runsource.Name = "m_update_runsource";
            this.m_update_runsource.Size = new System.Drawing.Size(186, 22);
            this.m_update_runsource.Text = "Compile and &restart \"Run source\" (Shift-Ctrl-U)";
            this.m_update_runsource.Click += new System.EventHandler(this.m_update_runsource_Click);
            // 
            // m_compile_runsource
            // 
            this.m_compile_runsource.Name = "m_compile_runsource";
            this.m_compile_runsource.Size = new System.Drawing.Size(186, 22);
            this.m_compile_runsource.Text = "C&ompile \"Run source\"  (Shift-Ctrl-C)";
            this.m_compile_runsource.Click += new System.EventHandler(this.m_compile_runsource_Click);
            // 
            // m_restart_runsource
            // 
            this.m_restart_runsource.Name = "m_restart_runsource";
            this.m_restart_runsource.Size = new System.Drawing.Size(186, 22);
            this.m_restart_runsource.Text = "&Restart \"Run source\"  (Shift-Ctrl-R)";
            this.m_restart_runsource.Click += new System.EventHandler(this.m_restart_runsource_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(183, 6);
            // 
            // m_quit
            // 
            this.m_quit.Name = "m_quit";
            this.m_quit.Size = new System.Drawing.Size(186, 22);
            this.m_quit.Text = "&Quit";
            this.m_quit.Click += new System.EventHandler(this.m_quit_Click);
            // 
            // m_grid
            // 
            this.m_grid.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_grid_set_max_width_height,
            this.m_resize_datatable_images});
            this.m_grid.Name = "m_grid";
            this.m_grid.Size = new System.Drawing.Size(35, 20);
            this.m_grid.Text = "&Grid";
            // 
            // m_grid_set_max_width_height
            // 
            this.m_grid_set_max_width_height.Name = "m_grid_set_max_width_height";
            this.m_grid_set_max_width_height.Size = new System.Drawing.Size(186, 22);
            this.m_grid_set_max_width_height.Text = "Set grid &max width height";
            this.m_grid_set_max_width_height.Checked = true;
            this.m_grid_set_max_width_height.CheckOnClick = true;
            this.m_grid_set_max_width_height.Click += new System.EventHandler(this.m_grid_set_max_width_height_Click);
            // 
            // m_resize_datatable_images
            // 
            this.m_resize_datatable_images.Name = "m_resize_datatable_images";
            this.m_resize_datatable_images.Size = new System.Drawing.Size(186, 22);
            this.m_resize_datatable_images.Text = "Resize data table &images";
            this.m_resize_datatable_images.Checked = true;
            this.m_resize_datatable_images.CheckOnClick = true;
            this.m_resize_datatable_images.Click += new System.EventHandler(this.m_resize_datatable_images_Click);
            // 
            // fWRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 614);
            this.Controls.Add(this.tc_result);
            this.Controls.Add(this.split_top);
            this.Controls.Add(this.pan_top);
            this.Controls.Add(this.menuStrip1);
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "fWRun";
            this.Text = "Run source";
            this.Load += new System.EventHandler(this.RunSourceForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RunSourceForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RunSourceForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fWRun_KeyDown);
            this.pan_top.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.me_source.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_source)).EndInit();
            this.pan_button.ResumeLayout(false);
            this.pan_button.PerformLayout();
            this.tc_result.ResumeLayout(false);
            this.tab_result.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_result)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.tab_result2.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.grid_result2)).EndInit();
            this.tab_result3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid_result3)).EndInit();
            this.tab_result4.ResumeLayout(false);
            this.tab_message.ResumeLayout(false);
            this.tab_message.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.Button bt_execute;
        private System.Windows.Forms.Panel pan_button;
        private System.Windows.Forms.Splitter split_top;
        private System.Windows.Forms.TabControl tc_result;
        private System.Windows.Forms.TabPage tab_result;
        private System.Windows.Forms.TabPage tab_message;
        private System.Windows.Forms.TextBox tb_message;
        private DevExpress.XtraGrid.GridControl grid_result;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        //private DevExpress.XtraEditors.MemoEdit me_source;
        private ScintillaNET.Scintilla tb_source;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem m_file;
        private System.Windows.Forms.ToolStripMenuItem m_new;
        private System.Windows.Forms.ToolStripMenuItem m_open;
        private System.Windows.Forms.ToolStripMenuItem m_save;
        private System.Windows.Forms.ToolStripMenuItem m_save_as;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem m_execute;
        private System.Windows.Forms.ToolStripMenuItem m_execute_on_main_thread;
        private System.Windows.Forms.ToolStripMenuItem m_execute_without_project;
        private System.Windows.Forms.ToolStripMenuItem m_compile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem m_update_runsource;
        private System.Windows.Forms.ToolStripMenuItem m_compile_runsource;
        private System.Windows.Forms.ToolStripMenuItem m_restart_runsource;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem m_quit;
        private System.Windows.Forms.ToolStripMenuItem m_grid;
        private System.Windows.Forms.ToolStripMenuItem m_grid_set_max_width_height;
        private System.Windows.Forms.ToolStripMenuItem m_resize_datatable_images;
        private System.Windows.Forms.TabPage tab_result2;
        //private System.Windows.Forms.DataGridView grid_result2;
        private System.Windows.Forms.ProgressBar pb_progress_bar;
        private System.Windows.Forms.Label lb_progress_label;
        private System.Windows.Forms.TabPage tab_result3;
        private System.Windows.Forms.DataGrid grid_result3;
        private System.Windows.Forms.Button bt_pause;
        private System.Windows.Forms.TabPage tab_result4;
        private System.Windows.Forms.TreeView tree_result;
    }
}

