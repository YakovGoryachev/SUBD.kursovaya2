namespace SUBD.kursovaya2
{
    partial class AdminPanel
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
            this.BackBut = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PhotoBut = new System.Windows.Forms.Button();
            this.DownloadPhotoBut = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BackBut
            // 
            this.BackBut.Location = new System.Drawing.Point(12, 399);
            this.BackBut.Name = "BackBut";
            this.BackBut.Size = new System.Drawing.Size(75, 39);
            this.BackBut.TabIndex = 0;
            this.BackBut.Text = "Назад";
            this.BackBut.UseVisualStyleBackColor = true;
            this.BackBut.Click += new System.EventHandler(this.BackBut_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(26, 54);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(212, 202);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Загружаемое фото";
            // 
            // PhotoBut
            // 
            this.PhotoBut.Location = new System.Drawing.Point(93, 399);
            this.PhotoBut.Name = "PhotoBut";
            this.PhotoBut.Size = new System.Drawing.Size(75, 39);
            this.PhotoBut.TabIndex = 3;
            this.PhotoBut.Text = "Выбор фото";
            this.PhotoBut.UseVisualStyleBackColor = true;
            this.PhotoBut.Click += new System.EventHandler(this.PhotoBut_Click);
            // 
            // DownloadPhotoBut
            // 
            this.DownloadPhotoBut.Location = new System.Drawing.Point(174, 399);
            this.DownloadPhotoBut.Name = "DownloadPhotoBut";
            this.DownloadPhotoBut.Size = new System.Drawing.Size(75, 39);
            this.DownloadPhotoBut.TabIndex = 4;
            this.DownloadPhotoBut.Text = "Загрузка фото в бд";
            this.DownloadPhotoBut.UseVisualStyleBackColor = true;
            this.DownloadPhotoBut.Click += new System.EventHandler(this.DownloadPhotoBut_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(244, 54);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(244, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Id фильма";
            // 
            // AdminPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.DownloadPhotoBut);
            this.Controls.Add(this.PhotoBut);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.BackBut);
            this.Name = "AdminPanel";
            this.Text = "AdminPanel";
            this.Load += new System.EventHandler(this.AdminPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BackBut;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button PhotoBut;
        private System.Windows.Forms.Button DownloadPhotoBut;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
    }
}