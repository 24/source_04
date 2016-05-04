namespace Test_ScintillaNET_01
{
    partial class Form1
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
            this.scintilla1 = new ScintillaNET.Scintilla();
            this.pan_top = new System.Windows.Forms.Panel();
            this.bt_next_bookmark = new System.Windows.Forms.Button();
            this.bt_previous_bookmark = new System.Windows.Forms.Button();
            this.pan_bottom = new System.Windows.Forms.Panel();
            this.tb_message = new System.Windows.Forms.TextBox();
            this.split_bottom = new System.Windows.Forms.Splitter();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.m_file = new System.Windows.Forms.ToolStripMenuItem();
            this.m_open = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_quit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.m_find = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.m_previous_bookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.m_next_bookmark = new System.Windows.Forms.ToolStripMenuItem();
            this.pan_status = new System.Windows.Forms.Panel();
            this.tb_status = new System.Windows.Forms.TextBox();
            this.pan_top.SuspendLayout();
            this.pan_bottom.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pan_status.SuspendLayout();
            this.SuspendLayout();
            // 
            // scintilla1
            // 
            this.scintilla1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla1.IndentWidth = 2;
            this.scintilla1.Location = new System.Drawing.Point(0, 54);
            this.scintilla1.Name = "scintilla1";
            this.scintilla1.Size = new System.Drawing.Size(1048, 439);
            this.scintilla1.TabIndex = 0;
            this.scintilla1.TabWidth = 2;
            this.scintilla1.UseTabs = false;
            // 
            // pan_top
            // 
            this.pan_top.Controls.Add(this.bt_next_bookmark);
            this.pan_top.Controls.Add(this.bt_previous_bookmark);
            this.pan_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_top.Location = new System.Drawing.Point(0, 24);
            this.pan_top.Name = "pan_top";
            this.pan_top.Size = new System.Drawing.Size(1048, 30);
            this.pan_top.TabIndex = 1;
            // 
            // bt_next_bookmark
            // 
            this.bt_next_bookmark.Dock = System.Windows.Forms.DockStyle.Left;
            this.bt_next_bookmark.Location = new System.Drawing.Point(154, 0);
            this.bt_next_bookmark.Name = "bt_next_bookmark";
            this.bt_next_bookmark.Size = new System.Drawing.Size(154, 30);
            this.bt_next_bookmark.TabIndex = 0;
            this.bt_next_bookmark.Text = "&Next bookmark";
            this.bt_next_bookmark.UseVisualStyleBackColor = true;
            this.bt_next_bookmark.Click += new System.EventHandler(this.bt_next_bookmark_Click);
            // 
            // bt_previous_bookmark
            // 
            this.bt_previous_bookmark.Dock = System.Windows.Forms.DockStyle.Left;
            this.bt_previous_bookmark.Location = new System.Drawing.Point(0, 0);
            this.bt_previous_bookmark.Name = "bt_previous_bookmark";
            this.bt_previous_bookmark.Size = new System.Drawing.Size(154, 30);
            this.bt_previous_bookmark.TabIndex = 1;
            this.bt_previous_bookmark.Text = "&Previous bookmark";
            this.bt_previous_bookmark.UseVisualStyleBackColor = true;
            this.bt_previous_bookmark.Click += new System.EventHandler(this.bt_previous_bookmark_Click);
            // 
            // pan_bottom
            // 
            this.pan_bottom.Controls.Add(this.tb_message);
            this.pan_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pan_bottom.Location = new System.Drawing.Point(0, 503);
            this.pan_bottom.Name = "pan_bottom";
            this.pan_bottom.Size = new System.Drawing.Size(1048, 136);
            this.pan_bottom.TabIndex = 2;
            // 
            // tb_message
            // 
            this.tb_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_message.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_message.Location = new System.Drawing.Point(0, 0);
            this.tb_message.Multiline = true;
            this.tb_message.Name = "tb_message";
            this.tb_message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_message.Size = new System.Drawing.Size(1048, 136);
            this.tb_message.TabIndex = 0;
            // 
            // split_bottom
            // 
            this.split_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.split_bottom.Location = new System.Drawing.Point(0, 493);
            this.split_bottom.Name = "split_bottom";
            this.split_bottom.Size = new System.Drawing.Size(1048, 10);
            this.split_bottom.TabIndex = 3;
            this.split_bottom.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_file,
            this.m_edit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1048, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // m_file
            // 
            this.m_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_open,
            this.toolStripSeparator1,
            this.m_quit});
            this.m_file.Name = "m_file";
            this.m_file.Size = new System.Drawing.Size(37, 20);
            this.m_file.Text = "&File";
            // 
            // m_open
            // 
            this.m_open.Name = "m_open";
            this.m_open.Size = new System.Drawing.Size(103, 22);
            this.m_open.Text = "&Open";
            this.m_open.Click += new System.EventHandler(this.m_open_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // m_quit
            // 
            this.m_quit.Name = "m_quit";
            this.m_quit.Size = new System.Drawing.Size(103, 22);
            this.m_quit.Text = "&Quit";
            this.m_quit.Click += new System.EventHandler(this.m_quit_Click);
            // 
            // m_edit
            // 
            this.m_edit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_find,
            this.toolStripSeparator2,
            this.m_previous_bookmark,
            this.m_next_bookmark});
            this.m_edit.Name = "m_edit";
            this.m_edit.Size = new System.Drawing.Size(39, 20);
            this.m_edit.Text = "&Edit";
            // 
            // m_find
            // 
            this.m_find.Name = "m_find";
            this.m_find.Size = new System.Drawing.Size(176, 22);
            this.m_find.Text = "&Find";
            this.m_find.Click += new System.EventHandler(this.m_find_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(173, 6);
            // 
            // m_previous_bookmark
            // 
            this.m_previous_bookmark.Name = "m_previous_bookmark";
            this.m_previous_bookmark.Size = new System.Drawing.Size(176, 22);
            this.m_previous_bookmark.Text = "&Previous bookmark";
            this.m_previous_bookmark.Click += new System.EventHandler(this.m_previous_bookmark_Click);
            // 
            // m_next_bookmark
            // 
            this.m_next_bookmark.Name = "m_next_bookmark";
            this.m_next_bookmark.Size = new System.Drawing.Size(176, 22);
            this.m_next_bookmark.Text = "&Next bookmark";
            this.m_next_bookmark.Click += new System.EventHandler(this.m_next_bookmark_Click);
            // 
            // pan_status
            // 
            this.pan_status.Controls.Add(this.tb_status);
            this.pan_status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pan_status.Location = new System.Drawing.Point(0, 639);
            this.pan_status.Name = "pan_status";
            this.pan_status.Size = new System.Drawing.Size(1048, 20);
            this.pan_status.TabIndex = 5;
            // 
            // tb_status
            // 
            this.tb_status.BackColor = System.Drawing.SystemColors.HotTrack;
            this.tb_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_status.ForeColor = System.Drawing.SystemColors.Window;
            this.tb_status.Location = new System.Drawing.Point(0, 0);
            this.tb_status.Name = "tb_status";
            this.tb_status.Size = new System.Drawing.Size(1048, 20);
            this.tb_status.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 659);
            this.Controls.Add(this.scintilla1);
            this.Controls.Add(this.split_bottom);
            this.Controls.Add(this.pan_bottom);
            this.Controls.Add(this.pan_status);
            this.Controls.Add(this.pan_top);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.pan_top.ResumeLayout(false);
            this.pan_bottom.ResumeLayout(false);
            this.pan_bottom.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pan_status.ResumeLayout(false);
            this.pan_status.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ScintillaNET.Scintilla scintilla1;
        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.Button bt_next_bookmark;
        private System.Windows.Forms.Button bt_previous_bookmark;
        private System.Windows.Forms.Panel pan_bottom;
        private System.Windows.Forms.TextBox tb_message;
        private System.Windows.Forms.Splitter split_bottom;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem m_file;
        private System.Windows.Forms.ToolStripMenuItem m_open;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem m_quit;
        private System.Windows.Forms.ToolStripMenuItem m_edit;
        private System.Windows.Forms.ToolStripMenuItem m_find;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem m_previous_bookmark;
        private System.Windows.Forms.ToolStripMenuItem m_next_bookmark;
        private System.Windows.Forms.Panel pan_status;
        private System.Windows.Forms.TextBox tb_status;
    }
}

