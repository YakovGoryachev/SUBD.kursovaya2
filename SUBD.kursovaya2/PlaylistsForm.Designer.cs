namespace SUBD.kursovaya2
{
    partial class PlaylistsForm
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
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.playlistsDataGridView = new System.Windows.Forms.DataGridView();
            this.filmsDataGridView = new System.Windows.Forms.DataGridView();
            this.DeletePlaylistButton = new System.Windows.Forms.Button();
            this.DeleteFilmButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.playlistsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filmsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // playlistsDataGridView
            // 
            this.playlistsDataGridView.Location = new System.Drawing.Point(12, 40);
            this.playlistsDataGridView.Name = "playlistsDataGridView";
            this.playlistsDataGridView.Size = new System.Drawing.Size(1860, 300);
            this.playlistsDataGridView.TabIndex = 1;
            this.playlistsDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.playlistsDataGridView_CellDoubleClick);
            // 
            // filmsDataGridView
            // 
            this.filmsDataGridView.Location = new System.Drawing.Point(12, 430);
            this.filmsDataGridView.Name = "filmsDataGridView";
            this.filmsDataGridView.Size = new System.Drawing.Size(1860, 500);
            this.filmsDataGridView.TabIndex = 4;
            this.filmsDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.filmsDataGridView_CellDoubleClick);
            // 
            // DeletePlaylistButton
            // 
            this.DeletePlaylistButton.Location = new System.Drawing.Point(12, 350);
            this.DeletePlaylistButton.Name = "DeletePlaylistButton";
            this.DeletePlaylistButton.Size = new System.Drawing.Size(150, 30);
            this.DeletePlaylistButton.TabIndex = 2;
            this.DeletePlaylistButton.Text = "Удалить плейлист";
            this.DeletePlaylistButton.Click += new System.EventHandler(this.DeletePlaylistButton_Click);
            // 
            // DeleteFilmButton
            // 
            this.DeleteFilmButton.Location = new System.Drawing.Point(12, 940);
            this.DeleteFilmButton.Name = "DeleteFilmButton";
            this.DeleteFilmButton.Size = new System.Drawing.Size(150, 30);
            this.DeleteFilmButton.TabIndex = 5;
            this.DeleteFilmButton.Text = "Удалить фильм";
            this.DeleteFilmButton.Click += new System.EventHandler(this.DeleteFilmButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(180, 940);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(150, 30);
            this.BackButton.TabIndex = 6;
            this.BackButton.Text = "Назад";
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Плейлисты";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.label2.Location = new System.Drawing.Point(12, 400);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 24);
            this.label2.TabIndex = 3;
            this.label2.Text = "Фильмы в плейлисте";
            // 
            // PlaylistsForm
            // 
            this.ClientSize = new System.Drawing.Size(1904, 1041);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.playlistsDataGridView);
            this.Controls.Add(this.DeletePlaylistButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.filmsDataGridView);
            this.Controls.Add(this.DeleteFilmButton);
            this.Controls.Add(this.BackButton);
            this.Name = "PlaylistsForm";
            this.Text = "Мои плейлисты";
            this.Load += new System.EventHandler(this.PlaylistsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.playlistsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filmsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.DataGridView playlistsDataGridView;
        private System.Windows.Forms.DataGridView filmsDataGridView;
        private System.Windows.Forms.Button DeletePlaylistButton;
        private System.Windows.Forms.Button DeleteFilmButton;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        #region Windows Form Designer generated code

        #endregion
    }
}