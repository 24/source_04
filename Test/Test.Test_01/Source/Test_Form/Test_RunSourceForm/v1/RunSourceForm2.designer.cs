namespace Test.Test_Form.Test_RunSourceForm.v1
{
    partial class RunSourceForm2
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunSourceForm));
            this.pan_top = new System.Windows.Forms.Panel();
            this.bt_execute = new System.Windows.Forms.Button();
            this.pan_button = new System.Windows.Forms.Panel();
            this.bt_pause = new System.Windows.Forms.Button();
            this.lb_progress_label = new System.Windows.Forms.Label();
            this.pb_progress_bar = new System.Windows.Forms.ProgressBar();
            this.split_top = new System.Windows.Forms.Splitter();
            this.tc_result = new System.Windows.Forms.TabControl();
            this.tab_message = new System.Windows.Forms.TabPage();
            this.pan_top.SuspendLayout();
            this.pan_button.SuspendLayout();
            this.tc_result.SuspendLayout();
            this.tab_message.SuspendLayout();
            this.SuspendLayout();
            // 
            // pan_top
            // 
            this.pan_top.Controls.Add(this.bt_execute);
            this.pan_top.Controls.Add(this.pan_button);
            this.pan_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_top.Location = new System.Drawing.Point(0, 24);
            this.pan_top.Name = "pan_top";
            this.pan_top.Size = new System.Drawing.Size(1056, 197);
            this.pan_top.TabIndex = 0;
            // 
            // bt_execute
            // 
            this.bt_execute.Location = new System.Drawing.Point(3, 3);
            this.bt_execute.Name = "bt_execute";
            this.bt_execute.Size = new System.Drawing.Size(75, 23);
            this.bt_execute.TabIndex = 1;
            this.bt_execute.Text = "&Run";
            this.bt_execute.UseVisualStyleBackColor = true;
            // 
            // pan_button
            // 
            this.pan_button.Controls.Add(this.bt_pause);
            this.pan_button.Controls.Add(this.lb_progress_label);
            this.pan_button.Controls.Add(this.pb_progress_bar);
            this.pan_button.Dock = System.Windows.Forms.DockStyle.Top;
            this.pan_button.Location = new System.Drawing.Point(0, 0);
            this.pan_button.Name = "pan_button";
            this.pan_button.Size = new System.Drawing.Size(1056, 35);
            this.pan_button.TabIndex = 2;
            // 
            // bt_pause
            // 
            this.bt_pause.Location = new System.Drawing.Point(84, 3);
            this.bt_pause.Name = "bt_pause";
            this.bt_pause.Size = new System.Drawing.Size(75, 23);
            this.bt_pause.TabIndex = 5;
            this.bt_pause.Text = "&Pause";
            this.bt_pause.UseVisualStyleBackColor = true;
            // 
            // lb_progress_label
            // 
            this.lb_progress_label.AutoSize = true;
            this.lb_progress_label.Location = new System.Drawing.Point(624, 1);
            this.lb_progress_label.Name = "lb_progress_label";
            this.lb_progress_label.Size = new System.Drawing.Size(47, 13);
            this.lb_progress_label.TabIndex = 1;
            this.lb_progress_label.Text = "progress";
            this.lb_progress_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pb_progress_bar
            // 
            this.pb_progress_bar.Location = new System.Drawing.Point(622, 16);
            this.pb_progress_bar.Name = "pb_progress_bar";
            this.pb_progress_bar.Size = new System.Drawing.Size(422, 16);
            this.pb_progress_bar.TabIndex = 0;
            // 
            // split_top
            // 
            this.split_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.split_top.Location = new System.Drawing.Point(0, 221);
            this.split_top.Name = "split_top";
            this.split_top.Size = new System.Drawing.Size(1056, 3);
            this.split_top.TabIndex = 1;
            this.split_top.TabStop = false;
            // 
            // tc_result
            // 
            this.tc_result.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tc_result.Controls.Add(this.tab_message);
            this.tc_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tc_result.Location = new System.Drawing.Point(0, 224);
            this.tc_result.Name = "tc_result";
            this.tc_result.SelectedIndex = 0;
            this.tc_result.Size = new System.Drawing.Size(1056, 390);
            this.tc_result.TabIndex = 2;
            // 
            // tab_message
            // 
            //this.tab_message.Controls.Add(this.tb_message);
            this.tab_message.Location = new System.Drawing.Point(4, 4);
            this.tab_message.Name = "tab_message";
            this.tab_message.Padding = new System.Windows.Forms.Padding(3);
            this.tab_message.Size = new System.Drawing.Size(1048, 364);
            this.tab_message.TabIndex = 1;
            this.tab_message.Text = "Messages";
            this.tab_message.UseVisualStyleBackColor = true;
            // 
            // fWRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 614);
            this.Controls.Add(this.tc_result);
            this.Controls.Add(this.split_top);
            this.Controls.Add(this.pan_top);
            this.KeyPreview = true;
            this.Name = "fWRun";
            this.Text = "Run source 2";
            this.pan_top.ResumeLayout(false);
            this.pan_button.ResumeLayout(false);
            this.pan_button.PerformLayout();
            this.tc_result.ResumeLayout(false);
            this.tab_message.ResumeLayout(false);
            this.tab_message.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pan_top;
        private System.Windows.Forms.Button bt_execute;
        private System.Windows.Forms.Panel pan_button;
        private System.Windows.Forms.Splitter split_top;
        private System.Windows.Forms.TabControl tc_result;
        private System.Windows.Forms.TabPage tab_message;
        private System.Windows.Forms.ProgressBar pb_progress_bar;
        private System.Windows.Forms.Label lb_progress_label;
        private System.Windows.Forms.Button bt_pause;
    }
}

