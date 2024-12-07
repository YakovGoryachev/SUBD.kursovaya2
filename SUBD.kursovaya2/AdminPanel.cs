using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUBD.kursovaya2
{
    public partial class AdminPanel : Form
    {
        public AdminPanel()
        {
            InitializeComponent();
        }
        public ApplicationForm AppForm; //указатель на основную форму
        NpgsqlConnection con;

        private void AdminPanel_Load(object sender, EventArgs e)
        {
            con = new NpgsqlConnection(
                "server=localhost; Port=1234; database=DataBaseGopar; userId=postgres; password=admin");
        }

        private void BackBut_Click(object sender, EventArgs e)
        {
            Hide();
            AppForm.Show();
        }
        byte[] imageBytes = null;
        private void PhotoBut_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "(*.jpg; *.jpeg; *.png) | *.jpg; *.jpeg; *.png";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imageBytes = File.ReadAllBytes(dialog.FileName);
                }
                Bitmap image;
                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    image = new Bitmap(stream);
                }
                pictureBox1.Image = image;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch (Exception ex)
            {

            }
        }

        private void DownloadPhotoBut_Click(object sender, EventArgs e)
        {
            try
            {
                DownloadPhotoInBD();
                MessageBox.Show("Вставка фото прошла успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DownGenreBut_Click(object sender, EventArgs e)
        {
            try
            {
                DownloadGenreInBD();
                MessageBox.Show("Вставка жанра прошла успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DownParticipantBut_Click(object sender, EventArgs e)
        {
            try
            {
                DownloadParticipantInBD();
                MessageBox.Show("Вставка участника прошла успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DownloadPhotoInBD()
        {
            if (textBox1.Text == "") { MessageBox.Show("Выберете название фильма"); return; }
            if (pictureBox1.Image == null) { MessageBox.Show("Выберете фото"); return; }
            con.Open();
            int idFilm;

            using (var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @namefilm", con))
            {
                cmd.Parameters.AddWithValue(@"namefilm", textBox1.Text.Trim());
                idFilm = Convert.ToInt16(cmd.ExecuteScalar());
                //textBox1.Text = idFilm.ToString();
            }

            using (var cmd = new NpgsqlCommand("insert into photo (id_film, photo)" +
                "values (@id_film, @photo)", con))
            {
                cmd.Parameters.AddWithValue("@id_film", idFilm);
                cmd.Parameters.AddWithValue("@photo", imageBytes);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        private void DownloadGenreInBD()
        {
            if (textBox1.Text == "") { MessageBox.Show("Введите название фильма");return; }
            if (textBox5.Text == "") { MessageBox.Show("Введите название жанра"); return; }
            int idFilm = 0;
            int idGenre = 0;
            int idGenreInFilm = 0;
            CheckTextbox();
            con.Open();
            using (var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @name",con))
            {
                cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                idFilm = Convert.ToInt16(cmd.ExecuteScalar());
            }
            if (idFilm == 0)
            { con.Close(); throw new Exception("Такого фильма нет"); }
            using (var cmd = new NpgsqlCommand("select \"id\" from genre where genre = @genre",con))
            {
                cmd.Parameters.AddWithValue("@genre",textBox5.Text.Trim());
                idGenre = Convert.ToInt16(cmd.ExecuteScalar());
            }
            if (idGenre == 0)
            { con.Close(); throw new Exception("Такого жанра нет"); }
            using (var cmd = new NpgsqlCommand("select \"id\" from genre_in_film where id_film = @id_film " +
                "and id_genre = @id_genre",con))
            {
                cmd.Parameters.AddWithValue("@id_film", idFilm);
                cmd.Parameters.AddWithValue("@id_genre", idGenre);
                idGenreInFilm = Convert.ToInt16(cmd.ExecuteScalar());
            }
            if (idGenreInFilm != 0)
            { con.Close(); throw new Exception("Такой жанр уже установлен для этого фильма"); }
            using (var cmd = new NpgsqlCommand("insert into genre_in_film (id_film, id_genre) " +
                "values (@id_film, @id_genre)", con))
            {
                cmd.Parameters.AddWithValue("@id_film", idFilm);
                cmd.Parameters.AddWithValue("@id_genre", idGenre);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }
        private void DownloadParticipantInBD()
        {
            if (textBox1.Text == "") { MessageBox.Show("Введите название фильма"); return; }
            if (textBox6.Text == "") { MessageBox.Show("Введите имя участника"); return; }
            if (textBox7.Text == "") { MessageBox.Show("Введите фамилия участника"); return; }
            //if (textBox8.Text == "") { MessageBox.Show("Введите отчество участника"); return; }
            if (textBox9.Text == "") { MessageBox.Show("Введите роль участника"); return; }
            CheckTextbox();
            con.Open();
            using (var transaction =  con.BeginTransaction())
            {
                int idRole = 0;
                int idFilm = 0;
                int idParticipant = 0;
                var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @name", con);
                
                cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                idFilm = Convert.ToInt16(cmd.ExecuteScalar());

                if (idFilm == 0)
                {
                    con.Close();
                    throw new Exception("Фильм не найден");
                }

                cmd = new NpgsqlCommand("select \"id\" from participant " +
                    "where firstname = @firstname and lastname = @lastname and surname = @surname", con);

                cmd.Parameters.AddWithValue("@firstname", textBox6.Text.Trim());
                cmd.Parameters.AddWithValue("@lastname", textBox7.Text.Trim());
                cmd.Parameters.AddWithValue("@surname", textBox8.Text.Trim());
                idParticipant = Convert.ToInt16(cmd.ExecuteScalar());

                if (idParticipant == 0)
                {

                    cmd = new NpgsqlCommand("insert into participant (firstname, lastname, surname) " +
                    "values (@firstname, @lastname, @surname)", con);

                    cmd.Parameters.AddWithValue("@firstname", textBox6.Text.Trim());
                    cmd.Parameters.AddWithValue("@lastname", textBox7.Text.Trim());
                    cmd.Parameters.AddWithValue("@surname", textBox8.Text.Trim());
                    cmd.ExecuteNonQuery();

                    cmd = new NpgsqlCommand("select \"id\" from participant " +
                    "where firstname = @firstname and lastname = @lastname and surname = @surname", con);

                    cmd.Parameters.AddWithValue("@firstname", textBox6.Text.Trim());
                    cmd.Parameters.AddWithValue("@lastname", textBox7.Text.Trim());
                    cmd.Parameters.AddWithValue("@surname", textBox8.Text.Trim());
                    idParticipant = Convert.ToInt16(cmd.ExecuteScalar());
                }

                cmd = new NpgsqlCommand("select \"id\" from roles where \"role\" = @role", con);
                
                cmd.Parameters.AddWithValue("@role", textBox9.Text.Trim());
                idRole = Convert.ToInt16(cmd.ExecuteScalar());

                if (idRole != 0)
                {
                    cmd = new NpgsqlCommand("insert into participant_film (id_film, id_participant, id_role) " +
                        "values (@id_film, @id_participant, @id_role)", con);
                    
                    cmd.Parameters.AddWithValue("@id_film", idFilm);
                    cmd.Parameters.AddWithValue("@id_participant", idParticipant);
                    cmd.Parameters.AddWithValue("@id_role", idRole);
                    cmd.ExecuteNonQuery();
                }
                //else
                //{
                //    using (var cmd = new NpgsqlCommand("insert into roles (\"role\") " +
                //        "values (@role)",con))
                //    {
                //        cmd.Parameters.AddWithValue("@role", textBox9.Text.Trim());
                //        cmd.ExecuteNonQuery();
                //    }
                //}
                transaction.Commit();
            }
            con.Close();
        }
        private void DownFilmBut_Click(object sender, EventArgs e)
        {
            try
            {
                int id = 0;
                CheckTextbox();
                if (textBox1.Text == "") { MessageBox.Show("Введите название фильма"); return; }
                if (richTextBox1.Text == "") { MessageBox.Show("Введите описание фильма"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Введите название студии"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Введите год выпуска фильма"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Введите название жанра");return; }
                if (textBox6.Text == "") { MessageBox.Show("Введите имя участника");return; }
                if (textBox7.Text == "") { MessageBox.Show("Введите фамилия участника");return; }
                //if (textBox8.Text == "") { MessageBox.Show("Введите отчество участника"); return;}
                if (textBox9.Text == "") { MessageBox.Show("Введите роль участника"); return; }
                if (pictureBox1.Image == null) { MessageBox.Show("Загрузите фото"); return; }
                con.Open();
                using (var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @name",con))
                {
                    cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                    id = Convert.ToInt16(cmd.ExecuteScalar());
                }
                if (id != 0) {con.Close(); throw new Exception("Такой фильм существует"); }
                if (textBox3.Text == "") //проверка на ссылку
                {
                    using (var cmd = new NpgsqlCommand("insert into film (\"name\", description, studio, date_release)" +
                        " values (@name, @description, @studio, @date_release)", con))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@description", richTextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@studio", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@date_release", Convert.ToInt16(textBox4.Text.Trim()));
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new NpgsqlCommand("insert into film (\"name\", description, studio, source_reference, date_release)" +
                        " values (@name, @description, @studio, @reference, @date_release)", con))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@description", richTextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@studio", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@reference", textBox3.Text.Trim());
                        cmd.Parameters.AddWithValue("@date_release", Convert.ToInt16(textBox4.Text.Trim()));
                        cmd.ExecuteNonQuery();
                    }
                }
                con.Close();
                DownloadPhotoInBD();
                DownloadGenreInBD();
                DownloadParticipantInBD();
                MessageBox.Show("Вставка прошла успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CheckTextbox()
        {
            string test = textBox1.Text.Trim();
            if (test != "")
            {
                if (!char.IsDigit(test[0]))
                {
                    if (!char.IsUpper(test[0]))
                    { throw new Exception("фильм должен начинаться с прописной буквы"); }
                }
            }
            test = textBox4.Text.Trim();
            if (test != "")
            {
                foreach (char c in test)
                {
                    if (c == ' ' || c == ',')
                        throw new Exception("В поле год выпуска не должно быть пробелов и ','");
                }
            }
            test = textBox5.Text.Trim();
            if (test != "")
            {
                if (char.IsUpper(test[0]))
                { throw new Exception("Жанр должен начинаться с маленькой буквы"); }
                foreach (char c in test)
                {
                    if (c == ' ' || c == ',')
                        throw new Exception("В поле жанр не должно быть пробелов и ','");

                }
            }
            test = textBox6.Text.Trim();
            if (test != "")
            {
                if (!char.IsUpper(test[0]))
                    throw new Exception("Имя должно начинаться с прописной буквы");
                foreach (char c in test)
                {
                    if (c == ' ' || c == ',')
                    {
                        throw new Exception("В поле имя не должно быть пробелов и запятых");
                    }
                }
            }
            test = textBox7.Text.Trim();
            if (test != "")
            {
                if (!char.IsUpper(test[0]))
                    throw new Exception("Фамилия должно начинаться с прописной буквы");
                foreach (char c in test)
                {
                    if (c == ' ' || c == ',')
                    {
                        throw new Exception("В поле фамилия не должно быть пробелов и запятых");
                    }
                }
            }
            test = textBox8.Text.Trim();
            if (test != "")
            {
                if (test.Length > 0)
                {
                    if (!char.IsUpper(test[0]))
                        throw new Exception("Отчество должно начинаться с прописной буквы");
                    foreach (char c in test)
                    {
                        if (c == ' ' || c == ',')
                            throw new Exception("В поле отчество не должно быть пробелов и запятых");
                    }
                }
            }
        }
    }
}
