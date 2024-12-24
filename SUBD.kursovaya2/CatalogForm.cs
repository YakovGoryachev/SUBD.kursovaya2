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
    public partial class CatalogForm : Form
    {
        public CatalogForm()
        {
            InitializeComponent();
        }
        public ApplicationForm appform;
        private NpgsqlConnection con;
        private string login;
        private string password;
        private void CatalogForm_Load(object sender, EventArgs e)
        {
            TopMost = true;
            WindowState = FormWindowState.Maximized;
            login = appform.login;
            password = appform.password;
            con = new NpgsqlConnection(
                $"server=localhost; Port=1234; database=DataBaseGopar; userId={login}; password={password}");

            LoadFilms();
        }

        private void LoadFilms()
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                
                using (var cmd = new NpgsqlCommand(
                    @"SELECT f.name as ""Название"", 
                  f.studio as ""Студия"", 
                  f.date_release as ""Год выпуска"",
                  COALESCE((SELECT ROUND(AVG(CAST(e.evaluation AS NUMERIC)), 1)
                    FROM evaluation e WHERE e.id_film = f.id), 0) as ""Рейтинг"",
                  string_agg(DISTINCT g.genre, ', ') as ""Жанры""
                FROM film f
                LEFT JOIN genre_in_film gf ON f.id = gf.id_film
                LEFT JOIN genre g ON gf.id_genre = g.id
                GROUP BY f.id, f.name, f.studio, f.date_release
                ORDER BY f.name", con))
                {
                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;

                    // Настройка внешнего вида
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.ReadOnly = true;
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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string filmName = dataGridView1.Rows[e.RowIndex].Cells["Название"].Value.ToString();

                // Создаем новый экземпляр FilmPanel
                if (appform.filmPanel != null)
                {
                    appform.filmPanel.Close();
                    appform.filmPanel = null;
                }

                appform.filmPanel = new FilmPanel();
                appform.filmPanel.appform = appform;
                appform.filmPanel.filmName = filmName;
                appform.filmPanel.Show();
                this.Close();
                appform.Hide();
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
            appform.Show();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.State != ConnectionState.Open)
                    con.Open();
                
                string searchCriteria = searchComboBox.SelectedItem.ToString();
                string searchText = searchTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchText) && searchCriteria != "Минимальная оценка")
                {
                    MessageBox.Show("Введите текст для поиска");
                    return;
                }

                string query = @"
                    SELECT DISTINCT f.name as ""Название"", 
                        f.studio as ""Студия"", 
                        f.date_release as ""Год выпуска"",
                        COALESCE((SELECT ROUND(AVG(CAST(e.evaluation AS NUMERIC)), 1)
                            FROM evaluation e WHERE e.id_film = f.id), 0) as ""Рейтинг"",
                        string_agg(DISTINCT g.genre, ', ') as ""Жанры""
                    FROM film f
                    LEFT JOIN genre_in_film gf ON f.id = gf.id_film
                    LEFT JOIN genre g ON gf.id_genre = g.id
                    LEFT JOIN participant_film pf ON f.id = pf.id_film
                    LEFT JOIN participant p ON pf.id_participant = p.id";

                string whereClause = "";
                NpgsqlParameter parameter = null;

                switch (searchCriteria)
                {
                    case "Название":
                        whereClause = " WHERE LOWER(f.name) LIKE LOWER(@search)";
                        parameter = new NpgsqlParameter("@search", $"%{searchText}%");
                        break;

                    case "Студия":
                        whereClause = " WHERE LOWER(f.studio) LIKE LOWER(@search)";
                        parameter = new NpgsqlParameter("@search", $"%{searchText}%");
                        break;

                    case "Артист":
                        whereClause = " WHERE LOWER(p.firstname) LIKE LOWER(@search) " +
                                     "OR LOWER(p.lastname) LIKE LOWER(@search) " +
                                     "OR LOWER(p.surname) LIKE LOWER(@search)";
                        parameter = new NpgsqlParameter("@search", $"%{searchText}%");
                        break;

                    case "Минимальная оценка":
                        if (!float.TryParse(searchText, out float rating))
                        {
                            MessageBox.Show("Введите корректное числовое значение для оценки");
                            return;
                        }
                        whereClause = @" WHERE (SELECT ROUND(AVG(CAST(e.evaluation AS NUMERIC)), 1)
                                        FROM evaluation e WHERE e.id_film = f.id) >= @rating";
                        parameter = new NpgsqlParameter("@rating", rating);
                        break;
                }

                query += whereClause + " GROUP BY f.id, f.name, f.studio, f.date_release ORDER BY f.name";

                using (var cmd = new NpgsqlCommand(query, con))
                {
                    if (parameter != null)
                        cmd.Parameters.Add(parameter);

                    var adapter = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}");
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            searchTextBox.Clear();
            searchComboBox.SelectedIndex = 0;
            LoadFilms();
        }

        // Добавим обработку клавиши Enter в поле поиска
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && searchTextBox.Focused)
            {
                searchButton_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
