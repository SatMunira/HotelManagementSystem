using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace HotelManagementSystem
{
    public partial class ManagerForm : Form
    {
        private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=HotelManagementDB";

        private DataGridView dgvGuests;
        private Button btnCheckIn;

        public ManagerForm()
        {
            InitializeComponent();
            InitializeComponents();
            LoadGuests();
        }

        private void InitializeComponents()
        {
            dgvGuests = new DataGridView();
            btnCheckIn = new Button();

            dgvGuests.Location = new System.Drawing.Point(10, 10);
            dgvGuests.Size = new System.Drawing.Size(400, 200);

            btnCheckIn.Location = new System.Drawing.Point(10, 220);
            btnCheckIn.Size = new System.Drawing.Size(150, 30);
            btnCheckIn.Text = "Заселить гостя";
            btnCheckIn.Click += new EventHandler(btnCheckIn_Click);

            this.Controls.Add(dgvGuests);
            this.Controls.Add(btnCheckIn);
        }

        private void LoadGuests()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM guests", connection))
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dgvGuests.DataSource = dataTable;
                }
            }
        }
        private void InitializeComponent()
        {
            // Этот метод может быть пустым, если вы не используете дизайнер формы
        }

        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            CheckInForm checkInForm = new CheckInForm();
            checkInForm.ShowDialog();

            // После заселения обновляем список гостей
            LoadGuests();
        }
    }
}
