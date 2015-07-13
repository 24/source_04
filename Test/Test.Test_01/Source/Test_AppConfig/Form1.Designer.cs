namespace Test.Test_AppConfig
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
            this.tb_log = new System.Windows.Forms.TextBox();
            this.bt_go = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tb_log
            // 
            this.tb_log.Location = new System.Drawing.Point(12, 293);
            this.tb_log.Multiline = true;
            this.tb_log.Name = "tb_log";
            this.tb_log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tb_log.Size = new System.Drawing.Size(1065, 267);
            this.tb_log.TabIndex = 0;
            // 
            // bt_go
            // 
            this.bt_go.Location = new System.Drawing.Point(57, 43);
            this.bt_go.Name = "bt_go";
            this.bt_go.Size = new System.Drawing.Size(239, 43);
            this.bt_go.TabIndex = 1;
            this.bt_go.Text = "&Go";
            this.bt_go.UseVisualStyleBackColor = true;
            this.bt_go.Click += new System.EventHandler(this.bt_go_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 572);
            this.Controls.Add(this.bt_go);
            this.Controls.Add(this.tb_log);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_log;
        private System.Windows.Forms.Button bt_go;
    }
}

