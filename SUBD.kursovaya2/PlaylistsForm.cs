using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUBD.kursovaya2
{
    public partial class PlaylistsForm : Form
    {
        public ApplicationForm appform;
        private FilmPanel filmPanel;
        private NpgsqlConnection con;
        private string login;
        private string password;
        private int userId;
        public PlaylistsForm()
        {
            InitializeComponent();
        }
        private void PlaylistsForm_Load(object sender, EventArgs e)
        {
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            login = appform.login;
            password = appform.password;
            con = new NpgsqlConnection(
                $"server=localhost; Port=1234; database=DataBaseGopar; userId={login}; password={password}");

            try
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT id FROM \"user\" WHERE login = @login", con))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    userId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            finally
            {
                con.Close();
            }

            LoadPlaylists();
        }

        private void LoadPlaylists()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                
                using (var cmd = new NpgsqlCommand(
                    @"SELECT p.name as ""Название"",
                  COUNT(pf.id_film) as ""Количество фильмов""
                FROM playlist p
                LEFT JOIN playlist_film pf ON p.id = pf.id_playlist
                WHERE p.id_user = @userId
                GROUP BY p.id, p.name
                ORDER BY p.name", con))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    playlistsDataGridView.DataSource = dt;

                    playlistsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    playlistsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    playlistsDataGridView.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке плейлистов: {ex.Message}");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void DeletePlaylistButton_Click(object sender, EventArgs e)
        {
            if (playlistsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите плейлист для удаления");
                return;
            }

            string playlistName = playlistsDataGridView.SelectedRows[0].Cells["Название"].Value.ToString();

            var result = MessageBox.Show(
                $"Вы действительно хотите удалить плейлист \"{playlistName}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            // Получаем ID плейлиста
                            int playlistId;
                            using (var cmd = new NpgsqlCommand(
                                "SELECT id FROM playlist WHERE name = @name AND id_user = @userId", con))
                            {
                                cmd.Parameters.AddWithValue("@name", playlistName);
                                cmd.Parameters.AddWithValue("@userId", userId);
                                playlistId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Удаляем связи с фильмами
                            using (var cmd = new NpgsqlCommand(
                                "DELETE FROM playlist_film WHERE id_playlist = @playlistId", con))
                            {
                                cmd.Parameters.AddWithValue("@playlistId", playlistId);
                                cmd.ExecuteNonQuery();
                            }

                            // Удаляем сам плейлист
                            using (var cmd = new NpgsqlCommand(
                                "DELETE FROM playlist WHERE id = @playlistId", con))
                            {
                                cmd.Parameters.AddWithValue("@playlistId", playlistId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            LoadPlaylists();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении плейлиста: {ex.Message}");
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

        private void playlistsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string playlistName = playlistsDataGridView.Rows[e.RowIndex].Cells["Название"].Value.ToString();
                ShowPlaylistFilms(playlistName);
            }
        }
        private void filmsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string filmName = filmsDataGridView.Rows[e.RowIndex].Cells["Название"].Value.ToString();
                ShowFilmPanel(filmName);
            }
        }
        private void ShowFilmPanel(string filmName)
        {
            if (filmPanel == null)
            {
                filmPanel = new FilmPanel();
                filmPanel.filmName = filmName;
                filmPanel.appform = appform;
                filmPanel.Show();
                Close();
            }
        }

        private void ShowPlaylistFilms(string playlistName)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                
                using (var cmd = new NpgsqlCommand(
                    @"SELECT f.name as ""Название"", 
                  f.studio as ""Студия"",
                  f.date_release as ""Год выпуска"",
                  p.name as ""Плейлист""
                FROM film f
                JOIN playlist_film pf ON f.id = pf.id_film
                JOIN playlist p ON pf.id_playlist = p.id
                WHERE p.name = @playlistName AND p.id_user = @userId
                ORDER BY f.name", con))
                {
                    cmd.Parameters.AddWithValue("@playlistName", playlistName);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    filmsDataGridView.DataSource = dt;

                    filmsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    filmsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    filmsDataGridView.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильмов: {ex.Message}");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void DeleteFilmButton_Click(object sender, EventArgs e)
        {
            if (filmsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите фильм для удаления из плейлиста");
                return;
            }

            string filmName = filmsDataGridView.SelectedRows[0].Cells["Название"].Value.ToString();
            string playlistName = filmsDataGridView.SelectedRows[0].Cells["Плейлист"].Value.ToString();

            var result = MessageBox.Show(
                $"Удалить фильм \"{filmName}\" из плейлиста \"{playlistName}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    con.Open();
                    using (var transaction = con.BeginTransaction())
                    {
                        try
                        {
                            using (var cmd = new NpgsqlCommand(
                                @"DELETE FROM playlist_film 
                            WHERE id_playlist = (SELECT id FROM playlist WHERE name = @playlistName AND id_user = @userId)
                            AND id_film = (SELECT id FROM film WHERE name = @filmName)", con))
                            {
                                cmd.Parameters.AddWithValue("@playlistName", playlistName);
                                cmd.Parameters.AddWithValue("@userId", userId);
                                cmd.Parameters.AddWithValue("@filmName", filmName);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            ShowPlaylistFilms(playlistName);
                            LoadPlaylists(); // Обновляем количество фильмов
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении фильма из плейлиста: {ex.Message}");
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                        con.Close();
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
            appform.Show();
        }
    }
}
