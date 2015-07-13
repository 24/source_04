namespace Test_RunSource
{
    partial class Test_RunSourceForm
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
            this.m_menu = new System.Windows.Forms.MenuStrip();
            this.m_file = new System.Windows.Forms.ToolStripMenuItem();
            this.m_new = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.m_quit = new System.Windows.Forms.ToolStripMenuItem();
            this.pan_top = new System.Windows.Forms.Panel();
            this.progress_bar = new System.Windows.Forms.ProgressBar();
            this.bt_pause = new System.Windows.Forms.Button();
            this.bt_run = new System.Windows.Forms.Button();
            this.pan_edit = new System.Windows.Forms.Panel();
            this.tb_source = new System.Windows.Forms.TextBox();
            this.split_edit = new System.Windows.Forms.Splitter();
            this.pan_result = new System.Windows.Forms.Panel();
            this.tc_result = new System.Windows.Forms.TabControl();
            this.tab_result1 = new System.Windows.Forms.TabPage();
            this.tab_result2 = new System.Windows.Forms.TabPage();
            this.tab_result3 = new System.Windows.Forms.TabPage();
            this.tab_result4 = new System.Windows.Forms.TabPage();
            this.tab_message = new System.Windows.Forms.TabPage();
            this.tb_message = new System.Windows.Forms.TextBox();
            this.m_menu.SuspendLayout();
            this.pan_top.SuspendLayout();
            this.pan_edit.SuspendLayout();
            this.pan_result.SuspendLayout();
            this.tc_result.SuspendLayout();
            this.tab_message.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_menu
            // 
            this.m_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_file});
            this.m_menu.Location = new System.Drawing.Point(0, 0);
            this.m_menu.Name = "m_menu";
            this.m_menu.Size = new System.Drawing.Size(1232, 24);
            this.m_menu.TabIndex = 0;
            this.m_menu.Text = "menuStrip1";
            // 
            // m_file
            // 
            this.m_file.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_new,
            this.toolStripSeparator1,
            this.m_quit});
            this.m_file.Name = "m_file";
            this.m_file.Size = new System.Drawing.Size(35, 20);
            this.m_file.Text = "&File";
            // 
            // m_new
            // 
            this.m_new.Name = "m_new";
            this.m_new.Size = new System.Drawing.Size(95, 22);
            this.m_new.Text = "&New";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(92, 6);
            // 
            // m_quit
            // 
            this.m_quit.Name = "m_quit";
            this.m_quit.Size = new System.Drawing.Size(95, 22);
            this.m_quit.Text = "&Quit";
            // 
            // pan_top
            // 
            this.pan_top.Controls.Add(this.progress_bar);
            this.pan_top.Controls.Add(this.bt_pause);
            this.pan_top.Controls.Add(this.bt_run);
            this.pan_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_top.Location = new System.Drawing.Point(0, 24);
            this.pan_top.Name = "pan_top";
            this.pan_top.Size = new System.Drawing.Size(1232, 43);
            this.pan_top.TabIndex = 1;
            // 
            // progress_bar
            // 
            this.progress_bar.Location = new System.Drawing.Point(246, 9);
            this.progress_bar.Name = "progress_bar";
            this.progress_bar.Size = new System.Drawing.Size(563, 23);
            this.progress_bar.TabIndex = 2;
            // 
            // bt_pause
            // 
            this.bt_pause.Location = new System.Drawing.Point(95, 9);
            this.bt_pause.Name = "bt_pause";
            this.bt_pause.Size = new System.Drawing.Size(75, 23);
            this.bt_pause.TabIndex = 1;
            this.bt_pause.Text = "&Pause";
            this.bt_pause.UseVisualStyleBackColor = true;
            // 
            // bt_run
            // 
            this.bt_run.Location = new System.Drawing.Point(14, 9);
            this.bt_run.Name = "bt_run";
            this.bt_run.Size = new System.Drawing.Size(75, 23);
            this.bt_run.TabIndex = 0;
            this.bt_run.Text = "&Run";
            this.bt_run.UseVisualStyleBackColor = true;
            this.bt_run.Click += new System.EventHandler(this.bt_run_Click);
            // 
            // pan_edit
            // 
            this.pan_edit.Controls.Add(this.tb_source);
            this.pan_edit.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_edit.Location = new System.Drawing.Point(0, 67);
            this.pan_edit.Name = "pan_edit";
            this.pan_edit.Size = new System.Drawing.Size(1232, 265);
            this.pan_edit.TabIndex = 2;
            // 
            // tb_source
            // 
            this.tb_source.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_source.Location = new System.Drawing.Point(0, 0);
            this.tb_source.Multiline = true;
            this.tb_source.Name = "tb_source";
            this.tb_source.Size = new System.Drawing.Size(1232, 265);
            this.tb_source.TabIndex = 0;
            // 
            // split_edit
            // 
            this.split_edit.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.split_edit.Dock = System.Windows.Forms.DockStyle.Top;
            this.split_edit.Location = new System.Drawing.Point(0, 332);
            this.split_edit.Name = "split_edit";
            this.split_edit.Size = new System.Drawing.Size(1232, 5);
            this.split_edit.TabIndex = 3;
            this.split_edit.TabStop = false;
            // 
            // pan_result
            // 
            this.pan_result.Controls.Add(this.tc_result);
            this.pan_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_result.Location = new System.Drawing.Point(0, 337);
            this.pan_result.Name = "pan_result";
            this.pan_result.Size = new System.Drawing.Size(1232, 303);
            this.pan_result.TabIndex = 4;
            // 
            // tc_result
            // 
            this.tc_result.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tc_result.Controls.Add(this.tab_result1);
            this.tc_result.Controls.Add(this.tab_result2);
            this.tc_result.Controls.Add(this.tab_result3);
            this.tc_result.Controls.Add(this.tab_result4);
            this.tc_result.Controls.Add(this.tab_message);
            this.tc_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc_result.Location = new System.Drawing.Point(0, 0);
            this.tc_result.Name = "tc_result";
            this.tc_result.SelectedIndex = 0;
            this.tc_result.Size = new System.Drawing.Size(1232, 303);
            this.tc_result.TabIndex = 0;
            // 
            // tab_result1
            // 
            this.tab_result1.Location = new System.Drawing.Point(4, 4);
            this.tab_result1.Name = "tab_result1";
            this.tab_result1.Padding = new System.Windows.Forms.Padding(3);
            this.tab_result1.Size = new System.Drawing.Size(1224, 277);
            this.tab_result1.TabIndex = 0;
            this.tab_result1.Text = "Results";
            this.tab_result1.UseVisualStyleBackColor = true;
            // 
            // tab_result2
            // 
            this.tab_result2.Location = new System.Drawing.Point(4, 4);
            this.tab_result2.Name = "tab_result2";
            this.tab_result2.Padding = new System.Windows.Forms.Padding(3);
            this.tab_result2.Size = new System.Drawing.Size(1224, 277);
            this.tab_result2.TabIndex = 1;
            this.tab_result2.Text = "Results";
            this.tab_result2.UseVisualStyleBackColor = true;
            // 
            // tab_result3
            // 
            this.tab_result3.Location = new System.Drawing.Point(4, 4);
            this.tab_result3.Name = "tab_result3";
            this.tab_result3.Padding = new System.Windows.Forms.Padding(3);
            this.tab_result3.Size = new System.Drawing.Size(1224, 277);
            this.tab_result3.TabIndex = 2;
            this.tab_result3.Text = "Results";
            this.tab_result3.UseVisualStyleBackColor = true;
            // 
            // tab_result4
            // 
            this.tab_result4.Location = new System.Drawing.Point(4, 4);
            this.tab_result4.Name = "tab_result4";
            this.tab_result4.Padding = new System.Windows.Forms.Padding(3);
            this.tab_result4.Size = new System.Drawing.Size(1224, 277);
            this.tab_result4.TabIndex = 3;
            this.tab_result4.Text = "Results";
            this.tab_result4.UseVisualStyleBackColor = true;
            // 
            // tab_message
            // 
            this.tab_message.Controls.Add(this.tb_message);
            this.tab_message.Location = new System.Drawing.Point(4, 4);
            this.tab_message.Name = "tab_message";
            this.tab_message.Padding = new System.Windows.Forms.Padding(3);
            this.tab_message.Size = new System.Drawing.Size(1224, 277);
            this.tab_message.TabIndex = 4;
            this.tab_message.Text = "Messages";
            this.tab_message.UseVisualStyleBackColor = true;
            // 
            // tb_message
            // 
            this.tb_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_message.Location = new System.Drawing.Point(3, 3);
            this.tb_message.Multiline = true;
            this.tb_message.Name = "tb_message";
            this.tb_message.Size = new System.Drawing.Size(1218, 271);
            this.tb_message.TabIndex = 0;
            // 
            // Test_RunSourceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1232, 640);
            this.Controls.Add(this.pan_result);
            this.Controls.Add(this.split_edit);
            this.Controls.Add(this.pan_edit);
            this.Controls.Add(this.pan_top);
            this.Controls.Add(this.m_menu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.m_menu;
            this.Name = "Test_RunSourceForm";
            this.Text = "Test_RunSourceForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Test_RunSourceForm_FormClosed);
            this.Load += new System.EventHandler(this.Test_RunSourceForm_Load);
            this.Shown += new System.EventHandler(this.Test_RunSourceForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Test_RunSourceForm_KeyDown);
            this.m_menu.ResumeLayout(false);
            this.m_menu.PerformLayout();
            this.pan_top.ResumeLayout(false);
            this.pan_edit.ResumeLayout(false);
            this.pan_edit.PerformLayout();
            this.pan_result.ResumeLayout(false);
            this.tc_result.ResumeLayout(false);
            this.tab_message.ResumeLayout(false);
            this.tab_message.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip m_menu;
        private System.Windows.Forms.ToolStripMenuItem m_file;
        private System.Windows.Forms.ToolStripMenuItem m_new;
        private System.Windows.Forms.ToolStripMenuItem m_quit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.Button bt_pause;
        private System.Windows.Forms.Button bt_run;
        private System.Windows.Forms.Panel pan_edit;
        private System.Windows.Forms.Splitter split_edit;
        private System.Windows.Forms.Panel pan_result;
        private System.Windows.Forms.ProgressBar progress_bar;
        private System.Windows.Forms.TextBox tb_source;
        private System.Windows.Forms.TabControl tc_result;
        private System.Windows.Forms.TabPage tab_result1;
        private System.Windows.Forms.TabPage tab_result2;
        private System.Windows.Forms.TabPage tab_result3;
        private System.Windows.Forms.TabPage tab_result4;
        private System.Windows.Forms.TabPage tab_message;
        private System.Windows.Forms.TextBox tb_message;
    }
}