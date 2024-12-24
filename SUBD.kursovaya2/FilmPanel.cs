using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUBD.kursovaya2
{
    public partial class FilmPanel : Form
    {
        public FilmPanel()
        {
            InitializeComponent();
        }
        public string login;
        public string password;
        public string filmName;
        public ApplicationForm appform;
        NpgsqlConnection con;
        private void FilmPanel_Load(object sender, EventArgs e)
        {
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            login = appform.login;
            password = appform.password;
            con = new NpgsqlConnection(
                $"server=localhost; Port=1234; database=DataBaseGopar; userId={login}; password={password}");
            ShowFilm();
        }
        private void ShowFilm()
        {
            try
            {
                DownLoadPhoto();
                DownloadInformation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        private void DownLoadPhoto()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                
                int idFilm = 0;
                using (var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @name",con))
                {
                    cmd.Parameters.AddWithValue("@name", filmName);
                    idFilm = Convert.ToInt32(cmd.ExecuteScalar());
                }
                if (idFilm == 0 || idFilm < 0) { throw new Exception("Такого фильма нет"); }
                using (var command = new NpgsqlCommand(
                    "select photo from photo where id_film = @id_film",con))
                {
                    command.Parameters.AddWithValue("@id_film", idFilm);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            byte[] photoBytes = (byte[])reader["photo"];
                            using (var ms = new MemoryStream(photoBytes))
                            {
                                Image image = Image.FromStream(ms);
                                pictureBox1.Image = image;
                                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        private void DownloadInformation()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                
                label1.Text = filmName;

                // Получаем основную информацию о фильме
                using (var cmd = new NpgsqlCommand(
                    @"SELECT f.description, f.studio, f.date_release, f.source_reference,
                    COALESCE(
                        (SELECT ROUND(AVG(CAST(e.evaluation AS NUMERIC)), 1)
                        FROM evaluation e 
                        WHERE e.id_film = f.id), 0
                    ) as avg_rating,
                    (SELECT COUNT(*) FROM comment c WHERE c.id_film = f.id) as comments_count,
                    (SELECT COUNT(*) FROM evaluation e WHERE e.id_film = f.id) as ratings_count
                    FROM film f 
                    WHERE f.""name"" = @name", con))
                {
                    cmd.Parameters.AddWithValue("@name", filmName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            richTextBox1.Text = reader["description"].ToString();
                            label7.Text = reader["studio"].ToString();
                            label6.Text = reader["avg_rating"].ToString();
                            label8.Text = $"Комментариев: {reader["comments_count"]}";
                            label9.Text = $"Оценок: {reader["ratings_count"]}";
                        }
                    }
                }

                // Получаем информацию об участниках фильма
                using (var cmd = new NpgsqlCommand(
                    @"SELECT p.firstname, p.lastname, p.surname, r.""role""
                    FROM film f
                    JOIN participant_film pf ON f.id = pf.id_film
                    JOIN participant p ON pf.id_participant = p.id
                    JOIN roles r ON pf.id_role = r.id
                    WHERE f.""name"" = @name
                    ORDER BY r.""role""", con))
                {
                    cmd.Parameters.AddWithValue("@name", filmName);
                    StringBuilder participantsInfo = new StringBuilder();
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        string currentRole = "";
                        while (reader.Read())
                        {
                            string role = reader["role"].ToString();
                            if (role != currentRole)
                            {
                                if (currentRole != "") // Добавляем пустую строку между ролями
                                    participantsInfo.AppendLine();
                                participantsInfo.AppendLine($"{role}:");
                                currentRole = role;
                            }
                            
                            string surname = reader["surname"] == DBNull.Value ? "" : " " + reader["surname"].ToString();
                            participantsInfo.AppendLine($"- {reader["firstname"]} {reader["lastname"]}{surname}");
                        }
                    }
                    
                    // Получаем жанры фильма
                    using (var cmd2 = new NpgsqlCommand(
                        @"SELECT string_agg(g.genre, ', ') as genres
                        FROM film f
                        JOIN genre_in_film gf ON f.id = gf.id_film
                        JOIN genre g ON gf.id_genre = g.id
                        WHERE f.""name"" = @name
                        GROUP BY f.id", con))
                    {
                        cmd2.Parameters.AddWithValue("@name", filmName);
                        var genres = cmd2.ExecuteScalar()?.ToString();
                        if (!string.IsNullOrEmpty(genres))
                        {
                            participantsInfo.AppendLine();
                            participantsInfo.AppendLine($"Жанры: {genres}");
                        }
                    }
                    
                    richTextBox2.Text = participantsInfo.ToString().Trim();
                }

                // Получаем комментарии к фильму
                using (var cmd = new NpgsqlCommand(
                    @"SELECT u.login, c.comment, TO_CHAR(c.public_date, 'DD.MM.YYYY HH24:MI') as formatted_date
                    FROM film f
                    JOIN comment c ON f.id = c.id_film
                    JOIN ""user"" u ON c.id_user = u.id
                    WHERE f.name = @name 
					ORDER BY c.public_date DESC", con))
                {
                    cmd.Parameters.AddWithValue("@name", filmName);
                    StringBuilder comments = new StringBuilder();
                    
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.AppendLine($"{reader["login"]} ({reader["formatted_date"]}):");
                            comments.AppendLine(reader["comment"].ToString());
                            comments.AppendLine();
                        }
                    }
                    richTextBox3.Text = comments.ToString().TrimEnd();
                }
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
        private void AddPlaylistBut_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем форму для ввода названия плейлиста
                Form inputForm = new Form()
                {
                    Width = 400,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = "Добавление в плейлист",
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    TopMost = true,
                    Owner = this
                };

                TextBox textBox = new TextBox()
                {
                    Left = 50,
                    Top = 20,
                    Width = 280
                };

                Button confirmButton = new Button()
                {
                    Text = "ОК",
                    Left = 50,
                    Width = 100,
                    Top = 60,
                    DialogResult = DialogResult.OK
                };

                Button cancelButton = new Button()
                {
                    Text = "Отмена",
                    Left = 230,
                    Width = 100,
                    Top = 60,
                    DialogResult = DialogResult.Cancel
                };

                Label label = new Label()
                {
                    Left = 50,
                    Top = 5,
                    Text = "Введите название плейлиста:",
                    Width = 280
                };

                inputForm.Controls.AddRange(new Control[] { textBox, confirmButton, cancelButton, label });
                inputForm.AcceptButton = confirmButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(textBox.Text))
                    return;

                string playlistName = textBox.Text.Trim();

                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        int userId;
                        // Получаем ID пользователя
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM \"user\" WHERE login = @login", con))
                        {
                            cmd.Parameters.AddWithValue("@login", login);
                            userId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        int filmId;
                        // Получаем ID фильма
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM film WHERE \"name\" = @name", con))
                        {
                            cmd.Parameters.AddWithValue("@name", filmName);
                            filmId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        int playlistId;
                        // Проверяем существование плейлиста
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM playlist WHERE id_user = @userId AND name = @name", con))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@name", playlistName);
                            var result = cmd.ExecuteScalar();

                            if (result == null || result == DBNull.Value)
                            {
                                // Создаем новый плейлист
                                using (var createCmd = new NpgsqlCommand(
                                    "INSERT INTO playlist (id_user, name) VALUES (@userId, @name) RETURNING id", con))
                                {
                                    createCmd.Parameters.AddWithValue("@userId", userId);
                                    createCmd.Parameters.AddWithValue("@name", playlistName);
                                    playlistId = Convert.ToInt32(createCmd.ExecuteScalar());
                                }
                            }
                            else
                            {
                                playlistId = Convert.ToInt32(result);
                                
                                // Проверяем, не добавлен ли уже фильм в этот плейлист
                                using (var checkCmd = new NpgsqlCommand(
                                    "SELECT COUNT(*) FROM playlist_film WHERE id_playlist = @playlistId AND id_film = @filmId", con))
                                {
                                    checkCmd.Parameters.AddWithValue("@playlistId", playlistId);
                                    checkCmd.Parameters.AddWithValue("@filmId", filmId);
                                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                                    if (count > 0)
                                    {
                                        throw new Exception("Этот фильм уже есть в указанном плейлисте");
                                    }
                                }
                            }
                        }

                        // Добавляем фильм в плейлист
                        using (var cmd = new NpgsqlCommand(
                            "INSERT INTO playlist_film (id_playlist, id_film) VALUES (@playlistId, @filmId)", con))
                        {
                            cmd.Parameters.AddWithValue("@playlistId", playlistId);
                            cmd.Parameters.AddWithValue("@filmId", filmId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Фильм успешно добавлен в плейлист");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Ошибка при добавлении фильма в плейлист: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void BackBut_Click(object sender, EventArgs e)
        {
            
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            
            if (con != null && con.State == ConnectionState.Open)
                con.Close();

            
            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox3.Clear();
            richTextBox4.Clear();

            
            this.Close();

            
            appform.filmPanel = null;
            appform.Show();
        }

        private void AddCommentBut_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(richTextBox4.Text))
                {
                    MessageBox.Show("Введите текст комментария");
                    return;
                }

                if (richTextBox4.Text.Length > 1000)
                {
                    MessageBox.Show("Комментарий не должен превышать 1000 символов");
                    return;
                }

                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        int userId, filmId;

                        
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM \"user\" WHERE login = @login", con))
                        {
                            cmd.Parameters.AddWithValue("@login", login);
                            var result = cmd.ExecuteScalar();
                            if (result == null)
                                throw new Exception("Пользователь не найден");
                            userId = Convert.ToInt32(result);
                        }

                        
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM film WHERE name = @name", con))
                        {
                            cmd.Parameters.AddWithValue("@name", filmName);
                            var result = cmd.ExecuteScalar();
                            if (result == null)
                                throw new Exception("Фильм не найден");
                            filmId = Convert.ToInt32(result);
                        }

                        
                        using (var cmd = new NpgsqlCommand(
                            @"INSERT INTO comment (id_user, id_film, comment, public_date) 
                            VALUES (@userId, @filmId, @comment, CURRENT_TIMESTAMP)", con))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@filmId", filmId);
                            cmd.Parameters.AddWithValue("@comment", richTextBox4.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        richTextBox4.Clear();
                        
                        
                        DownloadInformation();
                        MessageBox.Show("Комментарий успешно добавлен");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Ошибка при добавлении комментария: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void EvalBut_Click(object sender, EventArgs e)
        {
            try
            {
                // Создаем форму для ввода оценки
                Form inputForm = new Form()
                {
                    Width = 300,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = "Оценка фильма",
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    TopMost = true,
                    Owner = this
                };

                NumericUpDown numericUpDown = new NumericUpDown()
                {
                    Left = 50,
                    Top = 20,
                    Width = 180,
                    Minimum = 0,
                    Maximum = 10,
                    DecimalPlaces = 0,
                    Value = 5,
                    Increment = 1
                };

                Button confirmButton = new Button()
                {
                    Text = "ОК",
                    Left = 50,
                    Width = 80,
                    Top = 60,
                    DialogResult = DialogResult.OK
                };

                Button cancelButton = new Button()
                {
                    Text = "Отмена",
                    Left = 150,
                    Width = 80,
                    Top = 60,
                    DialogResult = DialogResult.Cancel
                };

                Label label = new Label()
                {
                    Left = 50,
                    Top = 5,
                    Text = "Введите оценку от 0 до 10:",
                    Width = 200
                };

                inputForm.Controls.AddRange(new Control[] { numericUpDown, confirmButton, cancelButton, label });
                inputForm.AcceptButton = confirmButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog() != DialogResult.OK)
                    return;

                decimal rating = numericUpDown.Value;

                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        int userId, filmId;

                        // Получаем ID пользователя
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM \"user\" WHERE login = @login", con))
                        {
                            cmd.Parameters.AddWithValue("@login", login);
                            var result = cmd.ExecuteScalar();
                            if (result == null)
                                throw new Exception("Пользователь не найден");
                            userId = Convert.ToInt32(result);
                        }

                        // Получаем ID фильма
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM film WHERE name = @name", con))
                        {
                            cmd.Parameters.AddWithValue("@name", filmName);
                            var result = cmd.ExecuteScalar();
                            if (result == null)
                                throw new Exception("Фильм не найден");
                            filmId = Convert.ToInt32(result);
                        }

                        // Проверяем, не оценивал ли уже пользователь этот фильм
                        using (var cmd = new NpgsqlCommand(
                            "SELECT id FROM evaluation WHERE id_user = @userId AND id_film = @filmId", con))
                        {
                            cmd.Parameters.AddWithValue("@userId", userId);
                            cmd.Parameters.AddWithValue("@filmId", filmId);
                            var existingEval = cmd.ExecuteScalar();

                            if (existingEval != null)
                            {
                                // Обновляем существующую оценку
                                using (var updateCmd = new NpgsqlCommand(
                                    "UPDATE evaluation SET evaluation = @rating WHERE id_user = @userId AND id_film = @filmId", con))
                                {
                                    updateCmd.Parameters.AddWithValue("@rating", rating);
                                    updateCmd.Parameters.AddWithValue("@userId", userId);
                                    updateCmd.Parameters.AddWithValue("@filmId", filmId);
                                    updateCmd.ExecuteNonQuery();
                                }
                                using (var updateDate = new NpgsqlCommand(
                                    "update evaluation set date_evaluation = current_date where id_user = @userId and id_film = @filmId",con))
                                {
                                    updateDate.Parameters.AddWithValue("@userId", userId);
                                    updateDate.Parameters.AddWithValue("@filmId", filmId);
                                    updateDate.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Добавляем новую оценку
                                using (var insertCmd = new NpgsqlCommand(
                                    "INSERT INTO evaluation (id_user, id_film, evaluation, date_evaluation) VALUES (@userId, @filmId, @rating,current_date)", con))
                                {
                                    insertCmd.Parameters.AddWithValue("@userId", userId);
                                    insertCmd.Parameters.AddWithValue("@filmId", filmId);
                                    insertCmd.Parameters.AddWithValue("@rating", rating);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Оценка успешно сохранена");
                        
                        // Обновляем информацию на форме
                        DownloadInformation();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Ошибка при сохранении оценки: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }
    }
}