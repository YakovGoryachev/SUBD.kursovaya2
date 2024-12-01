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
            if (textBox1.Text == "") { MessageBox.Show("Выберете id фильма"); return; }
            if (pictureBox1.Image == null) { MessageBox.Show("Выберете фото"); return; }
            con.Open();
            //Image image = pictureBox1.Image;
            //using (var ms = new MemoryStream())
            //{
            //    image.Save(ms, ImageFormat.Jpeg);
            //    imageBytes = ms.ToArray();
            //}
            ImageConverter convert = new ImageConverter();

            using (var cmd = new NpgsqlCommand("insert into photo (id_film, photo)" +
                "values (@id_film, @photo)", con))
            {
                cmd.Parameters.AddWithValue("@id_film", Convert.ToInt16(textBox1.Text));
                cmd.Parameters.AddWithValue("@photo", imageBytes);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Вставка фото прошла успешно");
            }
            con.Close();
        }
    }
}
