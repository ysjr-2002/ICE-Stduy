namespace FaceDetectAndCompare
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnGrouping = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDetectAndCompare = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtThreadcount = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMaxcount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnGrouping
            // 
            this.btnGrouping.Location = new System.Drawing.Point(228, 12);
            this.btnGrouping.Name = "btnGrouping";
            this.btnGrouping.Size = new System.Drawing.Size(75, 23);
            this.btnGrouping.TabIndex = 0;
            this.btnGrouping.Text = "文件分组";
            this.btnGrouping.UseVisualStyleBackColor = true;
            this.btnGrouping.Click += new System.EventHandler(this.btnGrouping_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = " 组号";
            // 
            // btnDetectAndCompare
            // 
            this.btnDetectAndCompare.Location = new System.Drawing.Point(228, 130);
            this.btnDetectAndCompare.Name = "btnDetectAndCompare";
            this.btnDetectAndCompare.Size = new System.Drawing.Size(75, 23);
            this.btnDetectAndCompare.TabIndex = 2;
            this.btnDetectAndCompare.Text = "检测比对";
            this.btnDetectAndCompare.UseVisualStyleBackColor = true;
            this.btnDetectAndCompare.Click += new System.EventHandler(this.btnDetectAndComapare_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(91, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "工作线程：";
            // 
            // txtThreadcount
            // 
            this.txtThreadcount.Location = new System.Drawing.Point(159, 14);
            this.txtThreadcount.Name = "txtThreadcount";
            this.txtThreadcount.Size = new System.Drawing.Size(63, 21);
            this.txtThreadcount.TabIndex = 4;
            this.txtThreadcount.Text = "2";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(228, 166);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "单张";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(141, 136);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "未开始工作";
            // 
            // txtMaxcount
            // 
            this.txtMaxcount.Location = new System.Drawing.Point(159, 98);
            this.txtMaxcount.Name = "txtMaxcount";
            this.txtMaxcount.Size = new System.Drawing.Size(63, 21);
            this.txtMaxcount.TabIndex = 8;
            this.txtMaxcount.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "返回最大图像数量：";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(228, 95);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "设置小图路径";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 222);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtMaxcount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtThreadcount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnDetectAndCompare);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGrouping);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "人像提取、比对测试";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGrouping;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDetectAndCompare;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtThreadcount;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMaxcount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button2;
    }
}

