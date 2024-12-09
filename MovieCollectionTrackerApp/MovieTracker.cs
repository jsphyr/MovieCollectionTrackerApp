using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MovieCollectionTrackerApp
{
    public partial class MovieTracker : Form
    {
        private Database db = new Database();
        public MovieTracker()
        {
            InitializeComponent();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text;
            string genre = txtGenre.Text;
            int releaseYear = int.Parse(txtReleaseYear.Text);
            string director = txtDirector.Text;

            string query = $"INSERT INTO Movies (Title, Genre, ReleaseYear, Director)" +
                           $" VALUES ('{title}', '{genre}', {releaseYear}, '{director}')";

            db.ExecuteNonQuery(query);

            MessageBox.Show("Movie added!");
            LoadMovies();
        }
        private void btnViewAll_Click(object sender, EventArgs e)
        {
            LoadMovies();
        }
        private void LoadMovies()
        {
            string query = "SELECT * FROM Movies";
            DataTable movies = db.ExecuteQuery(query);
            dgvMovies.DataSource = movies;

            MovieCount();
        }       
        private void MovieCount()
        {
            string query = "SELECT * FROM Movies";
            DataTable movies = db.ExecuteQuery(query);
            int movieCount = movies.Rows.Count;
            lblMovieCount.Text = movieCount.ToString();
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvMovies.SelectedRows.Count > 0)
            {
                int movieId = (int)dgvMovies.SelectedRows[0].Cells["MovieID"].Value;
                string title = txtTitle.Text;
                string genre = txtGenre.Text;
                int releaseYear = Convert.ToInt32(txtReleaseYear.Text);
                string director = txtDirector.Text;

                string query = $"UPDATE Movies SET Title='{title}', Genre='{genre}', ReleaseYear={releaseYear}, Director='{director}' WHERE MovieId={movieId}";

                db.ExecuteNonQuery(query);

                MessageBox.Show("Movie Updated!");
                LoadMovies();
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvMovies.SelectedRows.Count > 0)
            {
                int movieId = (int)dgvMovies.SelectedRows[0].Cells["MovieID"].Value;

                var confirm = MessageBox.Show("Are you sure to delete this movie?", "Confirm Delete", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    string query = $"DELETE FROM Movies WHERE MovieID = {movieId}";

                    db.ExecuteNonQuery(query);

                    MessageBox.Show("Movie deleted!");
                    LoadMovies();
                }
                else
                {
                    MessageBox.Show("Please select a movie to delete");
                }
            }
        }
        private void MovieTracker_Load(object sender, EventArgs e)
        {
            LoadMovies();
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text;

            string query = $"SELECT * FROM Movies " +
                           $"WHERE Title LIKE '%{keyword}%'" +
                           $" OR Genre LIKE '%{keyword}%'" +
                           $" OR ReleaseYear LIKE '%{keyword}%'" +
                           $" OR Director LIKE '%{keyword}%'"; 
            try
            {
                DataTable searchResults = db.ExecuteQuery(query);
                dgvMovies.DataSource = searchResults;

                txtSearch.Text = "";
                MovieCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void dgvMovies_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMovies.Rows[e.RowIndex];

                txtTitle.Text = row.Cells["Title"].Value.ToString();
                txtGenre.Text = row.Cells["Genre"].Value.ToString();
                txtReleaseYear.Text = row.Cells["ReleaseYear"].Value.ToString();
                txtDirector.Text = row.Cells["Director"].Value.ToString();
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtTitle.Text = "";
            txtGenre.Text = "";
            txtReleaseYear.Text = "";
            txtDirector.Text = "";
        }
    }
    public class Database
    {
        private string connString = "server=localhost;database=MovieDB;uid=root;pwd=BKjonpCFvhw1QnPe;";
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }
        public DataTable ExecuteQuery(string query)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                DataTable dt = new DataTable();

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd)) 
                {
                    adapter.Fill(dt);
                }

                return dt;
            }
        }
        public void ExecuteNonQuery(string query)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }
    }
}