using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUBD.kursovaya2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool dataExist;
        RegistrationForm RegForm;
        ApplicationForm AppForm;
        string dataAutFilePath = "dataFilePath.txt";

        private void Form1_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            LoadFromFile();
        }
        private void LoadFromFile()
        {
            if (File.Exists(dataAutFilePath))
            {
                string[] data = File.ReadAllLines(dataAutFilePath);
                if (data.Length == 2)
                {
                    textBox1.Text = data[0];
                    textBox2.Text = data[1];
                    dataExist = true;
                }
            }
        }
        private void EntryBut_Click(object sender, EventArgs e)
        {
            try
            {
                string verifLogin = "";
                string verifPass = "";
                int role = 1;
                using (NpgsqlConnection con = new NpgsqlConnection(
                    "server=localhost; Port=1234; database=DataBaseGopar; userId=postgres; password=admin"))
                {
                    string login = textBox1.Text;
                    string password = textBox2.Text;
                    if (GuardSqlInjection(login, "where") || GuardSqlInjection(password, "where")) { MessageBox.Show("Неккоректные данные"); return; }
                    if (GuardSqlInjection(login, "WHERE") || GuardSqlInjection(password, "WHERE")) { MessageBox.Show("Неккоректные данные"); return; }
                    if (login == "" || password == "") { MessageBox.Show("Введите данные"); return; }
                    string encryptedPassword = Encrypt(password);
                    con.Open();
                    string storedPassword = "";
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select id from \"user\" where login = @login", con))
                    {
                        cmd.Parameters.AddWithValue("login", login);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read()) { MessageBox.Show("Данного пользователя не существует"); return; }
                        }
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand("select password from \"user\" where login = @login", con))
                    {
                        cmd.Parameters.AddWithValue("login", login);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                storedPassword = reader.GetString(0);
                            }
                        }
                    }
                    using (NpgsqlCommand cmd = new NpgsqlCommand(
                        "select id_type_user from \"user\" where login = @login",con))
                    {
                        cmd.Parameters.AddWithValue("login", login);
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                role = Convert.ToInt32(reader.GetValue(0));
                            }
                            else { MessageBox.Show("Не удается получить роль"); }
                        }
                    }
                    con.Close();
                    if (storedPassword != encryptedPassword)
                    { MessageBox.Show("Неправильный пароль"); return; }
                    verifLogin = login;
                    verifPass = encryptedPassword;
                }
                Entry(verifLogin, verifPass, role);
                SaveDataAut(textBox1.Text, textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SaveDataAut(string login, string pass)
        {
            if (!dataExist)
            {
                using (StreamWriter writer = new StreamWriter(dataAutFilePath))
                {
                    writer.WriteLine(login);
                    writer.WriteLine(pass);
                }
            }
        }
        private void Entry(string login, string pass, int role)
        {
            try
            {
                AppForm = new ApplicationForm();
                AppForm.Form1 = this;
                AppForm.login = login;
                AppForm.password = pass;
                Hide();
                AppForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string Encrypt(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        private bool GuardSqlInjection(string main, string subSeq)
        {
            bool found = false;
            for (int i = 0; i < main.Length; i++)
            {
                if (main[i] == subSeq[0])
                {
                    int k = 0;
                    for (int j = 0; j < subSeq.Length; j++)
                    {
                        if (main[i + j] != subSeq[j])
                        {
                            break;
                        }
                        else { k++; }
                    }
                    if (k == subSeq.Length)
                    {
                        found = true; break;
                    }
                }
            }
            return found;
        }
        private void RegistrationBut_Click(object sender, EventArgs e)
        {
            try
            {
                if (RegForm == null)
                {
                    RegForm = new RegistrationForm();
                    RegForm.AutForm = this;
                    RegForm.Show();
                    Hide();
                }
                else if (RegForm.IsDisposed)
                {
                    RegForm = new RegistrationForm();
                    RegForm.AutForm = this;
                    RegForm.Show();
                    Hide();
                }
                RegForm.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExitBut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
