namespace Test
{
    partial class Form2
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
            this.tc_result = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.split_top = new System.Windows.Forms.Splitter();
            this.tb_source = new System.Windows.Forms.TextBox();
            this.tc_result.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan_top
            // 
            this.pan_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_top.Location = new System.Drawing.Point(0, 0);
            this.pan_top.Name = "pan_top";
            this.pan_top.Size = new System.Drawing.Size(878, 63);
            this.pan_top.TabIndex = 0;
            // 
            // tc_result
            // 
            this.tc_result.Controls.Add(this.tabPage1);
            this.tc_result.Controls.Add(this.tabPage2);
            this.tc_result.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tc_result.Location = new System.Drawing.Point(0, 293);
            this.tc_result.Name = "tc_result";
            this.tc_result.SelectedIndex = 0;
            this.tc_result.Size = new System.Drawing.Size(878, 295);
            this.tc_result.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(870, 269);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(437, 172);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // split_top
            // 
            this.split_top.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.split_top.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.split_top.Location = new System.Drawing.Point(0, 290);
            this.split_top.Name = "split_top";
            this.split_top.Size = new System.Drawing.Size(878, 3);
            this.split_top.TabIndex = 2;
            this.split_top.TabStop = false;
            // 
            // tb_source
            // 
            this.tb_source.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_source.Location = new System.Drawing.Point(0, 63);
            this.tb_source.Multiline = true;
            this.tb_source.Name = "tb_source";
            this.tb_source.Size = new System.Drawing.Size(878, 227);
            this.tb_source.TabIndex = 3;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 588);
            this.Controls.Add(this.tb_source);
            this.Controls.Add(this.split_top);
            this.Controls.Add(this.tc_result);
            this.Controls.Add(this.pan_top);
            this.Name = "Form2";
            this.Text = "Form2";
            this.tc_result.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.TabControl tc_result;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Splitter split_top;
        private System.Windows.Forms.TextBox tb_source;
    }
}