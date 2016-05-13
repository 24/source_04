using pb.Windows.Forms;
using ScintillaNET;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceForm
    {
        private IContainer components = null;

        private MenuStrip _menuStrip;
        private ToolStripMenuItem m_view_source_line_number;
        private ToolStripMenuItem m_run_init;
        private ToolStripMenuItem m_allow_multiple_execution;

        private Panel pan_button;
        private Button bt_execute;
        private Button bt_pause;
        private Button bt_stop;
        private Label lb_progress_label;
        private ProgressBar pb_progress_bar;

        private Panel pan_top;
        private Splitter split_top;
        private Scintilla tb_source;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void Initialize()
        {
            this.SuspendLayout();
            CreatePanButton();
            CreatePanTop();
            CreatePanStatus();
            CreateResultTab();
            CreateXtraGrid();
            CreateDataGridView();
            CreateDataGrid();
            CreateTreeView();
            CreateMenu();
            CreateForm();
            this.ResumeLayout(false);
            this.PerformLayout();
            //SetControlsOrder();
        }

        private void CreateForm()
        {
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1056, 614);
            this.Controls.Add(pan_status);
            this.Controls.Add(tc_result);
            this.Controls.Add(split_top);
            this.Controls.Add(pan_top);
            this.Controls.Add(_menuStrip);
            this.MainMenuStrip = _menuStrip;
            this.KeyPreview = true;
            this.Name = "RunSourceForm";
            this.Text = "Run source";
            this.FormClosing += RunSourceForm_FormClosing;
            this.FormClosed += RunSourceForm_FormClosed;
            this.Load += RunSourceForm_Load;
            this.KeyDown += RunSourceForm_KeyDown;
        }

        //private void SetControlsOrder()
        //{
        //    TraceFormControls();
        //    int i = 0;
        //    this.Controls.SetChildIndex(pan_status, i++);
        //    this.Controls.SetChildIndex(tc_result, i++);
        //    this.Controls.SetChildIndex(split_top, i++);
        //    this.Controls.SetChildIndex(pan_top, i++);
        //    this.Controls.SetChildIndex(_menuStrip, i++);
        //    TraceFormControls();
        //}

        //private void TraceFormControls()
        //{
        //    _trace.WriteLine("TraceFormControls :");
        //    _trace.WriteLine("  pan_status     : index {0}", this.Controls.IndexOf(pan_status));     // index 0
        //    _trace.WriteLine("  tc_result      : index {0}", this.Controls.IndexOf(tc_result));      // index 1
        //    _trace.WriteLine("  split_top      : index {0}", this.Controls.IndexOf(split_top));      // index 2
        //    _trace.WriteLine("  pan_top        : index {0}", this.Controls.IndexOf(pan_top));        // index 3
        //    _trace.WriteLine("  menuStrip      : index {0}", this.Controls.IndexOf(_menuStrip));     // index 4
        //}

        private void CreateMenu()
        {
            _menuStrip = new MenuStrip();
            _menuStrip.SuspendLayout();

            ToolStripMenuItem m_file = zForm.CreateMenuItem("&File");
            m_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                zForm.CreateMenuItem("&New (Ctrl-N)", onClick: m_new_Click),
                zForm.CreateMenuItem("&Open (Ctrl-O)", onClick: m_open_Click),
                zForm.CreateMenuItem("&Save (Ctrl-S)", onClick: m_save_Click),
                zForm.CreateMenuItem("Save &as (Ctrl-A)", onClick: m_save_as_Click),
                new ToolStripSeparator(),
                zForm.CreateMenuItem("&Execute (F5)", onClick: m_execute_Click),
                zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
                zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
                zForm.CreateMenuItem("Execute &without project (ctrl + F5)", onClick: m_execute_without_project_Click),
                zForm.CreateMenuItem("&Compile (Shift-Ctrl-B)", onClick: m_compile_Click),
                new ToolStripSeparator(),
                zForm.CreateMenuItem("Compile and &restart \"Run source\" (Shift-Ctrl-U)", onClick: m_update_runsource_Click),
                zForm.CreateMenuItem("C&ompile \"Run source\"  (Shift-Ctrl-C)", onClick: m_compile_runsource_Click),
                zForm.CreateMenuItem("&Restart \"Run source\"  (Shift-Ctrl-R)", onClick:  m_restart_runsource_Click),
                new ToolStripSeparator(),
                zForm.CreateMenuItem("&Quit", onClick: m_quit_Click),
            });

            ToolStripMenuItem m_options = zForm.CreateMenuItem("&Options");

            m_view_source_line_number = zForm.CreateMenuItem("View source line number", checkOnClick: true, @checked: true, onClick: m_view_source_line_number_Click);
            m_run_init = zForm.CreateMenuItem("Run &init", checkOnClick: true, @checked: true, onClick: m_run_init_Click);
            m_allow_multiple_execution = zForm.CreateMenuItem("&Allow multiple execution", checkOnClick: true, @checked: true, onClick: m_allow_multiple_execution_Click);

            m_options.DropDownItems.AddRange(new ToolStripItem[] {
                zForm.CreateMenuItem("Set grid &max width height", checkOnClick: true, @checked: true, onClick: m_grid_set_max_width_height_Click),
                zForm.CreateMenuItem("Resize data table images", checkOnClick: true, @checked: true, onClick: m_resize_datatable_images_Click),
                m_view_source_line_number,
                new ToolStripSeparator(),
                m_run_init,
                m_allow_multiple_execution
            });

            _menuStrip.Items.AddRange(new ToolStripItem[] { m_file, m_options });

            _menuStrip.ResumeLayout(false);
            _menuStrip.PerformLayout();
        }

        private Panel pan_status;
        private ToolStrip toolStrip1;
        private ToolStripTextBox toolStripTextBox1;
        private ToolStripTextBox status_line;
        private void CreatePanStatus()
        {
            this.pan_status = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.status_line = new System.Windows.Forms.ToolStripTextBox();
            this.pan_status.SuspendLayout();
            this.toolStrip1.SuspendLayout();

            this.pan_status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pan_status.BackColor = System.Drawing.SystemColors.Control;
            this.pan_status.Controls.Add(this.toolStrip1);
            this.pan_status.Location = new System.Drawing.Point(285, 595);
            this.pan_status.Name = "pan_status";
            this.pan_status.Size = new System.Drawing.Size(770, 19);
            this.pan_status.TabIndex = 0;

            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1,
            this.status_line});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(770, 25);
            this.toolStrip1.TabIndex = 0;

            this.toolStripTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox1.Text = "Line";
            this.toolStripTextBox1.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.status_line.BackColor = System.Drawing.SystemColors.Control;
            this.status_line.Name = "status_line";
            this.status_line.Size = new System.Drawing.Size(100, 25);

            this.pan_status.ResumeLayout(false);
            this.pan_status.PerformLayout();

            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
        }

        private void CreatePanTop()
        {
            tb_source = new Scintilla();
            tb_source.Dock = DockStyle.Fill;
            tb_source.TextChanged += tb_source_TextChanged;
            //tb_source.Location = new Point(0, 35);
            //tb_source.Size = new Size(1056, 162);
            //tb_source.Name = "tb_source";
            //tb_source.TabIndex = 3;
            //tb_source.TabWidth = 2;
            //tb_source.UseTabs = false;

            pan_top = new Panel();
            pan_top.SuspendLayout();
            pan_top.Controls.Add(tb_source);
            pan_top.Controls.Add(pan_button);
            pan_top.Dock = DockStyle.Top;
            pan_top.Location = new Point(0, 24);
            pan_top.Size = new Size(1056, 197);
            //pan_top.Name = "pan_top";
            //pan_top.TabIndex = 0;
            pan_top.ResumeLayout(false);

            split_top = new Splitter();
            split_top.Dock = DockStyle.Top;
            split_top.Location = new Point(0, 221);
            split_top.Name = "split_top";
            split_top.Size = new Size(1056, 3);
            split_top.TabIndex = 1;
            split_top.TabStop = false;
        }

        private void CreatePanButton()
        {
            lb_progress_label = new Label();
            lb_progress_label.AutoSize = true;
            lb_progress_label.Location = new Point(624, 1);
            lb_progress_label.Size = new Size(47, 13);
            lb_progress_label.Text = "progress";
            lb_progress_label.TextAlign = ContentAlignment.MiddleCenter;
            //lb_progress_label.Name = "lb_progress_label";
            //lb_progress_label.TabIndex = 1;

            pb_progress_bar = new ProgressBar();
            pb_progress_bar.Location = new Point(622, 16);
            pb_progress_bar.Size = new Size(422, 16);
            //pb_progress_bar.Name = "pb_progress_bar";
            //pb_progress_bar.TabIndex = 0;

            bt_execute = zForm.CreateButton("&Run", x: 3, y: 3, width: 75, height: 23, onClick: bt_execute_Click);
            bt_pause = zForm.CreateButton("&Pause", x: 87, y: 3, width: 75, height: 23, onClick: bt_pause_Click);
            bt_stop = zForm.CreateButton("&Stop", x: 171, y: 3, width: 75, height: 23, onClick: bt_stop_Click);

            pan_button = new Panel();
            pan_button.SuspendLayout();
            pan_button.BackColor = SystemColors.Control;
            pan_button.Controls.Add(bt_execute);
            pan_button.Controls.Add(bt_pause);
            pan_button.Controls.Add(bt_stop);
            pan_button.Controls.Add(lb_progress_label);
            pan_button.Controls.Add(pb_progress_bar);
            pan_button.Dock = DockStyle.Top;
            pan_button.Location = new Point(0, 0);
            pan_button.Size = new Size(1056, 35);
            //pan_button.Name = "pan_button";
            //pan_button.TabIndex = 2;

            // 
            // bt_execute
            // 
            //this.bt_execute.Location = new System.Drawing.Point(3, 3);
            //this.bt_execute.Name = "bt_execute";
            //this.bt_execute.Size = new System.Drawing.Size(75, 23);
            //this.bt_execute.TabIndex = 1;
            //this.bt_execute.Text = "&Run";
            //this.bt_execute.UseVisualStyleBackColor = true;
            //this.bt_execute.Click += new System.EventHandler(this.bt_execute_Click);
            // 
            // bt_stop
            // 
            // 
            //this.bt_stop.Enabled = false;
            //this.bt_stop.Location = new System.Drawing.Point(171, 3);
            //this.bt_stop.Name = "bt_stop";
            //this.bt_stop.Size = new System.Drawing.Size(75, 23);
            //this.bt_stop.TabIndex = 2;
            //this.bt_stop.Text = "&Stop";
            //this.bt_stop.UseVisualStyleBackColor = true;
            //this.bt_stop.Click += new System.EventHandler(this.bt_stop_Click);
            // 
            // bt_pause
            // 
            //
            //this.bt_pause.Location = new System.Drawing.Point(87, 3);
            //this.bt_pause.Name = "bt_pause";
            //this.bt_pause.Size = new System.Drawing.Size(75, 23);
            //this.bt_pause.TabIndex = 5;
            //this.bt_pause.Text = "&Pause";
            //this.bt_pause.UseVisualStyleBackColor = true;
            //this.bt_pause.Click += new System.EventHandler(this.bt_pause_Click);


            this.pan_button.ResumeLayout(false);
            this.pan_button.PerformLayout();
        }

        //private void InitializeMenu_v1()
        //{
        //    this.menuStrip1 = new System.Windows.Forms.MenuStrip();
        //    this.m_file = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_new = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_open = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_save = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_save_as = new System.Windows.Forms.ToolStripMenuItem();
        //    this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        //    this.m_execute = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_execute_on_main_thread = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_execute_without_project = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_compile = new System.Windows.Forms.ToolStripMenuItem();
        //    this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
        //    this.m_update_runsource = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_compile_runsource = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_restart_runsource = new System.Windows.Forms.ToolStripMenuItem();
        //    this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
        //    this.m_quit = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_view = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_grid_set_max_width_height = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_resize_datatable_images = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_view_source_line_number = new System.Windows.Forms.ToolStripMenuItem();
        //    this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
        //    this.m_run_init = new System.Windows.Forms.ToolStripMenuItem();
        //    this.m_allow_multiple_execution = new System.Windows.Forms.ToolStripMenuItem();

        //    this.menuStrip1.SuspendLayout();

        //    // 
        //    // menuStrip1
        //    // 
        //    this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        //    this.m_file,
        //    this.m_view});
        //    this.menuStrip1.Location = new System.Drawing.Point(0, 0);
        //    this.menuStrip1.Name = "menuStrip1";
        //    this.menuStrip1.Size = new System.Drawing.Size(1056, 24);
        //    this.menuStrip1.TabIndex = 3;
        //    this.menuStrip1.Text = "menuStrip1";
        //    // 
        //    // m_file
        //    // 
        //    this.m_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        //    this.m_new,
        //    this.m_open,
        //    this.m_save,
        //    this.m_save_as,
        //    this.toolStripSeparator1,
        //    this.m_execute,
        //    this.m_execute_on_main_thread,
        //    this.m_execute_without_project,
        //    this.m_compile,
        //    this.toolStripSeparator2,
        //    this.m_update_runsource,
        //    this.m_compile_runsource,
        //    this.m_restart_runsource,
        //    this.toolStripSeparator3,
        //    this.m_quit});
        //    this.m_file.Name = "m_file";
        //    this.m_file.Size = new System.Drawing.Size(37, 20);
        //    this.m_file.Text = "&File";
        //    // 
        //    // m_new
        //    // 
        //    this.m_new.Name = "m_new";
        //    this.m_new.Size = new System.Drawing.Size(322, 22);
        //    this.m_new.Text = "&New (Ctrl-N)";
        //    this.m_new.Click += new System.EventHandler(this.m_new_Click);
        //    // 
        //    // m_open
        //    // 
        //    this.m_open.Name = "m_open";
        //    this.m_open.Size = new System.Drawing.Size(322, 22);
        //    this.m_open.Text = "&Open (Ctrl-O)";
        //    this.m_open.Click += new System.EventHandler(this.m_open_Click);
        //    // 
        //    // m_save
        //    // 
        //    this.m_save.Name = "m_save";
        //    this.m_save.Size = new System.Drawing.Size(322, 22);
        //    this.m_save.Text = "&Save (Ctrl-S)";
        //    this.m_save.Click += new System.EventHandler(this.m_save_Click);
        //    // 
        //    // m_save_as
        //    // 
        //    this.m_save_as.Name = "m_save_as";
        //    this.m_save_as.Size = new System.Drawing.Size(322, 22);
        //    this.m_save_as.Text = "Save &as (Ctrl-A)";
        //    this.m_save_as.Click += new System.EventHandler(this.m_save_as_Click);
        //    // 
        //    // toolStripSeparator1
        //    // 
        //    this.toolStripSeparator1.Name = "toolStripSeparator1";
        //    this.toolStripSeparator1.Size = new System.Drawing.Size(319, 6);
        //    // 
        //    // m_execute
        //    // 
        //    this.m_execute.Name = "m_execute";
        //    this.m_execute.Size = new System.Drawing.Size(322, 22);
        //    this.m_execute.Text = "&Execute (F5)";
        //    this.m_execute.Click += new System.EventHandler(this.m_execute_Click);
        //    // 
        //    // m_execute_on_main_thread
        //    // 
        //    this.m_execute_on_main_thread.Name = "m_execute_on_main_thread";
        //    this.m_execute_on_main_thread.Size = new System.Drawing.Size(322, 22);
        //    this.m_execute_on_main_thread.Text = "Execute on &main thread (shift + F5)";
        //    this.m_execute_on_main_thread.Click += new System.EventHandler(this.m_execute_on_main_thread_Click);
        //    // 
        //    // m_execute_without_project
        //    // 
        //    this.m_execute_without_project.Name = "m_execute_without_project";
        //    this.m_execute_without_project.Size = new System.Drawing.Size(322, 22);
        //    this.m_execute_without_project.Text = "Execute &without project (ctrl + F5)";
        //    this.m_execute_without_project.Click += new System.EventHandler(this.m_execute_without_project_Click);
        //    // 
        //    // m_compile
        //    // 
        //    this.m_compile.Name = "m_compile";
        //    this.m_compile.Size = new System.Drawing.Size(322, 22);
        //    this.m_compile.Text = "&Compile (Shift-Ctrl-B)";
        //    this.m_compile.Click += new System.EventHandler(this.m_compile_Click);
        //    // 
        //    // toolStripSeparator2
        //    // 
        //    this.toolStripSeparator2.Name = "toolStripSeparator2";
        //    this.toolStripSeparator2.Size = new System.Drawing.Size(319, 6);
        //    // 
        //    // m_update_runsource
        //    // 
        //    this.m_update_runsource.Name = "m_update_runsource";
        //    this.m_update_runsource.Size = new System.Drawing.Size(322, 22);
        //    this.m_update_runsource.Text = "Compile and &restart \"Run source\" (Shift-Ctrl-U)";
        //    this.m_update_runsource.Click += new System.EventHandler(this.m_update_runsource_Click);
        //    // 
        //    // m_compile_runsource
        //    // 
        //    this.m_compile_runsource.Name = "m_compile_runsource";
        //    this.m_compile_runsource.Size = new System.Drawing.Size(322, 22);
        //    this.m_compile_runsource.Text = "C&ompile \"Run source\"  (Shift-Ctrl-C)";
        //    this.m_compile_runsource.Click += new System.EventHandler(this.m_compile_runsource_Click);
        //    // 
        //    // m_restart_runsource
        //    // 
        //    this.m_restart_runsource.Name = "m_restart_runsource";
        //    this.m_restart_runsource.Size = new System.Drawing.Size(322, 22);
        //    this.m_restart_runsource.Text = "&Restart \"Run source\"  (Shift-Ctrl-R)";
        //    this.m_restart_runsource.Click += new System.EventHandler(this.m_restart_runsource_Click);
        //    // 
        //    // toolStripSeparator3
        //    // 
        //    this.toolStripSeparator3.Name = "toolStripSeparator3";
        //    this.toolStripSeparator3.Size = new System.Drawing.Size(319, 6);
        //    // 
        //    // m_quit
        //    // 
        //    this.m_quit.Name = "m_quit";
        //    this.m_quit.Size = new System.Drawing.Size(322, 22);
        //    this.m_quit.Text = "&Quit";
        //    this.m_quit.Click += new System.EventHandler(this.m_quit_Click);
        //    // 
        //    // m_view
        //    // 
        //    this.m_view.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        //    this.m_grid_set_max_width_height,
        //    this.m_resize_datatable_images,
        //    this.m_view_source_line_number,
        //    this.toolStripSeparator4,
        //    this.m_run_init,
        //    this.m_allow_multiple_execution});
        //    this.m_view.Name = "m_view";
        //    this.m_view.Size = new System.Drawing.Size(56, 20);
        //    this.m_view.Text = "&Option";
        //    // 
        //    // m_grid_set_max_width_height
        //    // 
        //    this.m_grid_set_max_width_height.Checked = true;
        //    this.m_grid_set_max_width_height.CheckOnClick = true;
        //    this.m_grid_set_max_width_height.CheckState = System.Windows.Forms.CheckState.Checked;
        //    this.m_grid_set_max_width_height.Name = "m_grid_set_max_width_height";
        //    this.m_grid_set_max_width_height.Size = new System.Drawing.Size(209, 22);
        //    this.m_grid_set_max_width_height.Text = "Set grid &max width height";
        //    this.m_grid_set_max_width_height.Click += new System.EventHandler(this.m_grid_set_max_width_height_Click);
        //    // 
        //    // m_resize_datatable_images
        //    // 
        //    this.m_resize_datatable_images.Checked = true;
        //    this.m_resize_datatable_images.CheckOnClick = true;
        //    this.m_resize_datatable_images.CheckState = System.Windows.Forms.CheckState.Checked;
        //    this.m_resize_datatable_images.Name = "m_resize_datatable_images";
        //    this.m_resize_datatable_images.Size = new System.Drawing.Size(209, 22);
        //    this.m_resize_datatable_images.Text = "Resize data table images";
        //    this.m_resize_datatable_images.Click += new System.EventHandler(this.m_resize_datatable_images_Click);
        //    // 
        //    // m_view_source_line_number
        //    // 
        //    this.m_view_source_line_number.Checked = true;
        //    this.m_view_source_line_number.CheckOnClick = true;
        //    this.m_view_source_line_number.CheckState = System.Windows.Forms.CheckState.Checked;
        //    this.m_view_source_line_number.Name = "m_view_source_line_number";
        //    this.m_view_source_line_number.Size = new System.Drawing.Size(209, 22);
        //    this.m_view_source_line_number.Text = "View source line number";
        //    this.m_view_source_line_number.Click += new System.EventHandler(this.m_view_source_line_number_Click);
        //    // 
        //    // toolStripSeparator4
        //    // 
        //    this.toolStripSeparator4.Name = "toolStripSeparator4";
        //    this.toolStripSeparator4.Size = new System.Drawing.Size(206, 6);
        //    // 
        //    // m_run_init
        //    // 
        //    this.m_run_init.CheckOnClick = true;
        //    this.m_run_init.Name = "m_run_init";
        //    this.m_run_init.Size = new System.Drawing.Size(209, 22);
        //    this.m_run_init.Text = "Run &init";
        //    this.m_run_init.Click += new System.EventHandler(this.m_run_init_Click);
        //    // 
        //    // m_allow_multiple_execution
        //    // 
        //    this.m_allow_multiple_execution.CheckOnClick = true;
        //    this.m_allow_multiple_execution.Name = "m_allow_multiple_execution";
        //    this.m_allow_multiple_execution.Size = new System.Drawing.Size(209, 22);
        //    this.m_allow_multiple_execution.Text = "&Allow multiple execution";
        //    this.m_allow_multiple_execution.Click += new System.EventHandler(this.m_allow_multiple_execution_Click);

        //    this.Controls.Add(this.menuStrip1);
        //    this.MainMenuStrip = this.menuStrip1;

        //    this.menuStrip1.ResumeLayout(false);
        //    this.menuStrip1.PerformLayout();
        //}
    }
}
