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
                textBox1.Clear();
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
            con.Open();
            using (var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @name",con))
            {
                cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                idFilm = Convert.ToInt16(cmd.ExecuteScalar());
            }
            using (var cmd = new NpgsqlCommand("insert into genre (id_film, genre) " +
                "values (@id_film, @genre)",con))
            {
                cmd.Parameters.AddWithValue("@id_film", idFilm);
                cmd.Parameters.AddWithValue("@genre", textBox5.Text.Trim());
            }
            con.Close();
        }
        private void DownloadParticipantInBD()
        {
            if (textBox1.Text == "") { MessageBox.Show("Введите название фильма"); return; }
            if (textBox6.Text == "") { MessageBox.Show("Введите имя участника"); return; }
            if (textBox7.Text == "") { MessageBox.Show("Введите фамилия участника"); return; }
            if (textBox8.Text == "") { MessageBox.Show("Введите отчество участника"); return; }
            if (textBox9.Text == "") { MessageBox.Show("Введите роль участника"); return; }
            con.Open();
            using (var transaction =  con.BeginTransaction())
            {
                int idRole = 0;
                int idFilm = 0;
                int idParticipant = 0;
                using (var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @name",con))
                {
                    cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                    idFilm = Convert.ToInt16(cmd.ExecuteScalar());
                }
                using (var cmd = new NpgsqlCommand("insert into participant (firstname, lastname, surname) " +
                "values (@firstname, @lastname, @surname)", con))
                {
                    cmd.Parameters.AddWithValue("@firstname", textBox6.Text.Trim());
                    cmd.Parameters.AddWithValue("@lastname", textBox7.Text.Trim());
                    cmd.Parameters.AddWithValue("@surname", textBox8.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new NpgsqlCommand("select \"id\" from participant " +
                    "where firstname = @firstname and lastname = @lastname and surname = @surname",con))
                {
                    cmd.Parameters.AddWithValue("@firstname", textBox6.Text.Trim());
                    cmd.Parameters.AddWithValue("@lastname", textBox7.Text.Trim());
                    cmd.Parameters.AddWithValue("@surname", textBox8.Text.Trim());
                    idParticipant = Convert.ToInt16(cmd.ExecuteScalar());
                }
                using (var cmd = new NpgsqlCommand("select \"id\" from roles where \"role\" = @role"))
                {
                    cmd.Parameters.AddWithValue("@role", textBox9.Text.Trim());
                    idRole = Convert.ToInt16(cmd.ExecuteScalar());
                }
                if (idRole != 0)
                {
                    using (var cmd = new NpgsqlCommand("insert into participant_film (id_film, id_participant, id_role) " +
                        "values (@id_film, @id_participant, @id_role)",con))
                    {
                        cmd.Parameters.AddWithValue("@id_film", idFilm);
                        cmd.Parameters.AddWithValue("@id_participant", idParticipant);
                        cmd.Parameters.AddWithValue("@id_role", idRole);
                        cmd.ExecuteNonQuery();
                    }
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
            }
            con.Close();
        }
        private void DownFilmBut_Click(object sender, EventArgs e)
        {
            try
            {
                int id = 0;
                if (textBox1.Text == "") { MessageBox.Show("Введите название фильма"); return; }
                if (richTextBox1.Text == "") { MessageBox.Show("Введите описание фильма"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Введите название студии"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Введите год выпуска фильма"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Введите название жанра");return; }
                if (textBox6.Text == "") { MessageBox.Show("Введите имя участника");return; }
                if (textBox7.Text == "") { MessageBox.Show("Введите фамилия участника");return; }
                if (textBox8.Text == "") { MessageBox.Show("Введите отчество участника"); return;}
                if (textBox9.Text == "") { MessageBox.Show("Введите роль участника"); return; }
                con.Open();
                using (var cmd = new NpgsqlCommand("select \"id\" from film where \"name\" = @name",con))
                {
                    cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                    id = Convert.ToInt16(cmd.ExecuteScalar());
                }
                if (id == 0) { MessageBox.Show("В таблице уже есть фильм"); return; }
                if (textBox3.Text == "")
                {
                    using (var cmd = new NpgsqlCommand("insert into film (\"name\", description, studio, date_release)" +
                        " values (@name, @description, @studio, @date_release)", con))
                    {
                        cmd.Parameters.AddWithValue("@name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@description", richTextBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@studio", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@date_release", textBox4.Text.Trim());
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
                        cmd.Parameters.AddWithValue("@date_release", textBox4.Text.Trim());
                        cmd.ExecuteNonQuery();
                    }
                }
                con.Close();
                DownloadPhotoInBD();
                DownloadGenreInBD();
                DownloadParticipantInBD();
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                MessageBox.Show("Вставка прошла успешно");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
