using pb.Windows.Forms;
using ScintillaNET;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace runsourced
{
    partial class RunSourceForm_copy
    {
        //private IContainer components = null;

        private MenuStrip _menuStrip;

        private Panel pan_button;
        private Button bt_execute;
        private Button bt_pause;
        private Button bt_stop;
        private Label lb_progress_label;
        private ProgressBar pb_progress_bar;

        private Panel pan_top;
        private Splitter split_top;
        private Scintilla tb_source;

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        private void Initialize()
        {
            //this.SuspendLayout();
            //CreatePanButton();
            //CreatePanTop();
            //CreatePanStatus();
            //CreateResultTab();
            CreateXtraGrid();
            CreateDataGridView();
            CreateDataGrid();
            CreateTreeView();
            //CreateMenu();
            //CreateForm();
            //this.ResumeLayout(false);
            //this.PerformLayout();
            //SetControlsOrder();
        }

        //private void CreateForm()
        //{
        //    //this.AutoScaleDimensions = new SizeF(6F, 13F);
        //    //this.AutoScaleMode = AutoScaleMode.Font;
        //    //this.ClientSize = new Size(1056, 614);
        //    //this.Controls.Add(pan_status);
        //    //this.Controls.Add(tc_result);
        //    //this.Controls.Add(split_top);
        //    //this.Controls.Add(pan_top);
        //    //this.Controls.Add(_menuStrip);
        //    //this.MainMenuStrip = _menuStrip;
        //    //this.KeyPreview = true;
        //    //this.Name = "RunSourceForm";
        //    //this.Text = "Run source";
        //    //this.FormClosing += RunSourceForm_FormClosing;
        //    //this.FormClosed += RunSourceForm_FormClosed;
        //    //this.Load += RunSourceForm_Load;
        //    //this.KeyDown += RunSourceForm_KeyDown;
        //}

        //private void CreateMenu()
        //{
        //    _menuStrip = new MenuStrip();
        //    _menuStrip.SuspendLayout();

        //    ToolStripMenuItem m_file = zForm.CreateMenuItem("&File");
        //    m_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        //        zForm.CreateMenuItem("&New (Ctrl-N)", onClick: m_new_Click),
        //        zForm.CreateMenuItem("&Open (Ctrl-O)", onClick: m_open_Click),
        //        zForm.CreateMenuItem("&Save (Ctrl-S)", onClick: m_save_Click),
        //        zForm.CreateMenuItem("Save &as (Ctrl-A)", onClick: m_save_as_Click),
        //        new ToolStripSeparator(),
        //        zForm.CreateMenuItem("&Execute (F5)", onClick: m_execute_Click),
        //        zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
        //        zForm.CreateMenuItem("Execute on &main thread (shift + F5)", onClick: m_execute_on_main_thread_Click),
        //        zForm.CreateMenuItem("Execute &without project (ctrl + F5)", onClick: m_execute_without_project_Click),
        //        zForm.CreateMenuItem("&Compile (Shift-Ctrl-B)", onClick: m_compile_Click),
        //        new ToolStripSeparator(),
        //        zForm.CreateMenuItem("Compile and &restart \"Run source\" (Shift-Ctrl-U)", onClick: m_update_runsource_Click),
        //        zForm.CreateMenuItem("C&ompile \"Run source\"  (Shift-Ctrl-C)", onClick: m_compile_runsource_Click),
        //        zForm.CreateMenuItem("&Restart \"Run source\"  (Shift-Ctrl-R)", onClick:  m_restart_runsource_Click),
        //        new ToolStripSeparator(),
        //        zForm.CreateMenuItem("&Quit", onClick: m_quit_Click),
        //    });

        //    ToolStripMenuItem m_options = zForm.CreateMenuItem("&Options");

        //    m_view_source_line_number = zForm.CreateMenuItem("View source line number", checkOnClick: true, @checked: true, onClick: m_view_source_line_number_Click);
        //    m_run_init = zForm.CreateMenuItem("Run &init", checkOnClick: true, @checked: true, onClick: m_run_init_Click);
        //    m_allow_multiple_execution = zForm.CreateMenuItem("&Allow multiple execution", checkOnClick: true, @checked: true, onClick: m_allow_multiple_execution_Click);

        //    m_options.DropDownItems.AddRange(new ToolStripItem[] {
        //        zForm.CreateMenuItem("Set grid &max width height", checkOnClick: true, @checked: true, onClick: m_grid_set_max_width_height_Click),
        //        zForm.CreateMenuItem("Resize data table images", checkOnClick: true, @checked: true, onClick: m_resize_datatable_images_Click),
        //        m_view_source_line_number,
        //        new ToolStripSeparator(),
        //        m_run_init,
        //        m_allow_multiple_execution
        //    });

        //    _menuStrip.Items.AddRange(new ToolStripItem[] { m_file, m_options });

        //    _menuStrip.ResumeLayout(false);
        //    _menuStrip.PerformLayout();
        //}

        private Panel pan_status;
        private ToolStrip toolStrip1;
        private ToolStripTextBox toolStripTextBox1;
        private ToolStripTextBox status_line;
        //private void CreatePanStatus()
        //{
        //    this.pan_status = new System.Windows.Forms.Panel();
        //    this.toolStrip1 = new System.Windows.Forms.ToolStrip();
        //    this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
        //    this.status_line = new System.Windows.Forms.ToolStripTextBox();
        //    this.pan_status.SuspendLayout();
        //    this.toolStrip1.SuspendLayout();

        //    this.pan_status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        //    this.pan_status.BackColor = System.Drawing.SystemColors.Control;
        //    this.pan_status.Controls.Add(this.toolStrip1);
        //    this.pan_status.Location = new System.Drawing.Point(285, 595);
        //    this.pan_status.Name = "pan_status";
        //    this.pan_status.Size = new System.Drawing.Size(770, 19);
        //    this.pan_status.TabIndex = 0;

        //    this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        //    this.toolStripTextBox1,
        //    this.status_line});
        //    this.toolStrip1.Location = new System.Drawing.Point(0, 0);
        //    this.toolStrip1.Name = "toolStrip1";
        //    this.toolStrip1.Size = new System.Drawing.Size(770, 25);
        //    this.toolStrip1.TabIndex = 0;

        //    this.toolStripTextBox1.BackColor = System.Drawing.SystemColors.Control;
        //    this.toolStripTextBox1.Name = "toolStripTextBox1";
        //    this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
        //    this.toolStripTextBox1.Text = "Line";
        //    this.toolStripTextBox1.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;

        //    this.status_line.BackColor = System.Drawing.SystemColors.Control;
        //    this.status_line.Name = "status_line";
        //    this.status_line.Size = new System.Drawing.Size(100, 25);

        //    this.pan_status.ResumeLayout(false);
        //    this.pan_status.PerformLayout();

        //    this.toolStrip1.ResumeLayout(false);
        //    this.toolStrip1.PerformLayout();
        //}

        //private void CreatePanTop()
        //{
        //    //tb_source = new Scintilla();
        //    //tb_source.Dock = DockStyle.Fill;
        //    //tb_source.TextChanged += tb_source_TextChanged;
        //    ////tb_source.Location = new Point(0, 35);
        //    ////tb_source.Size = new Size(1056, 162);
        //    ////tb_source.Name = "tb_source";
        //    ////tb_source.TabIndex = 3;
        //    ////tb_source.TabWidth = 2;
        //    ////tb_source.UseTabs = false;

        //    //pan_top = new Panel();
        //    //pan_top.SuspendLayout();
        //    //pan_top.Controls.Add(tb_source);
        //    //pan_top.Controls.Add(pan_button);
        //    //pan_top.Dock = DockStyle.Top;
        //    //pan_top.Location = new Point(0, 24);
        //    //pan_top.Size = new Size(1056, 197);
        //    ////pan_top.Name = "pan_top";
        //    ////pan_top.TabIndex = 0;
        //    //pan_top.ResumeLayout(false);

        //    //split_top = new Splitter();
        //    //split_top.Dock = DockStyle.Top;
        //    //split_top.Location = new Point(0, 221);
        //    //split_top.Name = "split_top";
        //    //split_top.Size = new Size(1056, 3);
        //    //split_top.TabIndex = 1;
        //    //split_top.TabStop = false;
        //}

        //private void CreatePanButton()
        //{
        //    //lb_progress_label = new Label();
        //    //lb_progress_label.AutoSize = true;
        //    //lb_progress_label.Location = new Point(624, 1);
        //    //lb_progress_label.Size = new Size(47, 13);
        //    //lb_progress_label.Text = "progress";
        //    //lb_progress_label.TextAlign = ContentAlignment.MiddleCenter;
        //    ////lb_progress_label.Name = "lb_progress_label";
        //    ////lb_progress_label.TabIndex = 1;

        //    //pb_progress_bar = new ProgressBar();
        //    //pb_progress_bar.Location = new Point(622, 16);
        //    //pb_progress_bar.Size = new Size(422, 16);
        //    ////pb_progress_bar.Name = "pb_progress_bar";
        //    ////pb_progress_bar.TabIndex = 0;

        //    //bt_execute = zForm.CreateButton("&Run", x: 3, y: 3, width: 75, height: 23, onClick: bt_execute_Click);
        //    //bt_pause = zForm.CreateButton("&Pause", x: 87, y: 3, width: 75, height: 23, onClick: bt_pause_Click);
        //    //bt_stop = zForm.CreateButton("&Stop", x: 171, y: 3, width: 75, height: 23, onClick: bt_stop_Click);

        //    //pan_button = new Panel();
        //    //pan_button.SuspendLayout();
        //    //pan_button.BackColor = SystemColors.Control;
        //    //pan_button.Controls.Add(bt_execute);
        //    //pan_button.Controls.Add(bt_pause);
        //    //pan_button.Controls.Add(bt_stop);
        //    //pan_button.Controls.Add(lb_progress_label);
        //    //pan_button.Controls.Add(pb_progress_bar);
        //    //pan_button.Dock = DockStyle.Top;
        //    //pan_button.Location = new Point(0, 0);
        //    //pan_button.Size = new Size(1056, 35);

        //    //this.pan_button.ResumeLayout(false);
        //    //this.pan_button.PerformLayout();
        //}
    }
}
