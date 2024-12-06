using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUBD.kursovaya2
{
    public partial class ApplicationForm : Form
    {
        public ApplicationForm()
        {
            InitializeComponent();
        }
        public string login;
        public string password;
        public Form1 Form1;
        public AdminPanel AdminPanel1;
        NpgsqlConnection con;

        private void ApplicationForm_Load(object sender, EventArgs e)
        {
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            label1.Text = login;
            con = new NpgsqlConnection(
                $"server=localhost; Port=1234; database=DataBaseGopar; userId={login}; password={password}");
            CheckAdmin();
            DownloadingPhotos(); //допилить
        }
        private void DownloadingPhotos()
        {
            int countFilms;
            using (var cmd = new NpgsqlCommand("" +
                "select count(*) from film",con))
            {
                con.Open();
                countFilms = Convert.ToInt16(cmd.ExecuteScalar());
                con.Close();
            }
            PictureBox[] masPict = new PictureBox[] {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6
            };
            Label[] masLabels = new Label[] {label2, label3, label4, label5, label6, label7};
            DataSet ds = new DataSet();
            using (var da = new NpgsqlDataAdapter(
                    "select film.name, photo.photo from film inner join photo" +
                    " on film.id = photo.id_film", con))
            {
                con.Open();
                da.Fill(ds, "dataFilm");
            }
            //label2.Text = ds.Tables[0].Rows[0][1].ToString();
            Random rnd = new Random();
            int iindex = 6;
            if (countFilms > 6)
                iindex = 6;
            else iindex = countFilms;
            int[] masIndex = new int[6];
            List<int> mas = new List<int>();
            for (int i = 0; i < countFilms; i++)
            {
                mas.Add(i);
            }
            for (int i = 0; i < iindex; i++)
            {
                int indRand = rnd.Next(0,mas.Count);
                int ind = mas[indRand];
                mas.Remove(indRand);
                masLabels[i].Text = ds.Tables[0].Rows[ind][0].ToString();
                Bitmap image;
                using (var ms = new MemoryStream((byte[])ds.Tables[0].Rows[ind][1]))
                {
                    image = new Bitmap(ms);
                }
                masPict[i].Image = image;
                masPict[i].SizeMode = PictureBoxSizeMode.StretchImage;
                masIndex[i] = ind;
            }
        }
        private void CheckAdmin()
        {
            con.Open();
            int type = 0;
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "select id_type_user from \"user\" where login = @login";
                cmd.Parameters.AddWithValue("@login",login);
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        type = reader.GetInt16(0);
                    }
                }
            }
            con.Close();
            if (type == 2)
            {
                button2.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e) //вывод админ формы
        {
            if (AdminPanel1 == null)
            {
                AdminPanel1 = new AdminPanel();
                AdminPanel1.AppForm = this;
                AdminPanel1.Show();
                Hide();
            }
            else
            {
                AdminPanel1.AppForm = this;
                AdminPanel1.Show();
                Hide();
            }
        }
    }
}
