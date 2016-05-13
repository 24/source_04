namespace Test.Test_Form.Test
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
            this.tbTextBox1 = new System.Windows.Forms.TextBox();
            this.lbInstructions = new System.Windows.Forms.Label();
            this.btExecute = new System.Windows.Forms.Button();
            this.panLeft = new System.Windows.Forms.Panel();
            this.panInstructions = new System.Windows.Forms.Panel();
            this.panToolbar = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.splitPanelLeft = new System.Windows.Forms.Splitter();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panLeft.SuspendLayout();
            this.panInstructions.SuspendLayout();
            this.panToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbTextBox1
            // 
            this.tbTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTextBox1.Location = new System.Drawing.Point(0, 13);
            this.tbTextBox1.Multiline = true;
            this.tbTextBox1.Name = "tbTextBox1";
            this.tbTextBox1.Size = new System.Drawing.Size(345, 561);
            this.tbTextBox1.TabIndex = 0;
            // 
            // lbInstructions
            // 
            this.lbInstructions.AutoSize = true;
            this.lbInstructions.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbInstructions.Location = new System.Drawing.Point(0, 0);
            this.lbInstructions.Name = "lbInstructions";
            this.lbInstructions.Size = new System.Drawing.Size(60, 13);
            this.lbInstructions.TabIndex = 1;
            this.lbInstructions.Text = "instructions";
            // 
            // btExecute
            // 
            this.btExecute.Dock = System.Windows.Forms.DockStyle.Left;
            this.btExecute.Location = new System.Drawing.Point(0, 0);
            this.btExecute.Name = "btExecute";
            this.btExecute.Size = new System.Drawing.Size(75, 31);
            this.btExecute.TabIndex = 2;
            this.btExecute.Text = "&Execute";
            this.btExecute.UseVisualStyleBackColor = true;
            this.btExecute.Click += new System.EventHandler(this.btExecute_Click);
            // 
            // panLeft
            // 
            this.panLeft.Controls.Add(this.panInstructions);
            this.panLeft.Controls.Add(this.panToolbar);
            this.panLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panLeft.Location = new System.Drawing.Point(0, 24);
            this.panLeft.MinimumSize = new System.Drawing.Size(300, 0);
            this.panLeft.Name = "panLeft";
            this.panLeft.Size = new System.Drawing.Size(345, 605);
            this.panLeft.TabIndex = 3;
            // 
            // panInstructions
            // 
            this.panInstructions.Controls.Add(this.tbTextBox1);
            this.panInstructions.Controls.Add(this.lbInstructions);
            this.panInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panInstructions.Location = new System.Drawing.Point(0, 31);
            this.panInstructions.Name = "panInstructions";
            this.panInstructions.Size = new System.Drawing.Size(345, 574);
            this.panInstructions.TabIndex = 4;
            // 
            // panToolbar
            // 
            this.panToolbar.Controls.Add(this.button1);
            this.panToolbar.Controls.Add(this.btExecute);
            this.panToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panToolbar.Location = new System.Drawing.Point(0, 0);
            this.panToolbar.Name = "panToolbar";
            this.panToolbar.Size = new System.Drawing.Size(345, 31);
            this.panToolbar.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Left;
            this.button1.Location = new System.Drawing.Point(75, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 31);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // splitPanelLeft
            // 
            this.splitPanelLeft.BackColor = System.Drawing.SystemColors.ControlText;
            this.splitPanelLeft.Location = new System.Drawing.Point(345, 24);
            this.splitPanelLeft.Name = "splitPanelLeft";
            this.splitPanelLeft.Size = new System.Drawing.Size(3, 605);
            this.splitPanelLeft.TabIndex = 4;
            this.splitPanelLeft.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "lskd jqslk dsqd",
            "qsd q",
            "sd qsd ",
            "",
            "qsd ",
            "qsd ",
            "qsd qs",
            "d qsd ",
            "qsd ",
            "qs"});
            this.listBox1.Location = new System.Drawing.Point(453, 121);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(338, 407);
            this.listBox1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 629);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.splitPanelLeft);
            this.Controls.Add(this.panLeft);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.panLeft.ResumeLayout(false);
            this.panInstructions.ResumeLayout(false);
            this.panInstructions.PerformLayout();
            this.panToolbar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbTextBox1;
        private System.Windows.Forms.Label lbInstructions;
        private System.Windows.Forms.Button btExecute;
        private System.Windows.Forms.Panel panLeft;
        private System.Windows.Forms.Panel panInstructions;
        private System.Windows.Forms.Panel panToolbar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Splitter splitPanelLeft;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ListBox listBox1;
    }
}