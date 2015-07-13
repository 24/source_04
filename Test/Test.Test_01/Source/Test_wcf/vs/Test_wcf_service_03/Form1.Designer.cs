namespace Test_wcf_service
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
            this.bt_start_service = new System.Windows.Forms.Button();
            this.bt_stop_service = new System.Windows.Forms.Button();
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
            // bt_start_service
            // 
            this.bt_start_service.Location = new System.Drawing.Point(57, 43);
            this.bt_start_service.Name = "bt_start_service";
            this.bt_start_service.Size = new System.Drawing.Size(239, 43);
            this.bt_start_service.TabIndex = 1;
            this.bt_start_service.Text = "Start service";
            this.bt_start_service.UseVisualStyleBackColor = true;
            this.bt_start_service.Click += new System.EventHandler(this.bt_start_service_Click);
            // 
            // bt_stop_service
            // 
            this.bt_stop_service.Location = new System.Drawing.Point(384, 43);
            this.bt_stop_service.Name = "bt_stop_service";
            this.bt_stop_service.Size = new System.Drawing.Size(239, 43);
            this.bt_stop_service.TabIndex = 2;
            this.bt_stop_service.Text = "Stop service";
            this.bt_stop_service.UseVisualStyleBackColor = true;
            this.bt_stop_service.Click += new System.EventHandler(this.bt_stop_service_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1089, 572);
            this.Controls.Add(this.bt_stop_service);
            this.Controls.Add(this.bt_start_service);
            this.Controls.Add(this.tb_log);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_log;
        private System.Windows.Forms.Button bt_start_service;
        private System.Windows.Forms.Button bt_stop_service;
    }
}

