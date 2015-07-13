namespace pb.Windows.Forms
{
    partial class LogForm
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
            this.pan_progress = new System.Windows.Forms.Panel();
            this.pb_progress_bar = new System.Windows.Forms.ProgressBar();
            this.lb_progress_label = new System.Windows.Forms.Label();
            this.split_top2 = new System.Windows.Forms.Splitter();
            this.pan_fill = new System.Windows.Forms.Panel();
            this.pan_progress.SuspendLayout();
            this.pan_fill.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan_top
            // 
            this.pan_top.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pan_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_top.Location = new System.Drawing.Point(0, 0);
            this.pan_top.Name = "pan_top";
            this.pan_top.Size = new System.Drawing.Size(759, 26);
            this.pan_top.TabIndex = 0;
            // 
            // split_top
            // 
            this.split_top.BackColor = System.Drawing.SystemColors.ControlDark;
            this.split_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.split_top.Location = new System.Drawing.Point(0, 26);
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
            this.tb_log.Size = new System.Drawing.Size(759, 562);
            this.tb_log.TabIndex = 2;
            this.tb_log.WordWrap = false;
            // 
            // pan_progress
            // 
            this.pan_progress.Controls.Add(this.pb_progress_bar);
            this.pan_progress.Controls.Add(this.lb_progress_label);
            this.pan_progress.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_progress.Location = new System.Drawing.Point(0, 29);
            this.pan_progress.Name = "pan_progress";
            this.pan_progress.Size = new System.Drawing.Size(759, 31);
            this.pan_progress.TabIndex = 3;
            this.pan_progress.Visible = false;
            // 
            // pb_progress_bar
            // 
            this.pb_progress_bar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb_progress_bar.Location = new System.Drawing.Point(0, 13);
            this.pb_progress_bar.Name = "pb_progress_bar";
            this.pb_progress_bar.Size = new System.Drawing.Size(759, 18);
            this.pb_progress_bar.TabIndex = 2;
            // 
            // lb_progress_label
            // 
            this.lb_progress_label.AutoSize = true;
            this.lb_progress_label.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_progress_label.Location = new System.Drawing.Point(0, 0);
            this.lb_progress_label.Name = "lb_progress_label";
            this.lb_progress_label.Size = new System.Drawing.Size(29, 13);
            this.lb_progress_label.TabIndex = 0;
            this.lb_progress_label.Text = "label";
            // 
            // split_top2
            // 
            this.split_top2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.split_top2.Dock = System.Windows.Forms.DockStyle.Top;
            this.split_top2.Location = new System.Drawing.Point(0, 60);
            this.split_top2.Name = "split_top2";
            this.split_top2.Size = new System.Drawing.Size(759, 3);
            this.split_top2.TabIndex = 4;
            this.split_top2.TabStop = false;
            // 
            // pan_fill
            // 
            this.pan_fill.Controls.Add(this.tb_log);
            this.pan_fill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pan_fill.Location = new System.Drawing.Point(0, 63);
            this.pan_fill.Name = "pan_fill";
            this.pan_fill.Size = new System.Drawing.Size(759, 562);
            this.pan_fill.TabIndex = 5;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 625);
            this.Controls.Add(this.pan_fill);
            this.Controls.Add(this.split_top2);
            this.Controls.Add(this.pan_progress);
            this.Controls.Add(this.split_top);
            this.Controls.Add(this.pan_top);
            this.KeyPreview = true;
            this.Name = "LogForm";
            this.ShowInTaskbar = false;
            this.Text = "Log";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogForm_FormClosed);
            this.pan_progress.ResumeLayout(false);
            this.pan_progress.PerformLayout();
            this.pan_fill.ResumeLayout(false);
            this.pan_fill.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.Splitter split_top;
        private System.Windows.Forms.TextBox tb_log;
        private System.Windows.Forms.Panel pan_progress;
        private System.Windows.Forms.Splitter split_top2;
        private System.Windows.Forms.Panel pan_fill;
        private System.Windows.Forms.Label lb_progress_label;
        private System.Windows.Forms.ProgressBar pb_progress_bar;
    }
}