namespace pb.Windows.Forms
{
    partial class ScintillaFindForm
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
            this.tb_text = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_find_next = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tb_text
            // 
            this.tb_text.Location = new System.Drawing.Point(45, 23);
            this.tb_text.Name = "tb_text";
            this.tb_text.Size = new System.Drawing.Size(503, 20);
            this.tb_text.TabIndex = 0;
            this.tb_text.TextChanged += new System.EventHandler(this.tb_text_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "text :";
            // 
            // bt_find_next
            // 
            this.bt_find_next.Location = new System.Drawing.Point(609, 18);
            this.bt_find_next.Name = "bt_find_next";
            this.bt_find_next.Size = new System.Drawing.Size(110, 29);
            this.bt_find_next.TabIndex = 2;
            this.bt_find_next.Text = "Find &next";
            this.bt_find_next.UseVisualStyleBackColor = true;
            this.bt_find_next.Click += new System.EventHandler(this.bt_find_next_Click);
            // 
            // ScintillaFindForm
            // 
            this.AcceptButton = this.bt_find_next;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(871, 91);
            this.Controls.Add(this.bt_find_next);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_text);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScintillaFindForm";
            this.ShowInTaskbar = false;
            this.Text = "FindForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FindForm_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bt_find_next;
    }
}