namespace Pib
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pan_top = new System.Windows.Forms.Panel();
            this.split_top = new System.Windows.Forms.Splitter();
            this.tb_log = new System.Windows.Forms.TextBox();
            this.task1_pan_progress = new System.Windows.Forms.Panel();
            this.task1_pb_progress_bar2 = new System.Windows.Forms.ProgressBar();
            this.task1_lb_progress_label2 = new System.Windows.Forms.Label();
            this.task1_pb_progress_bar1 = new System.Windows.Forms.ProgressBar();
            this.task1_lb_progress_label1 = new System.Windows.Forms.Label();
            this.task1_pan_log = new System.Windows.Forms.Panel();
            this.task1_tb_log = new System.Windows.Forms.TextBox();
            this.pan_fill = new System.Windows.Forms.Panel();
            this.pan_task1 = new System.Windows.Forms.Panel();
            this.task1_split_progress = new System.Windows.Forms.Splitter();
            this.split_task1 = new System.Windows.Forms.Splitter();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.m_file = new System.Windows.Forms.ToolStripMenuItem();
            this.m_trace_winproc = new System.Windows.Forms.ToolStripMenuItem();
            this.task1_pan_progress.SuspendLayout();
            this.task1_pan_log.SuspendLayout();
            this.pan_fill.SuspendLayout();
            this.pan_task1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan_top
            // 
            this.pan_top.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pan_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_top.Location = new System.Drawing.Point(0, 24);
            this.pan_top.Name = "pan_top";
            this.pan_top.Size = new System.Drawing.Size(759, 26);
            this.pan_top.TabIndex = 0;
            // 
            // split_top
            // 
            this.split_top.BackColor = System.Drawing.SystemColors.ControlDark;
            this.split_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.split_top.Location = new System.Drawing.Point(0, 50);
            this.split_top.Name = "split_top";
            this.split_top.Size = new System.Drawing.Size(759, 3);
            this.split_top.TabIndex = 1;
            this.split_top.TabStop = false;
            // 
            // tb_log
            // 
            this.tb_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_log.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_log.Location = new System.Drawing.Point(0, 0);
            this.tb_log.Multiline = true;
            this.tb_log.Name = "tb_log";
            this.tb_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_log.Size = new System.Drawing.Size(759, 320);
            this.tb_log.TabIndex = 2;
            this.tb_log.WordWrap = false;
            // 
            // task1_pan_progress
            // 
            this.task1_pan_progress.Controls.Add(this.task1_pb_progress_bar2);
            this.task1_pan_progress.Controls.Add(this.task1_lb_progress_label2);
            this.task1_pan_progress.Controls.Add(this.task1_pb_progress_bar1);
            this.task1_pan_progress.Controls.Add(this.task1_lb_progress_label1);
            this.task1_pan_progress.Dock = System.Windows.Forms.DockStyle.Top;
            this.task1_pan_progress.Location = new System.Drawing.Point(0, 0);
            this.task1_pan_progress.Name = "task1_pan_progress";
            this.task1_pan_progress.Size = new System.Drawing.Size(759, 66);
            this.task1_pan_progress.TabIndex = 3;
            // 
            // task1_pb_progress_bar2
            // 
            this.task1_pb_progress_bar2.Dock = System.Windows.Forms.DockStyle.Top;
            this.task1_pb_progress_bar2.Location = new System.Drawing.Point(0, 46);
            this.task1_pb_progress_bar2.Name = "task1_pb_progress_bar2";
            this.task1_pb_progress_bar2.Size = new System.Drawing.Size(759, 20);
            this.task1_pb_progress_bar2.TabIndex = 4;
            // 
            // task1_lb_progress_label2
            // 
            this.task1_lb_progress_label2.AutoSize = true;
            this.task1_lb_progress_label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.task1_lb_progress_label2.Location = new System.Drawing.Point(0, 33);
            this.task1_lb_progress_label2.Name = "task1_lb_progress_label2";
            this.task1_lb_progress_label2.Size = new System.Drawing.Size(29, 13);
            this.task1_lb_progress_label2.TabIndex = 3;
            this.task1_lb_progress_label2.Text = "label";
            // 
            // task1_pb_progress_bar1
            // 
            this.task1_pb_progress_bar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.task1_pb_progress_bar1.Location = new System.Drawing.Point(0, 13);
            this.task1_pb_progress_bar1.Name = "task1_pb_progress_bar1";
            this.task1_pb_progress_bar1.Size = new System.Drawing.Size(759, 20);
            this.task1_pb_progress_bar1.TabIndex = 2;
            // 
            // task1_lb_progress_label1
            // 
            this.task1_lb_progress_label1.AutoSize = true;
            this.task1_lb_progress_label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.task1_lb_progress_label1.Location = new System.Drawing.Point(0, 0);
            this.task1_lb_progress_label1.Name = "task1_lb_progress_label1";
            this.task1_lb_progress_label1.Size = new System.Drawing.Size(29, 13);
            this.task1_lb_progress_label1.TabIndex = 0;
            this.task1_lb_progress_label1.Text = "label";
            // 
            // task1_pan_log
            // 
            this.task1_pan_log.Controls.Add(this.task1_tb_log);
            this.task1_pan_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.task1_pan_log.Location = new System.Drawing.Point(0, 66);
            this.task1_pan_log.Name = "task1_pan_log";
            this.task1_pan_log.Size = new System.Drawing.Size(759, 183);
            this.task1_pan_log.TabIndex = 5;
            // 
            // task1_tb_log
            // 
            this.task1_tb_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.task1_tb_log.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.task1_tb_log.Location = new System.Drawing.Point(0, 0);
            this.task1_tb_log.Multiline = true;
            this.task1_tb_log.Name = "task1_tb_log";
            this.task1_tb_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.task1_tb_log.Size = new System.Drawing.Size(759, 183);
            this.task1_tb_log.TabIndex = 3;
            this.task1_tb_log.WordWrap = false;
            // 
            // pan_fill
            // 
            this.pan_fill.Controls.Add(this.tb_log);
            this.pan_fill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_fill.Location = new System.Drawing.Point(0, 305);
            this.pan_fill.Name = "pan_fill";
            this.pan_fill.Size = new System.Drawing.Size(759, 320);
            this.pan_fill.TabIndex = 7;
            // 
            // pan_task1
            // 
            this.pan_task1.Controls.Add(this.task1_split_progress);
            this.pan_task1.Controls.Add(this.task1_pan_log);
            this.pan_task1.Controls.Add(this.task1_pan_progress);
            this.pan_task1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_task1.Location = new System.Drawing.Point(0, 53);
            this.pan_task1.Name = "pan_task1";
            this.pan_task1.Size = new System.Drawing.Size(759, 249);
            this.pan_task1.TabIndex = 8;
            this.pan_task1.Visible = false;
            // 
            // task1_split_progress
            // 
            this.task1_split_progress.BackColor = System.Drawing.SystemColors.ControlDark;
            this.task1_split_progress.Dock = System.Windows.Forms.DockStyle.Top;
            this.task1_split_progress.Location = new System.Drawing.Point(0, 66);
            this.task1_split_progress.Name = "task1_split_progress";
            this.task1_split_progress.Size = new System.Drawing.Size(759, 3);
            this.task1_split_progress.TabIndex = 4;
            this.task1_split_progress.TabStop = false;
            // 
            // split_task1
            // 
            this.split_task1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.split_task1.Dock = System.Windows.Forms.DockStyle.Top;
            this.split_task1.Location = new System.Drawing.Point(0, 302);
            this.split_task1.Name = "split_task1";
            this.split_task1.Size = new System.Drawing.Size(759, 3);
            this.split_task1.TabIndex = 9;
            this.split_task1.TabStop = false;
            this.split_task1.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_file});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(759, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // m_file
            // 
            this.m_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_trace_winproc});
            this.m_file.Name = "m_file";
            this.m_file.Size = new System.Drawing.Size(35, 20);
            this.m_file.Text = "&File";
            // 
            // m_trace_winproc
            // 
            this.m_trace_winproc.Name = "m_trace_winproc";
            this.m_trace_winproc.Size = new System.Drawing.Size(154, 22);
            this.m_trace_winproc.Text = "&Trace WinProc";
            this.m_trace_winproc.Click += new System.EventHandler(this.m_trace_winproc_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 625);
            this.Controls.Add(this.pan_fill);
            this.Controls.Add(this.split_task1);
            this.Controls.Add(this.pan_task1);
            this.Controls.Add(this.split_top);
            this.Controls.Add(this.pan_top);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.Text = "Pib";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.task1_pan_progress.ResumeLayout(false);
            this.task1_pan_progress.PerformLayout();
            this.task1_pan_log.ResumeLayout(false);
            this.task1_pan_log.PerformLayout();
            this.pan_fill.ResumeLayout(false);
            this.pan_fill.PerformLayout();
            this.pan_task1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.Splitter split_top;
        private System.Windows.Forms.TextBox tb_log;
        private System.Windows.Forms.Panel task1_pan_progress;
        private System.Windows.Forms.Panel task1_pan_log;
        private System.Windows.Forms.Label task1_lb_progress_label1;
        private System.Windows.Forms.ProgressBar task1_pb_progress_bar1;
        private System.Windows.Forms.Panel pan_fill;
        private System.Windows.Forms.TextBox task1_tb_log;
        private System.Windows.Forms.Panel pan_task1;
        private System.Windows.Forms.Splitter task1_split_progress;
        private System.Windows.Forms.Splitter split_task1;
        private System.Windows.Forms.ProgressBar task1_pb_progress_bar2;
        private System.Windows.Forms.Label task1_lb_progress_label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem m_file;
        private System.Windows.Forms.ToolStripMenuItem m_trace_winproc;
    }
}