﻿namespace UI
{
    partial class UploadForm
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
            this.TestTBX = new System.Windows.Forms.TextBox();
            this.progressBarTotol = new System.Windows.Forms.ProgressBar();
            this.uploadProgressLab = new System.Windows.Forms.Label();
            this.progressLable = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TestTBX
            // 
            this.TestTBX.Location = new System.Drawing.Point(12, 159);
            this.TestTBX.Multiline = true;
            this.TestTBX.Name = "TestTBX";
            this.TestTBX.Size = new System.Drawing.Size(260, 29);
            this.TestTBX.TabIndex = 0;
            // 
            // progressBarTotol
            // 
            this.progressBarTotol.Location = new System.Drawing.Point(38, 48);
            this.progressBarTotol.Name = "progressBarTotol";
            this.progressBarTotol.Size = new System.Drawing.Size(475, 23);
            this.progressBarTotol.TabIndex = 1;
            // 
            // uploadProgressLab
            // 
            this.uploadProgressLab.AutoSize = true;
            this.uploadProgressLab.Location = new System.Drawing.Point(38, 88);
            this.uploadProgressLab.Name = "uploadProgressLab";
            this.uploadProgressLab.Size = new System.Drawing.Size(167, 12);
            this.uploadProgressLab.TabIndex = 2;
            this.uploadProgressLab.Text = "正在整理数据上传，请稍后...";
            // 
            // progressLable
            // 
            this.progressLable.AutoSize = true;
            this.progressLable.Location = new System.Drawing.Point(519, 59);
            this.progressLable.Name = "progressLable";
            this.progressLable.Size = new System.Drawing.Size(17, 12);
            this.progressLable.TabIndex = 3;
            this.progressLable.Text = "0%";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(136, 227);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // UploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(568, 138);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.progressLable);
            this.Controls.Add(this.uploadProgressLab);
            this.Controls.Add(this.progressBarTotol);
            this.Controls.Add(this.TestTBX);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UploadForm";
            this.Text = "有家SaaS上传器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TestTBX;
        private System.Windows.Forms.ProgressBar progressBarTotol;
        private System.Windows.Forms.Label uploadProgressLab;
        private System.Windows.Forms.Label progressLable;
        private System.Windows.Forms.Button button1;
    }
}