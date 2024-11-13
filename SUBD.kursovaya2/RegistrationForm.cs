﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SUBD.kursovaya2
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();
        }
        public Form1 AutForm;
        private void RegistrationForm_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void RegistrationBut_Click(object sender, EventArgs e)
        {
            NpgsqlConnection con = new NpgsqlConnection(
                "server=localhost; Port=1234; database=DataBaseGopar; userId=postgres; password=admin");
            RegistrationInDatabase(con);
        }
        private void RegistrationInDatabase(NpgsqlConnection con)
        {
            if (textBox1.Text == "" || textBox2.Text == "") { MessageBox.Show("Заполните все поля"); return; }
            string login = textBox1.Text;
            string password = textBox2.Text;
            if (SearchUser(con, login, password)) { MessageBox.Show("Такой пользователь уже существует"); return; }
            if (GuardSqlInjection(login, "where") || GuardSqlInjection(password, "where")) { MessageBox.Show("Недопустимое значение"); return; }
            if (GuardSqlInjection(login, "WHERE") || GuardSqlInjection(password, "WHERE")) { MessageBox.Show("Недопустимое значение"); return; }
            string encryptedPassword = Encrypt(password);
        }
        private bool SearchUser(NpgsqlConnection con, string login, string password)
        {
            bool found = false;
            con.Open();
            NpgsqlCommand cmd = new NpgsqlCommand($"select id from \"user\" where login = @login", con);
            cmd.Parameters.AddWithValue("@login", login);
            if (cmd.ExecuteNonQuery() > -1) //если сущ
                found = true;
            con.Close();
            return found;
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
        private string Encrypt(string input)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        private void BackBut_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
                AutForm.Show();
            }
            catch (Exception ex)
            {

            }
        }

        private void ExitBut_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}