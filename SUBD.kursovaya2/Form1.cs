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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        NpgsqlConnection conn = new NpgsqlConnection(
            "server=localhost; Port=1234; database=DataBaseGopar; userId=gopar; password=1234");
        public Form2 fm;
        public static int id;
        private void PrintTable()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataMember = null;
            DataSet ds = new DataSet();
            conn.Open();
            ds.Clear();
            NpgsqlCommand cmd = new NpgsqlCommand("select * from costTable", conn);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            da.Fill(ds, "costTable");
            dataGridView1.DataSource = ds;
            dataGridView1.DataMember = "costTable";
            conn.Close();
        }
        private void button1_Click(object sender, EventArgs e) //Вывести
        {
            try
            {
                PrintTable();
                id = dataGridView1.RowCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e) //добавить
        {
            try
            {
                if (fm == null)
                {
                    fm = new Form2();
                    fm.Show();
                }
                else if (fm.IsDisposed)
                {
                    fm = new Form2();
                    fm.Show();
                }
                fm.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e) //удалить
        {
            try
            {
                PrintTable();
                conn.Open();
                id = dataGridView1.RowCount - 1;
                NpgsqlCommand cmd = new NpgsqlCommand($"delete from costTable where id = {id}", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                PrintTable();
                id = dataGridView1.RowCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
