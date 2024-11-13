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
        RegistrationForm RegForm;

        private void Form1_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void EntryBut_Click(object sender, EventArgs e)
        {

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
