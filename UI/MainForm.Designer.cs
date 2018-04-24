namespace UI
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.dirTbx = new System.Windows.Forms.TextBox();
            this.startBtn = new System.Windows.Forms.Button();
            this.scanBtn = new System.Windows.Forms.Button();
            this.widthTbx = new System.Windows.Forms.TextBox();
            this.heightTbx = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.compressBtn = new System.Windows.Forms.Button();
            this.uploadBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TestTBX
            // 
            this.TestTBX.Location = new System.Drawing.Point(78, 445);
            this.TestTBX.Multiline = true;
            this.TestTBX.Name = "TestTBX";
            this.TestTBX.Size = new System.Drawing.Size(260, 21);
            this.TestTBX.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "目录路径:";
            // 
            // dirTbx
            // 
            this.dirTbx.Location = new System.Drawing.Point(99, 58);
            this.dirTbx.Name = "dirTbx";
            this.dirTbx.Size = new System.Drawing.Size(278, 21);
            this.dirTbx.TabIndex = 2;
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(99, 164);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(75, 23);
            this.startBtn.TabIndex = 3;
            this.startBtn.Text = "提取数据";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // scanBtn
            // 
            this.scanBtn.Location = new System.Drawing.Point(399, 58);
            this.scanBtn.Name = "scanBtn";
            this.scanBtn.Size = new System.Drawing.Size(75, 23);
            this.scanBtn.TabIndex = 4;
            this.scanBtn.Text = "浏览";
            this.scanBtn.UseVisualStyleBackColor = true;
            this.scanBtn.Click += new System.EventHandler(this.scanBtn_Click);
            // 
            // widthTbx
            // 
            this.widthTbx.Location = new System.Drawing.Point(99, 112);
            this.widthTbx.Name = "widthTbx";
            this.widthTbx.Size = new System.Drawing.Size(100, 21);
            this.widthTbx.TabIndex = 5;
            this.widthTbx.Text = "500";
            // 
            // heightTbx
            // 
            this.heightTbx.Location = new System.Drawing.Point(277, 112);
            this.heightTbx.Name = "heightTbx";
            this.heightTbx.Size = new System.Drawing.Size(100, 21);
            this.heightTbx.TabIndex = 6;
            this.heightTbx.Text = "500";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "宽:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(248, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "高:";
            // 
            // compressBtn
            // 
            this.compressBtn.Location = new System.Drawing.Point(205, 164);
            this.compressBtn.Name = "compressBtn";
            this.compressBtn.Size = new System.Drawing.Size(75, 23);
            this.compressBtn.TabIndex = 9;
            this.compressBtn.Text = "压缩文件";
            this.compressBtn.UseVisualStyleBackColor = true;
            this.compressBtn.Click += new System.EventHandler(this.compressBtn_Click);
            // 
            // uploadBtn
            // 
            this.uploadBtn.Location = new System.Drawing.Point(302, 164);
            this.uploadBtn.Name = "uploadBtn";
            this.uploadBtn.Size = new System.Drawing.Size(75, 23);
            this.uploadBtn.TabIndex = 10;
            this.uploadBtn.Text = "上传文件";
            this.uploadBtn.UseVisualStyleBackColor = true;
            this.uploadBtn.Click += new System.EventHandler(this.uploadBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 221);
            this.Controls.Add(this.uploadBtn);
            this.Controls.Add(this.compressBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.heightTbx);
            this.Controls.Add(this.widthTbx);
            this.Controls.Add(this.scanBtn);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.dirTbx);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TestTBX);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "有家SaaS上传器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TestTBX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox dirTbx;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Button scanBtn;
        private System.Windows.Forms.TextBox widthTbx;
        private System.Windows.Forms.TextBox heightTbx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button compressBtn;
        private System.Windows.Forms.Button uploadBtn;
    }
}